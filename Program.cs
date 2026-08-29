using Microsoft.EntityFrameworkCore;
using EcommerceApi.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("ConnectionString");

// Se não encontrar a variável, tenta no appsettings.json

if (string.IsNullOrEmpty(connectionString))
{
    // Fallback: tenta ler do appsettings.json (apenas se não houver variável)
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"⚠️ Usando connection string do appsettings.json: {(connectionString?.Contains("localhost") == true ? "LOCALHOST" : "ALTERNATIVA")}");
}
else
{
    Console.WriteLine("✅ Connection String obtida da variável de ambiente!");
}

// Configurar o DbContext com a connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
else
{
    Console.WriteLine("❌ Connection String não encontrada!");
}

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.MapControllers();
app.MapGet("/", () => "API de E-commerce - Use /swagger");

app.Run();