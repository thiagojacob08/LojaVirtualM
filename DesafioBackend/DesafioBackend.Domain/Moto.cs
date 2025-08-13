using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioBackend.Domain
{
    public class Moto
    {
        [Key]
        public int Identificador { get; set; }  // Id único
        public int Ano { get; set; }
        public string Modelo { get; set; } = null!;
        public string Placa { get; set; } = null!; // Deve ser único
    }
}
