using EmployeeManagement.Web.Models;
using System;

namespace EmployeeManagement.Web.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Employees.Any()) return;   // only seed an empty table

        db.Employees.AddRange(
            new Employee { FullName = "Aisha Khan", Email = "aisha.khan@company.com", DepartmentId = 1, MonthlySalary = 6000, JoinedOn = new DateTime(2024, 1, 15) },
            new Employee { FullName = "Rahul Mehta", Email = "rahul.mehta@company.com", DepartmentId = 1, MonthlySalary = 5500, JoinedOn = new DateTime(2024, 6, 3) },
            new Employee { FullName = "Sara Ali", Email = "sara.ali@company.com", DepartmentId = 2, MonthlySalary = 4800, JoinedOn = new DateTime(2025, 2, 20) }
        );
        db.SaveChanges();
    }
}