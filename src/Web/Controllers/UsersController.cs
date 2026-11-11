using System.Security.Claims;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Superadmin")] // Solo los usuarios con rol Superadmin pueden acceder a los endpoints de este controlador
public class UsersController : ControllerBase
{
    // Inyección de dependencia del servicio que contiene la lógica de negocio de usuarios
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    // Endpoint PUT para cambiar el rol de un usuario (ej. Client a Admin)
    [HttpPut("{id}/role")]
    public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeUserRoleRequest request)
    {
        // Extraemos el ID del usuario autenticado desde los claims del token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // Validamos que el claim exista y que el texto pueda convertirse de forma segura a un número entero (int)
        if (string.IsNullOrWhiteSpace(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
        {
            throw new UnauthorizedAccessException("No se pudo identificar al usuario.");
        }
        // Ejecutamos la lógica en el servicio pasándole el ID objetivo, la solicitud y el ID del admin actual
        await _userService.ChangeRoleAsync(id, request, currentUserId);
        return NoContent();
    }

    // Endpoint PUT para activar a un usuario inactivo
    [HttpPut("{id}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        // Obtenemos el claim del ID del usuario logueado actualmente
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // Validamos que el claim exista y que el texto pueda convertirse de forma segura a un número entero (int)
        if (string.IsNullOrWhiteSpace(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
        {
            throw new UnauthorizedAccessException("No se pudo identificar al usuario.");
        }
        // Llamamos al servicio para activar al usuario validando que no sea el mismo administrador
        await _userService.ActivateAsync(id, currentUserId);
        return NoContent();
    }

    [HttpPut("{id}/desactivate")]
    public async Task<IActionResult> Desactivate(int id)
    {
        // Obtenemos el claim del ID del usuario logueado actualmente
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // Validamos que el claim exista y que el texto pueda convertirse de forma segura a un número entero (int)
        if (string.IsNullOrWhiteSpace(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
        {
            throw new UnauthorizedAccessException("No se pudo identificar al usuario.");
        }
        // Llamamos al servicio para bloquear o desactivar al usuario especificado
        await _userService.DesactivateAsync(id, currentUserId);
        return NoContent();
    }

    // Endpoint GET para listar todos los usuarios del sistema
    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    // Endpoint GET para buscar un usuario específico por su ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // Llamamos al servicio para buscar el usuario
        var user = await _userService.GetByIdAsync(id);
    
        return Ok(user);
    }
}