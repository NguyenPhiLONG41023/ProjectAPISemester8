using AutoMapper;
using AutoMapper.Execution;
using BusinessObject.Models;
using BusinessObject.ResourceModel.ViewModel;
using BusinessObject.ResourceModel.ViewModel.User;
using Microsoft.AspNetCore.Identity;
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
            CreateMap<IdentityUser, UserVM>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                .ForMember(dest => dest.RoleName, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<Product, ProductAddVM>().ReverseMap();
            CreateMap<ProductImport, Product>();
            CreateMap<Brand, BrandVM>().ReverseMap();
            CreateMap<Brand, BrandAddVM>().ReverseMap();
            CreateMap<Order, OrderVM>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailVM>()
                .ForMember(dest => dest.ProductName, otp => otp.MapFrom(src => src.Product.ProductName))
                .ReverseMap();
            CreateMap<Order, OrderUpdateVM>().ReverseMap();
        }
    }
}
