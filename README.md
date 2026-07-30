# Satyam Kumar — Portfolio

A production-quality full-stack portfolio application converted from a static HTML/jQuery/Bootstrap site to a modern **React + TypeScript + ASP.NET Core** architecture.

---

## ✨ Features

- **Single-page application** with animated section transitions
- **Animated sidebar** that expands/collapses based on the active section
- **Skill bars** that animate on scroll (replaces jQuery countTo + Waypoints)
- **Portfolio filter grid** (replaces Isotope.js)
- **Testimonial carousel** (replaces Owl Carousel)
- **Contact form** with server-side validation, rate limiting, and database storage (replaces EmailJS)
- **Custom animated cursor** (disabled on touch / reduced-motion)
- **Preloader** animation
- **WCAG 2.2 AA** accessibility — skip link, visible focus states, ARIA, semantic landmarks
- **Full SEO** — OG tags, Twitter cards, structured data, canonical URL
- **Dark/light** theme token system (blue default)

---

## 🏗️ Architecture

```
Portfolio.sln
├── src/
│   ├── Portfolio.Client/          # React + TypeScript + Vite SPA
│   ├── Portfolio.Api/             # ASP.NET Core 9 Web API
│   ├── Portfolio.Application/     # Business logic + interfaces + DTOs
│   ├── Portfolio.Domain/          # Entity classes
│   └── Portfolio.Infrastructure/  # EF Core + PostgreSQL + Email
├── tests/
│   ├── Portfolio.Api.Tests/       # xUnit integration tests
│   └── Portfolio.Application.Tests/
├── docs/                          # Architecture + migration docs
├── infra/                         # Infrastructure-as-code (future)
├── docker-compose.yml
└── Portfolio.sln
```

### Clean Architecture layers

```
Domain ← Application ← Infrastructure ← API
              ↑
           Frontend (React)
```

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | React 18, TypeScript, Vite, CSS Modules |
| State | TanStack Query v5 |
| Forms | React Hook Form + Zod |
| HTTP | Axios |
| Icons | Bootstrap Icons, Font Awesome |
| Backend | ASP.NET Core 9, C# |
| ORM | Entity Framework Core 9 |
| Database | PostgreSQL 16 |
| Testing (FE) | Vitest, React Testing Library |
| Testing (BE) | xUnit, FluentAssertions, WebApplicationFactory |
| Container | Docker, nginx |
| CI/CD | GitHub Actions |

---

## 🚀 Quick Start

### Prerequisites
- Node.js 22+
- .NET SDK 9.0
- PostgreSQL 16 (or Docker)

### Option A — Docker Compose (recommended)

```bash
# 1. Clone
git clone https://github.com/satyam6290/Portfolio.git
cd Portfolio

# 2. Configure environment
cp .env.example .env
# Edit .env and set POSTGRES_PASSWORD at minimum

# 3. Start everything
docker compose up --build

# Frontend: http://localhost
# API:      http://localhost:5000
# Swagger:  http://localhost:5000/swagger
```

### Option B — Local Development

**Frontend:**
```bash
cd src/Portfolio.Client
npm ci
cp .env.example .env.local    # set VITE_API_BASE_URL=http://localhost:5000
npm run dev
# → http://localhost:5173
```

**Backend:**
```bash
# Create PostgreSQL database
createdb portfolio

# Set connection string (or use user secrets)
cd src/Portfolio.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Database=portfolio;Username=YOUR_USER;Password=YOUR_PASS"

# Run migrations
dotnet ef database update --project ../Portfolio.Infrastructure

# Start API
dotnet run
# → http://localhost:5000
# → Swagger: http://localhost:5000/swagger
```

---

## ⚙️ Environment Variables

Copy `.env.example` to `.env` and fill in values. **Never commit `.env`.**

| Variable | Required | Description |
|----------|----------|-------------|
| `POSTGRES_PASSWORD` | ✅ | Database password |
| `POSTGRES_DB` | No | Database name (default: `portfolio`) |
| `POSTGRES_USER` | No | Database user (default: `portfolio_user`) |
| `SMTP_HOST` | No | SMTP server hostname |
| `SMTP_PORT` | No | SMTP port (default: `587`) |
| `SMTP_USER` | No | SMTP username |
| `SMTP_PASS` | No | SMTP password — **never commit** |
| `NOTIFY_EMAIL` | No | Where contact emails are forwarded |
| `VITE_API_BASE_URL` | No | API URL for frontend (blank = static data) |

---

## 🗃️ Database Migrations

```bash
# Create a migration (from repo root)
dotnet ef migrations add MigrationName \
  --project src/Portfolio.Infrastructure \
  --startup-project src/Portfolio.Api

# Apply migrations
dotnet ef database update \
  --project src/Portfolio.Infrastructure \
  --startup-project src/Portfolio.Api

# Remove last migration
dotnet ef migrations remove \
  --project src/Portfolio.Infrastructure \
  --startup-project src/Portfolio.Api
```

---

## 🧪 Testing

### Frontend
```bash
cd src/Portfolio.Client
npm run test           # Run all unit tests
npm run test:watch     # Watch mode
npm run typecheck      # Type-check without emitting
npm run lint           # ESLint
```

### Backend
```bash
# From repo root
dotnet test Portfolio.sln
```

---

## 🔌 API

Full API documentation: [docs/api-design.md](docs/api-design.md)

Interactive Swagger UI: `http://localhost:5000/swagger` (development only)

Health check: `GET /health`

---

## 🐳 Docker

```bash
# Build and start (production mode)
docker compose up --build

# Start without rebuilding
docker compose up

# Stop
docker compose down

# Remove volumes (wipes database)
docker compose down -v
```

---

## 📦 Project Scripts

### Frontend (`src/Portfolio.Client`)
| Command | Description |
|---------|-------------|
| `npm run dev` | Start Vite dev server |
| `npm run build` | Production build |
| `npm run preview` | Preview production build |
| `npm run lint` | Run ESLint |
| `npm run typecheck` | TypeScript type checking |
| `npm run test` | Run Vitest |

### Backend (from repo root)
| Command | Description |
|---------|-------------|
| `dotnet restore` | Restore NuGet packages |
| `dotnet build` | Build solution |
| `dotnet test` | Run all tests |
| `dotnet run --project src/Portfolio.Api` | Start API |

---

## 🔒 Security

- All SMTP credentials are in environment variables — never in source code
- The original EmailJS public key has been removed (was exposed in `satyam.js`)
- Rate limiting on all endpoints (100/min general, 5/10min contact)
- CORS restricted to configured origin
- Input validation on both frontend (Zod) and backend (DataAnnotations)
- Security headers: CSP, X-Frame-Options, X-Content-Type-Options, HSTS
- Request size limited to 1 MB
- No secrets in Docker images or CI logs

See [SECURITY.md](SECURITY.md) for the vulnerability reporting policy.

---

## ♿ Accessibility

- Skip-to-content link
- All interactive elements keyboard accessible
- Visible focus indicators
- Semantic HTML5 landmarks (`<main>`, `<nav>`, `<aside>`, `<section>`, `<article>`)
- Correct heading hierarchy (h1 → h2 → h3)
- `aria-current="page"` on active nav items
- `aria-expanded` on hamburger button
- `role="progressbar"` on skill bars
- `role="tablist"` + `role="tab"` on portfolio filters
- `aria-live` on animated counter values
- `alt` text on all images
- Custom cursor disabled for `prefers-reduced-motion` and touch devices
- Testimonial carousel keyboard navigable

---

## 📄 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

---

## 📜 Licence

MIT — see [LICENSE](LICENSE).
# portfolio-2.0
