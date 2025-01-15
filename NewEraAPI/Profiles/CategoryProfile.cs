using AutoMapper;
using NewEraAPI.DTOs.CategoryDTO;
using NewEraAPI.DTOs.Product_DTO;
using NewEraAPI.DTOs.ProductDTO;
using NewEraAPI.Models;

namespace NewEraAPI.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile() 
        {
            // Mapping for creating a Category
            CreateMap<CategoryCreateDTO, Category>();

            // Mapping for returning a Category
            CreateMap<Category, CategoryGetDTO>();

        }
    }
}
