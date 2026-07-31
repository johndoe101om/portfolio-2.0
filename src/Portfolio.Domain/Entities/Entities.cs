namespace Portfolio.Domain.Entities;

/// <summary>Base class for all domain entities.</summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>Portfolio owner's profile information.</summary>
public class Profile : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string AboutText { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string FreelanceStatus { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public string CvUrl { get; set; } = string.Empty;
    public double MapLat { get; set; }
    public double MapLng { get; set; }
}

/// <summary>Social media / external links.</summary>
public class SocialLink : BaseEntity
{
    public string Platform { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

/// <summary>Technical or language skill.</summary>
public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Percentage { get; set; }
    public string Category { get; set; } = "technical"; // technical | language
    public string? LanguageLevel { get; set; }
    public int? FilledDots { get; set; }
    public int? TotalDots { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>Statistics counter (projects done, designs, etc.).</summary>
public class Statistic : BaseEntity
{
    public string IconClass { get; set; } = string.Empty;
    public int Value { get; set; }
    public string Label { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

/// <summary>Service offering (web design, devops, etc.).</summary>
public class Service : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

/// <summary>Education history entry.</summary>
public class Education : BaseEntity
{
    public string Institution { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

/// <summary>Work experience or soft skill entry.</summary>
public class Experience : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Period { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "softskill"; // experience | softskill
    public int DisplayOrder { get; set; }
}

/// <summary>Master Technology dictionary entity.</summary>
public class Technology : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public string? BadgeColor { get; set; }
}

/// <summary>Master Category entity.</summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g. webdesign, webapp
    public string DisplayName { get; set; } = string.Empty; // e.g. Web Design, Web App
}

/// <summary>Portfolio project master entity.</summary>
public class Project : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string FullDescription { get; set; } = string.Empty;
    public string Status { get; set; } = "Completed"; // Completed | Planning | In Progress | Draft | Archived
    public string Visibility { get; set; } = "Public"; // Public | Private | Unlisted
    public bool IsPublished { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public bool IsDeleted { get; set; } = false; // Soft delete
    public string ResumeCategory { get; set; } = "Web"; // Web | Mobile | AI/Cloud | DevOps | Game | Other
    public string ExperienceType { get; set; } = "Professional"; // Professional | Personal | OpenSource | Client | Academic
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool IsCurrentlyWorking { get; set; } = false;
    public string ReadmeMarkdown { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? OgImageUrl { get; set; }
    public int DisplayOrder { get; set; }

    // Media & Thumbnail helper
    public string ThumbnailUrl { get; set; } = "/assets/images/placeholder.png";

    // Relational Collections
    public ICollection<ProjectImage> Images { get; set; } = new List<ProjectImage>();
    public ICollection<ProjectTechnology> ProjectTechnologies { get; set; } = new List<ProjectTechnology>();
    public ICollection<ProjectCategoryMapping> ProjectCategories { get; set; } = new List<ProjectCategoryMapping>();
    public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();
    public ICollection<ProjectLink> Links { get; set; } = new List<ProjectLink>();
    public ICollection<ProjectFeature> Features { get; set; } = new List<ProjectFeature>();
    public ICollection<ProjectAchievement> Achievements { get; set; } = new List<ProjectAchievement>();
}

/// <summary>Relational table connecting Project and Technology.</summary>
public class ProjectTechnology : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int TechnologyId { get; set; }
    public Technology Technology { get; set; } = null!;
}

/// <summary>Relational table connecting Project and Category.</summary>
public class ProjectCategoryMapping : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}

/// <summary>Relational table connecting Project and Skill.</summary>
public class ProjectSkill : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
}

/// <summary>Gallery and Thumbnail image stored in Supabase Storage.</summary>
public class ProjectImage : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string StoragePath { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsThumbnail { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    public int? Width { get; set; }
    public int? Height { get; set; }
}

/// <summary>External project links.</summary>
public class ProjectLink : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string LinkType { get; set; } = "Live"; // Live | GitHub | GitLab | Bitbucket | Documentation | YouTube | PlayStore | AppStore | Figma | CaseStudy | Blog
    public string Url { get; set; } = string.Empty;
    public string? Label { get; set; }
}

/// <summary>Dynamic project key feature.</summary>
public class ProjectFeature : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>Dynamic project achievement or milestone.</summary>
public class ProjectAchievement : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset? DateAchieved { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>Audit Log for all project mutations.</summary>
public class AuditLog : BaseEntity
{
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create | Update | Delete | SoftDelete | Restore | Duplicate | Publish | Unpublish | Archive | Feature
    public string PerformedBy { get; set; } = "Admin";
    public string ChangesJson { get; set; } = "{}";
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>Blog post.</summary>
public class BlogPost : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTimeOffset PublishedAt { get; set; }
    public string Author { get; set; } = string.Empty;
    public string TagsJson { get; set; } = "[]"; // JSON array
    public bool IsPublished { get; set; } = true;
}

/// <summary>Contact form submission.</summary>
public class ContactMessage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public bool IsRead { get; set; } = false;
    public bool EmailSent { get; set; } = false;
}

/// <summary>Key-value site settings.</summary>
public class SiteSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>Testimonial / quote.</summary>
public class Testimonial : BaseEntity
{
    public string Quote { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorTitle { get; set; } = string.Empty;
    public string AuthorImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
