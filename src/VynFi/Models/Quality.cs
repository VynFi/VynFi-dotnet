using System.Text.Json.Serialization;

namespace VynFi.Models;

public class QualityScore
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("job_id")] public string JobId { get; set; } = "";
    [JsonPropertyName("table_type")] public string TableType { get; set; } = "";
    [JsonPropertyName("rows")] public long Rows { get; set; }
    [JsonPropertyName("overall_score")] public double OverallScore { get; set; }
    [JsonPropertyName("benford_score")] public double BenfordScore { get; set; }
    [JsonPropertyName("correlation_score")] public double CorrelationScore { get; set; }
    [JsonPropertyName("distribution_score")] public double DistributionScore { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
}

public class DailyQuality
{
    [JsonPropertyName("date")] public string Date { get; set; } = "";
    [JsonPropertyName("score")] public double Score { get; set; }
}
