
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using TodoAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddSimpleConsole(opts =>
{
    opts.IncludeScopes = false;
    opts.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
    opts.ColorBehavior = LoggerColorBehavior.Disabled;
});

var connectionString = builder.Configuration.GetConnectionString("TodosDb");

builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
    });

builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1"));
}

app.UseCors("CorsPolicy");

using (var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<TodoDbContext>())
{
    if (context.Database.IsNpgsql())
    {
        context.Database.Migrate();
    }
}

app.MapFallback(() => Results.Redirect("/swagger"));

app.MapGet("/todos", async (TodoDbContext db) =>
{
    return await db.Todos.ToListAsync();
});

app.MapGet("/todos/{id}", async (TodoDbContext db, int id) =>
{
    return await db.Todos.FindAsync(id) switch
    {
        Todo todo => Results.Ok(todo),
        null => Results.NotFound()
    };
});

app.MapPost("/todos", async (TodoDbContext db, Todo todo) =>
{
    await db.Todos.AddAsync(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapPut("/todos/{id}", async (TodoDbContext db, int id, Todo todo) =>
{
    if (id != todo.Id)
    {
        return Results.BadRequest();
    }

    if (!await db.Todos.AnyAsync(x => x.Id == id))
    {
        return Results.NotFound();
    }

    db.Update(todo);
    await db.SaveChangesAsync();

    return Results.Ok(todo);
});

app.MapDelete("/todos/{id}", async (TodoDbContext db, int id) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
    {
        return Results.NotFound();
    }

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return Results.Ok(todo);
});

app.Run();
