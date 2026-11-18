using Application.Models;
using Domain.Entities;

namespace Application.Security;
// Abstracción para la generación de tokens JWT (se implementa en Infrastructure/Security/JwtTokenGenerator)


public interface IJwtTokenGenerator
{
    // Genera un token firmado a partir de los datos y rol del usuario

    AuthResponseDto GenerateToken(User user);

}