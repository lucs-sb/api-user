using FluentValidation.TestHelper;
using User.Api.Models;
using User.Api.Validators;

namespace User.Tests.Validators;

[TestFixture]
public sealed class CreateUserModelValidatorTests
{
    private CreateUserModelValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateUserModelValidator();
    }

    private static CreateUserModel ValidModel(
        string? name = "Teste",
        string? email = "teste@email.com",
        DateOnly? birthDate = null) => new(
        Name: name,
        Email: email,
        BirthDate: birthDate ?? new DateOnly(2000, 1, 1)
    );

    [TestCase("", TestName = "Should_have_error_when_name_is_empty")]
    [TestCase(null, TestName = "Should_have_error_when_name_is_null")]
    public void Should_have_error_when_name_is_invalid(string? name)
    {
        var model = ValidModel(name);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode("NotEmptyValidator");
    }

    [Test]
    public void Should_have_error_when_name_exceeds_50_characters()
    {
        var model = ValidModel(name: new string('a', 51));
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode("MaximumLengthValidator");
    }

    [Test]
    public void Should_not_have_error_when_name_is_valid()
    {
        var model = ValidModel();
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [TestCase("", TestName = "Should_have_error_when_email_is_empty")]
    [TestCase(null, TestName = "Should_have_error_when_email_is_null")]
    public void Should_have_error_when_email_is_empty_or_null(string? email)
    {
        var model = ValidModel(email: email);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorCode("NotEmptyValidator");
    }

    [Test]
    public void Should_have_error_when_email_has_invalid_format()
    {
        var model = ValidModel(email: "email-invalido");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorCode("EmailValidator");
    }

    [Test]
    public void Should_have_error_when_email_exceeds_254_characters()
    {
        var model = ValidModel(email: new string('a', 246) + "@test.com");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorCode("MaximumLengthValidator");
    }

    [Test]
    public void Should_not_have_error_when_email_is_valid()
    {
        var model = ValidModel();
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public void Should_have_error_when_birthdate_is_null()
    {
        var model = new CreateUserModel(Name: "Teste", Email: "teste@email.com", BirthDate: null);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.BirthDate)
            .WithErrorCode("NotEmptyValidator");
    }

    [TestCase(0, TestName = "Should_have_error_when_birthdate_is_today")]
    [TestCase(1, TestName = "Should_have_error_when_birthdate_is_in_the_future")]
    public void Should_have_error_when_birthdate_is_not_in_the_past(int daysFromNow)
    {
        var model = ValidModel(birthDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysFromNow)));
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.BirthDate)
            .WithErrorCode("LessThanValidator");
    }

    [Test]
    public void Should_not_have_error_when_birthdate_is_valid()
    {
        var model = ValidModel();
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.BirthDate);
    }

    [Test]
    public void Should_not_have_any_errors_when_all_fields_are_valid()
    {
        var model = ValidModel();
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}