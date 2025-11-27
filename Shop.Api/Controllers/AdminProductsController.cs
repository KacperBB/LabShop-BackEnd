using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Data;
using Shop.Api.Models;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles = "Admin,Moderator")]
    public class AdminProductsController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public AdminProductsController(ShopDbContext context)
        {
            _context = context;
        }

        // GET: /api/admin/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return Ok(products);
        }

        public class UpdateCategoryRequest
        {
            public int CategoryId { get; set; }
        }

        // PUT: /api/admin/products/{id}/category
        [HttpPut("{id:int}/category")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId);

            if (!categoryExists)
            {
                return BadRequest($"Category with id {request.CategoryId} does not exist.");
            }

            product.CategoryId = request.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class UpdateTagsRequest
        {
            public List<int> TagIds { get; set; } = new();
        }

        // PUT: /api/admin/products/{id}/tags
        [HttpPut("{id:int}/tags")]
        public async Task<IActionResult> UpdateTags(int id, UpdateTagsRequest request)
        {
            var product = await _context.Products
                .Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            var existingTagIds = await _context.Tags
                .Where(t => request.TagIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync();

            var currentTagIds = product.ProductTags.Select(pt => pt.TagId).ToList();

            var toRemove = product.ProductTags
                .Where(pt => !existingTagIds.Contains(pt.TagId))
                .ToList();

            foreach (var pt in toRemove)
            {
                product.ProductTags.Remove(pt);
            }

            var toAdd = existingTagIds
                .Where(tagId => !currentTagIds.Contains(tagId))
                .ToList();

            foreach (var tagId in toAdd)
            {
                product.ProductTags.Add(new ProductTag
                {
                    ProductId = product.Id,
                    TagId = tagId
                });
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
