using AutoMapper;
using DesafioBackend.Application.DTO;
using DesafioBackend.Domain.DTO;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.API.Controller;

[ApiController]
[Route("entregadores")]
public class EntregadorController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public EntregadorController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEntregador([FromBody] EntregadorCreateDTO dto)
    {
        if (await _dbContext.Entregadores.AnyAsync(e => e.CNPJ == dto.CNPJ || e.NumeroCNH == dto.NumeroCNH))
            return Conflict(new { message = "CNPJ ou CNH já cadastrado." });

        var entregador = _mapper.Map<Entregador>(dto);
        _dbContext.Entregadores.Add(entregador);
        await _dbContext.SaveChangesAsync();

        var readDto = _mapper.Map<EntregadorReadDTO>(entregador);
        return CreatedAtAction(nameof(GetEntregadorById), new { id = entregador.Identificador }, readDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEntregadorById(int id)
    {
        var entregador = await _dbContext.Entregadores.FindAsync(id);
        if (entregador == null) return NotFound();

        var readDto = _mapper.Map<EntregadorReadDTO>(entregador);
        return Ok(readDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetEntregadores([FromQuery] string? nome)
    {
        var query = _dbContext.Entregadores.AsQueryable();
        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(e => e.Nome.Contains(nome));

        var readDtos = _mapper.Map<List<EntregadorReadDTO>>(await query.ToListAsync());
        return Ok(readDtos);
    }

    [HttpPatch("{id}/foto")]
    public async Task<IActionResult> UpdateFotoCNH(int id, [FromForm] IFormFile foto)
    {
        var entregador = await _dbContext.Entregadores.FindAsync(id);
        if (entregador == null) return NotFound();

        // Validar extensão
        var ext = Path.GetExtension(foto.FileName).ToLower();
        if (ext != ".png" && ext != ".bmp")
            return BadRequest("Formato inválido. Aceito: png, bmp");

        // Salvar arquivo localmente
        var path = Path.Combine("Uploads", $"cnh_{id}{ext}");
        Directory.CreateDirectory("Uploads");
        using var stream = System.IO.File.Create(path);
        await foto.CopyToAsync(stream);

        entregador.ImagemCNHPath = path;
        await _dbContext.SaveChangesAsync();

        var readDto = _mapper.Map<EntregadorReadDTO>(entregador);
        return Ok(readDto);
    }
}
