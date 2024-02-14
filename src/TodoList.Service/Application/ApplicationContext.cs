using Microsoft.EntityFrameworkCore;

namespace TodoList.Service.Application;

public class ApplicationContext : DbContext
{
    public DbSet<Notebook> Notebooks { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notebook>(notebook =>
        {
            notebook.ToTable("Notebooks");
            notebook.HasKey(x => x.Id);
            notebook.Property(x => x.Title)
                .IsRequired();
            notebook.Property(x => x.Description)
                .IsRequired();
           
        });
    }
}
