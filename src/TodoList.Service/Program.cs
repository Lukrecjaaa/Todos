using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using TodoList.Service.Application;

var builder = WebApplication.CreateBuilder();
// Rejestracja usług w kontenerze
builder.Services.AddSqlServer<ApplicationContext>(builder.Configuration.GetConnectionString("Default"));
builder.Services.AddLogging();
// Tworzymy obiekt aplikacji webowej z usługami 
var app = builder.Build();
// Konfigurujemy middleware
var notebooks = new List<Notebook>
{
    new ()
    {
        Title = "Testowy"
    }
};
    
app.MapGet("/notebooks", async ([FromServices] ILogger<Notebook> logger, [FromServices] ApplicationContext db) =>
{
    return await db.Notebooks.ToListAsync();
});
app.MapGet("/notebooks/{id:int}", async ([FromServices] ApplicationContext db, int id) =>
{
    return await db.Notebooks.FirstOrDefaultAsync(notebook => notebook.Id == id);
});
app.MapPost("/notebooks", async ([FromBody]AddNotebookRequest request, [FromServices] ApplicationContext db) =>
{
    var notebook = new Notebook
    {
        Title = request.Title,
        Description = request.Description
    };
    db.Notebooks.Add(notebook);
    await db.SaveChangesAsync(); 
    return $"Added notebook with id: {notebook.Id}";
});

app.MapGet("/notebooks/includes/{text}", async ([FromServices] ApplicationContext db, string text) =>
{
    return await db.Notebooks.Where(notebook => notebook.Title.Contains(text)).ToListAsync();
});

using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<ApplicationContext>().Database.MigrateAsync();

// uruchamiamy aplikację
await app.RunAsync();

class AddNotebookRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
}

