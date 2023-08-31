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
    public class UserAddressController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public UserAddressController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // LAY USERADDRESSES CHUA XOA
        [HttpGet]
        [Route("get-all_useraddresses")]
        public async Task<ActionResult<ListAddress>> Index(int? page, int? pageSize, bool? orderByDesc, string? search)
        {
            IQueryable<UserAddress> query = _context.UserAddresses
                .Include(u => u.User)
                .Where(p => p.DeletedAt == null);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.User.Username.Contains(search));
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

            var userAddresses = await query.ToListAsync();

            if (userAddresses == null || userAddresses.Count == 0)
            {
                return NotFound();
            }

            //Map
            var userAddressesDTOs = _mapper.Map<List<UserAddressDTO>>(userAddresses);

            var response = new ListAddress
            {
                UserAddresses = userAddressesDTOs,
                TotalPages = totalPages,
                TotalItems = totalItems,
            };

            return response;

        }

        // LAY USERADDRESSES DA XOA
        [HttpGet]
        [Route("get-all_deletedUserAddresses")]
        public async Task<ActionResult<ListAddress>> DeletedUserAddresses(int? page, int? pageSize, bool? orderByDesc, string? search)
        {
            IQueryable<UserAddress> query = _context.UserAddresses
                .Include(u => u.User)
                .Where(p => p.DeletedAt != null);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.User.Username.Contains(search));
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

            var deletedUserAddresses = await query.ToListAsync();

            if (deletedUserAddresses == null || deletedUserAddresses.Count == 0)
            {
                return NotFound();
            }

            //Map
            var deletedUserAddressesDTOs = _mapper.Map<List<UserAddressDTO>>(deletedUserAddresses);

            var response = new ListAddress
            {
                UserAddresses = deletedUserAddressesDTOs,
                TotalPages = totalPages,
                TotalItems = totalItems,
            };

            return response;

        }



        // TIM THEO ID
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<UserAddressDTO>> Get(int id)
        {
            var userAddress = await _context.UserAddresses
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
            

            if (userAddress == null || userAddress.DeletedAt != null)
            {
                return NotFound();
            }

            //Map 
            var userAddressDTO = _mapper.Map<UserAddressDTO>(userAddress);

            return Ok(userAddressDTO);
        }


        // TIM THEO USER_ID
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<UserAddressDTO>> Get_By_USERID(int userId)
        {
            var userAddress = await _context.UserAddresses
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.UserId == userId);


            if (userAddress == null || userAddress.DeletedAt != null)
            {
                return NotFound();
            }

            //Map 
            var userAddressDTO = _mapper.Map<UserAddressDTO>(userAddress);

            return Ok(userAddressDTO);
        }


        // TAO MOI USERADDRESS
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<UserAddressDTO>> Create(UserAddressDTO data)
        {
            if (ModelState.IsValid)
            {
                //Map 
                var userAddress = _mapper.Map<UserAddress>(data);

                // Set the CreatedAt property to the current date and time
                userAddress.CreatedAt = DateTime.UtcNow;

                _context.UserAddresses.Add(userAddress);
                await _context.SaveChangesAsync();

                // Map the created user address back to UserAddressDTO and return it in the response
                var createdUserAddressDTO = _mapper.Map<UserAddressDTO>(userAddress);
                return CreatedAtAction(nameof(Get), new { id = userAddress.Id }, createdUserAddressDTO);
            }
            return BadRequest();
        }

        // XOA VAO THUNG RAC
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteUserAddress(int id)
        {
            if (_context.UserAddresses == null)
            {
                return NotFound();
            }
            var userAddress = await _context.UserAddresses.FindAsync(id);
            if (userAddress == null)
            {
                return NotFound();
            }

            
            userAddress.DeletedAt = DateTime.UtcNow; //Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // XOA HẲN KHỎI THUNG RAC
        [HttpDelete]
        [Route("permanently deleted")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.UserAddresses == null)
            {
                return NotFound();
            }
            var userAddress = await _context.UserAddresses.FindAsync(id);
            if (userAddress == null)
            {
                return NotFound();
            }


            if (userAddress.DeletedAt == null)
            {
                return BadRequest("Cannot delete");
            }
            else
            {
                _context.UserAddresses.Remove(userAddress);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }


        // UPDATE 
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> PutUserAddress(int id, UserAddressDTO userAddressDTO)
        {
            if (id != userAddressDTO.Id)
            {
                return BadRequest("The id in the URL does not match the id in the request body.");
            }

            //Check if the userAddress with the given id exists in the database
            var userAddress = await _context.UserAddresses.FindAsync(id);
            if (userAddress == null)
            {
                return NotFound();
            }

            //Map
            _mapper.Map(userAddressDTO, userAddress);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAddresstExists(id))
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


        // DUA RA KHỎI THUNG RAC
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreUserAddress(int id)
        {
            var userAddress = await _context.UserAddresses.FindAsync(id);
            if (userAddress == null)
            {
                return NotFound();
            }

            // Check if the userAddress is already restored (DeletedAt is null)
            if (userAddress.DeletedAt == null)
            {
                return BadRequest("The userAddress is already restored.");
            }

            // Restore the userAddress by setting DeletedAt to null
            userAddress.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool UserAddresstExists(int id)
        {
            return (_context.UserAddresses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
