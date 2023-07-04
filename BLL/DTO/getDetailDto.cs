using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class getDetailDto
    {
        public int ItemCardId { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterRate { get; set; }
        public decimal QtyBeforRate { get; set; }
        public decimal Quantity { get; set; }
        public int UnitId { get; set; }
        public decimal UnitRate { get; set; }

        public int StoreId { get; set; }
        public int StorePartId { get; set; }
        public decimal DisPercent { get; set; }
        public decimal DisAmount { get; set; }
        public decimal Tax1Percent { get; set; }
        public decimal Tax2Percent { get; set; }
        public decimal Tax3Percent { get; set; }
        public int TaxesId1 { get; set; }
        public int TaxesId2 { get; set; }
        public int TaxesId3 { get; set; }
        public bool Tax1IsAccomulative { get; set; }
        public bool Tax2IsAccomulative { get; set; }
        public bool Tax3IsAccomulative { get; set; }
    }
}
