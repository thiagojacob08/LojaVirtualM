
namespace DesafioBackend.Domain.ValueObjects;

public class FotoCnh
{
    public string CaminhoArquivo { get; set; }
    public string NomeArquivo { get; set; }

    public FotoCnh(string caminhoArquivo, string nomeArquivo)
    {
        CaminhoArquivo = caminhoArquivo;
        NomeArquivo = nomeArquivo;
    }
}
