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

// ── Project ───────────────────────────────────────────────────────────────────
public record ProjectDto(
    int Id,
    string Slug,
    string Title,
    string Description,
    string ImageUrl,
    IEnumerable<string> Categories,
    string? LiveUrl,
    IEnumerable<string> Technologies,
    int DisplayOrder
);

public class ProjectMutationDto
{
    [Required]
    [StringLength(160, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1200, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = "/assets/images/placeholder.png";

    [MinLength(1, ErrorMessage = "Select at least one category.")]
    public List<string> Categories { get; set; } = [];

    [StringLength(500)]
    [Url]
    public string? LiveUrl { get; set; }

    [MinLength(1, ErrorMessage = "Add at least one technology.")]
    public List<string> Technologies { get; set; } = [];

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
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
