using Microsoft.EntityFrameworkCore;

namespace GettingStarted.Models;
#pragma warning disable CS1591
public class TodoContext: DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
    
#pragma warning restore CS1591