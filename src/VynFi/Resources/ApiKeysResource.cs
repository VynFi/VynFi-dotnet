using VynFi.Models;

namespace VynFi.Resources;

/// <summary>API key management resource.</summary>
public class ApiKeysResource
{
    private readonly VynFiClient _client;
    internal ApiKeysResource(VynFiClient client) => _client = client;

    /// <summary>Create a new API key. The full key is only returned once.</summary>
    public Task<ApiKeyCreated> CreateAsync(CreateApiKeyRequest request, CancellationToken ct = default)
        => _client.RequestAsync<ApiKeyCreated>(HttpMethod.Post, "/v1/api-keys", request, ct);

    /// <summary>List all API keys (keys are masked).</summary>
    public Task<List<ApiKey>> ListAsync(CancellationToken ct = default)
        => _client.RequestListAsync<ApiKey>(HttpMethod.Get, "/v1/api-keys", ct);

    /// <summary>Get a single API key by ID.</summary>
    public Task<ApiKey> GetAsync(string keyId, CancellationToken ct = default)
        => _client.RequestAsync<ApiKey>(HttpMethod.Get, $"/v1/api-keys/{keyId}", ct);

    /// <summary>Update an API key's name or scopes.</summary>
    public Task<ApiKey> UpdateAsync(string keyId, UpdateApiKeyRequest request, CancellationToken ct = default)
        => _client.RequestAsync<ApiKey>(new HttpMethod("PATCH"), $"/v1/api-keys/{keyId}", request, ct);

    /// <summary>Revoke (delete) an API key.</summary>
    public Task RevokeAsync(string keyId, CancellationToken ct = default)
        => _client.RequestVoidAsync(HttpMethod.Delete, $"/v1/api-keys/{keyId}", ct);
}
