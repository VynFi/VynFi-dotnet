using System.Reflection;

namespace VynFi.Tests;

public static class TestHelper
{
    public static VynFiClient CreateClient(HttpMessageHandler handler, string baseUrl = "http://localhost")
    {
        var client = new VynFiClient("vf_test_key", baseUrl: baseUrl, maxRetries: 0);

        // Replace the internal HttpClient via reflection
        var field = typeof(VynFiClient).GetField("_http", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var oldHttp = (HttpClient)field.GetValue(client)!;
        oldHttp.Dispose();

        var newHttp = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
        newHttp.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "vf_test_key");
        newHttp.DefaultRequestHeaders.UserAgent.ParseAdd("vynfi-dotnet/0.1.0");
        newHttp.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        field.SetValue(client, newHttp);

        return client;
    }
}
