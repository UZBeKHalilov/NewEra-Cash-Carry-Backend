using AutoMapper;

using NewEraAPI.DTOs.CustomerDTO;
using NewEraAPI.Models;

namespace NewEraAPI.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile() 
        {
            CreateMap<CustomerCreateDTO, Customer>();
            CreateMap<Customer, CustomerGetDTO>();
        }
    }
}
