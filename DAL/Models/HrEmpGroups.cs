﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class HrEmpGroups
    {
        public HrEmpGroups()
        {
            HrEmployees = new HashSet<HrEmployees>();
            InverseParent = new HashSet<HrEmpGroups>();
        }

        public int HrempGroupId { get; set; }
        public string GroupCode { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public int? ParentId { get; set; }
        public byte? GroupType { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual HrEmpGroups Parent { get; set; }
        public virtual ICollection<HrEmployees> HrEmployees { get; set; }
        public virtual ICollection<HrEmpGroups> InverseParent { get; set; }
    }
}
