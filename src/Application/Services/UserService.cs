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
    // Modifica el rol de un usuario existente (ej. de Client a Admin o Superadmin)
    public async Task ChangeRoleAsync(int userId, ChangeUserRoleRequest request, int currentUserId)
    {
        var newRole = request.NewRole?.Trim();
        // Validamos que el rol no esté vacío
        if (string.IsNullOrWhiteSpace(newRole))
            throw new ArgumentException("El rol es requerido.");
        // Validamos que el rol esté dentro de los permitidos
        if (newRole != "Admin" && newRole != "Superadmin")
            throw new ArgumentException("Rol invalido. Solo se permite Admin o Superadmin.");
        // Evitamos que un administrador se cambie el rol a sí mismo
        if (userId == currentUserId)
            throw new ArgumentException("No puedes cambiarte el rol a ti mismo.");
        // Buscamos el usuario por su ID de forma asincrónica
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");
        // Verificamos que el usuario actual sea un cliente para poder ascenderlo
        if (user is not Client)
            throw new ArgumentException("Solo se puede cambiar el rol de usuarios con rol Client.");
        // Creamos la nueva instancia del usuario según el rol seleccionado usando polimorfismo
        User newUser = newRole switch
        {
            "Admin" => new Admin(user.Id, user.Email, user.Password, user.Active),
            "Superadmin" => new Superadmin(user.Id, user.Email, user.Password, user.Active),
            _ => throw new ArgumentException("Rol invalido.")
        };
        // Reemplazamos la entidad vieja por la nueva en el repositorio
        await _userRepository.ReplaceAsync(user, newUser);
    }

    // Activa un usuario existente que se encontraba inactivo
    public async Task ActivateAsync(int userId, int currentUserId)
    {
        // Evitamos que un administrador modifique su propio estado 
        if (userId == currentUserId)
            throw new ArgumentException("No puedes cambiar tu propio estado de activación.");

        // Buscamos al usuario en la base de datos
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        // Validamos si el usuario ya está activo, avisamos
        if (user.Active)
            throw new ArgumentException("El usuario ya se encuentra activo.");

        // Ejecutamos el método de negocio de la entidad para cambiar el estado en memoria
        user.Activate();

        // Actualizamos el registro en el repositorio (preparando el cambio)
        _userRepository.Update(user);
        // Como el update ya no guarda automáticamente, persistimos los cambios de forma explícita
        _userRepository.SaveChanges(); // (o _userRepository.SaveChanges() según cómo tengas definido el método)
    }

    // Desactiva un usuario existente bloqueando su acceso
    public async Task DesactivateAsync(int userId, int currentUserId)
    {
        // Evitamos que un administrador se auto-desactive por accidente
        if (userId == currentUserId)
            throw new ArgumentException("No puedes cambiar tu propio estado de activación.");

        // Buscamos al usuario en la base de datos
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        // Validamos si el usuario ya está inactivo, avisamos
        if (!user.Active)
            throw new ArgumentException("El usuario ya se encuentra inactivo.");

        // Ejecutamos el método de negocio de la entidad para desactivarlo en memoria
        user.Desactivate();

        // Actualizamos el registro en el repositorio
        _userRepository.Update(user);

        // Guardamos los cambios explícitamente en la base de datos
        _userRepository.SaveChanges(); // (ajustá el nombre del método de guardado según tu interfaz)
    }

    // Obtiene una lista de todos los usuarios registrados usando el método síncrono List()
    public IEnumerable<UserResponseDto> GetAll()
    {
        var users = _userRepository.List(); 

        return users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Email = u.Email,
            Active = u.Active,
            // Mapeamos el rol usando pattern matching según el tipo de entidad
            Role = u switch
            {
                Superadmin => "Superadmin",
                Admin => "Admin",
                Client => "Client",
                _ => "Unknown"
            }
        });
    }

    // Obtiene un usuario específico por su ID mapeándolo a un UserResponseDto para exponer solo la información necesaria
    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

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
}