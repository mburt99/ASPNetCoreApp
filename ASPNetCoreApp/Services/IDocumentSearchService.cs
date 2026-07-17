using ASPNetCoreApp.Models;

namespace ASPNetCoreApp.Services;

public interface IDocumentSearchService
{
    Task<SearchResult?> FindBestMatchAsync(string query);
}
