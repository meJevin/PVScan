using FluentValidation;
using PVScan.Identity.API.Contract.Requests.User;

namespace PVScan.Identity.Application.Validators
{
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(request => request.Username).NotEmpty();
            RuleFor(request => request.Password).NotEmpty();
        }
    }
}
