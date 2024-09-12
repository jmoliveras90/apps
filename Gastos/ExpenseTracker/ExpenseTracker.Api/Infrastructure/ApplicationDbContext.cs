using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Categories)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);

            
            modelBuilder.Entity<Expense>()
               .HasOne(e => e.User)
               .WithMany(u => u.Expenses)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<Expense>()
               .Property(e => e.Amount)
               .HasColumnType("decimal(18,2)");
        }
    }
}
