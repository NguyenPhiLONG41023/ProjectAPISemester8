using AutoMapper;
using AutoMapper.Execution;
using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.AutoMapper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<Product, ProductVM>()
                .ForMember(dest => dest.BrandName, otp => otp.MapFrom(src => src.Brand.BrandName))
                .ReverseMap();
            CreateMap<Product, ProductAddVM>().ReverseMap();
            CreateMap<Brand, BrandVM>().ReverseMap();
            CreateMap<Brand, BrandAddVM>().ReverseMap();
            CreateMap<Order, OrderVM>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailVM>().ReverseMap();
            CreateMap<Order, OrderUpdateVM>().ReverseMap();
        }
    }
}
