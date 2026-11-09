namespace Domain.Entities;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    // Propiedad de navegación para EF Core
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    // Constructor sin parámetros requerido por Entity Framework Core
    public Category() { }

    // Constructor de dominio con validación estricta
    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.");

        Name = name.Trim();
    }

    // Método de negocio para modificar el nombre de forma controlada
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.");

        Name = newName.Trim();
    }
}