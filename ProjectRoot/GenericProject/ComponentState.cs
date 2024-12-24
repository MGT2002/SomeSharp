using Ardalis.SmartEnum;

namespace GenericProject;

public class ComponentState : SmartEnum<ComponentState, int>
{
    public static readonly ComponentState New = new(nameof(New).ToLower(), 1);
    public static readonly ComponentState InProgress = new(nameof(InProgress).ToLower(), 2);
    public static readonly ComponentState Done = new(nameof(Done).ToLower(), 3);

    private ComponentState(string name, int value) : base(name, value) { }
}
