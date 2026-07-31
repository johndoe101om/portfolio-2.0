using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
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
public class ProjectService(IProjectRepository repo, IUnitOfWork uow, PortfolioDbContext db) : IProjectService
{
    public async Task<IEnumerable<ProjectDto>> GetProjectsAsync(string? category = null, CancellationToken ct = default)
    {
        var filter = new ProjectListFilterDto
        {
            Category = category,
            Page = 1,
            PageSize = 1000,
            IncludeUnpublished = false
        };
        var paged = await repo.GetPagedAsync(filter, ct);
        return paged.Items.Select(ToDto);
    }

    public async Task<PagedResultDto<ProjectDto>> GetPagedProjectsAsync(ProjectListFilterDto filter, CancellationToken ct = default)
    {
        var paged = await repo.GetPagedAsync(filter, ct);
        return new PagedResultDto<ProjectDto>
        {
            Items = paged.Items.Select(ToDto),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<ProjectDto?> GetBySlugAsync(string slug, bool includeUnpublished = false, CancellationToken ct = default)
    {
        var p = await repo.GetBySlugAsync(slug, includeUnpublished, ct);
        return p is null ? null : ToDto(p);
    }

    public async Task<ProjectDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var p = await repo.GetDetailsByIdAsync(id, includeDeleted: true, ct);
        return p is null ? null : ToDto(p);
    }

    public async Task<ProjectDto> CreateAsync(ProjectMutationDto dto, string performedBy = "Admin", CancellationToken ct = default)
    {
        var slug = string.IsNullOrWhiteSpace(dto.Slug)
            ? await GenerateSlugAsync(dto.Title, null, ct)
            : await GenerateSlugAsync(dto.Slug, null, ct);

        var project = new Project
        {
            Slug = slug,
            Title = dto.Title.Trim(),
            ShortDescription = dto.ShortDescription.Trim(),
            FullDescription = dto.FullDescription?.Trim() ?? string.Empty,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Completed" : dto.Status.Trim(),
            Visibility = string.IsNullOrWhiteSpace(dto.Visibility) ? "Public" : dto.Visibility.Trim(),
            IsPublished = dto.IsPublished,
            IsFeatured = dto.IsFeatured,
            ResumeCategory = string.IsNullOrWhiteSpace(dto.ResumeCategory) ? "Web" : dto.ResumeCategory.Trim(),
            ExperienceType = string.IsNullOrWhiteSpace(dto.ExperienceType) ? "Professional" : dto.ExperienceType.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCurrentlyWorking = dto.IsCurrentlyWorking,
            ReadmeMarkdown = dto.ReadmeMarkdown ?? string.Empty,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords,
            OgImageUrl = dto.OgImageUrl,
            DisplayOrder = dto.DisplayOrder > 0 ? dto.DisplayOrder : (await repo.GetMaxDisplayOrderAsync(ct) + 1),
            ThumbnailUrl = string.IsNullOrWhiteSpace(dto.ThumbnailUrl) ? "/assets/images/placeholder.png" : dto.ThumbnailUrl.Trim()
        };

        await MapRelationsAsync(project, dto, ct);
        await repo.AddAsync(project, ct);

        var audit = new AuditLog
        {
            EntityName = "Project",
            EntityId = project.Slug,
            Action = "Create",
            PerformedBy = performedBy,
            ChangesJson = JsonSerializer.Serialize(new { project.Title, project.Slug, project.Status, project.IsPublished }),
            Timestamp = DateTimeOffset.UtcNow
        };
        await repo.AddAuditLogAsync(audit, ct);

        await uow.SaveChangesAsync(ct);

        var created = await repo.GetDetailsByIdAsync(project.Id, includeDeleted: true, ct);
        return ToDto(created!);
    }

    public async Task<ProjectDto?> UpdateAsync(int id, ProjectMutationDto dto, string performedBy = "Admin", CancellationToken ct = default)
    {
        var project = await repo.GetDetailsByIdAsync(id, includeDeleted: true, ct);
        if (project is null) return null;

        var newSlug = string.IsNullOrWhiteSpace(dto.Slug)
            ? await GenerateSlugAsync(dto.Title, id, ct)
            : await GenerateSlugAsync(dto.Slug, id, ct);

        project.Slug = newSlug;
        project.Title = dto.Title.Trim();
        project.ShortDescription = dto.ShortDescription.Trim();
        project.FullDescription = dto.FullDescription?.Trim() ?? string.Empty;
        project.Status = string.IsNullOrWhiteSpace(dto.Status) ? project.Status : dto.Status.Trim();
        project.Visibility = string.IsNullOrWhiteSpace(dto.Visibility) ? project.Visibility : dto.Visibility.Trim();
        project.IsPublished = dto.IsPublished;
        project.IsFeatured = dto.IsFeatured;
        project.ResumeCategory = string.IsNullOrWhiteSpace(dto.ResumeCategory) ? project.ResumeCategory : dto.ResumeCategory.Trim();
        project.ExperienceType = string.IsNullOrWhiteSpace(dto.ExperienceType) ? project.ExperienceType : dto.ExperienceType.Trim();
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        project.IsCurrentlyWorking = dto.IsCurrentlyWorking;
        project.ReadmeMarkdown = dto.ReadmeMarkdown ?? string.Empty;
        project.MetaTitle = dto.MetaTitle;
        project.MetaDescription = dto.MetaDescription;
        project.MetaKeywords = dto.MetaKeywords;
        project.OgImageUrl = dto.OgImageUrl;
        project.DisplayOrder = dto.DisplayOrder;
        if (!string.IsNullOrWhiteSpace(dto.ThumbnailUrl))
        {
            project.ThumbnailUrl = dto.ThumbnailUrl.Trim();
        }

        // Clear and rebuild relational collections
        project.ProjectTechnologies.Clear();
        project.ProjectCategories.Clear();
        project.ProjectSkills.Clear();
        project.Images.Clear();
        project.Links.Clear();
        project.Features.Clear();
        project.Achievements.Clear();

        await MapRelationsAsync(project, dto, ct);
        repo.Update(project);

        var audit = new AuditLog
        {
            EntityName = "Project",
            EntityId = id.ToString(),
            Action = "Update",
            PerformedBy = performedBy,
            ChangesJson = JsonSerializer.Serialize(new { project.Title, project.Slug, project.Status, project.IsPublished }),
            Timestamp = DateTimeOffset.UtcNow
        };
        await repo.AddAuditLogAsync(audit, ct);

        await uow.SaveChangesAsync(ct);

        var updated = await repo.GetDetailsByIdAsync(id, includeDeleted: true, ct);
        return ToDto(updated!);
    }

    public async Task<bool> DeleteAsync(int id, bool permanent = false, string performedBy = "Admin", CancellationToken ct = default)
    {
        var project = await repo.GetDetailsByIdAsync(id, includeDeleted: true, ct);
        if (project is null) return false;

        if (permanent)
        {
            repo.Delete(project);
        }
        else
        {
            project.IsDeleted = true;
            repo.Update(project);
        }

        var audit = new AuditLog
        {
            EntityName = "Project",
            EntityId = id.ToString(),
            Action = permanent ? "Delete" : "SoftDelete",
            PerformedBy = performedBy,
            ChangesJson = JsonSerializer.Serialize(new { project.Title, Permanent = permanent }),
            Timestamp = DateTimeOffset.UtcNow
        };
        await repo.AddAuditLogAsync(audit, ct);

        await uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<ProjectDto?> DuplicateAsync(int id, string performedBy = "Admin", CancellationToken ct = default)
    {
        var source = await repo.GetDetailsByIdAsync(id, includeDeleted: true, ct);
        if (source is null) return null;

        var copyDto = new ProjectMutationDto
        {
            Title = $"{source.Title} (Copy)",
            Slug = await GenerateSlugAsync($"{source.Slug}-copy", null, ct),
            ShortDescription = source.ShortDescription,
            FullDescription = source.FullDescription,
            Status = "Draft",
            Visibility = source.Visibility,
            IsPublished = false,
            IsFeatured = false,
            ResumeCategory = source.ResumeCategory,
            ExperienceType = source.ExperienceType,
            StartDate = null, // Everything except dates
            EndDate = null,
            IsCurrentlyWorking = false,
            ReadmeMarkdown = source.ReadmeMarkdown,
            MetaTitle = source.MetaTitle,
            MetaDescription = source.MetaDescription,
            MetaKeywords = source.MetaKeywords,
            OgImageUrl = source.OgImageUrl,
            DisplayOrder = await repo.GetMaxDisplayOrderAsync(ct) + 1,
            ThumbnailUrl = source.ThumbnailUrl,
            Categories = source.ProjectCategories.Select(pc => pc.Category.DisplayName).ToList(),
            Technologies = source.ProjectTechnologies.Select(pt => pt.Technology.Name).ToList(),
            Skills = source.ProjectSkills.Select(ps => ps.Skill.Name).ToList(),
            Images = source.Images.Select(i => new ProjectImageDto(0, i.StoragePath, i.PublicUrl, i.AltText, i.IsThumbnail, i.DisplayOrder, i.Width, i.Height)).ToList(),
            Links = source.Links.Select(l => new ProjectLinkDto(0, l.LinkType, l.Url, l.Label)).ToList(),
            Features = source.Features.Select(f => new ProjectFeatureDto(0, f.Title, f.Description, f.IconClass, f.DisplayOrder)).ToList(),
            Achievements = source.Achievements.Select(a => new ProjectAchievementDto(0, a.Title, a.Description, null, a.DisplayOrder)).ToList()
        };

        var created = await CreateAsync(copyDto, performedBy, ct);

        var audit = new AuditLog
        {
            EntityName = "Project",
            EntityId = created.Id.ToString(),
            Action = "Duplicate",
            PerformedBy = performedBy,
            ChangesJson = JsonSerializer.Serialize(new { SourceId = id, ClonedSlug = created.Slug }),
            Timestamp = DateTimeOffset.UtcNow
        };
        await repo.AddAuditLogAsync(audit, ct);
        await uow.SaveChangesAsync(ct);

        return created;
    }

    public async Task<ProjectDto?> PublishAsync(int id, bool publish, string performedBy = "Admin", CancellationToken ct = default)
    {
        var p = await repo.GetDetailsByIdAsync(id, includeDeleted: true, ct);
        if (p is null) return null;

        p.IsPublished = publish;
        repo.Update(p);

        await repo.AddAuditLogAsync(new AuditLog
        {
            EntityName = "Project",
            EntityId = id.ToString(),
            Action = publish ? "Publish" : "Unpublish",
            PerformedBy = performedBy,
            ChangesJson = JsonSerializer.Serialize(new { p.Title, IsPublished = publish }),
            Timestamp = DateTimeOffset.UtcNow
        }, ct);

        await uow.SaveChangesAsync(ct);
        return ToDto(p);
    }

    public async Task<ProjectDto?> ArchiveAsync(int id, bool archive, string performedBy = "Admin", CancellationToken ct = default)
    {
        var p = await repo.GetDetailsByIdAsync(id, includeDeleted: true, ct);
        if (p is null) return null;

        p.Status = archive ? "Archived" : "Completed";
        repo.Update(p);

        await repo.AddAuditLogAsync(new AuditLog
        {
            EntityName = "Project",
            EntityId = id.ToString(),
            Action = archive ? "Archive" : "Unarchive",
            PerformedBy = performedBy,
            ChangesJson = JsonSerializer.Serialize(new { p.Title, p.Status }),
            Timestamp = DateTimeOffset.UtcNow
        }, ct);

        await uow.SaveChangesAsync(ct);
        return ToDto(p);
    }

    public async Task<ProjectDto?> RestoreAsync(int id, string performedBy = "Admin", CancellationToken ct = default)
    {
        var p = await repo.GetDetailsByIdAsync(id, includeDeleted: true, ct);
        if (p is null) return null;

        p.IsDeleted = false;
        repo.Update(p);

        await repo.AddAuditLogAsync(new AuditLog
        {
            EntityName = "Project",
            EntityId = id.ToString(),
            Action = "Restore",
            PerformedBy = performedBy,
            ChangesJson = JsonSerializer.Serialize(new { p.Title }),
            Timestamp = DateTimeOffset.UtcNow
        }, ct);

        await uow.SaveChangesAsync(ct);
        return ToDto(p);
    }

    public async Task<ProjectDto?> ToggleFeaturedAsync(int id, string performedBy = "Admin", CancellationToken ct = default)
    {
        var p = await repo.GetDetailsByIdAsync(id, includeDeleted: true, ct);
        if (p is null) return null;

        p.IsFeatured = !p.IsFeatured;
        repo.Update(p);

        await repo.AddAuditLogAsync(new AuditLog
        {
            EntityName = "Project",
            EntityId = id.ToString(),
            Action = "Feature",
            PerformedBy = performedBy,
            ChangesJson = JsonSerializer.Serialize(new { p.Title, p.IsFeatured }),
            Timestamp = DateTimeOffset.UtcNow
        }, ct);

        await uow.SaveChangesAsync(ct);
        return ToDto(p);
    }

    public async Task<bool> ReorderProjectsAsync(IEnumerable<int> orderedIds, CancellationToken ct = default)
    {
        var orderMap = orderedIds.Select((id, index) => new { id, index }).ToDictionary(x => x.id, x => x.index + 1);
        var projects = await db.Projects.Where(p => orderMap.Keys.Contains(p.Id)).ToListAsync(ct);

        foreach (var p in projects)
        {
            if (orderMap.TryGetValue(p.Id, out var newOrder))
            {
                p.DisplayOrder = newOrder;
            }
        }

        await uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ExecuteBulkActionAsync(BulkActionRequestDto dto, string performedBy = "Admin", CancellationToken ct = default)
    {
        if (dto.ProjectIds == null || dto.ProjectIds.Count == 0) return false;

        foreach (var id in dto.ProjectIds)
        {
            switch (dto.Action.ToLower().Trim())
            {
                case "publish":
                    await PublishAsync(id, true, performedBy, ct);
                    break;
                case "unpublish":
                    await PublishAsync(id, false, performedBy, ct);
                    break;
                case "archive":
                    await ArchiveAsync(id, true, performedBy, ct);
                    break;
                case "restore":
                    await RestoreAsync(id, performedBy, ct);
                    break;
                case "feature":
                    await ToggleFeaturedAsync(id, performedBy, ct);
                    break;
                case "delete":
                    await DeleteAsync(id, permanent: false, performedBy, ct);
                    break;
            }
        }

        return true;
    }

    public async Task<ProjectDashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default)
    {
        var stats = await repo.GetDashboardStatsAsync(ct);
        var recents = await repo.GetPagedAsync(new ProjectListFilterDto { Page = 1, PageSize = 5, SortBy = "Newest" }, ct);
        stats.RecentProjects = recents.Items.Select(ToDto);
        return stats;
    }

    public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(string? entityId = null, CancellationToken ct = default)
    {
        var logs = await repo.GetAuditLogsAsync(entityId, ct);
        return logs.Select(l => new AuditLogDto(l.Id, l.EntityName, l.EntityId, l.Action, l.PerformedBy, l.ChangesJson, l.Timestamp));
    }

    public async Task<(IEnumerable<string> Categories, IEnumerable<string> Technologies, IEnumerable<string> Skills)> GetMetadataOptionsAsync(CancellationToken ct = default)
    {
        var cats = (await repo.GetCategoriesAsync(ct)).Select(c => c.DisplayName);
        var techs = (await repo.GetTechnologiesAsync(ct)).Select(t => t.Name);
        var skills = (await repo.GetSkillsAsync(ct)).Select(s => s.Name);
        return (cats, techs, skills);
    }

    private async Task MapRelationsAsync(Project p, ProjectMutationDto dto, CancellationToken ct)
    {
        // Categories
        if (dto.Categories != null && dto.Categories.Count > 0)
        {
            var dbCats = await db.Categories.ToListAsync(ct);
            foreach (var catName in dto.Categories.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim()))
            {
                var normName = catName.ToLower().Replace(" ", "");
                var existingCat = dbCats.FirstOrDefault(c => c.Name.ToLower() == normName || c.DisplayName.ToLower() == catName.ToLower());
                if (existingCat is null)
                {
                    existingCat = new Category { Name = normName, DisplayName = catName };
                    db.Categories.Add(existingCat);
                    await db.SaveChangesAsync(ct);
                    dbCats.Add(existingCat);
                }
                p.ProjectCategories.Add(new ProjectCategoryMapping { Project = p, Category = existingCat });
            }
        }

        // Technologies
        if (dto.Technologies != null && dto.Technologies.Count > 0)
        {
            var dbTechs = await db.Technologies.ToListAsync(ct);
            foreach (var techName in dto.Technologies.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()))
            {
                var existingTech = dbTechs.FirstOrDefault(t => t.Name.ToLower() == techName.ToLower());
                if (existingTech is null)
                {
                    existingTech = new Technology { Name = techName };
                    db.Technologies.Add(existingTech);
                    await db.SaveChangesAsync(ct);
                    dbTechs.Add(existingTech);
                }
                p.ProjectTechnologies.Add(new ProjectTechnology { Project = p, Technology = existingTech });
            }
        }

        // Skills
        if (dto.Skills != null && dto.Skills.Count > 0)
        {
            var dbSkills = await db.Skills.ToListAsync(ct);
            foreach (var skillName in dto.Skills.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
            {
                var existingSkill = dbSkills.FirstOrDefault(s => s.Name.ToLower() == skillName.ToLower());
                if (existingSkill != null)
                {
                    p.ProjectSkills.Add(new ProjectSkill { Project = p, Skill = existingSkill });
                }
            }
        }

        // Images
        if (dto.Images != null)
        {
            foreach (var img in dto.Images)
            {
                p.Images.Add(new ProjectImage
                {
                    Project = p,
                    StoragePath = img.StoragePath ?? string.Empty,
                    PublicUrl = img.PublicUrl ?? string.Empty,
                    AltText = img.AltText,
                    IsThumbnail = img.IsThumbnail,
                    DisplayOrder = img.DisplayOrder,
                    Width = img.Width,
                    Height = img.Height
                });
            }
        }

        // Links
        if (dto.Links != null)
        {
            foreach (var link in dto.Links.Where(l => !string.IsNullOrWhiteSpace(l.Url)))
            {
                p.Links.Add(new ProjectLink
                {
                    Project = p,
                    LinkType = string.IsNullOrWhiteSpace(link.LinkType) ? "Live" : link.LinkType.Trim(),
                    Url = link.Url.Trim(),
                    Label = link.Label
                });
            }
        }

        // Features
        if (dto.Features != null)
        {
            foreach (var f in dto.Features.Where(f => !string.IsNullOrWhiteSpace(f.Title)))
            {
                p.Features.Add(new ProjectFeature
                {
                    Project = p,
                    Title = f.Title.Trim(),
                    Description = f.Description,
                    IconClass = f.IconClass,
                    DisplayOrder = f.DisplayOrder
                });
            }
        }

        // Achievements
        if (dto.Achievements != null)
        {
            foreach (var a in dto.Achievements.Where(a => !string.IsNullOrWhiteSpace(a.Title)))
            {
                p.Achievements.Add(new ProjectAchievement
                {
                    Project = p,
                    Title = a.Title.Trim(),
                    Description = a.Description,
                    DateAchieved = a.DateAchieved,
                    DisplayOrder = a.DisplayOrder
                });
            }
        }
    }

    private static ProjectDto ToDto(Project p)
    {
        var cats = p.ProjectCategories.SelectMany(pc => new[] { pc.Category.Name, pc.Category.DisplayName }).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
        var techs = p.ProjectTechnologies.Select(pt => pt.Technology.Name).Distinct().ToList();
        var skills = p.ProjectSkills.Select(ps => ps.Skill.Name).Distinct().ToList();

        var images = p.Images.Select(i => new ProjectImageDto(i.Id, i.StoragePath, i.PublicUrl, i.AltText, i.IsThumbnail, i.DisplayOrder, i.Width, i.Height)).ToList();
        var links = p.Links.Select(l => new ProjectLinkDto(l.Id, l.LinkType, l.Url, l.Label)).ToList();
        var features = p.Features.Select(f => new ProjectFeatureDto(f.Id, f.Title, f.Description, f.IconClass, f.DisplayOrder)).ToList();
        var achievements = p.Achievements.Select(a => new ProjectAchievementDto(a.Id, a.Title, a.Description, a.DateAchieved, a.DisplayOrder)).ToList();

        var durationText = CalculateDuration(p.StartDate, p.EndDate, p.IsCurrentlyWorking);

        return new ProjectDto(
            p.Id,
            p.Slug,
            p.Title,
            p.ShortDescription,
            p.FullDescription,
            p.Status,
            p.Visibility,
            p.IsPublished,
            p.IsFeatured,
            p.IsDeleted,
            p.ResumeCategory,
            p.ExperienceType,
            p.StartDate,
            p.EndDate,
            p.IsCurrentlyWorking,
            durationText,
            p.ReadmeMarkdown,
            p.MetaTitle,
            p.MetaDescription,
            p.MetaKeywords,
            p.OgImageUrl,
            p.DisplayOrder,
            p.ThumbnailUrl,
            p.CreatedAt,
            p.UpdatedAt,
            cats,
            techs,
            skills,
            images,
            links,
            features,
            achievements
        );
    }

    private static string CalculateDuration(DateTimeOffset? start, DateTimeOffset? end, bool currentlyWorking)
    {
        if (!start.HasValue) return "N/A";
        var endDate = currentlyWorking ? DateTimeOffset.UtcNow : (end ?? DateTimeOffset.UtcNow);

        var totalDays = (endDate - start.Value).TotalDays;
        if (totalDays < 30) return "< 1 Month";

        var months = (int)Math.Round(totalDays / 30.4375);
        if (months < 12) return $"{months} Month{(months > 1 ? "s" : "")}";

        var years = months / 12;
        var remMonths = months % 12;
        if (remMonths == 0) return $"{years} Year{(years > 1 ? "s" : "")}";
        return $"{years} Year{(years > 1 ? "s" : "")} {remMonths} Mo{(remMonths > 1 ? "s" : "")}";
    }

    private async Task<string> GenerateSlugAsync(string title, int? existingId, CancellationToken ct)
    {
        var baseSlug = ToSlug(title);
        if (string.IsNullOrWhiteSpace(baseSlug)) baseSlug = "project";
        var slug = baseSlug;
        var suffix = 2;

        while (await repo.SlugExistsAsync(slug, existingId, ct))
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
            else if ((c == ' ' || c == '-' || c == '_') && !lastWasDash && builder.Length > 0)
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
