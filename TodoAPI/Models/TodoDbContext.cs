using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}
