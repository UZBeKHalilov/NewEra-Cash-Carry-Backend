namespace ECommerceAPI.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public CustomerDTO Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }

    public class OrderCreateDTO
    {
        public int CustomerId { get; set; }
        //public string PaymentStatus { get; set; } = "Pending";
        public ICollection<OrderItemCreateDTO> OrderItems { get; set; }
    }


    public class OrderItemCreateDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductDTO Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }


}
