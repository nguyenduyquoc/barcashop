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
    public class UserDiscountController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public UserDiscountController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        /*// LAY USERDISCOUNTS CHUA XOA
        [HttpGet]
        [Route("get-all_userDiscounts")]
        public async Task<ActionResult<ListUserDiscountCode>> Index(int? page, int? pageSize, bool? orderByDesc, string? search)
        {
            IQueryable<UserDiscount> query = _context.UserDiscounts
                .Include(p => p.User)
                .Include(p => p.Discount)
                .Where(p => p.DeletedAt == null);

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

            var products = await query.ToListAsync();

            if (products == null || products.Count == 0)
            {
                return NotFound();
            }

            //Map
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);

            var response = new ListProduct
            {
                Products = productDTOs,
                TotalPages = totalPages,
                TotalItems = totalItems,
            };

            return response;

        }*/
    }
}
