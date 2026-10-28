# Employee Management System

![CI](https://github.com/<your-username>/employee-management-system/actions/workflows/ci.yml/badge.svg)

An ASP.NET Core MVC application for managing employees and departments.

## Features
- Employee and department CRUD with validation
- Search and paging
- Delete protection for departments that still have employees
- Unit tests and a GitHub Actions CI pipeline

## Tech stack
C#, ASP.NET Core MVC (.NET 10), Entity Framework Core, SQLite, xUnit, GitHub Actions

## Run locally
    dotnet run --project EmployeeManagement.Web

## Run tests
    dotnet test

**Live demo:** https://<your-service>.onrender.com

Demo login: `demo@company.com` / `Demo@12345` (can add and edit; delete is admin-only)

> Hosted on a free tier: if the site has been idle, the first load can take about a minute while it wakes up.
