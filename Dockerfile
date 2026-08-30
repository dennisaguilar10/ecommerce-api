# ============================================
# ESTÁGIO 1: BUILD
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar apenas o arquivo de projeto e restaurar dependências (cache)
COPY *.csproj .
RUN dotnet restore

# Copiar o resto do código e publicar
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ============================================
# ESTÁGIO 2: RUNTIME
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar os arquivos publicados
COPY --from=build /app/publish .

# Configurar porta e ambiente
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Porta exposta
EXPOSE 8080

# Iniciar a aplicação
ENTRYPOINT ["dotnet", "EcommerceApi.dll"]