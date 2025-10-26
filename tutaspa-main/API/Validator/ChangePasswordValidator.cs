using API.DTOs.Auth;
using FluentValidation;
    using Humanizer;
    using Microsoft.AspNetCore.Identity.Data;

    namespace API.Validator
    {
        public class ChangePasswordValidator : AbstractValidator<ResetPassDTO>
        {
            public ChangePasswordValidator()
            {
                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage("Mật khẩu không được để trống")
                    .MinimumLength(9).WithMessage("Mật khẩu phải có ít nhất 9 ký tự")
                    .Matches(@"[A-Z]+").WithMessage("Mật khẩu phải chứa ít nhất một chữ hoa")
                    .Matches(@"[a-z]+").WithMessage("Mật khẩu phải chứa ít nhất một chữ thường")
                    .Matches(@"\d+").WithMessage("Mật khẩu phải chứa ít nhất một chữ số")
                    .Matches(@"[\!\@\#\$\%\^\&\*\(\)\-\+=]+").WithMessage("Mật khẩu phải chứa ít nhất một ký tự đặc biệt");

                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.NewPassword).WithMessage("Mật khẩu xác nhận không khớp");
            }
        }
    }
