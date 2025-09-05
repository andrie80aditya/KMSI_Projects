using KMSI_Projects.Data;
using KMSI_Projects.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace KMSI_Projects.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Company)
                    .Include(u => u.Site)
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

                if (user == null || !VerifyPassword(password, user.PasswordHash))
                {
                    return null;
                }

                // Update last login
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Authentication failed for user: {username}");
                return null;
            }
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Site)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Site)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> CreateUserAsync(User user, string password)
        {
            try
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    return false;
                }

                // Check if email already exists
                if (!string.IsNullOrEmpty(user.Email) &&
                    await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    return false;
                }

                user.PasswordHash = HashPassword(password);
                user.CreatedDate = DateTime.Now;
                user.IsActive = true;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create user: {user.Username}");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.UserId);
                if (existingUser == null) return false;

                // Update properties (except password and system fields)
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.UserLevelId = user.UserLevelId;
                existingUser.SiteId = user.SiteId;
                existingUser.IsActive = user.IsActive;
                existingUser.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update user: {user.UserId}");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || !VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return false;
                }

                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to change password for user: {userId}");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to reset password for user: {userId}");
                return false;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByCompanyAsync(int companyId)
        {
            return await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Site)
                .Where(u => u.CompanyId == companyId)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersBySiteAsync(int siteId)
        {
            return await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Site)
                .Where(u => u.SiteId == siteId)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var saltedPassword = password + "KMSI_SALT_2025"; // Use a proper salt in production
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hashedPassword);
        }
    }

    // Audit Service Interface and Implementation
    public interface IAuditService
    {
        Task LogAsync(string tableName, int recordId, string action, object? oldValues = null, object? newValues = null);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int companyId, int? days = 30);
        Task<IEnumerable<AuditLog>> GetEntityAuditLogsAsync(string tableName, int recordId);
    }

    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuditService> _logger;

        public AuditService(ApplicationDbContext context, ILogger<AuditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(string tableName, int recordId, string action, object? oldValues = null, object? newValues = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    CompanyId = 1, // This should come from current context
                    UserId = 1, // This should come from current context
                    TableName = tableName,
                    RecordId = recordId,
                    Action = action,
                    OldValues = oldValues != null ? System.Text.Json.JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues != null ? System.Text.Json.JsonSerializer.Serialize(newValues) : null
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit trail");
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int companyId, int? days = 30)
        {
            var startDate = DateTime.Now.AddDays(-(days ?? 30));

            return await _context.AuditLogs
                .Include(al => al.User)
                .Where(al => al.CompanyId == companyId && al.ActionDate >= startDate)
                .OrderByDescending(al => al.ActionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetEntityAuditLogsAsync(string tableName, int recordId)
        {
            return await _context.AuditLogs
                .Include(al => al.User)
                .Where(al => al.TableName == tableName && al.RecordId == recordId)
                .OrderByDescending(al => al.ActionDate)
                .ToListAsync();
        }
    }
}
