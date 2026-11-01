namespace Application.Models;

// representa la informacion que el cliente envia para crear un producto
public record PostProductRequest(
    string Name,
    string Description,
    double Price,
    int Stock,
    int CategoryId,
    string Image
);