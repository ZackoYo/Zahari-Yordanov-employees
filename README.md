# Employees Work Tracker

A demo .NET 9 + Blazor Server application that identifies the pair of employees who worked together
on common projects for the longest period of time.

## Features
- Upload a CSV file (via Blazor UI or API).
- CSV parser supports:
  - Multiple date formats (`yyyy-MM-dd`, `dd/MM/yyyy`, `MM/dd/yyyy`, `dd.MM.yyyy`, etc.).
  - `NULL` or empty `DateTo` → treated as **today**.
  - Comma or semicolon delimiters.
- Calculates per-project overlaps and aggregates total days for each employee pair.
- Returns:
  - The top pair (Employee1, Employee2, TotalDaysTogether).
  - Breakdown by project.
  - Warnings for skipped/invalid rows.
- Simple Blazor Server UI with file upload, results table, and warnings.

## Architecture
- **Domain**: Pure models (`ProjectAssignment`, `DateRange`, `PairOverlapResult`, etc.).
- **Application**: Services for overlap calculation and CSV parsing.
- **API**: Minimal API endpoint (`POST /api/assignments/analyze`) for CSV upload and analysis.
- **Blazor**: UI to upload CSV and view results.

This follows a **Clean Architecture** approach:
- Domain and Application are independent of infrastructure.
- API and Blazor depend on Application.

## Run locally

### Requirements
- .NET 9 SDK

### Steps
```bash
# Clone
git clone https://github.com/ZackoYo/Zahari-Yordanov-employees.git
cd EmployeesWorkTracker

# Restore and build
dotnet build

# Run API (Swagger on https://localhost:5001/swagger or http://localhost:5000/swagger)
Ctrl+F5

# Run Blazor UI (default on https://localhost:5163)
dotnet run --project EmployeesWorkTracker.Blazor
