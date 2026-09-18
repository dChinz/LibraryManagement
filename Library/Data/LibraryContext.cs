using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<BorrowRecord> BorrowRecords { get; set; }
        public virtual DbSet<Fine> Fines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fine>().ToTable(nameof(Fine));
            modelBuilder.Entity<BorrowRecord>().ToTable(nameof(BorrowRecord));
            modelBuilder.Entity<Member>().ToTable(nameof(Member));
            modelBuilder.Entity<Book>().ToTable(nameof(Book));
            modelBuilder.Entity<Category>().ToTable(nameof(Category));
            modelBuilder.Entity<User>().ToTable(nameof(User));
        }
    }
}
