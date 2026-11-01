using System.Security.Claims;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Superadmin")]
public class UsersController : ControllerBase
{
    // Inyeccion del servicio que contiene la logica del cambio de rol 
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeUserRoleRequest request)
    {
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
        {
            throw new UnauthorizedAccessException("No se pudo identificar al usuario.");
        }

        await _userService.ChangeRoleAsync(id, request, currentUserId);
        return NoContent();
    }
}