using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Application.DTOs;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Services;
using Portfolio.Infrastructure.Repositories;
using Xunit;

namespace Portfolio.Application.Tests;

/// <summary>Creates a fresh in-memory DbContext per test.</summary>
public abstract class ServiceTestBase : IDisposable
{
    protected readonly PortfolioDbContext Db;

    protected ServiceTestBase()
    {
        var opts = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        Db = new PortfolioDbContext(opts);
        Db.Database.EnsureCreated(); // applies seed data
    }

    public void Dispose() => Db.Dispose();
}

// ── ProfileService tests ──────────────────────────────────────────────────────
public class ProfileServiceTests : ServiceTestBase
{
    [Fact]
    public async Task GetProfileAsync_ReturnsSeededProfile()
    {
        var svc = new ProfileService(Db);
        var profile = await svc.GetProfileAsync();

        profile.Should().NotBeNull();
        profile!.FullName.Should().Be("Satyam Kumar");
        profile.Email.Should().Be("sirsatyamchaudhary@gmail.com");
        profile.City.Should().Be("Chennai");
    }

    [Fact]
    public async Task GetSocialLinksAsync_ReturnsOrderedLinks()
    {
        var svc = new ProfileService(Db);
        var links = (await svc.GetSocialLinksAsync()).ToList();

        links.Should().HaveCount(3);
        links.Select(l => l.DisplayOrder).Should().BeInAscendingOrder();
        links.First().Platform.Should().Be("WhatsApp");
    }

    [Fact]
    public async Task GetSiteSettingsAsync_ReturnsEmptyDictionaryWhenNoSettings()
    {
        var svc = new ProfileService(Db);
        var settings = await svc.GetSiteSettingsAsync();
        settings.Should().NotBeNull();
    }
}

// ── SkillService tests ────────────────────────────────────────────────────────
public class SkillServiceTests : ServiceTestBase
{
    [Fact]
    public async Task GetSkillsAsync_ReturnsAllSeededSkills()
    {
        var svc = new SkillService(Db);
        var skills = (await svc.GetSkillsAsync()).ToList();

        skills.Should().HaveCount(5);
        skills.Should().Contain(s => s.Category == "technical");
        skills.Should().Contain(s => s.Category == "language");
    }

    [Fact]
    public async Task GetSkillsAsync_TechnicalSkillsHaveCorrectPercentage()
    {
        var svc = new SkillService(Db);
        var skills = (await svc.GetSkillsAsync()).ToList();

        var webDev = skills.FirstOrDefault(s => s.Name == "Web Developer");
        webDev.Should().NotBeNull();
        webDev!.Percentage.Should().Be(90);
    }

    [Fact]
    public async Task GetEducationAsync_ReturnsFourEntries()
    {
        var svc = new SkillService(Db);
        var education = (await svc.GetEducationAsync()).ToList();
        education.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetServicesAsync_ReturnsSixServices()
    {
        // Services are not seeded in EF seed but added below for this test
        Db.Services.Add(new Service { Title = "Test Service", Description = "Desc", IconClass = "bi bi-test", DisplayOrder = 1 });
        await Db.SaveChangesAsync();

        var svc = new SkillService(Db);
        var services = (await svc.GetServicesAsync()).ToList();
        services.Should().NotBeEmpty();
    }
}

// ── ProjectService tests ──────────────────────────────────────────────────────
public class ProjectServiceTests : ServiceTestBase
{
    private ProjectService CreateService()
    {
        var repo = new ProjectRepository(Db);
        var uow = new UnitOfWork(Db);
        return new ProjectService(repo, uow, Db);
    }

    [Fact]
    public async Task GetProjectsAsync_ReturnsAllSeededProjects()
    {
        var svc = CreateService();
        var projects = (await svc.GetProjectsAsync()).ToList();
        projects.Should().HaveCount(6);
    }

    [Fact]
    public async Task GetProjectsAsync_WithCategory_FiltersCorrectly()
    {
        var svc = CreateService();
        var webProjects = (await svc.GetProjectsAsync("webdesign")).ToList();

        webProjects.Should().NotBeEmpty();
        webProjects.All(p => p.Categories.Contains("Web Design")).Should().BeTrue();
    }

    [Fact]
    public async Task GetBySlugAsync_WithValidSlug_ReturnsProject()
    {
        var svc = CreateService();
        var project = await svc.GetBySlugAsync("tutor-finder");

        project.Should().NotBeNull();
        project!.Title.Should().Be("Tutor Finder");
        project.Slug.Should().Be("tutor-finder");
    }

    [Fact]
    public async Task GetBySlugAsync_WithInvalidSlug_ReturnsNull()
    {
        var svc = CreateService();
        var project = await svc.GetBySlugAsync("does-not-exist");
        project.Should().BeNull();
    }

    [Fact]
    public async Task GetProjectsAsync_WildcardCategory_ReturnsAll()
    {
        var svc = CreateService();
        var all = (await svc.GetProjectsAsync("*")).ToList();
        var none = (await svc.GetProjectsAsync()).ToList();
        all.Count.Should().Be(none.Count);
    }
}

// ── BlogService tests ─────────────────────────────────────────────────────────
public class BlogServiceTests : ServiceTestBase
{
    [Fact]
    public async Task GetPostsAsync_ReturnsFourSeededPosts()
    {
        var svc = new BlogService(Db);
        var (posts, total) = await svc.GetPostsAsync(1, 10);

        posts.Should().HaveCount(4);
        total.Should().Be(4);
    }

    [Fact]
    public async Task GetPostsAsync_PaginationWorks()
    {
        var svc = new BlogService(Db);
        var (page1, total) = await svc.GetPostsAsync(1, 2);
        var (page2, _) = await svc.GetPostsAsync(2, 2);

        page1.Should().HaveCount(2);
        page2.Should().HaveCount(2);
        total.Should().Be(4);
        page1.Select(p => p.Id).Should().NotIntersectWith(page2.Select(p => p.Id));
    }

    [Fact]
    public async Task GetPostsAsync_ReturnedInDescendingDateOrder()
    {
        var svc = new BlogService(Db);
        var (posts, _) = await svc.GetPostsAsync(1, 10);
        var dates = posts.Select(p => p.PublishedAt).ToList();
        dates.Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsCorrectPost()
    {
        var svc = new BlogService(Db);
        var post = await svc.GetBySlugAsync("enhancing-coding-logic");

        post.Should().NotBeNull();
        post!.Author.Should().Be("Satyam Kumar");
        post.Tags.Should().Contain("Programming");
    }

    [Fact]
    public async Task GetBySlugAsync_NonExistentSlug_ReturnsNull()
    {
        var svc = new BlogService(Db);
        var post = await svc.GetBySlugAsync("ghost-post");
        post.Should().BeNull();
    }
}

// ── ContactService tests ──────────────────────────────────────────────────────
public class ContactServiceTests : ServiceTestBase
{
    private static ContactMessageDto ValidDto() => new()
    {
        Name = "Test User",
        Email = "test@example.com",
        Subject = "Test subject for unit test",
        Message = "This is a valid test message with enough characters.",
    };

    [Fact]
    public async Task SubmitMessageAsync_ValidMessage_SavesToDB()
    {
        var emailSvc = new NullEmailService();
        var logger = NullLogger<ContactService>.Instance;
        var svc = new ContactService(Db, emailSvc, logger);

        var result = await svc.SubmitMessageAsync(ValidDto(), "127.0.0.1");

        result.Success.Should().BeTrue();
        Db.ContactMessages.Should().HaveCount(1);
        Db.ContactMessages.First().Name.Should().Be("Test User");
    }

    [Fact]
    public async Task SubmitMessageAsync_DuplicateSubmission_ReturnsFailure()
    {
        var emailSvc = new NullEmailService();
        var logger = NullLogger<ContactService>.Instance;
        var svc = new ContactService(Db, emailSvc, logger);

        var dto = ValidDto();
        await svc.SubmitMessageAsync(dto, "127.0.0.1");
        var result2 = await svc.SubmitMessageAsync(dto, "127.0.0.1");

        result2.Success.Should().BeFalse();
        result2.Message.Should().Contain("Duplicate");
        Db.ContactMessages.Should().HaveCount(1); // second not saved
    }

    [Fact]
    public async Task SubmitMessageAsync_StoresIpAddress()
    {
        var emailSvc = new NullEmailService();
        var logger = NullLogger<ContactService>.Instance;
        var svc = new ContactService(Db, emailSvc, logger);

        await svc.SubmitMessageAsync(ValidDto(), "192.168.1.100");

        Db.ContactMessages.First().IpAddress.Should().Be("192.168.1.100");
    }
}

// ── Helpers ───────────────────────────────────────────────────────────────────
/// <summary>No-op email service for unit tests — never sends real email.</summary>
internal sealed class NullEmailService : Portfolio.Application.Interfaces.IEmailService
{
    public Task<bool> SendContactNotificationAsync(
        string name, string email, string subject, string message,
        CancellationToken ct = default)
        => Task.FromResult(false);
}
