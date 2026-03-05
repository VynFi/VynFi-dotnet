# VynFi .NET SDK

Official .NET client for the [VynFi](https://vynfi.com) synthetic financial data API.

## Installation

```
dotnet add package VynFi
```

## Quick Start

```csharp
using VynFi;
using VynFi.Models;

using var client = new VynFiClient("vf_live_...");

// Generate synthetic data
var job = await client.Jobs.GenerateAsync(new GenerateRequest
{
    Tables = new() { new TableSpec { Name = "transactions", Rows = 1000 } },
    SectorSlug = "banking"
});
Console.WriteLine($"Job submitted: {job.Id}");

// Browse catalog
var sectors = await client.Catalog.ListSectorsAsync();
foreach (var s in sectors)
    Console.WriteLine($"{s.Slug}: {s.Name}");

// Check usage
var usage = await client.Usage.SummaryAsync();
Console.WriteLine($"Balance: {usage.Balance} credits");
```

## Resources

| Resource | Methods |
|----------|---------|
| `client.Jobs` | `GenerateAsync`, `GenerateQuickAsync`, `ListAsync`, `GetAsync`, `CancelAsync`, `DownloadAsync` |
| `client.Catalog` | `ListSectorsAsync`, `GetSectorAsync`, `ListAsync`, `GetFingerprintAsync` |
| `client.Usage` | `SummaryAsync`, `DailyAsync` |
| `client.ApiKeys` | `CreateAsync`, `ListAsync`, `GetAsync`, `UpdateAsync`, `RevokeAsync` |
| `client.Quality` | `ScoresAsync`, `TimelineAsync` |
| `client.Webhooks` | `CreateAsync`, `ListAsync`, `GetAsync`, `UpdateAsync`, `DeleteAsync`, `TestAsync` |
| `client.Billing` | `SubscriptionAsync`, `InvoicesAsync`, `PaymentMethodAsync` |

All methods accept an optional `CancellationToken`.

## Error Handling

All methods throw typed exceptions:

```csharp
try
{
    var job = await client.Jobs.GetAsync("bad-id");
}
catch (NotFoundException)
{
    Console.WriteLine("Job not found");
}
catch (RateLimitException)
{
    Console.WriteLine("Rate limited, retry later");
}
catch (VynFiException ex)
{
    Console.WriteLine($"API error: {ex.Message} (HTTP {ex.StatusCode})");
}
```

## Configuration

```csharp
using var client = new VynFiClient(
    apiKey: "vf_live_...",
    baseUrl: "https://api.vynfi.com",  // default
    maxRetries: 2,                      // default, retries on 429/5xx
    timeout: TimeSpan.FromSeconds(30)   // default
);
```

## Supported Frameworks

- .NET 8.0+
- .NET Standard 2.0 (for .NET Framework 4.6.1+ and .NET Core 2.0+)

## License

Apache-2.0
