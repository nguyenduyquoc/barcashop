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
    public class ProductController : ControllerBase
    {
        private readonly BarcashopContext _context;
        private readonly IMapper _mapper;

        public ProductController(BarcashopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // LAY PRODUCTS CHUA XOA
        [HttpGet]
        [Route("get-all_products")]
        public async Task<ActionResult<ListProduct>> Index(int? page, int? pageSize, bool? orderByDesc, string? search)
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Club)
                .Include(p => p.OrderProducts)
                .Include(p => p.ProductImages)
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

        }


        // LAY PRODUCTS DA XOA
        [HttpGet]
        [Route("deleted")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetDeletedProducts()
        {
            var deletedProducts = await _context.Products
                .Where(c => c.DeletedAt != null)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Club)
                .Include(p => p.ProductImages)
                .ToListAsync();

            if (deletedProducts == null || deletedProducts.Count == 0)
            {
                return NotFound();
            }

            // Map Product entities to ProductDTO
            var deletedProductDTOs = _mapper.Map<List<ProductDTO>>(deletedProducts);
            return deletedProductDTOs;
        }


        // TIM THEO ID
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Club)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || product.DeletedAt != null)
            {
                return NotFound();
            }

            //Map Product entities to ProductDTO
            var productDTO = _mapper.Map<ProductDTO>(product);

            return Ok(productDTO);
        }


        // TAO MOI PRODUCT
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<ProductDTO>> Create(ProductDTO data)
        {
            if (ModelState.IsValid)
            {
                //Check if product with the same name already exists
                if (_context.Products.Any(c => c.Name == data.Name))
                {
                    return BadRequest("A product with the same name already exists.");
                }
                //Map Product entities to ProductDTO
                var product = _mapper.Map<Product>(data);

                // Set the CreatedAt property to the current date and time
                product.CreatedAt = DateTime.UtcNow;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Map the created product back to ProductDTO and return it in the response
                var createdProductDTO = _mapper.Map<ProductDTO>(product);
                return CreatedAtAction(nameof(Get), new { id = product.Id }, createdProductDTO);
            }
            return BadRequest();
        }


        // Xoa VAO THUNG RAC
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            //_context.Products.Remove(product);
            product.DeletedAt = DateTime.UtcNow; //Soft delete
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // Xoa HẲN KHỎI THUNG RAC
        [HttpDelete]
        [Route("permanently deleted")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }


            if (product.DeletedAt == null)
            {
                return BadRequest("Cannot delete");
            }
            else
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }



            return NoContent();
        }


        // UPDATE 
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> PutProduct(int id, ProductDTO productDTO)
        {
            if (id != productDTO.Id)
            {
                return BadRequest("The id in the URL does not match the id in the request body.");
            }

            //Check if the product with the given id exists in the database
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            //Map the properties from the ProductDTO to the existing Product entity
            _mapper.Map(productDTO, product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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


        // DUA Product RA KHỎI THUNG RAC
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Check if the product is already restored (DeletedAt is null)
            if (product.DeletedAt == null)
            {
                return BadRequest("The product is already restored.");
            }

            // Restore the product by setting DeletedAt to null
            product.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
