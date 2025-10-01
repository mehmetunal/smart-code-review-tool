using FluentMigrator;

namespace SmartCodeReview.Mssql.Migrations;

/// <summary>
/// İlk veritabanı migration'ı
/// Tüm tabloları oluşturur
/// </summary>
[Migration(20250101000001, "Initial database migration")]
public class InitialDatabaseMigration : Migration
{
    public override void Up()
    {
        // Projects tablosu
        Create.Table("Projects")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Description").AsString(1000).Nullable()
            .WithColumn("RepositoryUrl").AsString(500).NotNullable()
            .WithColumn("FullName").AsString(200).NotNullable()
            .WithColumn("RepositoryId").AsInt64().Nullable()
            .WithColumn("WebhookSecret").AsString(100).Nullable()
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("UserId").AsGuid().NotNullable()
            .WithColumn("CreatedDate").AsDateTime().NotNullable()
            .WithColumn("UpdatedDate").AsDateTime().Nullable()
            .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatorUserId").AsGuid().Nullable()
            .WithColumn("UpdatedByUserId").AsGuid().Nullable();

        Create.Index("IX_Projects_FullName").OnTable("Projects").OnColumn("FullName");
        Create.Index("IX_Projects_UserId").OnTable("Projects").OnColumn("UserId");
        Create.Index("IX_Projects_IsActive").OnTable("Projects").OnColumn("IsActive");

        // CodeReviews tablosu
        Create.Table("CodeReviews")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("PullRequestNumber").AsInt32().NotNullable()
            .WithColumn("Title").AsString(500).NotNullable()
            .WithColumn("Description").AsString(2000).Nullable()
            .WithColumn("PullRequestUrl").AsString(500).NotNullable()
            .WithColumn("BranchName").AsString(200).NotNullable()
            .WithColumn("Status").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("QualityScore").AsInt32().Nullable()
            .WithColumn("TotalIssuesCount").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("CriticalIssuesCount").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("HighIssuesCount").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("MediumIssuesCount").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("LowIssuesCount").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("AnalysisStartTime").AsDateTime().Nullable()
            .WithColumn("AnalysisEndTime").AsDateTime().Nullable()
            .WithColumn("ErrorMessage").AsString(2000).Nullable()
            .WithColumn("ProjectId").AsGuid().NotNullable()
            .WithColumn("UserId").AsGuid().Nullable()
            .WithColumn("CreatedDate").AsDateTime().NotNullable()
            .WithColumn("UpdatedDate").AsDateTime().Nullable()
            .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatorUserId").AsGuid().Nullable()
            .WithColumn("UpdatedByUserId").AsGuid().Nullable();

        Create.Index("IX_CodeReviews_ProjectId").OnTable("CodeReviews").OnColumn("ProjectId");
        Create.Index("IX_CodeReviews_UserId").OnTable("CodeReviews").OnColumn("UserId");
        Create.Index("IX_CodeReviews_Status").OnTable("CodeReviews").OnColumn("Status");
        Create.Index("IX_CodeReviews_PullRequestNumber").OnTable("CodeReviews").OnColumn("PullRequestNumber");

        // Analyses tablosu
        Create.Table("Analyses")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Title").AsString(500).NotNullable()
            .WithColumn("Description").AsString(2000).NotNullable()
            .WithColumn("Category").AsInt32().NotNullable()
            .WithColumn("Severity").AsInt32().NotNullable()
            .WithColumn("FilePath").AsString(500).NotNullable()
            .WithColumn("LineNumber").AsInt32().Nullable()
            .WithColumn("CodeSnippet").AsString(4000).Nullable()
            .WithColumn("Suggestion").AsString(2000).Nullable()
            .WithColumn("IsAIGenerated").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("CommentId").AsInt64().Nullable()
            .WithColumn("CodeReviewId").AsGuid().NotNullable()
            .WithColumn("CreatedDate").AsDateTime().NotNullable()
            .WithColumn("UpdatedDate").AsDateTime().Nullable()
            .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatorUserId").AsGuid().Nullable()
            .WithColumn("UpdatedByUserId").AsGuid().Nullable();

        Create.Index("IX_Analyses_CodeReviewId").OnTable("Analyses").OnColumn("CodeReviewId");
        Create.Index("IX_Analyses_Category").OnTable("Analyses").OnColumn("Category");
        Create.Index("IX_Analyses_Severity").OnTable("Analyses").OnColumn("Severity");

        // FileAnalyses tablosu
        Create.Table("FileAnalyses")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("FilePath").AsString(500).NotNullable()
            .WithColumn("FileName").AsString(255).NotNullable()
            .WithColumn("Language").AsInt32().NotNullable()
            .WithColumn("AddedLines").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("DeletedLines").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("TotalChanges").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("QualityScore").AsInt32().Nullable()
            .WithColumn("IssuesCount").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("DiffContent").AsString(int.MaxValue).Nullable()
            .WithColumn("CodeReviewId").AsGuid().NotNullable()
            .WithColumn("CreatedDate").AsDateTime().NotNullable()
            .WithColumn("UpdatedDate").AsDateTime().Nullable()
            .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatorUserId").AsGuid().Nullable()
            .WithColumn("UpdatedByUserId").AsGuid().Nullable();

        Create.Index("IX_FileAnalyses_CodeReviewId").OnTable("FileAnalyses").OnColumn("CodeReviewId");
        Create.Index("IX_FileAnalyses_Language").OnTable("FileAnalyses").OnColumn("Language");

        // Foreign key'ler
        Create.ForeignKey("FK_CodeReviews_Projects")
            .FromTable("CodeReviews").ForeignColumn("ProjectId")
            .ToTable("Projects").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_Analyses_CodeReviews")
            .FromTable("Analyses").ForeignColumn("CodeReviewId")
            .ToTable("CodeReviews").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_FileAnalyses_CodeReviews")
            .FromTable("FileAnalyses").ForeignColumn("CodeReviewId")
            .ToTable("CodeReviews").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table("FileAnalyses");
        Delete.Table("Analyses");
        Delete.Table("CodeReviews");
        Delete.Table("Projects");
    }
}

