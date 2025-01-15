namespace NewEraAPI.DTOs.OrderDTO
{
    public class OrderCreateDTO
    {
        public int CustomerId { get; set; }
        public ICollection<OrderItemCreateDTO> OrderItems { get; set; }

    }
}
