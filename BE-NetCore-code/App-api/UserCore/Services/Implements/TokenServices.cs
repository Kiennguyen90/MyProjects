using CryptoInfrastructure;
using CryptoInfrastructure.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserCore.Services.Interfaces;

namespace UserCore.Services.Implements
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenServices> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserDbContext _userDbContext;
        public TokenServices(IConfiguration configuration, ILogger<TokenServices> logger, UserManager<ApplicationUser> userManager, UserDbContext userDbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _userDbContext = userDbContext;
        }
        public Task<string> GenerateAccessToken(ApplicationUser user)
        {
            var token = GenerateJWToken(user, "JwtAccessToken");
            return Task.FromResult(token);
        }

        
        public async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            try
            {
                var tokenValue = GenerateRandomTokenValue();
                var refreshtoken = await _userDbContext.ApplicationUserTokens.FirstOrDefaultAsync(x => x.UserId == user.Id && x.LoginProvider == "InternalProvider" && x.Name == "RefreshToken");
                if (refreshtoken != null)
                {
                    refreshtoken.Value = tokenValue;
                    refreshtoken.ExpireTime = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("ExpiryDuration:ExpiryDuration"));
                    _userDbContext.ApplicationUserTokens.Update(refreshtoken);
                }
                else
                {
                    var token = new ApplicationUserToken
                    {
                        UserId = user.Id,
                        LoginProvider = "InternalProvider",
                        Name = "RefreshToken",
                        Value = tokenValue,
                        CreatedAt = DateTime.UtcNow,
                        ExpireTime = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("ExpiryDuration:ExpiryDuration")),
                    };
                    await _userDbContext.ApplicationUserTokens.AddAsync(token);
                }
                await _userDbContext.SaveChangesAsync();
                return tokenValue;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating refresh token for user {user.Id}, Exception Message:{ex.Message}");
                return $"Error generating refresh token for user {user.Id}";
            }
            
        }

        public async Task<string> GeneratedAccessTokenbyRefreshToken(string refreshToken, string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Constants.StatusCode.UserNotFound;
                }

                var token = await _userDbContext.ApplicationUserTokens.FirstOrDefaultAsync(x => x.Value == refreshToken && x.UserId == user.Id);
                if (token == null)
                {
                    return Constants.StatusCode.RefreshTokenNotFound;
                }
                if(token.ExpireTime < DateTime.UtcNow)
                {
                    return Constants.StatusCode.RefreshTokenExpired;
                }
                var accessToken = GenerateJWToken(user, "JwtAccessToken");
                return accessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error GeneratedAccessTokenbyRefreshToken {refreshToken}, Exception Message:{ex.Message}");
                return $"Error GeneratedAccessTokenbyRefreshToken {refreshToken}";
            }
        }

        public async Task<bool> RemoveTokens(string userId)
        {
            try
            {
                var tokens = await _userDbContext.ApplicationUserTokens.Where(x => x.UserId == userId).ToListAsync();
                if (tokens != null && tokens.Count > 0)
                {
                    _userDbContext.ApplicationUserTokens.RemoveRange(tokens);
                    await _userDbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error RemoveTokens, Exception Message:{ex.Message}");
                return false;
            }
        }
        public Task<bool> ValidateAccessToken(string token, ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        private string GenerateJWToken(ApplicationUser user, string tokenType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email??""),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("UserNameClaim", user.FullName??"")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[$"{tokenType}:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration[$"{tokenType}:Issuer"] ?? "",
                audience: _configuration[$"{tokenType}:Audience"] ?? "",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>($"{tokenType}:ExpiryDuration")),
                signingCredentials: creds);
            return tokenHandler.WriteToken(token);
        }
        private string GenerateRandomTokenValue(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
