using AuthService.Models;

namespace AuthService.Interfaces
{
    public interface IEmailConfirmRepository
    {
        // Gửi mã xác nhận tới địa chỉ email của người dùng
        Task<bool> SendEmailConfirmationCodeAsync(string email);

        //Lưu mã xác nhận vào DB
        Task<bool> AddEmailConfirmCodeAsync(string email, string confirmationCode);

        //Kiểm tra mã xác nhận
        Task<bool> ValidateConfirmationCodeAsync(string email, string confirmationCode);

        //Cập nhật mã xác nhận
        Task UpdateConfirmationAsync(EmailConfirm emailConfirm);

        //Lấy mã xác nhận
        Task<EmailConfirm> GetConfirmationAsync(string email, string confirmationCode);
    }
}
