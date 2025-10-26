using API.Data;
using API.DTOs.Auth;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.Validator
{
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {

        private readonly ApplicationDbContext _context; 

        public RegisterValidator(ApplicationDbContext context)
        {
            _context = context; 

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Tên khách hàng không được để trống");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^0\d{9}$").WithMessage("Số điện thoại không đúng định dạng")
                .MustAsync( async (phoneNumber , tokcancellationen) =>
                {
                    Console.WriteLine(phoneNumber);
                    return !await CheckAvailablePhoneNumber(phoneNumber);
                }  ).WithMessage("Số điện thoại đã tồn tại")
                ;

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(9).WithMessage("Mật khẩu phải có ít nhất 9 ký tự")
                .Matches(@"[A-Z]+").WithMessage("Mật khẩu phải chứa ít nhất một chữ hoa")
                .Matches(@"[a-z]+").WithMessage("Mật khẩu phải chứa ít nhất một chữ thường")
                .Matches(@"\d+").WithMessage("Mật khẩu phải chứa ít nhất một chữ số")
                .Matches(@"[\!\@\#\$\%\^\&\*\(\)\-\+=]+").WithMessage("Mật khẩu phải chứa ít nhất một ký tự đặc biệt");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Mật khẩu xác nhận không khớp");
        }


        private Task<bool> CheckAvailablePhoneNumber(string phoneNumber)
        {
            return _context.Users.AnyAsync(x => x.PhoneNumber == phoneNumber);
        }
    }
}
