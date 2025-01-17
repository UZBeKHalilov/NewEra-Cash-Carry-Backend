using NewEraAPI.DTOs.CustomerDTO;
using NewEraAPI.DTOs;

namespace NewEraAPI.DTOs.OrderDTO
{
    public class OrderGetDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public CustomerGetDTO Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemGetDTO> OrderItems { get; set; }
    }
}
