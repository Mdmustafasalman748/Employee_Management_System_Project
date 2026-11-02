namespace EmployeeManagement.Web.Helpers;

public static class Ui
{
    public static string Initials(string? name)
    {
        var parts = (name ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0) return "?";
        if (parts.Length == 1) return parts[0][..1].ToUpperInvariant();
        return (parts[0][..1] + parts[^1][..1]).ToUpperInvariant();
    }

    public static int Hue(string? name)
    {
        var hue = 0;
        foreach (var c in name ?? "") hue = (hue * 31 + c) % 360;
        return hue;
    }

    public static string AvatarStyle(string? name)
    {
        var h = Hue(name);
        return $"background:hsl({h} 45% 88%);color:hsl({h} 60% 24%)";
    }
}