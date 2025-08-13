using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioBackend.Application
{
    public class EntregadorCreateDTO
    {
        [Required]
        public int Identificador { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        [Required]
        [RegularExpression(@"\d{14}", ErrorMessage = "CNPJ deve conter 14 dígitos numéricos.")]
        public string CNPJ { get; set; }

        [Required]
        public DateTime DataNascimento { get; set; }

        [Required]
        [StringLength(20)]
        public string NumeroCNH { get; set; }

        [Required]
        [RegularExpression("^(A|B|A\\+B)$", ErrorMessage = "TipoCNH deve ser A, B ou A+B")]
        public string TipoCNH { get; set; }
    }

    public class EntregadorUpdateCNHPhotoDTO
    {
        [Required]
        public byte[] ImagemCNH { get; set; } // O armazenamento será no disco, mas para API recebemos assim.
    }
}
