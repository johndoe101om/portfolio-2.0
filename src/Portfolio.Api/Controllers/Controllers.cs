using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;

namespace Portfolio.Api.Controllers;

// ── Base controller ───────────────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("general")]
public abstract class ApiControllerBase : ControllerBase
{
    protected bool HasAdminToken(IAdminAuthService auth) =>
        auth.IsTokenValid(Request.Headers.Authorization.ToString());

    protected IActionResult AdminUnauthorized() =>
        Unauthorized(new { detail = "Admin login is required for this operation." });
}

// ── Profile ──────────────────────────────────────────────────────────────────
[Route("api/godmode")]
public class GodmodeController(IAdminAuthService auth) : ApiControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(AdminAuthResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    public IActionResult Login([FromBody] AdminLoginRequest request)
    {
        var response = auth.SignIn(request);
        return response is null ? Unauthorized(new { detail = "Invalid admin credentials." }) : Ok(response);
    }
}

[Route("api/profile")]
public class ProfileController(IProfileService svc) : ApiControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 300)]
    [ProducesResponseType(typeof(ProfileDto), 200)]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await svc.GetProfileAsync(ct));
}

// ── Skills ────────────────────────────────────────────────────────────────────
[Route("api/skills")]
public class SkillsController(ISkillService svc) : ApiControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 300)]
    [ProducesResponseType(typeof(IEnumerable<SkillDto>), 200)]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await svc.GetSkillsAsync(ct));
}

// ── Experiences ───────────────────────────────────────────────────────────────
[Route("api/experiences")]
public class ExperiencesController(ISkillService svc) : ApiControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 300)]
    [ProducesResponseType(typeof(IEnumerable<ExperienceDto>), 200)]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await svc.GetExperiencesAsync(ct));
}

// ── Education ─────────────────────────────────────────────────────────────────
[Route("api/education")]
public class EducationController(ISkillService svc) : ApiControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 300)]
    [ProducesResponseType(typeof(IEnumerable<EducationDto>), 200)]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await svc.GetEducationAsync(ct));
}

// ── Services ──────────────────────────────────────────────────────────────────
[Route("api/services")]
public class ServicesController(ISkillService svc) : ApiControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 300)]
    [ProducesResponseType(typeof(IEnumerable<ServiceDto>), 200)]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await svc.GetServicesAsync(ct));
}

// ── Projects ──────────────────────────────────────────────────────────────────
[Route("api/projects")]
public class ProjectsController(IProjectService svc, IAdminAuthService adminAuth) : ApiControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 300)]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] string? category, CancellationToken ct) =>
        Ok(await svc.GetProjectsAsync(category, ct));

    [HttpGet("{slug}")]
    [ResponseCache(Duration = 300)]
    [ProducesResponseType(typeof(ProjectDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var project = await svc.GetBySlugAsync(slug, ct);
        return project is null ? NotFound(new { detail = $"Project '{slug}' not found." }) : Ok(project);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), 201)]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] ProjectMutationDto dto, CancellationToken ct)
    {
        if (!HasAdminToken(adminAuth))
            return AdminUnauthorized();

        var project = await svc.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetBySlug), new { slug = project.Slug }, project);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProjectDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] ProjectMutationDto dto, CancellationToken ct)
    {
        if (!HasAdminToken(adminAuth))
            return AdminUnauthorized();

        var project = await svc.UpdateAsync(id, dto, ct);
        return project is null ? NotFound(new { detail = $"Project id '{id}' not found." }) : Ok(project);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (!HasAdminToken(adminAuth))
            return AdminUnauthorized();

        var deleted = await svc.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound(new { detail = $"Project id '{id}' not found." });
    }
}

// ── Blog ──────────────────────────────────────────────────────────────────────
[Route("api/blog")]
public class BlogController(IBlogService svc, IAdminAuthService adminAuth) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BlogPostDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeUnpublished = false,
        CancellationToken ct = default)
    {
        if (includeUnpublished && !HasAdminToken(adminAuth))
            return AdminUnauthorized();

        Response.Headers["Cache-Control"] = includeUnpublished ? "no-store" : "public,max-age=300";

        var (posts, totalCount) = await svc.GetPostsAsync(page, pageSize, includeUnpublished, ct);
        Response.Headers["X-Pagination"] = System.Text.Json.JsonSerializer.Serialize(new
        {
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
        });
        return Ok(posts);
    }

    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(BlogPostDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetBySlug(
        string slug,
        [FromQuery] bool includeUnpublished = false,
        CancellationToken ct = default)
    {
        if (includeUnpublished && !HasAdminToken(adminAuth))
            return AdminUnauthorized();

        Response.Headers["Cache-Control"] = includeUnpublished ? "no-store" : "public,max-age=300";

        var post = await svc.GetBySlugAsync(slug, includeUnpublished, ct);
        return post is null ? NotFound(new { detail = $"Post '{slug}' not found." }) : Ok(post);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BlogPostDto), 201)]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] BlogPostMutationDto dto, CancellationToken ct)
    {
        if (!HasAdminToken(adminAuth))
            return AdminUnauthorized();

        var post = await svc.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetBySlug), new { slug = post.Slug }, post);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BlogPostDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] BlogPostMutationDto dto, CancellationToken ct)
    {
        if (!HasAdminToken(adminAuth))
            return AdminUnauthorized();

        var post = await svc.UpdateAsync(id, dto, ct);
        return post is null ? NotFound(new { detail = $"Post id '{id}' not found." }) : Ok(post);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (!HasAdminToken(adminAuth))
            return AdminUnauthorized();

        var deleted = await svc.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound(new { detail = $"Post id '{id}' not found." });
    }
}

// ── Social Links ──────────────────────────────────────────────────────────────
[Route("api/social-links")]
public class SocialLinksController(IProfileService svc) : ApiControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 600)]
    [ProducesResponseType(typeof(IEnumerable<SocialLinkDto>), 200)]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await svc.GetSocialLinksAsync(ct));
}

// ── Site Settings ─────────────────────────────────────────────────────────────
[Route("api/site-settings")]
public class SiteSettingsController(IProfileService svc) : ApiControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 600)]
    [ProducesResponseType(typeof(Dictionary<string, string>), 200)]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await svc.GetSiteSettingsAsync(ct));
}

// ── Contact ───────────────────────────────────────────────────────────────────
[Route("api/contact")]
public class ContactController(IContactService svc, ILogger<ContactController> logger) : ApiControllerBase
{
    [HttpPost]
    [EnableRateLimiting("contact")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(429)]
    public async Task<IActionResult> SendMessage(
        [FromBody] ContactMessageDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await svc.SubmitMessageAsync(dto, ip, ct);

        if (!result.Success)
        {
            logger.LogWarning("Contact form submission failed from {Ip}: {Reason}", ip, result.Message);
            return BadRequest(new { detail = result.Message });
        }

        return Ok(new { success = true, message = "Message sent successfully." });
    }
}
