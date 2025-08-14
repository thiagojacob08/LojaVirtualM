using AutoMapper;
using DesafioBackend.Application.DTO;
using DesafioBackend.Domain.DTO;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.API.Controller;

[ApiController]
[Route("locacoes")]
public class LocacaoController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public LocacaoController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CriarLocacao([FromBody] LocacaoCreateDTO dto)
    {
        var entregador = await _dbContext.Entregadores.FindAsync(dto.EntregadorId);
        if (entregador == null) return NotFound("Entregador não encontrado.");
        if (!entregador.TipoCNH.Contains("A"))
            return BadRequest("Entregador não habilitado para motos.");

        var moto = await _dbContext.Motos.FindAsync(dto.MotoId);
        if (moto == null) return NotFound("Moto não encontrada.");

        bool motoJaLocada = await _dbContext.Locacoes.AnyAsync(l => l.MotoId == dto.MotoId && l.Ativa);
        if (motoJaLocada)
            return Conflict("Moto já está em locação ativa.");

        var locacao = _mapper.Map<Locacao>(dto);
        locacao.DataInicio = DateTime.Today.AddDays(1);
        locacao.DataFimPrevisto = locacao.DataInicio.AddDays(dto.PlanoDias);
        locacao.Ativa = true;
        locacao.ValorDiaria = CalcularValorDiaria(dto.PlanoDias);
        locacao.ValorTotal = locacao.ValorDiaria * dto.PlanoDias;

        _dbContext.Locacoes.Add(locacao);
        await _dbContext.SaveChangesAsync();

        var readDto = _mapper.Map<LocacaoReadDTO>(locacao);
        return CreatedAtAction(nameof(GetLocacaoById), new { id = locacao.Id }, readDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocacaoById(int id)
    {
        var locacao = await _dbContext.Locacoes.FindAsync(id);
        if (locacao == null) return NotFound();

        var readDto = _mapper.Map<LocacaoReadDTO>(locacao);
        return Ok(readDto);
    }

    [HttpPatch("{id}/finalizar")]
    public async Task<IActionResult> FinalizarLocacao(int id)
    {
        var locacao = await _dbContext.Locacoes.FindAsync(id);
        if (locacao == null) return NotFound();
        if (!locacao.Ativa) return BadRequest("Locação já finalizada.");

        locacao.DataFimReal = DateTime.Today;
        locacao.Ativa = false;

        if (locacao.DataFimReal > locacao.DataFimPrevisto)
        {
            int diasAtraso = (locacao.DataFimReal.Value - locacao.DataFimPrevisto).Days;
            locacao.Multa = diasAtraso * locacao.ValorDiaria * 0.2m; // multa de 20% por dia
        }
        else if (locacao.DataFimReal < locacao.DataFimPrevisto)
        {
            int diasAntecipados = (locacao.DataFimPrevisto - locacao.DataFimReal.Value).Days;
            locacao.Multa = diasAntecipados * locacao.ValorDiaria * 0.4m; // multa de 40% por dia
        }

        if (locacao.Multa > 0 )
            locacao.ValorTotal += locacao.Multa;

        await _dbContext.SaveChangesAsync();

        var readDto = _mapper.Map<LocacaoReadDTO>(locacao);
        return Ok(readDto);
    }

    private decimal CalcularValorDiaria(int planoDias) =>
        planoDias switch
        {
            7 => 30m,
            15 => 28m,
            30 => 22m,
            45 => 20m,
            50 => 18m,
            _ => throw new ArgumentException("Plano inválido.")
        };
}
