using Domain.Entities;

namespace Application.Models;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool Active { get; set; }
    public string Role { get; set; } = string.Empty; 

    // Método estático para mapear un único usuario
    public static UserResponseDto Create(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Active = user.Active,
            Role = user switch
            {
                Superadmin => "Superadmin",
                Admin => "Admin",
                Client => "Client",
                _ => "Unknown"
            }
        };
    }

    // Método estático para mapear una lista completa de usuarios
    public static List<UserResponseDto> Create(IEnumerable<User> users)
    {
        return users.Select(Create).ToList();
    }
}