using Microsoft.Extensions.Hosting.Internal;

namespace KomsterHambatAutoClicker;

public class StateManagerOptions
{
    public const string Key = "StateManager";
    public virtual string StateFilePath { get; set; }
}