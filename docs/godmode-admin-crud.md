# Godmode Admin CRUD

## PRD (Product Requirements Document)
- Goal: Only the portfolio admin can create, edit, or delete projects and blog posts.
- Route: The private admin surface is available at `/godmode`.
- Login: The admin logs in with the configured admin email and password.
- Public users: Visitors can read published projects and published blog posts only; they cannot see CRUD controls.
- Admin users: Logged-in admins can manage projects and blog posts from one workspace.

## TRD (Technical Requirements Document)
- Backend: ASP.NET Core API exposes protected write endpoints for projects and blog posts.
- Auth: `/api/godmode/login` returns a signed bearer token for valid admin credentials.
- Authorization: Project/blog `POST`, `PUT`, and `DELETE` reject missing or invalid bearer tokens with `401`.
- Frontend: React stores the admin session in `localStorage` until the token expires.
- Caching: Admin draft reads use `Cache-Control: no-store`; public reads remain cacheable.

## Master Development Prompt
Build a private `/godmode` admin workspace for the portfolio. Keep public project and blog reads available, but require a valid admin bearer token for every create, update, and delete operation. The UI must start with a login wall, then expose project and blog management tabs backed by the API. Do not expose admin navigation in the public sidebar.

## Database Schema
- No new tables are required.
- Existing tables used:
  - `Projects`: managed through protected CRUD endpoints.
  - `ProjectTechnologies`: replaced on project updates and cascade-deleted with projects.
  - `BlogPosts`: managed through protected CRUD endpoints, including `IsPublished` for draft handling.
- Slugs are generated and de-duplicated server-side from titles.

## API Specification
- `POST /api/godmode/login`
  - Body: `{ "email": "...", "password": "..." }`
  - Success: `{ "token": "...", "email": "...", "expiresAt": "..." }`
- Public:
  - `GET /api/projects`
  - `GET /api/projects/{slug}`
  - `GET /api/blog`
  - `GET /api/blog/{slug}`
- Admin-only:
  - `POST /api/projects`
  - `PUT /api/projects/{id}`
  - `DELETE /api/projects/{id}`
  - `GET /api/blog?includeUnpublished=true`
  - `POST /api/blog`
  - `PUT /api/blog/{id}`
  - `DELETE /api/blog/{id}`

## System Architecture
- React SPA routes `/godmode` to `AdminPanel`.
- `AdminPanel` logs in through `/api/godmode/login`.
- API writes are sent through Axios with `Authorization: Bearer <token>`.
- ASP.NET Core controllers validate tokens through `IAdminAuthService`.
- EF Core services persist projects, technologies, and blog posts.

## Sprint Roadmap
- Sprint 1: Add admin auth service and protected backend CRUD.
- Sprint 2: Replace local-only admin UI with API-backed `/godmode` workspace.
- Sprint 3: Remove public admin navigation and move public portfolio reads to query-backed data.
- Sprint 4: Verify builds/tests and document deployment/security knobs.

## UI/UX Guidelines
- `/godmode` opens to a focused admin login screen.
- The authenticated workspace uses tabs for Projects and Blog.
- Lists support search, edit, delete, refresh, and new-item actions.
- Forms keep required fields visible and validation inline.
- Destructive actions require confirmation.

## DevOps & Deployment
- Configure production credentials with environment variables or configuration:
  - `AdminAuth__Email`
  - `AdminAuth__Password`
  - `AdminAuth__TokenSigningKey`
  - `AdminAuth__TokenHours`
- Rotate the default password before public deployment.
- Frontend builds use `vite.config.mjs` with Vite's runner config loader to avoid Windows config bundling permission issues.
- Ensure SPA hosting rewrites `/godmode` to `index.html`.

## AI Features Blueprint
- Current scope does not add AI behavior.
- Future optional admin AI features:
  - Blog excerpt generation from draft content.
  - Project description polishing.
  - Tag/category suggestions.
  - SEO title and meta description suggestions.
