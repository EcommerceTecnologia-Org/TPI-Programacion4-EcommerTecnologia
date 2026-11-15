namespace Domain.Entities;

public class Superadmin : User
{
    // 1. Constructor vacío requerido por EF Core
    public Superadmin() : base() { }

    // 2. Constructor para crear uno nuevo desde cero (para el DbSeeder)
    public Superadmin(string email, string password) : base(email, password) { }

    // 3. Constructor para promover a un usuario existente (para el UserService)
    public Superadmin(int id, string email, string password, bool active) : base(email, password)
    {
        Id = id;
        Active = active;
    }
}