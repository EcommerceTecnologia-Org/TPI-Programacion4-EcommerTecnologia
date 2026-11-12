using System.Text.RegularExpressions;
using Application.Models;
using Application.Security;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // Valida las credenciales del usuario y genera un token JWT si son correctas
    public AuthResponseDto Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // Buscamos al usuario por su email
        var user = _userRepository.GetByEmail(email);
        
        // Validamos existencia, si está activo y si la contraseña coincide
        if (user == null || !user.Active || user.Password != request.Password)
        {
            // Capturado por el Global Exception Handling
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        return _jwtTokenGenerator.GenerateToken(user);
    }

    // Registra un nuevo cliente en el sistema realizando validaciones de formato
    public AuthResponseDto Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // Validación de formato de email mediante expresión regular
        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; 
        if (!Regex.IsMatch(email, emailRegex))
        {
            throw new ArgumentException("El formato del correo electrónico no es válido.");
        }

        // Validación de existencia previa en la base de datos
        var existingUser = _userRepository.GetByEmail(email);
        if (existingUser != null)
        {
            throw new ArgumentException("El email ya se encuentra registrado.");
        }

        // Validaciones estrictas de seguridad para la contraseña
        var password = request.Password ?? string.Empty;
        if (password.Length <= 8 || !Regex.IsMatch(password, "[A-Z]") || !Regex.IsMatch(password, "[0-9]"))
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.");
        }

        // Creamos la entidad utilizando su constructor seguro
        var client = new Client(email, password);

        // Agregamos el cliente al repositorio y persistimos los cambios
        _userRepository.Add(client);

        return _jwtTokenGenerator.GenerateToken(client);
    }
}