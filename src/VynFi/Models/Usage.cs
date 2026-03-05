using System.Text.Json.Serialization;

namespace VynFi.Models;

public class UsageSummary
{
    [JsonPropertyName("balance")] public long Balance { get; set; }
    [JsonPropertyName("total_used")] public long TotalUsed { get; set; }
    [JsonPropertyName("total_reserved")] public long TotalReserved { get; set; }
    [JsonPropertyName("total_refunded")] public long TotalRefunded { get; set; }
    [JsonPropertyName("burn_rate")] public double BurnRate { get; set; }
    [JsonPropertyName("period_days")] public int PeriodDays { get; set; } = 30;
}

public class DailyUsage
{
    [JsonPropertyName("date")] public string Date { get; set; } = "";
    [JsonPropertyName("credits")] public long Credits { get; set; }
}

public class DailyUsageResponse
{
    [JsonPropertyName("daily")] public List<DailyUsage> Daily { get; set; } = new();
    [JsonPropertyName("by_table")] public Dictionary<string, long> ByTable { get; set; } = new();
}
