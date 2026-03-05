using System.Text.Json;
using VynFi.Models;

namespace VynFi.Resources;

/// <summary>Jobs resource — submit, list, get, cancel, download generation jobs.</summary>
public class JobsResource
{
    private readonly VynFiClient _client;
    internal JobsResource(VynFiClient client) => _client = client;

    /// <summary>Submit a new generation job (async — poll for completion).</summary>
    public Task<SubmitJobResponse> GenerateAsync(GenerateRequest request, CancellationToken ct = default)
        => _client.RequestAsync<SubmitJobResponse>(HttpMethod.Post, "/v1/generate", request, ct);

    /// <summary>Submit a quick generation job (blocks until complete for small jobs).</summary>
    public Task<Job> GenerateQuickAsync(GenerateRequest request, CancellationToken ct = default)
        => _client.RequestAsync<Job>(HttpMethod.Post, "/v1/generate/quick", request, ct);

    /// <summary>List jobs with optional filtering and pagination.</summary>
    public async Task<JobList> ListAsync(ListJobsParams? @params = null, CancellationToken ct = default)
    {
        var p = @params ?? new ListJobsParams();
        var query = $"?limit={p.Limit}";
        if (p.Status is not null) query += $"&status={Uri.EscapeDataString(p.Status)}";
        if (p.After is not null) query += $"&after={Uri.EscapeDataString(p.After)}";
        if (p.Before is not null) query += $"&before={Uri.EscapeDataString(p.Before)}";

        // API returns {"data": [...], "has_more": bool, "next_cursor": ...}
        var response = await _client.RequestAsync<JsonElement>(HttpMethod.Get, $"/v1/jobs{query}", ct).ConfigureAwait(false);
        var opts = VynFiClient.GetJsonOptions();

        if (response.ValueKind == JsonValueKind.Array)
        {
            var jobs = JsonSerializer.Deserialize<List<Job>>(response.GetRawText(), opts) ?? new();
            return new JobList { Jobs = jobs };
        }

        var list = new JobList();
        if (response.TryGetProperty("data", out var data) || response.TryGetProperty("jobs", out data))
            list.Jobs = JsonSerializer.Deserialize<List<Job>>(data.GetRawText(), opts) ?? new();
        if (response.TryGetProperty("has_more", out var hm))
            list.HasMore = hm.GetBoolean();
        if (response.TryGetProperty("next_cursor", out var nc) && nc.ValueKind != JsonValueKind.Null)
            list.NextCursor = nc.GetString();
        return list;
    }

    /// <summary>Get a single job by ID.</summary>
    public Task<Job> GetAsync(string jobId, CancellationToken ct = default)
        => _client.RequestAsync<Job>(HttpMethod.Get, $"/v1/jobs/{jobId}", ct);

    /// <summary>Cancel a running job.</summary>
    public Task<Job> CancelAsync(string jobId, CancellationToken ct = default)
        => _client.RequestAsync<Job>(HttpMethod.Delete, $"/v1/jobs/{jobId}", ct);

    /// <summary>Download the output of a completed job as raw bytes.</summary>
    public Task<byte[]> DownloadAsync(string jobId, CancellationToken ct = default)
        => _client.RequestBytesAsync(HttpMethod.Get, $"/v1/jobs/{jobId}/download", ct);
}
