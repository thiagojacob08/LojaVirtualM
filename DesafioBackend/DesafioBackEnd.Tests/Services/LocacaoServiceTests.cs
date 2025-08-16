using AutoMapper;
using DesafioBackend.Application.DTO;
using DesafioBackend.Application.Services;
using DesafioBackend.Domain.DTO;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Tests.Services;

public class LocacaoServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public LocacaoServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Locacao")
            .Options;
        _dbContext = new AppDbContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<LocacaoCreateDTO, Locacao>();
            cfg.CreateMap<Locacao, LocacaoReadDTO>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task CriarLocacao_Deve_Criar_Locacao_Corretamente()
    {
        var entregador = new Entregador { Identificador = 1, Nome = "Teste", TipoCNH = "A" };
        var moto = new Moto { Identificador = 1, Modelo = "ModeloX", Ano = 2024, Placa = "ABC1234" };
        await _dbContext.Entregadores.AddAsync(entregador);
        await _dbContext.Motos.AddAsync(moto);
        await _dbContext.SaveChangesAsync();

        var service = new LocacaoService(_dbContext, _mapper);

        var dto = new LocacaoCreateDTO { EntregadorId = 1, MotoId = 1, PlanoDias = 7 };
        var result = await service.CriarLocacaoAsync(dto);

        Assert.NotNull(result);
        Assert.True(result.Ativa);
        Assert.Equal(1, _dbContext.Locacoes.Count());
    }

    [Fact]
    public async Task FinalizarLocacao_Deve_Atualizar_ValorTotal()
    {
        var locacao = new Locacao { Id = 1, Ativa = true, ValorDiaria = 30, ValorTotal = 210, DataInicio = DateTime.UtcNow.AddDays(-7), DataFimPrevisto = DateTime.UtcNow.AddDays(-1) };
        await _dbContext.Locacoes.AddAsync(locacao);
        await _dbContext.SaveChangesAsync();

        var service = new LocacaoService(_dbContext, _mapper);
        var result = await service.FinalizarLocacaoAsync(1);

        Assert.False(result.Ativa);
        Assert.True(result.ValorTotal > 210); // Verifica multa aplicada
    }
}
