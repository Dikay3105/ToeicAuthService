using AuthService.Models;

namespace AuthService.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task RevokeTokenAsync(string token);
    }
}
