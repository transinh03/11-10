
using API.DTOs.Response;
using API.DTOs.Auth;
using Microsoft.AspNetCore.Identity.Data;

namespace API.IService
{
    public interface IAuthService
    {
        public Task<AuthResponse> AdminLogin(LoginDTO request);

        public Task<AuthResponse> UserLogin(UserLoginDTO request);

        public Task<AuthResponse> Resgister(RegisterDTO request);

        public Task<AuthResponse> RefreshToken(string refreshToken);

        public Task ResetPassword(ForgetPassDTO reset); 

        public Task  SendForgetPasswordOTP(string email , OTPType type);

        public Task<string> VerifiOtp(string email, string otp , OTPType type);

        public Task add(string email, string pass); 

        public Task<bool> ChangePassword(ResetPassDTO resetPassDTO);
        public Task Logout(string refreshToken);

        public Task<bool> CheckPhoneNumberExists(string phone);



        public enum OTPType
        {
            Email ,
            Phone
        }
    }
}
