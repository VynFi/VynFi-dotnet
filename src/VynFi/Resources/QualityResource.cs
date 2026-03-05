using VynFi.Models;

namespace VynFi.Resources;

/// <summary>Quality metrics resource.</summary>
public class QualityResource
{
    private readonly VynFiClient _client;
    internal QualityResource(VynFiClient client) => _client = client;

    /// <summary>Get quality scores for recent jobs.</summary>
    public Task<List<QualityScore>> ScoresAsync(CancellationToken ct = default)
        => _client.RequestListAsync<QualityScore>(HttpMethod.Get, "/v1/quality/scores", ct);

    /// <summary>Get daily quality score timeline.</summary>
    public Task<List<DailyQuality>> TimelineAsync(int? days = null, CancellationToken ct = default)
    {
        var path = days.HasValue ? $"/v1/quality/timeline?days={days.Value}" : "/v1/quality/timeline";
        return _client.RequestListAsync<DailyQuality>(HttpMethod.Get, path, ct);
    }
}
