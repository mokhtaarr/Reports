using System;
using System.Collections.Generic;
using System.Linq;
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



        [HttpPost, AllowAnonymous]
        public IEnumerable<GetPartitionAndStoreDto> GetPartitionWithStores(int ItemCardId, int storeid)
        {
            return _itemsBll.GetPartitionWithStores(ItemCardId, storeid);

        }



        [HttpPost, AllowAnonymous]
        public IActionResult CreateHeader([FromBody] HeaderDto dto)
        {

            var book = from counter in _db.SysCounter
                       where counter.BookId == dto.Header_BookId && counter.TrIdName == "InvId"
                       select counter.Counter;

            var salesInvoice = new MsSalesInvoice
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
                // last Edit in 11-6-2023
                Remarks = dto.Remarks,
                AddField3 = dto.AddField3,
                InvDueDate = dto.InvDueDate

            };



            salesInvoice.TrNo = ((int)(book).FirstOrDefault()) + 1;

            _db.MsSalesInvoice.Add(salesInvoice);
            _db.SaveChanges();

            var Counter = _db.SysCounter.FirstOrDefault(x => x.BookId == dto.Header_BookId);
            Counter.Counter = salesInvoice.TrNo;

            _db.SaveChanges();

            ResultHeaderAndDetialDto res = new ResultHeaderAndDetialDto();
            res.Invid = salesInvoice.InvId;
            res.Message = "تم أضافه الداتا بنجاح";

            return Ok(res);

        }

        [HttpPost]
        public IActionResult CreateDetail([FromBody] List<DetailDto> dto)
        {


            var InvD = 0;
            foreach (var item in dto)
            {

                _db.MsSalesInvoiceItemCard.Add(new MsSalesInvoiceItemCard()
                {
                    InvId = item.InvId,
                    ItemCardId = item.ItemCardId,
                    Price = item.Price,
                    //PriceAfterRate = item.PriceAfterRate,
                    QtyBeforRate = item.QtyBeforRate,
                    //Quantity = item.Quantity,
                    UnitId = item.UnitId,
                    UnitRate = item.UnitRate,
                    StoreId = item.StoreId,
                    StorePartId = item.StorePartId,
                    DisPercent = item.DisPercent,
                    DisAmount = item.DisAmount,
                    Tax1Percent = item.Tax1Percent,
                    Tax2Percent = item.Tax2Percent,
                    Tax3Percent = item.Tax3Percent,
                    TaxesId1 = item.Detail_TaxesId1,
                    TaxesId2 = item.Detail_TaxesId2,
                    TaxesId3 = item.Detail_TaxesId3,
                    Tax1IsAccomulative = item.Tax1IsAccomulative,
                    Tax2IsAccomulative = item.Tax2IsAccomulative,
                    Tax3IsAccomulative = item.Tax3IsAccomulative
                });
                InvD = item.InvId;
            }
            _db.SaveChanges();
            ResultHeaderAndDetialDto res = new ResultHeaderAndDetialDto();
            res.Invid = InvD;
            res.Message = "تم أضافه الداتا بنجاح";
            return Ok(res);
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
                InvDueDate = dto.InvDueDate
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
                _db.MsSalesOrderItemCard.Add(new MsSalesOrderItemCard()
                {
                    SalesOrderId = item.SalesOrderId,
                    ItemCardId = item.ItemCardId,
                    Price = item.Price,
                    PriceAfterRate = item.PriceAfterRate,
                    QtyBeforRate = item.QtyBeforRate,
                    Quantity = item.Quantity,
                    UnitId = item.UnitId,
                    UnitRate = item.UnitRate,
                    StoreId = item.StoreId,
                    StorePartId = item.StorePartId,
                    DisPercent = item.DisPercent,
                    DisAmount = item.DisAmount,
                    Tax1Percent = item.Tax1Percent,
                    Tax2Percent = item.Tax2Percent,
                    Tax3Percent = item.Tax3Percent,
                    TaxesId1 = item.Detail_TaxesId1,
                    TaxesId2 = item.Detail_TaxesId2,
                    TaxesId3 = item.Detail_TaxesId3,
                    Tax1IsAccomulative = item.Tax1IsAccomulative,
                    Tax2IsAccomulative = item.Tax2IsAccomulative,
                    Tax3IsAccomulative = item.Tax3IsAccomulative
                });
                OrderId = item.SalesOrderId;
            }
            _db.SaveChanges();
            OrderResultHeaderAndDetailDto res = new OrderResultHeaderAndDetailDto();
            res.id = OrderId;
            res.Message = "تم اضافه الداتا بنجاح";
            return Ok(res);
        }


        [HttpPost]
        public IActionResult headerSalesOffer([FromBody] SalesOfferDto dto)
        {

            var book = from counter in _db.SysCounter
                       where counter.BookId == dto.BookId && counter.TrIdName == "SalesOfferId"
                       select counter.Counter;

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
                InvDueDate = dto.InvDueDate
                
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
                _db.MsSalesOfferItemCard.Add(new MsSalesOfferItemCard()
                {
                    SalesOfferId = item.SalesOfferId,
                    ItemCardId = item.ItemCardId,
                    Price = item.Price,
                    PriceAfterRate = item.PriceAfterRate,
                    QtyBeforRate = item.QtyBeforRate,
                    Quantity = item.Quantity,
                    UnitId = item.UnitId,
                    UnitRate = item.UnitRate,
                    StoreId = item.StoreId,
                    StorePartId = item.StorePartId,
                    DisPercent = item.DisPercent,
                    DisAmount = item.DisAmount,
                    Tax1Percent = item.Tax1Percent,
                    Tax2Percent = item.Tax2Percent,
                    Tax3Percent = item.Tax3Percent,
                    TaxesId1 = item.Detail_TaxesId1,
                    TaxesId2 = item.Detail_TaxesId2,
                    TaxesId3 = item.Detail_TaxesId3,
                    Tax1IsAccomulative = item.Tax1IsAccomulative,
                    Tax2IsAccomulative = item.Tax2IsAccomulative,
                    Tax3IsAccomulative = item.Tax3IsAccomulative
                });
                OrderId = item.SalesOfferId;
            }
            _db.SaveChanges();
            OrderResultHeaderAndDetailDto res = new OrderResultHeaderAndDetailDto();
            res.id = OrderId;
            res.Message = "تم اضافه الداتا بنجاح";
            return Ok(res);
        }

    }
}
