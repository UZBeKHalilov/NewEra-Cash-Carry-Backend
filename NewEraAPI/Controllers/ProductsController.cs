using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NewEraAPI.DTOs.Product_DTO;
using NewEraAPI.Models;
using NewEraAPI.Data;

namespace NewEraAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly NewEraDBContext _context;
        private readonly IMapper _mapper;


        public ProductsController(NewEraDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductGetDTO>>> GetProducts()
        {
            // Mahsulotlarni Category bilan birga yuklash
            var products = await _context.Products
                                         .Include(p => p.Category) // Include the related Category
                                         .ToListAsync();

            // DTOga mapping qilish
            return Ok(_mapper.Map<IEnumerable<ProductGetDTO>>(products));
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductGetDTO>> GetProduct(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ID == id);

            if (product == null)
            {
                return NotFound();
            }
            var productGetDto = _mapper.Map<ProductGetDTO>(product);

            return productGetDto;

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ProductGetDTO>> PostProduct([FromBody] ProductCreateDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            var category = await _context.Categories.FindAsync(productDto.CategoryID);
            if (category == null)
            {
                return BadRequest("The specified category does not exist.");
            }

            product.Category = category;

            _context.Products.Add(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while saving the product. {ex}");
            }

            var savedProduct = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ID == product.ID);

            return CreatedAtAction(nameof(GetProduct), new { id = savedProduct.ID },
                                   _mapper.Map<ProductGetDTO>(savedProduct));
        }
        // PUT: api/Products/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ID)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }
    }
}
