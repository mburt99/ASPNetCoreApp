using ASPNetCoreApp.Models;
using ASPNetCoreApp.Services;

namespace ASPNetCoreApp.Tests;

public class DocumentSearchServiceTests
{
    private static List<Document> SampleDocuments() =>
    [
        new Document("1", "Resetting Your Password", "Account FAQ",
            "To reset your password, go to Settings and click Forgot Password."),
        new Document("2", "Refund Policy", "Billing FAQ",
            "Refunds are available within 30 days of purchase."),
        new Document("3", "Upgrading Your Plan", "Billing FAQ",
            "You can upgrade your subscription plan anytime from the Billing page.")
    ];

    [Fact]
    public async Task FindBestMatchAsync_TitleTermMatch_ReturnsExpectedDocument()
    {
        var service = new DocumentSearchService(SampleDocuments());

        var result = await service.FindBestMatchAsync("password reset");

        Assert.NotNull(result);
        Assert.Equal("Resetting Your Password", result!.Title);
    }

    [Fact]
    public async Task FindBestMatchAsync_NoOverlapQuery_ReturnsNull()
    {
        var service = new DocumentSearchService(SampleDocuments());

        var result = await service.FindBestMatchAsync("zzqx nonexistent gibberish");

        Assert.Null(result);
    }

    [Fact]
    public async Task FindBestMatchAsync_TitleMatchOutweighsContentOnlyMatch_ReturnsTitleMatch()
    {
        var service = new DocumentSearchService(SampleDocuments());

        var result = await service.FindBestMatchAsync("plan");

        Assert.NotNull(result);
        Assert.Equal("Upgrading Your Plan", result!.Title);
    }

    [Fact]
    public async Task FindBestMatchAsync_EmptyQuery_ReturnsNull()
    {
        var service = new DocumentSearchService(SampleDocuments());

        var result = await service.FindBestMatchAsync("");

        Assert.Null(result);
    }
}
