using DataTransferObject.BLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ICombosBL
    {
        Task<IEnumerable<GenericComboDTO>> GetEspecialidadesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetEspecialidadesFromTramiteAsync(int IdTramite, CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetSeccionesFromEspecialidadTramiteAsync(int IdTramiteEspecialidad, CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetTareasByEspecialidadAsync(int IdEspecialidad, CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetTareasBySeccionAsync(int IdSeccion, CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetTareasBySeccionFromRamaAyBAsync(int IdTramite, int IdSeccion, CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetProvinciasAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetTiposDeDocumentosAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetTiposDeObraAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetPerfilesAsync(CancellationToken cancellationToken = default);
        Task<List<GenericComboDTO>> GetEstados();
        Task<IEnumerable<GenericComboDTO>> GetSituacionesBcraAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetEspecialidadesSeccionesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetEspecialidadesSeccionesAsync(int IdEspecialidad, CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetEspecialidadesSeccionesFromRamaAyBAsync(int IdTramite, int IdEspecialidad, CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetEstadosEvaluacionAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetObrasPciaLP(bool soloActivas = false, string CuitEmpresa = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetObrasPciaLPParaLicitarAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboStrDTO>> GetEstadosObraAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetTramitesEstadosAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetEmpresasAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetTiposDeTramiteAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetTiposDeSociedadAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GenericComboDTO>> GetTiposDeCaracterLegalAsync(CancellationToken cancellationToken = default);
    }
}
