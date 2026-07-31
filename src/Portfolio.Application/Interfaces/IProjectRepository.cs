using Portfolio.Application.DTOs;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetBySlugAsync(string slug, bool includeDeleted = false, CancellationToken ct = default);
    Task<Project?> GetDetailsByIdAsync(int id, bool includeDeleted = false, CancellationToken ct = default);
    Task<PagedResultDto<Project>> GetPagedAsync(ProjectListFilterDto filter, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null, CancellationToken ct = default);
    Task<int> GetMaxDisplayOrderAsync(CancellationToken ct = default);
    Task<ProjectDashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default);
    Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string? entityId = null, CancellationToken ct = default);
    Task AddAuditLogAsync(AuditLog log, CancellationToken ct = default);
    Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken ct = default);
    Task<IEnumerable<Technology>> GetTechnologiesAsync(CancellationToken ct = default);
    Task<IEnumerable<Skill>> GetSkillsAsync(CancellationToken ct = default);
}
