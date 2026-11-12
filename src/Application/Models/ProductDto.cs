using Domain.Entities;

namespace Application.Models;

// DTO para estructurar la respuesta enviada al cliente HTTP
public record ProductDto(
    int Id,
    string Name,
    string Description,
    double Price,
    int Stock,
    int CategoryId,
    string Image,
    bool Active,
    double? PriceUsd = null // Precio convertido a dolares usando la cotizacion de la API externa
)
{
    // Transforma una entidad de dominio en el DTO de respuesta
    public static ProductDto Create(Product entity, double? dolarVenta = null)
    {
        return new ProductDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.Price,
            entity.Stock,
            entity.CategoryId,
            entity.Image,
            entity.Active,
            dolarVenta == null ? null : Math.Round(entity.Price / dolarVenta.Value, 2)
        );
    }

    // Sobrecarga para mapear una lista de entidades a una lista de DTOs
    public static List<ProductDto> Create(IEnumerable<Product> entities, double? dolarVenta = null)
    {
        var listDto = new List<ProductDto>();
        foreach (var entity in entities)
        {
            listDto.Add(Create(entity, dolarVenta));
        }

        return listDto;
    }
}