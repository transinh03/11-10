using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Security;
using API.Extensions;
using API.DTOs.Auth;
using API.IService;
using static API.IService.IAuthService;

namespace API.Controllers
{
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


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "Tên đăng nhập hoặc mật khẩu không được để trống." });
            }

            var result = await _authService.AdminLogin(request);
            if (!result.IsSuccess)
            {
                return BadRequest(new { result.Message });
            }


            if (!result.FirstLogin)
            {
                var refreshToken = result.Token.RefreshToken;
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                };

                Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
            }

            return Ok(new {  UserInfo = result.User, AccessToken = result.Token.Accesstoken });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogWarning("Refresh token không tồn tại trong request.");
                    return Unauthorized(new { Message = "Token không tồn tại." });
                }

                var result = await _authService.RefreshToken(refreshToken);
                if (!result.IsSuccess)
                {
                    return Unauthorized(new { result.Message });
                }

                Response.Cookies.Append("refreshToken", result.Token.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                });

                return Ok(new { AccessToken = result.Token.Accesstoken });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong RefreshToken: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Đã xảy ra lỗi hệ thống." });
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new { Message = "Yêu cầu không hợp lệ" });
                }

                await _authService.Logout(refreshToken);
                Response.Cookies.Delete("refreshToken");
                _logger.LogInformation("Logout thành công ");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Lỗi khi thực hiện yêu cầu: " + ex.Message });
            }

        }


        [HttpPost("reset-password")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "ChangePasswordOnly")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassDTO request)
        {
            try
            {
                request.UserID = User.GetUserId();
                var changePassResult = await _authService.ChangePassword(request);

                if (changePassResult)
                    return Ok();

                return BadRequest(new { message = "Đổi mật khẩu thất bài ,  vui lòng thử lại sau" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("forgot-password-otp/send/{Email}")]
        public async Task<IActionResult> ForgotPasswordOTP([FromRoute] string Email)
        {

            try
            {
                await _authService.SendForgetPasswordOTP(Email , OTPType.Email);
                return Ok(); 

            }

            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi gửi OTP: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi gửi OTP: " + ex.Message });
            }
        }


        [HttpPost("forgot-password-otp/verifi")]
        public async Task<IActionResult> VerifiForgotPasswordOTP([FromBody] VerifiOtp verifi )
        {
            try
            {
                 var token =  await _authService.VerifiOtp(verifi.Email,verifi.Otp,OTPType.Email);
                return Ok(new {token = token});

            }

            catch (Exception ex)
            {
                return BadRequest(new { message = "Xác thực thất bại :" + ex.InnerException?.Message });
            }
        }


        [HttpPost("set-new-pass")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "ChangePasswordOnly")]
        public async Task<IActionResult> SetPass([FromBody] ForgetPassDTO request)
        {
            try
            {
                request.UserId = User.GetUserId();
                await _authService.ResetPassword(request);

                return Ok(); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Đặt lại mật khẩu thất bại ,  vui lòng thử lại sau" });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(string Email, string Pass)
        {
            try
            {
                await _authService.add(Email, Pass);
                return Ok(new { message = "Thêm thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi thêm: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi thêm: " + ex.InnerException.Message });
            }

        }
    }
}