using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class SalesOfferDto
    {
        public int BookId { get; set; }
        public int TermId { get; set; }
        public int CurrencyId { get; set; }
        public int CustomerId { get; set; }
        public int RectSourceType { get; set; }
        public decimal Rate { get; set; }
        public decimal NotPaid { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal InvTotal { get; set; }

        public decimal DiscAmount { get; set; }
        public decimal DiscPercent { get; set; }
        public decimal DiscAmount2 { get; set; }
        public decimal DiscPercent2 { get; set; }
        public decimal TotalItemTax1 { get; set; }
        public decimal TotalItemTax2 { get; set; }
        public decimal TotalItemTax3 { get; set; }
        public decimal TaxValue1 { get; set; }
        public decimal TaxValue2 { get; set; }
        public decimal TaxValue3 { get; set; }
        public int TaxesId1 { get; set; }
        public int TaxesId2 { get; set; }
        public int TaxesId3 { get; set; }
        public decimal PriceAfterTax { get; set; }
        public decimal NetPrice { get; set; }
        public decimal PaidPrice { get; set; }
        public decimal PaidPriceVisa { get; set; }
        public decimal BankTransfer { get; set; }
        public string Remarks { get; set; }
        public string AddField3 { get; set; }
        public DateTime InvDueDate { get; set; }

        public int StoreId { get; set; }
        public decimal ExpenValue { get; set; }
        public int InvoiceType { get; set; }




    }
}
