using API.DTOs.Auth;
using API.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Twilio;
using Twilio.TwiML.Messaging;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;
using Microsoft.Extensions.Caching.Memory;
using Humanizer;


namespace API.Services
{
    public class OtpService : IOTPService
    {

        private readonly SmsSettings _smsSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly IGmailService _mail;

        public OtpService(IOptions<SmsSettings> smsOptions, IMemoryCache memoryCache, HttpClient httpClient, IGmailService mail)
        {
            _smsSettings = smsOptions.Value;
            _memoryCache = memoryCache;
            _httpClient = httpClient;
            _mail = mail;
        }

        public async Task<OtpResponse> SendOtpAsync(string phoneNumber)
        {
            var otp = GenerateOtp();
            var url = $"{_smsSettings.ApiUrl}?loginName={_smsSettings.LoginName}" +
                      $"&sign={_smsSettings.Sign}" +
                      $"&serviceTypeId={_smsSettings.ServiceTypeId}" +
                      $"&phoneNumber={phoneNumber}" +
                      $"&message={otp}" +
                      $"&brandName={_smsSettings.BrandName}";

            var response = await _httpClient.GetAsync(url);


            var jsonString = await response.Content.ReadAsStringAsync();
            var smsResponse = JsonSerializer.Deserialize<OtpResponse>(jsonString);

            if (smsResponse?.Code == 106) // Success
            {
                _memoryCache.Set($"OTP_{phoneNumber}", otp, TimeSpan.FromMinutes(5));
            }

            return smsResponse;
        }

        public bool VerifyOtp(string phoneNumber, string otp)
        {
            if (_memoryCache.TryGetValue($"OTP_{phoneNumber}", out string cachedOtp))
            {
                return cachedOtp == otp;
            }

            throw new Exception("OTP đã hết hạn");
        }
        public async Task<OtpResponse> SendOtpMailAsync(string email)
        {
            var otp = GenerateOtp();

            try
            {
                // Gửi email
                await _mail.SendEmailSendGridAsync(
                    email,
                    "Mã OTP xác thực - Tuta Spa",
                    $"<h2>Xin chào {email}</h2>" +
                    $"<p>Bạn vừa yêu cầu mã OTP để xác thực.</p>" +
                    $"<p>Mã OTP của bạn là: <strong style='font-size:18px;'>{otp}</strong></p>" +
                    "<p>Mã này có hiệu lực trong 5 phút.</p>"
                );

                // Lưu OTP vào cache để verify
                _memoryCache.Set($"OTP_{email}", otp, TimeSpan.FromMinutes(5));

                return new OtpResponse
                {
                    Code = 106,
                    Message = "OTP đã được gửi đến email thành công"
                };
            }
            catch (Exception ex)
            {
                return new OtpResponse
                {
                    Code = 500,
                    Message = $"Lỗi khi gửi OTP qua email: {ex.Message}"
                };
            }
        }
        public bool VerifyOtpMail(string email, string otp)
        {
            if (_memoryCache.TryGetValue($"OTP_{email}", out string cachedOtp))
            {
                if (cachedOtp == otp)
                {
                    // Nếu đúng thì xoá OTP khỏi cache để không dùng lại
                    _memoryCache.Remove($"OTP_{email}");
                    return true;
                }
                return false;
            }

            throw new Exception("OTP đã hết hạn hoặc không tồn tại");
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
