using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using User.Api.Mapper;
using User.Api.Model;
using User.Api.Validators;
using User.Application.DTOs.Request;
using User.Application.Interfaces.Repositories;
using User.Application.Interfaces.Services;
using User.Application.Services;
using User.Infrastructure.Repositories;
using User.Infrastructure.Repositories.Context;

UserMapper.Configure();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IValidator<CreateUserModel>, CreateUserModelValidator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/users", async (CreateUserModel model, IValidator<CreateUserModel> validator, IUserService userService, CancellationToken cancellationToken) =>
{
    var validation = await validator.ValidateAsync(model, cancellationToken);

    if (!validation.IsValid)
        return Results.ValidationProblem(validation.ToDictionary());

    var result = await userService.AddAsync(model.Adapt<CreateUserRequestDTO>(), cancellationToken);

    if (result.IsFailed)
        return Results.Problem(
            detail: result.Errors.First().Message,
            statusCode: StatusCodes.Status409Conflict);

    return Results.Created();
})
.WithName("CriarUsuario");

app.Run();