using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class addCustomerMessageDto
    {
        public string CustomerDescA { get; set; }
        public string Message { get; set; }
        public List<MsCustomerCategory> msCustomerCategories { get; set; }
    }
}
