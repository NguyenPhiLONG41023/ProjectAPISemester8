using AutoMapper;
using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using DataAccess.DataAccess;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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

        public IEnumerable<Product> ImportProduct(IFormFile fileImport)
        {
            CheckFileImport(fileImport);
            var customers = new List<ProductImport>();

            using (var stream = new MemoryStream())
            {
                // Copy tệp vào Stream:
                fileImport.CopyTo(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // Thực hiện đọc dữ liệu:
                using (var package = new ExcelPackage(stream))
                {
                    // Sheet đọc dữ liệu:
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    if (worksheet != null)
                    {
                        // Tổng số dòng dữ liệu:
                        var rowCount = worksheet.Dimension.Rows;
                        // Bắt đầu đọc dữ liệu (dòng thứ 2 trong Sheet)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var customerImport = new ProductImport
                            {
                                ProductId = Guid.NewGuid(),
                                BrandId = Guid.Parse(worksheet?.Cells[row, 1]?.Value?.ToString()?.Trim() ?? Guid.Empty.ToString()),
                                ProductName = worksheet.Cells[row, 2]?.Value?.ToString()?.Trim(),
                                Quantity = int.Parse(worksheet.Cells[row, 3]?.Value?.ToString()?.Trim() ?? "0"),
                                Description = worksheet.Cells[row, 4]?.Value?.ToString()?.Trim(),
                                Price = decimal.Parse(worksheet.Cells[row, 5]?.Value?.ToString()?.Trim() ?? "0"),
                                Image = worksheet.Cells[row, 6]?.Value?.ToString()?.Trim(),
                                Status = int.Parse(worksheet.Cells[row, 7]?.Value?.ToString()?.Trim() ?? "0"),
                            };
                            //Map
                            var customer = _mapper.Map<Product>(customerImport);
                            _context.Products.Add(customer);
                            var res = _context.SaveChanges();
                            if (res > 0)
                            {
                                customerImport.IsImported = true;
                            }
                            // Thực hiện thêm mới:
                            customers.Add(customerImport);
                        }
                    }
                }
            }
            return customers;
        }

        //check file import hợp lệ
        private void CheckFileImport(IFormFile fileImport)
        {
            if (fileImport == null || fileImport.Length <= 0)
            {
                throw new ArgumentException("The import file is empty or null.", nameof(fileImport));
            }
            if (!Path.GetExtension(fileImport.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The import file is not a valid .xlsx file.", nameof(fileImport));
            }
        }

        public List<ProductVM> SearchProductByName(string search)
        {
            var products = new List<Product>();
            try
            {
                products = _context.Products.Include(c => c.Brand)
                    .Where(pro =>
                        (string.IsNullOrEmpty(search) || pro.ProductName.Contains(search) || pro.ProductId.ToString() == search)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<List<ProductVM>>(products);
        }
    }
}
