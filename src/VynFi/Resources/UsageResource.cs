using VynFi.Models;

namespace VynFi.Resources;

/// <summary>Usage resource — credit balance and daily usage.</summary>
public class UsageResource
{
    private readonly VynFiClient _client;
    internal UsageResource(VynFiClient client) => _client = client;

    /// <summary>Get usage summary (balance, totals, burn rate).</summary>
    public Task<UsageSummary> SummaryAsync(CancellationToken ct = default)
        => _client.RequestAsync<UsageSummary>(HttpMethod.Get, "/v1/usage/summary", ct);

    /// <summary>Get daily usage breakdown with optional day count.</summary>
    public Task<DailyUsageResponse> DailyAsync(int? days = null, CancellationToken ct = default)
    {
        var path = days.HasValue ? $"/v1/usage/daily?days={days.Value}" : "/v1/usage/daily";
        return _client.RequestAsync<DailyUsageResponse>(HttpMethod.Get, path, ct);
    }
}
