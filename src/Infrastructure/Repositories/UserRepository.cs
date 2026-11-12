using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationContext context) : base(context)
    {

    }
    // Busca un usuario por su email de forma síncrona
    public User? GetByEmail(string email)
    {
        return _dbSet.FirstOrDefault(u => u.Email == email);
    }

    // Reemplaza una instancia de usuario por otra (mismo Id) para actualizar
    // el Discriminator de la herencia TPH en una unica transaccion
    public User Replace(User oldUser, User newUser)
    {
        _dbSet.Remove(oldUser);
        _dbSet.Add(newUser);
        _context.SaveChanges();
        return newUser;
    }
}