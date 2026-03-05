using System.Text.Json.Serialization;

namespace VynFi.Models;

public class Job
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("status")] public string Status { get; set; } = "";
    [JsonPropertyName("tables")] public object? Tables { get; set; }
    [JsonPropertyName("format")] public string Format { get; set; } = "json";
    [JsonPropertyName("credits_reserved")] public long? CreditsReserved { get; set; }
    [JsonPropertyName("credits_used")] public long? CreditsUsed { get; set; }
    [JsonPropertyName("sector_slug")] public string SectorSlug { get; set; } = "";
    [JsonPropertyName("progress")] public JobProgress? Progress { get; set; }
    [JsonPropertyName("output_path")] public string? OutputPath { get; set; }
    [JsonPropertyName("error")] public string? Error { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("completed_at")] public DateTimeOffset? CompletedAt { get; set; }
}

public class JobProgress
{
    [JsonPropertyName("percent")] public int Percent { get; set; }
    [JsonPropertyName("rows_generated")] public long RowsGenerated { get; set; }
    [JsonPropertyName("rows_total")] public long RowsTotal { get; set; }
}

public class SubmitJobResponse
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("status")] public string Status { get; set; } = "";
    [JsonPropertyName("credits_reserved")] public long CreditsReserved { get; set; }
    [JsonPropertyName("estimated_duration_seconds")] public long EstimatedDurationSeconds { get; set; }
    [JsonPropertyName("links")] public JobLinks? Links { get; set; }
}

public class JobLinks
{
    [JsonPropertyName("self")] public string SelfLink { get; set; } = "";
    [JsonPropertyName("stream")] public string Stream { get; set; } = "";
    [JsonPropertyName("cancel")] public string Cancel { get; set; } = "";
    [JsonPropertyName("download")] public string Download { get; set; } = "";
}

public class JobList
{
    [JsonPropertyName("jobs")] public List<Job> Jobs { get; set; } = new();
    [JsonPropertyName("has_more")] public bool HasMore { get; set; }
    [JsonPropertyName("next_cursor")] public string? NextCursor { get; set; }
}

public class GenerateRequest
{
    [JsonPropertyName("tables")] public List<TableSpec> Tables { get; set; } = new();
    [JsonPropertyName("format")] public string Format { get; set; } = "json";
    [JsonPropertyName("sector_slug")] public string SectorSlug { get; set; } = "retail";
}

public class TableSpec
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("rows")] public long Rows { get; set; }
}

public class ListJobsParams
{
    public int Limit { get; set; } = 20;
    public string? Status { get; set; }
    public string? After { get; set; }
    public string? Before { get; set; }
}
