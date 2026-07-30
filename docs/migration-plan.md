# Migration Plan

**Source:** Static HTML/jQuery portfolio  
**Target:** React + TypeScript + Vite + ASP.NET Core + PostgreSQL  
**Status:** Phase 1–8 complete (React frontend + backend scaffold)

---

## Phase 1 — Audit ✅
- Cloned and inspected every repository file
- Documented all HTML sections, JS interactions, assets, and content
- Identified security issues (EmailJS key exposure)
- Created `docs/current-application-audit.md`

## Phase 2 — Documentation ✅
- Created migration plan, component map, and API design docs
- Mapped every jQuery interaction to its React replacement

## Phase 3 — Scaffold ✅

### Frontend
```
Portfolio.Client/
  src/
    api/           — TanStack Query hooks + Axios client + static seed data
    components/
      layout/      — AppShell, DesktopSidebar, RightUtilityRail, MobileNavigation
      sections/    — HeroSection, AboutSection, ResumeSection, PortfolioSection, BlogSection, ContactSection
      ui/          — LoadingScreen, CustomCursor, SkillProgress, StatCounter, ErrorBoundary
      contact/     — ContactForm (zod + react-hook-form)
    hooks/         — useSectionNavigation, useCounter, useInView
    styles/        — tokens.css, global.css
    types/         — index.ts (all domain types)
    test/          — setup.ts
```

### Backend
```
Portfolio.Domain/
  Entities.cs      — 11 entity classes

Portfolio.Application/
  DTOs/            — 11 DTO record types
  Interfaces/      — 6 service interfaces
  Services/        — 5 service implementations (EF Core)

Portfolio.Infrastructure/
  Data/            — PortfolioDbContext (EF Core + PostgreSQL + seed data)
  Email/           — SmtpEmailService

Portfolio.Api/
  Controllers/     — 10 REST API controllers
  Middleware/      — RequestLoggingMiddleware
  Program.cs       — Full DI, CORS, rate limiting, health checks, Swagger
```

## Phase 4 — Design System ✅
- Created `tokens.css` with CSS custom properties covering all original colours
- Blue primary (`#028ac9`) preserved exactly from `assets/colors/blue.css`
- All layout proportions (sidebar 300px/80px, rail 80px, main area) preserved
- All original animations (preloader, floating circles, section transitions) reproduced in CSS

## Phase 5 — Section Migration ✅
All 6 sections implemented as React components:

| Section | Original | React Implementation |
|---------|----------|---------------------|
| Hero | HTML + Typed.js | `HeroSection` + `useState`/`setInterval` |
| About | HTML + countTo + Waypoints + Owl Carousel | `AboutSection` + `useCounter` + `useInView` + `TestimonialCarousel` |
| Resume | HTML + jQuery | `ResumeSection` — pure React |
| Portfolio | HTML + Isotope.js | `PortfolioSection` + filter state |
| Blog | HTML + jQuery | `BlogSection` — pure React |
| Contact | HTML + EmailJS | `ContactSection` + `ContactForm` + ASP.NET Core API |

## Phase 6 — jQuery Removal ✅
All 14 jQuery interactions replaced with React equivalents (see audit doc).

## Phase 7 — Backend Implementation ✅
- Clean Architecture in place (Domain → Application → Infrastructure → API)
- Entity Framework Core with PostgreSQL
- All 11 API endpoints implemented
- Contact form stores to DB, optional SMTP email via secure config
- Rate limiting: 100/min general, 5/10min contact
- Anti-spam: duplicate detection (same email+subject within 5 min)
- No secrets in code — all via environment variables

## Phase 8 — Security Remediation ✅
- Removed EmailJS public key from client code
- Contact form credentials moved to server environment config
- Security headers: X-Content-Type-Options, X-Frame-Options, CSP, HSTS (prod)
- Input validation: Zod (frontend) + DataAnnotations (backend)
- Rate limiting on all endpoints

## Phase 9 — Testing ✅ (scaffolded)
- Frontend: Vitest + React Testing Library
  - `ContactForm.test.tsx` — 5 tests
  - `PortfolioSection.test.tsx` — 5 tests
  - `MobileNavigation.test.tsx` — 8 tests
- Backend: xUnit + FluentAssertions + WebApplicationFactory
  - Profile, Skills, Projects (filtering + slug), Blog (pagination + slug), Contact (validation + submit), Health check

## Phase 10 — Docker & CI ✅
- Frontend Dockerfile (Node builder → nginx)
- Backend Dockerfile (SDK builder → ASP.NET runtime)
- `docker-compose.yml` with health checks, volumes, environment variables
- `.env.example` template
- GitHub Actions: frontend CI, backend CI, Docker build verification

## Phase 11 — Content Migration ✅
All existing content seeded into `PortfolioDbContext.SeedData()`:
- Profile, 3 Social Links, 5 Skills, 4 Statistics, 6 Services, 4 Education entries,
  4 Soft Skills, 6 Projects with technologies, 4 Blog Posts

## Remaining Steps (post-scaffold)

1. **Copy image assets** from `portfolio-source/assets/images/` to
   `Portfolio.Client/public/assets/images/` with semantic filenames
2. **Run `dotnet ef migrations add InitialCreate`** once .NET SDK available
3. **Complete blog post content** — add full article body to `Content` field in seed data
4. **Add Playwright E2E tests** (`tests/Portfolio.Client.Tests/`)
5. **Apply theme colour switcher** — dark/light toggle
6. **Add sitemap.xml and robots.txt** to `Portfolio.Client/public/`
7. **Run Lighthouse audits** and address any remaining gaps
8. **Deploy** following README deployment instructions

---

## Feature Parity Checklist

| Original Feature | Status | Notes |
|-----------------|--------|-------|
| Hero with animated roles | ✅ | `useState` + `setInterval` replaces Typed.js |
| Sidebar open/close animation | ✅ | CSS `transition` + React state |
| Mobile hamburger menu | ✅ | `MobileNavigation` component |
| Preloader animation | ✅ | `LoadingScreen` component |
| Custom cursor | ✅ | `CustomCursor` — disabled on touch/reduced-motion |
| Background floating circles | ✅ | CSS keyframe animation |
| About bio + personal info | ✅ | `AboutSection` |
| Animated stat counters | ✅ | `useCounter` + `useInView` |
| Skill progress bars | ✅ | `SkillProgress` + CSS transition on intersection |
| Language dot bars | ✅ | `LanguageSkill` sub-component |
| Testimonials carousel | ✅ | `TestimonialCarousel` with dot navigation |
| Services cards | ✅ | `ResumeSection` |
| Education timeline | ✅ | `ResumeSection` |
| Soft skills | ✅ | `ResumeSection` |
| Portfolio filter tabs | ✅ | `PortfolioSection` with filter state |
| Portfolio cards with hover overlay | ✅ | CSS hover + overlay |
| Blog cards with date | ✅ | `BlogSection` |
| Contact info boxes | ✅ | `ContactSection` |
| Contact form with validation | ✅ | `ContactForm` + Zod |
| Right rail with date + social | ✅ | `RightUtilityRail` |
| Prev/next section navigation | ✅ | `useSectionNavigation` |
| Right rail social links | ✅ | `RightUtilityRail` |
| Responsive mobile layout | ✅ | CSS Modules media queries |
| WCAG 2.2 AA accessibility | ✅ | Skip link, ARIA, focus styles, semantic HTML |
| SEO meta tags | ✅ | `index.html` with OG + Twitter cards |
