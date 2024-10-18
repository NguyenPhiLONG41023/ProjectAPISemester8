using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.ResourceModel.ViewModel
{
    public class CartItem
    {
        public ProductVM product {  get; set; }
        public int ammount { get; set; }
    }
}
