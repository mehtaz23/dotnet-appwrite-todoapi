// Relevant using directives
using Appwrite;
using Appwrite.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Get necessary Appwrite configuration from appsettings.json
var projectId = builder.Configuration["Appwrite:Project_Id"];
var apiKey = builder.Configuration["Appwrite:Api_Key"];
var databaseId = builder.Configuration["Appwrite:Database_Id"];
var collectionId = builder.Configuration["Appwrite:Collection_Id"];

// Initialize object for Appwrite Client
var client = new Client()
    .SetEndpoint("https://cloud.appwrite.io/v1")
    .SetProject(projectId)
    .SetKey(apiKey);

// Initialize object for Database service APIs
var database = new Databases(client);

// Create CRUD API endpoints
app.MapGet("/todos", async () => {
    try {
        var todos = await database.ListDocuments(
            databaseId: databaseId,
            collectionId: collectionId
        );
        return Results.Ok(todos);
    } catch (AppwriteException e) {
        return Results.NotFound(new Dictionary<string, string> { {"message", e.Message} });
    }
})
.WithName("GetAllTodos");

app.MapGet("/todos/{id}", async (string id) => {
    try {
        var todo = await database.GetDocument(
            databaseId: databaseId,
            collectionId: collectionId,
            documentId: id
        );
        return Results.Ok(todo);
    } catch (AppwriteException e) {
        return Results.NotFound(new Dictionary<string, string> { {"message", e.Message} });
    }
})
.WithName("GetTodo");

app.MapPost("/todos", async (Todo todo) => {
    try {
        var document = await database.CreateDocument(
            databaseId: databaseId,
            collectionId: collectionId,
            documentId: ID.Unique(),
            data: todo
        );
        return Results.Created($"/todos/{document.Id}", document);
    } catch (AppwriteException e) {
        return Results.BadRequest(new Dictionary<string, string> { {"message", e.Message} });
    }
})
.WithName("CreateTodo");

app.MapPut("/todos/{id}", async (string id, Todo todo) => {
    try {
        var document = await database.UpdateDocument(
            databaseId: databaseId,
            collectionId: collectionId,
            documentId: id,
            data: todo
        );
        return Results.NoContent();
    } catch (AppwriteException e) {
        return Results.BadRequest(new Dictionary<string, string> { {"message", e.Message} });
    }
})
.WithName("UpdateTodo");

app.MapDelete("/todos/{id}", async(string id) => {
    try {
        var document = await database.DeleteDocument(
            databaseId: databaseId,
            collectionId: collectionId,
            documentId: id
        );
        return Results.Ok(document);
    } catch (AppwriteException e) {
        return Results.NotFound(new Dictionary<string, string> { {"message", e.Message} });
    }
})
.WithName("DeleteTodo");

app.Run();
