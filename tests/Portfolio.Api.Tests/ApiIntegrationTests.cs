using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.DTOs;
using Portfolio.Infrastructure.Data;
using Xunit;

namespace Portfolio.Api.Tests;

/// <summary>
/// Shared WebApplicationFactory that swaps PostgreSQL for an in-memory database
/// so integration tests run without a real database connection.
/// </summary>
public class PortfolioWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real DbContext descriptors before swapping to InMemory.
            var dbContextDescriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<PortfolioDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    IsDbContextOptionsConfiguration(d))
                .ToList();

            foreach (var descriptor in dbContextDescriptors)
                services.Remove(descriptor);

            var databaseName = "TestDb_" + Guid.NewGuid();

            // Add in-memory database
            services.AddDbContext<PortfolioDbContext>(opts =>
                opts.UseInMemoryDatabase(databaseName));

            // Seed test data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
            db.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }

    private static bool IsDbContextOptionsConfiguration(ServiceDescriptor descriptor)
    {
        return descriptor.ServiceType.IsGenericType &&
               descriptor.ServiceType.GetGenericTypeDefinition().FullName ==
               "Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsConfiguration`1" &&
               descriptor.ServiceType.GenericTypeArguments[0] == typeof(PortfolioDbContext);
    }
}

// ── Profile tests ─────────────────────────────────────────────────────────────
public class ProfileEndpointTests(PortfolioWebApplicationFactory factory)
    : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetProfile_Returns200_WithSeededData()
    {
        var response = await _client.GetAsync("/api/profile");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var profile = await response.Content.ReadFromJsonAsync<ProfileDto>();
        profile.Should().NotBeNull();
        profile!.FullName.Should().Be("Satyam Kumar");
        profile.Email.Should().NotBeNullOrEmpty();
    }
}

// ── Skills tests ──────────────────────────────────────────────────────────────
public class SkillsEndpointTests(PortfolioWebApplicationFactory factory)
    : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetSkills_Returns200_WithItems()
    {
        var response = await _client.GetAsync("/api/skills");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var skills = await response.Content.ReadFromJsonAsync<List<SkillDto>>();
        skills.Should().NotBeNullOrEmpty();
    }
}

// ── Projects tests ────────────────────────────────────────────────────────────
public class ProjectsEndpointTests(PortfolioWebApplicationFactory factory)
    : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetProjects_Returns200_WithSeededProjects()
    {
        var response = await _client.GetAsync("/api/projects");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var projects = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();
        projects.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProjects_WithCategoryFilter_ReturnsFilteredResults()
    {
        var response = await _client.GetAsync("/api/projects?category=webdesign");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var projects = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();
        projects.Should().NotBeNull();
        projects!.All(p => p.Categories.Contains("webdesign")).Should().BeTrue();
    }

    [Fact]
    public async Task GetProjectBySlug_WithValidSlug_Returns200()
    {
        var response = await _client.GetAsync("/api/projects/tutor-finder");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        project.Should().NotBeNull();
        project!.Slug.Should().Be("tutor-finder");
    }

    [Fact]
    public async Task GetProjectBySlug_WithInvalidSlug_Returns404()
    {
        var response = await _client.GetAsync("/api/projects/nonexistent-project");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

// ── Blog tests ────────────────────────────────────────────────────────────────
public class BlogEndpointTests(PortfolioWebApplicationFactory factory)
    : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetBlogPosts_Returns200_WithPaginationHeaders()
    {
        var response = await _client.GetAsync("/api/blog");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("X-Pagination");
    }

    [Fact]
    public async Task GetBlogPosts_WithPagination_ReturnsCorrectPage()
    {
        var response = await _client.GetAsync("/api/blog?page=1&pageSize=2");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var posts = await response.Content.ReadFromJsonAsync<List<BlogPostDto>>();
        posts.Should().NotBeNull();
        posts!.Count.Should().BeLessOrEqualTo(2);
    }

    [Fact]
    public async Task GetBlogPostBySlug_WithInvalidSlug_Returns404()
    {
        var response = await _client.GetAsync("/api/blog/nonexistent-post");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

// ── Contact tests ─────────────────────────────────────────────────────────────
public class AdminEndpointTests(PortfolioWebApplicationFactory factory)
    : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PostProject_WithoutAdminToken_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/projects", ValidProjectPayload());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithDefaultCredentials_AllowsProjectCreate()
    {
        var login = await _client.PostAsJsonAsync("/api/godmode/login", new AdminLoginRequest
        {
            Email = "johndoeunique101@gmail.com",
            Password = "$Atyam@100.",
        });

        login.StatusCode.Should().Be(HttpStatusCode.OK);
        var auth = await login.Content.ReadFromJsonAsync<AdminAuthResponse>();
        auth.Should().NotBeNull();
        auth!.Token.Should().NotBeNullOrWhiteSpace();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

        var response = await _client.PostAsJsonAsync("/api/projects", ValidProjectPayload());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        project.Should().NotBeNull();
        project!.Title.Should().Be("Admin Created Project");
    }

    private static ProjectMutationDto ValidProjectPayload() => new()
    {
        Title = "Admin Created Project",
        Description = "A project created through the protected admin API.",
        ImageUrl = "/assets/images/placeholder.png",
        Categories = ["webdesign"],
        Technologies = ["React", "ASP.NET Core"],
        DisplayOrder = 99,
    };
}

public class ContactEndpointTests(PortfolioWebApplicationFactory factory)
    : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PostContact_WithValidData_Returns200()
    {
        var payload = new ContactMessageDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Subject = "Test Subject for Integration",
            Message = "This is a test message with enough characters to pass validation.",
        };

        var response = await _client.PostAsJsonAsync("/api/contact", payload);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostContact_WithInvalidEmail_Returns400()
    {
        var payload = new
        {
            name = "Test",
            email = "not-an-email",
            subject = "Subject",
            message = "Message that is long enough to be valid",
        };

        var response = await _client.PostAsJsonAsync("/api/contact", payload);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostContact_WithMissingName_Returns400()
    {
        var payload = new
        {
            name = "",
            email = "test@example.com",
            subject = "Subject",
            message = "Valid message body here",
        };

        var response = await _client.PostAsJsonAsync("/api/contact", payload);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostContact_WithTooShortMessage_Returns400()
    {
        var payload = new ContactMessageDto
        {
            Name = "Test",
            Email = "test@example.com",
            Subject = "Subject",
            Message = "Short",
        };

        var response = await _client.PostAsJsonAsync("/api/contact", payload);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

// ── Health check test ─────────────────────────────────────────────────────────
public class HealthCheckTests(PortfolioWebApplicationFactory factory)
    : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task HealthCheck_Returns200()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
