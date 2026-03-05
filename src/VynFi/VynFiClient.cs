using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VynFi.Models;
using VynFi.Resources;

namespace VynFi;

/// <summary>VynFi API client for synthetic financial data generation.</summary>
public class VynFiClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly int _maxRetries;
    private bool _disposed;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>Jobs resource — submit, list, get, cancel, download generation jobs.</summary>
    public JobsResource Jobs { get; }
    /// <summary>Catalog resource — list sectors, tables, fingerprints.</summary>
    public CatalogResource Catalog { get; }
    /// <summary>Usage resource — credit balance and daily usage.</summary>
    public UsageResource Usage { get; }
    /// <summary>API key management resource.</summary>
    public ApiKeysResource ApiKeys { get; }
    /// <summary>Quality metrics resource.</summary>
    public QualityResource Quality { get; }
    /// <summary>Webhooks resource — CRUD and testing.</summary>
    public WebhooksResource Webhooks { get; }
    /// <summary>Billing resource — subscription, invoices, payment methods.</summary>
    public BillingResource Billing { get; }

    public VynFiClient(string apiKey, string baseUrl = "https://api.vynfi.com", int maxRetries = 2, TimeSpan? timeout = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key is required.", nameof(apiKey));

        _baseUrl = baseUrl.TrimEnd('/');
        _maxRetries = maxRetries;

        _http = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(30) };
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("vynfi-dotnet/0.1.0");
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        Jobs = new JobsResource(this);
        Catalog = new CatalogResource(this);
        Usage = new UsageResource(this);
        ApiKeys = new ApiKeysResource(this);
        Quality = new QualityResource(this);
        Webhooks = new WebhooksResource(this);
        Billing = new BillingResource(this);
    }

    // -- Internal request methods used by resources --

    internal async Task<T> RequestAsync<T>(HttpMethod method, string path, CancellationToken ct = default)
    {
        return await RequestAsync<T>(method, path, body: (object?)null, ct).ConfigureAwait(false);
    }

    internal async Task<T> RequestAsync<T>(HttpMethod method, string path, object? body, CancellationToken ct = default)
    {
        var url = $"{_baseUrl}{path}";
        Exception? lastException = null;

        for (int attempt = 0; attempt <= _maxRetries; attempt++)
        {
            using var request = new HttpRequestMessage(method, url);
            if (body is not null)
            {
                var json = JsonSerializer.Serialize(body, JsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response;
            try
            {
                response = await _http.SendAsync(request, ct).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
                if (attempt < _maxRetries)
                {
                    await Task.Delay(Backoff(attempt), ct).ConfigureAwait(false);
                    continue;
                }
                throw new VynFiException($"HTTP request failed: {ex.Message}");
            }

            if (ShouldRetry(response.StatusCode) && attempt < _maxRetries)
            {
                response.Dispose();
                await Task.Delay(Backoff(attempt), ct).ConfigureAwait(false);
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync(
#if NET8_0_OR_GREATER
                    ct
#endif
                ).ConfigureAwait(false);
                response.Dispose();
                throw MapException(response.StatusCode, errorJson);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                response.Dispose();
                return default!;
            }

            var responseJson = await response.Content.ReadAsStringAsync(
#if NET8_0_OR_GREATER
                ct
#endif
            ).ConfigureAwait(false);
            response.Dispose();
            return JsonSerializer.Deserialize<T>(responseJson, JsonOptions)!;
        }

        throw new VynFiException("Max retries exceeded", body: null);
    }

    internal async Task RequestVoidAsync(HttpMethod method, string path, CancellationToken ct = default)
    {
        await RequestAsync<object?>(method, path, body: (object?)null, ct).ConfigureAwait(false);
    }

    internal async Task<List<T>> RequestListAsync<T>(HttpMethod method, string path, CancellationToken ct = default)
    {
        var value = await RequestAsync<JsonElement>(method, path, ct).ConfigureAwait(false);
        return ExtractList<T>(value);
    }

    internal async Task<byte[]> RequestBytesAsync(HttpMethod method, string path, CancellationToken ct = default)
    {
        var url = $"{_baseUrl}{path}";
        using var request = new HttpRequestMessage(method, url);
        var response = await _http.SendAsync(request, ct).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync(
#if NET8_0_OR_GREATER
                ct
#endif
            ).ConfigureAwait(false);
            throw MapException(response.StatusCode, errorJson);
        }
        return await response.Content.ReadAsByteArrayAsync(
#if NET8_0_OR_GREATER
            ct
#endif
        ).ConfigureAwait(false);
    }

    internal static JsonSerializerOptions GetJsonOptions() => JsonOptions;

    private static List<T> ExtractList<T>(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Array)
            return JsonSerializer.Deserialize<List<T>>(value.GetRawText(), JsonOptions) ?? new();

        if (value.ValueKind == JsonValueKind.Object)
        {
            if (value.TryGetProperty("data", out var dataArr) && dataArr.ValueKind == JsonValueKind.Array)
                return JsonSerializer.Deserialize<List<T>>(dataArr.GetRawText(), JsonOptions) ?? new();

            foreach (var prop in value.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Array)
                    return JsonSerializer.Deserialize<List<T>>(prop.Value.GetRawText(), JsonOptions) ?? new();
            }
        }

        return new();
    }

    private static bool ShouldRetry(HttpStatusCode status)
        => status == (HttpStatusCode)429 || (int)status >= 500;

    private static TimeSpan Backoff(int attempt)
        => TimeSpan.FromMilliseconds(500 * Math.Pow(2, attempt));

    private static VynFiException MapException(HttpStatusCode status, string body)
    {
        ErrorBody? errorBody = null;
        string message;
        try
        {
            errorBody = JsonSerializer.Deserialize<ErrorBody>(body, JsonOptions);
            message = !string.IsNullOrEmpty(errorBody?.Detail) ? errorBody!.Detail
                     : !string.IsNullOrEmpty(errorBody?.Message) ? errorBody!.Message
                     : $"HTTP {(int)status}";
        }
        catch
        {
            message = $"HTTP {(int)status}";
        }

        return status switch
        {
            HttpStatusCode.Unauthorized => new AuthenticationException(message, errorBody),
            HttpStatusCode.PaymentRequired => new InsufficientCreditsException(message, errorBody),
            HttpStatusCode.NotFound => new NotFoundException(message, errorBody),
            HttpStatusCode.Conflict => new ConflictException(message, errorBody),
            (HttpStatusCode)422 => new ValidationException(message, errorBody),
            (HttpStatusCode)429 => new RateLimitException(message, errorBody),
            _ when (int)status >= 500 => new ServerException(message, errorBody),
            _ => new VynFiException(message, (int)status, errorBody),
        };
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _http.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
