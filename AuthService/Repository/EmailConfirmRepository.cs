using AuthService.Data;
using AuthService.Helper;
using AuthService.Interfaces;
using AuthService.Models;

namespace AuthService.Repository
{
    public class EmailConfirmRepository : IEmailConfirmRepository
    {
        private readonly IUserRepository _userRepository; // Repository để làm việc với cơ sở dữ liệu người dùng
        private readonly AuthDbContext _context;

        public EmailConfirmRepository(IUserRepository userRepository, AuthDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<bool> SendEmailConfirmationCodeAsync(string email)
        {
            var code = RNG.GenerateSixDigitNumber().ToString();
            if (await AddEmailConfirmCodeAsync(email, code))
            {
                await SendMail.SendVerificationEmailAsync(email, code);
                return true;
            }
            return false;
        }

        public async Task<bool> AddEmailConfirmCodeAsync(string email, string confirmationCode)
        {
            var emailConfirm = new EmailConfirm
            {
                Email = email,
                Code = confirmationCode
            };

            await _context.EmailConfirms.AddAsync(emailConfirm);
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
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
