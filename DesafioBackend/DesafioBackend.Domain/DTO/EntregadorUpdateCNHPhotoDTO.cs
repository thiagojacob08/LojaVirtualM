using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Application.DTO;

public class EntregadorUpdateCNHPhotoDTO
{
    [Required]
    public byte[] ImagemCNH { get; set; } // O armazenamento será no disco, mas para API recebemos assim.
}
