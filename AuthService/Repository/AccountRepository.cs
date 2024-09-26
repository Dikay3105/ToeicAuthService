using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AuthDbContext _context;
        private readonly IMapper _mapper;


        public AccountRepository(AuthDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(p => p.Username == username);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.UserID == userId);
        }


    }
}
