using NewEraAPI.DTOs.Product_DTO;

namespace NewEraAPI.DTOs.OrderDTO
{
    public class OrderItemGetDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductGetDTO Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
