using System.Text.Json.Serialization;

namespace VynFi.Models;

public class Webhook
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("url")] public string Url { get; set; } = "";
    [JsonPropertyName("events")] public List<string> Events { get; set; } = new();
    [JsonPropertyName("status")] public string Status { get; set; } = "active";
    [JsonPropertyName("secret")] public string? Secret { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTimeOffset? UpdatedAt { get; set; }
}

public class WebhookCreated
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("url")] public string Url { get; set; } = "";
    [JsonPropertyName("events")] public List<string> Events { get; set; } = new();
    [JsonPropertyName("secret")] public string Secret { get; set; } = "";
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
}

public class CreateWebhookRequest
{
    [JsonPropertyName("url")] public string Url { get; set; } = "";
    [JsonPropertyName("events")] public List<string> Events { get; set; } = new();
}

public class UpdateWebhookRequest
{
    [JsonPropertyName("url")] public string? Url { get; set; }
    [JsonPropertyName("events")] public List<string>? Events { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
}
