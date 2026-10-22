using EmployeeManagement.Web.Models;

namespace EmployeeManagement.Web.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Employees.Any()) return;   // only seed an empty table

        var names = new (string Name, int Dept, decimal Salary)[]
        {
            ("Aisha Khan", 1, 6000), ("Rahul Mehta", 1, 5500), ("Sara Ali", 2, 4800),
            ("Daniel Brown", 1, 5200), ("Priya Nair", 2, 4600), ("Omar Farouk", 1, 6400),
            ("Emily Clark", 2, 4900), ("Kenji Sato", 1, 7000), ("Lina Petrov", 2, 4700),
            ("Carlos Diaz", 1, 5800), ("Fatima Noor", 2, 5000), ("Tom Wright", 1, 5300)
        };

        var joined = new DateTime(2024, 1, 15);
        db.Employees.AddRange(names.Select((n, i) => new Employee
        {
            FullName = n.Name,
            Email = n.Name.ToLower().Replace(" ", ".") + "@company.com",
            DepartmentId = n.Dept,
            MonthlySalary = n.Salary,
            JoinedOn = joined.AddMonths(i)
        }));
        db.SaveChanges();
    }
}