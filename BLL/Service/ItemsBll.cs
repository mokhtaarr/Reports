using Azure;
using BLL.Authentication;
using BLL.DTO;
using DAL.Context;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using Static.Helper;
using Static.VM;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Service
{
    public class ItemsBll
    {
        private readonly IRepository<MsItemCard> _repository;
        private readonly IRepository<MsItemPartition> _msItemPartitionRepository;
        private readonly IRepository<MsItemImages> _msItemImagesRepository;
        //private readonly IRepository<MsItemVendors> _itemVendors;
        //private readonly IRepository<MsVendor> _vendor;
        //private readonly IRepository<MsItemImages> _ItemImages;
        private SmartERPStandardContext db;

        public ItemsBll(IRepository<MsItemCard> repository , SmartERPStandardContext _db, 
            IRepository<MsItemPartition> msItemPartitionRepository,IRepository<MsItemImages> msItemImagesRepository)
        {
            _repository = repository;
            db = _db;
            _msItemPartitionRepository = msItemPartitionRepository;
            _msItemImagesRepository = msItemImagesRepository;
        }

        public ResultDTO GetAllItems(int? Page_No, int? Size_No)
        {
            ResultDTO resultDTO = new ResultDTO() { Status = false, Message = "خطا بالبيانات" };

            try
            {
                int Size_Of_Page = Size_No ?? 9;
                int No_Of_Page = Page_No ?? 1;


                var data = (from itemCard in db.MsItemCard
                            join itemImages in db.MsItemImages
                            on itemCard.ItemCardId equals itemImages.ItemCardId into itemImagesGroup
                            from img in itemImagesGroup.DefaultIfEmpty() // هذا هو ال left join
                            select new VendorVM
                            {
                                ItemCardId = itemCard.ItemCardId,
                                itemcode = itemCard.ItemCode,
                                itemname = itemCard.ItemDescA,
                                Qty = itemCard.QtyInBox,
                                Price = itemCard.FirstPrice,
                                Price2 = itemCard.SecandPrice,
                                Price3 = itemCard.ThirdPrice,
                                CoastAVG = itemCard.CoastAverage,
                                TotalQtyInNotebook = db.MsItemPartition.Where(item => item.ItemCardId == itemCard.ItemCardId)
                                               .Sum(item => item.QtyInNotebook),
                                image = img != null ? img.ImgPath : null // يمكنك تضمين الصورة هنا
                            })
                            .Skip((No_Of_Page - 1) * Size_Of_Page)
                            .Take(Size_Of_Page)
                            .ToList();

                resultDTO.Page_No = No_Of_Page;
                resultDTO.Size_No = Size_Of_Page;
                resultDTO.data = data;
                resultDTO.Status = true;
                resultDTO.Message = "تم عرض البيانات بنجاح";

                return resultDTO;
            }
            catch (Exception ex)
            {
                // يجب التعامل مع الاستثناءات هنا، يمكنك إعادة رمية الاستثناء أو التسجيل والعمليات اللازمة.
                throw ex;
            }
        }

        //public ResultDTO GetAllItems(int? Page_No, int? Size_No)
        //{
        //    ResultDTO resultDTO = new ResultDTO() { Status = false, Message = "خطا بالبيانات" };

        //    try
        //    {
        //        int Size_Of_Page = Size_No ?? 9;
        //        int No_Of_Page = Page_No ?? 1;

        //        var data = (from itemCard in db.MsItemCard
        //                    join itemPartition in db.MsItemPartition
        //                    on itemCard.ItemCardId equals itemPartition.ItemCardId
        //                    join partition in db.MsPartition 
        //                    on itemPartition.StorePartId equals partition.StorePartId
        //                    join stores in db.MsStores
        //                    on itemPartition.StoreId equals stores.StoreId
        //                    join itemImages in db.MsItemImages
        //                    on itemCard.ItemCardId equals itemImages.ItemCardId into itemImagesGroup
        //                    from img in itemImagesGroup.DefaultIfEmpty() // هذا هو ال left join
        //                    select new VendorVM
        //                    {
        //                        ItemCardId = itemCard.ItemCardId,
        //                        itemcode = itemCard.ItemCode,
        //                        itemname = itemCard.ItemDescA,
        //                        Qty = itemCard.QtyInBox,
        //                        Price = itemCard.FirstPrice,
        //                        Price2 = itemCard.SecandPrice,
        //                        Price3 = itemCard.ThirdPrice,
        //                        CoastAVG = itemCard.CoastAverage,
        //                        StoreDescA = stores.StoreDescA,
        //                        PartDescA = partition.PartDescA,
        //                        QtyInNotebook = itemPartition.QtyInNotebook,
        //                        TotalQtyInNotebook = db.MsItemPartition.Where(item => item.ItemCardId == itemCard.ItemCardId)
        //                                       .Sum(item => item.QtyInNotebook),
        //                        image = img != null ? img.ImgPath : null // يمكنك تضمين الصورة هنا
        //                    })
        //                    .Skip((No_Of_Page - 1) * Size_Of_Page)
        //                    .Take(Size_Of_Page)
        //                    .ToList();

        //        resultDTO.Page_No = No_Of_Page;
        //        resultDTO.Size_No = Size_Of_Page;
        //        resultDTO.data = data;
        //        resultDTO.Status = true;
        //        resultDTO.Message = "تم عرض البيانات بنجاح";

        //        return resultDTO;
        //    }
        //    catch (Exception ex)
        //    {
        //        // يجب التعامل مع الاستثناءات هنا، يمكنك إعادة رمية الاستثناء أو التسجيل والعمليات اللازمة.
        //        throw ex;
        //    }
        //}


        //public ResultDTO GetAllItems(int? Page_No, int? Size_No)
        //{
        //    ResultDTO resultDTO = new ResultDTO() { Status = false, Message = "خطا بالبيانات" };

        //    try
        //    {
        //        int Size_Of_Page = Size_No ?? 9;
        //        int No_Of_Page = Page_No ?? 1;

        //        var data = (from itemCard in db.MsItemCard
        //                    join itemPartition in db.MsItemPartition
        //                    on itemCard.ItemCardId equals itemPartition.ItemCardId
        //                    select new VendorVM
        //                    {
        //                        ItemCardId = itemCard.ItemCardId,
        //                        itemcode = itemCard.ItemCode,
        //                        itemname = itemCard.ItemDescA,
        //                        Qty = itemCard.QtyInBox,
        //                        Price = itemCard.FirstPrice,
        //                        Price2 = itemCard.SecandPrice,
        //                        Price3 = itemCard.ThirdPrice,
        //                        CoastAVG = itemCard.CoastAverage,
        //                        QtyInNotebook = db.MsItemPartition
        //                                        .Where(item => item.ItemCardId == itemCard.ItemCardId)
        //                                        .Sum(item => item.QtyInNotebook)
        //                    })
        //                    .Skip((No_Of_Page - 1) * Size_Of_Page)
        //                    .Take(Size_Of_Page)
        //                    .ToList();

        //        resultDTO.Page_No = No_Of_Page;
        //        resultDTO.Size_No = Size_Of_Page;
        //        resultDTO.data = data;
        //        resultDTO.Status = true;
        //        resultDTO.Message = "تم عرض البيانات بنجاح";

        //        return resultDTO;
        //    }
        //    catch (Exception ex)
        //    {
        //        // يجب التعامل مع الاستثناءات هنا، يمكنك إعادة رمية الاستثناء أو التسجيل والعمليات اللازمة.
        //        throw ex;
        //    }
        //}



        //public decimal GetPartitionCount(int itemCardId)
        //{
        //    var totalItem = db.MsItemPartition
        //        .Where(item => item.ItemCardId == itemCardId)
        //        .Sum(item => item.QtyInNotebook);

        //    return (decimal)totalItem;
        //}
        //public ResultDTO GetAllItems(int? Page_No, int? Size_No)
        //{
        //    ResultDTO resultDTO = new ResultDTO() { Status = false, Message = "خطا بالبيانات" };

        //    try
        //    {
        //        int Size_Of_Page = Size_No ?? 9;
        //        int No_Of_Page = Page_No ?? 1;


        //        var data = (from itemCard in db.MsItemCard
        //                    join itemPartition in db.MsItemPartition 
        //                    on itemCard.ItemCardId equals itemPartition.ItemCardId 
        //                    select new VendorVM
        //                    {
        //                        ItemCardId = itemCard.ItemCardId,
        //                        itemcode = itemCard.ItemCode,
        //                        itemname = itemCard.ItemDescA,
        //                        Qty = itemCard.QtyInBox,
        //                        Price = itemCard.FirstPrice,
        //                        Price2 = itemCard.SecandPrice,
        //                        Price3 = itemCard.ThirdPrice,
        //                        CoastAVG = itemCard.CoastAverage,
        //                        QtyInNotebook = GetPartitionCount(itemCard.ItemCardId),
        //                    })
        //                    .Skip((No_Of_Page - 1) * Size_Of_Page)
        //                    .Take(Size_Of_Page)
        //                    .ToList();

        //        resultDTO.Page_No = No_Of_Page;
        //        resultDTO.Size_No = Size_Of_Page;
        //        resultDTO.data = data;
        //        resultDTO.Status = true;
        //        resultDTO.Message = "تم عرض البيانات بنجاح";

        //        return resultDTO;
        //    }
        //    catch (Exception ex)
        //    {
        //        return resultDTO;
        //    }
        //}

        //public ResultDTO GetAllItems(int? Page_No, int? Size_No)
        //{
        //    ResultDTO resultDTO = new ResultDTO() { Status = false, Message = "خطا بالبيانات" };
        //    try
        //    {
        //        int Size_Of_Page = (Size_No ?? 9);
        //        int No_Of_Page = (Page_No ?? 1);

        //        // استعلام لجلب البيانات من _msItemPartitionRepository
        //        var partitionData = _msItemPartitionRepository.GetAll();

        //        List<VendorVM> data = _repository.GetAll()
        //            .GroupJoin(
        //                _msItemImagesRepository.GetAll(),
        //                p => p.ItemCardId,
        //                img => img.ItemCardId,
        //                (p, images) => new { p, images })
        //            .SelectMany(
        //                x => x.images.DefaultIfEmpty(),
        //                (combined, img) => new
        //                {
        //                    combined.p,
        //                    combined.images,
        //                    img
        //                })
        //            .Join(
        //                partitionData, // استخدام البيانات من _msItemPartitionRepository
        //                combined => combined.p.ItemCardId,
        //                mp => mp.ItemCardId,
        //                (combined, mp) => new VendorVM
        //                {
        //                    ItemCardId = combined.p.ItemCardId,
        //                    itemcode = combined.p.ItemCode,
        //                    itemname = combined.p.ItemDescA,
        //                    Qty = combined.p.QtyInBox,
        //                    Price = combined.p.FirstPrice,
        //                    Price2 = combined.p.SecandPrice,
        //                    Price3 = combined.p.ThirdPrice,
        //                    CoastAVG = combined.p.CoastAverage,
        //                    QtyInNotebook = mp.QtyInNotebook, // هنا نأخذ الـ QtyInNotebook من جدول msitempartition
        //                    image = combined.img != null ? combined.img.ImgPath : null,
        //                })
        //            .Skip((No_Of_Page - 1) * Size_Of_Page)
        //            .Take(Size_Of_Page)
        //            .ToList();

        //        resultDTO.Page_No = No_Of_Page;
        //        resultDTO.Size_No = Size_Of_Page;
        //        resultDTO.data = data;
        //        resultDTO.Status = true;
        //        resultDTO.Message = "تم عرض البيانات بنجاح";
        //        return resultDTO;
        //    }
        //    catch (Exception ex)
        //    {
        //        return resultDTO;
        //    }
        //}

        //public ResultDTO GetAllItems(int? Page_No, int? Size_No)
        //{
        //    ResultDTO resultDTO = new ResultDTO() { Status = false, Message = "خطا بالبيانات" };
        //    try
        //    {
        //        int Size_Of_Page = (Size_No ?? 9);
        //        int No_Of_Page = (Page_No ?? 1);

        //        List<VendorVM> data = _repository.GetAll()
        //            .GroupJoin(
        //                _msItemImagesRepository.GetAll(),
        //                p => p.ItemCardId,
        //                img => img.ItemCardId,
        //                (p, images) => new { p, images })
        //            .SelectMany(
        //                x => x.images.DefaultIfEmpty(),
        //                (combined, img) => new
        //                {
        //                    combined.p,
        //                    combined.images,
        //                    img
        //                })
        //            .Join(
        //                _msItemPartitionRepository.GetAll(),
        //                combined => combined.p.ItemCardId,
        //                mp => mp.ItemCardId,
        //                (combined, mp) => new VendorVM
        //                {
        //                    ItemCardId = combined.p.ItemCardId,
        //                    itemcode = combined.p.ItemCode,
        //                    itemname = combined.p.ItemDescA,
        //                    Qty = combined.p.QtyInBox,
        //                    Price = combined.p.FirstPrice,
        //                    Price2 = combined.p.SecandPrice,
        //                    Price3 = combined.p.ThirdPrice,
        //                    CoastAVG = combined.p.CoastAverage,
        //                    QtyInNotebook = mp.QtyInNotebook, // هنا نأخذ الـ QtyInNotebook من جدول msitempartition
        //                    image = combined.img != null ? combined.img.ImgPath : null,
        //                })
        //            .Skip((No_Of_Page - 1) * Size_Of_Page)
        //            .Take(Size_Of_Page)
        //            .ToList();

        //        resultDTO.Page_No = No_Of_Page;
        //        resultDTO.Size_No = Size_Of_Page;
        //        resultDTO.data = data;
        //        resultDTO.Status = true;
        //        resultDTO.Message = "تم عرض البيانات بنجاح";
        //        return resultDTO;




        //    }
        //    catch (Exception ex)
        //    {
        //        return resultDTO;
        //    }
        //}


        //public ResultDTO GetAllItems(int? Page_No, int? Size_No)
        //{
        //    ResultDTO resultDTO = new ResultDTO() { Status = false, Message = "خطا بالبيانات" };
        //    try
        //    {
        //        int Size_Of_Page = (Size_No ?? 9);
        //        int No_Of_Page = (Page_No ?? 1);

        //        List<VendorVM> data = _repository.GetAll().ToList().Select(p => new VendorVM
        //        {
        //            ItemCardId = p.ItemCardId,
        //            itemcode = p.ItemCode,
        //            itemname = p.ItemDescA,
        //            Qty = p.QtyInBox,
        //            Price = p.FirstPrice,
        //            Price2 = p.SecandPrice,
        //            Price3 = p.ThirdPrice,
        //            CoastAVG = p.CoastAverage,
        //            ////Vendors = GetVendors(p.ItemCardId),
        //            image = GetImages(p.ItemCardId),
        //        }).Skip((No_Of_Page - 1) * Size_Of_Page).Take(Size_Of_Page).ToList();

        //        resultDTO.Page_No = No_Of_Page;
        //        resultDTO.Size_No = Size_Of_Page;
        //        resultDTO.data = data;
        //        resultDTO.Status = true;
        //        resultDTO.Message = "تم عرض البيانات بنجاح";
        //        return resultDTO;
        //    }
        //    catch(Exception ex)
        //    {
        //        return resultDTO;
        //    }
        //}

        public object GetVendors(int ItemCardId)
        {
            using (db = new SmartERPStandardContext())
            {
                var vendors = (from ItemVendors in db.MsItemVendors
                               join Vendor in db.MsVendor on ItemVendors.VendorId equals Vendor.VendorId
                               join ItemCard in db.MsItemCard on ItemVendors.ItemCardId equals ItemCard.ItemCardId
                               where ItemVendors.ItemCardId == ItemCardId
                               select new { VendorCode = Vendor.VendorCode, VendorDescA = Vendor.VendorDescA }).ToList();
                return vendors;
            }
        }

        public string GetImages(int ItemCardId)
        {
            using (db = new SmartERPStandardContext())
            {
                MsItemImages Item = db.MsItemImages.FirstOrDefault(v => v.ItemCardId == ItemCardId);
                string image = string.Empty;
                if (Item != null)
                    image = Item.Image != null ? Item.Image.ToString().Replace('"', ' ').Trim() : null;
                return image;
            }
        }

        public ResultDTO barcode(string qr)
        {
            ResultDTO resultDTO = new ResultDTO() { Status = false, Message = "خطأ في البيانات" };

            try
            {
                VendorVM data = (from itemCard in db.MsItemCard
                                 join itemPartition in db.MsItemPartition
                                 on itemCard.ItemCardId equals itemPartition.ItemCardId
                                 join partition in db.MsPartition
                                 on itemPartition.StorePartId equals partition.StorePartId
                                 join stores in db.MsStores
                                 on itemPartition.StoreId equals stores.StoreId
                                 join itemImages in db.MsItemImages
                                 on itemCard.ItemCardId equals itemImages.ItemCardId into itemImagesGroup
                                 from img in itemImagesGroup.DefaultIfEmpty() // هذا هو ال left join
                                 where itemCard.ItemCode == qr
                                 select new VendorVM
                                 {
                                     ItemCardId = itemCard.ItemCardId,
                                     itemcode = itemCard.ItemCode,
                                     itemname = itemCard.ItemDescA,
                                     Qty = itemCard.QtyInBox,
                                     Price = itemCard.FirstPrice,
                                     Price2 = itemCard.SecandPrice,
                                     Price3 = itemCard.ThirdPrice,
                                     CoastAVG = itemCard.CoastAverage,
                                     StoreDescA = stores.StoreDescA,
                                     PartDescA = partition.PartDescA,
                                     QtyInNotebook = itemPartition.QtyInNotebook,
                                     TotalQtyInNotebook = db.MsItemPartition.Where(item => item.ItemCardId == itemCard.ItemCardId)
                                               .Sum(item => item.QtyInNotebook),
                                     image = img != null ? img.ImgPath : null // يمكنك تضمين الصورة هنا

                                 })
                            .FirstOrDefault();

                if (data != null)
                {
                    resultDTO.data = data;
                    resultDTO.Status = true;
                    resultDTO.Message = "تم عرض البيانات بنجاح";
                }
                else
                {
                    resultDTO.Message = "لم يتم العثور على بيانات لهذا الباركود";
                }

                return resultDTO;
            }
            catch (Exception ex)
            {
                // يجب التعامل مع الاستثناءات هنا، يمكنك إعادة رمية الاستثناء أو التسجيل والعمليات اللازمة.
                throw ex;
            }
        }


        //public ResultDTO barcode(string qr)
        //{
        //    ResultDTO resultDTO = new ResultDTO() { Status = false, Message = "خطأ في البيانات" };
        //    try
        //    {
        //        VendorVM data = _repository.Find(x => x.ItemCode == qr)
        //            .Join(
        //                _msItemPartitionRepository.GetAll(),
        //                p => p.ItemCardId,
        //                mp => mp.ItemCardId,
        //                (p, mp) => new VendorVM
        //                {
        //                    ItemCardId = p.ItemCardId,
        //                    itemcode = p.ItemCode,
        //                    itemname = p.ItemDescA,
        //                    Qty = p.QtyInBox,
        //                    Price = p.FirstPrice,
        //                    Price2 = p.SecandPrice,
        //                    Price3 = p.ThirdPrice,
        //                    CoastAVG = p.CoastAverage,
        //                    Vendors = GetVendors(p.ItemCardId),
        //                    image = p.TaxItemCode,
        //                    QtyInNotebook = db.MsItemPartition.Where(mp=>mp.ItemCardId == mp.ItemCardId).Sum(mp=>mp.QtyInNotebook), // هنا نأخذ الـ QtyInNotebook من جدول msitempartition
        //                })
        //            .FirstOrDefault();

        //        if (data != null)
        //        {
        //            resultDTO.data = data;
        //            resultDTO.Status = true;
        //            resultDTO.Message = "تم عرض البيانات بنجاح";
        //        }
        //        else
        //        {
        //            resultDTO.Message = "لم يتم العثور على بيانات لهذا الباركود";
        //        }

        //        return resultDTO;
        //    }
        //    catch
        //    {
        //        return resultDTO;
        //    }
        //}

        //public ResultDTO barcode(string qr)
        //{
        //    ResultDTO resultDTO = new ResultDTO() { Status = false, Message = "خطا بالبيانات" };
        //    try
        //    {
        //        VendorVM data = _repository.Find(x => x.ItemCode == qr).Select(p => new VendorVM
        //        {
        //            ItemCardId = p.ItemCardId,
        //            itemcode = p.ItemCode,
        //            itemname = p.ItemDescA,
        //            Qty = p.QtyInBox,
        //            Price = p.FirstPrice,
        //            Price2 = p.SecandPrice,
        //            Price3 = p.ThirdPrice,
        //            CoastAVG = p.CoastAverage,
        //            Vendors = GetVendors(p.ItemCardId),
        //            image = p.TaxItemCode,
        //        }).FirstOrDefault();

        //        resultDTO.data = data;
        //        resultDTO.Status = true;
        //        resultDTO.Message = "تم عرض البيانات بنجاح";
        //        return resultDTO;
        //    }
        //    catch
        //    {
        //        return resultDTO;
        //    }
        //}



        public ItemNameDto GetItems(int? Page_No, int? Size_No)
        {
            ItemNameDto itmDto = new ItemNameDto();
            int Size_Of_Page = (Size_No ?? 9);
            int No_Of_Page = (Page_No ?? 1);

            var ItemsName = (
                             from item in db.MsItemCard
                             join img in db.MsItemImages
                             on item.ItemCardId equals img.ItemCardId into itemImages
                             from img in itemImages.DefaultIfEmpty()

                             //from item in db.MsItemCard
                             // join img in db.MsItemImages
                             //on item.ItemCardId equals img.ItemCardId into itemImages

                             select new ItemNameDto
                            {
                                ItemCardId=item.ItemCardId,
                                ItemDescA = item.ItemDescA,
                                ItemDescE = item.ItemDescE,
                                ItemCode = item.ItemCode,
                                ItemType = (int)item.ItemType,
                                IsSerialItem = (bool)item.IsSerialItem,
                                IsExpir = (bool)item.IsExpir,
                                IsDimension = (bool)item.IsDimension,
                                IsAttributeItem = (bool)item.IsAttributeItem,
                                IsCommisionPercent = (bool)item.IsCommisionPercent,
                                TaxesId1 = (int)item.TaxesId1,
                                Tax1Rate = (int)item.Tax1Rate,
                                TaxesId2 = (int)item.TaxesId2,
                                Tax2Rate= (decimal)item.Tax2Rate,
                                Tax3Rate= (int)item.Tax3Rate,
                                TaxesId3 = (int)item.TaxesId3,
                                Tax1ForPurch = (bool)item.Tax1ForPurch,
                                Tax2ForPurch= (bool)item.Tax2ForPurch,
                                Tax3ForPurch= (bool)item.Tax3ForPurch,
                                Tax1ForSale = (bool)item.Tax1ForSale,
                                Tax2ForSale= (bool)item.Tax2ForSale,
                                Tax3ForSale= (bool)item.Tax3ForSale,
                                Tax1PlusOrMinus = (bool)item.Tax1PlusOrMinus,
                                Tax2PlusOrMinus= (bool)item.Tax2PlusOrMinus,
                                Tax3PlusOrMinus = (bool)item.Tax3PlusOrMinus,
                                Tax1IsAccomulative = (bool)item.Tax1IsAccomulative,
                                Tax2IsAccomulative= (bool)item.Tax2IsAccomulative,
                                Tax3IsAccomulative= (bool)item.Tax3IsAccomulative,
                                Tax1Style= (int)item.Tax1Style,
                                Tax2Style = (int)item.Tax2Style,
                                Tax3Style= (int)item.Tax3Style,
                                IsCollection = (bool)item.IsCollection,
                                ProductTypeId = (int)item.ProductTypeId,
                                ItemColor = item.ItemColor,
                                
                                imagePath=img.ImgPath
                            }).Skip((No_Of_Page - 1) * Size_Of_Page).Take(Size_Of_Page);
            itmDto.Page_No = No_Of_Page;
            itmDto.Size_No = Size_Of_Page;
            itmDto.data = ItemsName;

            return itmDto;
        }

    

        //public ItemNameDto GetItemsWithFilter(int? Page_No, int? Size_No, int? storepartId = null, int? itemCategoryId = null)
        //{
        //    ItemNameDto itmDto = new ItemNameDto();
        //    int Size_Of_Page = (Size_No ?? 9);
        //    int No_Of_Page = (Page_No ?? 1);

        //    var query = (
        //                     from item in db.MsItemCard
        //                     join img in db.MsItemImages
        //                     on item.ItemCardId equals img.ItemCardId into itemImages
        //                     from img in itemImages.DefaultIfEmpty()

        //                     select new 
        //                     {
        //                         ItemCardId = item.ItemCardId,
        //                         ItemDescA = item.ItemDescA,
        //                         ItemDescE = item.ItemDescE,
        //                         ItemCode = item.ItemCode,
        //                         ItemType = (int)item.ItemType,
        //                         IsSerialItem = (bool)item.IsSerialItem,
        //                         IsExpir = (bool)item.IsExpir,
        //                         IsDimension = (bool)item.IsDimension,
        //                         IsAttributeItem = (bool)item.IsAttributeItem,
        //                         IsCommisionPercent = (bool)item.IsCommisionPercent,
        //                         TaxesId1 = (int)item.TaxesId1,
        //                         Tax1Rate = (int)item.Tax1Rate,
        //                         TaxesId2 = (int)item.TaxesId2,
        //                         Tax2Rate = (decimal)item.Tax2Rate,
        //                         Tax3Rate = (int)item.Tax3Rate,
        //                         TaxesId3 = (int)item.TaxesId3,
        //                         Tax1ForPurch = (bool)item.Tax1ForPurch,
        //                         Tax2ForPurch = (bool)item.Tax2ForPurch,
        //                         Tax3ForPurch = (bool)item.Tax3ForPurch,
        //                         Tax1ForSale = (bool)item.Tax1ForSale,
        //                         Tax2ForSale = (bool)item.Tax2ForSale,
        //                         Tax3ForSale = (bool)item.Tax3ForSale,
        //                         Tax1PlusOrMinus = (bool)item.Tax1PlusOrMinus,
        //                         Tax2PlusOrMinus = (bool)item.Tax2PlusOrMinus,
        //                         Tax3PlusOrMinus = (bool)item.Tax3PlusOrMinus,
        //                         Tax1IsAccomulative = (bool)item.Tax1IsAccomulative,
        //                         Tax2IsAccomulative = (bool)item.Tax2IsAccomulative,
        //                         Tax3IsAccomulative = (bool)item.Tax3IsAccomulative,
        //                         Tax1Style = (int)item.Tax1Style,
        //                         Tax2Style = (int)item.Tax2Style,
        //                         Tax3Style = (int)item.Tax3Style,
        //                         IsCollection = (bool)item.IsCollection,
        //                         ProductTypeId = (int)item.ProductTypeId,
        //                         ItemColor = item.ItemColor,
        //                         storepartId = item.StorePartId,
        //                         ItemCategoryId = item.ItemCategoryId,
        //                         imagePath = img.ImgPath
        //                     }).Skip((No_Of_Page - 1) * Size_Of_Page).Take(Size_Of_Page);

        //    if (storepartId.HasValue)
        //    {
        //        query = query.Where(q => q.storepartId == storepartId);
        //    }

        //    if (itemCategoryId.HasValue)
        //    {
        //        query = query.Where(q => q.ItemCategoryId == itemCategoryId);
        //    }


        //    itmDto.Page_No = No_Of_Page;
        //    itmDto.Size_No = Size_Of_Page;
        //    itmDto.data = query;

        //    return itmDto;
        //}


        public ItemNameDto GetItemscode(string code)
        {
            ItemNameDto itmDto = new ItemNameDto();


            var ItemsName = (from item in db.MsItemCard
                             join img in db.MsItemImages
                             on item.ItemCardId equals img.ItemCardId
                             where item.ItemCode == code
                             select new ItemNameDto
                             {
                                 ItemCardId = item.ItemCardId,
                                 ItemDescA = item.ItemDescA,
                                 ItemDescE = item.ItemDescE,
                                 ItemCode = item.ItemCode,
                                 ItemType = (int)item.ItemType,
                                 IsSerialItem = (bool)item.IsSerialItem,
                                 IsExpir = (bool)item.IsExpir,
                                 IsDimension = (bool)item.IsDimension,
                                 IsAttributeItem = (bool)item.IsAttributeItem,
                                 IsCommisionPercent = (bool)item.IsCommisionPercent,
                                 TaxesId1 = (int)item.TaxesId1,
                                 Tax1Rate = (int)item.Tax1Rate,
                                 TaxesId2 = (int)item.TaxesId2,
                                 Tax2Rate = (decimal)item.Tax2Rate,
                                 Tax3Rate = (int)item.Tax3Rate,
                                 TaxesId3 = (int)item.TaxesId3,
                                 Tax1ForPurch = (bool)item.Tax1ForPurch,
                                 Tax2ForPurch = (bool)item.Tax2ForPurch,
                                 Tax3ForPurch = (bool)item.Tax3ForPurch,
                                 Tax1ForSale = (bool)item.Tax1ForSale,
                                 Tax2ForSale = (bool)item.Tax2ForSale,
                                 Tax3ForSale = (bool)item.Tax3ForSale,
                                 Tax1PlusOrMinus = (bool)item.Tax1PlusOrMinus,
                                 Tax2PlusOrMinus = (bool)item.Tax2PlusOrMinus,
                                 Tax3PlusOrMinus = (bool)item.Tax3PlusOrMinus,
                                 Tax1IsAccomulative = (bool)item.Tax1IsAccomulative,
                                 Tax2IsAccomulative = (bool)item.Tax2IsAccomulative,
                                 Tax3IsAccomulative = (bool)item.Tax3IsAccomulative,
                                 Tax1Style = (int)item.Tax1Style,
                                 Tax2Style = (int)item.Tax2Style,
                                 Tax3Style = (int)item.Tax3Style,

                                 imagePath = img.ImgPath
                             });

            itmDto.data = ItemsName;

            return itmDto;
        }

        public IEnumerable<TaxesDto> GetTaxes()
        {
            var TaxesItem = from itm in db.MsTaxes
                            select new TaxesDto
                            {
                                TaxesId = itm.TaxesId,
                                TaxCode = itm.TaxCode,
                                TaxNameA = itm.TaxNameA,
                                TaxNameE = itm.TaxNameE,
                                TaxRate = (decimal)itm.TaxRate,
                                TaxStyle= (int)itm.TaxStyle,
                                IsAccomulative = (bool)itm.IsAccomulative,
                                PlusOrMinus= (bool)itm.PlusOrMinus
                            };
            return TaxesItem;
             
        }

        public IEnumerable<ItemByIdDto> ItemsByID(int id)
        {
           
            var items = from itmUnit in db.MsItemUnit

                        //itmCard in db.MsItemCard
                        //join
                        //itmUnit in db.MsItemUnit on itmCard.ItemCardId equals itmUnit.ItemCardId 
                        //join
                        //img in db.MsItemImages on itmCard.ItemCardId equals img.ItemCardId 
                        where itmUnit.ItemCardId == id 
                        select new ItemByIdDto
                        {
                            UnitId = itmUnit.UnitId,
                            UnitNam = itmUnit.UnitNam,
                            UnitNamE = itmUnit.UnitNameE,
                            BarCode1 = itmUnit.BarCode1,
                            IsDefaultPurchas = (bool)itmUnit.IsDefaultPurchas,
                            IsDefaultSale= (bool)itmUnit.IsDefaultSale,
                            Price1= (decimal)itmUnit.Price1,
                            Price2= (decimal)itmUnit.Price2,
                            Price3= (decimal)itmUnit.Price3,
                            Price4= (decimal)itmUnit.Price4,
                            Price5= (decimal)itmUnit.Price5,
                            Price6= (decimal)itmUnit.Price6,
                            Price7= (decimal)itmUnit.Price7,
                            Price8= (decimal)itmUnit.Price8,
                            Price9= (decimal)itmUnit.Price9,
                            Price10= (decimal)itmUnit.Price10,
                            Quantity1= (decimal)itmUnit.Quantity1,
                            Quantity2= (decimal)itmUnit.Quantity2,
                            Quantity3= (decimal)itmUnit.Quantity3,
                            Quantity4= (decimal)itmUnit.Quantity4,
                            Quantity5= (decimal)itmUnit.Quantity5,                            
                            UnitCode=itmUnit.UnitCode,
                            UnittRate= (decimal)itmUnit.UnittRate,
                        };
            return items;
        }

        public IEnumerable<ItemSerialAndPartitionDto> GetSerialAndpartition(int id)
        {
            var items = from itmSerial in db.MsItemSerials
                        join itmPart in db.MsItemPartition
                        on itmSerial.ItemCardId equals itmPart.ItemCardId join part in db.MsPartition on itmPart.StorePartId equals part.StorePartId
                        where itmSerial.ItemCardId == id && itmSerial.IsInOrOut == true
                        select new ItemSerialAndPartitionDto
                        {
                            ItemSerialId = itmSerial.ItemSerialId,
                            SRNo1 = itmSerial.Srno1,
                            Color= itmSerial.Color,
                            Height= (decimal)itmSerial.Height,
                            PartCode=part.PartCode,
                            PartDescA= part.PartDescA,
                            PartDescE=part.PartDescE,
                           StorePartId= part.StorePartId,
                           CoastAverage= (decimal)itmPart.CoastAverage,
                           QtyInNotebook = (decimal)itmPart.QtyInNotebook,
                           ReservedQty = (decimal)itmPart.ReservedQty   
                        };

            return items;
        }

        //public IEnumerable<ItemPartationDto> GetPartation(int id)
        //{
        //    var items = from itmPart in db.MsItemPartition
        //                join part in db.MsPartition
        //                on itmPart.StorePartId equals part.StorePartId
        //                where itmPart.ItemCardId == id

        //                select new ItemPartationDto
        //                {
        //                    itemPardId = itmPart.ItemPartId,
        //                    PartDescA = part.PartDescA,
        //                    PartDescE = part.PartDescE,
        //                    StorePartId = (int)itmPart.StorePartId,
        //                    CoastAverage = (decimal)itmPart.CoastAverage,
        //                    QtyInNotebook = (decimal)itmPart.QtyInNotebook,
        //                    ReservedQty = (decimal)itmPart.ReservedQty
        //                };

        //    return items;
        //}


        public IEnumerable<getHeaderDto> GetHeaderByCustomerId(string creatdBy)
        {
            var header = from salesInovice in db.MobSalesInvoice
                         join customer in db.MsCustomer
                         on salesInovice.CustomerId equals customer.CustomerId
                         join book in db.SysBooks
                         on salesInovice.BookId equals book.BookId
                         orderby salesInovice.CreatedAt descending
                         where salesInovice.CreatedBy == creatdBy  && salesInovice.DeletedAt == null && salesInovice.IsMobile == true
                         select new getHeaderDto
                         {
                             InvId = salesInovice.MobInvId,
                             CustomerCode = customer.CustomerCode,
                             CustomerDescA = customer.CustomerDescA,
                             NetPrice = (decimal)salesInovice.NetPrice,
                             DocTrno = book.PrefixCode + "-" + salesInovice.TrNo,
                             CreatedAt = (DateTime)salesInovice.CreatedAt,
                             AddField4_transport = salesInovice.AddField4,
                             AddField5_SalesManager = salesInovice.AddField5
                         };

            return header;
        }

        public IEnumerable<getOrderHeaderDto> GetOrderHeaderByCreatBy(string createdBy)
        {
            var orderHeader = from MsSalesOrder in db.MsSalesOrder
                              join customer in db.MsCustomer
                              on MsSalesOrder.CustomerId equals customer.CustomerId
                              join book in db.SysBooks
                              on MsSalesOrder.BookId equals book.BookId
                              orderby MsSalesOrder.CreatedAt descending
                              where MsSalesOrder.CreatedBy == createdBy && MsSalesOrder.DeletedAt == null && MsSalesOrder.IsMobile == true
                              select new getOrderHeaderDto
                              {
                                  SalesOrderId = MsSalesOrder.SalesOrderId,
                                  CustomerCode = customer.CustomerCode,
                                  CustomerDescA = customer.CustomerDescA,
                                  NetPrice = (decimal)MsSalesOrder.NetPrice,
                                  DocTrno = book.PrefixCode + "-" + MsSalesOrder.TrNo,
                                  CreatedAt = (DateTime)MsSalesOrder.CreatedAt,
                                  AddField4_transport = MsSalesOrder.AddField4,
                                  AddField5_SalesManager = MsSalesOrder.AddField5,
                              };
            return orderHeader;
                              
        }

        public IEnumerable<getheaderSalesOfferDto> getheaderSalesOfferByCreatBy(string createdBy)
        {
            var offerHeader = from salesOffer in db.MsSalesOffer
                              join customer in db.MsCustomer
                              on salesOffer.CustomerId equals customer.CustomerId
                              join book in db.SysBooks
                              on salesOffer.BookId equals book.BookId
                              orderby salesOffer.CreatedAt descending
                              where salesOffer.CreatedBy == createdBy && salesOffer.DeletedAt == null && salesOffer.IsMobile == true
                              select new getheaderSalesOfferDto
                              {
                                  SalesOfferId = salesOffer.SalesOfferId,
                                  CustomerCode = customer.CustomerCode,
                                  CustomerDescA = customer.CustomerDescA,
                                  NetPrice = (decimal)salesOffer.NetPrice,
                                  DocTrno = book.PrefixCode + "-" + salesOffer.TrNo,
                                  CreatedAt = (DateTime)salesOffer.CreatedAt,
                                  AddField4_transport = salesOffer.AddField4,
                                  AddField5_SalesManager = salesOffer.AddField5
                              };
            return offerHeader;
        }
        public IEnumerable<getDetailDto> GetDetailByInvId(int invId)
        {
            var Detail = from salesInvoiceItemCard in db.MobSalesInvoiceItemCard
                         join itemCard in db.MsItemCard
                         on salesInvoiceItemCard.ItemCardId equals itemCard.ItemCardId
                         join itemUnit in db.MsItemUnit
                         on salesInvoiceItemCard.UnitId equals itemUnit.UnitId
                         join storePartation in db.MsPartition
                         on salesInvoiceItemCard.StorePartId equals storePartation.StorePartId
                         join store in db.MsStores
                         on salesInvoiceItemCard.StoreId equals store.StoreId
                         where salesInvoiceItemCard.MobInvId == invId
                         select new getDetailDto
                         {
                             ItemCardId = (int)salesInvoiceItemCard.ItemCardId,
                             ItemCode = itemCard.ItemCode,
                             ItemDescA = itemCard.ItemDescA,
                             UnitNam = itemUnit.UnitNam,
                             PartCode = storePartation.PartCode,
                             PartDescA = storePartation.PartDescA,
                             StoreCode = store.StoreCode,
                             StoreDescA = store.StoreDescA,
                             Price = (decimal)salesInvoiceItemCard.Price,
                             PriceAfterRate = (decimal)salesInvoiceItemCard.PriceAfterRate,
                             QtyBeforRate = (decimal)salesInvoiceItemCard.QtyBeforRate,
                             Quantity = (decimal)salesInvoiceItemCard.Quantity,
                             UnitId = (int)salesInvoiceItemCard.UnitId,
                             UnitRate = (decimal)salesInvoiceItemCard.UnitRate,
                             StoreId = (int)salesInvoiceItemCard.StoreId,
                             StorePartId = (int)salesInvoiceItemCard.StorePartId,
                             DisPercent = (decimal)salesInvoiceItemCard.DisPercent,
                             DisAmount = (decimal)salesInvoiceItemCard.DisAmount,
                             Tax1Percent = (decimal)salesInvoiceItemCard.Tax1Percent,
                             Tax2Percent = (decimal)salesInvoiceItemCard.Tax2Percent,
                             Tax3Percent = (decimal)salesInvoiceItemCard.Tax3Percent,
                             TaxesId1 = (int)salesInvoiceItemCard.TaxesId1,
                             TaxesId2 = (int)salesInvoiceItemCard.TaxesId2,
                             TaxesId3 = (int)salesInvoiceItemCard.TaxesId3,
                             Tax1IsAccomulative = (bool)salesInvoiceItemCard.Tax1IsAccomulative,
                             Tax2IsAccomulative = (bool)salesInvoiceItemCard.Tax2IsAccomulative,
                             Tax3IsAccomulative = (bool)salesInvoiceItemCard.Tax3IsAccomulative
                         };
            return Detail;
                        
        }

        public IEnumerable<getOrderDetailDto> GetOrderDetailBySalesOrdertemCardId(int SalesOrderId)
        {
            var OrderDetail = from MsSalesOrderItemCard in db.MsSalesOrderItemCard
                              join itemCard in db.MsItemCard
                              on MsSalesOrderItemCard.ItemCardId equals itemCard.ItemCardId
                              join itemUnit in db.MsItemUnit
                              on MsSalesOrderItemCard.UnitId equals itemUnit.UnitId
                              join storePartation in db.MsPartition
                              on MsSalesOrderItemCard.StorePartId equals storePartation.StorePartId
                              join store in db.MsStores
                              on MsSalesOrderItemCard.StoreId equals store.StoreId
                              where MsSalesOrderItemCard.SalesOrderId == SalesOrderId
                              select new getOrderDetailDto
                              {
                                  ItemCardId = (int)MsSalesOrderItemCard.ItemCardId,
                                  ItemCode = itemCard.ItemCode,
                                  ItemDescA = itemCard.ItemDescA,
                                  UnitNam = itemUnit.UnitNam,
                                  PartCode = storePartation.PartCode,
                                  PartDescA = storePartation.PartDescA,
                                  StoreCode = store.StoreCode,
                                  StoreDescA = store.StoreDescA,
                                  Price = (decimal)MsSalesOrderItemCard.Price,
                                  PriceAfterRate = (decimal)MsSalesOrderItemCard.PriceAfterRate,
                                  QtyBeforRate = (decimal)MsSalesOrderItemCard.QtyBeforRate,
                                  Quantity = (decimal)MsSalesOrderItemCard.Quantity,
                                  UnitId = (int)MsSalesOrderItemCard.UnitId,
                                  UnitRate = (decimal)MsSalesOrderItemCard.UnitRate,
                                  StoreId = (int)MsSalesOrderItemCard.StoreId,
                                  StorePartId = (int)MsSalesOrderItemCard.StorePartId,
                                  DisPercent = (decimal)MsSalesOrderItemCard.DisPercent,
                                  DisAmount = (decimal)MsSalesOrderItemCard.DisAmount,
                                  Tax1Percent = (decimal)MsSalesOrderItemCard.Tax1Percent,
                                  Tax2Percent = (decimal)MsSalesOrderItemCard.Tax2Percent,
                                  Tax3Percent = (decimal)MsSalesOrderItemCard.Tax3Percent,
                                  TaxesId1 = (int)MsSalesOrderItemCard.TaxesId1,
                                  TaxesId2 = (int)MsSalesOrderItemCard.TaxesId2,
                                  TaxesId3 = (int)MsSalesOrderItemCard.TaxesId3,
                                  Tax1IsAccomulative = (bool)MsSalesOrderItemCard.Tax1IsAccomulative,
                                  Tax2IsAccomulative = (bool)MsSalesOrderItemCard.Tax2IsAccomulative,
                                  Tax3IsAccomulative = (bool)MsSalesOrderItemCard.Tax3IsAccomulative
                              };

            return OrderDetail;
        }

        public IEnumerable<getSalesOfferDetailDto> GetSalesOfferDetailBySalesOffertemCardId(int SalesOfferId)
        {
            var SalesOffer = from salesOffer in db.MsSalesOfferItemCard
                             join itemCard in db.MsItemCard
                              on salesOffer.ItemCardId equals itemCard.ItemCardId
                             join itemUnit in db.MsItemUnit
                             on salesOffer.UnitId equals itemUnit.UnitId
                             join storePartation in db.MsPartition
                             on salesOffer.StorePartId equals storePartation.StorePartId
                             join store in db.MsStores
                             on salesOffer.StoreId equals store.StoreId
                             where salesOffer.SalesOfferId == SalesOfferId
                             select new getSalesOfferDetailDto
                             {
                                 ItemCardId = (int)salesOffer.ItemCardId,
                                 ItemCode = itemCard.ItemCode,
                                 ItemDescA = itemCard.ItemDescA,
                                 UnitNam = itemUnit.UnitNam,
                                 PartCode = storePartation.PartCode,
                                 PartDescA = storePartation.PartDescA,
                                 StoreCode = store.StoreCode,
                                 StoreDescA = store.StoreDescA,
                                 Price = (decimal)salesOffer.Price,
                                 PriceAfterRate = (decimal)salesOffer.PriceAfterRate,
                                 QtyBeforRate = (decimal)salesOffer.QtyBeforRate,
                                 Quantity = (decimal)salesOffer.Quantity,
                                 UnitId = (int)salesOffer.UnitId,
                                 UnitRate = (decimal)salesOffer.UnitRate,
                                 StoreId = (int)salesOffer.StoreId,
                                 StorePartId = (int)salesOffer.StorePartId,
                                 DisPercent = (decimal)salesOffer.DisPercent,
                                 DisAmount = (decimal)salesOffer.DisAmount,
                                 Tax1Percent = (decimal)salesOffer.Tax1Percent,
                                 Tax2Percent = (decimal)salesOffer.Tax2Percent,
                                 Tax3Percent = (decimal)salesOffer.Tax3Percent,
                                 TaxesId1 = (int)salesOffer.TaxesId1,
                                 TaxesId2 = (int)salesOffer.TaxesId2,
                                 TaxesId3 = (int)salesOffer.TaxesId3,
                                 Tax1IsAccomulative = (bool)salesOffer.Tax1IsAccomulative,
                                 Tax2IsAccomulative = (bool)salesOffer.Tax2IsAccomulative,
                                 Tax3IsAccomulative = (bool)salesOffer.Tax3IsAccomulative
                             };

            return SalesOffer;
        }
        public IEnumerable<GetPartitionAndStoreDto> GetPartitionWithStores(int id , int storeid)
        {
            MsItemCardDefaulPartitions defaulPartition = db.MsItemCardDefaulPartitions.FirstOrDefault(p=>p.ItemCardId == id && p.StoreId == storeid);
            MsPartition DefaultStore = null ; 

            if (defaulPartition != null)
            {
                 DefaultStore = db.MsPartition.FirstOrDefault(p => p.StorePartId == defaulPartition.StorePartId);

            }

            var items = from Partition in db.MsPartition
                        join itmPartition in db.MsItemPartition
                        on Partition.StorePartId equals itmPartition.StorePartId
                        join store in db.MsStores on itmPartition.StoreId equals store.StoreId
                        where itmPartition.ItemCardId == id
                        select new GetPartitionAndStoreDto
                        {
                            PartCode = Partition.PartCode,
                            PartDescA = Partition.PartDescA,
                            PartDescE = Partition.PartDescE,
                            StorePartId = Partition.StorePartId,
                            QtyInNotebook = (decimal)itmPartition.QtyInNotebook,
                            CoastAverage = (decimal)itmPartition.CoastAverage,
                            ReservedQty = (decimal)itmPartition.ReservedQty,
                            StoreId = store.StoreId,
                            StoreCode = store.StoreCode,
                            StoreDescA = store.StoreDescA,
                            StoreDescE = store.StoreDescE,
                            DefaultStore = DefaultStore != null ? DefaultStore.PartDescA : string.Empty
                        };

            var st = from part in db.MsPartition
                     where part.StoreId == storeid
                     select new GetPartitionAndStoreDto
                     {
                         PartCode = part.PartCode,
                         PartDescA = part.PartDescA,
                         PartDescE = part.PartDescE,
                         StoreId = (int)part.StoreId,
                         StorePartId = part.StorePartId
                         
                     };


            if (items.Any())
            {
                return items;
            }
            return st;

        }

        public object AddHeadrt()
        {
            var items = from counter in db.SysCounter
                        join books in db.SysBooks
                        on counter.BookId equals books.BookId
                        join SalesInvoice in db.MsSalesInvoice on books.BookId equals SalesInvoice.BookId 
                        where counter.TrIdName == "InvId"
                        select new HeaderDto
                        {
                           
                        };
            return items;
        }

        public int getcounter(int getBookId)
        {
            var book = (from counter in db.SysCounter
                        where counter.BookId == getBookId && counter.TrIdName == "InvId" 
                        select  counter.Counter).Single();

            return Convert.ToInt32(book);
        }



    }
}
