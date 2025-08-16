using AutoMapper;
using DesafioBackend.API.Controller;
using DesafioBackend.Application.DTO;
using DesafioBackend.Application.Services;
using DesafioBackend.Domain.DTO;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Tests.Controller;

public class LocacaoControllerTests
{
    private readonly LocacaoController _controller;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public LocacaoControllerTests()
    {
        // Configurar DbContext InMemory
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _dbContext = new AppDbContext(options);

        // Configurar AutoMapper (usar o mesmo profile que a aplicação)
        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });
        _mapper = mockMapper.CreateMapper();

        // Criar controller com service real ou mock
        var locacaoService = new LocacaoService(_dbContext, _mapper);
        _controller = new LocacaoController(locacaoService);
    }

    [Fact]
    public async Task CriarLocacao_DeveRetornarCreated()
    {
        // Arrange
        var entregador = new Entregador { Identificador = 1, Nome = "Teste", TipoCNH = "A" };
        var moto = new Moto { Identificador = 1, Ano = 2023, Modelo = "Yamaha", Placa = "ABC1234" };
        await _dbContext.Entregadores.AddAsync(entregador);
        await _dbContext.Motos.AddAsync(moto);
        await _dbContext.SaveChangesAsync();

        var dto = new LocacaoCreateDTO
        {
            EntregadorId = 1,
            MotoId = 1,
            PlanoDias = 7
        };

        // Act
        var result = await _controller.CriarLocacao(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var readDto = Assert.IsType<LocacaoReadDTO>(createdResult.Value);
        Assert.Equal(dto.MotoId, readDto.MotoId);
        Assert.Equal(dto.EntregadorId, readDto.EntregadorId);
    }
}
