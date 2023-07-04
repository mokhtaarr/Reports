using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class getHeaderDto
    {
        public int InvId { get; set; }
        public string CustomerCode { get;set; }
        public string CustomerDescA { get; set; }
        public decimal NetPrice { get; set; }
        public string DocTrno { get; set; }
        public DateTime CreatedAt { get; set; }

       


    }
}
