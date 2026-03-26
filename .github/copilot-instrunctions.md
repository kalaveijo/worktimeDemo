# Copilot Instructions - Workhours App

## Project Goal
Build a weekly workhours application where authenticated users track project time per day and finalize each week.

## Core Functional Requirements
- User can register an account.
- User can log in with email/password.
- User can browse weeks (default current week).
- User can enter project name (text) and time per day.
- User can finalize a week; finalized entries are read-only.
- User can access only their own data.
- User can log out.
- Session expires after 15 minutes.

## Architecture Rules
- Backend: .NET REST API.
- Frontend: React + Vite.
- Database: PostgreSQL.
- ORM: Entity Framework Core.
- Auth: OpenIddict with bearer tokens.
- Tokens must contain claims needed for authorization (minimum stable user id).
- Business logic belongs in service classes.
- Controllers only orchestrate HTTP input/output and call services.
- Use dependency injection for services, clients, and infrastructure.
- External integrations must be behind custom clients.
- Keep folder separation clear:
	- Clients
	- Services
	- Models
- Backend and frontend must be containerizable.
- Local dev: PostgreSQL via Docker; backend/frontend run locally for debugging.
- Include separate backend and frontend test projects.

## Backend Conventions
- Keep controller actions thin and async.
- Enforce per-user data filtering in service/repository queries.
- Validate input DTOs and return consistent error contracts.
- Prevent updates when week status is finalized.
- Use transactions where multi-row consistency is required.
- Keep EF entities internal to infrastructure boundaries where practical; map to contracts/DTOs.

## Security and Authorization
- Require authentication on all workhours endpoints.
- Read user id from token claims, not request body.
- Never accept user id as editable client input for owned resources.
- Configure 15-minute access token lifetime.
- Reject expired/invalid tokens with standard 401 responses.

## Testing Expectations
- Backend tests:
	- Service-level business rules.
	- Authorization boundary tests (cross-user denial).
	- Finalization lock behavior.
- Frontend tests:
	- Login/logout flow.
	- Week navigation defaults.
	- Edit vs finalized read-only behavior.
- Add integration tests for core API happy path.

## Delivery Checklist
- API endpoints documented in README.
- Local setup instructions verified.
- Docker workflow documented.
- Test commands documented and passing.
