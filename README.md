# Employee Benefits Tracker

A full-stack HR tool for managing employee records and benefits enrollment status. Built with C#, ASP.NET Core, React TypeScript, and SQLite.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | C# / ASP.NET Core (.NET 10) / Entity Framework Core |
| Database | SQLite (auto-created, no migrations) |
| Testing | xUnit / WebApplicationFactory / SQLite in-memory |
| Frontend | React 18 / TypeScript / Vite / Tailwind CSS v4 |
| State | React Query v5 / Axios |
| Routing | React Router v6 |
| CI/CD | GitHub Actions |

## Architecture

```
EmployeeBenefitsTracker/
├── backend/
│   ├── EmployeeBenefitsTracker.Api/       # ASP.NET Core Web API
│   │   ├── Controllers/                    # HTTP layer — routes, status codes, exception mapping
│   │   ├── Services/                       # Business logic and validation
│   │   ├── Repositories/                   # EF Core data access
│   │   ├── Models/                         # Entity classes and enums
│   │   ├── DTOs/                           # Request/response shapes
│   │   └── Data/                           # AppDbContext and seeder
│   └── EmployeeBenefitsTracker.Tests/      # xUnit tests (24 total)
└── frontend/                               # Vite React TypeScript app
    └── src/
        ├── api/                            # Axios client + React Query hooks
        ├── components/                     # Badge, SummaryCard, Pagination, Modal, FormField
        ├── pages/                          # EmployeeList, EmployeeForm, EmployeeDetail
        ├── types/                          # Shared TypeScript interfaces
        └── utils/                          # formatters and constants
```

## Core Features

- **Employee CRUD** — create, view, edit, and delete employees
- **Benefits enrollment** — manage Health, Dental, Vision, Retirement, and Life Insurance per employee
- **Search and filters** — search by name/email, filter by department and enrollment status
- **Pagination** — 10 employees per page with page navigation
- **Dashboard summary** — total/active employees, pending enrollments, Health enrollment count, overall rate
- **Inline benefit management** — add, update status, and remove benefit enrollments from the detail page
- **105 seeded employees** with realistic benefit data for development

## API Endpoints

### Employee Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/employees` | Paged list (`?page=1&pageSize=10&search=&department=&benefitStatus=`) |
| GET | `/api/employees/{id}` | Single employee |
| POST | `/api/employees` | Create employee → 201 |
| PUT | `/api/employees/{id}` | Update employee |
| DELETE | `/api/employees/{id}` | Delete employee → 204 |

### Benefit Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/employees/{id}/benefits` | Employee's benefit enrollments |
| POST | `/api/employees/{id}/benefits` | Add benefit enrollment → 201 |
| PUT | `/api/employees/{empId}/benefits/{benefitId}` | Update benefit status |
| DELETE | `/api/employees/{empId}/benefits/{benefitId}` | Remove benefit enrollment → 204 |
| GET | `/api/benefits/summary` | Dashboard metrics |

## Database Schema

**Employees:** Id, FirstName, LastName, Email (unique, case-insensitive), Department, EmploymentStatus, CreatedAt, UpdatedAt

**BenefitEnrollments:** Id, EmployeeId (FK), BenefitType, EnrollmentStatus, EffectiveDate, Notes, CreatedAt, UpdatedAt

**Constraints:**
- Unique email index with NOCASE collation
- Unique composite index on (EmployeeId, BenefitType) — one enrollment per benefit type per employee
- Cascade delete: removing an employee removes all their benefit enrollments

> EF Core migrations are intentionally skipped for this portfolio project. The database is created automatically via `EnsureCreated()` on first run, with 105 seeded employees and realistic benefit enrollment data.

## Running the Backend

```bash
cd backend/EmployeeBenefitsTracker.Api
dotnet run
# API available at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger
```

## Running the Frontend

```bash
cd frontend
npm install
npm run dev
# App available at http://localhost:5173
```

## Running Tests

```bash
cd backend
dotnet test --verbosity normal
# 24 tests: 15 service tests (SQLite in-memory) + 9 controller tests (WebApplicationFactory)
```

## CI/CD

GitHub Actions runs two parallel jobs on every push and pull request to `main`:

**Backend job:**
1. Checkout
2. Setup .NET 10
3. `dotnet restore` — restore solution dependencies
4. `dotnet build --no-restore` — compile
5. `dotnet test --no-build` — run 24 xUnit tests

**Frontend job:**
1. Checkout
2. Setup Node 20 with npm cache
3. `npm ci` — install locked dependencies
4. `npm run build` — TypeScript type-check (`tsc -b`) then Vite production bundle

Both jobs must pass for a pull request to be considered mergeable.

## Resume Bullets

- Built C# ASP.NET Core REST APIs for 5 benefits workflows using EF Core and SQLite persistence.
- Developed React TypeScript dashboard with search, pagination, filters, and React Query across 100+ records.
- Added 20+ xUnit tests covering CRUD, validation, cascade deletes, filtering, and API error paths.
- Configured GitHub Actions CI validating .NET tests and React TypeScript builds across 2 parallel jobs.
