# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SJInovacao.Acesso.WebAPI/SJInovacao.Acesso.WebAPI.csproj", "src/SJInovacao.Acesso.WebAPI/"]
COPY ["src/SJInovacao.Acesso.Common/SJInovacao.Acesso.Common.csproj", "src/SJInovacao.Acesso.Common/"]
COPY ["src/SJInovacao.Acesso.IoC/SJInovacao.Acesso.IoC.csproj", "src/SJInovacao.Acesso.IoC/"]
COPY ["src/SJInovacao.Acesso.Modules.UserAccess.Application/SJInovacao.Acesso.Modules.UserAccess.Application.csproj", "src/SJInovacao.Acesso.Modules.UserAccess.Application/"]
COPY ["src/SJInovacao.Acesso.Modules.UserAccess.Domain/SJInovacao.Acesso.Modules.UserAccess.Domain.csproj", "src/SJInovacao.Acesso.Modules.UserAccess.Domain/"]
COPY ["src/SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM/SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.csproj", "src/SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM/"]
RUN dotnet restore "./src/SJInovacao.Acesso.WebAPI/SJInovacao.Acesso.WebAPI.csproj"
COPY . .
WORKDIR "/src/SJInovacao.Acesso.WebAPI"
RUN dotnet build "./SJInovacao.Acesso.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SJInovacao.Acesso.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SJInovacao.Acesso.WebAPI.dll"]