namespace CampaignHub.Application.Options;

public class MetaAdsOptions
{
    public const string SectionName = "MetaAds";

    public string? AppId { get; set; }
    public string? AppSecret { get; set; }
    public string BaseUrl { get; set; } = "https://graph.facebook.com/v21.0";
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
    public string? AccessToken { get; set; }
    public Dictionary<string, string>? AccountTokens { get; set; }
}
