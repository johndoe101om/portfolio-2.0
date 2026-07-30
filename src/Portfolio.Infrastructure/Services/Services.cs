using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;
using Portfolio.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Portfolio.Infrastructure.Services;

// ── ProfileService ────────────────────────────────────────────────────────────
public class ProfileService(PortfolioDbContext db) : IProfileService
{
    public async Task<ProfileDto?> GetProfileAsync(CancellationToken ct = default)
    {
        var p = await db.Profiles.AsNoTracking().FirstOrDefaultAsync(ct);
        if (p is null) return null;
        return new ProfileDto(p.Id, p.FullName, p.Title, p.Subtitle, p.AboutText,
            p.Phone, p.Email, p.Website, p.City, p.Country, p.Age, p.Degree,
            p.FreelanceStatus, p.ProfileImageUrl, p.CvUrl, p.MapLat, p.MapLng);
    }

    public async Task<IEnumerable<SocialLinkDto>> GetSocialLinksAsync(CancellationToken ct = default)
    {
        return await db.SocialLinks.AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SocialLinkDto(s.Id, s.Platform, s.Url, s.IconClass, s.DisplayOrder))
            .ToListAsync(ct);
    }

    public async Task<Dictionary<string, string>> GetSiteSettingsAsync(CancellationToken ct = default)
    {
        return await db.SiteSettings.AsNoTracking()
            .ToDictionaryAsync(s => s.Key, s => s.Value, ct);
    }
}

// ── SkillService ──────────────────────────────────────────────────────────────
public class SkillService(PortfolioDbContext db) : ISkillService
{
    public async Task<IEnumerable<SkillDto>> GetSkillsAsync(CancellationToken ct = default)
    {
        return await db.Skills.AsNoTracking()
            .OrderBy(s => s.Category).ThenBy(s => s.DisplayOrder)
            .Select(s => new SkillDto(s.Id, s.Name, s.Percentage, s.Category,
                s.LanguageLevel, s.FilledDots, s.TotalDots, s.DisplayOrder))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ExperienceDto>> GetExperiencesAsync(CancellationToken ct = default)
    {
        return await db.Experiences.AsNoTracking()
            .OrderBy(e => e.DisplayOrder)
            .Select(e => new ExperienceDto(e.Id, e.Title, e.Company, e.Period,
                e.Description, e.Category, e.DisplayOrder))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<EducationDto>> GetEducationAsync(CancellationToken ct = default)
    {
        return await db.Educations.AsNoTracking()
            .OrderBy(e => e.DisplayOrder)
            .Select(e => new EducationDto(e.Id, e.Institution, e.Period, e.Description, e.DisplayOrder))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ServiceDto>> GetServicesAsync(CancellationToken ct = default)
    {
        return await db.Services.AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new ServiceDto(s.Id, s.Title, s.Description, s.IconClass, s.DisplayOrder))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<StatisticDto>> GetStatisticsAsync(CancellationToken ct = default)
    {
        return await db.Statistics.AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new StatisticDto(s.Id, s.IconClass, s.Value, s.Label, s.DisplayOrder))
            .ToListAsync(ct);
    }
}

// ── ProjectService ────────────────────────────────────────────────────────────
public class ProjectService(PortfolioDbContext db) : IProjectService
{
    public async Task<IEnumerable<ProjectDto>> GetProjectsAsync(string? category = null, CancellationToken ct = default)
    {
        var query = db.Projects.AsNoTracking()
            .Include(p => p.Technologies)
            .OrderBy(p => p.DisplayOrder);

        var projects = await query.ToListAsync(ct);

        var dtos = projects.Select(p => ToDto(p));

        if (!string.IsNullOrWhiteSpace(category) && category != "*")
        {
            dtos = dtos.Where(p => p.Categories.Contains(category));
        }

        return dtos.ToList();
    }

    public async Task<ProjectDto?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var p = await db.Projects.AsNoTracking()
            .Include(p => p.Technologies)
            .FirstOrDefaultAsync(p => p.Slug == slug, ct);
        return p is null ? null : ToDto(p);
    }

    public async Task<ProjectDto> CreateAsync(ProjectMutationDto dto, CancellationToken ct = default)
    {
        var project = new Portfolio.Domain.Entities.Project
        {
            Slug = await GenerateUniqueProjectSlugAsync(dto.Title, null, ct),
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            ImageUrl = NormalizeUrl(dto.ImageUrl, "/assets/images/placeholder.png"),
            CategoriesJson = JsonSerializer.Serialize(NormalizeList(dto.Categories)),
            LiveUrl = string.IsNullOrWhiteSpace(dto.LiveUrl) ? null : dto.LiveUrl.Trim(),
            DisplayOrder = dto.DisplayOrder,
            Technologies = NormalizeList(dto.Technologies)
                .Select(name => new Portfolio.Domain.Entities.ProjectTechnology { Name = name })
                .ToList(),
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);
        return ToDto(project);
    }

    public async Task<ProjectDto?> UpdateAsync(int id, ProjectMutationDto dto, CancellationToken ct = default)
    {
        var project = await db.Projects
            .Include(p => p.Technologies)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (project is null)
            return null;

        project.Slug = await GenerateUniqueProjectSlugAsync(dto.Title, id, ct);
        project.Title = dto.Title.Trim();
        project.Description = dto.Description.Trim();
        project.ImageUrl = NormalizeUrl(dto.ImageUrl, "/assets/images/placeholder.png");
        project.CategoriesJson = JsonSerializer.Serialize(NormalizeList(dto.Categories));
        project.LiveUrl = string.IsNullOrWhiteSpace(dto.LiveUrl) ? null : dto.LiveUrl.Trim();
        project.DisplayOrder = dto.DisplayOrder;

        db.ProjectTechnologies.RemoveRange(project.Technologies);
        project.Technologies = NormalizeList(dto.Technologies)
            .Select(name => new Portfolio.Domain.Entities.ProjectTechnology { ProjectId = id, Name = name })
            .ToList();

        await db.SaveChangesAsync(ct);
        return ToDto(project);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var project = await db.Projects.FindAsync([id], ct);
        if (project is null)
            return false;

        db.Projects.Remove(project);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static ProjectDto ToDto(Portfolio.Domain.Entities.Project p)
    {
        var cats = JsonSerializer.Deserialize<IEnumerable<string>>(p.CategoriesJson) ?? [];
        var techs = p.Technologies.Select(t => t.Name);
        return new ProjectDto(p.Id, p.Slug, p.Title, p.Description, p.ImageUrl,
            cats, p.LiveUrl, techs, p.DisplayOrder);
    }

    private async Task<string> GenerateUniqueProjectSlugAsync(string title, int? existingId, CancellationToken ct)
    {
        var baseSlug = ToSlug(title);
        var slug = baseSlug;
        var suffix = 2;

        while (await db.Projects.AnyAsync(p => p.Slug == slug && (!existingId.HasValue || p.Id != existingId.Value), ct))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }

    private static string ToSlug(string value)
    {
        var builder = new StringBuilder();
        var lastWasDash = false;

        foreach (var c in value.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(c))
            {
                builder.Append(c);
                lastWasDash = false;
            }
            else if (!lastWasDash)
            {
                builder.Append('-');
                lastWasDash = true;
            }
        }

        var slug = builder.ToString().Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "project" : slug;
    }

    private static List<string> NormalizeList(IEnumerable<string> values) =>
        values.Select(v => v.Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    private static string NormalizeUrl(string? url, string fallback) =>
        string.IsNullOrWhiteSpace(url) ? fallback : url.Trim();
}

// ── BlogService ───────────────────────────────────────────────────────────────
public class BlogService(PortfolioDbContext db) : IBlogService
{
    public async Task<(IEnumerable<BlogPostDto>, int)> GetPostsAsync(
        int page,
        int pageSize,
        bool includeUnpublished = false,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var query = db.BlogPosts.AsNoTracking();

        if (!includeUnpublished)
            query = query.Where(b => b.IsPublished);

        query = query.OrderByDescending(b => b.PublishedAt);

        var totalCount = await query.CountAsync(ct);
        var posts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (posts.Select(ToDto), totalCount);
    }

    public async Task<BlogPostDto?> GetBySlugAsync(string slug, bool includeUnpublished = false, CancellationToken ct = default)
    {
        var query = db.BlogPosts.AsNoTracking().Where(b => b.Slug == slug);

        if (!includeUnpublished)
            query = query.Where(b => b.IsPublished);

        var post = await query.FirstOrDefaultAsync(ct);
        return post is null ? null : ToDto(post);
    }

    public async Task<BlogPostDto> CreateAsync(BlogPostMutationDto dto, CancellationToken ct = default)
    {
        var post = new Portfolio.Domain.Entities.BlogPost
        {
            Slug = await GenerateUniqueBlogSlugAsync(dto.Title, null, ct),
            Title = dto.Title.Trim(),
            Excerpt = dto.Excerpt.Trim(),
            Content = dto.Content?.Trim() ?? string.Empty,
            ImageUrl = NormalizeUrl(dto.ImageUrl, "/assets/images/placeholder.png"),
            PublishedAt = dto.PublishedAt ?? DateTimeOffset.UtcNow,
            Author = dto.Author.Trim(),
            TagsJson = JsonSerializer.Serialize(NormalizeList(dto.Tags)),
            IsPublished = dto.IsPublished,
        };

        db.BlogPosts.Add(post);
        await db.SaveChangesAsync(ct);
        return ToDto(post);
    }

    public async Task<BlogPostDto?> UpdateAsync(int id, BlogPostMutationDto dto, CancellationToken ct = default)
    {
        var post = await db.BlogPosts.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (post is null)
            return null;

        post.Slug = await GenerateUniqueBlogSlugAsync(dto.Title, id, ct);
        post.Title = dto.Title.Trim();
        post.Excerpt = dto.Excerpt.Trim();
        post.Content = dto.Content?.Trim() ?? string.Empty;
        post.ImageUrl = NormalizeUrl(dto.ImageUrl, "/assets/images/placeholder.png");
        post.PublishedAt = dto.PublishedAt ?? post.PublishedAt;
        post.Author = dto.Author.Trim();
        post.TagsJson = JsonSerializer.Serialize(NormalizeList(dto.Tags));
        post.IsPublished = dto.IsPublished;

        await db.SaveChangesAsync(ct);
        return ToDto(post);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var post = await db.BlogPosts.FindAsync([id], ct);
        if (post is null)
            return false;

        db.BlogPosts.Remove(post);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static BlogPostDto ToDto(Portfolio.Domain.Entities.BlogPost b)
    {
        var tags = JsonSerializer.Deserialize<IEnumerable<string>>(b.TagsJson) ?? [];
        return new BlogPostDto(b.Id, b.Slug, b.Title, b.Excerpt,
            string.IsNullOrEmpty(b.Content) ? null : b.Content,
            b.ImageUrl, b.PublishedAt, b.Author, tags, b.IsPublished);
    }

    private async Task<string> GenerateUniqueBlogSlugAsync(string title, int? existingId, CancellationToken ct)
    {
        var baseSlug = ToSlug(title, "post");
        var slug = baseSlug;
        var suffix = 2;

        while (await db.BlogPosts.AnyAsync(b => b.Slug == slug && (!existingId.HasValue || b.Id != existingId.Value), ct))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }

    private static string ToSlug(string value, string fallback)
    {
        var builder = new StringBuilder();
        var lastWasDash = false;

        foreach (var c in value.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(c))
            {
                builder.Append(c);
                lastWasDash = false;
            }
            else if (!lastWasDash)
            {
                builder.Append('-');
                lastWasDash = true;
            }
        }

        var slug = builder.ToString().Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? fallback : slug;
    }

    private static List<string> NormalizeList(IEnumerable<string> values) =>
        values.Select(v => v.Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    private static string NormalizeUrl(string? url, string fallback) =>
        string.IsNullOrWhiteSpace(url) ? fallback : url.Trim();
}

// ── ContactService ────────────────────────────────────────────────────────────
public class AdminAuthService(IConfiguration config) : IAdminAuthService
{
    private const string DefaultEmail = "johndoeunique101@gmail.com";
    private const string DefaultPassword = "$Atyam@100.";
    private const int DefaultTokenHours = 8;

    public AdminAuthResponse? SignIn(AdminLoginRequest request)
    {
        var email = GetAdminEmail();
        var password = GetAdminPassword();

        if (!FixedEquals(request.Email.Trim().ToLowerInvariant(), email.ToLowerInvariant()) ||
            !FixedEquals(request.Password, password))
        {
            return null;
        }

        var expiresAt = DateTimeOffset.UtcNow.AddHours(GetTokenHours());
        var payload = new AdminTokenPayload(email, expiresAt.ToUnixTimeSeconds(), "admin");
        var encodedPayload = Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
        var signature = Sign(encodedPayload);

        return new AdminAuthResponse($"{encodedPayload}.{signature}", email, expiresAt);
    }

    public bool IsTokenValid(string? authorizationHeader)
    {
        var token = ExtractBearerToken(authorizationHeader);
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var parts = token.Split('.', 2);
        if (parts.Length != 2)
            return false;

        var expectedSignature = Sign(parts[0]);
        if (!FixedEquals(parts[1], expectedSignature))
            return false;

        try
        {
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
            var payload = JsonSerializer.Deserialize<AdminTokenPayload>(payloadJson);
            if (payload is null)
                return false;

            return payload.Role == "admin" &&
                   payload.Email.Equals(GetAdminEmail(), StringComparison.OrdinalIgnoreCase) &&
                   payload.ExpiresAtUnix > DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        catch (FormatException)
        {
            return false;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private string GetAdminEmail() =>
        config["AdminAuth:Email"] ??
        Environment.GetEnvironmentVariable("ADMIN_EMAIL") ??
        DefaultEmail;

    private string GetAdminPassword() =>
        config["AdminAuth:Password"] ??
        Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ??
        DefaultPassword;

    private string GetSigningKey() =>
        config["AdminAuth:TokenSigningKey"] ??
        Environment.GetEnvironmentVariable("ADMIN_TOKEN_SIGNING_KEY") ??
        $"{GetAdminEmail()}:{GetAdminPassword()}:portfolio-admin-token-v1";

    private int GetTokenHours()
    {
        var value = config["AdminAuth:TokenHours"] ?? Environment.GetEnvironmentVariable("ADMIN_TOKEN_HOURS");
        return int.TryParse(value, out var hours) && hours > 0 ? hours : DefaultTokenHours;
    }

    private string Sign(string encodedPayload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(GetSigningKey()));
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(encodedPayload)));
    }

    private static string? ExtractBearerToken(string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
            return null;

        const string prefix = "Bearer ";
        return authorizationHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? authorizationHeader[prefix.Length..].Trim()
            : authorizationHeader.Trim();
    }

    private static bool FixedEquals(string left, string right)
    {
        var leftHash = SHA256.HashData(Encoding.UTF8.GetBytes(left));
        var rightHash = SHA256.HashData(Encoding.UTF8.GetBytes(right));
        return CryptographicOperations.FixedTimeEquals(leftHash, rightHash);
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value
            .Replace('-', '+')
            .Replace('_', '/');

        padded += (padded.Length % 4) switch
        {
            2 => "==",
            3 => "=",
            0 => string.Empty,
            _ => throw new FormatException("Invalid base64url token."),
        };

        return Convert.FromBase64String(padded);
    }

    private sealed record AdminTokenPayload(string Email, long ExpiresAtUnix, string Role);
}

public class ContactService(PortfolioDbContext db, IEmailService email, ILogger<ContactService> logger)
    : IContactService
{
    public async Task<ContactResult> SubmitMessageAsync(ContactMessageDto dto, string? ip, CancellationToken ct = default)
    {
        // Basic duplicate detection: same email + subject within 5 minutes
        var fiveMinutesAgo = DateTimeOffset.UtcNow.AddMinutes(-5);
        var isDuplicate = await db.ContactMessages.AnyAsync(m =>
            m.Email == dto.Email &&
            m.Subject == dto.Subject &&
            m.CreatedAt >= fiveMinutesAgo, ct);

        if (isDuplicate)
            return new ContactResult(false, "Duplicate submission detected. Please wait before sending again.");

        var message = new Portfolio.Domain.Entities.ContactMessage
        {
            Name = dto.Name,
            Email = dto.Email,
            Subject = dto.Subject,
            Message = dto.Message,
            IpAddress = ip,
        };

        db.ContactMessages.Add(message);
        await db.SaveChangesAsync(ct);

        // Attempt email notification (non-fatal on failure)
        try
        {
            var sent = await email.SendContactNotificationAsync(
                dto.Name, dto.Email, dto.Subject, dto.Message, ct);
            message.EmailSent = sent;
            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email notification for contact message {Id}", message.Id);
        }

        return new ContactResult(true, "Message received successfully.");
    }
}
