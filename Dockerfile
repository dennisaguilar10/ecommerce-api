# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia os arquivos do projeto
COPY . .
RUN dotnet restore

# Publica a aplicação
RUN dotnet publish -c Release -o /app/publish

# Estágio 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copia os arquivos publicados
COPY --from=build /app/publish .

# Expõe a porta 8080
EXPOSE 8080

# Define a URL base
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EcommerceApi.dll"]