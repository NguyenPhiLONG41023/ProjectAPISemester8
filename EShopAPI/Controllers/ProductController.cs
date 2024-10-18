using BusinessObject.Models;
using BusinessObject.ResourceModel.Common;
using BusinessObject.ResourceModel.ViewModel;
using DataAccess.DataAccess;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace EShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductRepository _repository;
        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult GetProducts()
        {
            var products = _repository.GetProductList().AsQueryable();
            if(products == null) return NotFound();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductByID(Guid id)
        {
            var products = _repository.GetProductByID(id);
            if (products == null) return NotFound();
            return Ok(products);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(Guid id)
        {
            var p = _repository.GetProductByID(id);
            if (p == null)
                return NotFound();
            _repository.DeleteProduct(id);
            return NoContent();
        }

        [HttpPost]
        public IActionResult InsertProduct([FromBody] ProductAddVM product)
        {
            _repository.InsertProduct(product);
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(Guid id, [FromBody] ProductAddVM product)
        {
            _repository.UpdateProduct(id, product);
            return NoContent();
        }

        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string? search, [FromQuery] decimal? priceFrom, [FromQuery] decimal? priceTo)
        {
            if (!priceFrom.HasValue)
            {
                priceTo = 99999999.99m;
            }
            if (!priceFrom.HasValue)
            {
                priceFrom = 0;
            }
            if (priceFrom > priceTo)
            {
                return BadRequest("PriceFrom should not be greater than PriceTo.");
            }
            var products = _repository.SearchProduct(search, priceFrom, priceTo);

            if (products == null || products.Count == 0)
            {
                return NotFound("No products found for the given search criteria.");
            }
            return Ok(products);
        }

        [HttpPost("import")]
        public IActionResult Import(IFormFile fileImport)
        {
            try
            {
                var res = _repository.ImportProduct(fileImport);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
