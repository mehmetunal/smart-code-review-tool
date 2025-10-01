using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartCodeReview.Data.Mssql;

namespace SmartCodeReview.Mssql;

/// <summary>
/// SmartCodeReview veritabanı context'i
/// </summary>
public class SmartCodeReviewDbContext : IdentityDbContext<User, Role, Guid>
{
    public SmartCodeReviewDbContext(DbContextOptions<SmartCodeReviewDbContext> options)
        : base(options)
    {
        // AsNoTracking global olarak aktif (performans için)
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    // DbSet'ler
    public DbSet<Project> Projects { get; set; }
    public DbSet<CodeReview> CodeReviews { get; set; }
    public DbSet<Analysis> Analyses { get; set; }
    public DbSet<FileAnalysis> FileAnalyses { get; set; }
    public DbSet<ApiConfiguration> ApiConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Identity tablolarının isimlerini özelleştir
        builder.Entity<User>().ToTable("Users");
        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        // User entity konfigürasyonu
        builder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.HasIndex(e => e.GitHubUsername);
            entity.HasIndex(e => e.GitLabUsername);
            
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.GitHubUsername).HasMaxLength(100);
            entity.Property(e => e.GitLabUsername).HasMaxLength(100);
        });

        // Project entity konfigürasyonu
        builder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.FullName);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsActive);
            
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.RepositoryUrl).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.WebhookSecret).HasMaxLength(100);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CodeReview entity konfigürasyonu
        builder.Entity<CodeReview>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PullRequestNumber);
            
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.PullRequestUrl).HasMaxLength(500).IsRequired();
            entity.Property(e => e.BranchName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            
            entity.HasOne(e => e.Project)
                .WithMany(p => p.CodeReviews)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.User)
                .WithMany(u => u.CodeReviews)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Analysis entity konfigürasyonu
        builder.Entity<Analysis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CodeReviewId);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Severity);
            
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000).IsRequired();
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
            entity.Property(e => e.CodeSnippet).HasMaxLength(4000);
            entity.Property(e => e.Suggestion).HasMaxLength(2000);
            
            entity.HasOne(e => e.CodeReview)
                .WithMany(c => c.Analyses)
                .HasForeignKey(e => e.CodeReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FileAnalysis entity konfigürasyonu
        builder.Entity<FileAnalysis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CodeReviewId);
            entity.HasIndex(e => e.Language);
            
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.DiffContent).HasMaxLength(int.MaxValue);
            
            entity.HasOne(e => e.CodeReview)
                .WithMany(c => c.FileAnalyses)
                .HasForeignKey(e => e.CodeReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

