using CampaignHub.Application.Options;
using Microsoft.Extensions.Options;

namespace CampaignHub.Infra.ExternalServices.Meta;

public class MetaTokenDelegatingHandler : DelegatingHandler
{
    private readonly MetaAdsOptions _options;

    public MetaTokenDelegatingHandler(IOptions<MetaAdsOptions> options)
    {
        _options = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _options.AccessToken;
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Meta Ads access token is not configured. Set MetaAds:AccessToken in User Secrets or environment variables.");

        var uriBuilder = new UriBuilder(request.RequestUri!);
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["access_token"] = token;
        uriBuilder.Query = query.ToString();
        request.RequestUri = uriBuilder.Uri;

        return await base.SendAsync(request, cancellationToken);
    }
}
