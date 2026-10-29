using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    // Inyección de dependencias del servicio de productos
    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    // GET es público (cualquiera puede ver los productos sin token)
    // Obtiene todos los productos (HTTP 200)
    [HttpGet]
    public ActionResult<List<ProductDto>> Get()
    {
        var result = _productService.GetAll();
        return Ok(result);
    }

    // Este GET por ID también es público
    // Busca un producto por su id. Si no existe, el middleware responde 404
    [HttpGet("{id}")]
    public ActionResult<ProductDto> GetById(int id)
    {
        var result = _productService.GetById(id);
        return Ok(result);
    }

    [Authorize] // Este POST requiere que el usuario esté logueado (crear producto)
    [HttpPost] // Registra un nuevo producto (HTTP 201 Created)
    public ActionResult<ProductDto> Post([FromBody] PostProductRequest request)
    {
        var newProduct = _productService.Create(request);
        return Ok(newProduct);
    }

    
    [Authorize] // DELETE requiere autenticación
    [HttpDelete("{id}")] // Elimina un producto según su id (HTTP 204 No Content). Si no existe, el middleware responde 404
    public IActionResult Delete(int id)
    {
        _productService.Delete(id);
        return NoContent();
    }
}