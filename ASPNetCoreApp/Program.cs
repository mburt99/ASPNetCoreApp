using ASPNetCoreApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<EmbeddingService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Ollama:BaseUrl"]!);
});

builder.Services.AddHttpClient<VectorStoreService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Qdrant:BaseUrl"]!);
});

builder.Services.AddHttpClient<AnswerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/documents", async (DocumentRequest request, EmbeddingService embeddingService, VectorStoreService vectorStore) =>
{
    if (string.IsNullOrWhiteSpace(request.Text))
        return Results.BadRequest("text must not be empty.");

    try
    {
        var vector = await embeddingService.GetEmbeddingAsync(request.Text);
        var id = await vectorStore.UpsertAsync(vector, request.Text);
        return Results.Ok(new { id });
    }
    catch (DependencyUnavailableException ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status502BadGateway);
    }
})
.WithName("IndexDocument")
.WithOpenApi();

app.MapPost("/ask", async (AskRequest request, EmbeddingService embeddingService, VectorStoreService vectorStore, AnswerService answerService) =>
{
    if (string.IsNullOrWhiteSpace(request.Question))
        return Results.BadRequest("question must not be empty.");

    try
    {
        var queryVector = await embeddingService.GetEmbeddingAsync(request.Question);
        var context = await vectorStore.SearchTopMatchAsync(queryVector);

        if (string.IsNullOrEmpty(context))
            return Results.NotFound("No matching context found for the question.");

        var answer = await answerService.GenerateAnswerAsync(context, request.Question);
        return Results.Ok(new { answer });
    }
    catch (DependencyUnavailableException ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status502BadGateway);
    }
})
.WithName("AskQuestion")
.WithOpenApi();

app.Run();

record DocumentRequest(string Text);
record AskRequest(string Question);

public partial class Program { }
