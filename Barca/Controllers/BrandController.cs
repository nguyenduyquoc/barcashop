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
    public class BrandController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public BrandController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // LAY BRAND CHUA XOA
        [HttpGet]
        [Route("get_all_brand")]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> Index()
        {
            var brands = await _context.Brands
                .Include(b => b.Products)
                    .ThenInclude(p => p.Club)
                .Include(b => b.Products)
                    .ThenInclude(p => p.Category)
                .Where(b => b.DeletedAt == null)
                .ToListAsync();


            if (brands == null || brands.Count == 0)
            {
                return NotFound();
            }

            var brandDTOs = _mapper.Map<List<BrandDTO>>(brands);

            return Ok(brandDTOs);
        }


        // LAY BRANDS DA XOA
        [HttpGet]
        [Route("get_brand_deleted")]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetBrandDeleted()
        {
            var brandsDeleted = await _context.Brands
                 .Include(b => b.Products)
                     .ThenInclude(p => p.Club)
                 .Include(b => b.Products)
                     .ThenInclude(p => p.Category)
                 .Where(b => b.DeletedAt != null)
                 .ToListAsync();

            if (brandsDeleted == null || brandsDeleted.Count == 0)
            {
                return NotFound();
            }

            var brandDTOs = _mapper.Map<List<BrandDTO>>(brandsDeleted);

            return Ok(brandDTOs);
        }


        //TIM THEO ID
        [HttpGet]
        [Route("get_by_id")]
        public async Task<ActionResult<BrandDTO>> Get(int id)
        {
            var brand = await _context.Brands
                .Include(b => b.Products)
                    .ThenInclude(c => c.Category)
                .Include(b => b.Products)
                    .ThenInclude(c=> c.Club)
                .FirstOrDefaultAsync(b => b.Id == id);


            if (brand == null || brand.DeletedAt != null)
            {
                return NotFound();
            }

            //Map Brand to BrandDTO
            var brandDTO = _mapper.Map<BrandDTO>(brand);

            return Ok(brandDTO);
        }


        // TAO MOI BRAND
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<BrandDTO>> Create(BrandDTO data)
        {
            if (ModelState.IsValid)
            {
                //Check if brand with the same name already exists
                if (_context.Brands.Any(b => b.Name == data.Name))
                {
                    return BadRequest("A category with the same name already exists.");
                }
                ////Map Brand to BrandDTO
                var brand = _mapper.Map<Brand>(data);

                // Set the CreatedAt property to the current date and time
                brand.CreatedAt = DateTime.UtcNow;

                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();

                // Map the created category back to BrandDTO and return it in the response
                var createdBrandDTO = _mapper.Map<BrandDTO>(brand);
                return CreatedAtAction(nameof(Get), new { id = brand.Id }, createdBrandDTO);
            }
            return BadRequest();
        }


        // Xoa VAO THUNG RAC
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            if (_context.Brands == null)
            {
                return NotFound();
            }
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            //_context.Brands.Remove(brand);
            brand.DeletedAt = DateTime.UtcNow; //Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // Xoa HẲN KHOI THUNG RAC
        [HttpDelete]
        [Route("dpermanently deleted")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Brands == null)
            {
                return NotFound();
            }
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            if (brand.DeletedAt == null)
            {
                return BadRequest("Cannot delete");
            }
            else
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }


        // UPDATE 
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> PutBrand(int id, BrandDTO brandDTO)
        {
            if (id != brandDTO.Id)
            {
                return BadRequest("The id in the URL does not match the id in the request body.");
            }

            //Check if the brand with the given id exists in the database
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            //Map the properties from the BrandDTO to the existing Brand entity
            _mapper.Map(brandDTO, brand);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(id))
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

        // DUA BRAND RA KHOI THUNG RAC
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            // Check if the brand is already restored (DeletedAt is null)
            if (brand.DeletedAt == null)
            {
                return BadRequest("The category is already restored.");
            }

            // Restore the brand by setting DeletedAt to null
            brand.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BrandExists(int id)
        {
            return (_context.Brands?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
