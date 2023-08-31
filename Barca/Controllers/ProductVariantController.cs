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
    public class ProductVariantController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public ProductVariantController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // LAY PRODUCTVARIANT CHUA XOA
        [HttpGet]
        [Route("get-all_productVariant")]
        public async Task<ActionResult<IEnumerable<ProductVariantDTO>>> Index()
        {
            var productVariant = await _context.ProductVariants
                .Include(p => p.MatchKind)
                .Include(p => p.Size)
                .Include(p => p.Product)
                .Where(c => c.DeletedAt == null)
                .ToListAsync();

            if (productVariant == null || productVariant.Count == 0)
            {
                return NotFound();
            }

            var productVariantDTOs = _mapper.Map<List<ProductVariantDTO>>(productVariant);

            return Ok(productVariantDTOs);
        }




        // LAY PRODUCTVARIANT DA XOA
        [HttpGet]
        [Route("deleted")]
        public async Task<ActionResult<IEnumerable<ProductVariantDTO>>> GetDeletedProductVariants()
        {
            var deletedProductVariants = await _context.ProductVariants
                .Where(c => c.DeletedAt != null) // Include only productVariants that are soft-deleted
                .ToListAsync();

            if (deletedProductVariants == null || deletedProductVariants.Count == 0)
            {
                return NotFound();
            }

            // Map ProductVariant entities to ProductVariantDTO
            var deletedProductVariantDTOs = _mapper.Map<List<ProductVariantDTO>>(deletedProductVariants);
            return deletedProductVariantDTOs;
        }




    }
}
