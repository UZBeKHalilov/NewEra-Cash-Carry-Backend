using AutoMapper;
using NewEraAPI.DTOs.CategoryDTO;
using NewEraAPI.DTOs.CustomerDTO;
using NewEraAPI.DTOs.OrderDTO;
using NewEraAPI.DTOs.Product_DTO;
using NewEraAPI.Models;

namespace NewEraAPI.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderCreateDTO, Order>()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItemCreateDTO, OrderItem>()
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<Order, OrderGetDTO>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemGetDTO>();
            CreateMap<Customer, CustomerGetDTO>();
            CreateMap<Product, ProductGetDTO>();
            CreateMap<Category, CategoryGetDTO>();
        }
    }
}
