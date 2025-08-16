using AutoMapper;
using DesafioBackend.Application.DTO;
using DesafioBackend.Application.Interface;
using DesafioBackend.Domain.DTO;
using DesafioBackend.Domain.Entities;
using DesafioBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Application.Services;

public class EntregadorService : IEntregadorService
{
    #region Propriedades
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    #endregion

    #region Construtor
    public EntregadorService(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    #endregion

    #region Métodos Assincronos
    public async Task<EntregadorReadDTO> CriarEntregadorAsync(EntregadorCreateDTO dto)
    {
        if (await _dbContext.Entregadores.AnyAsync(e => e.CNPJ == dto.CNPJ || e.NumeroCNH == dto.NumeroCNH))
            throw new InvalidOperationException("CNPJ ou CNH já cadastrado.");

        var entregador = _mapper.Map<Entregador>(dto);
        entregador.DataNascimento = DateTime.SpecifyKind(dto.DataNascimento, DateTimeKind.Utc);

        _dbContext.Entregadores.Add(entregador);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<EntregadorReadDTO>(entregador);
    }

    public async Task<EntregadorReadDTO> GetEntregadorByIdAsync(int id)
    {
        var entregador = await _dbContext.Entregadores.FindAsync(id);
        if (entregador == null)
            throw new KeyNotFoundException("Entregador não encontrado.");

        return _mapper.Map<EntregadorReadDTO>(entregador);
    }

    public async Task<List<EntregadorReadDTO>> GetEntregadoresAsync(string? nome)
    {
        var query = _dbContext.Entregadores.AsQueryable();
        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(e => e.Nome.Contains(nome));

        var entregadores = await query.ToListAsync();

        return _mapper.Map<List<EntregadorReadDTO>>(entregadores);
    }

    public async Task SalvarFotoCNHAsync(FotoCNHBase64Dto dto)
    {
        if (string.IsNullOrEmpty(dto.ArquivoBase64))
            throw new ArgumentException("Arquivo não informado");

        var bytes = Convert.FromBase64String(dto.ArquivoBase64);
        var caminho = Path.Combine("Uploads", dto.NomeArquivo);

        Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);
        await File.WriteAllBytesAsync(caminho, bytes);
    } 
    #endregion
}
