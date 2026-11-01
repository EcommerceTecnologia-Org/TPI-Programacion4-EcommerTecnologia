using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [AllowAnonymous]
    public ActionResult<List<ProductDto>> Get()
    {
        var result = _productService.GetAll();
        return Ok(result);
    }

    // Este GET por ID también es público
    // Busca un producto por su id. Si no existe, el middleware responde 404
    [HttpGet("{id}")]
    [AllowAnonymous]
    public ActionResult<ProductDto> GetById(int id)
    {
        var result = _productService.GetById(id);
        return Ok(result);
    }

    // Registra un nuevo producto (HTTP 201 Created)
    [HttpPost]
    [Authorize(Roles = "Admin,Superadmin")]
    public ActionResult<ProductDto> Post([FromBody] PostProductRequest request)
    {
        var newProduct = _productService.Create(request);
        return Ok(newProduct);
    }

    // Elimina un producto según su id (HTTP 204 No Content). Si no existe, el middleware responde 404
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Superadmin")]
    public IActionResult Delete(int id)
    {
        _productService.Delete(id);
        return NoContent();
    }

    // Actualiza un producto existente por su ID
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Superadmin")]
    public IActionResult Update(int id, [FromBody] PostProductRequest request)
    {
        var updatedProduct = _productService.Update(id, request);
        return Ok(updatedProduct);
    }
}