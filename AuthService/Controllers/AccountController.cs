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
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public AccountController(
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IMapper mapper,
            IOptions<AppSettings> options)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
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
            var existingUser = await _accountRepository.GetUserByUsernameAsync(model.Username);
            if (existingUser != null)
            {
                return BadRequest(new { EC = 1, EM = "Username already exists" });
            }

            var (passwordHash, salt) = PasswordHasher.HashPassword(model.PasswordHash);

            var newUser = new User
            {
                Username = model.Username,
                PasswordHash = passwordHash,
                Salt = salt,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.UtcNow
            };

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
            var confirmationCode = RNG.GenerateSixDigitNumber().ToString();

            try
            {
                await SendMail.SendVerificationEmailAsync(model.Email, confirmationCode);
                return Ok(new { EC = 0, EM = "Confirmation code sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { EC = -1, EM = "Failed to send email", Details = ex.Message });
            }
        }
    }
    public class EmailModel
    {
        public string Email { get; set; }
    }
}
