﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class MsGaLetterExport
    {
        public MsGaLetterExport()
        {
            MsGaLetterExportDetail = new HashSet<MsGaLetterExportDetail>();
            MsGaShipmentDetail = new HashSet<MsGaShipmentDetail>();
        }

        public int LetterExportId { get; set; }
        public int? StoreId { get; set; }
        public int? TrNo { get; set; }
        public DateTime? TrDate { get; set; }
        public int? CustomerId { get; set; }
        public int? DeliverId { get; set; }
        public string CustomsNo { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string AddField1 { get; set; }
        public string AddField2 { get; set; }

        public virtual ICollection<MsGaLetterExportDetail> MsGaLetterExportDetail { get; set; }
        public virtual ICollection<MsGaShipmentDetail> MsGaShipmentDetail { get; set; }
    }
}
