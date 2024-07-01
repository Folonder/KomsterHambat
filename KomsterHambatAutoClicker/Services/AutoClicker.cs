using KomsterHambatAutoClicker.Infrastructure;
using KomsterHambatAutoClicker.Models;
using Microsoft.Extensions.Logging;

namespace KomsterHambatAutoClicker
{
    public class AutoClicker
    {
        private readonly ILogger<AutoClicker> _logger;
        private readonly IKomsterHambatClient _komsterHambatClient;
        private readonly IStateManager _stateManager;
        private ClickerUser _user;
        private static readonly Random Random = new();

        public AutoClicker(ILogger<AutoClicker> logger, IKomsterHambatClient komsterHambatClient, IStateManager stateManager)
        {
            _logger = logger;
            _komsterHambatClient = komsterHambatClient;
            _stateManager = stateManager;
            _logger.LogInformation("Loading state...");
            LoadState();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await SynchronizeUserAsync();
                    var taps = await ImitateTaps();
                    await TapAsync(taps);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogInformation("Retrying in 5 seconds...");
                    await Task.Delay(5000, cancellationToken);
                    
                }
            }
            
            _logger.LogInformation("Saving state...");
            SaveState();
        }

        private async Task SynchronizeUserAsync()
        {
            try
            {
                _user = await _komsterHambatClient.SynchronizeUserAsync();

                _logger.LogInformation($"Synced: BalanceCoins={_user.BalanceCoins}, AvailableTaps={_user.AvailableTaps}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Sync request failed: {ex.Message}");
                throw;
            }
        }

        private async Task TapAsync(int count)
        {
            var tapRequest = new TapRequest
            {
                Count = count,
                AvailableTaps = _user.AvailableTaps - count,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            try
            {
                _user = await _komsterHambatClient.TapAsync(tapRequest);
                _logger.LogInformation($"Tapped: EarnedCoins={count * _user.EarnPerTap}, NewBalance={_user.BalanceCoins}, RemainingTaps={_user.AvailableTaps}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Tap request failed: {ex.Message}");
            }
        }

        private async Task<int> ImitateTaps()
        {
            var delay = Random.Next(2000, 4001);
            // Иммитируем задержку для нажатий
            await Task.Delay(delay);
            // Оптимальное количество кликов за период задержки, чтобы возможные клики успели восстановиться 
            try
            {
                return (delay / 1000) * _user.TapsRecoverPerSec / _user.EarnPerTap;
            }
            catch (DivideByZeroException ex)
            {
                throw new DivideByZeroException("Field 'earnPerTap' can't be null");
            }
        }

        private void SaveState()
        {
            try
            {
                _stateManager.SaveState(_user);
                _logger.LogInformation("State saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving state: {ex.Message}");
            }
        }

        private void LoadState()
        {
            try
            {
                _user = _stateManager.LoadState();
                _logger.LogInformation("State loaded successfully.");

            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
