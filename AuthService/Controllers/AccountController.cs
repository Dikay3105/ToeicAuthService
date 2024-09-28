using AuthService.Helper;
using AuthService.Helpers;
using AuthService.Interfaces;
using AuthService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IEmailConfirmRepository _emailConfirmRepository;
        private readonly IResetPasswordRepository _resetPasswordRepository;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public AccountController(
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IEmailConfirmRepository emailConfirmRepository,
            IResetPasswordRepository resetPasswordRepository,
            IMapper mapper,
            IOptions<AppSettings> options)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _emailConfirmRepository = emailConfirmRepository;
            _resetPasswordRepository = resetPasswordRepository;
            _mapper = mapper;
            _appSettings = options.Value;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Validate([FromBody] LoginModel model)
        {
            var user = await _accountRepository.GetUserByUsernameAsync(model.Username);
            if (user == null)
            {
                return Ok(new { EC = -1, EM = "Invalid Username" });
            }

            // Check if the password matches using the salt
            if (!PasswordHasher.VerifyPassword(model.Password, user.PasswordHash, user.Salt))
            {
                return Ok(new { EC = -1, EM = "Invalid Password" });
            }

            var token = await GenerateToken(user);
            return Ok(new { EC = 0, EM = "Login success", DT = token });
        }

        [HttpPost("Logout")]
        public IActionResult Logout([FromBody] LogoutModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                _refreshTokenRepository.RevokeTokenAsync(token); // Thu hồi access token
            }

            if (!string.IsNullOrEmpty(model.RefreshToken))
            {
                _refreshTokenRepository.RevokeTokenAsync(model.RefreshToken); // Thu hồi refresh token
            }

            return Ok(token);
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {
            var (passwordHash, salt) = PasswordHasher.HashPassword(model.PasswordHash);

            var newUser = new User
            {
                Username = model.Username,
                PasswordHash = passwordHash,
                Salt = salt,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.UtcNow,
                lastPasswordChange = ""
            };

            var checkValidMail = ValidateCheck.IsValidEmail(newUser.Email);
            if (!checkValidMail)
            {
                return StatusCode(422, new { EC = -1, EM = "Invalid email" });
            }

            var checkUsername = _userRepository.GetUsers()
                .FirstOrDefault(u => u.Username.Trim().ToUpper() == newUser.Username.Trim().ToUpper());

            if (checkUsername != null)
            {
                return StatusCode(422, new { EC = -1, EM = "Username already used" });
            }

            var checkEmail = _userRepository.GetUsers()
                .FirstOrDefault(u => u.Email.Trim().ToUpper() == newUser.Email.Trim().ToUpper());

            if (checkEmail != null)
            {
                return StatusCode(422, new { EC = -1, EM = "Email already used" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_userRepository.AddUser(newUser))
            {
                return StatusCode(422, new { EC = -1, EM = "Error saving user" });
            }

            var token = await GenerateToken(newUser);
            return Ok(new { EC = 0, EM = "Sign up successful", DT = token });
        }

        // API đổi mật khẩu
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _accountRepository.GetUserByUsernameAsync(model.Username);
            if (user == null)
            {
                return BadRequest(new { EC = -1, EM = "Người dùng không tồn tại" });
            }

            // Xác minh mật khẩu hiện tại
            if (!PasswordHasher.VerifyPassword(model.OldPassword, user.PasswordHash, user.Salt))
            {
                return BadRequest(new { EC = -1, EM = "Mật khẩu hiện tại không đúng" });
            }

            //Xác minh mật khẩu mới với mật khẩu cũ đã dùng trước đó
            if (PasswordHasher.VerifyPassword(model.NewPassword, user.lastPasswordChange, user.Salt))
            {
                return BadRequest(new { EC = -1, EM = "Mật khẩu này đã được dùng trước đó, bạn không nên sử dụng lại" });
            }

            try
            {
                // Cập nhật mật khẩu mới
                await _accountRepository.UpdatePasswordAsync(user.UserID, PasswordHasher.HashPasswordWithSalt(model.NewPassword, user.Salt));

                // Nếu cập nhật thành công
                return Ok(new { EC = 0, EM = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra trong quá trình cập nhật
                return StatusCode(500, new { EC = -1, EM = "Lỗi cập nhật mật khẩu", Details = ex.Message });
            }
        }



        private async Task<TokenModel> GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.Secretkey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserName", user.Username),
                    new Claim("Id", user.UserID.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = user.UserID,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1)
            };

            await _accountRepository.AddRefreshTokenAsync(refreshTokenEntity);

            return new TokenModel { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken([FromBody] TokenModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.Secretkey);
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };

            try
            {
                var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidationParams, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return Unauthorized(new { Success = false, Message = "Invalid token" });
                    }
                }

                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value);
                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiryDate).UtcDateTime;
                if (expiryDate > DateTime.UtcNow)
                {
                    return BadRequest(new { Success = false, Message = "Access token has not yet expired" });
                }

                var storedToken = await _accountRepository.GetRefreshTokenAsync(model.RefreshToken);
                if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked)
                {
                    return Unauthorized(new { Success = false, Message = "Invalid refresh token" });
                }

                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (storedToken.JwtId != jti)
                {
                    return Unauthorized(new { Success = false, Message = "Token mismatch" });
                }

                storedToken.IsUsed = true;
                storedToken.IsRevoked = true;
                await _accountRepository.UpdateRefreshTokenAsync(storedToken);

                var user = await _accountRepository.GetUserByIdAsync(storedToken.UserId);
                var newToken = await GenerateToken(user);

                return Ok(new { Success = true, Message = "Token renewed", Data = newToken });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = "Error occurred", Error = ex.Message });
            }
        }

        [HttpPost("SendConfirmEmailCode")]
        public async Task<IActionResult> SendConfirmEmailCode([FromBody] EmailModel model)
        {
            try
            {
                if (await _emailConfirmRepository.SendEmailConfirmationCodeAsync(model.Email))
                    return Ok(new { EC = 0, EM = "Confirmation code sent successfully" });
                return Ok(new { EC = -1, EM = "Cannot sent code" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { EC = -1, EM = "Failed to send email", Details = ex.Message });
            }
        }

        [HttpPost("CheckConfirmEmailCode")]
        public async Task<IActionResult> CheckConfirmEmailCode([FromBody] CheckEmailCodeModel model)
        {
            try
            {
                // Check if the confirmation code for the given email is correct
                var confirmation = await _emailConfirmRepository.GetConfirmationAsync(model.Email, model.ConfirmationCode);

                if (confirmation != null && !confirmation.IsUsed && confirmation.ExpiredAt > DateTime.UtcNow)
                {
                    // Mark the confirmation code as used
                    confirmation.IsUsed = true;
                    await _emailConfirmRepository.UpdateConfirmationAsync(confirmation);

                    return Ok(new { EC = 0, EM = "Confirmation code is valid" });
                }
                else
                {
                    return Ok(new { EC = -1, EM = "Invalid or expired confirmation code" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { EC = -1, EM = "Error validating confirmation code", DT = ex.Message });
            }
        }

        // API gửi mã xác nhận đổi mật khẩu tới email
        [HttpPost("SendResetCode")]
        public async Task<IActionResult> SendResetToken([FromBody] string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { EC = -1, EM = "Email không tồn tại" });
            }

            // Tạo mã xác nhận mới
            var resetToken = new ResetPassword
            {
                UserId = user.UserID,
                Token = RNG.GenerateSixDigitNumber().ToString(),
                ExpirationDate = DateTime.UtcNow.AddMinutes(1), // Mã hết hạn sau 1 phút
                CreatedAt = DateTime.UtcNow,
                Used = false
            };

            await _resetPasswordRepository.SaveResetTokenAsync(resetToken);

            // Gửi email (Giả sử `SendEmailAsync` là phương thức gửi email)
            await SendMail.SendPasswordResetEmailAsync(email, resetToken.Token);

            return Ok(new { EC = 0, EM = "Mã xác nhận đã được gửi qua email" });
        }

        //API kiểm tra mã xác nhận
        [HttpPost("VerifyResetCode")]
        public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenModel model)
        {
            var token = await _resetPasswordRepository.GetResetTokenAsync(model.Email, model.Token);

            if (token == null)
            {
                return BadRequest(new { EC = -1, EM = "Mã xác nhận không hợp lệ hoặc đã hết hạn" });
            }

            return Ok(new { EC = 0, EM = "Mã xác nhận hợp lệ", TokenId = token.Id });
        }

        //API cập nhật mật khẩu mới
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var token = await _resetPasswordRepository.GetResetTokenAsync(model.Email, model.Token);
            var user = await _userRepository.GetUserByEmailAsync(model.Email);

            if (token == null || token.Used || token.ExpirationDate <= DateTime.UtcNow)
            {
                return BadRequest(new { EC = -1, EM = "Mã xác nhận không hợp lệ hoặc đã hết hạn" });
            }

            // Cập nhật mật khẩu mới (giả sử mật khẩu đã được hash trước khi gọi API)
            await _accountRepository.UpdatePasswordAsync(token.UserId, PasswordHasher.HashPasswordWithSalt(model.NewPassword, user.Salt));

            // Đánh dấu mã xác nhận là đã sử dụng
            await _resetPasswordRepository.MarkTokenAsUsedAsync(token.Id);

            return Ok(new { EC = 0, EM = "Mật khẩu đã được cập nhật thành công" });
        }


    }
    public class EmailModel
    {
        public string Email { get; set; }
    }

    public class CheckEmailCodeModel
    {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
    }

    public class VerifyResetTokenModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; } // Mật khẩu đã được hash
    }

    public class ChangePasswordModel
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; } // Mật khẩu mới (đã hash)
    }

}
