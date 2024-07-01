using System.Text.Json;
using KomsterHambatAutoClicker.Models;
using Microsoft.Extensions.Options;

namespace KomsterHambatAutoClicker
{
    public class StateManager : IStateManager
    {
        private readonly string _stateFilePath;

        public StateManager(IOptions<StateManagerOptions> options)
        {
            _stateFilePath = options.Value.StateFilePath;
        }

        public void SaveState(ClickerUser user)
        {
            var stateJson = JsonSerializer.Serialize(user);
            File.WriteAllText(_stateFilePath, stateJson);
        }

        public ClickerUser LoadState()
        {
            if (!File.Exists(_stateFilePath))
            {
                throw new FileNotFoundException($"Can't find file with path: {_stateFilePath}");
            }
            
            var stateJson = File.ReadAllText(_stateFilePath);
            return JsonSerializer.Deserialize<ClickerUser>(stateJson) ??
                   throw new ArgumentNullException("Can't deserialize state file");
        }
    }
}