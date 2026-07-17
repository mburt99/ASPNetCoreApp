using System.Net;
using ASPNetCoreApp.Services;
using Xunit;

namespace ASPNetCoreApp.Tests;

public class VectorStoreServiceTests
{
    [Fact]
    public async Task UpsertAsync_Success_ReturnsAssignedIdAndSendsExpectedRequest()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"result":{"status":"acknowledged"}}""")
        });
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://qdrant.test") };
        var service = new VectorStoreService(http);

        var id = await service.UpsertAsync([0.1f, 0.2f], "some text");

        Assert.NotEqual(Guid.Empty, id);
        Assert.Equal("/collections/docs/points", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.Equal(HttpMethod.Put, handler.LastRequest.Method);
        Assert.Contains("\"text\":\"some text\"", handler.LastRequestBody);
    }

    [Fact]
    public async Task UpsertAsync_NonSuccessStatusCode_ThrowsDependencyUnavailableExceptionForQdrant()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://qdrant.test") };
        var service = new VectorStoreService(http);

        var ex = await Assert.ThrowsAsync<DependencyUnavailableException>(() => service.UpsertAsync([0.1f], "text"));
        Assert.Equal("Qdrant", ex.DependencyName);
    }

    [Fact]
    public async Task UpsertAsync_NetworkFailure_ThrowsDependencyUnavailableExceptionForQdrant()
    {
        var http = new HttpClient(new ThrowingHttpMessageHandler()) { BaseAddress = new Uri("http://qdrant.test") };
        var service = new VectorStoreService(http);

        var ex = await Assert.ThrowsAsync<DependencyUnavailableException>(() => service.UpsertAsync([0.1f], "text"));
        Assert.Equal("Qdrant", ex.DependencyName);
    }

    [Fact]
    public async Task SearchTopMatchAsync_WithResults_ReturnsPayloadText()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"result":[{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","score":0.9,"payload":{"text":"matched context"}}]}""")
        });
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://qdrant.test") };
        var service = new VectorStoreService(http);

        var result = await service.SearchTopMatchAsync([0.1f, 0.2f]);

        Assert.Equal("matched context", result);
        Assert.Equal("/collections/docs/points/search", handler.LastRequest!.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task SearchTopMatchAsync_NoResults_ReturnsNull()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"result":[]}""")
        });
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://qdrant.test") };
        var service = new VectorStoreService(http);

        var result = await service.SearchTopMatchAsync([0.1f, 0.2f]);

        Assert.Null(result);
    }

    [Fact]
    public async Task SearchTopMatchAsync_NonSuccessStatusCode_ThrowsDependencyUnavailableExceptionForQdrant()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://qdrant.test") };
        var service = new VectorStoreService(http);

        var ex = await Assert.ThrowsAsync<DependencyUnavailableException>(() => service.SearchTopMatchAsync([0.1f]));
        Assert.Equal("Qdrant", ex.DependencyName);
    }
}
