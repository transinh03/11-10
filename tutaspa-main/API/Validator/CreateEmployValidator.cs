using API.Data;
using API.DTOs.Employee;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.Validator
{
    public class CreateEmployValidator : AbstractValidator<CreateEmployeeDTO>
    {
        private readonly ApplicationDbContext _context; 
        public CreateEmployValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không đúng định dạng")
                .MustAsync(async (email, cancellationToken) =>
                {
                    return !await CheckAvailableEmail(email);
                }).WithMessage("Email đã tồn tại");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^0\d{9}$").WithMessage("Số điện thoại không đúng định dạng")
                .MustAsync(async (phoneNumber, cancellationToken) =>
                {
                    return !await CheckAvailablePhoneNumber(phoneNumber);
                }).WithMessage("Số điện thoại đã tồn tại"); 


            RuleFor(x => x.Name).NotNull().WithMessage("Tên không được để trống");
            

            RuleFor(x => x.Address).NotEmpty().WithMessage("Địa chỉ không được để trống");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Vai trò không hợp lệ")
                .NotNull().WithMessage("Vai trò không được để trống");

        }

        private Task<bool> CheckAvailableEmail(string email)
        {
            return _context.Users.AnyAsync( x => x.Email == email);

        }

        private Task<bool> CheckAvailablePhoneNumber(string phoneNumber)
        {
            return _context.Users.AnyAsync(x => x.PhoneNumber == phoneNumber);
        }



    }
}
