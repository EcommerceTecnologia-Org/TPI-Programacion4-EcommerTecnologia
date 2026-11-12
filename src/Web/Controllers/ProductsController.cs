using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
[Tags("3. Products")] // forzar un orden específico en la interfaz de usuario de Swagger
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
     public async Task<ActionResult<List<ProductDto>>> Get()
    {
        var result = await _productService.GetAll();
        return Ok(result);
    }


    // Este GET por ID también es público
    // Busca un producto por su id. Si no existe, el middleware responde 404
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var result = await _productService.GetById(id);
        return Ok(result);
    }


    // Este GET por ID de categoría también es público
    // Busca productos por su ID de categoría. Si no existe, el middleware responde 404
    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProductDto>>> GetByCategoryId(int categoryId)
    {
        var products = await _productService.GetByCategoryId(categoryId);
        return Ok(products);
    }

    // Registra un nuevo producto (HTTP 201 Created)
    [HttpPost]
    [Authorize(Roles = "Admin,Superadmin")]
    public ActionResult<ProductDto> Post([FromBody] PostProductRequest request)
    {
        var newProduct = _productService.Create(request);
        return Ok(newProduct);
    }

    // Endpoint PUT para eliminación lógica de producto según su id (HTTP 204 No Content). Si no existe, el middleware responde 404
    [HttpPut("{id}/desactivate")]
    [Authorize(Roles = "Admin,Superadmin")]
    public IActionResult Desactivate(int id)
    {
        _productService.Desactivate(id);
        return NoContent(); // HTTP 204 No Content indicando éxito
    }

    // Endpoint PUT para activar un producto inactivo 
    [HttpPut("{id}/activate")]
    [Authorize(Roles = "Admin,Superadmin")] 
    public IActionResult Activate(int id)
    {
        _productService.Activate(id);
        return NoContent(); // HTTP 204 No Content indicando éxito
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