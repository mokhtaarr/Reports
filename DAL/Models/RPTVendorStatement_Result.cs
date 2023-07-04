﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public partial class RPTVendorStatement_Result
    {
        public Nullable<int> CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerDescA { get; set; }
        public Nullable<int> TrNo { get; set; }
        public Nullable<System.DateTime> TrDate { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyDescA { get; set; }
        public string AccountCode { get; set; }
        public string AccountNameA { get; set; }
        public Nullable<decimal> OpenningBalanceDepitCurncy { get; set; }
        public Nullable<decimal> OpenningBalanceCreditCurncy { get; set; }
        public Nullable<decimal> OpenningBalanceDepit { get; set; }
        public Nullable<decimal> OpenningBalanceCredit { get; set; }
        public Nullable<decimal> AccCurrTrancDepit { get; set; }
        public Nullable<decimal> AccTotaCredit { get; set; }
        public Nullable<decimal> AccTotalDebit { get; set; }
        public Nullable<decimal> BalanceDebitLocal { get; set; }
        public Nullable<decimal> BalanceCreditLocal { get; set; }
        public Nullable<decimal> AccCurrTrancCreditCurncy { get; set; }
        public Nullable<decimal> AccTotaCreditCurncy { get; set; }
        public Nullable<decimal> BalanceDebitCurncy { get; set; }
        public Nullable<decimal> BalanceCreditCurncy { get; set; }
        public decimal DebitLocal { get; set; }
        public decimal CreditLocal { get; set; }
        public decimal DebitCurrency { get; set; }
        public decimal CreditCurrency { get; set; }
        public decimal BalanceLocalAfterDebit { get; set; }
        public decimal BalanceLocalAfterCredit { get; set; }
        public decimal BalanceCurrencyAfterDebit { get; set; }
        public decimal BalanceCurrencyAfterCredit { get; set; }
        public string TableCode { get; set; }
        public string Remarks { get; set; }
        public string HelpAccType { get; set; }
        public bool IsOpenning { get; set; }
        public decimal DebitLocalWithoutOpen { get; set; }
        public decimal CreditLocalWithoutOpen { get; set; }
        public decimal DebitCurrencyWithoutOpen { get; set; }
        public decimal CreditCurrencyWithoutOpen { get; set; }
        public string DocTrNo { get; set; }
        public Nullable<bool> CalcMethod { get; set; }
        public Nullable<bool> DefualtCurrency { get; set; }
        public Nullable<int> AccountId { get; set; }
        public string JurDesc { get; set; }
    }
}
