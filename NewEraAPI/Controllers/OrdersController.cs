
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using NewEraAPI.Data;
using NewEraAPI.DTOs.OrderDTO;
using NewEraAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewEraAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly NewEraDBContext _context;
        private readonly IMapper _mapper;

        public OrdersController(NewEraDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderGetDTO>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<OrderGetDTO>>(orders));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderGetDTO>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OrderGetDTO>(order));
        }


        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<OrderGetDTO>> PostOrder(OrderCreateDTO orderDto)
        {
            // Validate customer existence
            var customer = await _context.Customers.FindAsync(orderDto.CustomerId);
            if (customer == null)
            {
                return BadRequest("Customer not found.");
            }

            var order = new Order
            {
                CustomerId = orderDto.CustomerId,
                OrderDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;
            foreach (var itemDto in orderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null || product.Stock < itemDto.Quantity)
                {
                    return BadRequest($"Insufficient stock for product ID {itemDto.ProductId}");
                }

                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    Price = product.Price * itemDto.Quantity,
                    Product = product  // This will be ignored by EF when saving
                };

                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.Price;
                product.Stock -= itemDto.Quantity; // Deduct stock
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Optionally, fetch the order with related data for return
            var result = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, result);
        }

        // PUT and DELETE methods are left out for brevity. Implement similar to the GET and POST methods above.

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(o => o.Id == id);
        }
    }
}
