using EmployeeManagement.Web.Models;
using EmployeeManagement.Web.Services;
using System.Collections.Generic;
using System;
using Xunit;

namespace EmployeeManagement.Tests;

public class CsvExporterTests
{
    [Fact]
    public void Escape_QuotesFieldsContainingCommas()
    {
        Assert.Equal("\"Smith, John\"", CsvExporter.Escape("Smith, John"));
    }

    [Fact]
    public void Escape_DoublesEmbeddedQuotes()
    {
        Assert.Equal("\"say \"\"hi\"\"\"", CsvExporter.Escape("say \"hi\""));
    }

    [Fact]
    public void Escape_NeutralisesFormulaCharacters()
    {
        Assert.Equal("'=1+1", CsvExporter.Escape("=1+1"));
    }

    [Fact]
    public void Escape_NullBecomesEmpty()
    {
        Assert.Equal("", CsvExporter.Escape(null));
    }

    [Fact]
    public void Build_WritesHeaderAndOneRowPerEmployee()
    {
        var employees = new List<Employee>
        {
            new()
            {
                Id = 1, FullName = "Ann", Email = "ann@test.com",
                Department = new Department { Name = "IT", Location = "Block A" },
                MonthlySalary = 1500.5m, JoinedOn = new DateTime(2025, 3, 9)
            }
        };

        var lines = CsvExporter.Build(employees)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        Assert.Equal("Id,Full Name,Email,Department,Monthly Salary,Joined On", lines[0]);
        Assert.Equal("1,Ann,ann@test.com,IT,1500.50,2025-03-09", lines[1]);
    }
}