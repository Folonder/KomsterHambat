using KomsterHambatAutoClicker.Models;

namespace KomsterHambatAutoClicker;

public interface IStateManager
{
    public void SaveState(ClickerUser user);

    public ClickerUser LoadState();
}