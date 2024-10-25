using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;

namespace EShopAPI.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();

        public void RemoveLine(ProductVM product) => Lines.RemoveAll(l => l.Product.ProductId == product.ProductId);

        public decimal ComputeTotalValue() => Lines.Sum(e => e.Product.Price * e.Quantity);

        public void Clear() => Lines.Clear();

        public void AddItem(ProductVM product, int? quantity)
        {
            CartLine? line = Lines
                .Where(p => p.Product.ProductId == product.ProductId).FirstOrDefault();
            if (line == null)   //chưa có thì tạo mới
                Lines.Add(new CartLine 
                { 
                    Product = product, 
                    Quantity = quantity ?? 0,
                });
            else   //có rồi thì thêm số lượng
                line.Quantity += quantity ?? 0;
        }

        public class CartLine
        {
            public int CartLineID { get; set; }
            public ProductVM Product { get; set; } = new();
            public int Quantity { get; set; }
        }
    }
}
