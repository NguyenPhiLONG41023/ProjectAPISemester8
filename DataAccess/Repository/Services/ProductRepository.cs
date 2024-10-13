using AutoMapper;
using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using DataAccess.DataAccess;
using DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly NewProjectDBContext _context;
        private readonly IMapper _mapper;
        public ProductRepository(NewProjectDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void DeleteProduct(Guid proID)
        {
            try
            {
                Product pro = _context.Products.SingleOrDefault(p => p.ProductId == proID);
                if (pro != null)
                {
                    pro.OrderDetails.Clear();
                    _context.Products.Remove(pro);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Product does not already exist");
                }

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public ProductVM GetProductByID(Guid proID)
        {
            Product pro = null;
            try
            {
                pro = _context.Products.Include(x => x.Brand).SingleOrDefault(pro => pro.ProductId == proID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<ProductVM>(pro);
        }

        public List<ProductVM> GetProductList()
        {
            var products = new List<Product>();
            try
            {
                products = _context.Products.Include(x => x.Brand).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<List<ProductVM>>(products);
        }

        public void InsertProduct(ProductAddVM product)
        {
            try
            {
                var p = _mapper.Map<Product>(product);
                _context.Products.Add(p);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<ProductVM> SearchProduct(string search, decimal? priceFrom, decimal? priceTo)
        {
            var products = new List<Product>();
            try
            {
                products = _context.Products.Include(c => c.Brand)
                    .Where(pro =>
                        (string.IsNullOrEmpty(search) || pro.ProductName.Contains(search) || pro.ProductId.ToString() == search) &&
                        (pro.Price >= priceFrom && pro.Price <= priceTo)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<List<ProductVM>>(products);
        }

        public void UpdateProduct(Guid id, ProductAddVM pro)
        {
            try
            {
                var existingProduct = _context.Products.Find(id);
                if (existingProduct != null)
                {
                    _context.Entry(existingProduct).State = EntityState.Detached;
                }

                var p = _mapper.Map<Product>(pro);
                p.ProductId = id;
                _context.Entry<Product>(p).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
