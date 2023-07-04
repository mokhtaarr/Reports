﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class MsBusinessPartnerCategory
    {
        public MsBusinessPartnerCategory()
        {
            MsBusinessPartners = new HashSet<MsBusinessPartners>();
        }

        public int BsPartnerCatId { get; set; }
        public string PartnerCatCode { get; set; }
        public string PartnerCatDescA { get; set; }
        public string PartnerCatDescE { get; set; }
        public int? ParentCustomerCatId { get; set; }
        public byte? CustomerCatType { get; set; }
        public int? CustomerCatLevel { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<MsBusinessPartners> MsBusinessPartners { get; set; }
    }
}
