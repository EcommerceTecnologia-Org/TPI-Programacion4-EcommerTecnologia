using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Tags("4. Orders")] // forzar un orden específico en la interfaz de usuario de Swagger
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // Obtiene todas las órdenes 
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            // Extraemos únicamente el ID y el Role de los claims del token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            // Si es Admin o Superadmin, puede ver todas las órdenes. Si es cliente, solo las suyas.
            var orders = _orderService.GetOrdersForUser(userId, role);
            return Ok(orders);
        }

        // Busca una orden por ID
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            // Extraemos únicamente el ID y el Role de los claims del token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            // Si es Admin o Superadmin, puede ver todas las órdenes. Si es cliente, solo las suyas.
            var order = _orderService.GetById(id, userId, role);
            return Ok(order);
        }

        // Crea una nueva orden
        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] PostOrderRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"); // Conviene que el ID del cliente no venga desde el JSON que manda el usuario, sino que se extraiga directamente del token JWT
            var newOrder = _orderService.Create(userId, request);
            return CreatedAtAction(nameof(GetById), new { id = newOrder.Id }, newOrder); // Retorna un código HTTP 201 Created, incluye la URL del recurso recién creado (usando GetById) y devuelve el objeto order en el body
        }
        
        // Elimina una orden por ID (solo Admin y Superadmin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Superadmin")]
        public IActionResult Delete(int id)
        {
            _orderService.Delete(id);
            return NoContent();
        }
    }
}