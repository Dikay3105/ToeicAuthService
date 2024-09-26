using AuthService.Models;

namespace AuthService.Interfaces
{
    public interface IAccountRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task<User> GetUserByIdAsync(int userId);
    }
}
