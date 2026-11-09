namespace Domain.Entities;

public class OrderDetail
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public Order? Order { get; private set; }
    public int ProductId { get; private set; }
    public Product? Product { get; private set; }
    public int Quantity { get; private set; }
    public double UnitPrice { get; private set; }

    // Constructor sin parámetros requerido por Entity Framework Core
    public OrderDetail() { }

    // Constructor de dominio con validaciones estrictas
    public OrderDetail(int productId, int quantity, double unitPrice)
    {
        ValidateData(quantity, unitPrice);

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    // Método auxiliar privado para centralizar las reglas de validación
    private static void ValidateData(int quantity, double unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        if (unitPrice < 0)
            throw new ArgumentException("El precio unitario no puede ser negativo.");
    }
}