using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Auth
{
    public class UserLoginDTO
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
