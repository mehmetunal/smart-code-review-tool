using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Dto.Mssql.Auth;

namespace SmartCodeReview.Api.Controllers;

/// <summary>
/// Kimlik doğrulama controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcı kaydı
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Yeni kullanıcı oluşturuldu: {Email}", registerDto.Email);
                
                // User rolü ekle
                await _userManager.AddToRoleAsync(user, "User");
                
                return Ok(new { Message = "Kullanıcı başarıyla oluşturuldu" });
            }

            return BadRequest(new { Errors = result.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcı kaydı sırasında hata oluştu");
            return StatusCode(500, new { Message = "Kullanıcı kaydı başarısız" });
        }
    }

    /// <summary>
    /// Kullanıcı girişi
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                loginDto.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("Kullanıcı giriş yaptı: {Email}", loginDto.Email);
                return Ok(new { Message = "Giriş başarılı" });
            }

            if (result.IsLockedOut)
            {
                return BadRequest(new { Message = "Hesap kilitlendi" });
            }

            return Unauthorized(new { Message = "Email veya şifre hatalı" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Giriş sırasında hata oluştu");
            return StatusCode(500, new { Message = "Giriş başarısız" });
        }
    }
}

