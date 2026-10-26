using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    // Busca y devuelve un usuario en la base de datos a partir de su dirección de correo electrónico
    Task<User?> GetByEmailAsync(string email); // Retorna 'null' si no existe ningún usuario con ese email
    // Verifica si existe un usuario con el email indicado y si la contraseña ingresada coincide
    // Task<bool> ValidateCredentialsAsync(string email, string password); 
    // No conviene que toda la validacion este en AuthService ?
}