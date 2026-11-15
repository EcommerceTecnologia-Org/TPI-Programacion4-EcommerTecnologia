using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // Obtener todas las categorías
    public IEnumerable<CategoryDto> GetAll()
    {
        var categories = _categoryRepository.List();

        if (categories == null || !categories.Any())
        {
            throw new KeyNotFoundException("No se encontraron categorias registradas");
        }

        return CategoryDto.Create(categories);
    }

    // Obtener una categoría por ID o lanzar excepción si no existe
    public CategoryDto GetById(int id)
    {
        var category = _categoryRepository.GetById(id);

        if (category == null)
        {
            throw new KeyNotFoundException($"No se encontro la categoria con ID {id}.");
        }

        return CategoryDto.Create(category);
    }

    // Crear una nueva categoría usando el constructor de la entidad
    public CategoryDto Create(PostCategoryRequest request)
    {
        var normalizedName = request.Name?.Trim() ?? string.Empty;

        // Validamos si ya existe otra categoría con el mismo nombre en la BD
        var existingCategory = _categoryRepository.GetByName(normalizedName);
        if (existingCategory != null)
        {
            throw new ArgumentException($"Ya existe una categoría registrada con el nombre '{normalizedName}'.");
        }
        
        // La validación ocurre de forma segura dentro del constructor de Category
        var category = new Category(normalizedName);

        _categoryRepository.Add(category);

        return CategoryDto.Create(category);
    }

    // Eliminar una categoría
    public void Delete(int id)
    {
        var category = _categoryRepository.GetById(id);

        if (category == null)
        {
            throw new KeyNotFoundException($"No se puede eliminar. No existe la categoria con ID {id}.");
        }

        _categoryRepository.Delete(category);
    }

    // Modificar una categoría usando el método de negocio de la entidad
    public CategoryDto Update(int id, PostCategoryRequest request)
    {
        var category = _categoryRepository.GetById(id);
        
        if (category == null)
        {
            throw new KeyNotFoundException("La categoría no fue encontrada.");
        }

        var normalizedName = request.Name?.Trim() ?? string.Empty;

        // Validamos si el nuevo nombre ya pertenece a otra categoría distinta en la BD
        var existingCategory = _categoryRepository.GetByName(normalizedName);
        if (existingCategory != null && existingCategory.Id != id)
        {
            throw new ArgumentException($"Ya existe otra categoría registrada con el nombre '{normalizedName}'.");
        }

        // Delegamos la actualización y validación al dominio
        category.UpdateName(normalizedName);
        // Marcamos como actualizado en el Change Tracker
        _categoryRepository.Update(category);
        // Guardamos los cambios explícitamente en la base de datos
        _categoryRepository.SaveChanges();
    
        return CategoryDto.Create(category);
    }
}