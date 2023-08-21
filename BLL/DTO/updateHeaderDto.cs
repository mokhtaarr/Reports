using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class updateHeaderDto
    {   
        public string message { get; set; }
        public int invid { get; set; }
        public int SalesOrderId { get; set; }
        public int SalesOfferId { get; set; }
    }
}
