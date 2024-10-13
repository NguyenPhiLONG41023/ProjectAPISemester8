using AutoMapper;
using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using DataAccess.DataAccess;
using DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Services
{
    public class BrandRepository : IBrandRepository
    {
        private readonly NewProjectDBContext _context;
        private readonly IMapper _mapper;
        public BrandRepository(NewProjectDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public BrandVM GetBrandByID(Guid id)
        {
            Brand pro = null;
            try
            {
                pro = _context.Brands.SingleOrDefault(pro => pro.BrandId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<BrandVM>(pro);
        }

        public List<BrandVM> GetBrandList()
        {
            var brands = new List<Brand>();
            try
            {
                brands = _context.Brands.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<List<BrandVM>>(brands);
        }

        public void InsertBrand(BrandAddVM brand)
        {
            try
            {
                var p = _mapper.Map<Brand>(brand);
                _context.Brands.Add(p);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateBrand(Guid id, BrandAddVM brand)
        {
            try
            {
                var existingProduct = _context.Brands.Find(id);
                if (existingProduct != null)
                {
                    _context.Entry(existingProduct).State = EntityState.Detached;
                }

                var p = _mapper.Map<Brand>(brand);
                p.BrandId = id;
                _context.Entry<Brand>(p).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
