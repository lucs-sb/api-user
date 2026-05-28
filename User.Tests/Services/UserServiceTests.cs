using FluentAssertions;
using FluentResults;
using Moq;
using User.Application.DTOs.Request;
using User.Application.Interfaces.Repositories;
using User.Application.Services;

namespace User.Tests.Services;

[TestFixture]
public sealed class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private UserService _userService;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    private static CreateUserRequestDTO CreateRequest(string email = "teste@email.com") => new(
        Name: "Teste",
        Email: email,
        BirthDate: new DateOnly(1995, 1, 1)
    );

    private void SetupEmailNotRegistered(string email) =>
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);

    private void SetupEmailAlreadyRegistered(string email) =>
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.User { Name = "Existing", Email = email, BirthDate = new DateOnly(1990, 1, 1) });

    [Test]
    public async Task Should_return_success_when_email_is_not_registered()
    {
        var request = CreateRequest();
        SetupEmailNotRegistered(request.Email);

        var result = await _userService.AddAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Should_add_user_when_email_is_not_registered()
    {
        var request = CreateRequest();
        SetupEmailNotRegistered(request.Email);

        await _userService.AddAsync(request);

        _userRepositoryMock.Verify(r => r.AddAsync(
            It.Is<Domain.Entities.User>(u =>
                u.Name == request.Name &&
                u.Email == request.Email &&
                u.BirthDate == request.BirthDate),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Should_return_failure_when_email_is_already_registered()
    {
        var request = CreateRequest();
        SetupEmailAlreadyRegistered(request.Email);

        var result = await _userService.AddAsync(request);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("E-mail já está cadastrado.");
    }

    [Test]
    public async Task Should_not_add_user_when_email_is_already_registered()
    {
        var request = CreateRequest();
        SetupEmailAlreadyRegistered(request.Email);

        await _userService.AddAsync(request);

        _userRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<Domain.Entities.User>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Should_call_repository_with_correct_email_when_checking_duplicate()
    {
        const string email = "teste@email.com";
        var request = CreateRequest(email);
        SetupEmailNotRegistered(email);

        await _userService.AddAsync(request);

        _userRepositoryMock.Verify(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
    }
}
