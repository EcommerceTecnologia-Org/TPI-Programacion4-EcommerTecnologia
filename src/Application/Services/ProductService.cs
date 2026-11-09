using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    // Obtiene la lista completa de productos
    public List<ProductDto> GetAll()
    {
        var products = _productRepository.List();

        if (products == null || !products.Any())
        {
            throw new KeyNotFoundException("No se encontraron productos registrados.");
        }

        return ProductDto.Create(products);
    }

    // Busca un producto por ID
    public ProductDto GetById(int id)
    {
        var product = _productRepository.GetById(id);

        if (product == null)
        {
            throw new KeyNotFoundException($"No se encontró el producto con ID {id}.");
        }

        return ProductDto.Create(product);
    }

    // Crea un producto utilizando el constructor de dominio
    public ProductDto Create(PostProductRequest request)
    {
        // Validación de existencia de la categoría previa
        var category = _categoryRepository.GetById(request.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"La categoria con ID {request.CategoryId} no existe.");
        }

        // Instanciamos el producto usando el constructor seguro (las validaciones de nombre, precio y stock se ejecutan ahí dentro)
        var newProduct = new Product(
            request.Name,
            request.Description,
            (double)request.Price,
            request.Stock,
            request.Image,
            request.CategoryId
        );

        _productRepository.Add(newProduct);

        return ProductDto.Create(newProduct);
    }

    // Elimina un producto por ID
    public void Delete(int id)
    {
        var product = _productRepository.GetById(id);

        if (product == null)
        {
            throw new KeyNotFoundException($"No se encontró el producto con ID {id} para eliminar.");
        }

        _productRepository.Delete(product);
    }

    // Modifica un producto utilizando el método de negocio de la entidad
    public ProductDto Update(int id, PostProductRequest request)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            throw new KeyNotFoundException("El producto no fue encontrado.");
        }

        // Validación de existencia de la categoría
        var category = _categoryRepository.GetById(request.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"La categoria con ID {request.CategoryId} no existe.");
        }

        // Delegamos la actualización y validación al dominio
        product.Update(
            request.Name,
            request.Description,
            (double)request.Price,
            request.Stock,
            request.Image,
            request.CategoryId
        );

        _productRepository.Update(product);

        return ProductDto.Create(product);
    }
}