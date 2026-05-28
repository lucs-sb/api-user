using FluentValidation;
using User.Api.Models;

namespace User.Api.Validators;

public sealed class CreateUserModelValidator : AbstractValidator<CreateUserModel>
{
    public CreateUserModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254);

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
