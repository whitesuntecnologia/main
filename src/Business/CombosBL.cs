using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataAccess.Extends;
using DataTransferObject.BLs;
using Repository;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class CombosBL: ICombosBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;

        public CombosBL(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GenericComboEntity, GenericComboDTO>();
                cfg.CreateMap<GenericStringComboEntity, GenericComboStrDTO>();

            });
            _mapper = config.CreateMapper();
        }

        public async Task<IEnumerable<GenericComboDTO>> GetEspecialidadesAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetEspecialidadesAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetEspecialidadesFromTramiteAsync(int IdTramite,CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetEspecialidadesFromTramiteAsync(IdTramite,cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetSeccionesFromEspecialidadTramiteAsync(int IdTramiteEspecialidad, CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetSeccionesFromEspecialidadTramiteAsync(IdTramiteEspecialidad, cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }

        public async Task<IEnumerable<GenericComboDTO>> GetTiposDeObraAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetTiposDeObraAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }

        public async Task<IEnumerable<GenericComboDTO>> GetTareasByEspecialidadAsync(int IdEspecialidad, CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetTareasByEspecielidadAsync(IdEspecialidad, cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetTareasBySeccionAsync(int IdSeccion, CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetTareasBySeccionAsync(IdSeccion, cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetTareasBySeccionFromRamaAyBAsync(int IdTramite, int IdSeccion, CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetTareasBySeccionFromRamaAyBAsync(IdTramite,IdSeccion, cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }

        public async Task<IEnumerable<GenericComboDTO>> GetProvinciasAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetProvinciasAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }

        public async Task<IEnumerable<GenericComboDTO>> GetTiposDeDocumentosAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetTiposDeDocumentosAsync( cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetPerfilesAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetPerfilesAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }

        public Task<List<GenericComboDTO>> GetEstados()
        {
            var elements = (from d in typeof(Constants.UsuariosEstados).GetFields()
                                select new GenericComboDTO
                                {
                                    Id = (int)d.GetRawConstantValue(),
                                    Descripcion = d.Name
                                }).ToList();

            return Task.FromResult(elements);
        }


        public async Task<IEnumerable<GenericComboDTO>> GetSituacionesBcraAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetSituacionesBcraAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }

        public async Task<IEnumerable<GenericComboDTO>> GetEspecialidadesSeccionesAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetEspecialidadesSeccionesAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetEspecialidadesSeccionesAsync(int IdEspecialidad, CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetEspecialidadesSeccionesAsync(IdEspecialidad, cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetEspecialidadesSeccionesFromRamaAyBAsync(int IdTramite, int IdEspecialidad, CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetEspecialidadesSeccionesFromRamaAybAsync(IdTramite, IdEspecialidad, cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        
        public async Task<IEnumerable<GenericComboDTO>> GetEstadosEvaluacionAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetEstadosEvaluacionAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetObrasPciaLP(bool soloActivas = false, string CuitEmpresa = null, CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetObrasPciaLP(soloActivas, CuitEmpresa, cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetObrasPciaLPParaLicitarAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetObrasPciaLPParaLicitarAsync( cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboStrDTO>> GetEstadosObraAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetEstadosObraAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericStringComboEntity>, IEnumerable<GenericComboStrDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetTramitesEstadosAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetTramitesEstadosAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetEmpresasAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetEmpresasAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetTiposDeTramiteAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetTiposDeTramiteAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetTiposDeSociedadAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetTiposDeSociedadAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
        public async Task<IEnumerable<GenericComboDTO>> GetTiposDeCaracterLegalAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            CombosRepository repo = new(uow);
            var elements = await repo.GetTiposDeCaracterLegalAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GenericComboEntity>, IEnumerable<GenericComboDTO>>(elements);
        }
    }
}
