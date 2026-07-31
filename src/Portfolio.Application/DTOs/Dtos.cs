using System.ComponentModel.DataAnnotations;

namespace Portfolio.Application.DTOs;

// ── Profile ──────────────────────────────────────────────────────────────────
public record ProfileDto(
    int Id,
    string FullName,
    string Title,
    string Subtitle,
    string AboutText,
    string Phone,
    string Email,
    string Website,
    string City,
    string Country,
    int Age,
    string Degree,
    string FreelanceStatus,
    string ProfileImageUrl,
    string CvUrl,
    double MapLat,
    double MapLng
);

// ── Social Link ───────────────────────────────────────────────────────────────
public record SocialLinkDto(
    int Id,
    string Platform,
    string Url,
    string IconClass,
    int DisplayOrder
);

// ── Skill ─────────────────────────────────────────────────────────────────────
public record SkillDto(
    int Id,
    string Name,
    int Percentage,
    string Category,
    string? LanguageLevel,
    int? FilledDots,
    int? TotalDots,
    int DisplayOrder
);

// ── Statistic ─────────────────────────────────────────────────────────────────
public record StatisticDto(
    int Id,
    string IconClass,
    int Value,
    string Label,
    int DisplayOrder
);

// ── Service ───────────────────────────────────────────────────────────────────
public record ServiceDto(
    int Id,
    string Title,
    string Description,
    string IconClass,
    int DisplayOrder
);

// ── Education ─────────────────────────────────────────────────────────────────
public record EducationDto(
    int Id,
    string Institution,
    string Period,
    string Description,
    int DisplayOrder
);

// ── Experience ────────────────────────────────────────────────────────────────
public record ExperienceDto(
    int Id,
    string Title,
    string? Company,
    string? Period,
    string Description,
    string Category,
    int DisplayOrder
);

// ── Project & Related DTOs ───────────────────────────────────────────────────
public record ProjectImageDto(
    int Id,
    string StoragePath,
    string PublicUrl,
    string? AltText,
    bool IsThumbnail,
    int DisplayOrder,
    int? Width,
    int? Height
);

public record ProjectLinkDto(
    int Id,
    string LinkType,
    string Url,
    string? Label
);

public record ProjectFeatureDto(
    int Id,
    string Title,
    string? Description,
    string? IconClass,
    int DisplayOrder
);

public record ProjectAchievementDto(
    int Id,
    string Title,
    string? Description,
    DateTimeOffset? DateAchieved,
    int DisplayOrder
);

public record ProjectDto(
    int Id,
    string Slug,
    string Title,
    string ShortDescription,
    string FullDescription,
    string Status,
    string Visibility,
    bool IsPublished,
    bool IsFeatured,
    bool IsDeleted,
    string ResumeCategory,
    string ExperienceType,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    bool IsCurrentlyWorking,
    string DurationText,
    string ReadmeMarkdown,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    string? OgImageUrl,
    int DisplayOrder,
    string ThumbnailUrl,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IEnumerable<string> Categories,
    IEnumerable<string> Technologies,
    IEnumerable<string> Skills,
    IEnumerable<ProjectImageDto> Images,
    IEnumerable<ProjectLinkDto> Links,
    IEnumerable<ProjectFeatureDto> Features,
    IEnumerable<ProjectAchievementDto> Achievements
);

public class ProjectMutationDto
{
    [Required(ErrorMessage = "Project title is required.")]
    [StringLength(200, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    public string? Slug { get; set; }

    [Required(ErrorMessage = "Short description is required.")]
    [StringLength(500, MinimumLength = 5)]
    public string ShortDescription { get; set; } = string.Empty;

    [StringLength(10000)]
    public string FullDescription { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "Completed"; // Completed | Planning | In Progress | Draft | Archived

    [Required]
    public string Visibility { get; set; } = "Public"; // Public | Private | Unlisted

    public bool IsPublished { get; set; } = true;
    public bool IsFeatured { get; set; } = false;

    [Required]
    public string ResumeCategory { get; set; } = "Web"; // Web | Mobile | AI/Cloud | DevOps | Game | Other

    [Required]
    public string ExperienceType { get; set; } = "Professional"; // Professional | Personal | OpenSource | Client | Academic

    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool IsCurrentlyWorking { get; set; } = false;

    public string ReadmeMarkdown { get; set; } = string.Empty;

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? OgImageUrl { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }

    public string ThumbnailUrl { get; set; } = "/assets/images/placeholder.png";

    public List<string> Categories { get; set; } = [];
    public List<string> Technologies { get; set; } = [];
    public List<string> Skills { get; set; } = [];

    public List<ProjectImageDto> Images { get; set; } = [];
    public List<ProjectLinkDto> Links { get; set; } = [];
    public List<ProjectFeatureDto> Features { get; set; } = [];
    public List<ProjectAchievementDto> Achievements { get; set; } = [];
}

public class ProjectListFilterDto
{
    public string? Search { get; set; }
    public string? Category { get; set; }
    public string? Technology { get; set; }
    public string? Status { get; set; }
    public int? Year { get; set; }
    public bool? Featured { get; set; }
    public string? ExperienceType { get; set; }
    public string? ResumeCategory { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "Newest"; // Newest | Oldest | Alphabetical | Updated | Duration | Featured | Manual
    public bool IncludeDeleted { get; set; } = false;
    public bool IncludeUnpublished { get; set; } = false;
}

public class PagedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)(PageSize > 0 ? PageSize : 10));
}

public class ProjectDashboardStatsDto
{
    public int TotalProjects { get; set; }
    public int PublishedProjects { get; set; }
    public int DraftProjects { get; set; }
    public int FeaturedProjects { get; set; }
    public int ArchivedProjects { get; set; }
    public int TotalTechnologies { get; set; }
    public int TotalCategories { get; set; }
    public IEnumerable<ProjectDto> RecentProjects { get; set; } = [];
}

public record ImageUploadResultDto(
    bool Success,
    string StoragePath,
    string PublicUrl,
    string Message
);

public record AuditLogDto(
    int Id,
    string EntityName,
    string EntityId,
    string Action,
    string PerformedBy,
    string ChangesJson,
    DateTimeOffset Timestamp
);

public class BulkActionRequestDto
{
    [Required]
    public List<int> ProjectIds { get; set; } = [];

    [Required]
    public string Action { get; set; } = string.Empty; // publish | unpublish | archive | restore | feature | delete
}

public class ReorderProjectsRequestDto
{
    [Required]
    public List<int> OrderedProjectIds { get; set; } = [];
}

// ── Blog Post ─────────────────────────────────────────────────────────────────
public record BlogPostDto(
    int Id,
    string Slug,
    string Title,
    string Excerpt,
    string? Content,
    string ImageUrl,
    DateTimeOffset PublishedAt,
    string Author,
    IEnumerable<string> Tags,
    bool IsPublished
);

public class BlogPostMutationDto
{
    [Required]
    [StringLength(180, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1200, MinimumLength = 10)]
    public string Excerpt { get; set; } = string.Empty;

    [StringLength(20000)]
    public string? Content { get; set; }

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = "/assets/images/placeholder.png";

    public DateTimeOffset? PublishedAt { get; set; }

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Author { get; set; } = "Satyam Kumar";

    public List<string> Tags { get; set; } = [];

    public bool IsPublished { get; set; } = true;
}

public class AdminLoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public record AdminAuthResponse(
    string Token,
    string Email,
    DateTimeOffset ExpiresAt
);

// ── Contact Message ───────────────────────────────────────────────────────────
public class ContactMessageDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Subject is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Subject must be between 3 and 200 characters.")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message is required.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 2000 characters.")]
    public string Message { get; set; } = string.Empty;
}

// ── Contact Result ────────────────────────────────────────────────────────────
public record ContactResult(bool Success, string Message);
