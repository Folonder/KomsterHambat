using System.Text.Json.Serialization;

namespace KomsterHambatAutoClicker.Models;

public class TapRequest
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("availableTaps")]
    public int AvailableTaps { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }
}