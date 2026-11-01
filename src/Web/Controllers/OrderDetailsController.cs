using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Tags("4. Orders")] // forzar un orden específico en la interfaz de usuario de Swagger
    public class OrderDetailsController : ControllerBase
    {
        private readonly OrderDetailService _orderDetailService;

        public OrderDetailsController(OrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        // Obtiene todos los detalles de órdenes (requiere estar autenticado)
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var details = _orderDetailService.GetAll();
            return Ok(details);
        }

        // Busca un detalle por ID
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            var detail = _orderDetailService.GetById(id);
            return Ok(detail);
        }

        // Crea un nuevo detalle de orden
        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] PostOrderDetailRequest request)
        {
            var newDetail = _orderDetailService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = newDetail.Id }, newDetail);
        }

        // Elimina un detalle de orden por ID (solo Admin y Superadmin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Superadmin")]
        public IActionResult Delete(int id)
        {
            _orderDetailService.Delete(id);
            return NoContent();
        }
    }
}