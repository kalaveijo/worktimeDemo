# Frontend API Guide

## Base URL

- Local backend: `http://localhost:5020`
- All authenticated endpoints require `Authorization: Bearer <token>`

## Authentication Flow

1. Register a user with `POST /api/auth/register`
2. Login with `POST /api/auth/login`
3. Store `accessToken` in memory or a secure storage strategy used by the frontend
4. Send the token on every workhours request
5. Logout with `POST /api/auth/logout` to revoke the current token immediately

## Endpoints

### Register

- Method: `POST`
- URL: `/api/auth/register`
- Body:

```json
{
  "email": "alice@example.com",
  "password": "passw0rd"
}
```

- Success: `201 Created`

### Login

- Method: `POST`
- URL: `/api/auth/login`
- Body:

```json
{
  "email": "alice@example.com",
  "password": "passw0rd"
}
```

- Success body:

```json
{
  "accessToken": "<jwt>",
  "expiresAtUtc": "2026-03-27T11:15:00+00:00"
}
```

### Get Workweek

- Method: `GET`
- URL: `/api/weeks?date=2026-03-27`
- Notes:
  - `date` is optional. If omitted, backend uses the current UTC date.
  - Response creates the week if it does not already exist.
- Success body:

```json
{
  "id": "44bc182c-f737-4b2d-b722-df69129588b4",
  "weekStart": "2026-03-23",
  "weekEnd": "2026-03-29",
  "isFinalized": false,
  "createdAtUtc": "2026-03-27T10:00:00+00:00",
  "updatedAtUtc": "2026-03-27T10:00:00+00:00"
}
```

### Get Time Entries For a Week

- Method: `GET`
- URL: `/api/weeks/{workweekId}/entries`
- Success body:

```json
[
  {
    "id": "16a559db-3fce-444b-89c8-2f0fa93fe1a6",
    "workweekId": "44bc182c-f737-4b2d-b722-df69129588b4",
    "entryDate": "2026-03-23",
    "projectName": "Project Mercury",
    "hours": 7.5,
    "createdAtUtc": "2026-03-27T10:02:00+00:00",
    "updatedAtUtc": "2026-03-27T10:02:00+00:00"
  }
]
```

### Create Time Entry

- Method: `POST`
- URL: `/api/weeks/{workweekId}/entries`
- Body:

```json
{
  "entryDate": "2026-03-24",
  "projectName": "Project Mercury",
  "hours": 8
}
```

- Validation rules:
  - `projectName` is required and max `200` characters
  - `hours` must be `> 0` and `<= 24`
  - `entryDate` must fall within the selected week
  - duplicate `(projectName, entryDate)` pairs for the same week are rejected with `409`

### Update Time Entry

- Method: `PUT`
- URL: `/api/weeks/{workweekId}/entries/{entryId}`
- Body:

```json
{
  "hours": 6.5
}
```

### Delete Time Entry

- Method: `DELETE`
- URL: `/api/weeks/{workweekId}/entries/{entryId}`

### Update Finalized Flag

- Method: `PATCH`
- URL: `/api/weeks/{workweekId}/finalize`
- Body:

```json
{
  "isFinalized": true
}
```

## Error Contract

All handled API errors return:

```json
{
  "errorCode": "validation_error",
  "message": "Project name is required."
}
```

Common codes:

- `unauthorized`
- `validation_error`
- `conflict`
- `not_found`
- `server_error`

## Recommended Frontend Structure

- Keep auth state in a top-level React context or store
- Keep the access token in memory when possible
- Create one API client wrapper in `frontend/src/lib/api.ts`
- Split UI into:
  - login/register form
  - current week view
  - week summary header
  - time entry list
  - add/edit entry form

## Suggested Client Functions

```ts
register(email: string, password: string)
login(email: string, password: string)
logout(token: string)
getWeek(date?: string)
getEntries(workweekId: string)
createEntry(workweekId: string, payload: { entryDate: string; projectName: string; hours: number })
updateEntry(workweekId: string, entryId: string, payload: { hours: number })
deleteEntry(workweekId: string, entryId: string)
setFinalized(workweekId: string, isFinalized: boolean)
```

## Frontend Behavior Notes

- Fetch the current week on app load
- Then fetch the entries for the returned `workweekId`
- When the selected date changes, re-run `getWeek(date)` and then `getEntries(workweekId)`
- After create, update, delete, or finalize, refresh the entries and week metadata
- If any request returns `401`, clear local auth state and return the user to login

## Example Curl Commands

```bash
curl -X POST http://localhost:5020/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"alice@example.com","password":"passw0rd"}'
```

```bash
curl -X POST http://localhost:5020/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"alice@example.com","password":"passw0rd"}'
```

```bash
curl "http://localhost:5020/api/weeks?date=2026-03-27" \
  -H "Authorization: Bearer <token>"
```

## Environment Notes

- Frontend dev server origin must stay in backend CORS settings: `http://localhost:5173`
- Backend uses PostgreSQL on `localhost:5432`
- Backend uses Redis on `localhost:6379` for token revocation