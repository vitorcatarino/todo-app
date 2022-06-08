using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoAPI.Models;

namespace TodoAPI.Tests;

class TodoApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var root = new InMemoryDatabaseRoot();
 
        builder.ConfigureServices(services => 
        {
            services.AddScoped(sp =>
            {
                // Replace PostgreSQL with the in memory provider for tests
                return new DbContextOptionsBuilder<TodoDbContext>()
                            .UseInMemoryDatabase("Tests", root)
                            .UseApplicationServiceProvider(sp)
                            .Options;
            });
        });
 
        return base.CreateHost(builder);
    }
}