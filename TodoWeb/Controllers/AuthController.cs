using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using TodoWeb.Models;
using TodoWeb.Services;

namespace TodoWeb.Controllers
{
    /// <summary>
    /// 인증 API 컨트롤러
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// 로그인 API
        /// POST api/auth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.LoginAsync(request);

                if (result.Success)
                    return Ok(result);
                else
                    return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "로그인 중 오류: {Username}", request.Username);
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }

        /// <summary>
        /// 현재 사용자 정보
        /// GET api/auth/me
        /// </summary>
        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public ActionResult GetCurrentUser()
        {
            var username = User.Identity?.Name;
            return Ok(new { username, isAuthenticated = User.Identity?.IsAuthenticated });
        }
    }
}