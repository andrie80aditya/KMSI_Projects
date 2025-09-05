using KMSI_Projects.Models;
using Microsoft.EntityFrameworkCore;

namespace KMSI_Projects.Data
{
    /// <summary>
    /// Application Database Context for KMSI Course Management System
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Master Data
        public DbSet<Company> Companies { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookPrice> BookPrices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLevel> UserLevels { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        // Academic
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<GradeBook> GradeBooks { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        // Scheduling & Attendance
        public DbSet<TeacherSchedule> TeacherSchedules { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        // Billing & Finance
        public DbSet<BillingPeriod> BillingPeriods { get; set; }
        public DbSet<StudentBilling> StudentBillings { get; set; }
        public DbSet<TeacherPayroll> TeacherPayrolls { get; set; }

        // Inventory
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<BookRequisition> BookRequisitions { get; set; }
        public DbSet<BookRequisitionDetail> BookRequisitionDetails { get; set; }

        // Examination
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<StudentExamination> StudentExaminations { get; set; }

        // Audit
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }

        //System
        public DbSet<SystemNotification> SystemNotifications { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Company self-referencing relationship
            modelBuilder.Entity<Company>()
                .HasOne(c => c.ParentCompany)
                .WithMany(c => c.ChildCompanies)
                .HasForeignKey(c => c.ParentCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // User business relationships - prevent cascading deletes
            modelBuilder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserLevel)
                .WithMany(ul => ul.Users)
                .HasForeignKey(u => u.UserLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Site relationship
            modelBuilder.Entity<Site>()
                .HasOne(s => s.Company)
                .WithMany(c => c.Sites)
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // =============================================
            // INDEXES for performance
            // =============================================
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Company>()
                .HasIndex(c => c.CompanyCode)
                .IsUnique();

            // =============================================
            // SEED DATA
            // =============================================
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed User Levels
            var userLevels = UserLevel.CreateDefaultLevels();
            for (int i = 0; i < userLevels.Count; i++)
            {
                userLevels[i].UserLevelId = i + 1;
            }
            modelBuilder.Entity<UserLevel>().HasData(userLevels);

            // Seed Default Company
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    CompanyId = 1,
                    CompanyCode = "KMI",
                    CompanyName = "Kawai Music School Indonesia",
                    Address = "Jakarta Head Office",
                    City = "Jakarta",
                    Province = "DKI Jakarta",
                    Phone = "021-12345678",
                    Email = "info@kmsi.co.id",
                    IsHeadOffice = true,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            );
        }
    }
}
