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
    public class ProductImageController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public ProductImageController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // LAY PRODUCTIMAGES CHUA XOA
        [HttpGet]
        [Route("get-all_productsimage")]
        public async Task<ActionResult<ListProductImages>> Index(int? page, int? pageSize, bool? orderByDesc, string? search)
        {
            IQueryable<ProductImage> query = _context.ProductImages
                .Include(p => p.MatchKind)
                .Include(p => p.Product)
                .Where(p => p.DeletedAt == null);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(f => f.Product.Name.Contains(search));
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

            var productImages = await query.ToListAsync();

            if (productImages == null || productImages.Count == 0)
            {
                return NotFound();
            }

            //Map
            var productImagesDTOs = _mapper.Map<List<ProductImageDTO>>(productImages);

            var response = new ListProductImages
            {
                ProductImages = productImagesDTOs,
                TotalPages = totalPages,
                TotalItems = totalItems,
            };

            return response;

        }


        // LAY PRODUCTIMAGES CHUA XOA
        [HttpGet]
        [Route("get-all_deletedproductsimage")]
        public async Task<ActionResult<ListProductImages>> GetDeletedProductImages(int? page, int? pageSize, bool? orderByDesc, string? search)
        {
            IQueryable<ProductImage> query = _context.ProductImages
                .Include(p => p.MatchKind)
                .Include(p => p.Product)
                .Where(p => p.DeletedAt != null);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(f => f.Product.Name.Contains(search));
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

            var deletedProductImages = await query.ToListAsync();

            if (deletedProductImages == null || deletedProductImages.Count == 0)
            {
                return NotFound();
            }

            //Map
            var productImagesDTOs = _mapper.Map<List<ProductImageDTO>>(deletedProductImages);

            var response = new ListProductImages
            {
                ProductImages = productImagesDTOs,
                TotalPages = totalPages,
                TotalItems = totalItems,
            };

            return response;

        }

        // TIM THEO ID
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<ProductImageDTO>> Get(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);

            if (productImage == null || productImage.DeletedAt != null)
            {
                return NotFound();
            }

            //Map 
            var productImageDTO = _mapper.Map<ProductImageDTO>(productImage);

            return Ok(productImageDTO);
        }



        // TAO MOI PRODUCTIMAGE
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<ProductImageDTO>> Create(ProductImageDTO data)
        {
            if (ModelState.IsValid)
            {
                //Check if productImage with the same ImagePath already exists
                if (_context.ProductImages.Any(c => c.ImagePath == data.ImagePath))
                {
                    return BadRequest("A productImage with the same image path already exists.");
                }
                //Map 
                var productImage = _mapper.Map<ProductImage>(data);

                // Set the CreatedAt property to the current date and time
                productImage.CreatedAt = DateTime.UtcNow;

                _context.ProductImages.Add(productImage);
                await _context.SaveChangesAsync();

                
                var createdProductImageDTO = _mapper.Map<ProductImageDTO>(productImage);
                return CreatedAtAction(nameof(Get), new { id = productImage.Id }, createdProductImageDTO);
            }
            return BadRequest();
        }



        // XOA VAO THUNG RAC
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.ProductImages == null)
            {
                return NotFound();
            }
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }

            //_context.ProductImages.Remove(productImage);
            productImage.DeletedAt = DateTime.UtcNow; //Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // Xoa HẲN KHỎI THUNG RAC
        [HttpDelete]
        [Route("permanently deleted")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.ProductImages == null)
            {
                return NotFound();
            }
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }


            if (productImage.DeletedAt == null)
            {
                return BadRequest("Cannot delete");
            }
            else
            {
                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }


        // UPDATE 
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> PutProduct(int id, ProductImageDTO productImageDTO)
        {
            if (id != productImageDTO.Id)
            {
                return BadRequest("The id in the URL does not match the id in the request body.");
            }

            //Check if the product with the given id exists in the database
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }

            //Map
            _mapper.Map(productImageDTO, productImage);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductImageExists(id))
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

        // DUA ProductImage RA KHỎI THUNG RAC
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreProductImage(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }

            // Check if the productImage is already restored (DeletedAt is null)
            if (productImage.DeletedAt == null)
            {
                return BadRequest("The product Image is already restored.");
            }

            // Restore the productImage by setting DeletedAt to null
            productImage.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool ProductImageExists(int id)
        {
            return (_context.ProductImages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
