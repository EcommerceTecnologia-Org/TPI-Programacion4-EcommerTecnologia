using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly DolarService _dolarService;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, DolarService dolarService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _dolarService = dolarService;
    }

    // Obtiene la cotizacion de la API externa una sola vez por operacion
    private async Task<double?> ObtenerCotizacion()
    {
        return await _dolarService.GetVentaOficial();
    }

    // Obtiene la lista completa de productos
    public async Task<List<ProductDto>> GetAll()
    {
        var products = _productRepository.List();

        if (products == null || !products.Any())
        {
            throw new KeyNotFoundException("No se encontraron productos registrados.");
        }

        var dolarVenta = await ObtenerCotizacion();
        return ProductDto.Create(products, dolarVenta);
    }

    // Busca un producto por ID
    public async Task<ProductDto> GetById(int id)
    {
        var product = _productRepository.GetById(id);

        if (product == null)
        {
            throw new KeyNotFoundException($"No se encontró el producto con ID {id}.");
        }

        var dolarVenta = await ObtenerCotizacion();
        return ProductDto.Create(product, dolarVenta);
    }

    public async Task<List<ProductDto>> GetByCategoryId(int categoryId)
    {
        var category = _categoryRepository.GetById(categoryId);
        if (category == null)
        {
            throw new KeyNotFoundException($"No se encontró la categoría con ID {categoryId}.");
        }

        var products = _productRepository.GetByCategoryId(categoryId);

        if (products == null || !products.Any())
        {
            throw new KeyNotFoundException($"No se encontraron productos registrados para la categoría con ID {categoryId}.");
        }

        var dolarVenta = await ObtenerCotizacion();
        return ProductDto.Create(products, dolarVenta);
    }

    // Crea un producto utilizando el constructor de dominio
    public ProductDto Create(PostProductRequest request)
    {
        var category = _categoryRepository.GetById(request.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"La categoria con ID {request.CategoryId} no existe.");
        }

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

        var category = _categoryRepository.GetById(request.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"La categoria con ID {request.CategoryId} no existe.");
        }

        product.Update(
            request.Name,
            request.Description,
            (double)request.Price,
            request.Stock,
            request.Image,
            request.CategoryId
        );

        _productRepository.Update(product);
        _productRepository.SaveChanges();

        return ProductDto.Create(product);
    }
}