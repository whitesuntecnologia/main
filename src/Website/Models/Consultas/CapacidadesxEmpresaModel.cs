using DataTransferObject;
using DataTransferObject.BLs;

namespace Website.Models.Consultas
{
    public class CapacidadesxEmpresaModel
    {
        public List<GenericComboDTO> Especialidades { get; set; } = new();
        public int VencimientoSelected { get; set; } = 1; // Todas
        public int? IdEspecialidadSelected { get; set; } = 0; //Todas
    }
}
