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
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Static.Helper;
using Static.VM;

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

            var detailRange = _db.MobSalesInvoiceItemCard.Where(inv => inv.MobInvId == header.MobInvId).ToList();
            _db.MobSalesInvoiceItemCard.RemoveRange(detailRange);
            _db.SaveChanges();

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


        [HttpPost, AllowAnonymous]
        public IActionResult CreateOrderHeader(OrderHeaderDto dto)
        {
            var book = from counter in _db.SysCounter
                       where counter.BookId == dto.Header_BookId && counter.TrIdName == "SalesOrderId"
                       select counter.Counter;
            var empid = _db.GUsers.FirstOrDefault(x => x.UserId == Convert.ToInt32(dto.Header_CreatedBy));

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
                TaxesId1 = dto.Header_TaxesId1,
                TaxesId2 = dto.Header_TaxesId2,
                TaxesId3 = dto.Header_TaxesId3,
                PriceAfterTax = dto.Header_PriceAfterTax,
                NetPrice = dto.Header_NetPrice,
                PaidPrice = dto.Header_PaidPrice,
                PaidPriceVisa = dto.Header_PaidPriceVisa,
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

            salesOrder.TrNo = ((int)(book).FirstOrDefault()) + 1;


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

        [HttpPut("{SalesOrderId}")]
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
            Orderheader.InvoiceType = (byte?)dto.InvoiceType;
            Orderheader.TrDate= dto.TrDate;
            Orderheader.CreatedBy = dto.CreatedBy;
            Orderheader.EmpId = empid.EmpId;

            _db.SaveChanges();

            var detailRange = _db.MsSalesOrderItemCard.Where(inv => inv.SalesOrderId == Orderheader.SalesOrderId).ToList();
            _db.MsSalesOrderItemCard.RemoveRange(detailRange);
            _db.SaveChanges();

            updateDto.message = "update Successfully";
            updateDto.SalesOrderId = Orderheader.SalesOrderId;
            return Ok(updateDto);

        }

        [HttpDelete("{SalesOrderId},{UserId}")]
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

    }

}
