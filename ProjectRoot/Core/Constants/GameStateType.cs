using Ardalis.SmartEnum;
namespace Core.Constants;

public class GameStateType(string name, int value) : SmartEnum<GameStateType, int>(name, value)
{
    public static readonly GameStateType Dialog = new(nameof(Dialog), 1);
    public static readonly GameStateType Fight = new(nameof(Fight), 2);
}
