using System.Text.Json;
using ASPNetCoreApp.Models;

namespace ASPNetCoreApp.Services;

public static class DocumentLoader
{
    public static List<Document> LoadFromFile(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<Document>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Document>();
    }
}
