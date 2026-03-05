using VynFi.Models;

namespace VynFi.Resources;

/// <summary>Catalog resource — list sectors, tables, fingerprints.</summary>
public class CatalogResource
{
    private readonly VynFiClient _client;
    internal CatalogResource(VynFiClient client) => _client = client;

    /// <summary>List all available sector summaries.</summary>
    public Task<List<SectorSummary>> ListSectorsAsync(CancellationToken ct = default)
        => _client.RequestListAsync<SectorSummary>(HttpMethod.Get, "/v1/catalog/sectors", ct);

    /// <summary>Get full sector details including table definitions.</summary>
    public Task<Sector> GetSectorAsync(string slug, CancellationToken ct = default)
        => _client.RequestAsync<Sector>(HttpMethod.Get, $"/v1/catalog/sectors/{slug}", ct);

    /// <summary>List all catalog items (sector + profile pairs).</summary>
    public Task<List<CatalogItem>> ListAsync(CancellationToken ct = default)
        => _client.RequestListAsync<CatalogItem>(HttpMethod.Get, "/v1/catalog", ct);

    /// <summary>Get a fingerprint (statistical profile) for a sector/profile pair.</summary>
    public Task<Fingerprint> GetFingerprintAsync(string sector, string profile, CancellationToken ct = default)
        => _client.RequestAsync<Fingerprint>(HttpMethod.Get, $"/v1/catalog/{sector}/{profile}", ct);
}
