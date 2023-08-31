using AutoMapper;
using Barca.DTOs;
using Barca.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public CategoryController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // LAY CATEGORIES CHUA XOA
        [HttpGet]
        [Route("get-all_categories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.Brand)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Club)
                .Where(c => c.DeletedAt == null)
                .ToListAsync();

           
            if (categories == null || categories.Count == 0)
            {
                return NotFound();
            }

            var categoryDTOs = _mapper.Map<List<CategoryDTO>>(categories);

            return Ok(categoryDTOs);
        }


        // LAY CATEGORIES DA XOA
        [HttpGet]
        [Route("deleted")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetDeletedCategories()
        {
            var deletedCategories = await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.Brand)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Club)
                .Where(c => c.DeletedAt != null)
                .ToListAsync();

            if (deletedCategories == null || deletedCategories.Count == 0)
            {
                return NotFound();
            }

            // Map Category entities to CategoryDTO
            var deletedCategoryDTOs = _mapper.Map<List<CategoryDTO>>(deletedCategories);
            return deletedCategoryDTOs;
        }


        // TIM THEO ID
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<CategoryDTO>> Get(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.Brand)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Club)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (category == null || category.DeletedAt != null)
            {
                return NotFound();
            }

            //Map Category to CategoryDTO
            var categoryDTO = _mapper.Map<CategoryDTO>(category);

            return Ok(categoryDTO);
        }



        // TAO MOI CATEGORY
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<CategoryDTO>> Create(CategoryDTO data)
        {
            if (ModelState.IsValid)
            {
                //Check if category with the same name already exists
                if (_context.Categories.Any(c => c.Name == data.Name))
                {
                    return BadRequest("A category with the same name already exists.");
                }
                //Map CategoryDTO to Category
                var category = _mapper.Map<Category>(data);

                // Set the CreatedAt property to the current date and time
                category.CreatedAt = DateTime.UtcNow;

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Map the created category back to CategoryDTO and return it in the response
                var createdCategoryDTO = _mapper.Map<CategoryDTO>(category);
                return CreatedAtAction(nameof(Get), new { id = category.Id }, createdCategoryDTO);
            }
            return BadRequest();
        }


        // Xoa VAO THUNG RAC
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            //_context.Categories.Remove(category);
            category.DeletedAt = DateTime.UtcNow; //Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Xoa HẲN KHOI THUNG RAC
        [HttpDelete]
        [Route("permanently deleted")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            if (category.DeletedAt == null)
            {
                return BadRequest("Cannot delete");
            }
            else
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }


            return NoContent();
        }

        // UPDATE 
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> PutCategory(int id, CategoryDTO categoryDTO)
        {
            if (id != categoryDTO.Id)
            {
                return BadRequest("The id in the URL does not match the id in the request body.");
            }

            //Check if the category with the given id exists in the database
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            //Map the properties from the CategoryDTO to the existing Category entity
            _mapper.Map(categoryDTO, category);

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


        // DUA CATEGORY RA KHỎI THUNG RAC
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Check if the category is already restored (DeletedAt is null)
            if (category.DeletedAt == null)
            {
                return BadRequest("The category is already restored.");
            }

            // Restore the category by setting DeletedAt to null
            category.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
