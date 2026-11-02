using EmployeeManagement.Web.Helpers;
using Xunit;

namespace EmployeeManagement.Tests;

public class UiTests
{
    [Fact]
    public void Initials_UsesFirstAndLastName()
    {
        Assert.Equal("AK", Ui.Initials("Aisha Khan"));
        Assert.Equal("MW", Ui.Initials("Mary Jane Watson"));
    }

    [Fact]
    public void Initials_HandlesSingleNameAndEmpty()
    {
        Assert.Equal("C", Ui.Initials("cher"));
        Assert.Equal("?", Ui.Initials(""));
        Assert.Equal("?", Ui.Initials(null));
    }

    [Fact]
    public void Hue_IsStableAndWithinRange()
    {
        var hue = Ui.Hue("Aisha Khan");

        Assert.Equal(hue, Ui.Hue("Aisha Khan"));
        Assert.InRange(hue, 0, 359);
    }
}