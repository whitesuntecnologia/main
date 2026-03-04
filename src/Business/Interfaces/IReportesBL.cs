using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IReportesBL
    {
        Task<byte[]> GetPdfReliInscripcion(int IdTramite, string wwwrootPath, bool previsualizar = true);
        Task<byte[]> GetPdfReliLicitar(int IdTramite, string wwwrootPath, bool previsualizar = true);
        Task<byte[]> GetPdfReliActualizacion(int IdTramite, string wwwrootPath, bool previsualizar = true);
        Task<byte[]> GetPdfRecoInscripcion(int IdTramite, string wwwrootPath, bool previsualizar = true);
        Task<byte[]> GetPdfReliActualizacionFromEmpresa(int IdEmpresa, string wwwrootPath, bool previsualizar = true);
    }
}
