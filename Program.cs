using Microsoft.EntityFrameworkCore;
using EcommerceApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔥 LER A VARIÁVEL DO RAILWAY
var connectionString = Environment.GetEnvironmentVariable("ConnectionString");

if (string.IsNullOrEmpty(connectionString))
{
    // Fallback para desenvolvimento local
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("⚠️ Usando connection string do appsettings.json (desenvolvimento local)");
}
else
{
    Console.WriteLine("✅ Connection String obtida da variável de ambiente do Railway!");
}

// Configurar o DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicar migrações automaticamente
if (!string.IsNullOrEmpty(connectionString))
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