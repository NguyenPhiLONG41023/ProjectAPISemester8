using AutoMapper;
using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using DataAccess.DataAccess;
using DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Services
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly NewProjectDBContext _context;
        private readonly IMapper _mapper;

        public OrderDetailRepository(NewProjectDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //public void AddNew(OrderDetail ordd)
        //{
        //    try
        //    {
        //        _context.OrderDetails.Add(ordd);
        //        _context.SaveChanges();

        //    }
        //    catch (Exception ex) { throw new Exception(ex.Message); }
        //}

        public List<OrderDetailVM> GetOrderDetailsListByOrderID(Guid ordID)
        {
            var OrderDetails = new List<OrderDetail>();
            try
            {
                OrderDetails = _context.OrderDetails.Include(p => p.Product)
                    .Where(pro => pro.OrderId == ordID).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<List<OrderDetailVM>>(OrderDetails);
        }

        public decimal GetTotalPrice(Guid ordId)
        {
            decimal total = 0;
            var x = GetOrderDetailsListByOrderID(ordId).ToList();
            foreach (var order in x)
            {
                total += order.Quantity * order.Price;
            }
            return total;
        }
    }
}
