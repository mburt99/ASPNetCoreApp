namespace ASPNetCoreApp.Models;

public record SearchRequest(string Query);

public record SearchResult(string Title, string Source, string Content, int Score);
