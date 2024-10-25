using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Interfaces
{
    public interface IOrderRepository
    {
        IEnumerable<OrderVM> GetOrdersList();
        List<OrderVM> GetOrderListByUserID(string UserId);
        OrderVM GetOrderByID(Guid orderId);
        OrderVM GetNewestOrder();
        void AddOrder(OrderUpdateVM order);
        void UpdateOrder(Guid id, OrderUpdateVM order);
        //void DeleteOrder(Guid ordID);
        List<OrderVM> FilterByDate(DateTime? start, DateTime? end);
    }
}
