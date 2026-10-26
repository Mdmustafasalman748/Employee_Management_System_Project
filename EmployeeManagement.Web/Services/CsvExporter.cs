using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using EmployeeManagement.Web.Models;

namespace EmployeeManagement.Web.Services;

public static class CsvExporter
{
    public static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";

        var v = value;

        // Stop spreadsheet formulas from running
        if ("=+-@\t\r".IndexOf(v[0]) >= 0) v = "'" + v;

        // Quote fields that contain commas, quotes, or line breaks
        if (v.Contains(',') || v.Contains('"') || v.Contains('\n') || v.Contains('\r'))
            v = "\"" + v.Replace("\"", "\"\"") + "\"";

        return v;
    }

    public static string Build(IEnumerable<Employee> employees)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,Full Name,Email,Department,Monthly Salary,Joined On");

        foreach (var e in employees)
        {
            sb.AppendLine(string.Join(",",
                e.Id,
                Escape(e.FullName),
                Escape(e.Email),
                Escape(e.Department?.Name),
                e.MonthlySalary.ToString("0.00", CultureInfo.InvariantCulture),
                e.JoinedOn.ToString("yyyy-MM-dd")));
        }

        return sb.ToString();
    }
}