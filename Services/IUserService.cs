using KMSI_Projects.Models;

namespace KMSI_Projects.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> CreateUserAsync(User user, string password);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
        Task<IEnumerable<User>> GetUsersByCompanyAsync(int companyId);
        Task<IEnumerable<User>> GetUsersBySiteAsync(int siteId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
