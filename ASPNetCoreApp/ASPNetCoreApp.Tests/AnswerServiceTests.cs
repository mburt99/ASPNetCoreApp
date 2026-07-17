using System.Net;
using ASPNetCoreApp.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ASPNetCoreApp.Tests;

public class AnswerServiceTests
{
    private static IConfiguration Config() =>
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Anthropic:ApiKey"] = "test-key"
        }).Build();

    [Fact]
    public async Task GenerateAnswerAsync_Success_ReturnsAnswerText()
    {
        var json = """
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
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        });
        var service = new AnswerService(new HttpClient(handler), Config());

        var answer = await service.GenerateAnswerAsync("some context", "a question?");

        Assert.Equal("the answer", answer);
    }

    [Fact]
    public async Task GenerateAnswerAsync_ApiFailure_ThrowsDependencyUnavailableExceptionForClaude()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("""{"type":"error","error":{"type":"api_error","message":"boom"}}""")
        });
        var service = new AnswerService(new HttpClient(handler), Config());

        var ex = await Assert.ThrowsAsync<DependencyUnavailableException>(() => service.GenerateAnswerAsync("context", "question?"));
        Assert.Equal("Claude", ex.DependencyName);
    }

    [Fact]
    public async Task GenerateAnswerAsync_NetworkFailure_ThrowsDependencyUnavailableExceptionForClaude()
    {
        var service = new AnswerService(new HttpClient(new ThrowingHttpMessageHandler()), Config());

        var ex = await Assert.ThrowsAsync<DependencyUnavailableException>(() => service.GenerateAnswerAsync("context", "question?"));
        Assert.Equal("Claude", ex.DependencyName);
    }
}
