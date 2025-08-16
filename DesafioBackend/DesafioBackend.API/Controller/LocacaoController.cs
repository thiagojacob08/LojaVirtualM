using DesafioBackend.Application.DTO;
using DesafioBackend.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DesafioBackend.API.Controller;

[ApiController]
[Route("locacoes")]
public class LocacaoController : ControllerBase
{
    private readonly ILocacaoService _locacaoService;

    public LocacaoController(ILocacaoService locacaoService)
    {
        _locacaoService = locacaoService;
    }

    [HttpPost]
    public async Task<IActionResult> CriarLocacao([FromBody] LocacaoCreateDTO dto)
    {
        try
        {
            var locacaoDto = await _locacaoService.CriarLocacaoAsync(dto);
            return CreatedAtAction(nameof(GetLocacaoById), new { id = locacaoDto.Id }, locacaoDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao cadastrar uma locação.", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocacaoById(int id)
    {
        try
        {
            var locacao = await _locacaoService.GetLocacaoById(id);
            return Ok(locacao);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Erro ao buscar a locação de id {id}.", details = ex.Message });
        }
    }

    [HttpPatch("{id}/finalizar")]
    public async Task<IActionResult> FinalizarLocacao(int id)
    {
        try
        {
            var locacaoReadDto = await _locacaoService.FinalizarLocacaoAsync(id);
            return Ok(locacaoReadDto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Erro ao finalizar a locação de id {id}.", details = ex.Message });
        }
    }

}
