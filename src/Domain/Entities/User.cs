namespace Domain.Entities;

public abstract class User
{
    public int Id { get; protected set; }
    public string Email { get; protected set; } = string.Empty;
    public string Password { get; protected set; } = string.Empty;
    public bool Active { get; protected set; } = true;

    // Constructor protegido requerido por Entity Framework Core y clases hijas
    protected User() { }

    // Constructor que asegura que no se pueda instanciar un usuario con datos vacíos
    protected User(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es requerido.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña es requerida.");

        Email = email;
        Password = password;
        Active = true;
    }

    // Métodos para modificar estado de forma controlada 
    public void Desactivate() => Active = false;
    public void Activate() => Active = true;
}