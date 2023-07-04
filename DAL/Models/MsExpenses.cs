﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class MsExpenses
    {
        public int ExpensesId { get; set; }
        public string ExpensesCode { get; set; }
        public string ExpensesDescA { get; set; }
        public string ExpensesDescE { get; set; }
        public byte? ExpensesType { get; set; }
        public decimal? ExpensesValue { get; set; }
        public int? AccountId { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
