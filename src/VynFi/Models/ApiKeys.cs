using System.Text.Json.Serialization;

namespace VynFi.Models;

public class ApiKey
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("prefix")] public string Prefix { get; set; } = "";
    [JsonPropertyName("scopes")] public List<string> Scopes { get; set; } = new();
    [JsonPropertyName("status")] public string Status { get; set; } = "active";
    [JsonPropertyName("last_used_at")] public DateTimeOffset? LastUsedAt { get; set; }
    [JsonPropertyName("expires_at")] public DateTimeOffset? ExpiresAt { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
}

public class ApiKeyCreated
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("key")] public string Key { get; set; } = "";
    [JsonPropertyName("prefix")] public string Prefix { get; set; } = "";
    [JsonPropertyName("scopes")] public List<string> Scopes { get; set; } = new();
    [JsonPropertyName("expires_at")] public DateTimeOffset? ExpiresAt { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
}

public class CreateApiKeyRequest
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("scopes")] public List<string>? Scopes { get; set; }
    [JsonPropertyName("expires_in_days")] public int? ExpiresInDays { get; set; }
}

public class UpdateApiKeyRequest
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("scopes")] public List<string>? Scopes { get; set; }
}
