
﻿using API.DTOs.Auth;
using API.Extensions;
using API.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AccountController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {

            if (request == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            var result = await _authService.Resgister(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "Tên đăng nhập hoặc mật khẩu không được để trống." });
            }

            var result = await _authService.UserLogin(request);
            if (!result.IsSuccess)
            {
                return BadRequest(new { result.Message });
            }

                var refreshToken = result.Token.RefreshToken;
                Console.WriteLine(nameof(refreshToken) +":" + refreshToken );
                var cookieOptions = new CookieOptions
                {
                     HttpOnly = true,
                     Secure = true,
                     SameSite = SameSiteMode.None,
                     Expires = DateTime.UtcNow.AddDays(7),
                     Path = "/api/account"
                };

            Response.Cookies.Append("User_refreshToken", refreshToken, cookieOptions);


            return Ok(new {UserInfo = result.User , AccessToken = result.Token.Accesstoken });
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["User_refreshToken"];
                Console.WriteLine(refreshToken);
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

                Response.Cookies.Append("User_refreshToken", result.Token.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/api/account"
                });

                return Ok(new { AccessToken = result.Token.Accesstoken });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi trong RefreshToken: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Đã xảy ra lỗi hệ thống." });
            }
        }


        [HttpGet("phone/check")]
        public async Task<IActionResult> CheckPhoneNumber([FromQuery] string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return BadRequest(new { message = "Số điện thoại không được để trống." });
            }
            var isExist = await _authService.CheckPhoneNumberExists(phoneNumber);
            return Ok(new { exists = isExist });
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


        [HttpPost("forgot-password-otp/send/{phoneNumber}")]
        public async Task<IActionResult> ForgotPasswordOTP([FromRoute] string phoneNumber)
        {

            try
            {
                await _authService.SendForgetPasswordOTP(phoneNumber, IAuthService.OTPType.Phone);
                return Ok();

            }

            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi gửi OTP: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi gửi OTP: " + ex.Message });
            }
        }
        [HttpPost("forgot-password-otp/sendMail/{mail}")]
        public async Task<IActionResult> ForgotPasswordOTPMail([FromRoute] string mail)
        {

            try
            {
                await _authService.SendForgetPasswordOTP(mail, IAuthService.OTPType.Email);
                return Ok();

            }

            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi gửi OTP: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi gửi OTP: " + ex.Message });
            }
        }


        [HttpPost("forgot-password-otp/verifi")]
        public async Task<IActionResult> VerifiForgotPasswordOTP([FromBody] VerifiOtp verifi)
        {
            try
            {
                var token = await _authService.VerifiOtp(verifi.Email, verifi.Otp, IAuthService.OTPType.Phone);
                return Ok(new { token = token });

            }

            catch (Exception ex)
            {
                return BadRequest(new { message = "Xác thực thất bại :" + ex.InnerException?.Message });
            }
        }
        [HttpPost("forgot-password-otp/verifiMail")]
        public async Task<IActionResult> VerifiForgotPasswordOTPMail([FromBody] VerifiOtp verifi)
        {
            try
            {
                var token = await _authService.VerifiOtp(verifi.Email, verifi.Otp, IAuthService.OTPType.Email);
                return Ok(new { token = token });

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
    }
}
