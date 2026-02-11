using System.Net;
using CampaignHub.Application.Exceptions;

namespace CampaignHub.Application.Tests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void EntityNotFoundException_ShouldSetProperties()
    {
        var ex = new EntityNotFoundException("User", "123");

        Assert.Equal("User", ex.EntityName);
        Assert.Equal("123", ex.Id);
        Assert.Contains("User", ex.Message);
        Assert.Contains("123", ex.Message);
    }

    [Fact]
    public void EntityNotFoundException_MessageFormat()
    {
        var ex = new EntityNotFoundException("Campaign", "abc-def");

        Assert.Equal("Campaign with id 'abc-def' was not found.", ex.Message);
    }

    [Fact]
    public void MetaApiException_ShouldSetProperties()
    {
        var ex = new MetaApiException("Rate limit exceeded", 32, 2446079, HttpStatusCode.TooManyRequests);

        Assert.Equal("Rate limit exceeded", ex.Message);
        Assert.Equal(32, ex.ErrorCode);
        Assert.Equal(2446079, ex.ErrorSubcode);
        Assert.Equal(HttpStatusCode.TooManyRequests, ex.HttpStatusCode);
    }

    [Fact]
    public void MetaApiException_ShouldUseDefaults()
    {
        var ex = new MetaApiException("Error", 100);

        Assert.Null(ex.ErrorSubcode);
        Assert.Equal(HttpStatusCode.BadRequest, ex.HttpStatusCode);
    }
}
