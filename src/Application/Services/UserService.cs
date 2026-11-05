using Application.Models;
using Domain.Entities;
using Domain.Interfaces;


namespace Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task ChangeRoleAsync(int userId, ChangeUserRoleRequest request, int currentUserId)
    {
        var newRole = request.NewRole?.Trim();

        if (string.IsNullOrWhiteSpace(newRole))
            throw new ArgumentException("El rol es requerido.");

        if (newRole != "Admin" && newRole != "Superadmin")
            throw new ArgumentException("Rol invalido. Solo se permite Admin o Superadmin.");

        if (userId == currentUserId)
            throw new ArgumentException("No puedes cambiarte el rol a ti mismo.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        if (user is not Client)
            throw new ArgumentException("Solo se puede cambiar el rol de usuarios con rol Client.");

        User newUser = newRole switch
        {
            "Admin" => new Admin { Id = user.Id, Email = user.Email, Password = user.Password, Active = user.Active },
            "Superadmin" => new Superadmin { Id = user.Id, Email = user.Email, Password = user.Password, Active = user.Active },
            _ => throw new ArgumentException("Rol invalido.")
        };

        await _userRepository.ReplaceAsync(user, newUser);
    }
}