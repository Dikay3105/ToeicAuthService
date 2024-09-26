using AuthService.Helper;
using AuthService.Interfaces;

namespace AuthService.Repository
{
    public class EmailConfirmRepository : IEmailConfirmRepository
    {
        private readonly IUserRepository _userRepository; // Repository để làm việc với cơ sở dữ liệu người dùng

        public EmailConfirmRepository(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task SendEmailConfirmationCodeAsync(string email)
        {
            await SendMail.SendVerificationEmailAsync(email, "929292");
        }

        public async Task<bool> ConfirmEmailCodeAsync(string email, string confirmationCode)
        {
            //// Tìm người dùng theo email
            //var user = await _userRepository.GetUserByEmailAsync(email);
            //if (user == null)
            //{
            //    return false;
            //}

            //// Kiểm tra mã xác nhận
            //if (user.ConfirmationCode == confirmationCode)
            //{
            //    user.IsEmailConfirmed = true;
            //    user.ConfirmationCode = null; // Xóa mã xác nhận sau khi xác nhận thành công
            //    await _userRepository.UpdateUserAsync(user);
            //    return true;
            //}

            return false;
        }
    }
}
