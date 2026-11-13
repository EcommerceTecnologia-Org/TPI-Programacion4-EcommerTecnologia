namespace Domain.Entities;

public class Client : User
{
    // 1. Constructor vacío requerido por EF Core
    public Client() : base() { }
    // 2. Constructor para crear uno nuevo desde cero
    public Client(string email, string password) : base(email, password)
    {
    }
    // 3. Constructor para degradar a un usuario existente a Client (para el UserService)
    public Client(int id, string email, string password, bool active) : base(email, password)
    {
        Id = id;
        Active = active;
    }
}