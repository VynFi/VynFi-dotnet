using System.Net;
using VynFi.Models;

namespace VynFi.Tests;

public class VynFiClientTests
{
    [Fact]
    public void RequiresApiKey()
    {
        Assert.Throws<ArgumentException>(() => new VynFiClient(""));
    }

    [Fact]
    public async Task AuthHeaderSent()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """{"slug":"banking","name":"Banking"}""");
        using var client = TestHelper.CreateClient(handler, "http://localhost");
        await client.Catalog.GetSectorAsync("banking");

        Assert.NotNull(handler.LastRequest);
        Assert.Equal("Bearer", handler.LastRequest!.Headers.Authorization?.Scheme);
        Assert.Equal("vf_test_key", handler.LastRequest.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task NotFoundThrows()
    {
        var handler = new MockHttpHandler(HttpStatusCode.NotFound, """{"detail":"not found"}""");
        using var client = TestHelper.CreateClient(handler);
        await Assert.ThrowsAsync<NotFoundException>(() => client.Jobs.GetAsync("bad-id"));
    }

    [Fact]
    public async Task RateLimitThrows()
    {
        var handler = new MockHttpHandler((HttpStatusCode)429, """{"detail":"rate limited"}""");
        using var client = TestHelper.CreateClient(handler);
        await Assert.ThrowsAsync<RateLimitException>(() => client.Jobs.GetAsync("x"));
    }

    [Fact]
    public async Task ServerErrorThrows()
    {
        var handler = new MockHttpHandler(HttpStatusCode.InternalServerError, """{"detail":"internal error"}""");
        using var client = TestHelper.CreateClient(handler);
        await Assert.ThrowsAsync<ServerException>(() => client.Jobs.GetAsync("x"));
    }

    [Fact]
    public async Task UnauthorizedThrows()
    {
        var handler = new MockHttpHandler(HttpStatusCode.Unauthorized, """{"detail":"invalid key"}""");
        using var client = TestHelper.CreateClient(handler);
        await Assert.ThrowsAsync<AuthenticationException>(() => client.Jobs.GetAsync("x"));
    }

    [Fact]
    public async Task InsufficientCreditsThrows()
    {
        var handler = new MockHttpHandler(HttpStatusCode.PaymentRequired, """{"detail":"not enough credits"}""");
        using var client = TestHelper.CreateClient(handler);
        await Assert.ThrowsAsync<InsufficientCreditsException>(() =>
            client.Jobs.GenerateAsync(new GenerateRequest
            {
                Tables = new() { new TableSpec { Name = "t", Rows = 1 } }
            }));
    }

    [Fact]
    public async Task ValidationErrorThrows()
    {
        var handler = new MockHttpHandler((HttpStatusCode)422, """{"detail":"invalid rows"}""");
        using var client = TestHelper.CreateClient(handler);
        await Assert.ThrowsAsync<ValidationException>(() =>
            client.Jobs.GenerateAsync(new GenerateRequest
            {
                Tables = new() { new TableSpec { Name = "t", Rows = -1 } }
            }));
    }

    [Fact]
    public async Task GenerateJob()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """
        {
            "id": "job-1",
            "status": "queued",
            "credits_reserved": 100,
            "estimated_duration_seconds": 30
        }
        """);
        using var client = TestHelper.CreateClient(handler);
        var result = await client.Jobs.GenerateAsync(new GenerateRequest
        {
            Tables = new() { new TableSpec { Name = "transactions", Rows = 1000 } }
        });
        Assert.Equal("job-1", result.Id);
        Assert.Equal("queued", result.Status);
        Assert.Equal(100, result.CreditsReserved);
        Assert.Equal(30, result.EstimatedDurationSeconds);
    }

    [Fact]
    public async Task ListSectors()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """
        {
            "data": [
                {"slug": "banking", "name": "Banking", "description": "", "icon": "", "table_count": 3}
            ]
        }
        """);
        using var client = TestHelper.CreateClient(handler);
        var sectors = await client.Catalog.ListSectorsAsync();
        Assert.Single(sectors);
        Assert.Equal("banking", sectors[0].Slug);
        Assert.Equal(3, sectors[0].TableCount);
    }

    [Fact]
    public async Task GetJob()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """
        {
            "id": "job-123",
            "status": "completed",
            "format": "json",
            "credits_used": 50,
            "progress": {"percent": 100, "rows_generated": 1000, "rows_total": 1000}
        }
        """);
        using var client = TestHelper.CreateClient(handler);
        var job = await client.Jobs.GetAsync("job-123");
        Assert.Equal("job-123", job.Id);
        Assert.Equal("completed", job.Status);
        Assert.Equal(50, job.CreditsUsed);
        Assert.NotNull(job.Progress);
        Assert.Equal(100, job.Progress!.Percent);
        Assert.Equal(1000, job.Progress.RowsGenerated);
        Assert.Equal(1000, job.Progress.RowsTotal);
    }

    [Fact]
    public async Task UsageSummary()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """
        {"balance": 5000, "total_used": 1200, "total_reserved": 300, "total_refunded": 0, "burn_rate": 40.0, "period_days": 30}
        """);
        using var client = TestHelper.CreateClient(handler);
        var usage = await client.Usage.SummaryAsync();
        Assert.Equal(5000, usage.Balance);
        Assert.Equal(1200, usage.TotalUsed);
        Assert.Equal(300, usage.TotalReserved);
        Assert.Equal(0, usage.TotalRefunded);
        Assert.Equal(40.0, usage.BurnRate);
        Assert.Equal(30, usage.PeriodDays);
    }

    [Fact]
    public async Task CreateApiKey()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """
        {"id": "key-1", "name": "Test Key", "key": "vf_test_abc123", "prefix": "vf_test_", "scopes": ["read"]}
        """);
        using var client = TestHelper.CreateClient(handler);
        var key = await client.ApiKeys.CreateAsync(new CreateApiKeyRequest { Name = "Test Key" });
        Assert.Equal("key-1", key.Id);
        Assert.Equal("Test Key", key.Name);
        Assert.Equal("vf_test_abc123", key.Key);
        Assert.Equal("vf_test_", key.Prefix);
        Assert.Single(key.Scopes);
        Assert.Equal("read", key.Scopes[0]);
    }

    [Fact]
    public async Task GetSector()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """
        {
            "slug": "banking",
            "name": "Banking",
            "description": "Banking sector data",
            "icon": "bank",
            "multiplier": 1.2,
            "tables": [
                {"name": "accounts", "description": "Bank accounts", "base_rate": 1.0, "columns": []}
            ]
        }
        """);
        using var client = TestHelper.CreateClient(handler);
        var sector = await client.Catalog.GetSectorAsync("banking");
        Assert.Equal("banking", sector.Slug);
        Assert.Equal("Banking", sector.Name);
        Assert.Equal(1.2, sector.Multiplier);
        Assert.Single(sector.Tables);
        Assert.Equal("accounts", sector.Tables[0].Name);
    }

    [Fact]
    public async Task ListJobs()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """
        {
            "data": [
                {"id": "j-1", "status": "completed"},
                {"id": "j-2", "status": "running"}
            ],
            "has_more": true,
            "next_cursor": "cursor-abc"
        }
        """);
        using var client = TestHelper.CreateClient(handler);
        var list = await client.Jobs.ListAsync();
        Assert.Equal(2, list.Jobs.Count);
        Assert.Equal("j-1", list.Jobs[0].Id);
        Assert.Equal("j-2", list.Jobs[1].Id);
        Assert.True(list.HasMore);
        Assert.Equal("cursor-abc", list.NextCursor);
    }

    [Fact]
    public async Task CancelJob()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """
        {"id": "job-1", "status": "cancelled"}
        """);
        using var client = TestHelper.CreateClient(handler);
        var job = await client.Jobs.CancelAsync("job-1");
        Assert.Equal("job-1", job.Id);
        Assert.Equal("cancelled", job.Status);
    }

    [Fact]
    public async Task RequestSendsCorrectUrl()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """{"id":"j-1","status":"done"}""");
        using var client = TestHelper.CreateClient(handler, "http://test-api");
        await client.Jobs.GetAsync("j-1");

        Assert.NotNull(handler.LastRequest);
        Assert.Equal("http://test-api/v1/jobs/j-1", handler.LastRequest!.RequestUri?.ToString());
    }

    [Fact]
    public async Task PostSendsJsonBody()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, """
        {"id": "j-new", "status": "queued", "credits_reserved": 10, "estimated_duration_seconds": 5}
        """);
        using var client = TestHelper.CreateClient(handler);
        await client.Jobs.GenerateAsync(new GenerateRequest
        {
            Tables = new() { new TableSpec { Name = "t1", Rows = 100 } },
            Format = "csv",
            SectorSlug = "banking"
        });

        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.NotNull(handler.LastRequestBody);
        Assert.Contains("\"tables\"", handler.LastRequestBody);
        Assert.Contains("\"t1\"", handler.LastRequestBody);
        Assert.Contains("\"csv\"", handler.LastRequestBody);
    }

    [Fact]
    public void DisposeIsIdempotent()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, "{}");
        var client = TestHelper.CreateClient(handler);
        client.Dispose();
        client.Dispose(); // Should not throw
    }
}
