using AutoMapper;
using AutoMapper.Execution;
using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using DataAccess.DataAccess;
using DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly NewProjectDBContext _context;
        private readonly IMapper _mapper;
        public OrderRepository(NewProjectDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddOrder(OrderUpdateVM ord)
        {
            try
            {
                var p = _mapper.Map<Order>(ord);
                _context.Orders.Add(p);
                _context.SaveChanges();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        //public void DeleteOrder(Guid ordID)
        //{
        //    try
        //    {
        //        Order ord = GetOrderByID(ordID);
        //        if (ord != null)
        //        {
        //            ord.OrderDetails.Clear();
        //            _context.Orders.Remove(ord);
        //            _context.SaveChanges();
        //        }
        //        else
        //        {
        //            throw new Exception("Order does not already exist");
        //        }

        //    }
        //    catch (Exception ex) { throw new Exception(ex.Message); }
        //}

        public List<OrderVM> FilterByDate(DateTime? start, DateTime? end)
        {
            var result = new List<Order>();
            try
            {
                result = _context.Orders.Where(pro => pro.OrderDate >= start && pro.OrderDate <= end)
                    .OrderByDescending(pro => pro.OrderDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<List<OrderVM>>(result);
        }

        public OrderVM GetOrderByID(Guid orderId)
        {
            Order mem = null;
            try
            {
                mem = _context.Orders.SingleOrDefault(pro => pro.OrderId == orderId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<OrderVM>(mem);
        }

        public List<OrderVM> GetOrderListByUserID(string UserId)
        {
            var Orders = new List<Order>();
            try
            {
                Orders = _context.Orders.Where(pro => pro.UserId == UserId)
                    .OrderByDescending(pro => pro.OrderDate).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<List<OrderVM>>(Orders);
        }

        public IEnumerable<OrderVM> GetOrdersList()
        {
            var Orders = new List<Order>();
            try
            {
                Orders = _context.Orders.OrderByDescending(pro => pro.OrderDate).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<List<OrderVM>>(Orders);
        }

        public void UpdateOrder(Guid id, OrderUpdateVM ord)
        {
            try
            {
                var existingOrder = _context.Orders.Find(id);
                if (existingOrder != null)
                {
                    _context.Entry(existingOrder).State = EntityState.Detached;
                }

                var p = _mapper.Map<Order>(ord);
                p.OrderId = id;
                _context.Entry<Order>(p).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
