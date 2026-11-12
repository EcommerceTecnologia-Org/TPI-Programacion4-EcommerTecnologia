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
    public IActionResult ChangeRole(int id, [FromBody] ChangeUserRoleRequest request)
    {
        // Extraemos el ID del usuario autenticado desde los claims del token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Validamos que el claim exista y que el texto pueda convertirse de forma segura a un número entero (int)
        if (string.IsNullOrWhiteSpace(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
        {
            throw new UnauthorizedAccessException("No se pudo identificar al usuario.");
        }

        // Ejecutamos la lógica en el servicio pasándole el ID objetivo, la solicitud y el ID del admin actual
        _userService.ChangeRole(id, request, currentUserId);
        return NoContent();
    }

    // Endpoint PUT para activar a un usuario inactivo
    [HttpPut("{id}/activate")]
    public IActionResult Activate(int id)
    {
        // Obtenemos el claim del ID del usuario logueado actualmente
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Validamos que el claim exista y que el texto pueda convertirse de forma segura a un número entero (int)
        if (string.IsNullOrWhiteSpace(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
        {
            throw new UnauthorizedAccessException("No se pudo identificar al usuario.");
        }

        // Llamamos al servicio para activar al usuario validando que no sea el mismo administrador
        _userService.Activate(id, currentUserId);
        return NoContent();
    }

    // Endpoint PUT para desactivar un usuario existente
    [HttpPut("{id}/desactivate")]
    public IActionResult Desactivate(int id)
    {
        // Obtenemos el claim del ID del usuario logueado actualmente
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Validamos que el claim exista y que el texto pueda convertirse de forma segura a un número entero (int)
        if (string.IsNullOrWhiteSpace(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
        {
            throw new UnauthorizedAccessException("No se pudo identificar al usuario.");
        }

        // Llamamos al servicio para desactivar al usuario especificado
        _userService.Desactivate(id, currentUserId);
        return NoContent();
    }

    // Endpoint GET para listar todos los usuarios del sistema
    [HttpGet]
    public IActionResult GetAll()
    {
        // Obtenemos la lista mapeada directamente desde el servicio
        var users = _userService.GetAll();
        return Ok(users);
    }

    // Endpoint GET para buscar un usuario específico por su ID
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        // Llamamos al servicio para buscar y retornar el DTO del usuario
        var user = _userService.GetById(id);
    
        return Ok(user);
    }
}