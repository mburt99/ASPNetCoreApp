using Anthropic;
using Anthropic.Exceptions;
using Anthropic.Models.Messages;

namespace ASPNetCoreApp.Services;

public class AnswerService
{
    private readonly AnthropicClient _client;

    public AnswerService(HttpClient http, IConfiguration config)
    {
        var apiKey = config["Anthropic:ApiKey"] ?? throw new InvalidOperationException("Anthropic:ApiKey not found.");
        _client = new AnthropicClient { ApiKey = apiKey, HttpClient = http };
    }

    public async Task<string> GenerateAnswerAsync(string context, string question)
    {
        var prompt = $"Context:\n{context}\n\nQuestion: {question}\n\nAnswer using only the context above.";
        var parameters = new MessageCreateParams
        {
            MaxTokens = 1024,
            Messages = [new() { Role = Role.User, Content = prompt }],
            Model = "claude-sonnet-5",
        };

        Message message;
        try
        {
            message = await _client.Messages.Create(parameters);
        }
        catch (AnthropicException)
        {
            throw new DependencyUnavailableException("Claude", "Claude API call failed.");
        }

        foreach (var block in message.Content)
        {
            if (block.TryPickText(out var textBlock))
                return textBlock.Text;
        }

        throw new DependencyUnavailableException("Claude", "Claude returned no text content.");
    }
}
