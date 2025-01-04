using Ardalis.SmartEnum;
namespace Core.Constants;

public class GameStateType : SmartEnum<GameStateType, int>
{
    public static readonly GameStateType Dialog = new GameStateType(nameof(Dialog), 1);
    public static readonly GameStateType Fight = new GameStateType(nameof(Fight), 1);

    public GameStateType(string name, int value) : base(name, value)
    {
    }
}
