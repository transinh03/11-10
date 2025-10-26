namespace API.DTOs.Auth
{
    public class VerifyOTP
    {
        public string PhoneNumber { get; set; }
        public string OTP { get; set; }
    }
}
