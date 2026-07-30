# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS builder

WORKDIR /src

# Restore dependencies (layer cache)
COPY ["src/Portfolio.Api/Portfolio.Api.csproj", "src/Portfolio.Api/"]
COPY ["src/Portfolio.Application/Portfolio.Application.csproj", "src/Portfolio.Application/"]
COPY ["src/Portfolio.Domain/Portfolio.Domain.csproj", "src/Portfolio.Domain/"]
COPY ["src/Portfolio.Infrastructure/Portfolio.Infrastructure.csproj", "src/Portfolio.Infrastructure/"]

RUN dotnet restore "src/Portfolio.Api/Portfolio.Api.csproj"

# Copy all source and publish
COPY . .
RUN dotnet publish "src/Portfolio.Api/Portfolio.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 2: Runtime ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runner

# Security: run as non-root
RUN adduser -D -u 1001 appuser
USER appuser

WORKDIR /app
COPY --from=builder --chown=appuser:appuser /app/publish .

# Healthcheck
HEALTHCHECK --interval=30s --timeout=10s --start-period=20s --retries=3 \
    CMD wget -qO- http://localhost:8080/health || exit 1

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Portfolio.Api.dll"]
