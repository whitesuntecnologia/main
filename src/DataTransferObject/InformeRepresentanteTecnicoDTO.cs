using DataTransferObject.BLs;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class InformeRepresentanteTecnicoDTO
    {
        public string Apellido { get;set; }
        public string Nombres { get; set; }
        public decimal? Cuit { get; set; }
        public string RazonSocialEmpresa { get; set; }
        public List<EspecialidadDTO> Especialidades { get;set; }
        public List<TramitesDTO> Tramites { get; set; }
        public DateTime? FechaVencimientoMatricula { get; set; }
        public DateTime? FechaVencimientoContrato { get; set; }
        public string Tramites1Linea
        {
            get
            {
                return string.Join(", ", Tramites.Select(s => s.IdTramite.ToString() + 
                                        (s.IdEstado != Constants.TramitesEstados.Aprobado ? " (En Proceso)" : "") 
                                        ).ToArray());
            }
        }
        public string Especialidades1Linea
        {
            get
            {
                return string.Join(Environment.NewLine, Especialidades.Select(s => s.NombreEspecialidad).ToArray());
            }
        }
    }
}
