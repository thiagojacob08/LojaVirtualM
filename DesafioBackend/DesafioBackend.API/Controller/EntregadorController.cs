using DesafioBackend.Application.DTO;
using DesafioBackend.Application.Interface;
using DesafioBackend.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

[Route("entregadores")]
public class EntregadorController : ControllerBase
{
    private readonly IEntregadorService _entregadorService;

    public EntregadorController(IEntregadorService entregadorService)
    {
        _entregadorService = entregadorService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEntregador([FromBody] EntregadorCreateDTO dto)
    {
        try
        {
            var readDto = await _entregadorService.CriarEntregadorAsync(dto);
            return CreatedAtAction(nameof(GetEntregadorById), new { id = readDto.Identificador }, readDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEntregadorById(int id)
    {
        try
        {
            var readDto = await _entregadorService.GetEntregadorByIdAsync(id);
            return Ok(readDto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetEntregadores([FromQuery] string? nome)
    {
        try
        {
            var readDtos = await _entregadorService.GetEntregadoresAsync(nome);
            return Ok(readDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("foto-cnh")]
    public async Task<IActionResult> UpdateFotoCNH([FromBody] FotoCNHBase64Dto dto)
    {
        try
        {
            await _entregadorService.SalvarFotoCNHAsync(dto);
            return Ok($"Arquivo '{dto.NomeArquivo}' enviado com sucesso");
        }
        catch (FormatException)
        {
            return BadRequest("O arquivo enviado não está em formato Base64 válido");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}