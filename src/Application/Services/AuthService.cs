using Application.Models;
using Application.Security;
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
}