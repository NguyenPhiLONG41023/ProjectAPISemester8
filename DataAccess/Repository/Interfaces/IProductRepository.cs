using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Interfaces
{
    public interface IProductRepository
    {
        List<ProductVM> GetProductList();
        ProductVM GetProductByID(Guid proID);
        void InsertProduct(ProductAddVM product);
        void UpdateProduct(Guid id, ProductAddVM product);
        void DeleteProduct(Guid proID);
        List<ProductVM> SearchProduct(string search, decimal? priceFrom, decimal? priceTo);
        IEnumerable<Product> ImportProduct(IFormFile fileImport);
    }
}
