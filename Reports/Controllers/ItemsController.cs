using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Xml.Linq;
using Azure;
using BLL.DTO;
using BLL.Service;
using DAL.Context;
using DAL.Models;
using DAL.Models2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Static.Helper;
using Static.VM;
using static System.Reflection.Metadata.BlobBuilder;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Reports.Controllers
{
    public class ItemsController : BaseController
    {
        private readonly ItemsBll _itemsBll;
        private readonly SmartERPStandardContext _db;

        public ItemsController(ItemsBll itemsBll, SmartERPStandardContext db)
        {
            _itemsBll = itemsBll;
            _db = db;
        }

        [HttpPost, AllowAnonymous]
        public ResultDTO GetAllItems(int? Page_No, int? Size_No)
        {
            //HttpContext.Current.Server.ScriptTimeout = 2000;
            ResultDTO resultDTO = _itemsBll.GetAllItems(Page_No, Size_No);
            return resultDTO;
        }

        [HttpPost("itempartition")]
        public IActionResult GetPartitionCount(int itemcardid)
        {
            var totalItem = _db.MsItemPartition
                .Where(item => item.ItemCardId == itemcardid)
                .Sum(item => item.QtyInNotebook);

            return Ok(new { TotalQtyInNotebook = totalItem });
        }


        [HttpGet]
        public IEnumerable<TaxesDto> GetTaxes()
        {
            return _itemsBll.GetTaxes();
        }

        [HttpPost, AllowAnonymous]
        public ResultDTO barcode(string qr)
        {
            ResultDTO resultDTO = _itemsBll.barcode(qr);
            return resultDTO;
        }




        [HttpPost, AllowAnonymous]
        public ItemNameDto GetItems(int page_number, int page_size)
        {
            return _itemsBll.GetItems(page_number, page_size);
        }

        [HttpPost, AllowAnonymous]
        public ItemNameDto GetItemsbarcode(string code)
        {
            return _itemsBll.GetItemscode(code);
        }

        [HttpPost, AllowAnonymous]
        public IEnumerable<ItemByIdDto> GetItemsById(int CardItemId)
        {
            return _itemsBll.ItemsByID(CardItemId);
        }

        [HttpPost, AllowAnonymous]
        public IEnumerable<ItemSerialAndPartitionDto> GetItemSerialAndPartitions(int ItemCardId)
        {
            return _itemsBll.GetSerialAndpartition(ItemCardId);
        }

        //[HttpPost, AllowAnonymous]
        //public IEnumerable<ItemPartationDto> getPArtation(int ItemCardId)
        //{
        //    return _itemsBll.GetPartation(ItemCardId);
        //}




        [HttpPost, AllowAnonymous]
        public IEnumerable<GetPartitionAndStoreDto> GetPartitionWithStores(int ItemCardId, int storeid)
        {
            return _itemsBll.GetPartitionWithStores(ItemCardId, storeid);

        }

      



        [HttpPost, AllowAnonymous]
        public IActionResult CreateHeader([FromBody] HeaderDto dto)
        {

            var book = from counter in _db.SysCounter
                       where counter.BookId == dto.Header_BookId && counter.TrIdName == "MobInvId"
                       select counter.Counter;
            var empid = _db.GUsers.FirstOrDefault(x => x.UserId == Convert.ToInt32(dto.Header_CreatedBy));


            var MobsalesInvoice = new MobSalesInvoice
            {
                BookId = dto.Header_BookId,
                TermId = dto.Header_TermId,
                CurrencyId = dto.Header_CurrencyId,
                CustomerId = dto.Header_CustomerId,
                RectSourceType = (byte)dto.Header_RectSourceType,
                Rate = dto.Header_Rate,
                NotPaid = dto.NotPaid,
                CreatedBy = dto.Header_CreatedBy,
                CreatedAt = dto.Header_CreatedAt,
                InvTotal = dto.Header_InvTotal,
                DiscAmount = dto.Header_DiscAmount,
                DiscPercent = dto.Header_DiscPercent,
                DiscAmount2 = dto.Header_DiscAmount2,
                DiscPercent2 = dto.Header_DiscPercent2,
                TotalItemTax1 = dto.Header_TotalItemTax1,
                TotalItemTax2 = dto.Header_TotalItemTax2,
                TotalItemTax3 = dto.Header_TotalItemTax3,
                TaxValue1 = dto.Header_TaxValue1,
                TaxValue2 = dto.Header_TaxValue2,
                TaxValue3 = dto.Header_TaxValue3,
                TaxesId1 = dto.Header_TaxesId1,
                TaxesId2 = dto.Header_TaxesId2,
                TaxesId3 = dto.Header_TaxesId3,
                PriceAfterTax = dto.Header_PriceAfterTax,
                NetPrice = dto.Header_NetPrice,
                PaidPrice = dto.Header_PaidPrice,
                PaidPriceVisa = dto.Header_PaidPriceVisa,
                BankTransfer = dto.Header_BankTransfer,
                //  added on 11-6-2023
                Remarks = dto.Remarks,
                AddField3 = dto.AddField3,
                InvDueDate = dto.InvDueDate,
                IsMobile = true,
                StoreId = dto.StoreId,
                ExpenValue = dto.ExpenValue,
                //  added on 1-8-2023
                InvoiceType = (byte?)dto.InvoiceType,
                TrDate = dto.Header_CreatedAt,
                EmpId = empid.EmpId


            };

                MobsalesInvoice.TrNo = ((int)(book).FirstOrDefault()) + 1;
            


            _db.MobSalesInvoice.Add(MobsalesInvoice);
            _db.SaveChanges();

            var Counter = _db.SysCounter.FirstOrDefault(x => x.BookId == dto.Header_BookId);
            Counter.Counter = MobsalesInvoice.TrNo;

            _db.SaveChanges();

            ResultHeaderAndDetialDto res = new ResultHeaderAndDetialDto();
            res.Invid = MobsalesInvoice.MobInvId;
            res.Message = "تم أضافه الداتا بنجاح";

            return Ok(res);

        }

        

        [HttpPost]
        public IActionResult CreateDetail([FromBody] List<DetailDto> dto)
        {


            var InvD = 0;
            foreach (var item in dto)
            {
                var itmType = _db.MsItemCard.Find(item.ItemCardId);

                _db.MobSalesInvoiceItemCard.Add(new MobSalesInvoiceItemCard()
                {
                    MobInvId = item.InvId,
                    ItemCardId = item.ItemCardId,
                    Price = item.Price,
                    //PriceAfterRate = item.PriceAfterRate,
                    QtyBeforRate = item.QtyBeforRate,
                    // في حاله ال item is serial
                    //Quantity = item.Quantity,
                    UnitId = item.UnitId,
                    UnitRate = item.UnitRate,
                    StoreId = item.StoreId,
                    StorePartId = item.StorePartId,
                    DisAmount = item.DisAmount,
                    // اول معادله 

                    DisPercent = item.DisAmount > 0 ? (item.DisAmount / (item.Price * item.QtyBeforRate)) * 100 : 0,
                    // المعادله التانيه

                    Quantity = item.QtyBeforRate * item.UnitRate,

                    // التعديل الثالث الخاص ب ال item Type

                    ItemType = itmType.ItemType,

                    Tax1Percent = item.Tax1Percent,
                    Tax2Percent = item.Tax2Percent,
                    Tax3Percent = item.Tax3Percent,
                    TaxesId1 = item.Detail_TaxesId1,
                    TaxesId2 = item.Detail_TaxesId2,
                    TaxesId3 = item.Detail_TaxesId3,
                    Tax1IsAccomulative = item.Tax1IsAccomulative,
                    Tax2IsAccomulative = item.Tax2IsAccomulative,
                    Tax3IsAccomulative = item.Tax3IsAccomulative,
                    IsCollection = item.IsCollection
                });
                InvD = item.InvId;
            }
            _db.SaveChanges();
            ResultHeaderAndDetialDto res = new ResultHeaderAndDetialDto();
            res.Invid = InvD;
            res.Message = "تم أضافه الداتا بنجاح";
            return Ok(res);
        }

        [HttpPut("{invid}")]
        public async Task<IActionResult> UpdateHeader(int invid, [FromBody] UpdatedHeaderDto dto)
        {
            var updateDto = new updateHeaderDto();
            var header = await _db.MobSalesInvoice.FindAsync(invid);
            if (header == null) return NotFound(invid);
            var empid = _db.GUsers.FirstOrDefault(x => x.UserId == Convert.ToInt32(dto.CreatedBy));


            header.InvTotal = dto.Header_InvTotal;
            header.UpdateAt = dto.Header_UpdateAt;
            header.DiscAmount = dto.Header_DiscAmount;
            header.DiscPercent = dto.Header_DiscPercent;
            header.DiscAmount2 = dto.Header_DiscAmount2;
            header.DiscPercent2 = dto.Header_DiscPercent2;
            header.TotalItemTax1 = dto.Header_TotalItemTax1;
            header.TotalItemTax2 = dto.Header_TotalItemTax2;
            header.TotalItemTax3 = dto.Header_TotalItemTax3;
            header.TaxValue1 = dto.Header_TaxValue1;
            header.TaxValue2 = dto.Header_TaxValue2;
            header.TaxValue3 = dto.Header_TaxValue3;
            header.TaxesId1 = dto.Header_TaxesId1;
            header.TaxesId2 = dto.Header_TaxesId2;
            header.TaxesId3 = dto.Header_TaxesId3;
            header.PriceAfterTax = dto.Header_PriceAfterTax;
            header.NetPrice = dto.Header_NetPrice;
            header.PaidPrice = dto.Header_PaidPrice;
            header.PaidPriceVisa = dto.Header_PaidPriceVisa;
            header.BankTransfer = dto.Header_BankTransfer;
            header.Remarks = dto.Remarks;
            header.AddField3 = dto.AddField3;
            header.InvDueDate = dto.InvDueDate;
            header.StoreId = dto.StoreId;
            header.ExpenValue = dto.ExpenValue;
            //added on 1/8/2023
            header.InvoiceType = (byte?)dto.InvoiceType;
            header.CreatedBy = dto.CreatedBy;
            header.TrDate = dto.TrDate;
            header.EmpId = empid.EmpId;



            _db.SaveChanges();

            //var detailRange = _db.MobSalesInvoiceItemCard.Where(inv => inv.MobInvId == header.MobInvId).ToList();
            //_db.MobSalesInvoiceItemCard.RemoveRange(detailRange);
            //_db.SaveChanges();

            updateDto.message = "update Successfully";
            updateDto.invid = header.MobInvId;
            return Ok(updateDto);

        }

        [HttpDelete("{invid},{UserId}")]
        public async Task<IActionResult> DeleteHeader(int invid , int UserId)
        {
            var deleteMessageDto = new DeleteDto();
            var header = await _db.MobSalesInvoice.FindAsync(invid);
            if (header == null) return NotFound(invid);
            header.DeletedAt = DateTime.Now;
            header.DeletedBy = UserId.ToString();
            _db.SaveChanges();

            deleteMessageDto.message = "Data is Deleted Successfully";

            return Ok(deleteMessageDto);
        }

        [HttpGet]
        public IEnumerable<getHeaderDto> getHeaderByCustomerId(string createdBy)
        {
            return _itemsBll.GetHeaderByCustomerId(createdBy);
        }

        [HttpGet]
        public IEnumerable<getDetailDto> getDetailByInvId(int InvId)
        {
            return _itemsBll.GetDetailByInvId(InvId);
        }


        //[HttpPost, AllowAnonymous]
        //public IActionResult CreateOrderHeader(OrderHeaderDto dto)
        //{
        //    var book = from counter in _db.SysCounter
        //               where counter.BookId == dto.Header_BookId && counter.TrIdName == "SalesOrderId"
        //               select counter.Counter;
        //    var empid = _db.GUsers.FirstOrDefault(x => x.UserId == Convert.ToInt32(dto.Header_CreatedBy));

        //    var salesOrder = new MsSalesOrder
        //    {
        //        BookId = dto.Header_BookId,
        //        TermId = dto.Header_TermId,
        //        CurrencyId = dto.Header_CurrencyId,
        //        CustomerId = dto.Header_CustomerId,
        //        RectSourceType = (byte)dto.Header_RectSourceType,
        //        Rate = dto.Header_Rate,
        //        CreatedBy = dto.Header_CreatedBy,
        //        CreatedAt = dto.Header_CreatedAt,
        //        InvTotal = dto.Header_InvTotal,
        //        DiscAmount = dto.Header_DiscAmount,
        //        DiscPercent = dto.Header_DiscPercent,
        //        DiscAmount2 = dto.Header_DiscPercent2,
        //        DiscPercent2 = dto.Header_DiscPercent2,
        //        TotalItemTax1 = dto.Header_TotalItemTax1,
        //        TotalItemTax2 = dto.Header_TotalItemTax2,
        //        TotalItemTax3 = dto.Header_TotalItemTax3,
        //        TaxValue1 = dto.Header_TaxValue1,
        //        TaxValue2 = dto.Header_TaxValue2,
        //        TaxValue3 = dto.Header_TaxValue3,
        //        TaxesId1 = dto.Header_TaxesId1,
        //        TaxesId2 = dto.Header_TaxesId2,
        //        TaxesId3 = dto.Header_TaxesId3,
        //        PriceAfterTax = dto.Header_PriceAfterTax,
        //        NetPrice = dto.Header_NetPrice,
        //        PaidPrice = dto.Header_PaidPrice,
        //        PaidPriceVisa = dto.Header_PaidPriceVisa,
        //        Remarks = dto.Remarks,
        //        AddField3 = dto.AddField3,
        //        InvDueDate = dto.InvDueDate,
        //        IsMobile = true,
        //        StoreId = dto.StoreId,
        //        ExpenValue = dto.ExpenValue,
        //        //  added on 1-8-2023
        //        InvoiceType = (byte?)dto.InvoiceType,
        //        TrDate = dto.Header_CreatedAt,
        //        EmpId = empid.EmpId

        //    };

        //    salesOrder.TrNo = ((int)(book).FirstOrDefault()) + 1;


        //    _db.MsSalesOrder.Add(salesOrder);
        //    _db.SaveChanges();

        //    var Counter = _db.SysCounter.FirstOrDefault(x => x.BookId == dto.Header_BookId);
        //    Counter.Counter = salesOrder.TrNo;

        //    _db.SaveChanges();


        //    OrderResultHeaderAndDetailDto res = new OrderResultHeaderAndDetailDto();
        //    res.id = salesOrder.SalesOrderId;
        //    res.Message = "تم أضافه الداتا بنجاح";

        //    return Ok(res);
        //}

        [HttpPost, AllowAnonymous]
        public IActionResult CreateOrderHeader(OrderHeaderDto dto)
        {
            var book = from counter in _db.SysCounter
                       where counter.BookId == dto.Header_BookId && counter.TrIdName == "SalesOrderId"
                       select counter.Counter;

            var empid = _db.GUsers.FirstOrDefault(x => x.UserId == Convert.ToInt32(dto.Header_CreatedBy));

            var getBookId = _db.SysBooks.FirstOrDefault(b => b.BookId == dto.Header_BookId);

            var salesOrder = new MsSalesOrder
            {
                BookId = dto.Header_BookId,
                TermId = dto.Header_TermId,
                CurrencyId = dto.Header_CurrencyId,
                CustomerId = dto.Header_CustomerId,
                RectSourceType = (byte)dto.Header_RectSourceType,
                Rate = dto.Header_Rate,
                CreatedBy = dto.Header_CreatedBy,
                CreatedAt = dto.Header_CreatedAt,
                InvTotal = dto.Header_InvTotal,
                DiscAmount = dto.Header_DiscAmount,
                DiscPercent = dto.Header_DiscPercent,
                DiscAmount2 = dto.Header_DiscPercent2,
                DiscPercent2 = dto.Header_DiscPercent2,
                TotalItemTax1 = dto.Header_TotalItemTax1,
                TotalItemTax2 = dto.Header_TotalItemTax2,
                TotalItemTax3 = dto.Header_TotalItemTax3,
                TaxValue1 = dto.Header_TaxValue1,
                TaxValue2 = dto.Header_TaxValue2,
                TaxValue3 = dto.Header_TaxValue3,

                // تعديلات يوم  لما دخلت مع مصطفي ميتنج الصبح 9/10


                TaxesId1 = dto.Header_TaxesId1 == 0 ? null : dto.Header_TaxesId1,
                TaxesId2 = dto.Header_TaxesId2 == 0 ? null : dto.Header_TaxesId2,
                TaxesId3 = dto.Header_TaxesId3 == 0 ? null : dto.Header_TaxesId3,

                PriceAfterTax = dto.Header_PriceAfterTax,
                PaidPriceVisa = dto.Header_PaidPriceVisa,
                ExpenValue = dto.ExpenValue,


                // تعديلات يوم  لما دخلت مع مصطفي ميتنج الصبح 9/10
                // NetPrice = PriceAfterTax + ExpenseValue
                //فى حالة النقدى 0        
                //PaidPrice = NetPrice - PaidVisa
                //فى حالة الأجل 1
                //NotPaid = NetPrice - PaidVisa - PaidPrice

                NetPrice = dto.Header_PriceAfterTax + dto.ExpenValue,

                InvoiceType = (byte?)dto.InvoiceType,

                PaidPrice = dto.InvoiceType == 0 ? (dto.Header_PriceAfterTax + dto.ExpenValue) - dto.Header_PaidPriceVisa : dto.Header_PaidPrice,

                NotPaid = dto.InvoiceType == 1 ? (dto.Header_PriceAfterTax + dto.ExpenValue) - dto.Header_PaidPriceVisa - dto.Header_PaidPrice : 0,

                //PaidPrice = dto.Header_PaidPrice,
                Remarks = dto.Remarks,
                AddField3 = dto.AddField3,
                InvDueDate = dto.InvDueDate,
                IsMobile = true,
                StoreId = dto.StoreId,
                //  added on 1-8-2023
                TrDate = dto.Header_CreatedAt,
                EmpId = empid.EmpId

            };

            salesOrder.TrNo = (int)book.FirstOrDefault() + 1;


            _db.MsSalesOrder.Add(salesOrder);
            _db.SaveChanges();

            var Counter = _db.SysCounter.FirstOrDefault(x => x.BookId == dto.Header_BookId);
            Counter.Counter = salesOrder.TrNo;

            _db.SaveChanges();


            OrderResultHeaderAndDetailDto res = new OrderResultHeaderAndDetailDto();
            res.id = salesOrder.SalesOrderId;
            res.Message = "تم أضافه الداتا بنجاح";

            return Ok(res);
        }


        [HttpPost]
        public IActionResult CreateOrderDetail([FromBody]List<OrderDetailDto> dto)
        {
            var OrderId = 0;
            foreach (var item in dto)
            {
                var itmType = _db.MsItemCard.Find(item.ItemCardId);

                var itemPartition = _db.MsItemPartition
                          .Where(p => p.ItemCardId == item.ItemCardId && p.StorePartId == item.StorePartId)
                          .FirstOrDefault();

                if(itemPartition != null)
                {
                    decimal quantity = item.QtyBeforRate * item.UnitRate;
                    decimal? reservedQty = itemPartition.ReservedQty;
                    if (reservedQty == null)
                    {
                        reservedQty = 0;

                    }
                    reservedQty += quantity;
                    itemPartition.ReservedQty = reservedQty;
                }


                _db.MsSalesOrderItemCard.Add(new MsSalesOrderItemCard()
                {
                    SalesOrderId = item.SalesOrderId,
                    ItemCardId = item.ItemCardId,
                    Price = item.Price,
                    PriceAfterRate = item.PriceAfterRate,
                    QtyBeforRate = item.QtyBeforRate,
                    UnitId = item.UnitId,
                    UnitRate = item.UnitRate,
                    StoreId = item.StoreId,
                    StorePartId = item.StorePartId,
                    // اول معادله 

                    // المعادله التانيه

                    Quantity = item.QtyBeforRate * item.UnitRate,

                    // التعديل الثالث الخاص ب ال item Type

                    ItemType = itmType.ItemType,
                    DisAmount = item.DisAmount,
                    DisPercent = item.DisAmount > 0 ? (item.DisAmount / (item.Price * item.QtyBeforRate)) * 100 : 0,


                    // تعديلات يوم  لما دخلت مع مصطفي ميتنج الصبح 9/10


                    TaxesId1 = item.Detail_TaxesId1 == 0 ? null : item.Detail_TaxesId1,
                    TaxesId2 = item.Detail_TaxesId2 == 0 ? null : item.Detail_TaxesId2,
                    TaxesId3 = item.Detail_TaxesId3 == 0 ? null : item.Detail_TaxesId3,
                    Tax1IsAccomulative = item.Detail_TaxesId1 == 0 ? null : item.Tax1IsAccomulative,
                    Tax2IsAccomulative = item.Detail_TaxesId2 == 0 ? null : item.Tax2IsAccomulative,
                    Tax3IsAccomulative = item.Detail_TaxesId3 == 0 ? null : item.Tax3IsAccomulative,


                    Tax1Percent = item.Tax1Percent,
                    Tax2Percent = item.Tax2Percent,
                    Tax3Percent = item.Tax3Percent,
                   
                    IsCollection = item.IsCollection
                });
                OrderId = item.SalesOrderId;
            }
            _db.SaveChanges();
            OrderResultHeaderAndDetailDto res = new OrderResultHeaderAndDetailDto();
            res.id = OrderId;
            res.Message = "تم اضافه الداتا بنجاح";
            return Ok(res);
        }

       
        [HttpGet]
        public IEnumerable<getOrderHeaderDto> getOrderHeaderBycreatedBy(string createdBy)
        {
            return _itemsBll.GetOrderHeaderByCreatBy(createdBy);
        }

        [HttpGet]
        public IEnumerable<getOrderDetailDto> getOrderDetailBySalesOrdertemCardId(int SalesOrdertemCardId)
        {
            return _itemsBll.GetOrderDetailBySalesOrdertemCardId(SalesOrdertemCardId);
        }

        [HttpPost("{SalesOrderId}")]
        public async Task<IActionResult> UpdateOrderHeader(int SalesOrderId, [FromBody] UpdateOrderHeaderDto dto)
        {
            var updateDto = new updateHeaderDto();

            var Orderheader = await _db.MsSalesOrder.FindAsync(SalesOrderId);

            if (Orderheader == null) return NotFound(SalesOrderId);
            var empid = _db.GUsers.FirstOrDefault(x => x.UserId == Convert.ToInt32(dto.CreatedBy));


            Orderheader.InvTotal = dto.Header_InvTotal;
            Orderheader.UpdateAt = dto.Header_UpdateAt;
            Orderheader.DiscAmount = dto.Header_DiscAmount;
            Orderheader.DiscPercent = dto.Header_DiscPercent;
            Orderheader.DiscAmount2 = dto.Header_DiscAmount2;
            Orderheader.DiscPercent2 = dto.Header_DiscPercent2;
            Orderheader.TotalItemTax1 = dto.Header_TotalItemTax1;
            Orderheader.TotalItemTax2 = dto.Header_TotalItemTax2;
            Orderheader.TotalItemTax3 = dto.Header_TotalItemTax3;
            Orderheader.TaxValue1 = dto.Header_TaxValue1;
            Orderheader.TaxValue2 = dto.Header_TaxValue2;
            Orderheader.TaxValue3 = dto.Header_TaxValue3;
            Orderheader.TaxesId1 = dto.Header_TaxesId1 == 0 ? null : dto.Header_TaxesId1;
            Orderheader.TaxesId2 = dto.Header_TaxesId2 == 0 ? null : dto.Header_TaxesId2;
            Orderheader.TaxesId3 = dto.Header_TaxesId3 == 0 ? null : dto.Header_TaxesId3;
            Orderheader.PriceAfterTax = dto.Header_PriceAfterTax;
            //Orderheader.NetPrice = dto.Header_NetPrice;
            //Orderheader.PaidPrice = dto.Header_PaidPrice;
            Orderheader.PaidPriceVisa = dto.Header_PaidPriceVisa;
            Orderheader.Remarks = dto.Remarks;
            Orderheader.AddField3 = dto.AddField3;
            Orderheader.InvDueDate = dto.InvDueDate;
            Orderheader.StoreId = dto.StoreId;
            Orderheader.ExpenValue = dto.ExpenValue;
            //added on 1/8/2023
            Orderheader.InvoiceType = (byte?)dto.InvoiceType;
            Orderheader.TrDate= dto.TrDate;
            Orderheader.CreatedBy = dto.CreatedBy;
            Orderheader.EmpId = empid.EmpId;

            //5 - 12 - 2023
            //  التعديل الي حصل لما الابديت الخاص بالاهيدر حصل 

            Orderheader.NotPaid = dto.InvoiceType == 1 ? (dto.Header_PriceAfterTax + dto.ExpenValue) - dto.Header_PaidPriceVisa - dto.Header_PaidPrice : 0;
            Orderheader.PaidPrice = dto.InvoiceType == 0 ? (dto.Header_PriceAfterTax + dto.ExpenValue) - dto.Header_PaidPriceVisa : dto.Header_PaidPrice;
            Orderheader.NetPrice = dto.Header_PriceAfterTax + dto.ExpenValue;


            _db.SaveChanges();

            // التعديل الي كنت بمسح منه كل ال Detail الخاص بيه 
            //var detailRange = _db.MsSalesOrderItemCard.Where(inv => inv.SalesOrderId == Orderheader.SalesOrderId).ToList();
            //_db.MsSalesOrderItemCard.RemoveRange(detailRange);
            //_db.SaveChanges();

            updateDto.message = "update Successfully";
            updateDto.SalesOrderId = Orderheader.SalesOrderId;
            return Ok(updateDto);

        }

        [HttpPost]
        public IActionResult UpdateOrderDetail([FromBody] List<OrderDetailDto> dto)
        {
            foreach (OrderDetailDto item in dto)
            {
                MsItemCard itmType = _db.MsItemCard.Find(item.ItemCardId);

                MsSalesOrderItemCard GetOrderDetail 
                    = _db.MsSalesOrderItemCard.Where(p => p.SalesOrderId == item.SalesOrderId  && p.ItemCardId == item.ItemCardId && p.StorePartId == item.StorePartId).FirstOrDefault();
                MsItemPartition itemPartition = _db.MsItemPartition
                          .Where(p => p.ItemCardId == item.ItemCardId && p.StorePartId == item.StorePartId)
                          .FirstOrDefault();

    
                if (GetOrderDetail != null)
                {
                    // هنا انا جبت الكميه القديمه للمنتج قبل التعديل 
                    decimal? OldQuantity = GetOrderDetail.Quantity;


                    GetOrderDetail.SalesOrderId = item.SalesOrderId;
                    GetOrderDetail.ItemCardId = item.ItemCardId;
                    GetOrderDetail.Price = item.Price;
                    GetOrderDetail.PriceAfterRate = item.PriceAfterRate;
                    GetOrderDetail.QtyBeforRate = item.QtyBeforRate;
                    GetOrderDetail.UnitId = item.UnitId;
                    GetOrderDetail.UnitRate = item.UnitRate;
                    GetOrderDetail.StoreId = item.StoreId;
                    GetOrderDetail.StorePartId = item.StorePartId;
              
                
                // المعادله التانيه
                // فيها الكميه الجديده بعد التعديل  
                    GetOrderDetail.Quantity = item.QtyBeforRate * item.UnitRate;

                    decimal? NewQuantity = GetOrderDetail.Quantity;

                    if(OldQuantity > NewQuantity)
                    {
                        decimal? NewReservedQty = OldQuantity - NewQuantity;

                        if (itemPartition != null)
                        {

                            itemPartition.ReservedQty -= NewReservedQty;
                        }
                    }

                    if(OldQuantity < NewQuantity)
                    {
                        decimal? NewReservedQty = NewQuantity - OldQuantity;
                        if (itemPartition != null)
                        {
                            itemPartition.ReservedQty += NewReservedQty;
                        }

                    }

                    // التعديل الثالث الخاص ب ال item Type
                    GetOrderDetail.ItemType = itmType.ItemType;
                    GetOrderDetail.DisAmount = item.DisAmount;
                    GetOrderDetail.DisPercent = item.DisAmount > 0 ? (item.DisAmount / (item.Price * item.QtyBeforRate)) * 100 : 0;
                    GetOrderDetail.TaxesId1 = item.Detail_TaxesId1 == 0 ? null : item.Detail_TaxesId1;
                    GetOrderDetail.TaxesId2 = item.Detail_TaxesId2 == 0 ? null : item.Detail_TaxesId2;
                    GetOrderDetail.TaxesId3 = item.Detail_TaxesId3 == 0 ? null : item.Detail_TaxesId3;
                    GetOrderDetail.Tax1IsAccomulative = item.Detail_TaxesId1 == 0 ? null : item.Tax1IsAccomulative;
                    GetOrderDetail.Tax2IsAccomulative = item.Detail_TaxesId2 == 0 ? null : item.Tax2IsAccomulative;
                    GetOrderDetail.Tax3IsAccomulative = item.Detail_TaxesId3 == 0 ? null : item.Tax3IsAccomulative;
                    GetOrderDetail.Tax1Percent = item.Tax1Percent;
                    GetOrderDetail.Tax2Percent = item.Tax2Percent;
                    GetOrderDetail.Tax3Percent = item.Tax3Percent;
                    GetOrderDetail.IsCollection = item.IsCollection;
                    _db.SaveChanges();

                }
                else
                {
                    _db.MsSalesOrderItemCard.Add(new MsSalesOrderItemCard()
                    {
                        SalesOrderId = item.SalesOrderId,
                        ItemCardId = item.ItemCardId,
                        Price = item.Price,
                        PriceAfterRate = item.PriceAfterRate,
                        QtyBeforRate = item.QtyBeforRate,
                        UnitId = item.UnitId,
                        UnitRate = item.UnitRate,
                        StoreId = item.StoreId,
                        StorePartId = item.StorePartId,
                        // اول معادله 

                        // المعادله التانيه

                        Quantity = item.QtyBeforRate * item.UnitRate,

                        // التعديل الثالث الخاص ب ال item Type

                        ItemType = itmType.ItemType,
                        DisAmount = item.DisAmount,
                        DisPercent = item.DisAmount > 0 ? (item.DisAmount / (item.Price * item.QtyBeforRate)) * 100 : 0,


                        // تعديلات يوم  لما دخلت مع مصطفي ميتنج الصبح 9/10


                        TaxesId1 = item.Detail_TaxesId1 == 0 ? null : item.Detail_TaxesId1,
                        TaxesId2 = item.Detail_TaxesId2 == 0 ? null : item.Detail_TaxesId2,
                        TaxesId3 = item.Detail_TaxesId3 == 0 ? null : item.Detail_TaxesId3,
                        Tax1IsAccomulative = item.Detail_TaxesId1 == 0 ? null : item.Tax1IsAccomulative,
                        Tax2IsAccomulative = item.Detail_TaxesId2 == 0 ? null : item.Tax2IsAccomulative,
                        Tax3IsAccomulative = item.Detail_TaxesId3 == 0 ? null : item.Tax3IsAccomulative,


                        Tax1Percent = item.Tax1Percent,
                        Tax2Percent = item.Tax2Percent,
                        Tax3Percent = item.Tax3Percent,

                        IsCollection = item.IsCollection

                    });

                    _db.SaveChanges();
                }

            }
            var response = new { message = "تم تعديل الداتا بنجاح" };

            return Ok(response);
        }
             
        [HttpPost("{SalesOrderId},{UserId}")]
        public async Task<IActionResult> DeleteOrderHeader(int SalesOrderId, int UserId)
        {
            var deleteMessageDto = new DeleteDto();
            var header = await _db.MsSalesOrder.FindAsync(SalesOrderId);
            if (header == null) return NotFound(SalesOrderId);
            header.DeletedAt = DateTime.Now;
            header.DeletedBy = UserId.ToString();
            _db.SaveChanges();

            deleteMessageDto.message = "Data is Deleted Successfully";

            return Ok(deleteMessageDto);
        }

        [HttpPost]
        public IActionResult headerSalesOffer([FromBody] SalesOfferDto dto)
        {

            var book = from counter in _db.SysCounter
                       where counter.BookId == dto.BookId && counter.TrIdName == "SalesOfferId"
                       select counter.Counter;

            var empid = _db.GUsers.FirstOrDefault(x => x.UserId == Convert.ToInt32(dto.CreatedBy));


            var salesOffer = new MsSalesOffer
            {
                BookId = dto.BookId,
                TermId = dto.TermId,
                CurrencyId = dto.CurrencyId,
                CustomerId = dto.CustomerId,
                RectSourceType = (byte)dto.RectSourceType,
                Rate = dto.Rate,
                NotPaid = dto.NotPaid,
                CreatedBy = dto.CreatedBy,
                CreatedAt = dto.CreatedAt,
                InvTotal = dto.InvTotal,
                DiscAmount = dto.DiscAmount,
                DiscPercent = dto.DiscPercent,
                DiscAmount2 = dto.DiscAmount2,
                DiscPercent2 = dto.DiscPercent2,
                TotalItemTax1 = dto.TotalItemTax1,
                TotalItemTax2 = dto.TotalItemTax2,
                TotalItemTax3 = dto.TotalItemTax3,
                TaxValue1 = dto.TaxValue1,
                TaxValue2 = dto.TaxValue2,
                TaxValue3 = dto.TaxValue3,
                TaxesId1 = dto.TaxesId1,
                TaxesId2 = dto.TaxesId2,
                TaxesId3 = dto.TaxesId3,
                PriceAfterTax = dto.PriceAfterTax,
                NetPrice = dto.NetPrice,
                PaidPrice = dto.PaidPrice,
                PaidPriceVisa = dto.PaidPriceVisa,
                Remarks = dto.Remarks,
                AddField3 = dto.AddField3,
                InvDueDate = dto.InvDueDate,
                IsMobile = true,
                StoreId = dto.StoreId,
                ExpenValue = dto.ExpenValue,
                //added on 1/8/2023
                TrDate = dto.CreatedAt,
                InvoiceType = (byte?)dto.InvoiceType,
                EmpId = empid.EmpId
                
            };



            salesOffer.TrNo = ((int)(book).FirstOrDefault()) + 1;

            _db.MsSalesOffer.Add(salesOffer);
            _db.SaveChanges();

            var Counter = _db.SysCounter.FirstOrDefault(x => x.BookId == dto.BookId);
            Counter.Counter = salesOffer.TrNo;

            _db.SaveChanges();

            ResultSalesOfferDto res = new ResultSalesOfferDto();
            res.SalesOfferId = salesOffer.SalesOfferId;
            res.Message = "تم أضافه الداتا بنجاح";

            return Ok(res);

        }


        [HttpPost]
        public IActionResult CreateSalesOfferrDetail([FromBody] List<SalesOfferrDetailDto> dto)
        {
            var OrderId = 0;
            foreach (var item in dto)
            {
                var itmType = _db.MsItemCard.Find(item.ItemCardId);

                _db.MsSalesOfferItemCard.Add(new MsSalesOfferItemCard()
                {
                    SalesOfferId = item.SalesOfferId,
                    ItemCardId = item.ItemCardId,
                    Price = item.Price,
                    PriceAfterRate = item.PriceAfterRate,
                    QtyBeforRate = item.QtyBeforRate,
                    UnitId = item.UnitId,
                    UnitRate = item.UnitRate,
                    StoreId = item.StoreId,
                    StorePartId = item.StorePartId,
                    DisAmount = item.DisAmount,
                    // اول معادله 

                    DisPercent = item.DisAmount > 0 ? (item.DisAmount / (item.Price * item.QtyBeforRate)) * 100 : 0,
                    // المعادله التانيه

                    Quantity = item.QtyBeforRate * item.UnitRate,

                    // التعديل الثالث الخاص ب ال item Type

                    ItemType = itmType.ItemType,
                    Tax1Percent = item.Tax1Percent,
                    Tax2Percent = item.Tax2Percent,
                    Tax3Percent = item.Tax3Percent,
                    TaxesId1 = item.Detail_TaxesId1,
                    TaxesId2 = item.Detail_TaxesId2,
                    TaxesId3 = item.Detail_TaxesId3,
                    Tax1IsAccomulative = item.Tax1IsAccomulative,
                    Tax2IsAccomulative = item.Tax2IsAccomulative,
                    Tax3IsAccomulative = item.Tax3IsAccomulative,
                    IsCollection = item.IsCollection
                });
                OrderId = item.SalesOfferId;
            }
            _db.SaveChanges();
            OrderResultHeaderAndDetailDto res = new OrderResultHeaderAndDetailDto();
            res.id = OrderId;
            res.Message = "تم اضافه الداتا بنجاح";
            return Ok(res);
        }

        [HttpGet]
        public IEnumerable<getheaderSalesOfferDto> getHeaderSalesOfferBycreatedBy(string createdBy)
        {
            return _itemsBll.getheaderSalesOfferByCreatBy(createdBy);
        }

        [HttpGet]
        public IEnumerable<getSalesOfferDetailDto> getDetailSalesOfferBySalesOffertemCardId(int SalesOffertemCardId)
        {
            return _itemsBll.GetSalesOfferDetailBySalesOffertemCardId(SalesOffertemCardId);
        }

        [HttpPut("{SalesOfferId}")]
        public async Task<IActionResult> UpdateSalesOffer(int SalesOfferId, [FromBody] UpdateOrderHeaderDto dto)
        {
            var updateDto = new updateHeaderDto();
            var Orderheader = await _db.MsSalesOffer.FindAsync(SalesOfferId);
            if (Orderheader == null) return NotFound(SalesOfferId);
            var empid = _db.GUsers.FirstOrDefault(x => x.UserId == Convert.ToInt32(dto.CreatedBy));


            Orderheader.InvTotal = dto.Header_InvTotal;
            Orderheader.UpdateAt = dto.Header_UpdateAt;
            Orderheader.DiscAmount = dto.Header_DiscAmount;
            Orderheader.DiscPercent = dto.Header_DiscPercent;
            Orderheader.DiscAmount2 = dto.Header_DiscAmount2;
            Orderheader.DiscPercent2 = dto.Header_DiscPercent2;
            Orderheader.TotalItemTax1 = dto.Header_TotalItemTax1;
            Orderheader.TotalItemTax2 = dto.Header_TotalItemTax2;
            Orderheader.TotalItemTax3 = dto.Header_TotalItemTax3;
            Orderheader.TaxValue1 = dto.Header_TaxValue1;
            Orderheader.TaxValue2 = dto.Header_TaxValue2;
            Orderheader.TaxValue3 = dto.Header_TaxValue3;
            Orderheader.TaxesId1 = dto.Header_TaxesId1;
            Orderheader.TaxesId2 = dto.Header_TaxesId2;
            Orderheader.TaxesId3 = dto.Header_TaxesId3;
            Orderheader.PriceAfterTax = dto.Header_PriceAfterTax;
            Orderheader.NetPrice = dto.Header_NetPrice;
            Orderheader.PaidPrice = dto.Header_PaidPrice;
            Orderheader.PaidPriceVisa = dto.Header_PaidPriceVisa;
            Orderheader.Remarks = dto.Remarks;
            Orderheader.AddField3 = dto.AddField3;
            Orderheader.InvDueDate = dto.InvDueDate;
            Orderheader.StoreId = dto.StoreId;
            Orderheader.ExpenValue = dto.ExpenValue;
            //added on 1/8/2023
            Orderheader.CreatedBy = dto.CreatedBy;
            Orderheader.InvoiceType = (byte?)dto.InvoiceType;
            Orderheader.TrDate = dto.TrDate;
            Orderheader.EmpId = empid.EmpId;
                
            

            _db.SaveChanges();

            var detailRange = _db.MsSalesOfferItemCard.Where(inv => inv.SalesOfferId == Orderheader.SalesOfferId).ToList();
            _db.MsSalesOfferItemCard.RemoveRange(detailRange);
            _db.SaveChanges();

            updateDto.message = "update Successfully";
            updateDto.SalesOfferId = Orderheader.SalesOfferId;
            return Ok(updateDto);

        }

        [HttpDelete("{SalesOfferId},{UserId}")]
        public async Task<IActionResult> DeleteSalesOfferHeader(int SalesOfferId, int UserId)
        {
            var deleteMessageDto = new DeleteDto();
            var header = await _db.MsSalesOffer.FindAsync(SalesOfferId);
            if (header == null) return NotFound(SalesOfferId);
            header.DeletedAt = DateTime.Now;
            header.DeletedBy = UserId.ToString();
            _db.SaveChanges();

            deleteMessageDto.message = "Data is Deleted Successfully";

            return Ok(deleteMessageDto);
        }

        [HttpGet("getAllBills")]
        public async Task<IActionResult> GetAllBills(int pageNumber = 1, int pageSize = 10,
             string DocTrNo = null, string CustomerCode = null)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            var query = _db.SalesInvCustSearch.AsQueryable();

          

            if (!string.IsNullOrWhiteSpace(CustomerCode))
            {
                query = query.Where(bill => bill.CustomerCode == CustomerCode && bill.CustomerCode.Length == CustomerCode.Length);
            }

            if (!string.IsNullOrWhiteSpace(DocTrNo))
            {
                query = query.Where(bill => bill.DocTrNo.Contains(DocTrNo));
            }

            //if (!string.IsNullOrWhiteSpace(DocTrNo))
            //{
            //    query = query.Where(bill => bill.DocTrNo == DocTrNo);
            //}


            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }
            var bills = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var response = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = bills
            };

            return Ok(response);
        }

        //[HttpGet("GetInvoice")]
        // public async Task<IActionResult> GetInvoice(int invid)
        // {
        //     // استخراج الأعمدة التي تريدها من الجدول
        //     var data = await _db.MsSalesInvoiceItemCard
        //         .Where(items => items.InvId == invid)
        //        .Join(_db.MsItemCard, // الجدول الثاني الذي تريد الانضمام إليه
        //         item => item.ItemCardId, // مفتاح الانضمام من الجدول الأول
        //         msitem => msitem.ItemCardId, // مفتاح الانضمام من الجدول الثاني
        //        (item, msitem) => new // مشروع لاسترداد البيانات المطلوبة من الانضمام
        //       {
        //             item.PriceAfterRate,
        //             item.ItemCardId,
        //             msitem.ItemDescA,
        //             item.Price,
        //             item.QtyBeforRate,
        //             item.UnitId,
        //             item.UnitRate,
        //             item.StoreId,
        //             item.StorePartId,
        //             item.Quantity,
        //             item.ItemType,
        //             item.DisAmount,
        //             item.DisPercent

        //         })
        //         .ToListAsync(); 

        //     return Ok(data); // ترجيع البيانات كاستجابة HTTP معروفة
        // }

        [HttpGet("GetInvoice")]
        public async Task<IActionResult> GetInvoice(int invid)
        {
            var data = await (from item in _db.MsSalesInvoiceItemCard
                              where item.InvId == invid
                              join msitem in _db.MsItemCard on item.ItemCardId equals msitem.ItemCardId
                              join unit in _db.MsItemUnit on item.ItemCardId equals unit.ItemCardId into unitJoin
                              from unit in unitJoin.DefaultIfEmpty() // Left Join
                              select new
                              {
                                  item.PriceAfterRate,
                                  item.ItemCardId,
                                  msitem.ItemDescA,
                                  item.Price,
                                  item.QtyBeforRate,
                                  item.UnitId,
                                  item.UnitRate,
                                  item.StoreId,
                                  item.StorePartId,
                                  item.Quantity,
                                  item.ItemType,
                                  item.DisAmount,
                                  item.DisPercent,
                                  UnitName = unit.UnitNam
                              })
            .ToListAsync();

            return Ok(data);
        }

        [HttpPost, AllowAnonymous]
        public IActionResult ReturnRequestHeader([FromBody] HeaderDto dto)
        {

            var book = from counter in _db.SysCounter
                       where counter.BookId == dto.Header_BookId && counter.TrIdName == "ReqsalesId"
                       select counter.Counter;

            var empid = _db.GUsers.FirstOrDefault(x => x.UserId == Convert.ToInt32(dto.Header_CreatedBy));

           
            var ReturnSalesReq = new MsReturnSalesReq
            {
                BookId = dto.Header_BookId,
                TermId = dto.Header_TermId,
                CurrencyId = dto.Header_CurrencyId,
                CustomerId = dto.Header_CustomerId,
                RectSourceType = (byte)dto.Header_RectSourceType,
                Rate = dto.Header_Rate,
                NotPaid = dto.NotPaid,
                CreatedBy = dto.Header_CreatedBy,
                CreatedAt = dto.Header_CreatedAt,
                InvTotal = dto.Header_InvTotal,
                DiscAmount = dto.Header_DiscAmount,
                DiscPercent = dto.Header_DiscPercent,
                DiscAmount2 = dto.Header_DiscAmount2,
                DiscPercent2 = dto.Header_DiscPercent2,
                TotalItemTax1 = dto.Header_TotalItemTax1,
                TotalItemTax2 = dto.Header_TotalItemTax2,
                TotalItemTax3 = dto.Header_TotalItemTax3,
                TaxValue1 = dto.Header_TaxValue1,
                TaxValue2 = dto.Header_TaxValue2,
                TaxValue3 = dto.Header_TaxValue3,
                TaxesId1 = dto.Header_TaxesId1,
                TaxesId2 = dto.Header_TaxesId2,
                TaxesId3 = dto.Header_TaxesId3,
                PriceAfterTax = dto.Header_PriceAfterTax,
                NetPrice = dto.Header_NetPrice,
                PaidPrice = dto.Header_PaidPrice,
                PaidPriceVisa = dto.Header_PaidPriceVisa,
                BankTransfer = dto.Header_BankTransfer,
                //  added on 11-6-2023
                Remarks = dto.Remarks,
                AddField3 = dto.AddField3,
                InvDueDate = dto.InvDueDate,
                IsMobile = true,
                StoreId = dto.StoreId,
                ExpenValue = dto.ExpenValue,
                //  added on 1-8-2023
                InvoiceType = (byte?)dto.InvoiceType,
                TrDate = dto.Header_CreatedAt,
                EmpId = empid.EmpId


            };

          //  string prefixCode = _db.SysBooks
          //.Where(book => book.BookId == )
          //.Select(book => book.PrefixCode)
          //.FirstOrDefault();


            ReturnSalesReq.TrNo = ((int)(book).FirstOrDefault()) + 1;



            _db.MsReturnSalesReq.Add(ReturnSalesReq);
            _db.SaveChanges();

            var Counter = _db.SysCounter.FirstOrDefault(x => x.BookId == dto.Header_BookId);
            Counter.Counter = ReturnSalesReq.TrNo;

            _db.SaveChanges();

            var response = new
            {
                InvReqsalesId = ReturnSalesReq.ReqsalesId,
                Message = "تم أضافه الداتا بنجاح",
             };
            return Ok(response);

        }

        [HttpGet("getAllDetail")]
        public IActionResult getAllDetail()
        {
            return Ok(_db.MsReturnSalesReqItemCard.ToList());
        }

        [HttpPost]
        public IActionResult ReturnRequestDetail([FromBody] List<DetailReturnReqDto> dto)
        {


            var reqsalesId = 0;

            foreach (var item in dto)
            {
                var itmType = _db.MsItemCard.Find(item.ItemCardId);

                _db.MsReturnSalesReqItemCard.Add(new MsReturnSalesReqItemCard()
                {
                    ReqsalesId = item.ReqsalesId,
                    ItemCardId = item.ItemCardId,
                    Price = item.Price,
                    //PriceAfterRate = item.PriceAfterRate,
                    QtyBeforRate = item.QtyBeforRate,
                    // في حاله ال item is serial
                    //Quantity = item.Quantity,
                    UnitId = item.UnitId,
                    UnitRate = item.UnitRate,
                    StoreId = item.StoreId,
                    StorePartId = item.StorePartId,
                    DisAmount = item.DisAmount,
                    // اول معادله 

                    DisPercent = item.DisAmount > 0 ? (item.DisAmount / (item.Price * item.QtyBeforRate)) * 100 : 0,
                    // المعادله التانيه

                    Quantity = item.QtyBeforRate * item.UnitRate,

                    // التعديل الثالث الخاص ب ال item Type

                    ItemType = itmType.ItemType,

                    Tax1Percent = item.Tax1Percent,
                    Tax2Percent = item.Tax2Percent,
                    Tax3Percent = item.Tax3Percent,
                    TaxesId1 = item.Detail_TaxesId1,
                    TaxesId2 = item.Detail_TaxesId2,
                    TaxesId3 = item.Detail_TaxesId3,
                    Tax1IsAccomulative = item.Tax1IsAccomulative,
                    Tax2IsAccomulative = item.Tax2IsAccomulative,
                    Tax3IsAccomulative = item.Tax3IsAccomulative,
                    IsCollection = item.IsCollection
                });
                reqsalesId = item.ReqsalesId;
            }
            _db.SaveChanges();
            //ResultHeaderAndDetialDto res = new ResultHeaderAndDetialDto();
            //res.Invid = reqsalesId;
            //res.Message = 

            var response = new
            {
                ReqsalesId = reqsalesId,
                Message = "تم أضافه الداتا بنجاح"
            };
            return Ok(response);
        }



        [HttpPost("ReturnedInovice")]
        public async Task<IActionResult> ReturnedInovice(List<QuantityUpdate> updates)
        {
            try
            {
                if (updates.Any(update => update == null || update.Invid == 0 || update.ItemCardId == 0 || update.ReturnedQuantity == 0))
                {

                    var res = new
                    {
                        Message = "يجب ملء جميع القيم في القائمة ."
                    };

                    return BadRequest(res);
                }

                var book = from counter in _db.SysCounter
                           where counter.TrIdName == "ReqsalesId"
                           select counter.Counter;


                var itemcardids = updates.Select(update => update.ItemCardId).ToList();

                var invides = updates.Select(update => update.Invid).ToList();

                var MsSalesInvoiceheader = await _db.MsSalesInvoice.Where(item => invides.Contains(item.InvId)).ToListAsync();

                var existingDetail = await _db.MsReturnSalesReqItemCard
                .Where(req => invides.Contains((int)req.InvId) && itemcardids.Contains((int)req.ItemCardId))
                .Select(req => new { req.InvId, req.ItemCardId, req.StorePartId })
                .ToListAsync();

                var detail = await _db.MsSalesInvoiceItemCard.Where(item => invides.Contains((int)item.InvId) && itemcardids.Contains((int)item.ItemCardId))
               .ToListAsync();

                //var DetailReturnSalesReqs = new List<MsReturnSalesReqItemCard>();


                //var ArrayReturnSalesReqs = new List<MsReturnSalesReq>();


                foreach (var salesInvoice in MsSalesInvoiceheader)
                {

                    var returnSalesReq = new MsReturnSalesReq
                    {
                        InvId = salesInvoice.InvId,
                        CustomerId = salesInvoice.CustomerId,
                        StoreId = salesInvoice.StoreId,
                        CurrencyId = salesInvoice.CurrencyId,
                        BookId = salesInvoice.BookId,
                        TermId = salesInvoice.TermId,
                        Rate = salesInvoice.Rate,
                        NotPaid = salesInvoice.NotPaid,
                        CreatedBy = salesInvoice.CreatedBy,
                        CreatedAt = salesInvoice.CreatedAt,
                        InvTotal = salesInvoice.InvTotal,
                        EmpId = salesInvoice.EmpId,
                        TrDate = salesInvoice.CreatedAt,
                        InvoiceType = salesInvoice.InvoiceType,
                    };

                    //string prefixCode = _db.SysBooks
                    // .Where(book => book.BookId == salesInvoice.BookId)
                    // .Select(book => book.PrefixCode)
                    // .FirstOrDefault();

                    returnSalesReq.TrNo = ((int)(book).FirstOrDefault()) + 1;

                    var Counter = _db.SysCounter.FirstOrDefault(x => x.TrIdName == "ReqsalesId");
                    Counter.Counter = returnSalesReq.TrNo;

                    _db.MsReturnSalesReq.Add(returnSalesReq);
                    await _db.SaveChangesAsync(); // حفظ التغييرات في قاعدة البيانات

                    foreach (var detailSalesReq in detail)
                    {
                        if (!existingDetail.Any(d => d.InvId == detailSalesReq.InvId && d.ItemCardId == detailSalesReq.ItemCardId && d.StorePartId == detailSalesReq.StorePartId))
                        {
                            var detailData = new MsReturnSalesReqItemCard
                            {
                                ReqsalesId = returnSalesReq.ReqsalesId,
                                InvId = detailSalesReq.InvId,
                                ItemCardId = detailSalesReq.ItemCardId,
                                Price = detailSalesReq.Price,
                                ServicePrice = detailSalesReq.Price,
                                QtyBeforRate = detailSalesReq.QtyBeforRate,
                                UnitId = detailSalesReq.UnitId,
                                UnitRate = detailSalesReq.UnitRate,
                                StoreId = detailSalesReq.StoreId,
                                StorePartId = detailSalesReq.StorePartId,
                                DisAmount = detailSalesReq.DisAmount,
                                DisPercent = detailSalesReq.DisPercent,
                                Quantity = detailSalesReq.Quantity,
                                ItemType = detailSalesReq.ItemType,
                            };

                            _db.MsReturnSalesReqItemCard.Add(detailData);
                            await _db.SaveChangesAsync();
                        }
                    }

                   

                }

                var recordsToUpdate = await _db.MsReturnSalesReqItemCard
                    .Where(item => itemcardids.Contains((int)item.ItemCardId) && invides.Contains((int)item.InvId))
                    .ToListAsync();

                foreach (var update in updates)
                {
                    var record = recordsToUpdate.FirstOrDefault(item => item.ItemCardId == update.ItemCardId && item.InvId == update.Invid && recordsToUpdate.Any(rt => rt.StorePartId == item.StorePartId));

                    if (record != null)
                    {
                        record.ReturnedQuantity = record.ReturnedQuantity ?? 0;

                        record.ReturnedQuantity += update.ReturnedQuantity;

                        record.Remarks = update.note;

                    };

                    if (record.Quantity < record.ReturnedQuantity)
                    {

                        record.ReturnedQuantity = record.Quantity;

                    }

                    if (update.UpdatePrice != 0 )
                    {
                        record.Price = update.UpdatePrice;
                    }

                   

                }

                await _db.SaveChangesAsync();

                var response = new { Success = "تم اضافه طلب المرتجع بنجاح" };
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, "حدث خطأ أثناء اضافه طلب المرتجع الكميات");
            }
        }

        // دي الفانكشن المكتوبه بشكل صحيح 

        //[HttpPost("ReturnedInovice")]
        //public async Task<IActionResult> ReturnedInovice(List<QuantityUpdate> updates)
        //{
        //    try
        //    {
        //        if (updates.Any(update => update == null || update.Invid == 0 || update.ItemCardId == 0 || update.ReturnedQuantity == 0))
        //        {

        //            var res = new
        //            {
        //                Message = "يجب ملء جميع القيم في القائمة ."
        //            };

        //             return BadRequest(res);
        //        }

        //        var book = from counter in _db.SysCounter
        //                   where  counter.TrIdName == "ReqsalesId"
        //                   select counter.Counter;


        //        var itemcardids = updates.Select(update => update.ItemCardId).ToList();

        //        var invides = updates.Select(update => update.Invid).ToList();

        //        var MsSalesInvoiceheader = await _db.MsSalesInvoice.Where(item => invides.Contains(item.InvId)).ToListAsync();

        //        var ArrayReturnSalesReqs = new List<MsReturnSalesReq>();


        //        foreach (var salesInvoice in MsSalesInvoiceheader)
        //        {

        //                var returnSalesReq = new MsReturnSalesReq
        //                {
        //                    InvId = salesInvoice.InvId,
        //                    CustomerId = salesInvoice.CustomerId,
        //                    StoreId = salesInvoice.StoreId,
        //                    CurrencyId = salesInvoice.CurrencyId,
        //                    BookId = salesInvoice.BookId,
        //                    TermId = salesInvoice.TermId,
        //                    Rate = salesInvoice.Rate,
        //                    NotPaid = salesInvoice.NotPaid,
        //                    CreatedBy = salesInvoice.CreatedBy,
        //                    CreatedAt = salesInvoice.CreatedAt,
        //                    InvTotal = salesInvoice.InvTotal,
        //                    EmpId = salesInvoice.EmpId,
        //                    TrDate = salesInvoice.CreatedAt,
        //                    InvoiceType = salesInvoice.InvoiceType,
        //                };

        //            returnSalesReq.TrNo = ((int)(book).FirstOrDefault()) + 1;

        //            var Counter = _db.SysCounter.FirstOrDefault(x => x.TrIdName == "ReqsalesId");
        //            Counter.Counter = returnSalesReq.TrNo;


        //            ArrayReturnSalesReqs.Add(returnSalesReq);

        //        }

        //        _db.MsReturnSalesReq.AddRange(ArrayReturnSalesReqs);
        //        await _db.SaveChangesAsync(); // حفظ التغييرات في قاعدة البيانات


        //        var detail = await _db.MsSalesInvoiceItemCard.Where(item => invides.Contains((int)item.InvId) && itemcardids.Contains((int)item.ItemCardId))
        //         .ToListAsync();


        //        var existingDetail = await _db.MsReturnSalesReqItemCard
        //            .Where(req => invides.Contains((int)req.InvId) && itemcardids.Contains((int)req.ItemCardId))
        //            .Select(req => new { req.InvId, req.ItemCardId,req.StorePartId})
        //            .ToListAsync();

        //        var DetailReturnSalesReqs = new List<MsReturnSalesReqItemCard>();

        //        foreach (var detailSalesReq in detail)
        //        {
        //            if (!existingDetail.Any(d => d.InvId == detailSalesReq.InvId && d.ItemCardId == detailSalesReq.ItemCardId && d.StorePartId == detailSalesReq.StorePartId))
        //            {
        //                var detailData = new MsReturnSalesReqItemCard
        //                {
        //                    InvId = detailSalesReq.InvId,
        //                    ItemCardId = detailSalesReq.ItemCardId,
        //                    Price = detailSalesReq.Price,
        //                    QtyBeforRate = detailSalesReq.QtyBeforRate,
        //                    UnitId = detailSalesReq.UnitId,
        //                    UnitRate = detailSalesReq.UnitRate,
        //                    StoreId = detailSalesReq.StoreId,
        //                    StorePartId = detailSalesReq.StorePartId,
        //                    DisAmount = detailSalesReq.DisAmount,
        //                    DisPercent = detailSalesReq.DisPercent,
        //                    Quantity = detailSalesReq.Quantity,
        //                    ItemType = detailSalesReq.ItemType,
        //                };
        //                DetailReturnSalesReqs.Add(detailData);
        //            }                      
        //        }

        //        _db.MsReturnSalesReqItemCard.AddRange(DetailReturnSalesReqs);
        //        await _db.SaveChangesAsync();


        //        var recordsToUpdate = await _db.MsReturnSalesReqItemCard
        //            .Where(item => itemcardids.Contains((int)item.ItemCardId) && invides.Contains((int)item.InvId))
        //            .ToListAsync();

        //        foreach (var update in updates)
        //        {
        //            var record = recordsToUpdate.FirstOrDefault(item => item.ItemCardId == update.ItemCardId && item.InvId == update.Invid);

        //            if (record != null)
        //            {
        //                record.ReturnedQuantity = record.ReturnedQuantity ?? 0;

        //                record.ReturnedQuantity += update.ReturnedQuantity;

        //            };

        //            if (record.Quantity < record.ReturnedQuantity)
        //            {

        //                record.ReturnedQuantity = record.Quantity;

        //            }
        //        }

        //        await _db.SaveChangesAsync();

        //        var response = new { Success = "تم اضافه طلب المرتجع بنجاح" };
        //        return Ok(response);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, "حدث خطأ أثناء اضافه طلب المرتجع الكميات");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> GetReturnReuestDeatail (int reqId)
        {
            var data =  from reqDetail in _db.MsReturnSalesReqItemCard
                       join msitemCard in _db.MsItemCard
                       on reqDetail.ItemCardId equals msitemCard.ItemCardId
                       where reqDetail.ReqsalesId == reqId
                       select new
                       {
                           reqDetail.ItemCardId,
                           msitemCard.ItemDescA,
                           reqDetail.ReqsalesId,
                           reqDetail.Price,
                           reqDetail.ServicePrice,
                           reqDetail.Quantity,
                           reqDetail.ReturnedQuantity,
                           reqDetail.InvId,
                           reqDetail.StoreId,
                           reqDetail.StorePartId,
                           reqDetail.Remarks,

                       };

            var reusltData = await data.ToListAsync();

            return Ok(reusltData);
        }


        [HttpGet("GetAllReturnInvoiceRequest")]
        public async Task<IActionResult> GetAllReturnInvoiceRequest(int pageNumber = 1, int pageSize = 10,
            string DocTrNo = null, string CustomerCode = null)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            var query = _db.ReturnSalesReqSearch.AsQueryable();



            if (!string.IsNullOrWhiteSpace(CustomerCode))
            {
                query = query.Where(bill => bill.CustomerCode == CustomerCode && bill.CustomerCode.Length == CustomerCode.Length);
            }

            if (!string.IsNullOrWhiteSpace(DocTrNo))
            {
                //query = query.Where(bill => bill.DocTrNo == DocTrNo);
                query = query.Where(bill => bill.DocTrNo.Contains(DocTrNo));

            }


            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }
            var bills = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var response = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = bills
            };

            return Ok(response);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllPartition()
        //{
        //    var SelectPartiton =  _db.MsPartition.Select(p => new
        //    {
        //        itempardId = p.ite
        //    })
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllPartition()
        {
            // اختيار الأعمدة المطلوبة
            var selectedColumns = await _db.MsPartition
                .Select(p => new
                {
                    StorePartId = p.StorePartId,
                    PartDescA = p.PartDescA,
                })
                .ToListAsync();

            return Ok(selectedColumns);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllStores()
        {
            // اختيار الأعمدة المطلوبة
            var selectedColumns = await _db.MsStores
                .Select(p => new
                {
                    StoreId = p.StoreId,
                    StoreDescA = p.StoreDescA,
                })
                .ToListAsync();

            return Ok(selectedColumns);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var cat = await _db.MsItemCategory.Select(c => new
            {
                ItemCategoryId = c.ItemCategoryId,
                ItemCatDescA = c.ItemCatDescA,
            })
                .ToListAsync();

            return Ok(cat);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductsTypes()
        {
            var productType = await _db.SrProductsTypes.Select(c => new
            {
                ProductTypeId = c.ProductTypeId,
                DescA = c.DescA

            }).ToListAsync();

            return Ok(productType);
        }

        [HttpGet]
        public async Task<IActionResult> GradeTypeCeramic()
        {
            var type = await _db.PrintGradeType.Select(t => new
            {
                GradId = t.GradId,
                DescA = t.DescA
            }).ToListAsync();

            return Ok(type);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBrands()
        {
            var brand = await _db.SrBrands.Select(b => new
            {
                BrandId = b.BrandId,
                DescA = b.DescA

            }).ToListAsync();

            return Ok(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Print_CoverType()
        {
            var colorType = await _db.PrintCoverType.Select(p => new
            {
                CoverTypeId = p.CoverTypeId,
                DescA = p.DescA
            }).ToListAsync();

            return Ok(colorType);
        }

        [HttpGet]
        public async Task<IActionResult> Print_SizeType()
        {
            var sizeType = await _db.PrintSizeType.Select(s => new
            {
                SizeTypeId = s.SizeTypeId,
                DescA = s.DescA
            }).ToListAsync();

            return Ok(sizeType);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult>  GetItemsWithFilter(int pageNumber = 1, int pageSize = 10,
            int? storepartId = null, int? itemCategoryId = null,int? StoreId = null , int? ProductTypeId = null,
            int? GradId = null , int? BrandId = null , int? CoverTypeId = null, int? SizeTypeId = null)
        {
           

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            var query = (
                             from item in _db.MsItemCard
                             join img in _db.MsItemImages
                             on item.ItemCardId equals img.ItemCardId into itemImages
                             from img in itemImages.DefaultIfEmpty()
                             join partition in _db.MsItemPartition
                             on item.ItemCardId equals partition.ItemCardId into itemPartitions
                             from partition in itemPartitions.DefaultIfEmpty()

                             select new
                             {
                                 ItemCardId = item.ItemCardId,
                                 storepartId = partition.StorePartId,
                                 ItemCategoryId = item.ItemCategoryId,
                                 StoreId = item.StoreId,
                                 GradId = item.GradId,
                                 CoverTypeId = item.CoverTypeId,
                                 SizeTypeId = item.SizeTypeId,
                                 BrandId = item.BrandId,
                                 ItemDescA = item.ItemDescA,
                                 ItemDescE = item.ItemDescE,
                                 ItemCode = item.ItemCode,
                                 firstPrice = item.FirstPrice,
                                 SecandPrice = item.SecandPrice,
                                 ThirdPrice = item.ThirdPrice,
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

                                 // QtyInNotebook = _db.MsItemPartition.
                                 // Where(t => t.ItemCardId == item.ItemCardId && t.StorePartId == item.StorePartId)
                                 //.Select(p => p.QtyInNotebook).FirstOrDefault(),

                                 QtyInNotebook = partition.QtyInNotebook,

                                 IsCollection = (bool)item.IsCollection,
                                 ProductTypeId = (int)item.ProductTypeId,
                                 ItemColor = item.ItemColor,
                                 imagePath = img.ImgPath
                             });

            if (storepartId.HasValue)
            {
                query = query.Where(q => q.storepartId == storepartId);
            }

            if (itemCategoryId.HasValue)
            {
                query = query.Where(q => q.ItemCategoryId == itemCategoryId);
            }

            if (StoreId.HasValue)
            {
                query = query.Where(q => q.StoreId == StoreId);
            }

            if (ProductTypeId.HasValue)
            {
                query = query.Where(p => p.ProductTypeId == ProductTypeId);
            }

            if (GradId.HasValue)
            {
                query = query.Where(g => g.GradId == GradId);
            }

            if (BrandId.HasValue)
            {
                query = query.Where(b => b.BrandId == BrandId);
            }

            if (CoverTypeId.HasValue)
            {
                query = query.Where(q=> q.CoverTypeId == CoverTypeId);
            }

            if(SizeTypeId.HasValue)
            {
                query = query.Where(s => s.SizeTypeId == SizeTypeId);
            }
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            var bills = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var response = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = bills
            };

            return Ok(response);

        }
        //    [HttpPost("ProductQuantityOut")]
        //    public async Task<IActionResult> ProductQuantityOut(List<QuantityUpdate> updates)
        //    {
        //        try
        //        {

        //            if (updates.Any(update => update == null || update.Invid == 0 || update.ItemCardId == 0 || update.ReturnedQuantity == 0))
        //            {

        //                var res = new
        //                {
        //                    Message = "يجب ملء جميع القيم في القائمة ."
        //                };

        //                return BadRequest(res);
        //            }

        //            var itemcardids = updates.Select(update => update.ItemCardId).ToList();

        //            var invides = updates.Select(update => update.Invid).ToList();

        //            var MsSalesInvoiceheader = await _db.MsSalesInvoice.Where(item => invides.Contains(item.InvId)).ToListAsync();

        //            var detail = await _db.MsSalesInvoiceItemCard.Where(item => invides.Contains((int)item.InvId) && itemcardids.Contains((int)item.ItemCardId))
        //            .ToListAsync();


        //            foreach (var salesInvoice in MsSalesInvoiceheader)
        //            {
        //                var returnSalesReq = new MsReturnSalesReq
        //                {
        //                    InvId = salesInvoice.InvId,
        //                    CustomerId = salesInvoice.CustomerId,
        //                    StoreId = salesInvoice.StoreId,
        //                    CurrencyId = salesInvoice.CurrencyId,
        //                    BookId = salesInvoice.BookId,
        //                    TermId = salesInvoice.TermId,
        //                    Rate = salesInvoice.Rate,
        //                    NotPaid = salesInvoice.NotPaid,
        //                    CreatedBy = salesInvoice.CreatedBy,
        //                    CreatedAt = salesInvoice.CreatedAt,
        //                    InvTotal = salesInvoice.InvTotal,
        //                    EmpId = salesInvoice.EmpId,
        //                    TrDate = salesInvoice.CreatedAt,
        //                    InvoiceType = salesInvoice.InvoiceType,
        //                };
        //                _db.MsReturnSalesReq.Add(returnSalesReq);
        //                await _db.SaveChangesAsync(); // حفظ التغييرات في قاعدة البيانات

        //                foreach (var detailSalesReq in detail)
        //                {
        //                    var detailData = new MsReturnSalesReqItemCard
        //                    {
        //                        ReqsalesId = returnSalesReq.ReqsalesId,
        //                        InvId = detailSalesReq.InvId,
        //                        ItemCardId = detailSalesReq.ItemCardId,
        //                        Price = detailSalesReq.Price,
        //                        QtyBeforRate = detailSalesReq.QtyBeforRate,
        //                        UnitId = detailSalesReq.UnitId,
        //                        UnitRate = detailSalesReq.UnitRate,
        //                        StoreId = detailSalesReq.StoreId,
        //                        StorePartId = detailSalesReq.StorePartId,
        //                        DisAmount = detailSalesReq.DisAmount,
        //                        DisPercent = detailSalesReq.DisPercent,
        //                        Quantity = detailSalesReq.Quantity,
        //                        ItemType = detailSalesReq.ItemType,
        //                    };

        //                    _db.MsReturnSalesReqItemCard.Add(detailData);
        //                    await _db.SaveChangesAsync();
        //                }

        //            }




        //            var recordsToUpdate = await _db.MsReturnSalesReqItemCard
        //                .Where(item => itemcardids.Contains((int)item.ItemCardId) && invides.Contains((int)item.InvId))
        //                .ToListAsync();

        //            foreach (var update in updates)
        //            {
        //                var record = recordsToUpdate.FirstOrDefault(item => item.ItemCardId == update.ItemCardId);
        //                if (record != null)
        //                {
        //                    record.ReturnedQuantity = update.ReturnedQuantity;
        //                }
        //            }

        //            await _db.SaveChangesAsync();

        //            var response = new { Success = "تم اضافه طلب المرتجع بنجاح" };
        //            return Ok(response);
        //        }
        //        catch (Exception)
        //        {
        //            return StatusCode(500, "حدث خطأ أثناء اضافه طلب المرتجع الكميات");
        //        }
        //    }

    }

}
