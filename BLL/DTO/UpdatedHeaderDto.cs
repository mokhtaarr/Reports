using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class UpdatedHeaderDto
    {

        public DateTime Header_UpdateAt { get; set; }
        public decimal Header_InvTotal { get; set; }
        public decimal Header_DiscAmount { get; set; }
        public decimal Header_DiscPercent { get; set; }
        public decimal Header_DiscAmount2 { get; set; }
        public decimal Header_DiscPercent2 { get; set; }
        public decimal Header_TotalItemTax1 { get; set; }
        public decimal Header_TotalItemTax2 { get; set; }
        public decimal Header_TotalItemTax3 { get; set; }
        public decimal Header_TaxValue1 { get; set; }
        public decimal Header_TaxValue2 { get; set; }
        public decimal Header_TaxValue3 { get; set; }
        public int Header_TaxesId1 { get; set; }
        public int Header_TaxesId2 { get; set; }
        public int Header_TaxesId3 { get; set; }
        public decimal Header_PriceAfterTax { get; set; }
        public decimal Header_NetPrice { get; set; }
        public decimal Header_PaidPrice { get; set; }
        public decimal Header_PaidPriceVisa { get; set; }
        public decimal Header_BankTransfer { get; set; }

        public string Remarks { get; set; }
        public string AddField3 { get; set; }
        public DateTime InvDueDate { get; set; }

    }
}
