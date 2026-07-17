using System.Net.Http.Json;

namespace ASPNetCoreApp.Services;

public class EmbeddingService(HttpClient http)
{
    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        HttpResponseMessage response;
        try
        {
            response = await http.PostAsJsonAsync("/api/embed", new { model = "nomic-embed-text", input = text });
        }
        catch (HttpRequestException)
        {
            throw new DependencyUnavailableException("Ollama", "Ollama is unreachable.");
        }

        if (!response.IsSuccessStatusCode)
            throw new DependencyUnavailableException("Ollama", $"Ollama returned {(int)response.StatusCode}.");

        var result = await response.Content.ReadFromJsonAsync<OllamaEmbedResponse>();
        return result!.Embeddings[0];
    }

    record OllamaEmbedResponse(float[][] Embeddings);
}
