# Workhours Demo

A full-stack application for recording weekly workhours by project, with secure per-user access and week finalization.

## Features
- Account registration and login (email/password).
- Bearer token authentication using OpenIddict.
- Week browsing (defaults to current week).
- Daily time entry per project (free-text project name).
- Finalize week to lock entries.
- Strict user data isolation (users see only their own records).
- Logout and automatic session expiration after 15 minutes.

## Tech Stack
- Backend: .NET REST API
- Frontend: React + Vite
- Auth: OpenIddict (bearer tokens)
- Database: PostgreSQL
- ORM: Entity Framework Core
- Testing: Dedicated backend and frontend test projects
- Containers: Backend and frontend containerized; PostgreSQL in Docker for local development

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

## API Expectations
- Auth endpoints:
	- POST /api/auth/register
	- POST /api/auth/login
	- POST /api/auth/logout
- Workhours endpoints (authorized):
	- GET /api/weeks/{year}/{week} (or default current week)
	- PUT /api/weeks/{year}/{week}/entries
	- POST /api/weeks/{year}/{week}/finalize

All workhours endpoints must resolve the current user from token claims and enforce ownership server-side.

## Local Development
- Run PostgreSQL in Docker.
- Run backend locally for debugging.
- Run frontend locally for debugging.
- Configure connection strings and auth settings in appsettings and env files.

## Security Notes
- Access token lifetime: 15 minutes.
- Protected endpoints require valid bearer token.
- Tokens include user identifier claim used for authorization.

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
