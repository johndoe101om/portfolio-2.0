using Microsoft.EntityFrameworkCore;
using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories;

public class ProjectRepository(PortfolioDbContext dbContext) : Repository<Project>(dbContext), IProjectRepository
{
    private IQueryable<Project> GetFullyIncludedQuery(bool includeDeleted = false)
    {
        var query = DbContext.Projects.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(p => !p.IsDeleted);
        }

        return query
            .Include(p => p.Images)
            .Include(p => p.ProjectTechnologies).ThenInclude(pt => pt.Technology)
            .Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category)
            .Include(p => p.ProjectSkills).ThenInclude(ps => ps.Skill)
            .Include(p => p.Links)
            .Include(p => p.Features)
            .Include(p => p.Achievements);
    }

    public async Task<Project?> GetBySlugAsync(string slug, bool includeDeleted = false, CancellationToken ct = default)
    {
        return await GetFullyIncludedQuery(includeDeleted)
            .FirstOrDefaultAsync(p => p.Slug.ToLower() == slug.ToLower(), ct);
    }

    public async Task<Project?> GetDetailsByIdAsync(int id, bool includeDeleted = false, CancellationToken ct = default)
    {
        return await GetFullyIncludedQuery(includeDeleted)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<PagedResultDto<Project>> GetPagedAsync(ProjectListFilterDto filter, CancellationToken ct = default)
    {
        var query = DbContext.Projects.AsQueryable();

        if (!filter.IncludeDeleted)
        {
            query = query.Where(p => !p.IsDeleted);
        }

        if (!filter.IncludeUnpublished)
        {
            query = query.Where(p => p.IsPublished);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(search) ||
                p.ShortDescription.ToLower().Contains(search) ||
                p.FullDescription.ToLower().Contains(search) ||
                p.Slug.ToLower().Contains(search) ||
                p.ReadmeMarkdown.ToLower().Contains(search) ||
                p.ProjectTechnologies.Any(pt => pt.Technology.Name.ToLower().Contains(search)) ||
                p.ProjectCategories.Any(pc => pc.Category.Name.ToLower().Contains(search) || pc.Category.DisplayName.ToLower().Contains(search)) ||
                p.ProjectSkills.Any(ps => ps.Skill.Name.ToLower().Contains(search)) ||
                p.Links.Any(l => l.Url.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Category) && filter.Category != "*")
        {
            var cat = filter.Category.Trim().ToLower();
            query = query.Where(p => p.ProjectCategories.Any(pc => pc.Category.Name.ToLower() == cat || pc.Category.DisplayName.ToLower() == cat));
        }

        if (!string.IsNullOrWhiteSpace(filter.Technology))
        {
            var tech = filter.Technology.Trim().ToLower();
            query = query.Where(p => p.ProjectTechnologies.Any(pt => pt.Technology.Name.ToLower() == tech));
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            var st = filter.Status.Trim().ToLower();
            query = query.Where(p => p.Status.ToLower() == st);
        }

        if (!string.IsNullOrWhiteSpace(filter.ExperienceType))
        {
            var exp = filter.ExperienceType.Trim().ToLower();
            query = query.Where(p => p.ExperienceType.ToLower() == exp);
        }

        if (!string.IsNullOrWhiteSpace(filter.ResumeCategory))
        {
            var res = filter.ResumeCategory.Trim().ToLower();
            query = query.Where(p => p.ResumeCategory.ToLower() == res);
        }

        if (filter.Featured.HasValue)
        {
            query = query.Where(p => p.IsFeatured == filter.Featured.Value);
        }

        if (filter.Year.HasValue)
        {
            query = query.Where(p => (p.StartDate.HasValue && p.StartDate.Value.Year == filter.Year.Value) || p.CreatedAt.Year == filter.Year.Value);
        }

        query = filter.SortBy switch
        {
            "Oldest" => query.OrderBy(p => p.CreatedAt),
            "Alphabetical" => query.OrderBy(p => p.Title),
            "Updated" => query.OrderByDescending(p => p.UpdatedAt),
            "Featured" => query.OrderByDescending(p => p.IsFeatured).ThenBy(p => p.DisplayOrder),
            "Manual" => query.OrderBy(p => p.DisplayOrder),
            _ => query.OrderByDescending(p => p.CreatedAt) // Newest default
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Include(p => p.Images)
            .Include(p => p.ProjectTechnologies).ThenInclude(pt => pt.Technology)
            .Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category)
            .Include(p => p.ProjectSkills).ThenInclude(ps => ps.Skill)
            .Include(p => p.Links)
            .Include(p => p.Features)
            .Include(p => p.Achievements)
            .ToListAsync(ct);

        return new PagedResultDto<Project>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null, CancellationToken ct = default)
    {
        var query = DbContext.Projects.Where(p => p.Slug.ToLower() == slug.ToLower());
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        return await query.AnyAsync(ct);
    }

    public async Task<int> GetMaxDisplayOrderAsync(CancellationToken ct = default)
    {
        if (!await DbContext.Projects.AnyAsync(ct)) return 0;
        return await DbContext.Projects.MaxAsync(p => p.DisplayOrder, ct);
    }

    public async Task<ProjectDashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default)
    {
        var total = await DbContext.Projects.CountAsync(p => !p.IsDeleted, ct);
        var published = await DbContext.Projects.CountAsync(p => !p.IsDeleted && p.IsPublished, ct);
        var draft = await DbContext.Projects.CountAsync(p => !p.IsDeleted && !p.IsPublished, ct);
        var featured = await DbContext.Projects.CountAsync(p => !p.IsDeleted && p.IsFeatured, ct);
        var archived = await DbContext.Projects.CountAsync(p => !p.IsDeleted && p.Status == "Archived", ct);
        var totalTechs = await DbContext.Technologies.CountAsync(ct);
        var totalCats = await DbContext.Categories.CountAsync(ct);

        return new ProjectDashboardStatsDto
        {
            TotalProjects = total,
            PublishedProjects = published,
            DraftProjects = draft,
            FeaturedProjects = featured,
            ArchivedProjects = archived,
            TotalTechnologies = totalTechs,
            TotalCategories = totalCats
        };
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string? entityId = null, CancellationToken ct = default)
    {
        var query = DbContext.AuditLogs.AsNoTracking().Where(a => a.EntityName == "Project");
        if (!string.IsNullOrWhiteSpace(entityId))
        {
            query = query.Where(a => a.EntityId == entityId);
        }
        return await query.OrderByDescending(a => a.Timestamp).Take(50).ToListAsync(ct);
    }

    public async Task AddAuditLogAsync(AuditLog log, CancellationToken ct = default)
    {
        await DbContext.AuditLogs.AddAsync(log, ct);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken ct = default)
    {
        return await DbContext.Categories.AsNoTracking().OrderBy(c => c.DisplayName).ToListAsync(ct);
    }

    public async Task<IEnumerable<Technology>> GetTechnologiesAsync(CancellationToken ct = default)
    {
        return await DbContext.Technologies.AsNoTracking().OrderBy(t => t.Name).ToListAsync(ct);
    }

    public async Task<IEnumerable<Skill>> GetSkillsAsync(CancellationToken ct = default)
    {
        return await DbContext.Skills.AsNoTracking().OrderBy(s => s.Name).ToListAsync(ct);
    }
}
