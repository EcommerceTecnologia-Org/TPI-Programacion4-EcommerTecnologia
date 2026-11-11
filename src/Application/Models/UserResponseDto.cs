namespace Application.Models;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool Active { get; set; }
    public string Role { get; set; } = string.Empty; 
}