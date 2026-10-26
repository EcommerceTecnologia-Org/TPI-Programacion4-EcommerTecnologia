using Application.Models;
using Domain.Entities;

namespace Application.Security;

public interface IJwtTokenGenerator
{
    AuthResponseDto GenerateToken(User user);
}