using AutoMapper;
using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderRepository _repository;
        private readonly IMapper _mapper;
        public OrderController(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetOrdersList()
        {
            var orders = _repository.GetOrdersList();
            var OrderVMs = _mapper.Map<List<OrderVM>>(orders);
            return Ok(OrderVMs);
        }

        [HttpGet("getorderbyuserid")]
        public IActionResult GetOrderListByUserID(string uid)
        {
            var order = _repository.GetOrderListByUserID(uid);
            if (order == null)
            {
                return NotFound();
            }
            var p = _mapper.Map<List<OrderVM>>(order);
            return Ok(p);
        }

        [HttpGet("getorderbyid")]
        public IActionResult GetOrderByID(Guid id)
        {
            var order = _repository.GetOrderByID(id);
            if (order == null)
            {
                return NotFound();
            }
            var p = _mapper.Map<OrderVM>(order);
            return Ok(p);
        }

        [HttpPost]
        public IActionResult AddOrder([FromBody] OrderUpdateVM p)
        {
            _repository.AddOrder(p);
            return NoContent();
        }


        //[HttpDelete("{id}")]
        //public IActionResult DeleteOrder(int id)
        //{
        //    var p = _repository.GetOrderByID(id);
        //    if (p == null)
        //        return NotFound();
        //    _repository.DeleteOrder(id);
        //    return NoContent();
        //}

        [HttpPut("{id}")]
        public IActionResult UpdateOrder(Guid id, [FromBody] OrderUpdateVM order)
        {
            
            //p.MemberId = order.MemberId;
            _repository.UpdateOrder(id, order);
            return NoContent();
        }

        [HttpGet("searchorderbydate")]
        public IActionResult FilterOrdersByDate(DateTime? start, DateTime? end)
        {
            if (!start.HasValue && !end.HasValue)
            {
                var allOrders = _repository.GetOrdersList();
                return Ok(allOrders);
            }
            if (start > end)
            {
                return BadRequest("StartDate can not be latter than EndDate");
            }
            var orders = _repository.FilterByDate(start, end);
            if (orders == null || orders.Count == 0)
            {
                return NotFound();
            }
            return Ok(orders);
        }
    }
}
