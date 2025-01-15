using NewEraAPI.Models;
using NewEraAPI.DTOs.Product_DTO;

namespace NewEraAPI.DTOs.CategoryDTO
{
    public class CategoryGetDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
