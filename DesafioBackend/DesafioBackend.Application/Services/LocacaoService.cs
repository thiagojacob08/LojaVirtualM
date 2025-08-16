using AutoMapper;
using DesafioBackend.Application.DTO;
using DesafioBackend.Application.Interface;
using DesafioBackend.Domain.DTO;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Domain.Enum;
using DesafioBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Application.Services;

public class LocacaoService : ILocacaoService
{
    #region Propriedades
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper; 
    #endregion

    #region Construtor
    public LocacaoService(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    } 
    #endregion

    #region metodos Assincronos
    public async Task<LocacaoReadDTO> CriarLocacaoAsync(LocacaoCreateDTO locacaodto)
    {
        var entregador = await _dbContext.Entregadores.FindAsync(locacaodto.EntregadorId);
        if (entregador == null)
            throw new Exception("Entregador não encontrado.");

        if (!entregador.TipoCNH.Contains("A"))
            throw new Exception("Entregador não habilitado para motos.");

        var moto = await _dbContext.Motos.FindAsync(locacaodto.MotoId);
        if (moto == null)
            throw new Exception("Moto não encontrada.");

        bool motoJaLocada = await _dbContext.Locacoes.AnyAsync(l => l.MotoId == locacaodto.MotoId && l.Ativa);
        if (motoJaLocada)
            throw new Exception("Moto já está em locação ativa.");

        Locacao locacao = CriaLocacao(locacaodto);

        _dbContext.Locacoes.Add(locacao);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<LocacaoReadDTO>(locacao);
    }

    public async Task<LocacaoReadDTO> FinalizarLocacaoAsync(int id)
    {
        var locacao = await _dbContext.Locacoes.FindAsync(id);
        if (locacao == null)
            throw new KeyNotFoundException("Locação não encontrada.");

        if (!locacao.Ativa)
            throw new InvalidOperationException("Locação já finalizada.");

        locacao.DataFimReal = DateTime.Today;
        locacao.Ativa = false;

        VerificaMulta(locacao);

        if (locacao.Multa > 0)
            locacao.ValorTotal += locacao.Multa;

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<LocacaoReadDTO>(locacao);
    }

    public async Task<LocacaoReadDTO> GetLocacaoById(int idLocacao)
    {
        var locacao = await _dbContext.Locacoes.FindAsync(idLocacao);
        if (locacao == null)
            throw new Exception("Locação não encontrada.");

        var locacaoDto = _mapper.Map<LocacaoReadDTO>(locacao);
        return locacaoDto;
    } 
    #endregion

    #region Metodos Sincronos

    private Locacao CriaLocacao(LocacaoCreateDTO locacaoDto)
    {
        var locacao = _mapper.Map<Locacao>(locacaoDto);
        locacao.DataInicio = DateTime.SpecifyKind(DateTime.Today.AddDays(1), DateTimeKind.Utc);
        locacao.DataFimPrevisto = DateTime.SpecifyKind(locacao.DataInicio.AddDays(locacaoDto.PlanoDias), DateTimeKind.Utc);
        locacao.Ativa = true;
        locacao.ValorDiaria = CalcularValorDiaria(locacaoDto.PlanoDias);
        locacao.ValorTotal = locacao.ValorDiaria * locacaoDto.PlanoDias;
        return locacao;
    }

    public decimal CalcularValorDiaria(int planoDias)
    {
        if (Enum.IsDefined(typeof(PlanoLocacao), planoDias))
        {
            var plano = (PlanoLocacao)planoDias;
            return ValoresPlano[plano];
        }

        throw new ArgumentException("Plano de dias inválido");
    }

    private static readonly Dictionary<PlanoLocacao, decimal> ValoresPlano = new()
    {
        { PlanoLocacao.SeteDias, 30 },
        { PlanoLocacao.QuinzeDias, 28 },
        { PlanoLocacao.TrintaDias, 22 },
        { PlanoLocacao.QuarentaCincoDias, 20 },
        { PlanoLocacao.CinquentaDias, 18 }
    };

    private static void VerificaMulta(Locacao locacao)
    {
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
    } 

    #endregion
}
