using VynFi.Models;

namespace VynFi.Resources;

/// <summary>Billing resource — subscription, invoices, payment methods.</summary>
public class BillingResource
{
    private readonly VynFiClient _client;
    internal BillingResource(VynFiClient client) => _client = client;

    /// <summary>Get current subscription details.</summary>
    public Task<Subscription> SubscriptionAsync(CancellationToken ct = default)
        => _client.RequestAsync<Subscription>(HttpMethod.Get, "/v1/billing/subscription", ct);

    /// <summary>List invoices.</summary>
    public Task<List<Invoice>> InvoicesAsync(CancellationToken ct = default)
        => _client.RequestListAsync<Invoice>(HttpMethod.Get, "/v1/billing/invoices", ct);

    /// <summary>Get the current payment method on file.</summary>
    public Task<PaymentMethod> PaymentMethodAsync(CancellationToken ct = default)
        => _client.RequestAsync<PaymentMethod>(HttpMethod.Get, "/v1/billing/payment-method", ct);
}
