using DesafioBackend.API.DTOs;
using DesafioBackend.Application;
using DesafioBackend.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.API;

[ApiController]
[Route("motos")]
public class MotoController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMessagePublisher _messagePublisher; // Para publicar evento (RabbitMQ)

    public MotoController(AppDbContext dbContext, IMessagePublisher messagePublisher)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMoto([FromBody] MotoCreateDTO dto)
    {
        if (await _dbContext.Motos.AnyAsync(m => m.Placa == dto.Placa))
            return Conflict(new { message = "Placa já cadastrada." });

        var moto = new Moto
        {
            Identificador = dto.Identificador,
            Ano = dto.Ano,
            Modelo = dto.Modelo,
            Placa = dto.Placa
        };

        _dbContext.Motos.Add(moto);
        await _dbContext.SaveChangesAsync();

        // Publicar evento de moto cadastrada
        var evento = new { MotoId = moto.Identificador, Ano = moto.Ano, Modelo = moto.Modelo, Placa = moto.Placa };
        await _messagePublisher.PublishAsync("moto.cadastrada", evento);

        return CreatedAtAction(nameof(GetMotoById), new { id = moto.Identificador }, moto);
    }

    [HttpGet]
    public async Task<IActionResult> GetMotos([FromQuery] string? placa)
    {
        var query = _dbContext.Motos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(placa))
            query = query.Where(m => m.Placa == placa);

        var motos = await query.ToListAsync();
        return Ok(motos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMotoById(int id)
    {
        var moto = await _dbContext.Motos.FindAsync(id);
        if (moto == null)
            return NotFound();

        return Ok(moto);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateMotoPlaca(int id, [FromBody] MotoUpdateDTO dto)
    {
        var moto = await _dbContext.Motos.FindAsync(id);
        if (moto == null)
            return NotFound();

        if (await _dbContext.Motos.AnyAsync(m => m.Placa == dto.Placa && m.Identificador != id))
            return Conflict(new { message = "Placa já cadastrada para outra moto." });

        moto.Placa = dto.Placa;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMoto(int id)
    {
        var moto = await _dbContext.Motos.FindAsync(id);
        if (moto == null)
            return NotFound();

        var hasLocacao = await _dbContext.Locacoes.AnyAsync(l => l.MotoId == id);
        if (hasLocacao)
            return BadRequest(new { message = "Moto possui locações e não pode ser removida." });

        _dbContext.Motos.Remove(moto);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}

