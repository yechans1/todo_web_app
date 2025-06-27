using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TodoWeb.Models;

namespace TodoWeb.Services
{
    /// <summary>
    /// 인증 서비스 인터페이스
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 로그인 처리
        /// </summary>
        Task<LoginResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// JWT 토큰 생성
        /// </summary>
        string GenerateToken(string username);

        /// <summary>
        /// 토큰 검증
        /// </summary>
        bool ValidateToken(string token);
    }
}