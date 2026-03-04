using System;

namespace DataTransferObject
{
    public class TramitesRepresentantesTecnicosDesvinculacionesDTO
    {
        public int IdDesvinculacion { get; set; }
        public int IdRepresentanteTecnico { get; set; }
        public int IdFileDesvinculacion { get; set; }
        public string FilenameDesvinculacion { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public string Createuser { get; set; } = string.Empty;
       
    }
}