# Contributing

Thank you for your interest in contributing to this portfolio project!

## Getting Started

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Follow the setup instructions in [README.md](README.md)

## Development Guidelines

### Frontend (React / TypeScript)
- All components must be written in TypeScript with strict mode
- Use CSS Modules for styling — no inline styles or global class names
- All colours and spacing must use CSS custom properties from `tokens.css`
- No jQuery or direct DOM manipulation outside a justified `useRef`
- Every new component should have a corresponding `__tests__/` file
- Run `npm run lint` and `npm run typecheck` before pushing

### Backend (ASP.NET Core)
- Follow Clean Architecture — no business logic in controllers, no DB logic in services
- Use `async`/`await` and `CancellationToken` throughout
- New endpoints need integration tests in `Portfolio.Api.Tests`
- Run `dotnet format` before pushing

### Commits
Use [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` new feature
- `fix:` bug fix
- `docs:` documentation only
- `style:` formatting (no logic change)
- `refactor:` code change with no new feature or fix
- `test:` adding or fixing tests
- `chore:` build/CI changes

## Pull Request Checklist

- [ ] `npm run lint` passes
- [ ] `npm run typecheck` passes
- [ ] `npm run test` passes
- [ ] `dotnet build` passes
- [ ] `dotnet test` passes
- [ ] No secrets committed
- [ ] Accessibility: new interactive elements are keyboard accessible
- [ ] Documentation updated if behaviour changed

## Code of Conduct

See [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md).
