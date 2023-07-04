﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class CodInstallmentTemps
    {
        public CodInstallmentTemps()
        {
            CodInstallmentTempsDetail = new HashSet<CodInstallmentTempsDetail>();
            ProjProjUnitInstallTemp = new HashSet<ProjProjUnitInstallTemp>();
        }

        public int InstallTempId { get; set; }
        public int Code { get; set; }
        public string DescA { get; set; }
        public string DescE { get; set; }
        public int? Aid { get; set; }
        public bool? IsPercent { get; set; }
        public decimal? YearsCount { get; set; }
        public bool? CanEditInstallManual { get; set; }
        public bool? IsRental { get; set; }
        public bool? AutoRenew { get; set; }
        public decimal? AddValue { get; set; }
        public int? EveryPayCount { get; set; }
        public int? AfterPeriod { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string RemarksA { get; set; }
        public string RemarksE { get; set; }

        public virtual ICollection<CodInstallmentTempsDetail> CodInstallmentTempsDetail { get; set; }
        public virtual ICollection<ProjProjUnitInstallTemp> ProjProjUnitInstallTemp { get; set; }
    }
}
