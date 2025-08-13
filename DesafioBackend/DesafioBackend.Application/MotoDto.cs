using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioBackend.Application
{
    public class MotoCreateDTO
    {
        [Required]
        public int Identificador { get; set; }

        [Required]
        [Range(1900, 2100)]
        public int Ano { get; set; }

        [Required]
        [StringLength(100)]
        public string Modelo { get; set; }

        [Required]
        [StringLength(10)]
        public string Placa { get; set; }
    }

    public class MotoUpdateDTO
    {
        [Required]
        [StringLength(10)]
        public string Placa { get; set; }
    }
}
