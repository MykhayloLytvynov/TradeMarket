using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Data
{
    public class TradeMarketDbContext : DbContext
    {
        public TradeMarketDbContext(DbContextOptions<TradeMarketDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptDetail> ReceiptsDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReceiptDetail>()
                .HasKey(rd => new { rd.ReceiptId, rd.ProductId });
        }
    }
}
