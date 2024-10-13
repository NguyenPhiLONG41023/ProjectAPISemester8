﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.ResourceModel.ViewModel
{
    public class OrderUpdateVM
    {
        public string UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
    }
}
