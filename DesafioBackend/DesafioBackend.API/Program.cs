using DesafioBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Logs detalhados
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Conexão com Postgres (tratamento de exceção)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    try
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Falha ao configurar DbContext: {ex.Message}");
    }
});

// RabbitMQ (tratamento de exceção)
builder.Services.AddSingleton<IMessagePublisher>(sp =>
{
    try
    {
        var publisher = new RabbitMqMessagePublisher();
        publisher.InitializeAsync("localhost").GetAwaiter().GetResult();
        return publisher;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Falha ao inicializar RabbitMQ: {ex.Message}");
        return null; // Swagger e API ainda vão subir
    }
});

// Controllers
builder.Services.AddControllers();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<FileUploadOperation>();
});

var app = builder.Build();

// Middleware para mostrar exceções em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao iniciar a aplicação: {ex.Message}");
    throw;
}
