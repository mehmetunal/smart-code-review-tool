using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Mssql;

namespace SmartCodeReview.Api.Extensions;

/// <summary>
/// Seed data extension metodları
/// </summary>
public static class SeedDataExtensions
{
    /// <summary>
    /// Seed data'yı oluşturur
    /// </summary>
    public static async Task SeedDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = services.GetRequiredService<SmartCodeReviewDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();

            // Veritabanı oluştur
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("Veritabanı oluşturuldu/kontrol edildi");

            // Rolleri oluştur
            await SeedRolesAsync(roleManager, logger);

            // Admin kullanıcısı oluştur
            await SeedAdminUserAsync(userManager, logger);

            logger.LogInformation("Seed data başarıyla oluşturuldu");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seed data oluşturulurken hata oluştu");
            throw;
        }
    }

    /// <summary>
    /// Rolleri oluşturur
    /// </summary>
    private static async Task SeedRolesAsync(RoleManager<Role> roleManager, ILogger logger)
    {
        string[] roles = { "Admin", "User" };

        foreach (var roleName in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var role = new Role
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    CreatedDate = DateTime.UtcNow
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Rol oluşturuldu: {RoleName}", roleName);
                }
                else
                {
                    logger.LogError("Rol oluşturulamadı: {RoleName}, Hatalar: {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    /// <summary>
    /// Admin kullanıcısı oluşturur
    /// </summary>
    private static async Task SeedAdminUserAsync(UserManager<User> userManager, ILogger logger)
    {
        const string adminEmail = "admin@gmail.com";
        const string adminPassword = "Super123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Admin kullanıcısı oluşturuldu: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Admin kullanıcısı oluşturulamadı: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Admin kullanıcısı zaten mevcut: {Email}", adminEmail);
        }
    }
}

