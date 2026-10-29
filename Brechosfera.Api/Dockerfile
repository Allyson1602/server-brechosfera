# ---- Etapa 1: compilar ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Copia só os .csproj, mantendo a estrutura de pastas
COPY Brechosfera.Api/Brechosfera.Api.csproj							Brechosfera.Api/
COPY Brechosfera.Application/Brechosfera.Application.csproj			Brechosfera.Application/
COPY Brechosfera.Domain/Brechosfera.Domain.csproj					Brechosfera.Domain/
COPY Brechosfera.Infrastructure/Brechosfera.Infrastructure.csproj	Brechosfera.Infrastructure/

# 2) Restaura os pacotes (essa camada fica em cache)
RUN dotnet restore Brechosfera.Api/Brechosfera.Api.csproj

# 3) Copia o código todo e publica
COPY . .
RUN dotnet publish Brechosfera.Api/Brechosfera.Api.csproj -c Release -o /app --no-restore

# ---- Etapa 2: rodar ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Brechosfera.Api.dll"]
