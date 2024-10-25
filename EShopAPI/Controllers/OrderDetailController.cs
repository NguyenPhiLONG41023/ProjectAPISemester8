using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace EShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private IOrderDetailRepository _repository;
        public OrderDetailController(IOrderDetailRepository repository)
        {
            _repository = repository;
        }
        [HttpGet("GetOrderDetailsListByOrderID")]
        [EnableQuery]
        public IActionResult GetOrderDetailByOrderId(Guid id) 
        {
            var orderdetails = _repository.GetOrderDetailsListByOrderID(id).AsQueryable();
            if (orderdetails == null)
            {
                return NotFound();
            }
            return Ok(orderdetails);
        }

        [HttpGet("GetTotalPriceByOrderID")]
        public IActionResult GetTotalPriceByOrderID(Guid orderId)
        {
            var orderdetails = _repository.GetTotalPrice(orderId);
            if (orderdetails == null) return NotFound();
            return Ok(orderdetails);
        }

        [HttpPost]
        public IActionResult AddOrderDetail([FromBody] OrderDetailAddVM p)
        {
            _repository.AddNew(p);
            return NoContent();
        }
    }
}
