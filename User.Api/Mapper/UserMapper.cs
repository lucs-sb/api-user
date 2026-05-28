using Mapster;
using User.Api.Model;
using User.Application.DTOs.Request;

namespace User.Api.Mapper;

public static class UserMapper
{
    public static void Configure()
    {
        TypeAdapterConfig<CreateUserRequestDTO, Domain.Entities.User>
            .NewConfig()
            .Map(dest => dest.CreatedAt, _ => DateTime.UtcNow)
            .Map(dest => dest.UpdatedAt, _ => DateTime.UtcNow);
    }
}
