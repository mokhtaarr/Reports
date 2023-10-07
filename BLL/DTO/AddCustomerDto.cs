using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BLL.DTO
{
    public class AddCustomerDto
    {
        [Required,MaxLength(50)]
        public string CustomerCode { get; set; }
        [Required, MaxLength(100)]
        public string CustomerDescA { get; set; }
        [Required, MaxLength(100)]
        public string CustomerDescE { get; set; }

        public string Tel { get; set; }
        public string Te2 { get; set; }
        public string TaxRefNo { get; set; }

        public string Email { get; set;}

        public string Address { get; set; }

        public int CustomerCatId { get; set; }
    }
}
