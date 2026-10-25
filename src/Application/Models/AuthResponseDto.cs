namespace Application.Models;

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt,
    string Email,
    string Role
);