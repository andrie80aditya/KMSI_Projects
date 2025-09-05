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

            // Configure Company self-referencing relationship
            modelBuilder.Entity<Company>()
                .HasOne(c => c.ParentCompany)
                .WithMany(c => c.ChildCompanies)
                .HasForeignKey(c => c.ParentCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure User relationships to avoid cycles
            modelBuilder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Site)
                .WithMany()
                .HasForeignKey(u => u.SiteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Teacher relationships
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne()
                .HasForeignKey<Teacher>(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Student relationships
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Company)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure GradeBook composite key
            modelBuilder.Entity<GradeBook>()
                .HasIndex(gb => new { gb.GradeId, gb.BookId })
                .IsUnique();

            // Configure decimal properties
            modelBuilder.Entity<TeacherPayroll>()
                .Property(tp => tp.HourlyRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TeacherPayroll>()
                .Property(tp => tp.NetSalary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StudentBilling>()
                .Property(sb => sb.TotalAmount)
                .HasPrecision(18, 2);

            // Configure audit triggers and constraints
            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => new { al.CompanyId, al.ActionDate });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Head Office Company
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    CompanyId = 1,
                    CompanyCode = "KMI",
                    CompanyName = "Kawai Music School Indonesia",
                    IsHeadOffice = true,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            );

            // Seed Default User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    CompanyId = 1,
                    Username = "admin",
                    Email = "admin@kawaimusic.id",
                    FirstName = "System",
                    LastName = "Administrator",
                    UserLevelId = 1,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            );
        }
    }
}
