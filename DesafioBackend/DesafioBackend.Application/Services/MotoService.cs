using AutoMapper;
using DesafioBackend.Application.DTO;
using DesafioBackend.Application.Interface;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Application.Services;

public class MotoService : IMotoService
{
    #region Propriedades
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IMessagePublisher _messagePublisher;
    #endregion

    #region Construtor
    public MotoService(AppDbContext dbContext, IMapper mapper, IMessagePublisher messagePublisher)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _messagePublisher = messagePublisher;
    }
    #endregion

    #region Métodos Assincronos
    public async Task<MotoReadDTO> CriarMotoAsync(MotoCreateDTO dto)
    {
        if (await _dbContext.Motos.AnyAsync(m => m.Placa == dto.Placa))
            throw new InvalidOperationException("Placa já cadastrada.");

        var moto = _mapper.Map<Moto>(dto);
        _dbContext.Motos.Add(moto);
        await _dbContext.SaveChangesAsync();
        await VerificaAnoMoto(moto);

        return _mapper.Map<MotoReadDTO>(moto);
    }

    public async Task<List<MotoReadDTO>> ListarMotosAsync(string? placa = null)
    {
        var query = _dbContext.Motos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(placa))
            query = query.Where(m => m.Placa == placa);

        var motos = await query.ToListAsync();
        return _mapper.Map<List<MotoReadDTO>>(motos);
    }

    public async Task<MotoReadDTO?> ObterMotoPorIdAsync(int id)
    {
        var moto = await _dbContext.Motos.FindAsync(id);
        if (moto == null)
            return null;

        return _mapper.Map<MotoReadDTO>(moto);
    }

    public async Task<MotoReadDTO> AtualizarPlacaAsync(int id, MotoUpdateDTO dto)
    {
        var moto = await _dbContext.Motos.FindAsync(id);
        if (moto == null)
            throw new KeyNotFoundException("Moto não encontrada.");

        if (await _dbContext.Motos.AnyAsync(m => m.Placa == dto.Placa && m.Identificador != id))
            throw new InvalidOperationException($"Placa {dto.Placa} já cadastrada para outra moto.");

        moto.Placa = dto.Placa;
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<MotoReadDTO>(moto);
    }

    public async Task DeletarMotoAsync(int id)
    {
        var moto = await _dbContext.Motos.FindAsync(id);
        if (moto == null)
            throw new KeyNotFoundException("Moto não encontrada.");

        var hasLocacao = await _dbContext.Locacoes.AnyAsync(l => l.MotoId == id);
        if (hasLocacao)
            throw new InvalidOperationException("Moto possui locação e não pode ser removida.");

        _dbContext.Motos.Remove(moto);
        await _dbContext.SaveChangesAsync();
    }

    private async Task VerificaAnoMoto(Moto moto)
    {
        if (moto.Ano == 2024)
        {
            var evento = new { MotoId = moto.Identificador, moto.Ano, moto.Modelo, moto.Placa };
            await _messagePublisher.PublishAsync("moto.cadastrada", evento);
        }
    } 
    #endregion
}
