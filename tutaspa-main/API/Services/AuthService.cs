using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.IService;
using API.DTOs.Response;
using API.DTOs.Auth;
using API.Models;
using FluentValidation;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Twilio.Rest.Api.V2010.Account;
using Humanizer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity.Data;
using static API.IService.IAuthService;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly IValidator<RegisterDTO> _validator;
        private readonly IValidator<ResetPassDTO> _reSetPassvalidator;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;
        private readonly IGmailService emailService;
        private readonly IOTPService _otpService;


        public AuthService(IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, ILogger<AuthService> logger, IValidator<RegisterDTO> v, IUserRepository userRepository, RoleManager<IdentityRole> roleManager, IValidator<ResetPassDTO> _reSetPassvalidator, IGmailService email, IMemoryCache cache, IOTPService otpService )
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
            _validator = v;
            _userRepository = userRepository;
            _roleManager = roleManager;
            this._reSetPassvalidator = _reSetPassvalidator;
            emailService = email;
            _cache = cache;
            _otpService = otpService;
        }

        public async Task<bool> ChangePassword(ResetPassDTO resetPassDTO)
        {
            var valResult = await _reSetPassvalidator.ValidateAsync(resetPassDTO);
            if (!valResult.IsValid)
            {
                _logger.LogWarning("Đổi mật khẩu thất bại do dữ liệu không hợp lệ: " + string.Join(", ", valResult.Errors.Select(e => e.ErrorMessage)));
                throw new Exception(string.Join(", ", valResult.Errors.Select(e => e.ErrorMessage)));
            }

            var user = await _userManager.FindByIdAsync(resetPassDTO.UserID);
            CheckUserNull(user);

            var checkPasswordResult =  await _userManager.CheckPasswordAsync(user, resetPassDTO.OldPassword);
            if (!checkPasswordResult)
                throw new Exception("Mật khẩu cũ không chính xác");

            var changePassResult = await _userManager.ChangePasswordAsync(user, resetPassDTO.OldPassword, resetPassDTO.NewPassword);
            if (!changePassResult.Succeeded)
            {
                _logger.LogWarning($"Đổi mật khẩu thất bại userId {user.Id} : " + string.Join(" | ", changePassResult.Errors.Select(e => e.Description)));
                throw new Exception("Đổi mật khẩu thất bại , vui lòng thử lại sau");
            }


            if (user.FisrtLogin)
            {
                user.FisrtLogin = false;
                await _userManager.UpdateAsync(user);
            }

            return true ; 

        }


        private void CheckUserNull(User user)
        {
            if (user == null || user.IsDeleted)
                throw new Exception("User không tồn tại"); 
        }

        public async Task<AuthResponse> AdminLogin(LoginDTO request)
        {
            try
            {
                _logger.LogInformation("Login attempt for username: {UserName}", request.Email);

                var user = await _userManager.FindByEmailAsync(request.Email);

                CheckUserNull(user);

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("User") || roles == null)
                    return Failure("Tài khoản không có quyền truy cập");


                var loginResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!loginResult.Succeeded)
                {
                    _logger.LogWarning($"Đăng nhập thất bại Id tài khoản : {user.Id}");
                    return Failure("Sai mật khẩu");
                }


                ////Nếu đăng nhập lần đầu , trả về token để đổi pass
                //if (user.FisrtLogin)
                //{
                //    var changePassToken = _tokenService.GenerateChangePasswordToken(user);
                //    return new AuthResponse
                //    {
                //        IsSuccess = true,
                //        FirstLogin = user.FisrtLogin,
                //        Message = "Đăng nhập lần đầu, vui lòng đổi mật khẩu để tiếp tục",
                //        Token = changePassToken

                //    };
                //}

                _logger.LogInformation($"Login successfully user :  {user.Id}");

           
                var token = await _tokenService.GenerateToken(user, roles);
                return Success("Đăng nhập thành công", token,Role : roles.FirstOrDefault(), new UserInfo
                {
                    Id = user.Id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "Unknown"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi đăng nhập tài khoản :  {UserName}", request.Email + ex.InnerException?.Message  );
                return Failure("Xảy ra lỗi :" + ex.Message);
            }
        }
        public async Task<AuthResponse> UserLogin(UserLoginDTO request)
        {
            try
            {
                _logger.LogInformation("Login attempt for username: {UserName}", request.PhoneNumber);

                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
                CheckUserNull(user);

                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("User"))
                    return Failure("Tài khoản không có quyền truy cập");

                var loginResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!loginResult.Succeeded)
                {
                    _logger.LogWarning($"Đăng nhập thất bại Id tài khoản : {user.Id}");
                    return Failure("Sai mật khẩu");
                }

                _logger.LogInformation($"Login successfully user :  {user.Id}");
                var token = await _tokenService.GenerateToken(user,await GetRoles(user));
                return Success("Đăng nhập thành công", token,null ,  await _userRepository.GetUserByIdAsync(user.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi đăng nhập tài khoản :  {UserName}", request.PhoneNumber + ex.InnerException?.Message);
                return Failure("Xảy ra lỗi :" + ex.Message);
            }
        }


        private AuthResponse Success(string Message, TokenDTO token,string Role , UserInfo? user = null)
        {
        
            return new AuthResponse
            {
                IsSuccess = true,
                Token = token,
                Message = Message,
                User = user,
                Role = Role 
            };
        }

        private AuthResponse Failure(string Message)
        {
            return new AuthResponse
            {
                IsSuccess = false,
                Token = null,
                Message = Message

            };
        }


        public async Task Logout(string refreshToken)
        {
            await _tokenService.RevokeToken(refreshToken);

            await _signInManager.SignOutAsync();
        }

        public async Task<AuthResponse> RefreshToken(string refreshToken)
        {
            try
            {
                var token = await _tokenService.CheckFreshToken(refreshToken);
                // Kiểm tra refresh token từ cơ sở dữ liệu hoặc bộ nhớ
                if (token is null)
                {
                    return Failure("Refresh token không hợp lệ hoặc đã hết hạn");
                }

                // Tìm người dùng theo UserID
                var user = await _userManager.FindByIdAsync(token.UserId);
                CheckUserNull(user); 

                // Tạo lại access token và refresh token mới
                var newToken = await _tokenService.GenerateToken(user, await GetRoles(user));


                return Success("Đăng nhập thành công", newToken,null,null);
            }
            catch (Exception ex)
            {
                return Failure("Xảy ra lỗi :" + ex.Message);
            }
        }

        private async Task<IList<string>> GetRoles(User user) => await _userManager.GetRolesAsync(user);

        public async Task add(string email , string pass) 
        {
            var user = new User()
            {
                Email = email,
                UserName = email.Replace("@gmail.com",""),
                Name = "Testing"
               
            };
            await _userManager.CreateAsync(user, pass);

            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                adminRole = new IdentityRole("Admin");
                await _roleManager.CreateAsync(adminRole);
            }

            await _userManager.AddToRoleAsync(user, "Admin");

        }

        public async Task<AuthResponse> Resgister(RegisterDTO request)
        {
            try
            {
                var valid = await _validator.ValidateAsync(request);
                if (!valid.IsValid)
                    return Failure(string.Join(", ", valid.Errors));

                var existingEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingEmail != null)
                {
                    return Failure("Email đã tồn tại, vui lòng dùng email khác.");
                }

                var existingPhone = await _userManager.FindByNameAsync(request.PhoneNumber);
                if (existingPhone != null)
                {
                    return Failure("Số điện thoại đã tồn tại, vui lòng dùng số khác.");
                }

                var newUser = new User
                {
                    PhoneNumber = request.PhoneNumber,
                    UserName = request.PhoneNumber,
                    Email = request.Email,
                    Name = request.Name,
                    FisrtLogin = false,
                };

                var result = await _userManager.CreateAsync(newUser, request.Password).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    return Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                // Kiểm tra và tạo vai trò "User" nếu chưa tồn tại
                var userRole = await _roleManager.FindByNameAsync("User");
                if (userRole == null)
                {
                    userRole = new IdentityRole("User");
                    await _roleManager.CreateAsync(userRole);
                }

                await _userManager.AddToRoleAsync(newUser, "User").ConfigureAwait(false);
                _logger.LogInformation($"Đăng kí thành công {newUser.Id}");

                return Success("Đăng kí thành công", null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi đăng kí tài khoản :  {UserName}", request.PhoneNumber + ex.InnerException?.Message);
                return Failure("Xảy ra lỗi :" + ex.Message);
            }
        }

        public async Task SendForgetPasswordOTP(string email ,OTPType type)
        {
            var user = type == OTPType.Email ?  await _userManager.FindByEmailAsync(email) : await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == email);
            CheckUserNull(user);

            switch (type)
            {
                case OTPType.Email:
                    await SendOTPViaEmail(user.Email);
                    break;
                case OTPType.Phone:
                    await SendOTPViaPhone(user.PhoneNumber);
                    break;
                default:
                    throw new Exception("Yêu cầu không hợp lệ");
            }
        }

        private async Task SendOTPViaEmail(string email)
        {
            var randPass = RandomNumberGenerator.GetInt32(111111, 999999).ToString();

            _cache.Set($"otp_{email}", randPass, DateTimeOffset.Now.AddMinutes(5));
            await emailService.SendEmailSendGridAsync(email, "Đây là mã xác nhận đổi mật khẩu của bạn :", $"<h1>Mã xác nhận có hiệu lực trong 5 phút</h1><p>Vui lòng không cung cấp cho bất kỳ ai</p><p>Mã xác nhận là: <strong>{randPass}</strong></p>");
        }
        

        private async Task SendOTPViaPhone(string phoneNumber)
        {
            await _otpService.SendOtpAsync(phoneNumber);
        }


        public async Task<string> VerifiOtp(string email, string otp , OTPType type)
        {
            var result = type == OTPType.Email ? VerifiEmailOTP(email, otp) : VerifiPhoneOTP(email, otp);

            if (!result)
                throw new Exception("Mã OTP không hợp lệ hoặc đã hết hạn");

            var user = type == OTPType.Email ? await _userManager.FindByEmailAsync(email) : await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == email);

            var token = _tokenService.GenerateChangePasswordToken(user);
            return token.Accesstoken ;        
        }

        private  bool VerifiEmailOTP(string email, string otp)
        {
            var inMemoryOtp = _cache.Get<string>($"otp_{email}");

            if (string.IsNullOrEmpty(inMemoryOtp))
                return false;

            if (inMemoryOtp != otp)
                return false;
            _cache.Remove($"otp_{email}");
            return true;
        }

        private  bool VerifiPhoneOTP(string phone, string otp)
        {
            return  _otpService.VerifyOtp(phone, otp);

        }


        public async Task ResetPassword(ForgetPassDTO reset)
        {
            var user = await _userManager.FindByIdAsync(reset.UserId);
            CheckUserNull(user);
            await _userManager.RemovePasswordAsync(user); 
            await _userManager.AddPasswordAsync(user, reset.NewPassword);

        }

        public async Task<bool> CheckPhoneNumberExists(string phone)
        {
            return await _userManager.Users.AnyAsync(u => u.PhoneNumber == phone);
        }
    }
}
