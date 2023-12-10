using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class QuantityUpdate
    {
        public int Invid { get; set; }
        public int ItemCardId { get; set; }
        public decimal ReturnedQuantity { get; set; }
        public decimal? UpdatePrice { get; set; }
        public string note { get; set; }
    }
}
