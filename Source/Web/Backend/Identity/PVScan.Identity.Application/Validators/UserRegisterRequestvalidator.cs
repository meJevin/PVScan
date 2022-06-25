using FluentValidation;
using PVScan.Identity.API.Contract.Requests.User;

namespace PVScan.Identity.Application.Validators
{
    public class UserRegisterRequestvalidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestvalidator()
        {
            RuleFor(request => request.Username).NotEmpty();
            RuleFor(request => request.Password).NotEmpty();
            RuleFor(request => request.Email).NotEmpty().EmailAddress();
        }
    }
}
