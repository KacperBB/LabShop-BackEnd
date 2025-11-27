using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Data;
using Shop.Api.Models;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public TagsController(ShopDbContext context)
        {
            _context = context;
        }

        // GET /api/tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAll()
        {
            var tags = await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();

            return Ok(tags);
        }

        // POST /api/tags  (np. z panelu admina)
        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<Tag>> Create(Tag tag)
        {
            if (string.IsNullOrWhiteSpace(tag.Name))
                return BadRequest("Name is required.");

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = tag.Id }, tag);
        }
    }
}
