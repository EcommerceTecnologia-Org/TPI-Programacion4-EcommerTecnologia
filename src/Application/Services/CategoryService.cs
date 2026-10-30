using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class CategoryService
{
    private readonly IRepository<Category> _categoryRepository;

    public CategoryService(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // Obtener todas las categorías
    public IEnumerable<CategoryDto> GetAll()
    {
        var categories = _categoryRepository.List();
        return CategoryDto.Create(categories);
    }

    // Obtener una categoría por ID o lanzar excepción si no existe
    public CategoryDto GetById(int id)
    {
        var category = _categoryRepository.GetById(id);

        // Si no existe, lanzamos KeyNotFoundException
        if (category == null)
        {
            throw new KeyNotFoundException($"No se encontro la categoria con ID {id}.");
        }

        return CategoryDto.Create(category);
    }

    // Crear una nueva categoría con validaciones
    public CategoryDto Create(PostCategoryRequest request)
    {
        // Validación de regla de negocio
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("El nombre de la categoria no puede estar vacio.");
        }

        var category = new Category
        {
            Name = request.Name
        };

        _categoryRepository.Add(category);

        return CategoryDto.Create(category);
    }

    // Eliminar una categoría
    public void Delete(int id)
    {
        var category = _categoryRepository.GetById(id);

        // si no existe Id
        if (category == null)
        {
            throw new KeyNotFoundException($"No se puede eliminar. No existe la categoria con ID {id}.");
        }

        _categoryRepository.Delete(category);
    }

    // Modificar una categoría
    public CategoryDto Update(int id, PostCategoryRequest request)
    {
        var category = _categoryRepository.GetById(id);
        // si no existe Id
        if (category == null)
        {
            throw new KeyNotFoundException("La categoría no fue encontrada.");
        }
        // si el nombre que se quiere poner esta vacio
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.");
        }

        category.Name = request.Name;
        
        _categoryRepository.Update(category);

        return CategoryDto.Create(category);
    }
}