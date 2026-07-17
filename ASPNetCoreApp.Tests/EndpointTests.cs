using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ASPNetCoreApp.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ASPNetCoreApp.Tests;

public class EndpointTests
{
    private static WebApplicationFactory<Program> CreateFactory(
        Func<HttpRequestMessage, HttpResponseMessage>? ollamaResponder = null,
        Func<HttpRequestMessage, HttpResponseMessage>? qdrantResponder = null,
        Func<HttpRequestMessage, HttpResponseMessage>? claudeResponder = null)
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Anthropic:ApiKey"] = "test-key"
                });
            });

            builder.ConfigureServices(services =>
            {
                if (ollamaResponder is not null)
                {
                    services.AddHttpClient<EmbeddingService>()
                        .ConfigurePrimaryHttpMessageHandler(() => new FakeHttpMessageHandler(ollamaResponder));
                }

                if (qdrantResponder is not null)
                {
                    services.AddHttpClient<VectorStoreService>()
                        .ConfigurePrimaryHttpMessageHandler(() => new FakeHttpMessageHandler(qdrantResponder));
                }

                if (claudeResponder is not null)
                {
                    services.AddHttpClient<AnswerService>()
                        .ConfigurePrimaryHttpMessageHandler(() => new FakeHttpMessageHandler(claudeResponder));
                }
            });
        });
    }

    private static HttpResponseMessage JsonOk(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body) };

    [Fact]
    public async Task PostDocuments_HappyPath_ReturnsAssignedId()
    {
        using var factory = CreateFactory(
            ollamaResponder: _ => JsonOk("""{"embeddings":[[0.1,0.2]]}"""),
            qdrantResponder: _ => JsonOk("""{"result":{"status":"acknowledged"}}"""));
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/documents", new { text = "a real document" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("id").GetInt32() > 0);
    }

    [Fact]
    public async Task PostDocuments_EmptyText_ReturnsBadRequest()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/documents", new { text = "   " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostAsk_HappyPath_ReturnsAnswer()
    {
        var claudeJson = """
        {
          "id": "msg_test",
          "type": "message",
          "role": "assistant",
          "content": [{"type":"text","text":"the answer"}],
          "model": "claude-sonnet-5",
          "stop_reason": "end_turn",
          "stop_sequence": null,
          "usage": {"input_tokens":10,"output_tokens":5}
        }
        """;
        using var factory = CreateFactory(
            ollamaResponder: _ => JsonOk("""{"embeddings":[[0.1,0.2]]}"""),
            qdrantResponder: _ => JsonOk("""{"result":[{"id":1,"score":0.9,"payload":{"text":"matched context"}}]}"""),
            claudeResponder: _ => JsonOk(claudeJson));
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/ask", new { question = "a real question?" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("the answer", body.GetProperty("answer").GetString());
    }

    [Fact]
    public async Task PostAsk_EmptyQuestion_ReturnsBadRequest()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/ask", new { question = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostAsk_NoSearchResults_ReturnsNotFound()
    {
        using var factory = CreateFactory(
            ollamaResponder: _ => JsonOk("""{"embeddings":[[0.1,0.2]]}"""),
            qdrantResponder: _ => JsonOk("""{"result":[]}"""));
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/ask", new { question = "a real question?" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
