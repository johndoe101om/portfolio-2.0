using Portfolio.Application.DTOs;

namespace Portfolio.Application.Interfaces;

public interface IProfileService
{
    Task<ProfileDto?> GetProfileAsync(CancellationToken ct = default);
    Task<IEnumerable<SocialLinkDto>> GetSocialLinksAsync(CancellationToken ct = default);
    Task<Dictionary<string, string>> GetSiteSettingsAsync(CancellationToken ct = default);
}

public interface ISkillService
{
    Task<IEnumerable<SkillDto>> GetSkillsAsync(CancellationToken ct = default);
    Task<IEnumerable<ExperienceDto>> GetExperiencesAsync(CancellationToken ct = default);
    Task<IEnumerable<EducationDto>> GetEducationAsync(CancellationToken ct = default);
    Task<IEnumerable<ServiceDto>> GetServicesAsync(CancellationToken ct = default);
    Task<IEnumerable<StatisticDto>> GetStatisticsAsync(CancellationToken ct = default);
}

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetProjectsAsync(string? category = null, CancellationToken ct = default);
    Task<ProjectDto?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<ProjectDto> CreateAsync(ProjectMutationDto dto, CancellationToken ct = default);
    Task<ProjectDto?> UpdateAsync(int id, ProjectMutationDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IBlogService
{
    Task<(IEnumerable<BlogPostDto> Posts, int TotalCount)> GetPostsAsync(int page, int pageSize, bool includeUnpublished = false, CancellationToken ct = default);
    Task<BlogPostDto?> GetBySlugAsync(string slug, bool includeUnpublished = false, CancellationToken ct = default);
    Task<BlogPostDto> CreateAsync(BlogPostMutationDto dto, CancellationToken ct = default);
    Task<BlogPostDto?> UpdateAsync(int id, BlogPostMutationDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IAdminAuthService
{
    AdminAuthResponse? SignIn(AdminLoginRequest request);
    bool IsTokenValid(string? authorizationHeader);
}

public interface IContactService
{
    Task<ContactResult> SubmitMessageAsync(ContactMessageDto dto, string? ip, CancellationToken ct = default);
}

public interface IEmailService
{
    Task<bool> SendContactNotificationAsync(string name, string email, string subject, string message, CancellationToken ct = default);
}
