using API.DTOs.Auth;

namespace API.IService
{
    public interface IOTPService
    {
        Task<OtpResponse> SendOtpAsync(string phoneNumber);
        bool VerifyOtp(string phoneNumber, string otp);
        Task<OtpResponse> SendOtpMailAsync(string mail);
        bool VerifyOtpMail(string email, string otp);

    }

}
