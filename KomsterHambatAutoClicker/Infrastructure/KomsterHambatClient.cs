using System.Net.Http.Json;
using KomsterHambatAutoClicker.Models;
using Microsoft.Extensions.Options;

namespace KomsterHambatAutoClicker.Infrastructure;

public class KomsterHambatClient : IKomsterHambatClient
{
    private readonly HttpClient _httpClient;

    public KomsterHambatClient(IOptions<KomsterHambatClientOptions> settings)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(settings.Value.BaseAddress) };
        _httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {settings.Value.Token}");
    }
    
    
    public async Task<ClickerUser> SynchronizeUserAsync()
    {
        var response = await _httpClient.PostAsync("clicker/sync", null);
        response.EnsureSuccessStatusCode();
        var syncResponse = await response.Content.ReadFromJsonAsync<SyncResponse>();
        var user = syncResponse.ClickerUser;
        return user;
    }

    public async Task<ClickerUser> TapAsync(TapRequest tapRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("clicker/tap", tapRequest);
        response.EnsureSuccessStatusCode();

        var tapResponse = await response.Content.ReadFromJsonAsync<TapResponse>();
        var user = tapResponse.ClickerUser;
        return user;
    }
}