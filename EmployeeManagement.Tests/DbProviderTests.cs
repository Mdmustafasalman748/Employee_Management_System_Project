using EmployeeManagement.Web.Data;
using Xunit;

namespace EmployeeManagement.Tests;

public class DbProviderTests
{
    [Fact]
    public void IsPostgres_False_ForSqliteConnectionString()
    {
        Assert.False(DbProvider.IsPostgres("Data Source=employees.db"));
    }

    [Fact]
    public void IsPostgres_True_ForNpgsqlConnectionString()
    {
        Assert.True(DbProvider.IsPostgres("Host=localhost;Database=hr;Username=u;Password=p"));
    }

    [Fact]
    public void IsPostgres_True_ForPostgresUrl()
    {
        Assert.True(DbProvider.IsPostgres("postgres://u:p@db.example.com:5432/hr"));
    }

    [Fact]
    public void ToNpgsql_ConvertsUrlToKeyValueFormat()
    {
        var result = DbProvider.ToNpgsql("postgres://alice:s%40cret@db.example.com:6543/hr?sslmode=require");

        Assert.Equal(
            "Host=db.example.com;Port=6543;Database=hr;Username=alice;Password=s@cret;SSL Mode=Require",
            result);
    }

    [Fact]
    public void ToNpgsql_LeavesKeyValueStringsUnchanged()
    {
        const string cs = "Host=localhost;Database=hr;Username=u;Password=p";

        Assert.Equal(cs, DbProvider.ToNpgsql(cs));
    }
}