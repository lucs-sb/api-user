using FluentValidation;
using User.Api.Model;

namespace User.Api.Validators;

public sealed class UpdateUserModelValidator : AbstractValidator<UpdateUserModel>
{
    public UpdateUserModelValidator()
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
