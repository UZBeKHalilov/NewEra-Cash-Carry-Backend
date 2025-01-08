using AutoMapper;
using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ECommerceAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ECommerceDbContext _context;
        private readonly IMapper _mapper;


        public ProductsController(ECommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Products
        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A list of products.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            // Mahsulotlarni Category bilan birga yuklash
            var products = await _context.Products
                                         .Include(p => p.Category) // Categoryni qo'shish
                                         .ToListAsync();

            // DTOga mapping qilish
            return Ok(_mapper.Map<IEnumerable<ProductDTO>>(products));
        }


        // GET: api/Products/5
        /// <summary>
        /// Retrieves a specific product by ID.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The requested product.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            // Mahsulotni Category bilan birga yuklash
            var product = await _context.Products
                                        .Include(p => p.Category) // Categoryni qo'shish
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // DTOga mapping qilish
            return _mapper.Map<ProductDTO>(product);
        }

        // GET: api/Products/search
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedList<ProductDTO>>> SearchProducts(
            [FromQuery] string? name,
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true,
            [FromQuery] bool stockOnly = false)
        {
            var query = _context.Products.AsQueryable();

            // Filter by name
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            // Filter by category
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Filter by Stock only
            if (stockOnly)
            {
                query = query.Where(p => p.Stock > 0);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = ascending
                    ? query.OrderBy(p => EF.Property<object>(p, sortBy))
                    : query.OrderByDescending(p => EF.Property<object>(p, sortBy));
            }



            // Include category information and apply pagination
            var totalItems = await query.CountAsync();
            var products = await query
                .Include(p => p.Category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productDTOs = _mapper.Map<List<ProductDTO>>(products);

            return Ok(new PaginatedList<ProductDTO>
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = productDTOs
            });
        }

        // POST: api/Products/
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct([FromBody] ProductCreateDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            if (string.IsNullOrEmpty(product.Name))
            {
                Log.Warning("Product creation failed due to missing name.");
                return BadRequest(new { Message = "Product name is required.", ErrorCode = "ERR_MISSING_PRODUCT_NAME" });
            }


            var category = await _context.Categories.FindAsync(productDto.CategoryId);
            if (category == null)
            {
                Log.Warning("Product creation failed due to non-existent category with ID {CategoryId}.", productDto.CategoryId);
                return BadRequest("The specified category does not exist.");
            }

            product.Category = category;

            _context.Products.Add(product);

            try
            {
                await _context.SaveChangesAsync();
                Log.Information("Product {ProductName} created with ID {ProductId}", product.Name, product.Id);
                var createdProduct = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == product.Id);

                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id },
                                               _mapper.Map<ProductDTO>(createdProduct));

            }
            catch (DbUpdateException ex)
            {
                Log.Error(ex, "Failed to save product {ProductName} to database.", product.Name);

                return StatusCode(500, "An error occurred while saving the product.");
            }

            //var savedProduct = await _context.Products
            //    .Include(p => p.Category)
            //    .FirstOrDefaultAsync(p => p.Id == product.Id);

            //return CreatedAtAction(nameof(GetProduct), new { id = savedProduct.Id },
            //                       _mapper.Map<ProductDTO>(savedProduct));
        }


        // POST: api/Products/5/upload-image/
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file, [FromServices] IWebHostEnvironment hostEnvironment)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            // Ensure the images directory exists
            var imagePath = Path.Combine(hostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
            }

            // Restrict file size (example: 5MB)
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
            {
                return BadRequest("File size exceeds the maximum limit of 5MB.");
            }
            
            // Generate a unique file name and save the file
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(imagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Cleanup old image if exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldImagePath = Path.Combine(hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Update the product's ImageUrl
            product.ImageUrl = $"/images/{fileName}";
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ProductDTO>(product));
        }


        // PUT: api/Products/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
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
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
