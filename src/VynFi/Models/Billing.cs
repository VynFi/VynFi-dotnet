using System.Text.Json.Serialization;

namespace VynFi.Models;

public class Subscription
{
    [JsonPropertyName("tier")] public string Tier { get; set; } = "free";
    [JsonPropertyName("status")] public string Status { get; set; } = "active";
    [JsonPropertyName("current_period_end")] public DateTimeOffset? CurrentPeriodEnd { get; set; }
    [JsonPropertyName("cancel_at_period_end")] public bool CancelAtPeriodEnd { get; set; }
}

public class Invoice
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("amount")] public long Amount { get; set; }
    [JsonPropertyName("currency")] public string Currency { get; set; } = "usd";
    [JsonPropertyName("status")] public string Status { get; set; } = "";
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("pdf_url")] public string? PdfUrl { get; set; }
}

public class PaymentMethod
{
    [JsonPropertyName("type")] public string Type { get; set; } = "";
    [JsonPropertyName("brand")] public string Brand { get; set; } = "";
    [JsonPropertyName("last4")] public string Last4 { get; set; } = "";
    [JsonPropertyName("exp_month")] public int ExpMonth { get; set; }
    [JsonPropertyName("exp_year")] public int ExpYear { get; set; }
}
