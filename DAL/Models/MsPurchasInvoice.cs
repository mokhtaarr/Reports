﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class MsPurchasInvoice
    {
        public MsPurchasInvoice()
        {
            MsPaymentNote = new HashSet<MsPaymentNote>();
            MsPurchInvVehicleJobOrderJoin = new HashSet<MsPurchInvVehicleJobOrderJoin>();
            MsPurchasInvoiceCurrencies = new HashSet<MsPurchasInvoiceCurrencies>();
            MsPurchasInvoiceMultiAccounts = new HashSet<MsPurchasInvoiceMultiAccounts>();
            MsPurchaseInvoiceCostReceived = new HashSet<MsPurchaseInvoiceCostReceived>();
            MsPurchaseInvoiceExpenses = new HashSet<MsPurchaseInvoiceExpenses>();
            MsPurchaseInvoiceItemCard = new HashSet<MsPurchaseInvoiceItemCard>();
            ProdJobOrderPurchaseInvoices = new HashSet<ProdJobOrderPurchaseInvoices>();
            SrVehicleRentPurchJoin = new HashSet<SrVehicleRentPurchJoin>();
        }

        public int PurInvId { get; set; }
        public int? VendorId { get; set; }
        public int? StorId { get; set; }
        public int? PurOrderId { get; set; }
        public int? PurOrderReqId { get; set; }
        public int? BookId { get; set; }
        public int? TermId { get; set; }
        public int? CurrencyId { get; set; }
        public int? VendBranchId { get; set; }
        public int? ExpensesId { get; set; }
        public int? FinancialIntervalsId { get; set; }
        public string DbtableName { get; set; }
        public int? DbtableId { get; set; }
        public string AccountTableName { get; set; }
        public byte? RectSourceType { get; set; }
        public int? AccountId { get; set; }
        public int? TaxesId1 { get; set; }
        public decimal? TaxValue1 { get; set; }
        public int? TaxesId2 { get; set; }
        public decimal? TaxValue2 { get; set; }
        public int? TaxesId3 { get; set; }
        public decimal? TaxValue3 { get; set; }
        public int? Aid { get; set; }
        public int? TrNo { get; set; }
        public string ManualTrNo { get; set; }
        public DateTime? TrDate { get; set; }
        public string InvDescA { get; set; }
        public string InvDescE { get; set; }
        public string Remarks { get; set; }
        public string AddField3 { get; set; }
        public string AddField4 { get; set; }
        public string AddField5 { get; set; }
        public string AddField6 { get; set; }
        public string AddField7 { get; set; }
        public byte? InvoiceType { get; set; }
        public DateTime? InvDueDate { get; set; }
        public decimal? TotalItemTax1 { get; set; }
        public decimal? TotalItemTax2 { get; set; }
        public decimal? TotalItemTax3 { get; set; }
        public decimal? TotalExpensesTaxes1 { get; set; }
        public decimal? TotalExpensesTaxes2 { get; set; }
        public decimal? TotalExpensesTaxes3 { get; set; }
        public decimal? TotalTaxValu { get; set; }
        public decimal? InvTotal { get; set; }
        public decimal? DiscPercent { get; set; }
        public decimal? DiscAmount { get; set; }
        public decimal? DiscPercent2 { get; set; }
        public decimal? DiscAmount2 { get; set; }
        public decimal? DiscPercent3 { get; set; }
        public decimal? DiscAmount3 { get; set; }
        public decimal? DiscPercent4 { get; set; }
        public decimal? DiscAmount4 { get; set; }
        public decimal? PriceAfterTax { get; set; }
        public decimal? ExpenValue { get; set; }
        public decimal? PaidPrice { get; set; }
        public decimal? NetPrice { get; set; }
        public decimal? PaidPriceVisa { get; set; }
        public decimal? BankTransfer { get; set; }
        public bool? Closed { get; set; }
        public bool? IsPrinted { get; set; }
        public decimal? Rate { get; set; }
        public decimal? NetPriceBeforCurr { get; set; }
        public decimal? ExpenValueBeforCurr { get; set; }
        public decimal? ExpenValueWithCurr { get; set; }
        public decimal? AdvancExpenseWithCurr { get; set; }
        public decimal? AdvancExpenseBeforCurr { get; set; }
        public bool? IsDelivered { get; set; }
        public bool? IsPosted { get; set; }
        public string Postedby { get; set; }
        public DateTime? PostedDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public DateTime? UncloseDate { get; set; }
        public int? ClosedBy { get; set; }
        public int? UnclosedBy { get; set; }
        public int? PermPrinted { get; set; }
        public DateTime? PermPrintedAt { get; set; }
        public bool? IsPaid { get; set; }
        public int? PaidDocId { get; set; }
        public decimal? NotPaid { get; set; }
        public int? TermCostCenterId { get; set; }
        public decimal? TermCostCenterValue { get; set; }
        public bool? IsShippingInv { get; set; }
        public int? IsNoCostDeliver { get; set; }
        public bool? DeliverNoCostExecut { get; set; }
        public bool? MultiResourceDeliver { get; set; }
        public bool? EtaxSent { get; set; }
        public DateTime? EtaxSentTime { get; set; }
        public string EtaxRemarks { get; set; }
        public string EtaxReference { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsReturned { get; set; }
        public int? ShiftId { get; set; }
        public byte[] LastUpdateTime { get; set; }
        public bool? IsRemoteEntity { get; set; }
        public int? RemotId { get; set; }
        public int? MainVendServerId { get; set; }

        public virtual MsPurchasOrder PurOrder { get; set; }
        public virtual MsTaxes TaxesId1Navigation { get; set; }
        public virtual ICollection<MsPaymentNote> MsPaymentNote { get; set; }
        public virtual ICollection<MsPurchInvVehicleJobOrderJoin> MsPurchInvVehicleJobOrderJoin { get; set; }
        public virtual ICollection<MsPurchasInvoiceCurrencies> MsPurchasInvoiceCurrencies { get; set; }
        public virtual ICollection<MsPurchasInvoiceMultiAccounts> MsPurchasInvoiceMultiAccounts { get; set; }
        public virtual ICollection<MsPurchaseInvoiceCostReceived> MsPurchaseInvoiceCostReceived { get; set; }
        public virtual ICollection<MsPurchaseInvoiceExpenses> MsPurchaseInvoiceExpenses { get; set; }
        public virtual ICollection<MsPurchaseInvoiceItemCard> MsPurchaseInvoiceItemCard { get; set; }
        public virtual ICollection<ProdJobOrderPurchaseInvoices> ProdJobOrderPurchaseInvoices { get; set; }
        public virtual ICollection<SrVehicleRentPurchJoin> SrVehicleRentPurchJoin { get; set; }
    }
}
