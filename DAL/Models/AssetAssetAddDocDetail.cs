﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class AssetAssetAddDocDetail
    {
        public int AddAssetDetailId { get; set; }
        public int? AssetAddId { get; set; }
        public int? AssetId { get; set; }
        public int? AssetAccountId { get; set; }
        public int? CostCenterId { get; set; }
        public decimal? AddValue { get; set; }
        public string Remarks { get; set; }
        public string Remarks1 { get; set; }
        public string Remarks2 { get; set; }
        public string Remarks3 { get; set; }

        public virtual AssetAssetAddDoc AssetAdd { get; set; }
    }
}
