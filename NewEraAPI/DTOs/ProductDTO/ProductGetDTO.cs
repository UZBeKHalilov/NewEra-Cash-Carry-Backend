using NewEraAPI.DTOs.CategoryDTO;
namespace NewEraAPI.DTOs.Product_DTO
{
    public class ProductGetDTO : IBaseGetDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryID { get; set; }
        public CategoryCreateDTO Category { get; set; }
    }
}
