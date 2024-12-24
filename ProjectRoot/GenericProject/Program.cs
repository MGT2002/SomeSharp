using GenericProject;

ComponentState current = ComponentState.New;

switch (current.Name)
{
    case nameof(ComponentState.New):
        break;
    case nameof(ComponentState.Done):
        break;
    default:
		break;
}

public static class Constatns
{
    public static readonly PageInfo AdminPage = new(nameof(AdminPage), "/admin", "Admin page");
    public static readonly PageInfo EmployeePage = new("E", "/admin", "Admin page");

    public record PageInfo(string Name, string Url, string AdditionalInfo);
}