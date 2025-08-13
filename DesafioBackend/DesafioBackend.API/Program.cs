using DesafioBackend.Domain;
using DesafioBackend.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RabbitMQ.Client;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Conexão com Postgres
builder.Services.AddTransient(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddSingleton<IMessagePublisher>(sp =>
    new RabbitMqMessagePublisher("localhost"));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();


// 1️⃣ Teste com Banco de Dados
app.MapGet("/db-test", async (NpgsqlConnection conn) =>
{
    try
    {
        await conn.OpenAsync();
        using var cmd = new NpgsqlCommand("SELECT version()", conn);
        var version = await cmd.ExecuteScalarAsync();
        return Results.Ok(new { DatabaseVersion = version });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});


// 2️⃣ Enviar mensagem para RabbitMQ
app.MapGet("/rabbit-send", async () =>
{
    try
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "teste",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        string message = $"Mensagem de teste {DateTime.Now}";
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: "",
                                        routingKey: "teste",
                                        mandatory: false,
                                        body: body);

        return Results.Ok($"Enviado: {message}");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});


// 3️⃣ Ler mensagem do RabbitMQ
app.MapGet("/rabbit-receive", async () =>
{
    try
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        var result = await channel.BasicGetAsync(queue: "teste", autoAck: true);
        if (result == null)
            return Results.NotFound("Nenhuma mensagem na fila");

        var message = Encoding.UTF8.GetString(result.Body.ToArray());
        return Results.Ok($"Recebido: {message}");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/motos", async (AppDbContext db) =>
{
    var motos = await db.Motos.ToListAsync();
    return Results.Ok(motos);
});

app.MapPost("/motos", async (AppDbContext db, [FromBody] Moto moto) =>
{
    // Verifica se placa já existe
    if (await db.Motos.AnyAsync(m => m.Placa == moto.Placa))
        return Results.Conflict($"Moto com placa {moto.Placa} já cadastrada.");

    db.Motos.Add(moto);
    await db.SaveChangesAsync();

    // Aqui poderia publicar evento de moto cadastrada (mensageria)
    return Results.Created($"/motos/{moto.Identificador}", moto);
});

app.MapGet("/motos", async (AppDbContext db, string? placa) =>
{
    var query = db.Motos.AsQueryable();

    if (!string.IsNullOrEmpty(placa))
        query = query.Where(m => m.Placa.Contains(placa));

    var motos = await query.ToListAsync();
    return Results.Ok(motos);
});

app.MapPut("/motos/{id:int}", async (AppDbContext db, int id, Moto motoAtualizada) =>
{
    var moto = await db.Motos.FindAsync(id);
    if (moto == null)
        return Results.NotFound();

    // Só altera placa
    if (moto.Placa != motoAtualizada.Placa)
    {
        // Verifica se nova placa já existe
        if (await db.Motos.AnyAsync(m => m.Placa == motoAtualizada.Placa && m.Identificador != id))
            return Results.Conflict($"Moto com placa {motoAtualizada.Placa} já cadastrada.");

        moto.Placa = motoAtualizada.Placa;
        await db.SaveChangesAsync();
    }

    return Results.NoContent();
});

app.MapDelete("/motos/{id:int}", async (AppDbContext db, int id) =>
{
    var moto = await db.Motos.FindAsync(id);
    if (moto == null)
        return Results.NotFound();

    // Verifica se tem locações associadas
    var temLocacoes = await db.Locacoes.AnyAsync(l => l.MotoId == id);
    if (temLocacoes)
        return Results.Conflict("Não é possível remover moto com locações associadas.");

    db.Motos.Remove(moto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPost("/motos", async (AppDbContext db, ConnectionFactory factory, Moto moto) =>
{
    // Verifica se placa já existe
    if (await db.Motos.AnyAsync(m => m.Placa == moto.Placa))
        return Results.Conflict($"Moto com placa {moto.Placa} já cadastrada.");

    db.Motos.Add(moto);
    await db.SaveChangesAsync();

    // Publicar evento "moto cadastrada" no RabbitMQ
    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();

    var evento = new
    {
        moto.Identificador,
        moto.Ano,
        moto.Modelo,
        moto.Placa
    };

    var jsonEvento = System.Text.Json.JsonSerializer.Serialize(evento);
    var body = Encoding.UTF8.GetBytes(jsonEvento);

    await channel.QueueDeclareAsync(queue: "moto_cadastrada",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

    await channel.BasicPublishAsync(exchange: "",
                                    routingKey: "moto_cadastrada",
                                    mandatory: false,
                                    body: body);

    return Results.Created($"/motos/{moto.Identificador}", moto);
});


app.Run();
