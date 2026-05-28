using Microsoft.EntityFrameworkCore;
using User.Api.Model;
using User.Application.DTOs.Request;
using User.Application.Interfaces.Repositories;
using User.Application.Interfaces.Services;
using User.Application.Services;
using User.Infrastructure.Repositories;
using User.Infrastructure.Repositories.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

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

app.MapPost("/users", async (CreateUserModel model, IUserService userService, CancellationToken cancellationToken) =>
{
    CreateUserRequestDTO request = new(model.Name, model.Email, model.BirthDate);

    await userService.AddAsync(request, cancellationToken);

    return Results.Created();
})
.WithName("CriarUsuario");

app.Run();