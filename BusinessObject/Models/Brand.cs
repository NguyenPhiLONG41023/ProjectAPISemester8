using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Brand
    {
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
