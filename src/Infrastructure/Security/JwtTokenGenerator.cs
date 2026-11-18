using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Models;
using Application.Security;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;
// Responsable de crear tokens JWT firmados para la autenticación de usuarios
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    // Genera el token que se entrega al usuario luego de loguearse o registrarse
    public AuthResponseDto GenerateToken(User user)
    {
        // Resolvemos el rol a partir del tipo concreto de la entidad (polimorfismo TPH),
        // este claim será lo que verifiquen los [Authorize] de los controllers
        string role = user switch
        {
            Superadmin => "Superadmin",
            Admin => "Admin",
            Client => "Client",
            _ => throw new InvalidOperationException("Tipo de usuario no soportado para autenticacion")
        };
        // Claims que viajan dentro del token: id, email y rol del usuario
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, role)
        };
        // Firmamos el token con HMAC-SHA256 usando la clave secreta de configuración,
        // valida que el token no fue alterado
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Vencimiento del token calculado en UTC según la configuración
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt, 
            signingCredentials: creds   
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
 
        return new AuthResponseDto(tokenString, expiresAt, user.Email, role);

    }
}