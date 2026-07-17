using ASPNetCoreApp.Models;

namespace ASPNetCoreApp.Services;

public class DocumentSearchService : IDocumentSearchService
{
    private readonly List<Document> _documents;

    public DocumentSearchService(List<Document> documents)
    {
        _documents = documents;
    }

    public Task<SearchResult?> FindBestMatchAsync(string query)
    {
        var queryTerms = Tokenize(query);
        if (queryTerms.Count == 0)
        {
            return Task.FromResult<SearchResult?>(null);
        }

        Document? best = null;
        var bestScore = 0;

        foreach (var doc in _documents)
        {
            var titleTerms = Tokenize(doc.Title);
            var contentTerms = Tokenize(doc.Content);

            var score = queryTerms.Count(t => titleTerms.Contains(t)) * 2
                      + queryTerms.Count(t => contentTerms.Contains(t));

            if (score > bestScore)
            {
                bestScore = score;
                best = doc;
            }
        }

        if (best is null || bestScore == 0)
        {
            return Task.FromResult<SearchResult?>(null);
        }

        return Task.FromResult<SearchResult?>(new SearchResult(best.Title, best.Source, best.Content, bestScore));
    }

    private static HashSet<string> Tokenize(string text) =>
        text.ToLowerInvariant()
            .Split(new[] { ' ', '.', ',', '/', '\'', '?', '!', '>', ':' }, StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet();
}
