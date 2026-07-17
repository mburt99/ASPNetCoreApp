using ASPNetCoreApp.Models;
using ASPNetCoreApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var documentsPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "documents.json");
var documents = DocumentLoader.LoadFromFile(documentsPath);
builder.Services.AddSingleton<IDocumentSearchService>(new DocumentSearchService(documents));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/search", async (SearchRequest request, IDocumentSearchService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Query))
    {
        return Results.BadRequest("Query is required.");
    }

    var result = await service.FindBestMatchAsync(request.Query);
    return result is null ? Results.NotFound() : Results.Ok(result);
})
.WithName("SearchDocuments")
.WithOpenApi();

app.Run();

public partial class Program { }
