using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

namespace ECommerceAPI.Profiles
{
    public class OrderProfile : Profile
    {
        //public OrderProfile()
        //{
        //    CreateMap<OrderCreateDTO, Order>()
        //        .ForMember(dest => dest.Customer, opt => opt.Ignore())
        //        .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

        //    CreateMap<OrderItemCreateDTO, OrderItem>()
        //        .ForMember(dest => dest.Order, opt => opt.Ignore())
        //        .ForMember(dest => dest.Product, opt => opt.Ignore());
        //}

        public OrderProfile()
        {
            CreateMap<OrderCreateDTO, Order>()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
                //.ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus));

            CreateMap<OrderItemCreateDTO, OrderItem>()
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemDTO>();
            CreateMap<Customer, CustomerDTO>();
            CreateMap<Product, ProductDTO>();
            CreateMap<Category, CategoryDTO>();
        }
    }
}
