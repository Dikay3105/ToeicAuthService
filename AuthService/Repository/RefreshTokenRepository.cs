using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _context;

        public RefreshTokenRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == token);
        }


        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeTokenAsync(string token)
        {
            var refreshToken = await GetRefreshTokenAsync(token);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await UpdateRefreshTokenAsync(refreshToken);
            }
        }
    }
}
