using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class NotificacionDTO
    {
        public int IdNotificacion { get; set; }

        public int? IdTramite { get; set; }

        public string Titulo { get; set; } = null!;

        public string Mensaje { get; set; } = null!;

        public bool Leido { get; set; }

        public DateTime CreateDate { get; set; }
        
    }
}
