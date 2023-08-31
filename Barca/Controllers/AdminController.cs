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
    public class AdminController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public AdminController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // LAY ADMINS CHUA XOA
        [HttpGet]
        [Route("get-all_admins")]
        public async Task<ActionResult<IEnumerable<AdminDTO>>> Index()
        {
            var admins = await _context.Admins
                .Include(c => c.User)
                .Where(c => c.DeletedAt == null)
                .ToListAsync();

            if (admins == null || admins.Count == 0)
            {
                return NotFound();
            }

            var adminDTOs = _mapper.Map<List<AdminDTO>>(admins);

            return Ok(adminDTOs);
        }



        // LAY ADMINS DA XOA
        [HttpGet]
        [Route("deleted")]
        public async Task<ActionResult<IEnumerable<AdminDTO>>> GetDeletedAdmins()
        {
            var deletedAdmins = await _context.Admins
                .Where(c => c.DeletedAt != null)
                .ToListAsync();

            if (deletedAdmins == null || deletedAdmins.Count == 0)
            {
                return NotFound();
            }

            // Map Admin entities to AdminDTO
            var deletedAdminDTOs = _mapper.Map<List<AdminDTO>>(deletedAdmins);
            return deletedAdminDTOs;
        }


        // TIM THEO ID
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<AdminDTO>> Get(int id)
        {
            var admin = await _context.Admins.FindAsync(id);

            if (admin == null || admin.DeletedAt != null)
            {
                return NotFound();
            }

            //Map Admin to AdminDTO
            var adminDTO = _mapper.Map<AdminDTO>(admin);

            return Ok(adminDTO);
        }



        // TAO MOI ADMIN
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<AdminDTO>> Create(AdminDTO data)
        {
            if (ModelState.IsValid)
            {
                //Check if admin with the same userId already exists
                if (_context.Admins.Any(c => c.UserId == data.UserId))
                {
                    return BadRequest("A admin with the same userID already exists.");
                }
                //Map AdminDTO to Admin
                var admin = _mapper.Map<Admin>(data);

                // Set the CreatedAt property to the current date and time
                admin.CreatedAt = DateTime.UtcNow;

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                // Map the created admin back to AdminDTO and return it in the response
                var createdAdminDTO = _mapper.Map<AdminDTO>(admin);
                return CreatedAtAction(nameof(Get), new { id = admin.Id }, createdAdminDTO);
            }
            return BadRequest();
        }



        // XOA VAO THUNG RAC
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            if (_context.Admins == null)
            {
                return NotFound();
            }
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            //_context.Admins.Remove(admin);
            admin.DeletedAt = DateTime.UtcNow; //Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // XOA HAWNR KHOI THUNG RAC
        [HttpDelete]
        [Route("permanently deleted")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Admins == null)
            {
                return NotFound();
            }
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            if (admin.DeletedAt == null)
            {
                return BadRequest("Cannot delete because because this admin is not in the temporarily deleted item ");
            }
            else
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }


        // UPDATE 
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> PutAdmin(int id, AdminDTO adminDTO)
        {
            if (id != adminDTO.Id)
            {
                return BadRequest("The id in the URL does not match the id in the request body.");
            }

            //Check if the admin with the given id exists in the database
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            //Map the properties from the AdminDTO to the existing Admin entity
            _mapper.Map(adminDTO, admin);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(id))
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

        // DUA ADMIN RA KHỎI THUNG RAC
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            // Check if the admin is already restored (DeletedAt is null)
            if (admin.DeletedAt == null)
            {
                return BadRequest("The admin is already restored.");
            }

            // Restore the admin by setting DeletedAt to null
            admin.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool AdminExists(int id)
        {
            return (_context.Admins?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
