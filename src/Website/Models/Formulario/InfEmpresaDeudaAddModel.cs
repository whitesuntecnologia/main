using DocumentFormat.OpenXml.Presentation;
using FluentValidation;
using NuGet.Protocol;

namespace Website.Models.Formulario
{
    public class InfEmpresaDeudaAddModel
    {
        public int? IdTramiteInfEmpDeuda { get; set; }
        public int? IdTramite { get; set; }
        public int? IdTramiteInfEmp { get; set; }

        public string? Entidad { get; set; } = null!;

        public int? Mes { get; set; } = null!;
        public int? Anio { get; set; } = null!;
        public string? Periodo {
            get
            {
                string strMes = (Mes.HasValue ? Mes.Value.ToString("00") : "");
                string strAnio = (Anio.HasValue ? Anio.Value.ToString() : "");
                return strMes + "/" + strAnio;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var partes = value.Split('/');
                    if (partes.Length == 2)
                    {
                        if (int.TryParse(partes[0], out int mes))
                            Mes = mes;
                        if (int.TryParse(partes[1], out int anio))
                            Anio = anio;
                    }
                }
                else
                {
                    Mes = null;
                    Anio = null;
                }
            }
        }

        public int? Situacion { get; set; }

        public decimal? Monto { get; set; }

        public int? DiasDeAtraso { get; set; }
        
    }
    
}
