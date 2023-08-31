using AutoMapper;
using Barca.DTOs;
using Barca.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Barca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FootballClubController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public FootballClubController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // LAY FootballClub CHUA XOA
        [HttpGet]
        [Route("get-all_FootballClub")]
        public async Task<ActionResult<ListFootballClub>> Index(int? page, int? pageSize, bool? orderByDesc, string? search)
        {
            IQueryable<FootballClub> query = _context.FootballClubs
                .Include(f => f.Products)
                    .ThenInclude(p => p.Brand)
                .Include(f => f.Products)
                    .ThenInclude(p => p.Category)
                .Where(c => c.DeletedAt == null);

            if(!string.IsNullOrEmpty(search) )
            {
                query = query.Where(f => f.Name.Contains(search));
            }

            // Calculate the total number of items matching the criteria
            int totalItems = await query.CountAsync();

            // Initialize variables for totalPages and itemsPerPage
            int? totalPages = null;
            int itemsPerPage = 0;

            // Apply sorting by CreateAt if the 'orderByDesc' parameter is provided and true
            if (orderByDesc.HasValue && orderByDesc.Value)
            {
                query = query.OrderByDescending(c => c.CreatedAt);
            }
            else
            {
                query = query.OrderBy(c => c.CreatedAt);
            }


            // Apply pagination if the 'page' and 'pageSize' parameters are provided
            if (page.HasValue && pageSize.HasValue)
            {
                int currentPage = page.Value;
                itemsPerPage = pageSize.Value;
                totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
                query = query.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage);
            }

            var footballClubs = await query.ToListAsync();

            if (footballClubs == null || footballClubs.Count == 0)
            {
                return NotFound();
            }


            //Map
            var footballClubDTOs = _mapper.Map<List<FootballClubDTO>>(footballClubs);

            var response = new ListFootballClub
            {
                FootballClubs = footballClubDTOs,
                TotalPages = totalPages,
                TotalItems = totalItems,
            };

            return response;
        }


        // LAY FOOTBALLCLUB DA XOA
        [HttpGet]
        [Route("deleted")]
        public async Task<ActionResult<ListFootballClub>> GetDeletedFootballClub(int? page, int? pageSize, bool? orderByDesc, string? search)
        {
            IQueryable<FootballClub> query = _context.FootballClubs
                .Include(f => f.Products)
                    .ThenInclude(p => p.Brand)
                .Include(f => f.Products)
                    .ThenInclude(p => p.Category)
                .Where(c => c.DeletedAt != null);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(f => f.Name.Contains(search));
            }

            // Calculate the total number of items matching the criteria
            int totalItems = await query.CountAsync();

            // Initialize variables for totalPages and itemsPerPage
            int? totalPages = null;
            int itemsPerPage = 0;

            // Apply sorting by CreateAt if the 'orderByDesc' parameter is provided and true
            if (orderByDesc.HasValue && orderByDesc.Value)
            {
                query = query.OrderByDescending(c => c.CreatedAt);
            }
            else
            {
                query = query.OrderBy(c => c.CreatedAt);
            }


            // Apply pagination if the 'page' and 'pageSize' parameters are provided
            if (page.HasValue && pageSize.HasValue)
            {
                int currentPage = page.Value;
                itemsPerPage = pageSize.Value;
                totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
                query = query.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage);
            }

            var deletedFootballClubs = await query.ToListAsync();

            if (deletedFootballClubs == null || deletedFootballClubs.Count == 0)
            {
                return NotFound();
            }


            //Map
            var deletedFootballClubsDTOs = _mapper.Map<List<FootballClubDTO>>(deletedFootballClubs);

            var response = new ListFootballClub
            {
                FootballClubs = deletedFootballClubsDTOs,
                TotalPages = totalPages,
                TotalItems = totalItems,
            };

            return response;
        }


        // TIM THEO ID
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<FootballClubDTO>> Get(int id)
        {
            
            var footballClub = await _context.FootballClubs
                .Include(f => f.Products)
                    .ThenInclude(p => p.Brand)
                .Include(f => f.Products)
                    .ThenInclude(p => p.Category) 
                .FirstOrDefaultAsync(f => f.Id == id);

            if (footballClub == null || footballClub.DeletedAt != null)
            {
                return NotFound();
            }

            //Map FootballClub entities to FootballClubDTO
            var footballClubDTO = _mapper.Map<FootballClubDTO>(footballClub);

            return Ok(footballClubDTO);
        }



        // TAO MOI FOOTBALLCLUB
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<FootballClubDTO>> Create(FootballClubDTO data)
        {
            if (ModelState.IsValid)
            {
                //Check if footballClub with the same name already exists
                if (_context.FootballClubs.Any(c => c.Name == data.Name))
                {
                    return BadRequest("A footballClub with the same name already exists.");
                }
                //Map FootballClubDTO to FootballClub
                var footballClub = _mapper.Map<FootballClub>(data);

                // Set the CreatedAt property to the current date and time
                footballClub.CreatedAt = DateTime.UtcNow;

                _context.FootballClubs.Add(footballClub);
                await _context.SaveChangesAsync();

                // Map the created footballClub back to FootballClubDTO and return it in the response
                var createdFootballClubDTO = _mapper.Map<FootballClubDTO>(footballClub);
                return CreatedAtAction(nameof(Get), new { id = footballClub.Id }, createdFootballClubDTO);
            }
            return BadRequest();
        }


        // Xoa VAO THUNG RAC
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteFootballClub(int id)
        {
            if (_context.FootballClubs == null)
            {
                return NotFound();
            }
            var footballClub = await _context.FootballClubs.FindAsync(id);
            if (footballClub == null)
            {
                return NotFound();
            }

            //_context.FootballClubs.Remove(footballClub);
            footballClub.DeletedAt = DateTime.UtcNow; //Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // Xoa HẢN KHỎI THUNG RAC
        [HttpDelete]
        [Route("permanently deleted")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.FootballClubs == null)
            {
                return NotFound();
            }
            var footballClub = await _context.FootballClubs.FindAsync(id);
            if (footballClub == null)
            {
                return NotFound();
            }


            if (footballClub.DeletedAt == null)
            {
                return BadRequest("Cannot delete");
            }
            else
            {
                _context.FootballClubs.Remove(footballClub);
                await _context.SaveChangesAsync();
            }


            return NoContent();
        }



        // UPDATE 
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> PutFootballClub(int id, FootballClubDTO footballClubDTO)
        {
            if (id != footballClubDTO.Id)
            {
                return BadRequest("The id in the URL does not match the id in the request body.");
            }

            //Check if the footballClub with the given id exists in the database
            var footballClub = await _context.FootballClubs.FindAsync(id);
            if (footballClub == null)
            {
                return NotFound();
            }

            //Map the properties from the FootballClubDTO to the existing FootballClub entity
            _mapper.Map(footballClubDTO, footballClub);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FootballClubExists(id))
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


        // DUA FootballClub RA KHỎI THUNG RAC
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreFootballClub(int id)
        {
            var footballClub = await _context.FootballClubs.FindAsync(id);
            if (footballClub == null)
            {
                return NotFound();
            }

            // Check if the FootballClub is already restored (DeletedAt is null)
            if (footballClub.DeletedAt == null)
            {
                return BadRequest("The FootballClub is already restored.");
            }

            // Restore the footballClub by setting DeletedAt to null
            footballClub.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool FootballClubExists(int id)
        {
            return (_context.FootballClubs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
