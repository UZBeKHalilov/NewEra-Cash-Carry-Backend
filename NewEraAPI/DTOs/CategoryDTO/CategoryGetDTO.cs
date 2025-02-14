using NewEraAPI.Models;

namespace NewEraAPI.DTOs.CategoryDTO
{
    public class CategoryGetDTO : IBaseGetDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
