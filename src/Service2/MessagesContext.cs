using Microsoft.EntityFrameworkCore;

namespace Shared;

public class MessagesContext : DbContext
{
    public MessagesContext(DbContextOptions<MessagesContext> options) : base(options)
    {
    }
    public virtual DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityColumns();
        modelBuilder.Entity<Message>().ToTable("Message").HasKey(e => e.Id);
        modelBuilder.Entity<Message>().Property(e => e.Id).ValueGeneratedOnAdd();
    }

    public void EnsureCreated()
    {
        Database.EnsureCreated();
    }
}
