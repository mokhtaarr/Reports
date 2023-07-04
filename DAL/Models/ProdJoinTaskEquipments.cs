﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class ProdJoinTaskEquipments
    {
        public int TaskEquipJoinId { get; set; }
        public int? TaskId { get; set; }
        public int? EquipId { get; set; }
        public int? RequestedNumber { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ProdTasks Task { get; set; }
    }
}
