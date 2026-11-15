using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    // Reemplaza una instancia de usuario por otra (manteniendo el mismo Id)
    // para actualizar el Discriminator en la herencia TPH. Devuelve la nueva entidad guardada.
    User Replace(User oldUser, User newUser);
    
    // Busca y devuelve un usuario en la base de datos a partir de su correo electrónico
    User? GetByEmail(string email);
}