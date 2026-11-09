namespace Domain.Entities;

public class Client : User
{
    public Client() : base() { }

    public Client(string email, string password) : base(email, password)
    {
    }
}