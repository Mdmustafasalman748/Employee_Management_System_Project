namespace EmployeeManagement.Web.Data;

public static class DbProvider
{
    public static bool IsPostgres(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) return false;

        return IsPostgresUrl(connectionString)
            || connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase);
    }

    public static string ToNpgsql(string connectionString)
    {
        if (!IsPostgresUrl(connectionString)) return connectionString;

        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':', 2);
        var user = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        var ssl = uri.Query.Contains("sslmode=require", StringComparison.OrdinalIgnoreCase)
            ? ";SSL Mode=Require"
            : "";

        return $"Host={uri.Host};Port={port};Database={database};Username={user};Password={password}{ssl}";
    }

    private static bool IsPostgresUrl(string value) =>
        value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
        || value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase);
}