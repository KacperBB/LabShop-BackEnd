using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Data;
using Shop.Api.Models;
using System.Security.Claims;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/products/{productId:int}/reviews")]
    public class ProductReviewsController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public ProductReviewsController(ShopDbContext context)
        {
            _context = context;
        }

        public class ReviewDto
        {
            public int Id { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public string UserName { get; set; } = string.Empty;
        }

        public class CreateOrUpdateReviewRequest
        {
            public int Rating { get; set; }
            public string Comment { get; set; } = string.Empty;
        }

        // GET: /api/products/{productId}/reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews(int productId)
        {
            var reviews = await _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var result = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                UserName = r.User.UserName ?? r.User.Email ?? "Użytkownik"
            });

            return Ok(result);
        }

        // POST: /api/products/{productId}/reviews
        // Upsert: jeśli user już dodał opinię -> edytujemy
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> CreateOrUpdate(int productId, CreateOrUpdateReviewRequest request)
        {
            if (request.Rating < 1 || request.Rating > 5)
                return BadRequest("Rating musi być w zakresie 1–5.");

            var product = await _context.Products.FindAsync(productId);
            if (product is null) return NotFound("Product not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var now = DateTime.UtcNow;

            var existing = await _context.ProductReviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

            if (existing is null)
            {
                var review = new ProductReview
                {
                    ProductId = productId,
                    UserId = userId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    CreatedAt = now
                };

                _context.ProductReviews.Add(review);
                await _context.SaveChangesAsync();

                // dociągamy User
                await _context.Entry(review).Reference(r => r.User).LoadAsync();

                return Ok(new ReviewDto
                {
                    Id = review.Id,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    UserName = review.User.UserName ?? review.User.Email ?? "Użytkownik"
                });
            }
            else
            {
                existing.Rating = request.Rating;
                existing.Comment = request.Comment;
                existing.CreatedAt = now;

                await _context.SaveChangesAsync();

                return Ok(new ReviewDto
                {
                    Id = existing.Id,
                    Rating = existing.Rating,
                    Comment = existing.Comment,
                    CreatedAt = existing.CreatedAt,
                    UserName = existing.User.UserName ?? existing.User.Email ?? "Użytkownik"
                });
            }
        }

        // DELETE: /api/products/{productId}/reviews/{id}
        // user może usunąć swoją, admin/mod może usunąć każdą
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int productId, int id)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if (review is null || review.ProductId != productId)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Moderator");

            if (!isAdmin && review.UserId != userId)
                return Forbid();

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
