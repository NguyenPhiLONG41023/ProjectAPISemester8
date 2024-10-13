using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Interfaces
{
    public interface IBrandRepository
    {
        List<BrandVM> GetBrandList();
        BrandVM GetBrandByID(Guid id);
        void InsertBrand(BrandAddVM brand);
        void UpdateBrand(Guid id, BrandAddVM brand);
    }
}
