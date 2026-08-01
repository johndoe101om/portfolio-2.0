using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectManagementRelationalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LiveUrl",
                table: "Projects",
                newName: "OgImageUrl");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Projects",
                newName: "Visibility");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Projects",
                newName: "ThumbnailUrl");

            migrationBuilder.RenameColumn(
                name: "CategoriesJson",
                table: "Projects",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "TechnologyId",
                table: "ProjectTechnologies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndDate",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExperienceType",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullDescription",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrentlyWorking",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywords",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReadmeMarkdown",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResumeCategory",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDate",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityName = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    PerformedBy = table.Column<string>(type: "text", nullable: false),
                    ChangesJson = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAchievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DateAchieved = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAchievements_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IconClass = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectFeatures_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    StoragePath = table.Column<string>(type: "text", nullable: false),
                    PublicUrl = table.Column<string>(type: "text", nullable: false),
                    AltText = table.Column<string>(type: "text", nullable: true),
                    IsThumbnail = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectImages_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    LinkType = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLinks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSkills_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Technologies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IconClass = table.Column<string>(type: "text", nullable: true),
                    BadgeColor = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectCategories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "DisplayName", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Web Design", "webdesign", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Web App", "webapp", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mobile", "mobiledesign", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 4, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Game", "gamedesign", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            EnsureProjectSeedRows(migrationBuilder);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 1,
                column: "TechnologyId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 2,
                column: "TechnologyId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 3,
                column: "TechnologyId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 4,
                column: "TechnologyId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 5,
                column: "TechnologyId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 6,
                column: "TechnologyId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 7,
                column: "TechnologyId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 8,
                column: "TechnologyId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 9,
                column: "TechnologyId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 10,
                column: "TechnologyId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 11,
                column: "TechnologyId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 12,
                column: "TechnologyId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 13,
                column: "TechnologyId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 14,
                column: "TechnologyId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 15,
                column: "TechnologyId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 16,
                column: "TechnologyId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "ExperienceType", "FullDescription", "IsCurrentlyWorking", "IsDeleted", "IsFeatured", "IsPublished", "MetaDescription", "MetaKeywords", "MetaTitle", "ReadmeMarkdown", "ResumeCategory", "ShortDescription", "StartDate", "Status", "ThumbnailUrl", "Visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Professional", "Tutor Finder is a high-performance web application designed to bridge the gap between tutors and students.", false, false, true, true, null, null, null, "# Tutor Finder\n\nFull stack platform built with React, Node.js, and MongoDB.", "Web", "A platform connecting students with tutors based on subject, location, and availability.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Completed", "/assets/images/project-tutor-finder.png", "Public" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EndDate", "ExperienceType", "FullDescription", "IsCurrentlyWorking", "IsDeleted", "IsFeatured", "IsPublished", "MetaDescription", "MetaKeywords", "MetaTitle", "ReadmeMarkdown", "ResumeCategory", "ShortDescription", "StartDate", "Status", "ThumbnailUrl", "Visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Personal", "CollegeLake provides comprehensive institute search, degree comparison, and student reviews.", false, false, true, true, null, null, null, "# CollegeLake\n\nMobile-first college research portal.", "Mobile", "A mobile-friendly college discovery and comparison application.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Completed", "/assets/images/project-college-lake.png", "Public" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EndDate", "ExperienceType", "FullDescription", "IsCurrentlyWorking", "IsDeleted", "IsFeatured", "IsPublished", "MetaDescription", "MetaKeywords", "MetaTitle", "ReadmeMarkdown", "ResumeCategory", "ShortDescription", "StartDate", "Status", "ThumbnailUrl", "Visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Client", "Canvas-powered digital signature generator with instant PNG export.", false, false, false, true, null, null, null, "# Online Signature Generator", "Web", "A web application allowing users to create and save digital signatures.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Completed", "/assets/images/project-online-signature.png", "Public" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EndDate", "ExperienceType", "FullDescription", "IsCurrentlyWorking", "IsDeleted", "IsFeatured", "IsPublished", "MetaDescription", "MetaKeywords", "MetaTitle", "ReadmeMarkdown", "ResumeCategory", "ShortDescription", "StartDate", "Status", "ThumbnailUrl", "Visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Personal", "Interactive tech roadmap builder.", false, false, false, true, null, null, null, "# Skill Navigator", "Web", "An application that helps users assess and plan their technology skill development.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Completed", "/assets/images/project-skill-navigator.png", "Public" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EndDate", "ExperienceType", "FullDescription", "IsCurrentlyWorking", "IsDeleted", "IsFeatured", "IsPublished", "MetaDescription", "MetaKeywords", "MetaTitle", "ReadmeMarkdown", "ResumeCategory", "ShortDescription", "StartDate", "Status", "ThumbnailUrl", "Visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Personal", "Unity-powered digital card game.", false, false, true, true, null, null, null, "# Raja Mantri Game", "Game", "Digital version of the classic Indian card game with online multiplayer.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Completed", "/assets/images/project-game.png", "Public" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "EndDate", "ExperienceType", "FullDescription", "IsCurrentlyWorking", "IsDeleted", "IsFeatured", "IsPublished", "MetaDescription", "MetaKeywords", "MetaTitle", "ReadmeMarkdown", "ResumeCategory", "ShortDescription", "StartDate", "Status", "ThumbnailUrl", "Visibility" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Personal", "Responsive interactive portfolio.", false, false, false, true, null, null, null, "# Personal Portfolio", "Web", "A mobile-first personal portfolio with animated transitions.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Completed", "/assets/images/project-portfolio.png", "Public" });

            migrationBuilder.InsertData(
                table: "Technologies",
                columns: new[] { "Id", "BadgeColor", "CreatedAt", "IconClass", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "React", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Node.js", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "MongoDB", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 4, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "React Native", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 5, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Firebase", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 6, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "JavaScript", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 7, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Canvas API", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 8, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "PHP", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 9, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "TailwindCSS", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 10, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Unity", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 11, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "C#", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 12, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "WebGL", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 13, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "SCSS", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 14, null, new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Framer Motion", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            EnsureProjectTechnologyRowsFromNames(migrationBuilder);
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProjectTechnologies");

            EnsureProjectTechnologySeedRows(migrationBuilder);
            EnsureProjectCategorySeedRows(migrationBuilder);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTechnologies_TechnologyId",
                table: "ProjectTechnologies",
                column: "TechnologyId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsDeleted",
                table: "Projects",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsFeatured",
                table: "Projects",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsPublished",
                table: "Projects",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Status",
                table: "Projects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAchievements_ProjectId",
                table: "ProjectAchievements",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCategories_CategoryId",
                table: "ProjectCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCategories_ProjectId",
                table: "ProjectCategories",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFeatures_ProjectId",
                table: "ProjectFeatures",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectImages_ProjectId",
                table: "ProjectImages",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLinks_ProjectId",
                table: "ProjectLinks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_ProjectId",
                table: "ProjectSkills",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_SkillId",
                table: "ProjectSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Technologies_Name",
                table: "Technologies",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTechnologies_Technologies_TechnologyId",
                table: "ProjectTechnologies",
                column: "TechnologyId",
                principalTable: "Technologies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        private static void EnsureProjectSeedRows(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO ""Projects""
    (""Id"", ""Slug"", ""Title"", ""ShortDescription"", ""FullDescription"", ""Status"", ""Visibility"", ""IsPublished"",
     ""IsFeatured"", ""IsDeleted"", ""ResumeCategory"", ""ExperienceType"", ""StartDate"", ""EndDate"",
     ""IsCurrentlyWorking"", ""ReadmeMarkdown"", ""MetaTitle"", ""MetaDescription"", ""MetaKeywords"", ""OgImageUrl"",
     ""DisplayOrder"", ""ThumbnailUrl"", ""CreatedAt"", ""UpdatedAt"")
VALUES
    (1, 'tutor-finder', 'Tutor Finder',
     'A platform connecting students with tutors based on subject, location, and availability.',
     'Tutor Finder is a high-performance web application designed to bridge the gap between tutors and students.',
     'Completed', 'Public', TRUE, TRUE, FALSE, 'Web', 'Professional',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-06-01 00:00:00+00', FALSE,
     E'# Tutor Finder\n\nFull stack platform built with React, Node.js, and MongoDB.',
     NULL, NULL, NULL, NULL, 1, '/assets/images/project-tutor-finder.png',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (2, 'college-lake', 'CollegeLake',
     'A mobile-friendly college discovery and comparison application.',
     'CollegeLake provides comprehensive institute search, degree comparison, and student reviews.',
     'Completed', 'Public', TRUE, TRUE, FALSE, 'Mobile', 'Personal',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-04-01 00:00:00+00', FALSE,
     E'# CollegeLake\n\nMobile-first college research portal.',
     NULL, NULL, NULL, NULL, 2, '/assets/images/project-college-lake.png',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (3, 'online-signature', 'Online Signature',
     'A web application allowing users to create and save digital signatures.',
     'Canvas-powered digital signature generator with instant PNG export.',
     'Completed', 'Public', TRUE, FALSE, FALSE, 'Web', 'Client',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-03-01 00:00:00+00', FALSE,
     '# Online Signature Generator',
     NULL, NULL, NULL, NULL, 3, '/assets/images/project-online-signature.png',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (4, 'skill-navigator', 'Skill Navigator App',
     'An application that helps users assess and plan their technology skill development.',
     'Interactive tech roadmap builder.',
     'Completed', 'Public', TRUE, FALSE, FALSE, 'Web', 'Personal',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-05-01 00:00:00+00', FALSE,
     '# Skill Navigator',
     NULL, NULL, NULL, NULL, 4, '/assets/images/project-skill-navigator.png',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (5, 'raja-mantri', 'Raja Mantri Chor Sipahi',
     'Digital version of the classic Indian card game with online multiplayer.',
     'Unity-powered digital card game.',
     'Completed', 'Public', TRUE, TRUE, FALSE, 'Game', 'Personal',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-07-01 00:00:00+00', FALSE,
     '# Raja Mantri Game',
     NULL, NULL, NULL, NULL, 5, '/assets/images/project-game.png',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (6, 'detailed-portfolio', 'Detailed Portfolio',
     'A mobile-first personal portfolio with animated transitions.',
     'Responsive interactive portfolio.',
     'Completed', 'Public', TRUE, FALSE, FALSE, 'Web', 'Personal',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-03-01 00:00:00+00', FALSE,
     '# Personal Portfolio',
     NULL, NULL, NULL, NULL, 6, '/assets/images/project-portfolio.png',
     TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00')
ON CONFLICT (""Id"") DO UPDATE SET
    ""Slug"" = EXCLUDED.""Slug"",
    ""Title"" = EXCLUDED.""Title"",
    ""ShortDescription"" = EXCLUDED.""ShortDescription"",
    ""FullDescription"" = EXCLUDED.""FullDescription"",
    ""Status"" = EXCLUDED.""Status"",
    ""Visibility"" = EXCLUDED.""Visibility"",
    ""IsPublished"" = EXCLUDED.""IsPublished"",
    ""IsFeatured"" = EXCLUDED.""IsFeatured"",
    ""IsDeleted"" = EXCLUDED.""IsDeleted"",
    ""ResumeCategory"" = EXCLUDED.""ResumeCategory"",
    ""ExperienceType"" = EXCLUDED.""ExperienceType"",
    ""StartDate"" = EXCLUDED.""StartDate"",
    ""EndDate"" = EXCLUDED.""EndDate"",
    ""IsCurrentlyWorking"" = EXCLUDED.""IsCurrentlyWorking"",
    ""ReadmeMarkdown"" = EXCLUDED.""ReadmeMarkdown"",
    ""MetaTitle"" = EXCLUDED.""MetaTitle"",
    ""MetaDescription"" = EXCLUDED.""MetaDescription"",
    ""MetaKeywords"" = EXCLUDED.""MetaKeywords"",
    ""OgImageUrl"" = EXCLUDED.""OgImageUrl"",
    ""DisplayOrder"" = EXCLUDED.""DisplayOrder"",
    ""ThumbnailUrl"" = EXCLUDED.""ThumbnailUrl"",
    ""UpdatedAt"" = EXCLUDED.""UpdatedAt"";
");
        }

        private static void EnsureProjectTechnologySeedRows(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO ""ProjectTechnologies"" (""Id"", ""ProjectId"", ""TechnologyId"", ""CreatedAt"", ""UpdatedAt"")
SELECT v.""Id"", v.""ProjectId"", v.""TechnologyId"", v.""CreatedAt"", v.""UpdatedAt""
FROM (VALUES
    (1, 1, 1, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (2, 1, 2, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (3, 1, 3, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (4, 2, 4, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (5, 2, 5, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (6, 3, 6, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (7, 3, 7, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (8, 3, 8, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (9, 4, 1, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (10, 4, 9, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (11, 5, 10, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (12, 5, 11, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (13, 5, 12, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (14, 6, 1, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (15, 6, 13, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (16, 6, 14, TIMESTAMPTZ '2024-01-01 00:00:00+00', TIMESTAMPTZ '2024-01-01 00:00:00+00')
) AS v(""Id"", ""ProjectId"", ""TechnologyId"", ""CreatedAt"", ""UpdatedAt"")
JOIN ""Projects"" p ON p.""Id"" = v.""ProjectId""
JOIN ""Technologies"" t ON t.""Id"" = v.""TechnologyId""
ON CONFLICT (""Id"") DO UPDATE SET
    ""ProjectId"" = EXCLUDED.""ProjectId"",
    ""TechnologyId"" = EXCLUDED.""TechnologyId"",
    ""UpdatedAt"" = EXCLUDED.""UpdatedAt"";
");
        }

        private static void EnsureProjectTechnologyRowsFromNames(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SELECT setval(
    pg_get_serial_sequence('""Technologies""', 'Id'),
    COALESCE((SELECT MAX(""Id"") FROM ""Technologies""), 0) + 1,
    false);

INSERT INTO ""Technologies"" (""Name"", ""CreatedAt"", ""UpdatedAt"")
SELECT DISTINCT pt.""Name"", NOW(), NOW()
FROM ""ProjectTechnologies"" pt
WHERE pt.""TechnologyId"" = 0
  AND btrim(pt.""Name"") <> ''
  AND NOT EXISTS (
      SELECT 1
      FROM ""Technologies"" t
      WHERE t.""Name"" = pt.""Name"");

UPDATE ""ProjectTechnologies"" pt
SET ""TechnologyId"" = t.""Id"",
    ""UpdatedAt"" = NOW()
FROM ""Technologies"" t
WHERE pt.""TechnologyId"" = 0
  AND pt.""Name"" = t.""Name"";

INSERT INTO ""Technologies"" (""Name"", ""CreatedAt"", ""UpdatedAt"")
SELECT 'Uncategorized', NOW(), NOW()
WHERE EXISTS (
    SELECT 1
    FROM ""ProjectTechnologies""
    WHERE ""TechnologyId"" = 0)
  AND NOT EXISTS (
      SELECT 1
      FROM ""Technologies""
      WHERE ""Name"" = 'Uncategorized');

UPDATE ""ProjectTechnologies"" pt
SET ""TechnologyId"" = t.""Id"",
    ""UpdatedAt"" = NOW()
FROM ""Technologies"" t
WHERE pt.""TechnologyId"" = 0
  AND t.""Name"" = 'Uncategorized';
");
        }

        private static void EnsureProjectCategorySeedRows(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO ""ProjectCategories"" (""Id"", ""CategoryId"", ""CreatedAt"", ""ProjectId"", ""UpdatedAt"")
SELECT v.""Id"", v.""CategoryId"", v.""CreatedAt"", v.""ProjectId"", v.""UpdatedAt""
FROM (VALUES
    (1, 1, TIMESTAMPTZ '2024-01-01 00:00:00+00', 1, TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (2, 2, TIMESTAMPTZ '2024-01-01 00:00:00+00', 1, TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (3, 3, TIMESTAMPTZ '2024-01-01 00:00:00+00', 2, TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (4, 2, TIMESTAMPTZ '2024-01-01 00:00:00+00', 2, TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (5, 1, TIMESTAMPTZ '2024-01-01 00:00:00+00', 3, TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (6, 2, TIMESTAMPTZ '2024-01-01 00:00:00+00', 3, TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (7, 1, TIMESTAMPTZ '2024-01-01 00:00:00+00', 4, TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (8, 4, TIMESTAMPTZ '2024-01-01 00:00:00+00', 5, TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (9, 2, TIMESTAMPTZ '2024-01-01 00:00:00+00', 5, TIMESTAMPTZ '2024-01-01 00:00:00+00'),
    (10, 3, TIMESTAMPTZ '2024-01-01 00:00:00+00', 6, TIMESTAMPTZ '2024-01-01 00:00:00+00')
) AS v(""Id"", ""CategoryId"", ""CreatedAt"", ""ProjectId"", ""UpdatedAt"")
JOIN ""Projects"" p ON p.""Id"" = v.""ProjectId""
JOIN ""Categories"" c ON c.""Id"" = v.""CategoryId""
ON CONFLICT (""Id"") DO UPDATE SET
    ""CategoryId"" = EXCLUDED.""CategoryId"",
    ""ProjectId"" = EXCLUDED.""ProjectId"",
    ""UpdatedAt"" = EXCLUDED.""UpdatedAt"";
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTechnologies_Technologies_TechnologyId",
                table: "ProjectTechnologies");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ProjectAchievements");

            migrationBuilder.DropTable(
                name: "ProjectCategories");

            migrationBuilder.DropTable(
                name: "ProjectFeatures");

            migrationBuilder.DropTable(
                name: "ProjectImages");

            migrationBuilder.DropTable(
                name: "ProjectLinks");

            migrationBuilder.DropTable(
                name: "ProjectSkills");

            migrationBuilder.DropTable(
                name: "Technologies");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTechnologies_TechnologyId",
                table: "ProjectTechnologies");

            migrationBuilder.DropIndex(
                name: "IX_Projects_IsDeleted",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_IsFeatured",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_IsPublished",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Status",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TechnologyId",
                table: "ProjectTechnologies");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ExperienceType",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FullDescription",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsCurrentlyWorking",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MetaKeywords",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ReadmeMarkdown",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ResumeCategory",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "Visibility",
                table: "Projects",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "ThumbnailUrl",
                table: "Projects",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Projects",
                newName: "CategoriesJson");

            migrationBuilder.RenameColumn(
                name: "OgImageUrl",
                table: "Projects",
                newName: "LiveUrl");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProjectTechnologies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "React");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Node.js");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "MongoDB");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "React Native");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Firebase");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "JavaScript");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Canvas API");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 8,
                column: "Name",
                value: "PHP");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "React");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "TailwindCSS");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 11,
                column: "Name",
                value: "Unity");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 12,
                column: "Name",
                value: "C#");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 13,
                column: "Name",
                value: "WebGL");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 14,
                column: "Name",
                value: "React");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 15,
                column: "Name",
                value: "SCSS");

            migrationBuilder.UpdateData(
                table: "ProjectTechnologies",
                keyColumn: "Id",
                keyValue: 16,
                column: "Name",
                value: "Framer Motion");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoriesJson", "Description", "ImageUrl" },
                values: new object[] { "[\"webdesign\",\"webapp\"]", "A platform connecting students with tutors based on subject, location, and availability.", "/assets/images/project-tutor-finder.png" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoriesJson", "Description", "ImageUrl" },
                values: new object[] { "[\"mobiledesign\",\"webapp\"]", "A mobile-friendly college discovery and comparison application.", "/assets/images/project-college-lake.png" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoriesJson", "Description", "ImageUrl" },
                values: new object[] { "[\"webdesign\",\"webapp\"]", "A web application allowing users to create and save digital signatures.", "/assets/images/project-online-signature.png" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoriesJson", "Description", "ImageUrl" },
                values: new object[] { "[\"webdesign\"]", "An application that helps users assess and plan their technology skill development.", "/assets/images/project-skill-navigator.png" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoriesJson", "Description", "ImageUrl" },
                values: new object[] { "[\"gamedesign\",\"webapp\"]", "Digital version of the classic Indian card game with online multiplayer.", "/assets/images/project-game.png" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoriesJson", "Description", "ImageUrl" },
                values: new object[] { "[\"mobiledesign\"]", "A mobile-first personal portfolio with animated transitions.", "/assets/images/project-portfolio.png" });
        }
    }
}
