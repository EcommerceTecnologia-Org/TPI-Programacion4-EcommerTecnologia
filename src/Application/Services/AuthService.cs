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
    public async Task<AuthResponseDto> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.Active || user.Password != request.Password)
        {
            throw new UnauthorizedAccessException("Credenciales invalidas.");
        }

        return _jwtTokenGenerator.GenerateToken(user);
    }

    public async Task<AuthResponseDto> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // Validación de formato de email usando Regex
        // Rechaza strings vacíos, textos sin arroba, cadenas con espacios, o correos a los que les falte el dominio o la extensión final
        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; 
        if (!Regex.IsMatch(email, emailRegex))
        {
            throw new ArgumentException("El formato del correo electrónico no es válido.");
        }

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
        {
            throw new ArgumentException("El email ya se encuentra registrado.");
        }

        var password = request.Password ?? string.Empty;
        if (password.Length <= 8)
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.");
        }

        if (!Regex.IsMatch(password, "[A-Z]"))
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.");
        }

        if (!Regex.IsMatch(password, "[0-9]"))
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.");
        }

        var client = new Client
        {
            Email = email,
            Password = password,
            Active = true
        };

        _userRepository.Add(client);

        return _jwtTokenGenerator.GenerateToken(client);
    }
}
