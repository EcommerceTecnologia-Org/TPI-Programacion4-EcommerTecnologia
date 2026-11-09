using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Order
{
    public int Id { get; private set; }
    public int ClientId { get; private set; }
    public double Total { get; private set; }

    // Propiedad de navegación
    public ICollection<OrderDetail> OrderDetails { get; private set; } = new List<OrderDetail>();

    // Constructor sin parámetros requerido por Entity Framework Core
    public Order() { }

    // Constructor de dominio con validación estricta
    public Order(int clientId, double total)
    {
        if (total < 0)
            throw new ArgumentException("El total de la orden no puede ser negativo.");

        ClientId = clientId;
        Total = total;
    }
}