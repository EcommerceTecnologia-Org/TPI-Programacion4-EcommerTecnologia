using System.Runtime.CompilerServices;

namespace Application.Security;
// Configuración de seguridad para los tokens JWT, leída desde appsettings.json (sección "Jwt")
public class JwtSettings
{
    // Clave secreta usada para firmar y validar los tokens (HMAC-SHA256)

    public string Key { get; set; } = string.Empty;
    
    // Emisor del token (quién lo crea)
    public string Issuer { get; set; } = string.Empty;
    // Audiencia del token (para quién está destinado)
    public string Audience { get; set; } = string.Empty;
    // Minutos de vigencia del token antes de expirar

    public int ExpiresMinutes { get; set; } = 60;
}