# Employee Benefits Tracker — Design Spec
**Date:** 2026-06-03  
**Status:** Approved

---

## 1. Project Goal

A full-stack CRUD application simulating an internal HR/payroll tool where users manage employees and track benefits enrollment. Built to demonstrate C#, ASP.NET Core, React, SQLite, xUnit, and GitHub Actions skills for internship applications.

No authentication — open internal dashboard.

---

## 2. Tech Stack

| Layer | Technology |
|---|---|
| Backend | C#, ASP.NET Core Web API (.NET 8) |
| ORM | Entity Framework Core + SQLite |
| Testing | xUnit, SQLite in-memory, WebApplicationFactory |
| Frontend | React 18, TypeScript, Vite |
| Styling | Tailwind CSS |
| Data fetching | Axios + React Query |
| CI/CD | GitHub Actions |

---

## 3. Project Structure

```
EmployeeBenefitsTracker/
├── backend/
│   ├── EmployeeBenefitsTracker.sln
│   ├── EmployeeBenefitsTracker.Api/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Repositories/
│   │   ├── Models/
│   │   ├── DTOs/
│   │   ├── Data/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   └── EmployeeBenefitsTracker.Tests/
│       ├── Services/
│       ├── Controllers/
│       └── TestHelpers/
├── frontend/
│   └── src/
│       ├── api/
│       ├── components/
│       ├── pages/
│       ├── types/
│       ├── utils/
│       ├── App.tsx
│       └── main.tsx
├── .github/
│   └── workflows/
│       └── ci.yml
└── README.md
```

---

## 4. Data Models

### Employee (entity)

| Field | Type | Notes |
|---|---|---|
| Id | int | Primary key |
| FirstName | string | Required |
| LastName | string | Required |
| Email | string | Required, unique |
| Department | string | Required |
| EmploymentStatus | enum | Active, Inactive |
| CreatedAt | DateTime | Set on create |
| UpdatedAt | DateTime | Updated on save |

### BenefitEnrollment (entity)

| Field | Type | Notes |
|---|---|---|
| Id | int | Primary key |
| EmployeeId | int | Foreign key → Employee |
| BenefitType | enum | Health, Dental, Vision, Retirement, Life |
| EnrollmentStatus | enum | NotEnrolled, Pending, Enrolled, Waived |
| EffectiveDate | DateTime? | Optional |
| Notes | string? | Optional |
| CreatedAt | DateTime | Set on create |
| UpdatedAt | DateTime | Updated on save |

**Relationship:** One Employee → many BenefitEnrollments. Cascade delete on employee removal.

**Database:** SQLite file at `backend/EmployeeBenefitsTracker.Api/data/benefits.db`. Created automatically via `EnsureCreated()` + seeder on first run. EF Core migrations are intentionally skipped for this portfolio project.

**Seed data:** 100+ mock employees with realistic benefit enrollment records.

---

## 5. DTOs

Each resource has separate DTOs — EF entities are never exposed directly through the API.

- `CreateEmployeeDto` — required fields + email
- `UpdateEmployeeDto` — same fields, all updatable
- `EmployeeResponseDto` — includes Id, all fields, CreatedAt, UpdatedAt
- `PagedResult<T>` — `{ items, page, pageSize, totalCount, totalPages }`
- `CreateBenefitEnrollmentDto`
- `UpdateBenefitEnrollmentDto`
- `BenefitEnrollmentResponseDto`
- `BenefitSummaryDto` — dashboard metrics

---

## 6. Backend Architecture

**Layers:**
- **Controllers** — HTTP routing, status codes, exception mapping
- **Services** — business logic, validation, orchestration
- **Repositories** — EF Core data access, queries, pagination
- **Data** — `AppDbContext`, seeder

**Exception mapping in controllers:**
- `ArgumentException` → `400 Bad Request`
- `NotFoundException` → `404 Not Found`

**Business validation in services:**
- Required field checks
- Email format validation
- Duplicate email check (case-insensitive) → `ArgumentException`
- Duplicate BenefitType per employee check → `ArgumentException` (one enrollment record per BenefitType per employee)
- Valid enum values for BenefitType and EnrollmentStatus

---

## 7. API Endpoints

### Employee Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/employees` | Paged list with filters |
| GET | `/api/employees/{id}` | Single employee |
| POST | `/api/employees` | Create employee → 201 |
| PUT | `/api/employees/{id}` | Update employee |
| DELETE | `/api/employees/{id}` | Delete employee → 204 |

**Query params for GET /api/employees:**
```
?page=1&pageSize=10&search=alice&department=Engineering&benefitStatus=Enrolled
```

**Paged response shape:**
```json
{ "items": [...], "page": 1, "pageSize": 10, "totalCount": 105, "totalPages": 11 }
```

### Benefit Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/employees/{id}/benefits` | All benefits for employee |
| POST | `/api/employees/{id}/benefits` | Add benefit enrollment → 201 |
| PUT | `/api/employees/{employeeId}/benefits/{benefitId}` | Update benefit status |
| DELETE | `/api/employees/{employeeId}/benefits/{benefitId}` | Remove benefit enrollment → 204 |

### Summary Endpoint

| Method | Route | Description |
|---|---|---|
| GET | `/api/benefits/summary` | Dashboard metrics |

**Summary response:**
```json
{
  "totalEmployees": 105,
  "activeEmployees": 87,
  "pendingEnrollments": 12,
  "enrolledByBenefitType": {
    "Health": 72, "Dental": 65, "Vision": 50, "Retirement": 80, "Life": 45
  },
  "overallEnrollmentRate": 0.68
}
```

---

## 8. Frontend Pages & Components

### Pages

| Route | Component | Description |
|---|---|---|
| `/` | EmployeeList | Table + summary cards + filters |
| `/employees/new` | EmployeeForm | Create mode |
| `/employees/:id` | EmployeeDetail | Info + benefits management |
| `/employees/:id/edit` | EmployeeForm | Edit mode (same component) |

### EmployeeList
- 5 summary stat cards: Total Employees, Active Employees, Health Insurance Enrolled, Pending Enrollments, Overall Enrollment Rate
- Searchable/filterable paginated table (search by name/email, filter by department, filter by benefit status)
- Delete button per row with confirmation
- Links to employee detail

### EmployeeDetail
- Employee info section
- Benefits enrollment table with Add/Update/Delete per enrollment
- Enrollment status shown as colored badge

### EmployeeForm (shared)
- Required field validation (client-side)
- Email format validation
- Duplicate email error surfaced from backend as form-level error

### Reusable components
- `Table` — sortable columns, loading/empty states
- `Badge` — colored status labels
- `Modal` — confirmation dialogs
- `SummaryCard` — metric display card
- Form field components (Input, Select, TextArea)

### State management
- **React Query** for all server state: fetching, caching, loading/error states, cache invalidation after mutations
- **No Redux or Zustand** — no global client state needed
- **Controlled components** for all forms

### API client
- Axios instance in `src/api/` using `VITE_API_BASE_URL` env variable (e.g., `http://localhost:5000`)
- Individual React Query hooks per resource: `useEmployees`, `useEmployee`, `useCreateEmployee`, `useUpdateEmployee`, `useDeleteEmployee`, `useEmployeeBenefits`, `useCreateBenefit`, `useUpdateBenefit`, `useDeleteBenefit`, `useBenefitSummary`

---

## 9. Testing

**Project:** `EmployeeBenefitsTracker.Tests` (separate `.csproj`, part of the solution)

**Database:** SQLite in-memory connection — not EF Core's in-memory provider. Used throughout to ensure relationships, cascade deletes, filtering, and aggregations behave as in production.

**No mocking of repository or DB layer** — all service tests use the real Service → Repository → EF Core → SQLite in-memory stack.

### Service Tests (EmployeeServiceTests, BenefitServiceTests)

1. Create employee successfully
2. Reject duplicate email → ArgumentException
3. Reject missing required fields → ArgumentException
4. Update employee successfully
5. Get employee returns NotFoundException for missing id
6. Update employee returns NotFoundException for missing id
7. Delete employee returns NotFoundException for missing id
8. Delete employee cascades benefit enrollments
9. Add benefit enrollment successfully
10. Update benefit status successfully
11. Reject invalid benefit type → ArgumentException
12. Reject invalid enrollment status → ArgumentException
13. Reject duplicate BenefitType for same employee → ArgumentException

### Controller Tests (WebApplicationFactory + test SQLite DB)

14. `GET /api/employees` returns 200 with paged result
15. `GET /api/employees` with search, department, and benefitStatus filters returns correct paged result
16. `POST /api/employees` returns 201 with created employee
17. `PUT /api/employees/{id}` on missing employee returns 404
18. `DELETE /api/employees/{id}` returns 204
19. `GET /api/benefits/summary` returns correct counts
20. `POST /api/employees/{id}/benefits` on missing employee returns 404
21. `PUT /api/employees/{employeeId}/benefits/{benefitId}` returns 404 when benefit does not belong to that employee

**Total: 21 tests** (exceeds the 15-test minimum)

---

## 10. CI/CD

**File:** `.github/workflows/ci.yml`  
**Triggers:** push and PR to `main`

**Two parallel jobs:**

```yaml
backend:
  - Checkout
  - Setup .NET 8
  - dotnet restore (solution file)
  - dotnet build --no-restore
  - dotnet test --no-build

frontend:
  - Checkout
  - Setup Node 20
  - npm ci
  - npm run build  # runs: tsc -b && vite build
```

`npm run build` validates TypeScript type-checking AND production bundling in CI.

---

## 11. README Sections

1. Project overview
2. Tech stack
3. Text-based architecture
4. Core features
5. Full API endpoint table
6. Database schema summary
7. Backend setup and run commands (`dotnet run`)
8. Frontend setup and run commands (`npm run dev`)
9. Test commands (`dotnet test`)
10. CI/CD explanation
11. Note: EF Core migrations intentionally skipped; `EnsureCreated()` used for simplicity in this portfolio project
12. Resume bullet examples

---

## 12. Resume Bullets (target)

- Built C# ASP.NET Core REST APIs for 5 benefits workflows using EF Core and SQLite persistence.
- Developed React TypeScript dashboard with search, pagination, filters, and React Query across 100+ records.
- Added 20+ xUnit tests covering CRUD, validation, cascade deletes, filtering, and API error paths.
- Configured GitHub Actions CI validating .NET tests and React TypeScript builds across 2 parallel jobs.
