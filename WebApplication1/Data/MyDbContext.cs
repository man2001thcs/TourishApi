using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        #region Dbset
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<FullReceipt> FullReceipt { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Receipt>(entity =>
            {
                entity.ToTable(nameof(Receipt));
                entity.HasKey(receipt => receipt.ReceiptId);
                entity.Property(receipt => receipt.CreatedDate).IsRequired().HasDefaultValueSql("getutcdate()");
                entity.Property(receipt => receipt.UserId).IsRequired();
            });

            modelBuilder.Entity<FullReceipt>(entity =>
            {
                entity.ToTable("FullReceipt");
                entity.HasKey(e => new { e.ProductId, e.ReceiptId });

                entity.HasOne(e => e.Receipt)
                .WithMany(e => e.FullReceiptList)
                .HasForeignKey(e => e.ReceiptId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_FullReceipt_Receipt");

                entity.HasOne(e => e.Book)
                .WithMany(e => e.FullReceiptList)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_FullReceipt_Book");
            });
        }
    }
}
