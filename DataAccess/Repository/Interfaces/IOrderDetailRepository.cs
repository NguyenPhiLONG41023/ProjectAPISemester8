using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Interfaces
{
    public interface IOrderDetailRepository
    {
        List<OrderDetailVM> GetOrderDetailsListByOrderID(Guid ordID);
        //void AddNew(OrderDetail ordd);
        decimal GetTotalPrice(Guid ordId);
    }
}
