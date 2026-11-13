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
    // Modifica el rol de un usuario existente (permite ascender o degradar entre Client, Admin y Superadmin)
    public void ChangeRole(int userId, ChangeUserRoleRequest request, int currentUserId)
    {
        var newRole = request.NewRole?.Trim();

        // Validamos que el rol no esté vacío ni nulo
        if (string.IsNullOrWhiteSpace(newRole))
            throw new ArgumentException("El rol es requerido.");

        // Validamos que el rol destino esté dentro de los permitidos por el sistema
        if (newRole != "Client" && newRole != "Admin" && newRole != "Superadmin")
            throw new ArgumentException("Rol invalido. Solo se permite Client, Admin o Superadmin.");

        // Seguridad: Evitamos que un administrador se auto-modifique el rol
        if (userId == currentUserId)
            throw new ArgumentException("No puedes cambiarte el rol a ti mismo.");

        // Buscamos al usuario
        var user = _userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        // Creamos la nueva instancia del usuario según el rol seleccionado utilizando polimorfismo 
        User newUser = newRole switch
        {
            "Client" => new Client(user.Id, user.Email, user.Password, user.Active),
            "Admin" => new Admin(user.Id, user.Email, user.Password, user.Active),
            "Superadmin" => new Superadmin(user.Id, user.Email, user.Password, user.Active),
            _ => throw new ArgumentException("Rol invalido.")
        };

        // Reemplazamos la entidad vieja por la nueva en el repositorio para actualizar el Discriminator de TPH
        _userRepository.Replace(user, newUser);
    }
    // Activa un usuario existente que se encontraba inactivo
    public void Activate(int userId, int currentUserId)
    {
        // Evitamos que un administrador modifique su propio estado de activación por accidente
        if (userId == currentUserId)
            throw new ArgumentException("No puedes cambiar tu propio estado de activación.");

        var user = _userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        // Validamos si el usuario ya se encuentra activo para evitar operaciones redundantes
        if (user.Active)
            throw new ArgumentException("El usuario ya se encuentra activo.");

        // Ejecutamos el método de dominio para cambiar el estado en memoria
        user.Activate();

        // Actualizamos y persistimos los cambios de forma explícita en la base de datos
        _userRepository.Update(user);
        _userRepository.SaveChanges();
    }

    // Desactiva un usuario existente bloqueando su acceso
    public void Desactivate(int userId, int currentUserId)
    {
        // Evitamos que un administrador se auto-desactive
        if (userId == currentUserId)
            throw new ArgumentException("No puedes cambiar tu propio estado de activación.");

        var user = _userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        // Validamos si el usuario ya está inactivo
        if (!user.Active)
            throw new ArgumentException("El usuario ya se encuentra inactivo.");

        // Ejecutamos el método de dominio para desactivarlo en memoria
        user.Desactivate();

        // Actualizamos y persistimos los cambios en la base de datos
        _userRepository.Update(user);
        _userRepository.SaveChanges();
    }

    // Obtiene una lista completa de todos los usuarios registrados, mapeándolos al DTO de respuesta
    public List<UserResponseDto> GetAll()
    {
        var users = _userRepository.List();

        if (users == null || !users.Any())
        {
            throw new KeyNotFoundException("No se encontraron usuarios registrados.");
        }

        // Delegamos el mapeo de la colección directamente al DTO
        return UserResponseDto.Create(users);
    }

    // Obtiene un usuario específico por su ID mapeándolo a un UserResponseDto para exponer solo la información necesaria
    public UserResponseDto GetById(int id)
    {
        var user = _userRepository.GetById(id);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        // Delegamos el mapeo del usuario individual al DTO
        return UserResponseDto.Create(user);
    }
}