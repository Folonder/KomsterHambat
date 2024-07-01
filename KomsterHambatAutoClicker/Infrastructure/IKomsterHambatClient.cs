using KomsterHambatAutoClicker.Models;

namespace KomsterHambatAutoClicker.Infrastructure;

public interface IKomsterHambatClient
{
    Task<ClickerUser> SynchronizeUserAsync();
    Task<ClickerUser> TapAsync(TapRequest tapRequest);
}