using System.Net;
using ASPNetCoreApp.Services;
using Xunit;

namespace ASPNetCoreApp.Tests;

public class EmbeddingServiceTests
{
    [Fact]
    public async Task GetEmbeddingAsync_Success_ReturnsEmbeddingAndSendsExpectedRequest()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"embeddings":[[0.1,0.2,0.3]]}""")
        });
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://ollama.test") };
        var service = new EmbeddingService(http);

        var result = await service.GetEmbeddingAsync("some text");

        Assert.Equal([0.1f, 0.2f, 0.3f], result);
        Assert.Equal("/api/embed", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.Contains("\"model\":\"nomic-embed-text\"", handler.LastRequestBody);
        Assert.Contains("\"input\":\"some text\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task GetEmbeddingAsync_NonSuccessStatusCode_ThrowsDependencyUnavailableExceptionForOllama()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://ollama.test") };
        var service = new EmbeddingService(http);

        var ex = await Assert.ThrowsAsync<DependencyUnavailableException>(() => service.GetEmbeddingAsync("text"));
        Assert.Equal("Ollama", ex.DependencyName);
    }

    [Fact]
    public async Task GetEmbeddingAsync_NetworkFailure_ThrowsDependencyUnavailableExceptionForOllama()
    {
        var http = new HttpClient(new ThrowingHttpMessageHandler()) { BaseAddress = new Uri("http://ollama.test") };
        var service = new EmbeddingService(http);

        var ex = await Assert.ThrowsAsync<DependencyUnavailableException>(() => service.GetEmbeddingAsync("text"));
        Assert.Equal("Ollama", ex.DependencyName);
    }
}
