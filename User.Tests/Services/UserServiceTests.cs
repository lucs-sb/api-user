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

    [TestCase(10, 10, 1, TestName = "Should_return_one_page_when_records_equal_page_size")]
    [TestCase(11, 10, 2, TestName = "Should_return_two_pages_when_records_exceed_page_size")]
    [TestCase(0, 10, 0, TestName = "Should_return_zero_pages_when_there_are_no_records")]
    public async Task Should_calculate_total_pages_correctly(int totalRecords, int pageSize, int expectedTotalPages)
    {
        _userRepositoryMock
            .Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalRecords);

        _userRepositoryMock
            .Setup(r => r.GetAllAsync(1, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _userService.GetAllAsync(1, pageSize);

        result.TotalPages.Should().Be(expectedTotalPages);
    }

    [Test]
    public async Task Should_return_correct_pagination_metadata()
    {
        _userRepositoryMock
            .Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(25);

        _userRepositoryMock
            .Setup(r => r.GetAllAsync(2, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _userService.GetAllAsync(page: 2, pageSize: 10);

        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalRecords.Should().Be(25);
        result.TotalPages.Should().Be(3);
    }

    [Test]
    public async Task Should_return_empty_data_when_there_are_no_users()
    {
        _userRepositoryMock
            .Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _userRepositoryMock
            .Setup(r => r.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _userService.GetAllAsync(1, 10);

        result.Data.Should().BeEmpty();
    }

    [Test]
    public async Task Should_map_user_fields_correctly()
    {
        var user = new Domain.Entities.User { Id = 1, Name = "Teste", Email = "teste@email.com", BirthDate = new DateOnly(1995, 1, 1) };

        _userRepositoryMock
            .Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _userRepositoryMock
            .Setup(r => r.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync([user]);

        var result = await _userService.GetAllAsync(1, 10);

        var dto = result.Data!.First();
        dto.Id.Should().Be(user.Id);
        dto.Name.Should().Be(user.Name);
        dto.Email.Should().Be(user.Email);
        dto.BirthDate.Should().Be(user.BirthDate);
    }

    [Test]
    public async Task Should_return_failure_when_deleting_nonexistent_user()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        var result = await _userService.DeleteAsync(1);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("Usuário não encontrado.");
    }

    [Test]
    public async Task Should_delete_user_when_found()
    {
        var user = new Domain.Entities.User { Id = 1, Name = "Teste", Email = "teste@email.com", BirthDate = new DateOnly(1995, 1, 1) };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _userService.DeleteAsync(1);

        result.IsSuccess.Should().BeTrue();
        _userRepositoryMock.Verify(r => r.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Should_return_null_when_user_is_not_found()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        var result = await _userService.GetByIdAsync(1);

        result.Should().BeNull();
    }

    [Test]
    public async Task Should_return_user_when_found_by_id()
    {
        var user = new Domain.Entities.User { Id = 1, Name = "Teste", Email = "teste@email.com", BirthDate = new DateOnly(1995, 1, 1) };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _userService.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
        result.BirthDate.Should().Be(user.BirthDate);
    }
}
