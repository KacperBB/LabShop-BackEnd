using Microsoft.AspNetCore.Mvc;
using Shop.Api.Data;
using Shop.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public EventsController(ShopDbContext context)
        {
            _context = context;
        }

        public class CreateEventRequest
        {
            public string Type { get; set; } = string.Empty;
            public int? ProductId { get; set; }
            public string? SessionId { get; set; }
            public int? DurationSeconds { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEventRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Type))
            {
                return BadRequest("Type is required.");
            }

            var ev = new UserEvent
            {
                Type = request.Type,
                ProductId = request.ProductId,
                SessionId = request.SessionId,
                DurationSeconds = request.DurationSeconds,
                OccurredAt = DateTime.UtcNow
            };

            _context.UserEvents.Add(ev);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
