﻿using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class ProdRecipe
    {
        public ProdRecipe()
        {
            ProdRecipeDetail = new HashSet<ProdRecipeDetail>();
        }

        public int RecipeId { get; set; }
        public string RecipeCode { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public int? ItemCardId { get; set; }
        public int? ItemAtrribBatchId { get; set; }
        public int? UnitId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<ProdRecipeDetail> ProdRecipeDetail { get; set; }
    }
}
