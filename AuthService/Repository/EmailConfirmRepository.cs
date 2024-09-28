using AuthService.Data;
using AuthService.Helper;
using AuthService.Interfaces;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

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
                Code = confirmationCode,
                ExpiredAt = DateTime.UtcNow.AddMinutes(1),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false
            };

            await _context.EmailConfirms.AddAsync(emailConfirm);
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public async Task<bool> ValidateConfirmationCodeAsync(string email, string confirmationCode)
        {
            var emailConfirmation = await _context.EmailConfirms.FirstOrDefaultAsync(ec => ec.Email == email
                                                                        && ec.Code == confirmationCode
                                                                        && ec.IsUsed == true
                                                                        && ec.ExpiredAt > DateTime.UtcNow);
            return emailConfirmation != null;
        }

        public async Task<EmailConfirm> GetConfirmationAsync(string email, string confirmationCode)
        {
            // Giả sử bạn có ngữ cảnh DbContext là _context
            return await _context.EmailConfirms
                .FirstOrDefaultAsync(ec => ec.Email == email && ec.Code == confirmationCode);
        }


        public async Task UpdateConfirmationAsync(EmailConfirm emailConfirm)
        {
            _context.EmailConfirms.Update(emailConfirm);
            await _context.SaveChangesAsync();
        }

    }
}
