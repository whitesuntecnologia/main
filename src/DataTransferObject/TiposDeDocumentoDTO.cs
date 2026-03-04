using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TiposDeDocumentoDTO
    {
        public int IdTipoDocumento { get; set; }
        public string Descripcion { get; set; } = null!;
        public int? TamanioMaximoMb { get; set; }
        public string Extension { get; set; } = null!;
        public string AcronimoGde { get; set; }
        public bool Obligatorio { get; set; }
        public bool SePermiteVariasVeces { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; } = null!;

    }
}
