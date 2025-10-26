using API.DTOs.Auth;
using API.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Twilio.Clients;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OTPController : Controller
    {
        private readonly IOTPService _otp;


        public OTPController(IOTPService otp)
        {
            _otp = otp;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Index([FromQuery]string phoneNumber)
        {
            try
            {
                var result =  await _otp.SendOtpAsync(phoneNumber);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }

        }

        [HttpPost("sendMail")]
        public async Task<IActionResult> Indexx([FromQuery] string mail)
        {
            try
            {
                var result = await _otp.SendOtpMailAsync(mail);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("verify")]
        public  IActionResult Verrifi([FromBody] VerifyOTP verify)
        {
            try
            {
                var result =  _otp.VerifyOtp(verify.PhoneNumber,verify.OTP);

                return result ? Ok() : BadRequest(new { message = "Mã OTP không chính xác vui lòng thử lại"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("verifyMail")]
        public IActionResult VerifyMailOtp([FromBody] VerifiOtp verify)
        {
            try
            {
                var result = _otp.VerifyOtpMail(verify.Email, verify.Otp);
                return result
                    ? Ok(new { message = "Xác thực OTP email thành công" })
                    : BadRequest(new { message = "Mã OTP email không chính xác, vui lòng thử lại" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
