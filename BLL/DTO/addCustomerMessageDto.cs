using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class addCustomerMessageDto
    {
        public int CustomerId { get; set; }
        public string CustomerDescA { get; set; }
        public byte? SalPrice { get; set; }

        public string Message { get; set; }
        public int msCustomerCategories { get; set; }
    }
}
