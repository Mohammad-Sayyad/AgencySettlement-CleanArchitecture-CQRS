//using AgencySettlement.Application.Abstractions.External;
//using AgencySettlement.Application.Common;
//using Microsoft.Extensions.Options;
//using System.Net;
//using System.Net.Http.Json;
//using System.Text.Json;

//namespace AgencySettlement.Infrastructure.External;

//public sealed class ExternalExamApiOptions
//{
//    public string BaseUrl { get; set; } = "http://localhost:5022/";
//    public string ExamsPath { get; set; } = "api/FakeApi";
//    public int TimeoutSeconds { get; set; } = 60;
//}

//public sealed class ExternalExamApi(
//    HttpClient httpClient,
//    IOptions<ExternalExamApiOptions> options)
//    : IExternalExamApi
//{
//    public async Task<IReadOnlyList<ExternalExamItem>> GetExamsAsync(
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            using var response = await httpClient.GetAsync(
//                options.Value.ExamsPath,
//                cancellationToken);

//            if (!response.IsSuccessStatusCode)
//            {
//                throw new ExternalServiceException(
//                    "EXTERNAL_API_HTTP_ERROR",
//                    $"External exam API returned HTTP {(int)response.StatusCode}.",
//                    (int)response.StatusCode);
//            }

//            var payload = await response.Content.ReadFromJsonAsync<JsonElement>(
//                cancellationToken: cancellationToken);

//            if (payload.ValueKind == JsonValueKind.Array)
//                return payload.Deserialize<List<ExternalExamItem>>() ?? [];

//            if (payload.ValueKind == JsonValueKind.Object)
//            {
//                if (payload.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
//                    return data.Deserialize<List<ExternalExamItem>>() ?? [];

//                if (payload.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array)
//                    return items.Deserialize<List<ExternalExamItem>>() ?? [];
//            }

//            throw new ExternalServiceException(
//                "EXTERNAL_API_INVALID_RESPONSE",
//                "ساختار پاسخ API خارجی باید شامل لیست رکوردهای آزمون باشد.",
//                (int)HttpStatusCode.BadGateway);
//        }
//        catch (ExternalServiceException)
//        {
//            throw;
//        }
//        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
//        {
//            throw new ExternalServiceException(
//                "EXTERNAL_API_TIMEOUT",
//                "دریافت اطلاعات از API خارجی با timeout مواجه شد.",
//                (int)HttpStatusCode.GatewayTimeout);
//        }
//        catch (HttpRequestException ex)
//        {
//            throw new ExternalServiceException(
//                "EXTERNAL_API_UNAVAILABLE",
//                "API خارجی در دسترس نیست.",
//                (int)HttpStatusCode.BadGateway,
//                ex);
//        }
//        catch (JsonException ex)
//        {
//            throw new ExternalServiceException(
//                "EXTERNAL_API_INVALID_RESPONSE",
//                "پاسخ API خارجی قابل پردازش نیست.",
//                (int)HttpStatusCode.BadGateway,
//                ex);
//        }
//    }
//}


using AgencySettlement.Application.Abstractions.External;
using AgencySettlement.Application.Common;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AgencySettlement.Infrastructure.External;

public sealed class ExternalExamApiOptions
{
    public string BaseUrl { get; set; } = "http://localhost:5022/";
    public string ExamsPath { get; set; } = "api/FakeApi";
    public int TimeoutSeconds { get; set; } = 60;
}

public sealed class ExternalExamApi(
    HttpClient httpClient,
    IOptions<ExternalExamApiOptions> options)
    : IExternalExamApi
{
    public async Task<IReadOnlyList<ExternalExamGroup>> GetExamsAsync(
    CancellationToken cancellationToken = default)
    {
        try
        {
            var result =
                await httpClient.GetFromJsonAsync<List<ExternalExamGroup>>(
                    options.Value.ExamsPath,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    },
                    cancellationToken);

            return result ?? [];
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new ExternalServiceException(
                "EXTERNAL_API_TIMEOUT",
                "دریافت اطلاعات از API خارجی با timeout مواجه شد.",
                (int)HttpStatusCode.GatewayTimeout);
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalServiceException(
                "EXTERNAL_API_UNAVAILABLE",
                "API خارجی در دسترس نیست.",
                (int)HttpStatusCode.BadGateway,
                ex);
        }
        catch (JsonException ex)
        {
            throw new ExternalServiceException(
                "EXTERNAL_API_INVALID_RESPONSE",
                "پاسخ API خارجی قابل پردازش نیست.",
                (int)HttpStatusCode.BadGateway,
                ex);
        }
    }

    
}
