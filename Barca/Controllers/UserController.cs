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
    public class UserController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public UserController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // LAY USERS CHUA XOA
        [HttpGet]
        [Route("get-all_users")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Index()
        {
            var users = await _context.Users
                .Where(u => u.DeletedAt == null)
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound();
            }

            var userDTOs = _mapper.Map<List<UserDTO>>(users);

            return Ok(userDTOs);
        }


        // LAY USERS DA XOA
        [HttpGet]
        [Route("deleted")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetDeletedUsers()
        {
            var deletedUsers = await _context.Users
                .Where(u => u.DeletedAt != null) // Include only users that are soft-deleted
                .ToListAsync();

            if (deletedUsers == null || deletedUsers.Count == 0)
            {
                return NotFound();
            }

            // Map Category entities to CategoryDTO
            var deletedUserDTOs = _mapper.Map<List<UserDTO>>(deletedUsers);
            return deletedUserDTOs;
        }


        // TIM THEO ID
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null || user.DeletedAt != null)
            {
                return NotFound();
            }

            //Map User to UserDTO
            var userDTO = _mapper.Map<UserDTO>(user);

            return Ok(userDTO);
        }


        // TAO MOI USER
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<UserDTO>> Create(UserDTO data)
        {
            if (ModelState.IsValid)
            {
                //Check if user with the same username already exists
                if (_context.Users.Any(c => c.Username == data.Username))
                {
                    return BadRequest("A category with the same username already exists.");
                }
                //Map UserDTO to User
                var user = _mapper.Map<User>(data);

                // Set the CreatedAt property to the current date and time
                user.CreatedAt = DateTime.UtcNow;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Map the created user back to UserDTO and return it in the response
                var createdUserDTO = _mapper.Map<UserDTO>(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, createdUserDTO);
            }
            return BadRequest();
        }


        // XOA VAO THUNG RAC
        [HttpDelete]
        [Route("permanently deleted")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            //_context.Users.Remove(user);
            user.DeletedAt = DateTime.UtcNow; //Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // UPDATE 
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> PutUser(int id, UserDTO userDTO)
        {
            if (id != userDTO.Id)
            {
                return BadRequest("The id in the URL does not match the id in the request body.");
            }

            //Check if the user with the given id exists in the database
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            //Map the properties from the UserDTO to the existing User entity
            _mapper.Map(userDTO, user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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


        // DUA USER RA KHỎI THUNG RAC
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user is already restored (DeletedAt is null)
            if (user.DeletedAt == null)
            {
                return BadRequest("The user is already restored.");
            }

            // Restore the user by setting DeletedAt to null
            user.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
