using Microsoft.Extensions.Logging.Console;
using TodoWebapp.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Logging.ClearProviders().AddSimpleConsole(opts =>
{
    opts.IncludeScopes = false;
    opts.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
    opts.ColorBehavior = LoggerColorBehavior.Disabled;
});

//builder.Services.AddSingleton<ITodoService>(new TodoService(configuration["Services:TodoAPI"]));
builder.Services.AddHttpClient<TodoService>();
builder.Services.AddSingleton<ITodoService, TodoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
