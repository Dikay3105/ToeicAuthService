using System.Net;
using System.Net.Mail;

namespace AuthService.Helper
{
    public class SendMail
    {
        // Phương thức gửi email xác thực (bất đồng bộ)
        public static async Task SendVerificationEmailAsync(string toEmail, string verificationCode)
        {
            string subject = "Mã xác thực Email";

            // Tạo nội dung email cho xác thực
            string templatePath = "./Helper/MailTemplate/ConfirmEmailMail.html"; // Đường dẫn tới file HTML
            string body = await File.ReadAllTextAsync(templatePath);

            // Thay thế placeholder trong HTML bằng mã xác thực
            body = body.Replace("{VerificationCode}", verificationCode);

            await SendEmailAsync(toEmail, subject, body);
        }

        // Phương thức gửi email đặt lại mật khẩu (bất đồng bộ)
        public static async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            string fromEmail = "your_email@example.com"; // Địa chỉ email của bạn
            string emailPassword = "your_email_password"; // Mật khẩu email của bạn
            string subject = "Đặt lại mật khẩu";

            // Tạo nội dung email cho đặt lại mật khẩu
            string body = $@"
            <html>
                <body>
                    <h2>Yêu cầu đặt lại mật khẩu</h2>
                    <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Vui lòng nhấp vào liên kết bên dưới để đặt lại mật khẩu của bạn:</p>
                    <a href='{resetLink}'>Đặt lại mật khẩu</a>
                    <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
                    <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>
                </body>
            </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        // Phương thức riêng để gửi email (bất đồng bộ)
        private static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            string fromEmail = "hahahahavbd@gmail.com"; // Địa chỉ email của bạn
            string emailPassword = "dqqh ajpa ulwl bpam"; // Mật khẩu email của bạn

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true; // Gửi email dưới dạng HTML

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)) // Thay thế bằng máy chủ SMTP của bạn
                {
                    smtp.Credentials = new NetworkCredential(fromEmail, emailPassword);
                    smtp.EnableSsl = true; // Kích hoạt SSL

                    try
                    {
                        await smtp.SendMailAsync(mail); // Gọi phương thức SendMailAsync bất đồng bộ
                    }
                    catch (Exception ex)
                    {
                        // Ghi log hoặc xử lý lỗi tại đây
                        throw new Exception($"Failed to send email: {ex.Message}", ex);
                    }
                }
            }
        }

    }
}
