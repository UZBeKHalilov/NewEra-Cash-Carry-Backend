using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewEraAPI.Data;
using NewEraAPI.DTOs.CategoryDTO;
using NewEraAPI.Models;
using AutoMapper;
using System.Collections.Immutable;

namespace NewEraAPI.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly NewEraDBContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(NewEraDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET : api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryGetDTO>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryGetDTO>>(categories));
        }

        // GET : api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryGetDTO>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CategoryGetDTO>(category));
        }

        // POST : api/Categories
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CategoryGetDTO>> PostCategory(CategoryCreateDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.ID }, _mapper.Map<CategoryGetDTO>(category));
        }



        // PUT : api/Categories/5

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryCreateDTO categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _mapper.Map(categoryDto, category);  // Updates category with dto data

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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
        // DELETE: api/Categories/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.ID == id);
        }
    }

}
