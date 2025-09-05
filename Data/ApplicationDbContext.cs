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

            // Company self - referencing relationship
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
            // STUDENT-GRADE RELATIONSHIPS CONFIGURATION
            // =============================================

            // Explicit configuration for Grade.CurrentStudents relationship
            modelBuilder.Entity<Grade>()
                .HasMany(g => g.CurrentStudents)
                .WithOne(s => s.CurrentGrade)
                .HasForeignKey(s => s.CurrentGradeId)
                .OnDelete(DeleteBehavior.SetNull); // Set to null when grade is deleted

            // Student relationships - prevent cascading deletes
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Company)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Site)
                .WithMany(st => st.Students)
                .HasForeignKey(s => s.SiteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.AssignedTeacher)
                .WithMany(t => t.AssignedStudents)
                .HasForeignKey(s => s.AssignedTeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Grade relationships
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Company)
                .WithMany(c => c.Grades)
                .HasForeignKey(g => g.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student Grade History relationships
            modelBuilder.Entity<StudentGradeHistory>()
                .HasOne(sgh => sgh.Student)
                .WithMany(s => s.StudentGradeHistories)
                .HasForeignKey(sgh => sgh.StudentId)
                .OnDelete(DeleteBehavior.Cascade); // Delete history when student is deleted

            modelBuilder.Entity<StudentGradeHistory>()
                .HasOne(sgh => sgh.Grade)
                .WithMany(g => g.StudentGradeHistories)
                .HasForeignKey(sgh => sgh.GradeId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete grade if there's history

            // Grade Book relationships
            modelBuilder.Entity<GradeBook>()
                .HasOne(gb => gb.Grade)
                .WithMany(g => g.GradeBooks)
                .HasForeignKey(gb => gb.GradeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GradeBook>()
                .HasOne(gb => gb.Book)
                .WithMany(b => b.GradeBooks)
                .HasForeignKey(gb => gb.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // =============================================
            // AUDIT TRAIL RELATIONSHIPS
            // =============================================

            // Student audit trail relationships
            modelBuilder.Entity<Student>()
                .HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.UpdatedByUser)
                .WithMany()
                .HasForeignKey(s => s.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Grade audit trail relationships
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.CreatedByUser)
                .WithMany()
                .HasForeignKey(g => g.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.UpdatedByUser)
                .WithMany()
                .HasForeignKey(g => g.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Teacher audit trail relationships
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.UpdatedByUser)
                .WithMany()
                .HasForeignKey(t => t.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Book audit trail relationships
            modelBuilder.Entity<Book>()
                .HasOne(b => b.CreatedByUser)
                .WithMany()
                .HasForeignKey(b => b.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.UpdatedByUser)
                .WithMany()
                .HasForeignKey(b => b.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Additional audit trail relationships for other entities
            modelBuilder.Entity<EmailTemplate>()
                .HasOne(et => et.CreatedByUser)
                .WithMany()
                .HasForeignKey(et => et.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EmailTemplate>()
                .HasOne(et => et.UpdatedByUser)
                .WithMany()
                .HasForeignKey(et => et.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentExamination>()
                .HasOne(se => se.CreatedByUser)
                .WithMany()
                .HasForeignKey(se => se.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentExamination>()
                .HasOne(se => se.UpdatedByUser)
                .WithMany()
                .HasForeignKey(se => se.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentBilling>()
                .HasOne(sb => sb.CreatedByUser)
                .WithMany()
                .HasForeignKey(sb => sb.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentBilling>()
                .HasOne(sb => sb.UpdatedByUser)
                .WithMany()
                .HasForeignKey(sb => sb.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TeacherPayroll>()
                .HasOne(tp => tp.CreatedByUser)
                .WithMany()
                .HasForeignKey(tp => tp.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TeacherPayroll>()
                .HasOne(tp => tp.UpdatedByUser)
                .WithMany()
                .HasForeignKey(tp => tp.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BookRequisition>()
                .HasOne(br => br.CreatedByUser)
                .WithMany()
                .HasForeignKey(br => br.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BookRequisition>()
                .HasOne(br => br.UpdatedByUser)
                .WithMany()
                .HasForeignKey(br => br.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // StudentGradeHistory audit trail
            modelBuilder.Entity<StudentGradeHistory>()
                .HasOne(sgh => sgh.CreatedByUser)
                .WithMany()
                .HasForeignKey(sgh => sgh.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

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
