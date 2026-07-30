using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;
using System.Text.Json;

namespace Portfolio.Infrastructure.Data;

public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options) { }

    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<SocialLink> SocialLinks => Set<SocialLink>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Statistic> Statistics => Set<Statistic>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTechnology> ProjectTechnologies => Set<ProjectTechnology>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // Profile
        mb.Entity<Profile>(e =>
        {
            e.HasIndex(p => p.Email).IsUnique();
        });

        // SocialLink
        mb.Entity<SocialLink>(e =>
        {
            e.HasIndex(s => s.Platform).IsUnique();
            e.Property(s => s.DisplayOrder).HasDefaultValue(0);
        });

        // Skill
        mb.Entity<Skill>(e =>
        {
            e.HasIndex(s => new { s.Name, s.Category }).IsUnique();
        });

        // Project
        mb.Entity<Project>(e =>
        {
            e.HasIndex(p => p.Slug).IsUnique();
            e.HasMany(p => p.Technologies)
             .WithOne(t => t.Project)
             .HasForeignKey(t => t.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // BlogPost
        mb.Entity<BlogPost>(e =>
        {
            e.HasIndex(b => b.Slug).IsUnique();
            e.HasIndex(b => b.PublishedAt);
        });

        // ContactMessage
        mb.Entity<ContactMessage>(e =>
        {
            e.HasIndex(c => c.CreatedAt);
            e.Property(c => c.IpAddress).HasMaxLength(45);
        });

        // SiteSetting
        mb.Entity<SiteSetting>(e =>
        {
            e.HasIndex(s => s.Key).IsUnique();
        });

        // Auto-update timestamps on save
        foreach (var entityType in mb.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                mb.Entity(entityType.ClrType)
                    .Property<DateTimeOffset>("CreatedAt")
                    .HasDefaultValueSql("NOW()");
            }
        }

        SeedData(mb);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }

    private static void SeedData(ModelBuilder mb)
    {
        // Profile
        mb.Entity<Profile>().HasData(new Profile
        {
            Id = 1,
            FullName = "Satyam Kumar",
            Title = "Web Developer",
            Subtitle = "App Developer",
            AboutText = "Spirited software engineer with a love for clean code and problem-solving. Always exploring new technologies and methodologies to enhance development efficiency. Driven by a desire to create robust, scalable, and user-friendly software solutions.",
            Phone = "+91 9113394936",
            Email = "sirsatyamchaudhary@gmail.com",
            Website = "www.codersatyam.com",
            City = "Chennai",
            Country = "INDIA",
            Age = 20,
            Degree = "Bachelor of Engineering",
            FreelanceStatus = "Available",
            ProfileImageUrl = "/assets/images/profile.jpg",
            CvUrl = "https://drive.google.com/file/d/1P28ffSgcD7xEWpu02UgWMAV1b3kp_fyJ/view?usp=sharing",
            MapLat = 43.053454,
            MapLng = -76.144508,
            CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
        });

        // Social Links
        mb.Entity<SocialLink>().HasData(
            new SocialLink { Id = 1, Platform = "WhatsApp",  Url = "https://wa.me/qr/TZU5O77ZT4MGN1",                 IconClass = "bi bi-whatsapp",  DisplayOrder = 1, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new SocialLink { Id = 2, Platform = "Instagram", Url = "https://www.instagram.com/be_stranger7964/",       IconClass = "bi bi-instagram", DisplayOrder = 2, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new SocialLink { Id = 3, Platform = "LinkedIn",  Url = "https://www.linkedin.com/in/satyam-webdeveloper/", IconClass = "bi bi-linkedin",  DisplayOrder = 3, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) }
        );

        // Statistics
        mb.Entity<Statistic>().HasData(
            new Statistic { Id = 1, IconClass = "bi bi-palette",       Value = 2,  Label = "DevOps Projects", DisplayOrder = 1, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Statistic { Id = 2, IconClass = "bi bi-laptop",        Value = 12, Label = "Web Designs",     DisplayOrder = 2, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Statistic { Id = 3, IconClass = "bi bi-award",         Value = 26, Label = "Web Development",  DisplayOrder = 3, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Statistic { Id = 4, IconClass = "bi bi-journal-check", Value = 40, Label = "Projects Done",   DisplayOrder = 4, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) }
        );

        // Skills
        mb.Entity<Skill>().HasData(
            new Skill { Id = 1, Name = "Web Design",    Percentage = 75, Category = "technical", DisplayOrder = 1, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Skill { Id = 2, Name = "Web Developer", Percentage = 90, Category = "technical", DisplayOrder = 2, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Skill { Id = 3, Name = "Cloud",         Percentage = 85, Category = "technical", DisplayOrder = 3, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Skill { Id = 4, Name = "Hindi",   Category = "language", Percentage = 95, LanguageLevel = "Expert",       FilledDots = 9,  TotalDots = 10, DisplayOrder = 1, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Skill { Id = 5, Name = "English", Category = "language", Percentage = 80, LanguageLevel = "Intermediate",  FilledDots = 8,  TotalDots = 10, DisplayOrder = 2, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) }
        );

        // Projects (categories stored as JSON)
        mb.Entity<Project>().HasData(
            new Project { Id = 1, Slug = "tutor-finder",       Title = "Tutor Finder",          Description = "A platform connecting students with tutors based on subject, location, and availability.", ImageUrl = "/assets/images/project-tutor-finder.png",      CategoriesJson = "[\"webdesign\",\"webapp\"]",            DisplayOrder = 1, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Project { Id = 2, Slug = "college-lake",       Title = "CollegeLake",           Description = "A mobile-friendly college discovery and comparison application.",                        ImageUrl = "/assets/images/project-college-lake.png",       CategoriesJson = "[\"mobiledesign\",\"webapp\"]",         DisplayOrder = 2, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Project { Id = 3, Slug = "online-signature",   Title = "Online Signature",      Description = "A web application allowing users to create and save digital signatures.",                ImageUrl = "/assets/images/project-online-signature.png",   CategoriesJson = "[\"webdesign\",\"webapp\"]",            DisplayOrder = 3, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Project { Id = 4, Slug = "skill-navigator",    Title = "Skill Navigator App",   Description = "An application that helps users assess and plan their technology skill development.",    ImageUrl = "/assets/images/project-skill-navigator.png",    CategoriesJson = "[\"webdesign\"]",                      DisplayOrder = 4, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Project { Id = 5, Slug = "raja-mantri",        Title = "Raja Mantri Chor Sipahi", Description = "Digital version of the classic Indian card game with online multiplayer.",            ImageUrl = "/assets/images/project-game.png",                CategoriesJson = "[\"gamedesign\",\"webapp\"]",           DisplayOrder = 5, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Project { Id = 6, Slug = "detailed-portfolio", Title = "Detailed Portfolio",    Description = "A mobile-first personal portfolio with animated transitions.",                           ImageUrl = "/assets/images/project-portfolio.png",          CategoriesJson = "[\"mobiledesign\"]",                   DisplayOrder = 6, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) }
        );

        // Project technologies
        mb.Entity<ProjectTechnology>().HasData(
            new ProjectTechnology { Id = 1, ProjectId = 1, Name = "React", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 2, ProjectId = 1, Name = "Node.js", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 3, ProjectId = 1, Name = "MongoDB", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 4, ProjectId = 2, Name = "React Native", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 5, ProjectId = 2, Name = "Firebase", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 6, ProjectId = 3, Name = "JavaScript", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 7, ProjectId = 3, Name = "Canvas API", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 8, ProjectId = 3, Name = "PHP", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 9, ProjectId = 4, Name = "React", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 10, ProjectId = 4, Name = "TailwindCSS", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 11, ProjectId = 5, Name = "Unity", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 12, ProjectId = 5, Name = "C#", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 13, ProjectId = 5, Name = "WebGL", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 14, ProjectId = 6, Name = "React", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 15, ProjectId = 6, Name = "SCSS", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new ProjectTechnology { Id = 16, ProjectId = 6, Name = "Framer Motion", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) }
        );

        // Services
        mb.Entity<Service>().HasData(
            new Service { Id = 1, Title = "Web Design", IconClass = "bi bi-laptop", Description = "Modern, visually engaging, and user-centered web designs for seamless experiences across devices.", DisplayOrder = 1, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Service { Id = 2, Title = "DevOps Engineer", IconClass = "fa-solid fa-infinity", Description = "Automation, continuous integration, deployment workflows, and scalable infrastructure practices.", DisplayOrder = 2, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Service { Id = 3, Title = "Web Development", IconClass = "bi bi-award", Description = "Robust, responsive, and dynamic web applications using modern frontend and backend technologies.", DisplayOrder = 3, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Service { Id = 4, Title = "Data Visualization", IconClass = "fa-solid fa-database", Description = "Clear, engaging, and interactive visualizations that turn data into useful insight.", DisplayOrder = 4, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Service { Id = 5, Title = "Generative AI", IconClass = "fa-solid fa-wand-magic-sparkles", Description = "AI-driven solutions for content generation, automation, and intelligent user experiences.", DisplayOrder = 5, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Service { Id = 6, Title = "Game Development", IconClass = "bi bi-controller", Description = "Immersive game experiences combining technical implementation with design and storytelling.", DisplayOrder = 6, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) }
        );

        // Education
        mb.Entity<Education>().HasData(
            new Education { Id = 1, Institution = "Dr Sarvapalli Radhakrishnan Shiksha Samrat, Simrahi", Period = "2009 - 2017", Description = "Schooling years that built a foundation in academics, teamwork, leadership, and problem-solving.", DisplayOrder = 1, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Education { Id = 2, Institution = "Sanskar Bharti Global School, Phulparas", Period = "2017 - 2019", Description = "Focused on strengthening academic skills in Mathematics and Science while building discipline and work ethic.", DisplayOrder = 2, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Education { Id = 3, Institution = "B.S.S College, Supaul", Period = "2019 - 2021", Description = "Studied Physics, Chemistry, and Mathematics while developing analytical skills and interest in technology.", DisplayOrder = 3, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Education { Id = 4, Institution = "B.E in Computer Science and Engineering - Chennai", Period = "2021 - 2025", Description = "Studied software development, data science, and technology through coursework, projects, hackathons, and collaboration.", DisplayOrder = 4, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) }
        );

        // Experience and soft skills
        mb.Entity<Experience>().HasData(
            new Experience { Id = 1, Title = "Team Leader", Category = "softskill", Description = "Led project teams by supporting collaboration, creative problem-solving, and delivery against shared goals.", DisplayOrder = 1, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Experience { Id = 2, Title = "Business Development", Category = "softskill", Description = "Identifies growth opportunities through market analysis, lead generation, and relationship building.", DisplayOrder = 2, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Experience { Id = 3, Title = "Adaptability", Category = "softskill", Description = "Adapts quickly to changing circumstances, assesses problems, and pivots strategies effectively.", DisplayOrder = 3, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Experience { Id = 4, Title = "Work Ethic", Category = "softskill", Description = "Committed to integrity, accountability, diligence, and consistent execution across projects.", DisplayOrder = 4, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) }
        );

        // Testimonials
        mb.Entity<Testimonial>().HasData(
            new Testimonial { Id = 1, Quote = "Design is not just what it looks like and feels like. Design is how it works.", AuthorName = "Steve Jobs", AuthorTitle = "Designer", AuthorImageUrl = "/assets/images/testimonial-steve.png", DisplayOrder = 1, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Testimonial { Id = 2, Quote = "Any fool can write code that a computer can understand. Good programmers write code that humans can understand.", AuthorName = "Martin Fowler", AuthorTitle = "Developer", AuthorImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTtT1e-oQ6PQHr72kZzulDQlAqp0pxVEqo-sg&s", DisplayOrder = 2, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new Testimonial { Id = 3, Quote = "Good design is obvious. Great design is transparent!", AuthorName = "Joe Sparano", AuthorTitle = "Web Designer", AuthorImageUrl = "/assets/images/testimonial-joe.jpg", DisplayOrder = 3, CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) }
        );

        // Site settings
        mb.Entity<SiteSetting>().HasData(
            new SiteSetting { Id = 1, Key = "RotatingRoles", Value = "[\"App Developer\",\"Web Developer\",\"DevOps Engineer\",\"Cloud Engineer\"]", Description = "Hero rotating role labels.", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) },
            new SiteSetting { Id = 2, Key = "KnowledgeAreas", Value = "[\"Machine Learning\",\"Data Science\",\"Software Development\",\"Teaching Web Design\"]", Description = "About-section knowledge areas.", CreatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,1,1,0,0,0,TimeSpan.Zero) }
        );

        // Blog Posts
        mb.Entity<BlogPost>().HasData(
            new BlogPost { Id = 1, Slug = "best-way-to-become-good-web-designer",        Title = "The best way to become a good web designer",                  Excerpt = "Web design is not just about making things look pretty.", ImageUrl = "/assets/images/blog-web-designer.png", PublishedAt = new DateTimeOffset(2024,6,20,0,0,0,TimeSpan.Zero), Author = "Satyam Kumar", TagsJson = "[\"Web Design\",\"Career\"]",             Content = "", IsPublished = true, CreatedAt = new DateTimeOffset(2024,6,20,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,6,20,0,0,0,TimeSpan.Zero) },
            new BlogPost { Id = 2, Slug = "enhancing-coding-logic",                      Title = "Enhancing Coding Logic: Practices to Sharpen Your Skills",   Excerpt = "Coding has become an essential skill across various fields.",  ImageUrl = "/assets/images/blog-coding-logic.png",  PublishedAt = new DateTimeOffset(2024,7,18,0,0,0,TimeSpan.Zero), Author = "Satyam Kumar", TagsJson = "[\"Programming\",\"Best Practices\"]",    Content = "", IsPublished = true, CreatedAt = new DateTimeOffset(2024,7,18,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,7,18,0,0,0,TimeSpan.Zero) },
            new BlogPost { Id = 3, Slug = "practices-for-personal-and-professional-growth", Title = "Practices for Personal and Professional Growth",          Excerpt = "Technical skills alone aren't enough to ensure success.",     ImageUrl = "/assets/images/blog-growth.png",        PublishedAt = new DateTimeOffset(2024,9,12,0,0,0,TimeSpan.Zero), Author = "Satyam Kumar", TagsJson = "[\"Soft Skills\",\"Career Growth\"]",     Content = "", IsPublished = true, CreatedAt = new DateTimeOffset(2024,9,12,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,9,12,0,0,0,TimeSpan.Zero) },
            new BlogPost { Id = 4, Slug = "how-to-crack-any-technical-interview",        Title = "How to Crack Any Technical Interview in the IT Sector",      Excerpt = "With proper preparation you can navigate any interview.",     ImageUrl = "/assets/images/blog-interview.png",     PublishedAt = new DateTimeOffset(2024,9,28,0,0,0,TimeSpan.Zero), Author = "Satyam Kumar", TagsJson = "[\"Interview\",\"Career\"]",              Content = "", IsPublished = true, CreatedAt = new DateTimeOffset(2024,9,28,0,0,0,TimeSpan.Zero), UpdatedAt = new DateTimeOffset(2024,9,28,0,0,0,TimeSpan.Zero) }
        );
    }
}
