using Microsoft.EntityFrameworkCore;
using EcommerceApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔥 USAR O NOME EXATO DA VARIÁVEL DO RAILWAY
var connectionString = Environment.GetEnvironmentVariable("DATABASE_PRIVATE_URL");

// Se não encontrar, tenta o nome alternativo
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = Environment.GetEnvironmentVariable("Postgres_DATABASE_PRIVATE_URL");
}

// Se ainda não encontrar, tenta a variável que você criou
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = Environment.GetEnvironmentVariable("ConnectionString");
}

// Fallback para desenvolvimento local
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("⚠️ Usando connection string do appsettings.json (desenvolvimento local)");
}
else
{
    Console.WriteLine($"✅ Connection String obtida do Railway! Tamanho: {connectionString.Length} caracteres");
}

// Configurar o DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Lista todas as variáveis de ambiente (para debug)
Console.WriteLine("=== VARIÁVEIS DE AMBIENTE ===");
foreach (var env in Environment.GetEnvironmentVariables().Keys)
{
    if (env.ToString().Contains("DATABASE") || env.ToString().Contains("Connection") || env.ToString().Contains("POSTGRES"))
    {
        Console.WriteLine($"{env} = {Environment.GetEnvironmentVariable(env.ToString())}");
    }
}
Console.WriteLine("=============================");

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