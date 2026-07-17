using System.Net.Http.Json;

namespace ASPNetCoreApp.Services;

public class VectorStoreService(HttpClient http)
{
    private static int _nextId;

    public async Task<int> UpsertAsync(float[] vector, string text)
    {
        var id = Interlocked.Increment(ref _nextId);
        var body = new { points = new[] { new { id, vector, payload = new { text } } } };

        HttpResponseMessage response;
        try
        {
            response = await http.PutAsJsonAsync("/collections/docs/points", body);
        }
        catch (HttpRequestException)
        {
            throw new DependencyUnavailableException("Qdrant", "Qdrant is unreachable.");
        }

        if (!response.IsSuccessStatusCode)
            throw new DependencyUnavailableException("Qdrant", $"Qdrant returned {(int)response.StatusCode}.");

        return id;
    }

    public async Task<string?> SearchTopMatchAsync(float[] queryVector)
    {
        var body = new { vector = queryVector, limit = 1, with_payload = true };

        HttpResponseMessage response;
        try
        {
            response = await http.PostAsJsonAsync("/collections/docs/points/search", body);
        }
        catch (HttpRequestException)
        {
            throw new DependencyUnavailableException("Qdrant", "Qdrant is unreachable.");
        }

        if (!response.IsSuccessStatusCode)
            throw new DependencyUnavailableException("Qdrant", $"Qdrant returned {(int)response.StatusCode}.");

        var result = await response.Content.ReadFromJsonAsync<QdrantSearchResponse>();
        return result?.Result.FirstOrDefault()?.Payload.GetValueOrDefault("text");
    }

    record QdrantSearchResponse(List<QdrantPoint> Result);
    record QdrantPoint(int Id, float Score, Dictionary<string, string> Payload);
}
