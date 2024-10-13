using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private IBrandRepository _repository;
        public BrandController(IBrandRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public IActionResult GetBrands() 
        {
            var brands = _repository.GetBrandList();
            return Ok(brands);
        }

        [HttpPost]
        public IActionResult InsertBrand([FromBody] BrandAddVM brand)
        {
            _repository.InsertBrand(brand);
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBrand(Guid id, [FromBody] BrandAddVM brand)
        {
            var existingBrand = _repository.GetBrandByID(id);
            if (existingBrand == null)
            {
                return NotFound();
            }
            _repository.UpdateBrand(id, brand);
            return NoContent();
        }
    }
}
