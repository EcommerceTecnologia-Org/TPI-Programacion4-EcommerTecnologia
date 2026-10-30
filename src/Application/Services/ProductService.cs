using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository; // Repositorio para consultar categorías en la base de datos

    // Inyección de dependencias de ambos repositorios
    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    // Obtiene la lista completa de productos traducida a DTOs
    public List<ProductDto> GetAll()
    {
        var products = _productRepository.List();
        return ProductDto.Create(products);
    }

    // Busca un producto por ID. Si no existe, lanza KeyNotFoundException (el Middleware lo convierte en HTTP 404)
    public ProductDto GetById(int id)
    {
        var product = _productRepository.GetById(id);

        if (product == null)
        {
            throw new KeyNotFoundException($"No se encontró el producto con ID {id}.");
        }

        return ProductDto.Create(product);
    }

    // Crea un producto aplicando las validaciones de negocio necesarias
    public ProductDto Create(PostProductRequest request)
    {
        // Validation: El nombre no puede estar vacío
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("El nombre del producto no puede estar vacío.");
        }

        // Validation: El precio debe ser un valor positivo
        if (request.Price <= 0)
        {
            throw new ArgumentException("El precio del producto debe ser mayor a cero.");
        }

        // Validation: Se verifica que la categoría exista en la base de datos antes de guardar
        var category = _categoryRepository.GetById(request.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"La categoria con ID {request.CategoryId} no existe.");
        }

        // Mapeo del objeto DTO/Request a la entidad de dominio
        var newProduct = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = (double)request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            Image = request.Image,
            Active = true
        };

        _productRepository.Add(newProduct);

        return ProductDto.Create(newProduct);
    }

    // Elimina un producto por ID. Si no existe, lanza KeyNotFoundException (el Middleware responde HTTP 404)
    public void Delete(int id)
    {
        var product = _productRepository.GetById(id);

        if (product == null)
        {
            throw new KeyNotFoundException($"No se encontró el producto con ID {id} para eliminar.");
        }

        _productRepository.Delete(product);
    }

    // Modifica un producto por ID
    public ProductDto Update(int id, PostProductRequest request)
    {
        // Validar que el producto exista en la base de datos
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            throw new KeyNotFoundException("El producto no fue encontrado.");
        }

        // Validación: El nombre no puede estar vacío
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("El nombre del producto no puede estar vacio.");
        }

        // Validación: El precio debe ser un valor positivo
        if (request.Price <= 0)
        {
            throw new ArgumentException("El precio del producto debe ser mayor a cero.");
        }

        // Validación: Se verifica que la categoría exista en la base de datos antes de guardar
        var category = _categoryRepository.GetById(request.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"La categoria con ID {request.CategoryId} no existe.");
        }

        // Actualizamos las propiedades con los datos nuevos del request
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;
        product.Image = request.Image;

        _productRepository.Update(product);

        return ProductDto.Create(product);
    }
}