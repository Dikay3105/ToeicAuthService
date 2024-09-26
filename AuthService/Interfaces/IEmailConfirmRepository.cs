namespace AuthService.Interfaces
{
    public interface IEmailConfirmRepository
    {
        // Gửi mã xác nhận tới địa chỉ email của người dùng
        Task SendEmailConfirmationCodeAsync(string email);

        // Xác nhận mã xác nhận email
        Task<bool> ConfirmEmailCodeAsync(string email, string confirmationCode);
    }
}
