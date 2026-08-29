# Estágio 1: Build (compila a aplicação)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia os arquivos do projeto e restaura as dependências
COPY . .
RUN dotnet restore

# Publica a aplicação em modo Release
RUN dotnet publish -c Release -o /app/publish

# Estágio 2: Runtime (executa a aplicação)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copia os arquivos publicados do estágio de build
COPY --from=build /app/publish .

# Porta padrão que o Railway espera
EXPOSE 8080

# Variável de ambiente para o Railway gerenciar a porta [citation:3][citation:4]
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EcommerceApi.dll"]