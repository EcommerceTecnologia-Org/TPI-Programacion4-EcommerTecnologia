namespace Domain.Entities;

public class Admin : User
{
    // 1. Constructor vacío requerido por EF Core
    public Admin() : base() { }

    // 2. Constructor para crear uno nuevo desde cero
    public Admin(string email, string password) : base(email, password) { }

    // 3. Constructor para promover a un usuario existente (para el UserService)
    public Admin(int id, string email, string password, bool active) : base(email, password)
    {
        Id = id;
        Active = active;
    }
}