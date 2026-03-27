# Workhours Demo

A full-stack application for recording weekly workhours by project, with secure per-user access and week finalization.

## Features
- Account registration and login (email/password).
- Bearer token authentication with 15-minute JWT access tokens.
- Week browsing (defaults to current week).
- Daily time entry per project (free-text project name).
- Finalize week with a persisted boolean status.
- Strict user data isolation (users see only their own records).
- Logout with immediate token revocation plus automatic session expiration after 15 minutes.

## Tech Stack
- Backend: .NET REST API
- Frontend: React + Vite
- Auth: JWT bearer tokens
- Database: PostgreSQL
- Token revocation: Redis
- ORM: Entity Framework Core
- Testing: Dedicated backend and frontend test projects
- Containers: Backend and frontend containerized; PostgreSQL and Redis in Docker for local development

## High-Level Architecture
- Controllers: HTTP transport only, no business logic.
- Services: Business rules and orchestration.
- Clients: External service integration wrappers.
- Models: Domain and transport models.
- Infrastructure: EF Core persistence and auth wiring.
- DI: Used across all layers.

## Functional Scope
1. User registers account.
2. User logs in with email/password.
3. User navigates workweeks (current week default).
4. User enters project + time per day.
5. User finalizes week (read-only afterward).
6. User can fetch/update only their own data.
7. User logs out.
8. Session/token expires after 15 minutes.

## Suggested Repository Structure
- /backend
	- /Api
	- /Application
	- /Domain
	- /Infrastructure
	- /Tests
- /frontend
	- /src
	- /tests
- /docker
	- docker-compose for local database/full stack options

## Current Repository Layout
- /workhours: ASP.NET Core backend API
- /workhours.Tests: backend unit tests (xUnit)
- /frontend: React + Vite + TypeScript frontend with Vitest component tests
- /docker-compose.yml: local PostgreSQL stack
- /.env.example: local environment variable template for DB and API wiring

## API Expectations
- Auth endpoints:
	- POST /api/auth/register
	- POST /api/auth/login
	- POST /api/auth/logout
- Workhours endpoints (authorized):
	- GET /api/weeks?date=2026-03-27
	- GET /api/weeks/{workweekId}/entries
	- POST /api/weeks/{workweekId}/entries
	- PUT /api/weeks/{workweekId}/entries/{entryId}
	- DELETE /api/weeks/{workweekId}/entries/{entryId}
	- PATCH /api/weeks/{workweekId}/finalize

All workhours endpoints must resolve the current user from token claims and enforce ownership server-side.

## Local Development
1. Copy `.env.example` to `.env` at repository root and adjust values if needed.
2. Start local database from repository root:
	- `docker compose up -d`
3. Run backend from `/workhours`:
	- `dotnet ef database update`
	- `dotnet run`
4. Run frontend from `/frontend`:
	- `npm install`
	- copy `.env.example` to `.env.local` (optional override)
	- `npm run dev`

### Local Networking Notes
- Frontend dev server runs on `http://localhost:5173`.
- Frontend calls backend directly through `VITE_API_BASE_URL`.
- Backend CORS allow list is configured for `http://localhost:5173`.
- Redis runs on `localhost:6379` for logout token revocation.

## Security Notes
- Access token lifetime: 15 minutes.
- Protected endpoints require valid bearer token.
- Tokens include user identifier claim used for authorization.
- Logged out tokens are blacklisted immediately until they expire.

## Database Artifacts
- EF Core migrations live in `/workhours/Infrastructure/Data/Migrations`.
- Idempotent SQL deployment script lives in `/workhours/Infrastructure/Migrations/Initial.sql`.
- Regenerate the script with:
	- `dotnet ef migrations script --idempotent --project ./workhours/workhours.csproj --startup-project ./workhours/workhours.csproj --output ./workhours/Infrastructure/Migrations/Initial.sql`

## Testing
- Backend tests:
	- Service logic
	- Authorization constraints
	- Finalization lock behavior
- Frontend tests:
	- Auth flow
	- Week navigation
	- Entry editing/finalization UI states

## Definition of Done
- Functional requirements implemented.
- Authorization guarantees validated by tests.
- Local setup works as documented.
- Build and test commands pass.
- Container workflow available for app components.
