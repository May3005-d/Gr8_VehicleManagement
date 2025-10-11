using Gr8_VehicleManagement.Data.Entities;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for all 15 tables
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<VehicleVersion> VehicleVersions { get; set; }
        public DbSet<VehicleInventory> VehicleInventories { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<SalesTarget> SalesTargets { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // USER & ROLES CONFIGURATION
            // ==========================================
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Password).HasMaxLength(500);
                entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.UserType).HasDefaultValue(UserType.Customer);
                // Removed Avatar and MfaSecret configuration
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                
                // Relationship with Dealer
                entity.HasOne(e => e.Dealer)
                    .WithMany(d => d.Users)
                    .HasForeignKey(e => e.DealerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ==========================================
            // DEALER CONFIGURATION
            // ==========================================
            
            modelBuilder.Entity<Dealer>(entity =>
            {
                entity.ToTable("Dealers");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(500).IsRequired();
                entity.Property(e => e.City).HasMaxLength(100).IsRequired();
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
            });

            // ==========================================
            // VEHICLE CONFIGURATION
            // ==========================================
            
            modelBuilder.Entity<VehicleModel>(entity =>
            {
                entity.ToTable("VehicleModels");
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<VehicleVersion>(entity =>
            {
                entity.ToTable("VehicleVersions");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.VersionName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ColorName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ColorCode).HasMaxLength(20);
                
                entity.Property(e => e.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.SellingPrice).HasColumnType("decimal(18,2)").IsRequired();
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.Model)
                    .WithMany(m => m.VehicleVersions)
                    .HasForeignKey(e => e.ModelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<VehicleInventory>(entity =>
            {
                entity.ToTable("VehicleInventory");
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.VIN).IsUnique();
                entity.Property(e => e.VIN).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(200);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.Version)
                    .WithMany(v => v.VehicleInventories)
                    .HasForeignKey(e => e.VersionId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Dealer)
                    .WithMany(d => d.VehicleInventories)
                    .HasForeignKey(e => e.DealerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<VehicleImage>(entity =>
            {
                entity.ToTable("VehicleImages");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.ImageUrl).HasMaxLength(500).IsRequired();
                entity.Property(e => e.ImageType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.VehicleModel)
                    .WithMany(m => m.VehicleImages)
                    .HasForeignKey(e => e.VehicleModelId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.VehicleVersion)
                    .WithMany(v => v.VehicleImages)
                    .HasForeignKey(e => e.VehicleVersionId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.VehicleInventory)
                    .WithMany(i => i.VehicleImages)
                    .HasForeignKey(e => e.VehicleInventoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // CUSTOMER CONFIGURATION
            // ==========================================
            
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                
                entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.Source).HasMaxLength(50);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.AssignedUser)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ==========================================
            // ORDER & PAYMENT CONFIGURATION
            // ==========================================
            
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.OrderCode).IsUnique();
                entity.Property(e => e.OrderCode).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ContractNumber).HasMaxLength(50);
                
                entity.Property(e => e.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.DepositAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.RemainingAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.MonthlyPayment).HasColumnType("decimal(18,2)");
                entity.Property(e => e.InterestRate).HasColumnType("decimal(5,2)");
                
                entity.Property(e => e.OrderDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.Property(e => e.InstallmentProvider).HasMaxLength(200);
                
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Dealer)
                    .WithMany(d => d.Orders)
                    .HasForeignKey(e => e.DealerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Creator)
                    .WithMany(u => u.CreatedOrders)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.VehicleVersion)
                    .WithMany(v => v.Orders)
                    .HasForeignKey(e => e.VersionId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.VehicleInventory)
                    .WithMany(i => i.Orders)
                    .HasForeignKey(e => e.VehicleInventoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.PaymentNumber).IsUnique();
                entity.Property(e => e.PaymentNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.TransactionId).HasMaxLength(100);
                entity.Property(e => e.PaymentProof).HasMaxLength(500);
                
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.LateFee).HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.PaymentDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Payments)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Processor)
                    .WithMany(u => u.ProcessedPayments)
                    .HasForeignKey(e => e.ProcessedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ==========================================
            // PROMOTION CONFIGURATION
            // ==========================================
            
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("Promotions");
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                
                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.MinOrderAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MaxDiscountAmount).HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
            });

            // ==========================================
            // SALES TARGET CONFIGURATION
            // ==========================================
            
            modelBuilder.Entity<SalesTarget>(entity =>
            {
                entity.ToTable("SalesTargets");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.TargetAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.AchievementAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                entity.Property(e => e.CommissionRate).HasColumnType("decimal(5,2)").HasDefaultValue(0);
                entity.Property(e => e.CommissionEarned).HasColumnType("decimal(18,2)").HasDefaultValue(0);
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.Dealer)
                    .WithMany(d => d.SalesTargets)
                    .HasForeignKey(e => e.DealerId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.SalesTargets)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ==========================================
            // FEEDBACK CONFIGURATION
            // ==========================================
            
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedbacks");
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.FeedbackNumber).IsUnique();
                entity.Property(e => e.FeedbackNumber).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired(); // Changed from Subject
                
                entity.Property(e => e.Compensation).HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Feedbacks)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(e => e.Dealer)
                    .WithMany(d => d.Feedbacks)
                    .HasForeignKey(e => e.DealerId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.AssignedUser)
                    .WithMany(u => u.AssignedFeedbacks)
                    .HasForeignKey(e => e.AssignedUserId) // Changed from AssignedTo
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ==========================================
            // AUDIT LOG CONFIGURATION
            // ==========================================
            
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.UserEmail).HasMaxLength(256);
                entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
                entity.Property(e => e.EntityType).HasMaxLength(50);
                entity.Property(e => e.EntityId).HasMaxLength(50);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.Result).HasMaxLength(20).IsRequired();
                
                entity.Property(e => e.Timestamp).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}

