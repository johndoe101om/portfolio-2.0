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

/// <summary>Portfolio project.</summary>
public class Project : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string CategoriesJson { get; set; } = "[]"; // JSON array of category strings
    public string? LiveUrl { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<ProjectTechnology> Technologies { get; set; } = new List<ProjectTechnology>();
}

/// <summary>Technology used in a project.</summary>
public class ProjectTechnology : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
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
