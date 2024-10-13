﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.ResourceModel.ViewModel
{
    public class ProductVM
    {
        public Guid ProductId { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public int Status { get; set; }
    }
}