namespace Application.Models;
// representa la informacion que envia el usuario para logearse
// Record para evitar modificaiones accidentales del DTO una vez recibido
public record LoginRequest(
    string Email,
    string Password
);