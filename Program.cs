using Microsoft.EntityFrameworkCore;
using EcommerceApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔥 LER A VARIÁVEL DO RAILWAY
var connectionString = Environment.GetEnvironmentVariable("DATABASE_PRIVATE_URL");

if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgresql://"))
{
    Console.WriteLine("✅ Convertendo URL para formato Npgsql...");
    connectionString = ConvertPostgresUrlToConnectionString(connectionString);
}
else if (!string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("⚠️ A variável não parece ser uma URL PostgreSQL.");
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("⚠️ Usando connection string do appsettings.json (desenvolvimento local)");
}

if (!string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine($"✅ Connection String obtida! Tamanho: {connectionString.Length} caracteres");
}

// Configurar o DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicar migrações
if (!string.IsNullOrEmpty(connectionString) && !connectionString.Contains("localhost"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            dbContext.Database.Migrate();
            Console.WriteLine("✅ Migrações aplicadas com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao aplicar migrações: {ex.Message}");
        }
    }
}

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.MapControllers();
app.MapGet("/", () => "API de E-commerce - Use /swagger");

app.Run();

// 🔥 FUNÇÃO PARA CONVERTER URL POSTGRESQL PARA FORMATO NPGSQL
static string ConvertPostgresUrlToConnectionString(string url)
{
    try
    {
        var uri = new Uri(url);
        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao converter URL: {ex.Message}");
        return url; // Retorna a URL original como fallback
    }
}