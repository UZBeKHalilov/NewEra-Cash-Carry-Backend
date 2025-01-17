using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NewEraAPI.DTOs.Product_DTO;
using NewEraAPI.DTOs;
using NewEraAPI.Models;
using NewEraAPI.Data;
using Serilog;

namespace NewEraAPI.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]

    public class ProductsV2Controller : ControllerBase
    {
        private readonly NewEraDBContext _context;
        private readonly IMapper _mapper;


        public ProductsV2Controller(NewEraDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets products for API version 2.0.
        /// </summary>
        /// <returns>A message indicating this is version 2.0 of the Products API.</returns>
         // GET: api/Products/search
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedList<ProductGetDTO>>> SearchProducts(
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
                query = query.Where(p => p.CategoryID == categoryId);
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

            var productDTOs = _mapper.Map<List<ProductGetDTO>>(products);

            return Ok(new PaginatedList<ProductGetDTO>
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = productDTOs
            });
        }

    }
}