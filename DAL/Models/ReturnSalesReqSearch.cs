﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class ReturnSalesReqSearch
    {
        public int? StoreId { get; set; }
        public int? TrNo { get; set; }
        public DateTime TrDate { get; set; }

        public decimal? NetPrice { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerDescA { get; set; }

        public int ReqsalesId { get; set; }

        public string DocTrNo { get; set;}

        public string TermCode { get; set; }
        public string TermName { get; set; }
        public byte TermType { get; set; }

        public int TermId { get; set; }
        public int BookId { get; set; }
        public string SalesInvDocTrNo { get; set; }

    }
}
