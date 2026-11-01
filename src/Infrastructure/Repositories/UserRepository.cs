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
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    // Busca un usuario por su Id de forma asincronica
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
    }

    // Reemplaza una instancia de usuario por otra (mismo Id) para actualizar
    // el Discriminator de la herencia TPH en una unica transaccion
    public async Task<User> ReplaceAsync(User oldUser, User newUser)
    {
        _dbSet.Remove(oldUser);
        _dbSet.Add(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }
}