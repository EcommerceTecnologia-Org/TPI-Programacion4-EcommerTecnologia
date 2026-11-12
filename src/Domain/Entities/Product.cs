namespace Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Image { get; private set; } = string.Empty;
    public double Price { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public bool Active { get; private set; } = true;
    public int Stock { get; private set; }

    // Clave foránea y navegación
    public int CategoryId { get; private set; }
    public Category? Category { get; private set; }

    // Constructor sin parámetros requerido por Entity Framework Core
    public Product() { }

    // Constructor de dominio con validaciones estrictas
    public Product(string name, string description, double price, int stock, string image, int categoryId)
    {
        ValidateData(name, price, stock);

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price;
        Stock = stock;
        Image = image?.Trim() ?? string.Empty;
        CategoryId = categoryId;
        Active = true;
    }

    // Método de negocio para actualizar el producto de forma controlada
    public void Update(string name, string description, double price, int stock, string image, int categoryId)
    {
        ValidateData(name, price, stock);

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price;
        Stock = stock;
        Image = image?.Trim() ?? string.Empty;
        CategoryId = categoryId;
    }

    // Método auxiliar privado para centralizar las reglas de validación
    private static void ValidateData(string name, double price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto no puede estar vacío.");

        if (price <= 0)
            throw new ArgumentException("El precio del producto debe ser mayor a cero.");

        if (stock < 0)
            throw new ArgumentException("El stock del producto no puede ser negativo.");
    }

    // Método de negocio para desactivar el producto (eliminación lógica)
    public void Desactivate()
    {
        Active = false;
    }

    // Método de negocio para activar el producto nuevamente 
    public void Activate()
    {
        Active = true;
    }

    // Método de negocio para descontar stock de forma segura utilizado en el services de Order
    public void SubtractStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("La cantidad a descontar debe ser mayor a cero.");
        }

        if (Stock < quantity)
        {
            throw new InvalidOperationException($"Stock insuficiente para el producto '{Name}'. Stock actual: {Stock}, solicitado: {quantity}.");
        }

        Stock -= quantity;
    }
}