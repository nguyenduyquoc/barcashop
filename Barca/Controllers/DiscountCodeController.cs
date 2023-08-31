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
    public class DiscountCodeController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public DiscountCodeController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // LAY DISCOUNTCODE CHUA XOA
        [HttpGet]
        [Route("get-all_discountCodes")]
        public async Task<ActionResult<IEnumerable<DiscountCodeDTO>>> Index()
        {
            var discountCodes = await _context.DiscountCodes
                .Where(c => c.DeletedAt == null)
                .ToListAsync();

            if (discountCodes == null || discountCodes.Count == 0)
            {
                return NotFound();
            }

            var discountCodeDTOs = _mapper.Map<List<DiscountCodeDTO>>(discountCodes);

            return Ok(discountCodeDTOs);
        }



        // LAY DISCOUNTCODE DA XOA
        [HttpGet]
        [Route("deleted")]
        public async Task<ActionResult<IEnumerable<DiscountCodeDTO>>> GetDeletedDiscountCodes()
        {
            var deletedDiscountCodes = await _context.DiscountCodes
                .Where(c => c.DeletedAt != null) // Include only discountCodes that are soft-deleted
                .ToListAsync();

            if (deletedDiscountCodes == null || deletedDiscountCodes.Count == 0)
            {
                return NotFound();
            }

            // Map DiscountCode entities to DiscountCodeDTO
            var deletedDiscountCodeDTOs = _mapper.Map<List<DiscountCodeDTO>>(deletedDiscountCodes);
            return deletedDiscountCodeDTOs;
        }


        // TIM THEO ID
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<DiscountCodeDTO>> Get(int id)
        {
            var discountCode = await _context.DiscountCodes.FindAsync(id);

            if (discountCode == null || discountCode.DeletedAt != null)
            {
                return NotFound();
            }

            //Map DiscountCode to DiscountCodeDTO
            var discountCodeDTO = _mapper.Map<DiscountCodeDTO>(discountCode);

            return Ok(discountCodeDTO);
        }



        // TAO MOI DISCOUNTCODE
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<DiscountCodeDTO>> Create(DiscountCodeDTO data)
        {
            if (ModelState.IsValid)
            {
                //Check if discountCode with the same name already exists
                if (_context.DiscountCodes.Any(c => c.Name == data.Name))
                {
                    return BadRequest("A discountCode with the same name already exists.");
                }
                //Map DiscountDTO to DiscountCode
                var discountCode = _mapper.Map<DiscountCode>(data);

                // Set the CreatedAt property to the current date and time
                discountCode.CreatedAt = DateTime.UtcNow;

                _context.DiscountCodes.Add(discountCode);
                await _context.SaveChangesAsync();

                // Map the created discountCode back to DiscountCodeDTO and return it in the response
                var createdDiscountCodeDTO = _mapper.Map<DiscountCodeDTO>(discountCode);
                return CreatedAtAction(nameof(Get), new { id = discountCode.Id }, createdDiscountCodeDTO);
            }
            return BadRequest();
        }


        // XOA VAO THUNG RAC
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteDiscountCode(int id)
        {
            if (_context.DiscountCodes == null)
            {
                return NotFound();
            }
            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound();
            }

            //_context.DiscountCodes.Remove(discountCode);
            discountCode.DeletedAt = DateTime.UtcNow; //Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // XOA HẲN KHOI THUNG RAC
        [HttpDelete]
        [Route("permanently deleted")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.DiscountCodes == null)
            {
                return NotFound();
            }
            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound();
            }

            if (discountCode.DeletedAt == null)
            {
                return BadRequest("Cannot delete");
            }
            else
            {
                _context.DiscountCodes.Remove(discountCode);
                await _context.SaveChangesAsync();
            }


            return NoContent();
        }

        // UPDATE 
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> PutDiscountCode(int id, DiscountCodeDTO discountCodeDTO)
        {
            if (id != discountCodeDTO.Id)
            {
                return BadRequest("The id in the URL does not match the id in the request body.");
            }

            //Check if the discountCode with the given id exists in the database
            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound();
            }

            //Map the properties from the DiscountCodeDTO to the existing DiscountCode entity
            _mapper.Map(discountCodeDTO, discountCode);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscountCodeExists(id))
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


        // DUA DISCOUNTCODE RA KHỎI THUNG RAC
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreDiscountCode(int id)
        {
            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound();
            }

            // Check if the discountCode is already restored (DeletedAt is null)
            if (discountCode.DeletedAt == null)
            {
                return BadRequest("The discountCode is already restored.");
            }

            // Restore the discountCode by setting DeletedAt to null
            discountCode.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool DiscountCodeExists(int id)
        {
            return (_context.DiscountCodes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
