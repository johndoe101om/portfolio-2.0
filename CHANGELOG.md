# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [1.0.0] — 2025-07-20

### Added
- Complete React + TypeScript + Vite frontend replacing static HTML
- ASP.NET Core 9 Web API with Clean Architecture (Domain / Application / Infrastructure / API)
- PostgreSQL database via Entity Framework Core with full seed data
- All 11 REST API endpoints (`/api/profile`, `/api/skills`, `/api/projects`, `/api/blog`, `/api/contact`, etc.)
- TanStack Query v5 for data fetching with static seed data fallback
- React Hook Form + Zod for type-safe, accessible contact form validation
- `useSectionNavigation` hook replacing all jQuery sidebar toggling
- `useCounter` hook with `requestAnimationFrame` replacing jquery.countTo
- `useInView` hook with `IntersectionObserver` replacing Waypoints.js
- `SkillProgress` component with animated bars triggered on scroll
- `TestimonialCarousel` component replacing Owl Carousel
- Portfolio filtering via React state replacing Isotope.js
- `CustomCursor` component (disabled on touch / reduced-motion)
- `LoadingScreen` preloader component
- CSS design tokens (`tokens.css`) with full blue theme token system
- WCAG 2.2 AA accessibility: skip link, ARIA landmarks, focus styles, keyboard navigation
- SEO: Open Graph, Twitter cards, canonical URL, sitemap.xml, robots.txt
- Docker Compose for full-stack local development
- Multi-stage Dockerfiles for frontend (nginx) and backend (ASP.NET)
- GitHub Actions CI/CD: frontend lint/test/build, backend build/test, Docker verification
- 18 frontend unit tests (Vitest + React Testing Library)
- 12 backend integration tests (xUnit + WebApplicationFactory)
- 20 Playwright E2E tests covering all major user flows
- Rate limiting: 100 req/min general, 5/10min contact
- Contact message deduplication (same email + subject within 5 min)
- Secure SMTP email via environment variables (no credentials in code)

### Changed
- Replaced static HTML/jQuery/Bootstrap with React SPA
- Replaced EmailJS (exposed API key) with secure server-side contact API
- All image assets renamed from cryptic numbers (`1.png`, `12.png`) to semantic names
- Removed all jQuery, Bootstrap JS, and third-party plugins (Isotope, Typed.js, Waypoints, etc.)

### Security
- **Critical:** Removed EmailJS public key (`sirsatyam6290`) that was exposed in client-side JS
- Added Content-Security-Policy, X-Frame-Options, HSTS, X-Content-Type-Options headers
- Added request size limiting (1 MB max)
- CORS restricted to configured origins only
- No secrets in source control; `.env.example` template provided

### Fixed
- Cryptic `alt="/"` text on all images replaced with descriptive alt text
- Missing `aria-label` on social links
- Missing heading hierarchy (now correct h1 → h2 → h3)
