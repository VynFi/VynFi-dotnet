using System.Text.Json;
using VynFi.Models;

namespace VynFi.Resources;

/// <summary>Webhooks resource — CRUD and testing.</summary>
public class WebhooksResource
{
    private readonly VynFiClient _client;
    internal WebhooksResource(VynFiClient client) => _client = client;

    /// <summary>Create a new webhook endpoint.</summary>
    public Task<WebhookCreated> CreateAsync(CreateWebhookRequest request, CancellationToken ct = default)
        => _client.RequestAsync<WebhookCreated>(HttpMethod.Post, "/v1/webhooks", request, ct);

    /// <summary>List all webhooks.</summary>
    public Task<List<Webhook>> ListAsync(CancellationToken ct = default)
        => _client.RequestListAsync<Webhook>(HttpMethod.Get, "/v1/webhooks", ct);

    /// <summary>Get a single webhook by ID.</summary>
    public Task<Webhook> GetAsync(string webhookId, CancellationToken ct = default)
        => _client.RequestAsync<Webhook>(HttpMethod.Get, $"/v1/webhooks/{webhookId}", ct);

    /// <summary>Update a webhook's URL, events, or status.</summary>
    public Task<Webhook> UpdateAsync(string webhookId, UpdateWebhookRequest request, CancellationToken ct = default)
        => _client.RequestAsync<Webhook>(new HttpMethod("PATCH"), $"/v1/webhooks/{webhookId}", request, ct);

    /// <summary>Delete a webhook.</summary>
    public Task DeleteAsync(string webhookId, CancellationToken ct = default)
        => _client.RequestVoidAsync(HttpMethod.Delete, $"/v1/webhooks/{webhookId}", ct);

    /// <summary>Send a test event to a webhook endpoint.</summary>
    public Task<JsonElement> TestAsync(string webhookId, CancellationToken ct = default)
        => _client.RequestAsync<JsonElement>(HttpMethod.Post, $"/v1/webhooks/{webhookId}/test", ct);
}
