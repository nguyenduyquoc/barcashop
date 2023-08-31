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
    public class MatchKindController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public MatchKindController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // LAY MATCHKINDs CHUA XOA
        [HttpGet]
        [Route("get-all_matchKinds")]
        public async Task<ActionResult<IEnumerable<MatchKindDTO>>> Index()
        {
            var matchKinds = await _context.MatchKinds
                .Include(c => c.ProductImages).
                    ThenInclude(p => p.Product)
                .Where(c => c.DeletedAt == null)
                .ToListAsync();

            if (matchKinds == null || matchKinds.Count == 0)
            {
                return NotFound();
            }

            var matchKindDTOs = _mapper.Map<List<MatchKindDTO>>(matchKinds);

            return Ok(matchKindDTOs);
        }


        // LAY MATCHKINDS DA XOA
        [HttpGet]
        [Route("deleted")]
        public async Task<ActionResult<IEnumerable<MatchKindDTO>>> GetDeletedMatchKinds()
        {
            var deletedMatchKinds = await _context.MatchKinds
                .Where(c => c.DeletedAt != null) // Include only matchKinds that are soft-deleted
                .Include(c => c.ProductImages)
                    .ThenInclude(p => p.Product)
                .ToListAsync();

            if (deletedMatchKinds == null || deletedMatchKinds.Count == 0)
            {
                return NotFound();
            }

            // Map MatchKind entities to MatchKindDTO
            var deletedMatchKindDTOs = _mapper.Map<List<MatchKindDTO>>(deletedMatchKinds);
            return deletedMatchKindDTOs;
        }


        // TIM THEO ID
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<MatchKindDTO>> Get(int id)
        {
            var matchKind = await _context.MatchKinds
                .Include(c => c.ProductImages)
                    .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (matchKind == null || matchKind.DeletedAt != null)
            {
                return NotFound();
            }

            //Map MatchKind to MatchKindDTO
            var matchKindDTO = _mapper.Map<MatchKindDTO>(matchKind);

            return Ok(matchKindDTO);
        }



        // TAO MOI MATCHKIND
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<MatchKindDTO>> Create(MatchKindDTO data)
        {
            if (ModelState.IsValid)
            {
                //Check if matchKind with the same name already exists
                if (_context.MatchKinds.Any(c => c.MatchKindName == data.MatchKindName))
                {
                    return BadRequest("A matchkind with the same name already exists.");
                }
                //Map MatchKindDTO to MatchKind
                var matchKind = _mapper.Map<MatchKind>(data);

                // Set the CreatedAt property to the current date and time
                matchKind.CreatedAt = DateTime.UtcNow;

                _context.MatchKinds.Add(matchKind);
                await _context.SaveChangesAsync();

                // Map the created match kind back to MatchKindDTO and return it in the response
                var createdMatchKindDTO = _mapper.Map<MatchKindDTO>(matchKind);
                return CreatedAtAction(nameof(Get), new { id = matchKind.Id }, createdMatchKindDTO);
            }
            return BadRequest();
        }


        // XOA VAO THUNG RAC
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteMatchKind(int id)
        {
            if (_context.MatchKinds == null)
            {
                return NotFound();
            }
            var matchKind = await _context.MatchKinds.FindAsync(id);
            if (matchKind == null)
            {
                return NotFound();
            }

            //_context.MatchKinds.Remove(matchKind);
            matchKind.DeletedAt = DateTime.UtcNow; //Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // XOA HẲN KHOI THUNG RAC
        [HttpDelete]
        [Route("permanently deleted")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.MatchKinds == null)
            {
                return NotFound();
            }
            var matchKind = await _context.MatchKinds.FindAsync(id);
            if (matchKind == null)
            {
                return NotFound();
            }

            if (matchKind.DeletedAt == null)
            {
                return BadRequest("Cannot delete");
            }
            else
            {
                _context.MatchKinds.Remove(matchKind);
                await _context.SaveChangesAsync();
            }


            return NoContent();
        }

        // UPDATE 
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> PutMatchKind(int id, MatchKindDTO matchKindDTO)
        {
            if (id != matchKindDTO.Id)
            {
                return BadRequest("The id in the URL does not match the id in the request body.");
            }

            //Check if the match kind with the given id exists in the database
            var matchKind = await _context.MatchKinds.FindAsync(id);
            if (matchKind == null)
            {
                return NotFound();
            }

            //Map the properties from the MatchKindDTO to the existing MatchKind entity
            _mapper.Map(matchKindDTO, matchKind);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchKindExists(id))
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


        // DUA MATCHKIND RA KHỎI THUNG RAC
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreMatchKind(int id)
        {
            var matchKind = await _context.MatchKinds.FindAsync(id);
            if (matchKind == null)
            {
                return NotFound();
            }

            // Check if the matchKind is already restored (DeletedAt is null)
            if (matchKind.DeletedAt == null)
            {
                return BadRequest("The match kind is already restored.");
            }

            // Restore the match kind by setting DeletedAt to null
            matchKind.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool MatchKindExists(int id)
        {
            return (_context.MatchKinds?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
