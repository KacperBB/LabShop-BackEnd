using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Data;
using Shop.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Shop.Api.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.UserName,
                user.FirstName,
                user.LastName,
                roles
            });
        }

        public class UpdateRolesRequest
        {
            public List<string> Roles { get; set; } = new();
        }

        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> UpdateRoles(string userId, UpdateRolesRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var validRoles = new[] { "Admin", "Moderator", "User" };

            // Walidacja – nie pozwalamy ustawić nieistniejących ról
            foreach (var role in request.Roles)
            {
                if (!validRoles.Contains(role))
                {
                    return BadRequest($"Invalid role: {role}");
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Usuwamy stare role
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded) return BadRequest(removeResult.Errors);

            // Dodajemy nowe
            var addResult = await _userManager.AddToRolesAsync(user, request.Roles);
            if (!addResult.Succeeded) return BadRequest(addResult.Errors);

            return NoContent();
        }
    }
}
