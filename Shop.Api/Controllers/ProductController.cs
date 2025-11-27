using Microsoft.AspNetCore.Mvc;
using Shop.Api.Data;
using Shop.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public ProductsController(ShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
                return NotFound();

            return Ok(product);
        }


        // POST /api/products
        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest("Product name is required.");
            }

            if (product.Price <= 0)
            {
                return BadRequest("Product price must be greater than zero.");
            }

            // KLUCZOWA LINIJKA:
            product.Id = 0; // albo default(int), żeby EF potraktował to jako "bez Id, wygeneruj"

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }


        // PUT /api/products/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Product updatedProduct)
        {
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct is null)
            {
                return NotFound();
            }

            // Prosta walidacja
            if (string.IsNullOrWhiteSpace(updatedProduct.Name))
            {
                return BadRequest("Product name is required.");
            }

            if (updatedProduct.Price <= 0)
            {
                return BadRequest("Product price must be greater than zero.");
            }

            // Aktualizacja pól
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.StockQuantity = updatedProduct.StockQuantity;

            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // DELETE /api/products/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct is null)
            {
                return NotFound();
            }

            _context.Products.Remove(existingProduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET /api/products/search?categoryId=1&tagId=2&minPrice=10&maxPrice=100&query=klawiatura
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> Search(
    [FromQuery] int? categoryId,
    [FromQuery] int? tagId,
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice,
    [FromQuery] string? query)
        {
            var productsQuery = _context.Products
                .Include(p => p.Promotion)
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery
                    .Where(p => p.CategoryId == categoryId.Value);
            }

            if (tagId.HasValue)
            {
                productsQuery = productsQuery
                    .Where(p => p.ProductTags.Any(pt => pt.TagId == tagId.Value));
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(query) || p.Description.Contains(query));
            }

            var result = await productsQuery.ToListAsync();
            return Ok(result);
        }


    }
}
