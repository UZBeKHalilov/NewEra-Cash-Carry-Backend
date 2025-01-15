using AutoMapper;
using NewEraAPI.DTOs.Product_DTO;
using NewEraAPI.DTOs.CategoryDTO;
using NewEraAPI.Models;

namespace NewEraAPI.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Mapping for creating a product
            CreateMap<ProductCreateDTO, Product>()
                .ForMember(dest => dest.CategoryID, opt => opt.MapFrom(src => src.CategoryID))
                .ForMember(dest => dest.Category, opt => opt.Ignore()); // Ignore Category since we only have the ID to map

            // Mapping Product to ProtuctDTO
            CreateMap<Product, ProductGetDTO>()
                .ForMember(dest => dest.CategoryID, opt => opt.MapFrom(src => src.CategoryID));

            CreateMap<Category, CategoryCreateDTO>(); // THE FIXING BIG BUG PLEASE DONT TOUCH

        }
    }

}
