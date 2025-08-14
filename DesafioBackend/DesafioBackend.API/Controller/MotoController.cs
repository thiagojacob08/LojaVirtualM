using AutoMapper;
using DesafioBackend.Application;
using DesafioBackend.Application.DTO;
using DesafioBackend.Domain;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace DesafioBackend.API.Controller;

[ApiController]
[Route("motos")]
public class MotoController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IMapper _mapper;

    public MotoController(AppDbContext dbContext, IMessagePublisher messagePublisher, IMapper mapper)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMoto([FromBody] MotoCreateDTO dto)
    {
        if (await _dbContext.Motos.AnyAsync(m => m.Placa == dto.Placa))
            return Conflict(new { message = "Placa já cadastrada." });

        var moto = _mapper.Map<Moto>(dto);
        _dbContext.Motos.Add(moto);
        await _dbContext.SaveChangesAsync();

        // Publicar evento apenas se ano = 2024
        if (moto.Ano == 2024)
        {
            var evento = new { MotoId = moto.Identificador, moto.Ano, moto.Modelo, moto.Placa };
            await _messagePublisher.PublishAsync("moto.cadastrada", evento);
        }

        var readDto = _mapper.Map<MotoReadDTO>(moto);
        return CreatedAtAction(nameof(GetMotoById), new { id = moto.Identificador }, readDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetMotos([FromQuery] string? placa)
    {
        var query = _dbContext.Motos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(placa))
            query = query.Where(m => m.Placa == placa);

        var motos = await query.ToListAsync();
        var readDtos = _mapper.Map<List<MotoReadDTO>>(motos);
        return Ok(readDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMotoById(int id)
    {
        var moto = await _dbContext.Motos.FindAsync(id);
        if (moto == null)
            return NotFound();

        var readDto = _mapper.Map<MotoReadDTO>(moto);
        return Ok(readDto);
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

        var readDto = _mapper.Map<MotoReadDTO>(moto);
        return Ok(readDto);
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
