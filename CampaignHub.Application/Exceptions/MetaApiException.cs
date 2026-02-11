using System.Net;

namespace CampaignHub.Application.Exceptions;

public class MetaApiException : Exception
{
    public int ErrorCode { get; }
    public int? ErrorSubcode { get; }
    public HttpStatusCode HttpStatusCode { get; }

    public MetaApiException(string message, int errorCode, int? errorSubcode = null, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        ErrorCode = errorCode;
        ErrorSubcode = errorSubcode;
        HttpStatusCode = httpStatusCode;
    }
}
