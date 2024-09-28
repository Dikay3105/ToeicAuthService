using AuthService.Data;
using AuthService.Interfaces;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repository
{
    public class ResetPasswordRepository : IResetPasswordRepository
    {
        private readonly AuthDbContext _context;

        public ResetPasswordRepository(AuthDbContext context)
        {
            _context = context;
        }

        // Lưu mã xác nhận đổi mật khẩu
        public async Task SaveResetTokenAsync(ResetPassword token)
        {
            await _context.ResetPasswords.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        // Kiểm tra mã xác nhận đổi mật khẩu và trả về token nếu hợp lệ
        public async Task<ResetPassword> GetResetTokenAsync(string email, string token)
        {
            return await _context.ResetPasswords
                .Include(t => t.User) // Đảm bảo bao gồm thông tin người dùng liên kết
                .FirstOrDefaultAsync(t => t.User.Email == email && t.Token == token && !t.Used && t.ExpirationDate > DateTime.UtcNow);
        }



        // Đánh dấu mã xác nhận đã được sử dụng
        public async Task MarkTokenAsUsedAsync(int tokenId)
        {
            var token = await _context.ResetPasswords.FindAsync(tokenId);
            if (token != null)
            {
                token.Used = true; // Đánh dấu token là đã sử dụng
                await _context.SaveChangesAsync();
            }
        }
    }
}
