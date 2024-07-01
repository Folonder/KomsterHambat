using System.Text.Json.Serialization;

namespace KomsterHambatAutoClicker.Models;

public class SyncResponse
{
    [JsonPropertyName("clickerUser")]
    public ClickerUser ClickerUser { get; set; }
}