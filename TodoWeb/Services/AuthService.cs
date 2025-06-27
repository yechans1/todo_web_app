using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoWeb.Models;

namespace TodoWeb.Services
{
    /// <summary>
    /// 인증 서비스 구현
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly AdminAccount _adminAccount;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IOptions<JwtSettings> jwtSettings,
            IOptions<AdminAccount> adminAccount,
            ILogger<AuthService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _adminAccount = adminAccount.Value;
            _logger = logger;
        }

        /// <summary>
        /// 로그인 처리
        /// </summary>
        public Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("로그인 시도: {Username}", request.Username);

            // admin 계정 확인
            if (request.Username == _adminAccount.Username && 
                request.Password == _adminAccount.Password)
            {
                // JWT 토큰 생성
                var token = GenerateToken(request.Username);
                var expiresAt = DateTime.Now.AddHours(_jwtSettings.ExpirationHours);

                _logger.LogInformation("로그인 성공: {Username}", request.Username);

                return Task.FromResult(new LoginResponse
                {
                    Success = true,
                    Message = "로그인 성공",
                    Token = token,
                    ExpiresAt = expiresAt
                });
            }
            else
            {
                _logger.LogWarning("로그인 실패: {Username}", request.Username);

                return Task.FromResult(new LoginResponse
                {
                    Success = false,
                    Message = "사용자명 또는 비밀번호가 올바르지 않습니다."
                });
            }
        }

        /// <summary>
        /// JWT 토큰 생성
        /// </summary>
        public string GenerateToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim("username", username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, 
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                    ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(_jwtSettings.ExpirationHours),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("JWT 토큰 생성 완료: {Username}", username);
            return tokenString;
        }

        /// <summary>
        /// 토큰 검증
        /// </summary>
        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}