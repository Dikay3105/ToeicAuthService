using AuthService.Models;

namespace AuthService.Interfaces
{
    public interface IResetPasswordRepository
    {
        Task SaveResetTokenAsync(ResetPassword token);

        // Phương thức kiểm tra mã xác nhận
        Task<ResetPassword> GetResetTokenAsync(string email, string token);


        // Phương thức đánh dấu mã xác nhận là đã sử dụng
        Task MarkTokenAsUsedAsync(int tokenId);
    }
}
