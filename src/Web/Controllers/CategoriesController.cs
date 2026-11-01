using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Web.Controllers;

[Authorize] // Protege todos los endpoints de este controlador
[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoriesController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // Obtiene el listado completo de categorías
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAll()
    {
        var categories = _categoryService.GetAll();
        return Ok(categories);
    }

    // Busca una categoría por su ID. Si no existe, el middleware captura la excepción KeyNotFoundException y devuelve 404
    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetById(int id)
    {
        var category = _categoryService.GetById(id);
        return Ok(category);
    }

    // Crea una nueva categoría. Si los datos fallan la validación, el middleware captura ArgumentException y devuelve 400
    [HttpPost]
    [Authorize(Roles = "Admin,Superadmin")]
    public IActionResult Create([FromBody] PostCategoryRequest request)
    {
        var newCategory = _categoryService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = newCategory.Id }, newCategory);
    }

    // Elimina una categoría por su ID. Si no existe, el middleware la ataja y responde 404
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Superadmin")]
    public IActionResult Delete(int id)
    {
        _categoryService.Delete(id);
        return NoContent();
    }

    // Actualiza una categoría existente por su ID
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Superadmin")]
    public IActionResult Update(int id, [FromBody] PostCategoryRequest request)
    {
        var updatedCategory = _categoryService.Update(id, request);
        return Ok(updatedCategory);
    }
}

