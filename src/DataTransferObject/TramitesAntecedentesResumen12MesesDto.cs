using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public partial class TramitesAntecedentesDdjjFilaDto
    {
        public int Mes { get; set; }
        public int Anio{ get; set; }
        public List<TramitesAntecedentesDdjjColumnaDto> Antecedentes { get; set; } = new();
        public decimal MontoFila{ get; set; }
    }
    public partial class TramitesAntecedentesDdjjColumnaDto
    {
        public int IdAntecedente { get; set; }
        public string NombreObra { get; set; }
        public decimal MontoMensual { get; set; }
        public bool PermiteEditarMonto{ get; set; }
    }
    public partial class TramitesAntecedentesResumen12MesesDto
    {
        public int MesInicio { get; set; }
        public int AnioInicio { get; set; }
        public int MesFin { get; set; }
        public int AnioFin { get; set; }
        public decimal TotalEjecutado { get; set; }
        public decimal TotalMejores12Meses { get; set; }
    }
    public partial class TramitesAntecedentesTotalMensualDto
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Total { get; set; }
    }
    public partial class TramitesAntecedentesTotales12mesesDto
    {
        
        public int IdAntecedente { get; set; }
        public string NombreObra { get; set; }
        public decimal TotalObra { get; set; }
        public decimal Total { get; set; }
        public decimal IndiceActivo { get; set; }
        public decimal MontoActivo { get; set; }
    }
}
