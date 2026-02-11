using System.Net.Http.Json;
using System.Text.Json;
using CampaignHub.Application.DTOs.Meta;
using CampaignHub.Application.Interfaces;
using CampaignHub.Application.Options;
using Microsoft.Extensions.Options;

namespace CampaignHub.Infra.ExternalServices.Meta;

public class MetaAdsApiClient : IMetaAdsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly MetaAdsOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public MetaAdsApiClient(HttpClient httpClient, IOptions<MetaAdsOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<MetaCampaignDto>> GetCampaignsAsync(string adAccountExternalId, CancellationToken ct = default)
    {
        var url = $"{_options.BaseUrl}/{adAccountExternalId}/campaigns?fields=id,name,status,start_time,stop_time&limit=100";
        return await FetchAllPagesAsync<MetaCampaignDto>(url, ct);
    }

    public async Task<IReadOnlyList<MetaAdSetDto>> GetAdSetsAsync(string campaignExternalId, CancellationToken ct = default)
    {
        var url = $"{_options.BaseUrl}/{campaignExternalId}/adsets?fields=id,campaign_id,name,status,daily_budget&limit=100";
        return await FetchAllPagesAsync<MetaAdSetDto>(url, ct);
    }

    public async Task<IReadOnlyList<MetaAdDto>> GetAdsAsync(string adSetExternalId, CancellationToken ct = default)
    {
        var url = $"{_options.BaseUrl}/{adSetExternalId}/ads?fields=id,adset_id,name,status&limit=100";
        return await FetchAllPagesAsync<MetaAdDto>(url, ct);
    }

    public async Task<IReadOnlyList<MetaInsightDto>> GetCampaignInsightsAsync(string adAccountExternalId, DateOnly since, DateOnly until, CancellationToken ct = default)
    {
        var url = $"{_options.BaseUrl}/{adAccountExternalId}/insights?fields=campaign_id,date_start,date_stop,spend,actions&level=campaign&time_increment=monthly&time_range={{\"since\":\"{since:yyyy-MM-dd}\",\"until\":\"{until:yyyy-MM-dd}\"}}&limit=100";
        var rawInsights = await FetchAllPagesAsync<JsonElement>(url, ct);

        var results = new List<MetaInsightDto>();
        foreach (var raw in rawInsights)
        {
            var campaignId = raw.GetProperty("campaign_id").GetString() ?? "";
            var dateStart = raw.GetProperty("date_start").GetString() ?? "";
            var dateStop = raw.GetProperty("date_stop").GetString() ?? "";
            var spend = raw.TryGetProperty("spend", out var spendEl) ? spendEl.GetString() : null;

            int leads = 0;
            int purchases = 0;
            string? purchaseValue = null;

            if (raw.TryGetProperty("actions", out var actionsEl) && actionsEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var action in actionsEl.EnumerateArray())
                {
                    var actionType = action.GetProperty("action_type").GetString();
                    var value = action.TryGetProperty("value", out var valEl) ? valEl.GetString() : "0";

                    if (actionType == "lead")
                        int.TryParse(value, out leads);
                    else if (actionType == "purchase" || actionType == "omni_purchase")
                    {
                        int.TryParse(value, out purchases);
                    }
                }
            }

            if (raw.TryGetProperty("action_values", out var actionValuesEl) && actionValuesEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var av in actionValuesEl.EnumerateArray())
                {
                    var actionType = av.GetProperty("action_type").GetString();
                    if (actionType == "purchase" || actionType == "omni_purchase")
                        purchaseValue = av.TryGetProperty("value", out var pvEl) ? pvEl.GetString() : null;
                }
            }

            results.Add(new MetaInsightDto(campaignId, dateStart, dateStop, spend, leads, purchases, purchaseValue));
        }

        return results;
    }

    private async Task<List<T>> FetchAllPagesAsync<T>(string url, CancellationToken ct)
    {
        var allItems = new List<T>();
        string? nextUrl = url;

        while (!string.IsNullOrEmpty(nextUrl))
        {
            var response = await _httpClient.GetAsync(nextUrl, ct);
            response.EnsureSuccessStatusCode();

            var page = await response.Content.ReadFromJsonAsync<MetaPaginatedResponse<T>>(JsonOptions, ct);
            if (page?.Data is not null)
                allItems.AddRange(page.Data);

            nextUrl = page?.Paging?.Next;
        }

        return allItems;
    }
}
