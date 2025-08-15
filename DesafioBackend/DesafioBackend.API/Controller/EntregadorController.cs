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
        try
        {
            if (await _dbContext.Entregadores.AnyAsync(e => e.CNPJ == dto.CNPJ || e.NumeroCNH == dto.NumeroCNH))
                return Conflict(new { message = "CNPJ ou CNH já cadastrado." });

            var entregador = _mapper.Map<Entregador>(dto);
            entregador.DataNascimento = DateTime.SpecifyKind(dto.DataNascimento, DateTimeKind.Utc);
            _dbContext.Entregadores.Add(entregador);
            await _dbContext.SaveChangesAsync();

            var readDto = _mapper.Map<EntregadorReadDTO>(entregador);
            return CreatedAtAction(nameof(GetEntregadorById), new { id = entregador.Identificador }, readDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao cadastrar um entregador", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEntregadorById(int id)
    {
        try
        {
            var entregador = await _dbContext.Entregadores.FindAsync(id);
            if (entregador == null) return NotFound();

            var readDto = _mapper.Map<EntregadorReadDTO>(entregador);
            return Ok(readDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Erro ao buscar o entregador: {id}.", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetEntregadores([FromQuery] string? nome)
    {
        try
        {
            var query = _dbContext.Entregadores.AsQueryable();
            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(e => e.Nome.Contains(nome));

            var readDtos = _mapper.Map<List<EntregadorReadDTO>>(await query.ToListAsync());
            return Ok(readDtos);
        }
        catch (Exception ex)
        {

            return StatusCode(500, new { message = $"Erro ao buscar os entregadores.", details = ex.Message });
        }
    }

    [HttpPost("foto-cnh")]
    public IActionResult UpdateFotoCNH([FromBody] FotoCNHBase64Dto dto)
    {
        if (string.IsNullOrEmpty(dto.ArquivoBase64))
            return BadRequest("Arquivo não informado");

        try
        {
            var bytes = Convert.FromBase64String(dto.ArquivoBase64);
                        
            var caminho = Path.Combine("Uploads", dto.NomeArquivo);
            Directory.CreateDirectory(Path.GetDirectoryName(caminho));
            System.IO.File.WriteAllBytes(caminho, bytes);

            return Ok($"Arquivo '{dto.NomeArquivo}' enviado com sucesso");
        }
        catch (FormatException)
        {
            return BadRequest("O arquivo enviado não está em formato Base64 válido");
        }
    }

}
