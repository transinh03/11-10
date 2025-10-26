using API.Models;
using API.Models.Chat;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // -------------------------- DbSet --------------------------
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<LoaiDichVu> LoaiDichVus { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBatch> ProductBatches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<InventoryHistory> InventoryHistories { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }

        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<DatLich> DatLiches { get; set; }
        public DbSet<ChiTietDatLich> ChiTietDatLiches { get; set; }

        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
		public DbSet<Expense> Expenses { get; set; }

		// -------------------------- Fluent API --------------------------
		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Id)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // DichVu
            modelBuilder.Entity<DichVu>(entity =>
            {
                entity.Property(e => e.Gia)
                      .HasColumnType("decimal(18,2)");
            });

            // Product
            modelBuilder.Entity<Product>()
                .Property(p => p.CurrentSellingPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductBatches)
                .WithOne(b => b.Product)
                .HasForeignKey(b => b.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductBatch
            modelBuilder.Entity<ProductBatch>()
                .Property(b => b.ImportPrice)
                .HasColumnType("decimal(18,2)");

            // InventoryHistory → ProductBatch
            modelBuilder.Entity<InventoryHistory>()
                .HasOne(h => h.Batch)
                .WithMany()
                .HasForeignKey(h => h.BatchId)
                .OnDelete(DeleteBehavior.Restrict);

            // ChatSession
            modelBuilder.Entity<ChatSession>(entity =>
            {
                entity.ToTable("ChatSessions");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.SessionId).IsUnique();
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.AdminId);
                entity.HasIndex(e => e.StartTime);
                entity.HasIndex(e => e.Status);

                entity.Property(e => e.CustomerName).HasMaxLength(100);
                entity.Property(e => e.AdminName).HasMaxLength(100);
                entity.Property(e => e.CustomerIP).HasMaxLength(45); // IPv6 max length
                entity.Property(e => e.UserAgent).HasMaxLength(500);
            });

            // ChatMessage
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("ChatMessages");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.MessageId).IsUnique();
                entity.HasIndex(e => e.SessionId);
                entity.HasIndex(e => e.FromUserId);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.IsFromAdmin);
                entity.HasIndex(e => e.Status);

                entity.Property(e => e.FromUserName).HasMaxLength(100);
                entity.Property(e => e.Message).HasMaxLength(2000);

                entity.HasOne(m => m.ChatSession)
                      .WithMany(s => s.Messages)
                      .HasForeignKey(m => m.SessionId)
                      .HasPrincipalKey(s => s.SessionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

			modelBuilder.Entity<Expense>(entity =>
			{
				entity.ToTable("Expenses");
				entity.HasKey(e => e.ExpenseId);
				entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
				entity.Property(e => e.ExpenseType).HasMaxLength(100);
				entity.Property(e => e.Date);
				entity.Property(e => e.Note).HasMaxLength(500);
			});
		}
    }
}
