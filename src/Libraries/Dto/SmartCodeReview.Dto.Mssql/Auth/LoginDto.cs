namespace SmartCodeReview.Dto.Mssql.Auth;

/// <summary>
/// Kullanıcı giriş DTO
/// </summary>
public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool RememberMe { get; set; } = false;
}

