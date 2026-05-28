FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["User.Api/User.Api.csproj", "User.Api/"]
COPY ["User.Domain/User.Domain.csproj", "User.Domain/"]
COPY ["User.Application/User.Application.csproj", "User.Application/"]
COPY ["User.Infrastructure/User.Infrastructure.csproj", "User.Infrastructure/"]
RUN dotnet restore "User.Api/User.Api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "User.Api/User.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "User.Api/User.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.Api.dll"]