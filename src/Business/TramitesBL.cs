using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;
using StaticClass;
using Microsoft.AspNetCore.Identity;
using DataAccess;
using System.Security.Authentication;
using DocumentFormat.OpenXml.InkML;
using DataAccess.EntitiesCustom;
using Microsoft.IdentityModel.Xml;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Business
{
    public class TramitesBL: ITramitesBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IMapper _mapperCopiarTramite;
        private readonly IUsuariosBL _usuarioBL;
        private readonly ITablasBL _tablasBL;

        public TramitesBL(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL, ITablasBL tablasBL, UserManager<UserProfile> userManager, ITramitesEvaluacionBL tramiteEvaluacionBL)
        {
            _uowFactory = Guard.Against.Null(uowFactory);
            _usuarioBL = usuarioBL;
            _tablasBL = tablasBL;

            var config = new MapperConfiguration(cfg =>
            {
             
                #region Generales
                cfg.CreateMap<Tramites, TramitesDTO>()
                    .ForMember(dest=> dest.NombreEstado, opt => opt.MapFrom(src => src.IdEstadoNavigation.NombreEstado))
                    .ForMember(dest => dest.CuitEmpresa, opt => opt.MapFrom(src => src.IdEmpresaNavigation.CuitEmpresa))
                    .ForMember(dest => dest.RazonSocial, opt => opt.MapFrom(src => src.IdEmpresaNavigation.RazonSocial))
                    .ForMember(dest => dest.IdGrupoTramite, opt => opt.MapFrom(src => src.IdTipoTramiteNavigation.IdGrupoTramite))
                    .ForMember(dest => dest.NombreTipoTramite, opt => opt.MapFrom(src => src.IdTipoTramiteNavigation.Descripcion))
                    .ForMember(dest => dest.NombreObraPciaLP, opt => opt.MapFrom(src => src.IdObraPciaLpNavigation.ObraNombre))
                    .ForMember(dest => dest.EvaluadorAsignado, opt => opt.MapFrom(src => userManager.Users
                                                                                .Where(x => x.Id == src.UsuarioAsignado)
                                                                                .Select(s => s.NombreyApellido)
                                                                                .FirstOrDefault()))
    ;
                cfg.CreateMap<TramitesDTO,Tramites>();
                cfg.CreateMap<Provincias, ProvinciaDTO>().ReverseMap();
                cfg.CreateMap<Tramites, ItemGrillaConsultarTramitesDTO>()
                    .ForMember(dest => dest.NombreTipoTramite, opt => opt.MapFrom(src => src.IdTipoTramiteNavigation.Descripcion))
                    .ForMember(dest => dest.NombreEstado, opt => opt.MapFrom(src => src.IdEstadoNavigation.NombreEstado))
                ;
                cfg.CreateMap<Tramites, ItemGrillaMisCertificadosDTO>()
                    .ForMember(dest => dest.NombreTipoTramite, opt => opt.MapFrom(src => src.IdTipoTramiteNavigation.Descripcion))
                    
                ;
                cfg.CreateMap<Tramites, ItemGrillaBandejaDTO>()
                    .ForMember(dest => dest.NombreEstado, opt => opt.MapFrom(src => src.IdEstadoNavigation.NombreEstado))
                    .ForMember(dest => dest.NombreTipoTramite, opt => opt.MapFrom(src => src.IdTipoTramiteNavigation.Descripcion))
                    .ForMember(dest => dest.UsernameEmpresa, opt => opt.MapFrom(src => userManager.Users
                                                                                .Where(x => x.Id == src.IdEmpresaNavigation.UseridRepresentante)
                                                                                .Select(s => s.UserName)
                                                                                .FirstOrDefault()))
                ;

                cfg.CreateMap<Tramites, ItemGrillaConsultaTramiteDTO>()
                    
                    .ForMember(dest => dest.NombreEstado, opt => opt.MapFrom(src => src.IdEstadoNavigation.NombreEstado))
                    .ForMember(dest => dest.NombreTipoTramite, opt => opt.MapFrom(src => src.IdTipoTramiteNavigation.Descripcion))
                    .ForMember(dest => dest.UsernameEmpresa, opt => opt.MapFrom(src => userManager.Users
                                                                                .Where(x => x.Id == src.IdEmpresaNavigation.UseridRepresentante)
                                                                                .Select(s => s.UserName)
                                                                                .FirstOrDefault()))
                    .ForMember(dest => dest.UsernameAsignado, opt => opt.MapFrom(src => userManager.Users
                                                                                            .Where(x => x.Id == src.UsuarioAsignado)
                                                                                            .Select(s => s.UserName)
                                                                                            .FirstOrDefault()))
                    .ForMember(dest => dest.RazonSocialEmpresa, opt => opt.MapFrom(src => src.IdEmpresaNavigation.RazonSocial))
                ;

                cfg.CreateMap<TramitesFormulariosEvaluados, TramiteFormularioEvaluadoDTO>()
                    .ForMember(dest=> dest.NombreEstadoEvaluacion, opt => opt.MapFrom( src => src.IdEstadoEvaluacionNavigation.NombreEstadoEvaluacion))
                ;
                cfg.CreateMap<TramiteFormularioEvaluadoDTO, TramitesFormulariosEvaluados>();

                #endregion

                #region Especialidades
                cfg.CreateMap<TramitesEspecialidades, ItemGrillaEspecialidadesDTO>()
                    .ForMember(dest => dest.DescripcionEspecialidad, opt => opt.MapFrom(src => src.IdEspecialidadNavigation.NombreEspecialidad))
                    .ForMember(dest => dest.Rama, opt => opt.MapFrom(src => src.IdEspecialidadNavigation.Rama))
                    .ForMember(dest => dest.Secciones, opt => opt.MapFrom(src => src.TramitesEspecialidadesSecciones))
                ;
                cfg.CreateMap<TramitesEspecialidadesSecciones, ItemGrillaEspecialidadesSeccionesDTO>()
                    .ForMember(dest => dest.DescripcionSeccion, opt => opt.MapFrom(src => src.IdSeccionNavigation.DescripcionSeccion))
                    .ForMember(dest => dest.Tareas, opt => opt.MapFrom(src => src.TramitesEspecialidadesTareas))
                    .ForMember(dest => dest.Baja, opt => opt.MapFrom(src => src.IdSeccionNavigation.Baja))
                    .ForMember(dest => dest.CoefCapacTecnicaxEquipo, opt => opt.MapFrom(src => src.IdSeccionNavigation.CoefCapacTecnicaxEquipo))
                    .ForMember(dest => dest.MultiplicadorCapEconomica, opt => opt.MapFrom(src => src.IdSeccionNavigation.MultiplicadorCapEconomica))

                ;
                cfg.CreateMap<TramitesEspecialidadesTareas, ItemGrillaEspecialidadesTareasDTO>()
                    .ForMember(dest => dest.DescripcionTarea, opt => opt.MapFrom(src => src.IdTareaNavigation.DescripcionTarea))
                    .ForMember(dest => dest.Equipos, opt => opt.MapFrom(src => src.TramitesEspecialidadesEquipos))
                ;
                cfg.CreateMap<TramitesEspecialidadesEquipos, ItemGrillaEspecialidadesEquiposDTO>()
                    .ForMember(dest => dest.DescripcionEquipo, opt => opt.MapFrom(src => src.IdEquipoNavigation.DescripcionEquipo))
                ;
                #endregion

                #region Representantes técnicos
                cfg.CreateMap<TramitesRepresentanteTecnicoJurisdiccionDTO, TramitesRepresentantesTecnicosJurisdicciones>();
                cfg.CreateMap<TramitesRepresentantesTecnicosJurisdicciones, TramitesRepresentanteTecnicoJurisdiccionDTO>()
                    .ForMember(dest => dest.DescripcionProvincia, opt => opt.MapFrom(src => src.IdProvinciaNavigation.Descripcion))
                ;
                
                cfg.CreateMap<TramitesRepresentanteTecnicoDTO, TramitesRepresentantesTecnicos>();
                cfg.CreateMap<TramitesRepresentantesTecnicos, TramitesRepresentanteTecnicoDTO>()
                    //Identifica si el representante técnico se encuentra registrado en la empresa (en base al CUIT) para mostrar o no la desvinculacion
                    .ForMember(dest => dest.EstaRegistrado, opt => opt.MapFrom(src => src.IdTramiteNavigation.IdEmpresaNavigation.EmpresasRepresentantesTecnicos.Any(x=> x.Cuit == src.Cuit)))
                    .ForMember(dest => dest.EstaDesvinculado, opt => opt.MapFrom(src => src.TramitesRepresentantesTecnicosDesvinculaciones != null))
                    .ForMember(dest => dest.IdFileDesvinculacion, opt => opt.MapFrom(src => src.TramitesRepresentantesTecnicosDesvinculaciones.IdFileDesvinculacion))
                ;
                cfg.CreateMap<TramitesRepresentanteTecnicoEspecilidadDTO, TramitesRepresentantesTecnicosEspecialidades>();
                cfg.CreateMap<TramitesRepresentantesTecnicosEspecialidades, TramitesRepresentanteTecnicoEspecilidadDTO>()
                   .ForMember(dest => dest.NombreEspecialidad, opt => opt.MapFrom(src => src.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation.NombreEspecialidad))
                ;
                cfg.CreateMap<InformeRepresentanteTecnicoCustom, InformeRepresentanteTecnicoDTO>();
                cfg.CreateMap<Especialidades, EspecialidadDTO>();
                cfg.CreateMap<TramitesRepresentantesTecnicosDesvinculacionesDTO, TramitesRepresentantesTecnicosDesvinculaciones>();
                cfg.CreateMap<TramitesRepresentantesTecnicosDesvinculaciones, TramitesRepresentantesTecnicosDesvinculacionesDTO>();
                #endregion

                #region Información de Empresa
                cfg.CreateMap<TramitesInfEmp, TramitesInfEmpDTO>().ReverseMap();
                cfg.CreateMap<TramitesInfEmpDocumentos, TramitesInfEmpDocumentoDTO>()
                    .ForMember(dest=> dest.DescripcionTipoDocumento, opt => opt.MapFrom( src => src.IdTipoDocumentoNavigation.Descripcion))
                    .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.IdFileNavigation.Size))
                ;
                cfg.CreateMap<TramitesInfEmpDocumentoDTO, TramitesInfEmpDocumentos>();
                
                cfg.CreateMap<TramitesInfEmpDeudaDTO, TramitesInfEmpDeudas>();
                cfg.CreateMap<TramitesInfEmpDeudas, TramitesInfEmpDeudaDTO>()
                    .ForMember(dest => dest.IdTramite, opt => opt.MapFrom(src => src.IdTramiteInfEmpNavigation.IdTramite))
                ;

                cfg.CreateMap<TramitesInfEmpCons, TramitesInfEmpConsDto>()
                    .ForMember(dest => dest.Personas, opt => opt.MapFrom(src => src.TramitesInfEmpConsPersonas))
                    .ForMember(dest => dest.Documentos, opt => opt.MapFrom(src => src.TramitesInfEmpConsDocumentos))
                    .ForMember(dest => dest.Deudas, opt => opt.MapFrom(src => src.TramitesInfEmpConsDeudas))
                ;
                cfg.CreateMap<TramitesInfEmpConsDto, TramitesInfEmpCons>();
                cfg.CreateMap<TramitesInfEmpConsPersonas, TramitesInfEmpConsPersonaDto>().ReverseMap();
                cfg.CreateMap<TramitesInfEmpConsDocumentos, TramitesInfEmpConsDocumentoDto>()
                    .ForMember(dest => dest.DescripcionTipoDocumento, opt => opt.MapFrom(src => src.IdTipoDocumentoNavigation.Descripcion))
                    .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.IdFileNavigation.Size))
                ;
                cfg.CreateMap<TramitesInfEmpConsDeudas, TramitesInfEmpConsDeudaDto>()
                    .ForMember(dest => dest.IdTramite, opt => opt.MapFrom(src => src.IdTramiteInfEmpConsNavigation.IdTramite))
                ;

                cfg.CreateMap<TramitesInfEmpConsDocumentoDto, TramitesInfEmpConsDocumentos>();
                cfg.CreateMap<TramitesInfEmpConsDeudaDto, TramitesInfEmpConsDeudas>();

                #endregion

                #region Balance General
                cfg.CreateMap<TramitesBalancesGenerales, TramitesBalanceGeneralDTO>()
                    .ForMember(dest => dest.IdTipoTramite, opt => opt.MapFrom(src => src.IdTramiteNavigation.IdTipoTramite))
                ;
                cfg.CreateMap<TramitesBalanceGeneralDTO, TramitesBalancesGenerales>();
                #endregion

                #region Equipos de la Empresa
                cfg.CreateMap<TramitesEquipos, TramitesEquipoDTO>();
                cfg.CreateMap<TramitesEquipoDTO, TramitesEquipos>();
                #endregion

                #region Bienes Raíces de la Empresa
                cfg.CreateMap<TramitesBienesRaices, TramitesBienesRaicesDTO>()
                    .ForMember(dest=> dest.SinDatosForm8, opt => opt.MapFrom( src => src.IdTramiteNavigation.SinDatosForm8 ))
                ;
                cfg.CreateMap<TramitesBienesRaicesDTO, TramitesBienesRaices>();
                #endregion

                #region Obras
                cfg.CreateMap<TramitesObras, TramitesObrasDTO>()
                .ForMember(dest => dest.NombreEspecialidad, opt => opt.MapFrom(src => src.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation.NombreEspecialidad))
                .ForMember(dest => dest.NombreObra, opt => opt.MapFrom(src => src.IdObraPciaLpNavigation.ObraNombre))
                .ForMember(dest => dest.Expediente, opt => opt.MapFrom(src => src.IdObraPciaLpNavigation.Expediente))
                .ForMember(dest => dest.NombreSeccion, opt => opt.MapFrom(src => src.IdTramiteEspecialidadSeccionNavigation.IdSeccionNavigation.DescripcionSeccion))
                .ForMember(dest => dest.NombreTipoObra , opt => opt.MapFrom( src => src.IdTipoObraNavigation.Nombre))
                ;
                cfg.CreateMap<TramitesObrasDTO, TramitesObras>();
                #endregion

                #region Obras en Ejecucion
                cfg.CreateMap<TramitesObrasEjecucion, TramitesObraEjecucionDTO>()
                    .ForMember(dest => dest.NombreTipoObra, opt => opt.MapFrom(src => src.IdTipoObraNavigation.Nombre))
                    .ForMember(dest => dest.ObraNombre, opt => opt.MapFrom(src => src.IdObraPciaLpNavigation.ObraNombre))
                    .ForMember(dest => dest.PorcentajeParticipacionUTE, opt => opt.MapFrom(src => src.IdObraPciaLpNavigation.PorcentajeParticipacion))
                ;
                cfg.CreateMap<TramitesObraEjecucionDTO, TramitesObrasEjecucion>();
                #endregion

                #region Mapeo para copiar de Tramites a Empresas - Balances

                // Mapeo del balance general principal (TramitesBalancesGenerales → EmpresasBalancesGenerales)
                cfg.CreateMap<TramitesBalancesGenerales, EmpresasBalancesGenerales>()
                    .ForMember(dest => dest.IdEmpresaBalanceGeneral, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateUser, opt => opt.Ignore())
                    ;

                // Mapeo de la evaluación general (TramitesBalancesGeneralesEvaluar → EmpresasBalancesGeneralesEvaluar)
                cfg.CreateMap<TramitesBalancesGeneralesEvaluar, EmpresasBalancesGeneralesEvaluar>()
                    .ForMember(dest => dest.IdEmpresaBalanceGeneralEvaluar, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateUser, opt => opt.Ignore());

                // Mapeo de Constancia de Capacidad
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarConstanciaCap, EmpresasConstanciaCap>()
                    .ForMember(dest => dest.IdEmpresaConstanciaCap, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore());

                // Mapeo de Capacidad Técnica
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarCapTecnica, EmpresasCapTecnica>()
                    .ForMember(dest => dest.IdEmpresaCapTecnica, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore());

                // Mapeo de Capacidad de Ejecución
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarCapEjec, EmpresasCapEjec>()
                    .ForMember(dest => dest.IdEmpresaCapEjec, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore());

                // Mapeo de Capacidad de Producción
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarCapProd, EmpresasCapProd>()
                    .ForMember(dest => dest.IdEmpresaCapProd, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore());

                // Mapeo de Capacidad Técnica por Equipo
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarCapTecxEquipo, EmpresasCapTecxEquipo>()
                    .ForMember(dest => dest.IdEmpresaCapTecxEquipo, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore());

                // Mapeo de Detalle de Obras en Ejecución
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarDetalleObrasEjecucion, EmpresasDetalleObrasEjecucion>()
                    .ForMember(dest => dest.IdEmpresaDetalleObrasEjecucion, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore());

                #endregion

                #region Mapeo para copiar de Tramites a Empresas - Otras tablas
                
                // Mapeo de Información de Empresa
                cfg.CreateMap<TramitesInfEmp, EmpresasInf>()
                    .ForMember(dest => dest.IdEmpresaInf, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore());
                ;
                // Mapeo de Documentos de Información de Empresa
                cfg.CreateMap<TramitesInfEmpDocumentos, EmpresasInfDocumentos>()
                    .ForMember(dest => dest.IdEmpresaInfDocumento, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore());

                // Mapeo de Equipos
                cfg.CreateMap<TramitesEquipos, EmpresasEquipos>()
                    .ForMember(dest => dest.IdEmpresaEquipo, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore());

                // Mapeo de Bienes Raíces
                cfg.CreateMap<TramitesBienesRaices, EmpresasBienesRaices>()
                    .ForMember(dest => dest.IdEmpresaBienRaiz, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore());

                // Mapeo de Obras
                cfg.CreateMap<TramitesObras, EmpresasObras>()
                    .ForMember(dest => dest.IdEmpresaObra, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaEspecialidad, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaEspecialidadNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaEspecialidadSeccion, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaEspecialidadSeccionNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore());

                // Mapeo de Antecedentes
                cfg.CreateMap<TramitesAntecedentes, EmpresasAntecedentes>()
                    .ForMember(dest => dest.IdEmpresaAntecedente, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaEspecialidad, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaEspecialidadNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaEspecialidadSeccion, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaEspecialidadSeccionNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.EmpresasAntecedentesDdjjmensual,
                               opt => opt.MapFrom(src => src.TramitesAntecedentesDdjjmensual));

                // Mapeo de Antecedentes DDJJ Mensual
                cfg.CreateMap<TramitesAntecedentesDdjjmensual, EmpresasAntecedentesDdjjmensual>()
                    .ForMember(dest => dest.IdEmpresaAntecedenteDdjjmensual, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaAntecedente, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaAntecedenteNavigation, opt => opt.Ignore());

                // Mapeo de Obras en Ejecución
                cfg.CreateMap<TramitesObrasEjecucion, EmpresasObrasEjecucion>()
                    .ForMember(dest => dest.IdEmpresaObraEjec, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresa, opt => opt.Ignore())
                    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore());

                #endregion
            });
            _mapper = config.CreateMapper();

            var config2 = new MapperConfiguration(cfg =>
            {

                #region Mapeo para copiar de Empresas a Tramites - Balances

                

                cfg.CreateMap<EmpresasInf, TramitesInfEmp>()
                    .ForMember(dest => dest.IdTramiteInfEmp, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramite, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.TramitesInfEmpDeudas, opt => opt.MapFrom( src=> src.IdEmpresaNavigation.EmpresasDeudas))
                    .ForMember(dest => dest.TramitesInfEmpDocumentos, opt => opt.MapFrom(src => src.IdEmpresaNavigation.EmpresasInfDocumentos))
                ;
                cfg.CreateMap<EmpresasDeudas, TramitesInfEmpDeudas>()
                    .ForMember(dest => dest.IdTramiteInfEmpDeuda, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteInfEmp, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteInfEmpNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                ;
                cfg.CreateMap<EmpresasInfDocumentos, TramitesInfEmpDocumentos>()
                    .ForMember(dest => dest.IdTramiteInfEmpDocumento, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteInfEmp, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteInfEmpNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                ;

                // Mapeo del balance general principal (EmpresasBalancesGenerales → TramitesBalancesGenerales)
                cfg.CreateMap<EmpresasBalancesGenerales, TramitesBalancesGenerales>()
                    .ForMember(dest => dest.IdTramiteBalanceGeneral, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramite, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluar, opt => opt.MapFrom(src => src.IdEmpresaNavigation.EmpresasBalancesGeneralesEvaluar.FirstOrDefault()))

                    // ===== OFICIALES =====
                    .ForMember(d => d.OoficDepositosCortoPlazoEmp,
                               o => o.MapFrom(s => s.OoficDepositosCortoPlazo))
                    .ForMember(d => d.OoficDepositosLargoPlazoEmp,
                               o => o.MapFrom(s => s.OoficDepositosLargoPlazo))
                    .ForMember(d => d.OoficInversionesCortoPlazoEmp,
                               o => o.MapFrom(s => s.OoficInversionesCortoPlazo))
                    .ForMember(d => d.OoficInversionesLargoPlazoEmp,
                               o => o.MapFrom(s => s.OoficInversionesLargoPlazo))
                    .ForMember(d => d.OoficOtrosConceptosEmp,
                               o => o.MapFrom(s => s.OoficOtrosConceptos))

                    // ===== OTRAS PARTIDAS =====
                    .ForMember(d => d.OpartDepositosCortoPlazoEmp,
                               o => o.MapFrom(s => s.OpartDepositosCortoPlazo))
                    .ForMember(d => d.OpartDepositosLargoPlazoEmp,
                               o => o.MapFrom(s => s.OpartDepositosLargoPlazo))
                    .ForMember(d => d.OpartInversionesCortoPlazoEmp,
                               o => o.MapFrom(s => s.OpartInversionesCortoPlazo))
                    .ForMember(d => d.OpartInversionesLargoPlazoEmp,
                               o => o.MapFrom(s => s.OpartInversionesLargoPlazo))
                    .ForMember(d => d.OpartInversionesEmp,
                               o => o.MapFrom(s => s.OpartInversiones))
                    .ForMember(d => d.OpartOtrosConceptosEmp,
                               o => o.MapFrom(s => s.OpartOtrosConceptos))

                    // ===== DISPONIBILIDADES =====
                    .ForMember(d => d.DispCajayBancosEmp,
                               o => o.MapFrom(s => s.DispCajayBancos))

                    // ===== BIENES DE USO =====
                    .ForMember(d => d.BusoInversionesEmp,
                               o => o.MapFrom(s => s.BusoInversiones))
                    .ForMember(d => d.BusoMaqUtilAfecEmp,
                               o => o.MapFrom(s => s.BusoMaqUtilAfec))
                    .ForMember(d => d.BusoMaqUtilNoAfecEmp,
                               o => o.MapFrom(s => s.BusoMaqUtilNoAfec))
                    .ForMember(d => d.BusoBienesRaicesAfecEmp,
                               o => o.MapFrom(s => s.BusoBienesRaicesAfec))
                    .ForMember(d => d.BusoBienesRaicesNoAfecEmp,
                               o => o.MapFrom(s => s.BusoBienesRaicesNoAfec))
                    .ForMember(d => d.BusoOtrosConceptosEmp,
                               o => o.MapFrom(s => s.BusoOtrosConceptos))

                    // ===== BIENES DE CAMBIO =====
                    .ForMember(d => d.BcamMaterialesEmp,
                               o => o.MapFrom(s => s.BcamMateriales))
                    .ForMember(d => d.BcamOtrosConceptosEmp,
                               o => o.MapFrom(s => s.BcamOtrosConceptos))

                    // ===== DEUDAS =====
                    .ForMember(d => d.DeuCortoPlazoEmp,
                               o => o.MapFrom(s => s.DeuCortoPlazo))
                    .ForMember(d => d.DeuLargoPlazoEmp,
                               o => o.MapFrom(s => s.DeuLargoPlazo))

                    ;


                // Mapeo de la evaluación general (EmpresasBalancesGeneralesEvaluar → TramitesBalancesGeneralesEvaluar)
                cfg.CreateMap<EmpresasBalancesGeneralesEvaluar, TramitesBalancesGeneralesEvaluar>()
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluar, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneral, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarCapEjec, opt => opt.MapFrom(src=> src.IdEmpresaNavigation.EmpresasCapEjec))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarCapProd, opt => opt.MapFrom(src => src.IdEmpresaNavigation.EmpresasCapProd))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarCapTecnica, opt => opt.MapFrom(src => src.IdEmpresaNavigation.EmpresasCapTecnica))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarCapTecxEquipo, opt => opt.MapFrom(src => src.IdEmpresaNavigation.EmpresasCapTecxEquipo))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarConstanciaCap, opt => opt.MapFrom(src => src.IdEmpresaNavigation.EmpresasConstanciaCap))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarDetalleObrasEjecucion, opt => opt.MapFrom(src => src.IdEmpresaNavigation.EmpresasDetalleObrasEjecucion))
                ;

                // Mapeo de Constancia de Capacidad
                cfg.CreateMap<EmpresasConstanciaCap, TramitesBalancesGeneralesEvaluarConstanciaCap>()
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarCc, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluar, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarNavigation, opt => opt.Ignore());

                // Mapeo de Capacidad Técnica
                cfg.CreateMap<EmpresasCapTecnica, TramitesBalancesGeneralesEvaluarCapTecnica>()
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarCt, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluar, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarNavigation, opt => opt.Ignore());

                // Mapeo de Capacidad de Ejecución
                cfg.CreateMap<EmpresasCapEjec, TramitesBalancesGeneralesEvaluarCapEjec>()
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarCe, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluar, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarNavigation, opt => opt.Ignore());

                // Mapeo de Capacidad de Producción
                cfg.CreateMap<EmpresasCapProd, TramitesBalancesGeneralesEvaluarCapProd>()
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarCp, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluar, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarNavigation, opt => opt.Ignore());

                // Mapeo de Capacidad Técnica por Equipo
                cfg.CreateMap<EmpresasCapTecxEquipo, TramitesBalancesGeneralesEvaluarCapTecxEquipo>()
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarCte, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluar, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarNavigation, opt => opt.Ignore());

                // Mapeo de Detalle de Obras en Ejecución
                cfg.CreateMap<EmpresasDetalleObrasEjecucion, TramitesBalancesGeneralesEvaluarDetalleObrasEjecucion>()
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarDoe, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluar, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteBalanceGeneralEvaluarNavigation, opt => opt.Ignore());

                #endregion

                cfg.CreateMap<EmpresasEquipos, TramitesEquipos>()
                    .ForMember(dest => dest.IdTramiteEquipo, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramite, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                ;

                cfg.CreateMap<EmpresasBienesRaices, TramitesBienesRaices>()
                    .ForMember(dest => dest.IdTramiteBienRaiz, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramite, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                ;

                cfg.CreateMap<EmpresasObras, TramitesObras>()
                    .ForMember(dest => dest.IdTramiteObra, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramite, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteEspecialidad, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteEspecialidadNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteEspecialidadSeccion, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteEspecialidadSeccionNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateUser, opt => opt.Ignore())
                ;

                cfg.CreateMap<EmpresasAntecedentes, TramitesAntecedentes>()
                    .ForMember(dest => dest.IdTramiteAntecedente, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramite, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteNavigation, opt => opt.Ignore())

                    // Estas FK NO se pueden mapear directamente porque son de contextos diferentes
                    // Deben ser asignadas manualmente buscando la especialidad/sección correspondiente en el trámite
                    .ForMember(dest => dest.IdTramiteEspecialidad, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteEspecialidadNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteEspecialidadSeccion, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteEspecialidadSeccionNavigation, opt => opt.Ignore())

                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateUser, opt => opt.Ignore())

                    // Mapear la colección hija
                    .ForMember(dest => dest.TramitesAntecedentesDdjjmensual,
                               opt => opt.MapFrom(src => src.EmpresasAntecedentesDdjjmensual))
                ;

                // Mapeo de la tabla hija DDJJ Mensual
                cfg.CreateMap<EmpresasAntecedentesDdjjmensual, TramitesAntecedentesDdjjmensual>()
                    .ForMember(dest => dest.IdTramiteAntecedenteDdjjmensual, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteAntecedente, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteAntecedenteNavigation, opt => opt.Ignore())
                ;

                cfg.CreateMap<EmpresasObrasEjecucion, TramitesObrasEjecucion>()
                    .ForMember(dest => dest.IdTramiteObraEjec, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramite, opt => opt.Ignore())
                    .ForMember(dest => dest.IdTramiteNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateDate, opt => opt.Ignore())
                    .ForMember(dest => dest.LastUpdateUser, opt => opt.Ignore())
                ;
            });
            _mapperCopiarTramite = config2.CreateMapper();
        }
        #region Tramite
        public async Task<TramitesDTO> CrearTramiteAsync(int IdTipoTramite)
        {
            TramitesDTO result = null;
            string userid = await _usuarioBL.GetCurrentUserid();
            int userIdEmpresa = 0;
            int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out userIdEmpresa);

            if (userIdEmpresa == 0)
                throw new ArgumentException("No se encontró la empresa relacionada al usuario. Por favor comuniquelo al Administrador.");
            
            using var uow = _uowFactory.GetUnitOfWork();

            var _repoTramites = new TramitesRepository(uow);

            var tramiteEntity = await _repoTramites.AddAsync(new Tramites
            {
                IdentificadorUnico = Guid.NewGuid().ToString(),
                IdTipoTramite = IdTipoTramite,
                IdEmpresa = userIdEmpresa,
                IdEstado = StaticClass.Constants.TramitesEstados.EditarInformacion,
                CreateDate = DateTime.Now,
                CreateUser = userid,
            });

            result = _mapper.Map<Tramites, TramitesDTO>(tramiteEntity);

            return result;
        }
        public async Task<TramitesDTO> CrearTramiteCertParaLicitarAsync(int IdObraPciaLP)
        {
            TramitesDTO result = null;
            string userid = await _usuarioBL.GetCurrentUserid();
            int userIdEmpresa = 0;
            Tramites TramiteActualizacionCompletaPresentada = null;
            int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out userIdEmpresa);
            using var uow = _uowFactory.GetUnitOfWork();

            var _repoTramites = new TramitesRepository(uow);
            var _repoEmpresas = new EmpresasRepository(uow);
            var _repoEmpresasRepresentantesTecnicos = new EmpresasRepresentantesTecnicosRepository(uow);
            var _repoTramitesRepresentantesTecnicos = new TramitesRepresentantesTecnicosRepository(uow);

            var tramiteAnterior = await _repoTramites.FirstOrDefaultAsync(x => x.IdEmpresa == userIdEmpresa 
                                                                            && x.IdObraPciaLp == IdObraPciaLP 
                                                                            && (x.IdEstado != Constants.TramitesEstados.Anulado 
                                                                                && x.IdEstado != Constants.TramitesEstados.Rechazado)
                                                                          );
            if (tramiteAnterior != null)
            {
                throw new ArgumentException($"No se puede iniciar el trámite debido a que ya existe actuamente un trámite con el mismo pedido. Trámite Nro {tramiteAnterior.IdTramite}");
            }

            var empresaEntity = await _repoEmpresas.FirstOrDefaultAsync(x=> x.IdEmpresa == userIdEmpresa);
            var tramiteOrigen = await _repoTramites.FirstOrDefaultAsync(x => x.IdTramite == empresaEntity.IdTramiteOrigen.GetValueOrDefault());

            //Se verifica si hay una actualización completa presentada (en evaluación, elevado para la firma u observado)
            TramiteActualizacionCompletaPresentada = await _repoTramites.Where(x => x.IdEmpresa == userIdEmpresa &&
                                                    (x.IdEstado == Constants.TramitesEstados.EnEvaluacion ||
                                                     x.IdEstado == Constants.TramitesEstados.ElevadoParaLaFirma ||
                                                     x.IdEstado == Constants.TramitesEstados.Observado)
                                                     && 
                                                     (
                                                        x.IdTipoTramite != Constants.TiposDeTramite.Reli_Licitar 
                                                        && x.IdTipoTramiteNavigation.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores)
                                                     )
                                                    .OrderByDescending(o=> o.IdTramite)
                                                    .FirstOrDefaultAsync();


            if (tramiteOrigen == null)
            {
                throw new ArgumentException($"No se puede iniciar el trámite debido a que no hay ningún trámite en estado aprobado.");
            }

            if (empresaEntity.Vencimiento < DateTime.Today)
            {
                //Si hay una actualización completa presentada (en evaluación, elevado para la firma u observado)
                //se permite iniciar el trámite aunque el último aprobado se encuentre vencido, ya que se entiende que se está en proceso de actualización de la capacidad técnica.
                if (TramiteActualizacionCompletaPresentada != null)
                {
                    //Calcula el Vencimiento de la capacidad del trámite que se está presentando.
                    var FechaBalance = TramiteActualizacionCompletaPresentada.TramitesBalancesGenerales.Max(m => m.FechaBalance);
                    var FechaVencimiento = FechaBalance.AddMonths(18);

                    if (FechaVencimiento < DateTime.Today)
                        throw new ArgumentException($"No se puede iniciar el trámite debido a que la capacidad se encuentra vencida  y el trámite que está en proceso de evaluación no tiene correctamente la fecha de balance o bien está vencida {FechaVencimiento}.");
                }
                else
                {
                    var FechaVencimiento = empresaEntity.Vencimiento.HasValue ? empresaEntity.Vencimiento.Value.ToString("dd/MM/yyyy") : string.Empty;
                    throw new ArgumentException($"No se puede iniciar el trámite debido a que la capacidad se encuentra vencida desde el {FechaVencimiento}. Presentar inscripción o actualización según corresponda.");
                }

            }


            if (!(await _repoEmpresasRepresentantesTecnicos.AnyAsync(x => x.IdEmpresa == userIdEmpresa && x.FechaVencimientoContrato > DateTime.Today)))
            {
                //Si hay una actualización completa presentada (en evaluación, elevado para la firma u observado)
                //se permite iniciar el trámite aunque el último aprobado se encuentre vencido, ya que se entiende que se está en proceso de actualización de la capacidad técnica.
                if (TramiteActualizacionCompletaPresentada != null)
                {
                    var HayRepresentantesTecnicosVigentes = TramiteActualizacionCompletaPresentada.TramitesRepresentantesTecnicos
                        .Where(x => x.TramitesRepresentantesTecnicosDesvinculaciones == null && x.FechaVencimientoContrato >= DateTime.Today)
                        .Any();
                    if (!HayRepresentantesTecnicosVigentes)
                        throw new ArgumentException($"No se puede iniciar el trámite debido a que la actualización presentada no posee representantes técnicos vigentes. Debe contar con al menos un representante técnico con contrato vigente para iniciar el trámite.");
                }
                else
                {
                    throw new ArgumentException($"No se puede iniciar el trámite debido a que la empresa no posee representantes técnicos vigentes. Debe contar con al menos un representante técnico con contrato vigente para iniciar el trámite o bien un trámite de actualización presentado (En Evaluación).");
                }
            }


            var tramiteEntity = new Tramites
            {
                IdentificadorUnico = Guid.NewGuid().ToString(),
                IdTipoTramite = Constants.TiposDeTramite.Reli_Licitar,
                IdObraPciaLp = IdObraPciaLP,
                IdEmpresa = userIdEmpresa,
                IdEstado = StaticClass.Constants.TramitesEstados.EditarInformacion,
                IdTramiteOrigen = tramiteOrigen.IdTramite,
                NroBoleta = tramiteOrigen.NroBoleta,
                IdFileBoleta = tramiteOrigen.IdFileBoleta,
                FilenameBoleta = tramiteOrigen.FilenameBoleta,
                IdFileCumplimientoFiscal = tramiteOrigen.IdFileCumplimientoFiscal,
                FilenameCumplimientoFiscal = tramiteOrigen.FilenameCumplimientoFiscal,
                SinDatosForm8 = tramiteOrigen.SinDatosForm8,
                SinDatosForm10 = tramiteOrigen.SinDatosForm10,
                SinDatosForm11 = tramiteOrigen.SinDatosForm11,
                SinDatosForm12 = tramiteOrigen.SinDatosForm12,
                CreateDate = DateTime.Now,
                CreateUser = userid,
            };


            tramiteEntity = await CopiarDatosTramite(userid, tramiteOrigen, tramiteEntity);

            // Si el trámite de licitación no tiene representantes técnicos pero hay una actualización completa presentada,
            // se copian los representantes técnicos vigentes de la actualización completa al nuevo trámite de licitación.
            if (!tramiteEntity.TramitesRepresentantesTecnicos.Any() && tramiteEntity.IdTipoTramite == Constants.TiposDeTramite.Reli_Licitar)
            {
                if (TramiteActualizacionCompletaPresentada != null)
                {
                    // Si el trámite no tiene representantes técnicos pero hay una actualización completa presentada,
                    // se copian los representantes técnicos vigentes de la actualización completa al nuevo trámite de licitación.
                    var RepresentantesTecnicosEntity = TramiteActualizacionCompletaPresentada.TramitesRepresentantesTecnicos
                        // No copiar los desvinculados ni los vencidos
                        .Where(x => x.TramitesRepresentantesTecnicosDesvinculaciones == null && x.FechaVencimientoContrato > DateTime.Today) 
                        .Select(s => new TramitesRepresentantesTecnicos
                        {
                            IdTramite = tramiteEntity.IdTramite,
                            Apellido = s.Apellido,
                            Nombres = s.Nombres,
                            Cuit = s.Cuit,
                            Cargo = s.Cargo,
                            Matricula = s.Matricula,        
                            FechaVencimientoMatricula = s.FechaVencimientoMatricula,
                            FechaVencimientoContrato = s.FechaVencimientoContrato,
                            IdFileContrato = s.IdFileContrato,
                            FilenameContrato = s.FilenameContrato,
                            IdFileBoleta = s.IdFileBoleta,
                            FilenameBoleta = s.FilenameBoleta,
                            IdFileMatricula = s.IdFileMatricula,
                            FilenameMatricula = s.FilenameMatricula,
                            CreateDate = DateTime.Now,
                            CreateUser = userid,
                            TramitesRepresentantesTecnicosEspecialidades = s.TramitesRepresentantesTecnicosEspecialidades
                                .Select(se => new TramitesRepresentantesTecnicosEspecialidades
                                {
                                    IdTramiteEspecialidad = tramiteEntity.TramitesEspecialidades
                                        .First(x => x.IdEspecialidad == se.IdTramiteEspecialidadNavigation.IdEspecialidad).IdTramiteEspecialidad,
                                }).ToList(),
                            TramitesRepresentantesTecnicosJurisdicciones = s.TramitesRepresentantesTecnicosJurisdicciones
                                .Select(sj => new TramitesRepresentantesTecnicosJurisdicciones
                                {
                                    IdProvincia = sj.IdProvincia,
                                }).ToList()
                        }).ToList();

                    // Persistir los representantes técnicos en la base de datos
                    await _repoTramitesRepresentantesTecnicos.UpdateARangeAsync(RepresentantesTecnicosEntity);
                    tramiteEntity.TramitesRepresentantesTecnicos = RepresentantesTecnicosEntity;
                }
            }

            result = _mapper.Map<Tramites, TramitesDTO>(tramiteEntity);

            return result;
        }
        public async Task<TramitesDTO> CrearTramiteActualizacionAsync(int IdTipoTramite, int IdEmpresa)
        {
            TramitesDTO result = null;
            string userid = await _usuarioBL.GetCurrentUserid();
            using var uow = _uowFactory.GetUnitOfWork();

            var _repoTramites = new TramitesRepository(uow);

            var tramitePendiente = await _repoTramites.Where(x => x.IdEmpresa == IdEmpresa 
                                                    && (x.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionSoloTecnicos ||
                                                        x.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCapacidadTecnica ||
                                                        x.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCompleta )
                                                    && x.IdEstado != Constants.TramitesEstados.Aprobado
                                                    && x.IdEstado != Constants.TramitesEstados.Rechazado
                                                    && x.IdEstado != Constants.TramitesEstados.Anulado)
                                                    .FirstOrDefaultAsync();
            if(tramitePendiente != null)
            {
                throw new ArgumentException($"No se puede iniciar el trámite debido a que existe otro trámite de Actualización pendiente de resolución. El mismo es el Nro: {tramitePendiente.IdTramite}");
            }


            var tramiteOrigen = await _repoTramites.Where(x => x.IdEmpresa == IdEmpresa && x.IdEstado == Constants.TramitesEstados.Aprobado)
                                                    .OrderByDescending(o => o.IdTramite)
                                                    .FirstOrDefaultAsync();

            if (tramiteOrigen == null)
            {
                throw new ArgumentException($"No se puede iniciar el trámite debido a que no hay ningún trámite en estado aprobado.");
            }

            var tramiteEntity = new Tramites
            {
                IdentificadorUnico = Guid.NewGuid().ToString(),
                IdTipoTramite = IdTipoTramite,
                IdObraPciaLp = null,
                IdEmpresa = IdEmpresa,
                IdEstado = StaticClass.Constants.TramitesEstados.EditarInformacion,
                NroBoleta = tramiteOrigen.NroBoleta,
                IdFileBoleta = tramiteOrigen.IdFileBoleta,
                FilenameBoleta = tramiteOrigen.FilenameBoleta,
                IdFileCumplimientoFiscal = tramiteOrigen.IdFileCumplimientoFiscal,
                FilenameCumplimientoFiscal = tramiteOrigen.FilenameCumplimientoFiscal,
                SinDatosForm8 = tramiteOrigen.SinDatosForm8,
                SinDatosForm10 = tramiteOrigen.SinDatosForm10,
                SinDatosForm11 = tramiteOrigen.SinDatosForm11,
                SinDatosForm12 = tramiteOrigen.SinDatosForm12,
                IdTramiteOrigen = tramiteOrigen.IdTramite,
                CreateDate = DateTime.Now,
                CreateUser = userid,
            };

            tramiteEntity = await CopiarDatosTramite(userid, tramiteOrigen, tramiteEntity);
            result = _mapper.Map<Tramites, TramitesDTO>(tramiteEntity);

            return result;
        }

        private async Task<Tramites> CopiarDatosTramite(string userid, Tramites tramiteOrigen, Tramites tramiteDestino)
        {
            var uowR = _uowFactory.GetUnitOfWork();
            var repoTramitesRead = new TramitesRepository(uowR);
            
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoTramites = new TramitesRepository(uow);
                    var repoEmpresas = new EmpresasRepository(uow);

                    tramiteDestino = await repoTramites.AddAsync(tramiteDestino);
                    var empresaEntity = await repoEmpresas.FirstOrDefaultAsync(x => x.IdEmpresa == tramiteOrigen.IdEmpresa);

                    //Copia de Especialidades
                    tramiteDestino.TramitesEspecialidades = empresaEntity.EmpresasEspecialidades.Select(s => new TramitesEspecialidades
                    {
                        IdEspecialidad = s.IdEspecialidad,
                        CreateDate = DateTime.Now,
                        CreateUser = userid,
                        TramitesEspecialidadesSecciones = s.EmpresasEspecialidadesSecciones.Select(ss => new TramitesEspecialidadesSecciones
                        {
                            IdSeccion = ss.IdSeccion,
                            CreateDate = DateTime.Now,
                            CreateUser = userid,
                            TramitesEspecialidadesTareas = ss.EmpresasEspecialidadesTareas.Select(st => new TramitesEspecialidadesTareas
                            {
                                IdTarea = st.IdTarea,
                                TramitesEspecialidadesEquipos = st.EmpresasEspecialidadesEquipos.Select(se => new TramitesEspecialidadesEquipos
                                {
                                    IdEquipo = se.IdEquipo,
                                }).ToList(),
                            }).ToList(),
                        }).ToList(),
                    }).ToList();

                    tramiteDestino = await repoTramites.UpdateAsync(tramiteDestino);
                    var Especialidades = tramiteDestino.TramitesEspecialidades;


                    tramiteDestino.TramitesInfEmp = _mapperCopiarTramite.Map<TramitesInfEmp>(empresaEntity.EmpresasInf.FirstOrDefault());
                    tramiteDestino.TramitesInfEmp.CreateDate = DateTime.Now;
                    tramiteDestino.TramitesInfEmp.CreateUser = userid;
                    tramiteDestino.TramitesInfEmp.TramitesInfEmpDeudas.ToList().ForEach(e =>
                    {
                        e.CreateDate = DateTime.Now;
                        e.CreateUser = userid;
                    });
                    tramiteDestino.TramitesInfEmp.TramitesInfEmpDocumentos.ToList().ForEach(e =>
                    {
                        e.CreateDate = DateTime.Now;
                        e.CreateUser = userid;
                    });

                    //// Copia Datos de la empresa
                    //tramiteDestino.TramitesInfEmp = new TramitesInfEmp
                    //{
                    //    AniosAntiguedadEmpresa = empresaEntity.AniosAntiguedad.GetValueOrDefault(),
                    //    CreateDate = DateTime.Now,
                    //    CreateUser = userid,
                    //    TramitesInfEmpDocumentos = empresaEntity.EmpresasInfDocumentos.Select(s => new TramitesInfEmpDocumentos
                    //    {
                    //        IdTipoDocumento = s.IdTipoDocumento,
                    //        IdFile = s.IdFile,
                    //        Filename = s.Filename,
                    //        CreateDate = DateTime.Now,
                    //        CreateUser = userid,
                    //    }).ToList(),
                    //    TramitesInfEmpDeudas = empresaEntity.EmpresasDeudas.Select(s => new TramitesInfEmpDeudas
                    //    {
                    //        Entidad = s.Entidad,
                    //        Periodo = s.Periodo,
                    //        Monto = s.Monto,
                    //        DiasDeAtraso = s.DiasDeAtraso,
                    //        CreateDate = DateTime.Now,
                    //        CreateUser = userid
                    //    }).ToList(),
                    //};

                    tramiteDestino = await repoTramites.UpdateAsync(tramiteDestino);

                    //Copiar Balance General
                    tramiteDestino.TramitesBalancesGenerales = _mapperCopiarTramite.Map<List<TramitesBalancesGenerales>>(empresaEntity.EmpresasBalancesGenerales);
                    foreach (var item in tramiteDestino.TramitesBalancesGenerales)
                    {
                        item.CreateDate = DateTime.Now;
                        item.CreateUser = userid;
                        item.TramitesBalancesGeneralesEvaluar.CreateDate = DateTime.Now;
                        item.TramitesBalancesGeneralesEvaluar.CreateUser = userid;
                    }
                    tramiteDestino = await repoTramites.UpdateAsync(tramiteDestino);

                    //Copiar los equipos
                    tramiteDestino.TramitesEquipos = _mapperCopiarTramite.Map<List<TramitesEquipos>>(empresaEntity.EmpresasEquipos);
                    tramiteDestino.TramitesEquipos.ToList().ForEach(e =>
                    {
                        e.CreateDate = DateTime.Now;
                        e.CreateUser = userid;
                    });
                    tramiteDestino = await repoTramites.UpdateAsync(tramiteDestino);

                    //Copiar los Inmuebles
                    tramiteDestino.TramitesBienesRaices = _mapperCopiarTramite.Map<List<TramitesBienesRaices>>(empresaEntity.EmpresasBienesRaices);
                    tramiteDestino.TramitesBienesRaices.ToList().ForEach(e =>
                    {
                        e.CreateDate = DateTime.Now;
                        e.CreateUser = userid;
                    });
                    tramiteDestino = await repoTramites.UpdateAsync(tramiteDestino);


                    //Copiar Representantes Técnicos cuyo contrato no esté vencido
                    if (empresaEntity != null && empresaEntity.EmpresasRepresentantesTecnicos.Any(x => x.FechaVencimientoContrato >= DateTime.Today))
                    {
                        tramiteDestino.TramitesRepresentantesTecnicos = empresaEntity.EmpresasRepresentantesTecnicos.Where(x=> x.FechaVencimientoContrato >= DateTime.Today)
                            .Select(s => new TramitesRepresentantesTecnicos
                            {
                                Apellido = s.Apellido,
                                Nombres = s.Nombres,
                                Cuit = s.Cuit,
                                Cargo = s.Cargo,
                                Matricula = s.Matricula,
                                FechaVencimientoMatricula = s.FechaVencimientoMatricula,
                                FechaVencimientoContrato = s.FechaVencimientoContrato,
                                IdFileContrato = s.IdFileContrato,
                                FilenameContrato = s.FilenameContrato,
                                IdFileBoleta = s.IdFileBoleta,
                                FilenameBoleta = s.FilenameBoleta,
                                IdFileMatricula = s.IdFileMatricula,
                                FilenameMatricula = s.FilenameMatricula,
                                CreateDate = DateTime.Now,
                                CreateUser = userid,
                                TramitesRepresentantesTecnicosEspecialidades = s.EmpresasRepresentantesTecnicosEspecialidades
                                    .Select(se => new TramitesRepresentantesTecnicosEspecialidades
                                    {
                                        IdTramiteEspecialidad = Especialidades.First(x => x.IdEspecialidad == se.IdEmpresaEspecialidadNavigation.IdEspecialidad).IdTramiteEspecialidad,
                                    }).ToList(),
                                TramitesRepresentantesTecnicosJurisdicciones = s.EmpresasRepresentantesTecnicosJurisdicciones
                                    .Select(sj => new TramitesRepresentantesTecnicosJurisdicciones
                                    {
                                        IdProvincia = sj.IdProvincia,
                                    }).ToList(),
                            }).ToList();
                    }

                    tramiteDestino = await repoTramites.UpdateAsync(tramiteDestino);


                    //Copiar Obras
                    tramiteDestino.TramitesObras = empresaEntity.EmpresasObras.Select(obra =>
                    {
                        var tramiteObra = _mapperCopiarTramite.Map<TramitesObras>(obra);

                        // Asignar manualmente las FK buscando en el contexto del trámite
                        tramiteObra.IdTramiteEspecialidad = Especialidades
                            .First(x => x.IdEspecialidad == obra.IdEmpresaEspecialidadNavigation.IdEspecialidad)
                            .IdTramiteEspecialidad;

                        tramiteObra.IdTramiteEspecialidadSeccion = Especialidades
                            .SelectMany(sm => sm.TramitesEspecialidadesSecciones)
                            .First(x => x.IdSeccion == obra.IdEmpresaEspecialidadSeccionNavigation.IdSeccion)
                            .IdTramiteEspecialidadSeccion;

                        tramiteObra.CreateDate = DateTime.Now;
                        tramiteObra.CreateUser = userid;

                        return tramiteObra;
                    }).ToList();

                    tramiteDestino = await repoTramites.UpdateAsync(tramiteDestino);


                    //Copiar los Antecedentes de producción desde la Empresa
                    tramiteDestino.TramitesAntecedentes = empresaEntity.EmpresasAntecedentes.Select(antecedente =>
                    {
                        var tramiteAntecedente = _mapperCopiarTramite.Map<TramitesAntecedentes>(antecedente);

                        // Asignar manualmente las FK buscando en el contexto del trámite
                        tramiteAntecedente.IdTramiteEspecialidad = Especialidades
                            .First(x => x.IdEspecialidad == antecedente.IdEmpresaEspecialidadNavigation.IdEspecialidad)
                            .IdTramiteEspecialidad;

                        tramiteAntecedente.IdTramiteEspecialidadSeccion = Especialidades
                            .SelectMany(sm => sm.TramitesEspecialidadesSecciones)
                            .First(x => x.IdSeccion == antecedente.IdEmpresaEspecialidadSeccionNavigation.IdSeccion)
                            .IdTramiteEspecialidadSeccion;

                        // Asignar campos de auditoría
                        tramiteAntecedente.CreateDate = DateTime.Now;
                        tramiteAntecedente.CreateUser = userid;

                        return tramiteAntecedente;
                    }).ToList();

                    tramiteDestino = await repoTramites.UpdateAsync(tramiteDestino);


                    //Copiar las Obras en Ejecución
                    tramiteDestino.TramitesObrasEjecucion = _mapperCopiarTramite.Map<List<TramitesObrasEjecucion>>(empresaEntity.EmpresasObrasEjecucion);
                    tramiteDestino.TramitesObrasEjecucion.ToList().ForEach(e =>
                    {
                        e.CreateDate = DateTime.Now;
                        e.CreateUser = userid;
                    });
                    
                    tramiteDestino = await repoTramites.UpdateAsync(tramiteDestino);

                    await uow.CommitAsync();

                }
                catch (Exception ex)
                {
                    uow.RollBack();
                    throw new ArgumentException(ex.Message);
                }
            }

            return tramiteDestino;
        }
        public async Task ActualizarTramiteAsync(TramitesDTO tramite)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var _repoTramites = new TramitesRepository(uow);

            var entity = await _repoTramites.FirstOrDefaultAsync(x => x.IdTramite == tramite.IdTramite);
            _mapper.Map<TramitesDTO, Tramites>(tramite, entity);
            entity.LastUpdateUser = await _usuarioBL.GetCurrentUserid();
            entity.LastUpdateDate = DateTime.Now;

            await _repoTramites.UpdateAsync(entity);
            
        }
        public async Task ActualizarTramiteSinDatosFormAsync(int IdTramite, int NroFormulario , bool value)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var _repoTramites = new TramitesRepository(uow);
            await _repoTramites.ActualizarSinDatosForm(IdTramite,NroFormulario, value);
        }

        public async Task ActualizarEstadoTramiteAsync(int IdTramite, int IdEstado)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            
            var _repoTramites = new TramitesRepository(uow);
            var _repoFormEvaluados = new TramitesFormulariosEvaluadosRepository(uow);
            var _repoBalances = new TramitesBalancesGeneralesRepository(uow);

            var entity = await _repoTramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);

            entity.IdEstado = IdEstado;


            //Cada vez que se presenta el trámite se copian los datos del tramite anterior
            //Cuando es alguna de las actualizaciones 
            if (IdEstado == Constants.TramitesEstados.EnEvaluacion)
            {
                List<int> FormulariosExceptuadosDeCopia = new();

                if (entity.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionSoloTecnicos)
                    FormulariosExceptuadosDeCopia.Add((int) Constants.Formularios.RepresentantesTecnicos);

                if (entity.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCapacidadTecnica)
                {
                    FormulariosExceptuadosDeCopia.Add((int)Constants.Formularios.RepresentantesTecnicos);
                    FormulariosExceptuadosDeCopia.Add((int)Constants.Formularios.Equipos);
                    FormulariosExceptuadosDeCopia.Add((int)Constants.Formularios.Obras);
                }
                if (entity.IdTipoTramite == Constants.TiposDeTramite.Reli_Licitar)
                {
                    FormulariosExceptuadosDeCopia.Add((int)Constants.Formularios.BoletaPago);
                    FormulariosExceptuadosDeCopia.Add((int)Constants.Formularios.ObrasEnEjecucion);
                }

                await _repoFormEvaluados.CopiarEvaluacionAnteriorAsync(IdTramite,FormulariosExceptuadosDeCopia);
                //solo se copia la primera vez que se presenta el trámite
                await _repoBalances.CopiarDatosEmpresaToEvaluador(IdTramite);
            }

            //Si se aprueba o eleva a la firma se establece la fecha de vencimiento.
            if (IdEstado == Constants.TramitesEstados.ElevadoParaLaFirma ||
                IdEstado == Constants.TramitesEstados.Aprobado)
            {
                var FechaBalance = entity.TramitesBalancesGenerales.Max(m => m.FechaBalance);
                entity.FechaVencimiento = FechaBalance.AddMonths(18);
            }

            await _repoTramites.UpdateAsync(entity);

        }
        public async Task AprobarTramiteAsync(TramitesDTO tramite)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var _repoTramites = new TramitesRepository(uow);
            var _repoEmpresas = new EmpresasRepository(uow);

            var userid = await _usuarioBL.GetCurrentUserid();

            var entity = await _repoTramites.FirstOrDefaultAsync(x => x.IdTramite == tramite.IdTramite);

            using (var tran = await uow.Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Busca la empresa según sea Licitadores o Consultores
                    var entityEmpresa = await _repoEmpresas.FirstOrDefaultAsync(x => x.IdEmpresa == tramite.IdEmpresa);

                    _mapper.Map<TramitesDTO, Tramites>(tramite, entity);
                    entity.IdEstado = Constants.TramitesEstados.Aprobado;

                    entity.LastUpdateUser = userid;
                    entity.LastUpdateDate = DateTime.Now;
                    await _repoTramites.UpdateAsync(entity);

                    bool EsTramiteLicitadores = entity.IdTipoTramiteNavigation.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores;

                    // Copiar toda la estructura del trámite a la empresa
                    await CopiarEstructuraTramiteAEmpresaAsync(
                        uow,
                        entity,
                        entityEmpresa,
                        userid,
                        EsTramiteLicitadores
                    );

                    await _repoEmpresas.UpdateAsync(entityEmpresa);
                    await uow.Context.SaveChangesAsync();

                    await tran.CommitAsync();
                }
                catch (Exception)
                {
                    await tran.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<TramitesDTO> GetTramiteByGuidAsync(string IdentificadorUnico)
        {
            TramitesDTO result = null;
            using var uow = _uowFactory.GetUnitOfWork();
            var _repoTramites = new TramitesRepository(uow);
            int userIdEmpresa = 0;
            int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out userIdEmpresa);

            var tramiteEntity = await _repoTramites.FirstOrDefaultAsync(x => x.IdentificadorUnico == IdentificadorUnico);
            result = _mapper.Map<Tramites, TramitesDTO>(tramiteEntity);

            if (result.IdEmpresa == userIdEmpresa &&
                    (result.IdEstado == Constants.TramitesEstados.EditarInformacion ||
                     result.IdEstado == Constants.TramitesEstados.Observado)
                )
            {
                result.PermiteEditarEmpresa = true;
            }

            return result;
        }
        public async Task<TramitesDTO> GetTramiteByIdAsync(int IdTramite)
        {
            TramitesDTO result = null;
            using var uow = _uowFactory.GetUnitOfWork();
            var _repoTramites = new TramitesRepository(uow);
            int userIdEmpresa = 0;
            int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out userIdEmpresa);

            var tramiteEntity = await _repoTramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            result = _mapper.Map<Tramites, TramitesDTO>(tramiteEntity);

            if (result.IdEmpresa == userIdEmpresa &&
                    (result.IdEstado == Constants.TramitesEstados.EditarInformacion ||
                     result.IdEstado == Constants.TramitesEstados.Observado)
                )
            {
                result.PermiteEditarEmpresa = true;
            }

            return result;
        }
        public async Task<List<ItemGrillaConsultarTramitesDTO>> GetTramitesGrillaByEmpresaAsync(int IdEmpresa)
        {
            List<ItemGrillaConsultarTramitesDTO> result = new List<ItemGrillaConsultarTramitesDTO>();

            using var uow = _uowFactory.GetUnitOfWork();
            var _repoTramites = new TramitesRepository(uow);

            var elements = await _repoTramites.Where(x => x.IdEmpresa == IdEmpresa).ToListAsync();
            result = _mapper.Map<List<Tramites>, List<ItemGrillaConsultarTramitesDTO>>(elements);

            return result;
        }
        public async Task<List<ItemGrillaMisCertificadosDTO>> GetCertificadosGrillaByEmpresaAsync(int IdEmpresa)
        {
            List<ItemGrillaMisCertificadosDTO> result = new List<ItemGrillaMisCertificadosDTO>();

            using var uow = _uowFactory.GetUnitOfWork();
            var _repoTramites = new TramitesRepository(uow);

            var elements = await _repoTramites.Where(x => x.IdEmpresa == IdEmpresa && x.IdFileGedo.HasValue).ToListAsync();
            result = _mapper.Map<List<Tramites>, List<ItemGrillaMisCertificadosDTO>>(elements);

            return result;
        }
        public async Task<List<TramitesDTO>> GetTramitesByEmpresaAsync(int IdEmpresa)
        {
            List<TramitesDTO> result = new List<TramitesDTO>();

            using var uow = _uowFactory.GetUnitOfWork();
            var _repoTramites = new TramitesRepository(uow);

            var elements = await _repoTramites.Where(x => x.IdEmpresa == IdEmpresa).ToListAsync();
            result = _mapper.Map<List<Tramites>, List<TramitesDTO>>(elements);

            return result;
        }

        public async Task<List<ItemGrillaBandejaDTO>> GetTramitesBandejaAsync(string userid)
        {
            List<ItemGrillaBandejaDTO> result = new List<ItemGrillaBandejaDTO>();

            using var uow = _uowFactory.GetUnitOfWork();
            var _repoTramites = new TramitesRepository(uow);

            int[] estados = new int[] { Constants.TramitesEstados.EnEvaluacion ,
                                        Constants.TramitesEstados.ElevadoParaLaFirma };
            var elements = await _repoTramites.Where(x => estados.Contains(x.IdEstado) &&
                                                        (x.UsuarioAsignado == null || x.UsuarioAsignado == userid)).ToListAsync();
            
            result = _mapper.Map<List<Tramites>, List<ItemGrillaBandejaDTO>>(elements);

            return result;
        }
        public async Task<Dictionary<int,bool>> GetFormularioGuardadoAsync(int IdTramite)
        {
            Dictionary<int, bool> result = new Dictionary<int, bool>();
            
            var uof = _uowFactory.GetUnitOfWork();
            var repoTramite = new TramitesRepository(uof);
            var tramite = await repoTramite.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            
            result.Add(0, tramite.IdFileBoleta.HasValue);
            result.Add(1, tramite.TramitesEspecialidades.Count > 0);
            
            if(tramite.IdTipoTramiteNavigation.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores)
                result.Add(2, tramite.TramitesInfEmp != null);
            else if (tramite.IdTipoTramiteNavigation.IdGrupoTramite == Constants.GruposDeTramite.RegistroConsultores)
                result.Add(2, tramite.TramitesInfEmpCons != null);

            result.Add(3, tramite.TramitesInfEmp != null && tramite.TramitesInfEmp.TramitesInfEmpDeudas.Count > 0);
            result.Add(6,tramite.TramitesBalancesGenerales.Count > 0);
            result.Add(7,tramite.TramitesEquipos != null && tramite.TramitesEquipos.Count > 0);
            result.Add(8,tramite.TramitesBienesRaices != null && tramite.TramitesBienesRaices.Count > 0 || tramite.SinDatosForm8);
            result.Add(9, tramite.TramitesRepresentantesTecnicos.Count > 0);
            result.Add(10,(tramite.TramitesObras.Count > 0 || tramite.SinDatosForm10));
            result.Add(11,(tramite.TramitesAntecedentes.Count > 0 || tramite.SinDatosForm11));
            result.Add(12,(tramite.TramitesObrasEjecucion.Count > 0 || tramite.SinDatosForm12));

            return result;
        }
        public async Task<List<TramiteFormularioEvaluadoDTO>> GetFormulariosEvaluadosAsync(int IdTramite, int NroNotificacion)
        {
            List<TramiteFormularioEvaluadoDTO> result;

            var uof = _uowFactory.GetUnitOfWork();
            var repoTramiteFormularios = new TramitesFormulariosEvaluadosRepository(uof);
            var formulariosEvaluados = await repoTramiteFormularios.Where(x => x.IdTramite == IdTramite && x.NroNotificacion == NroNotificacion).ToListAsync();
            result = _mapper.Map<List<TramitesFormulariosEvaluados>, List<TramiteFormularioEvaluadoDTO>>(formulariosEvaluados); 

            return result;
        }
        public async Task<List<TramiteFormularioEvaluadoDTO>> GetFormulariosEvaluadosAsync(int IdTramite)
        {
            List<TramiteFormularioEvaluadoDTO> result;

            var uof = _uowFactory.GetUnitOfWork();
            var repoTramiteFormularios = new TramitesFormulariosEvaluadosRepository(uof);
            var formulariosEvaluados = await repoTramiteFormularios.Where(x => x.IdTramite == IdTramite).ToListAsync();
            result = _mapper.Map<List<TramitesFormulariosEvaluados>, List<TramiteFormularioEvaluadoDTO>>(formulariosEvaluados);

            return result;
        }
        public Task<List<ItemGrillaConsultaTramiteDTO>> GetTramitesAsync(FilterConsultaTramitesDTO filter, int skip, int cantidadRegistros, out int TotalCount )
        {
            List<ItemGrillaConsultaTramiteDTO> result;
            
            using var uow = _uowFactory.GetUnitOfWork();
            var _repoTramites = new TramitesRepository(uow);

            var q = _repoTramites.DbSet.AsQueryable();

            if (filter.IdEstado.HasValue)
                q = q.Where(x => x.IdEstado == filter.IdEstado.Value);

            if (filter.IdTramite.HasValue)
                q = q.Where(x => x.IdTramite == filter.IdTramite.Value);

            if (filter.IdEmpresa.HasValue)
                q = q.Where(x => x.IdEmpresa == filter.IdEmpresa.Value);

            q = q.OrderByDescending(o => o.IdTramite);
            TotalCount = q.Count();
            var elements = q.Skip(skip).Take(cantidadRegistros).ToList();

            result = _mapper.Map<List<Tramites>, List<ItemGrillaConsultaTramiteDTO>>(elements);

            return Task.FromResult(result);
        }
        #endregion

        #region "Especialidades"

        public async Task AgregarEspecialidadAsync(int IdTramite, int IdEspecialidad, int IdSeccion, List<int> lstTareas)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            string userid = await _usuarioBL.GetCurrentUserid();

            var repoEspecialidadesTareas = new EspecialidadesTareasRepository(uow);

            var repoEspecialidadesSecciones = new EspecialidadesSeccionesRepository(uow);
            var repoTramiteEspecialidades = new TramitesEspecialidadesRepository(uow);
            var repoTramiteEspecialidadesSecciones = new TramitesEspecialidadesSeccionesRepository(uow);
            var repoTramiteEspecialidadesTareas = new TramitesEspecialidadesTareasRepository(uow);
            var repoTramiteEspecialidadesEquipos = new TramitesEspecialidadesEquiposRepository(uow);

            var lstTareaEntity = await repoEspecialidadesTareas.Where(x => lstTareas.Contains(x.IdTarea)).ToListAsync();

            //Valida que las tareas que se estan agregando no se encuentrem ya en el trámite
            foreach (var tarea in lstTareaEntity)
            {
                var existe = await repoTramiteEspecialidadesTareas.FirstOrDefaultAsync(x => 
                                                    x.IdTramiteEspecialidadSeccionNavigation.IdTramiteEspecialidadNavigation.IdEspecialidad == IdEspecialidad
                                                    && x.IdTramiteEspecialidadSeccionNavigation.IdTramiteEspecialidadNavigation.IdTramite == IdTramite 
                                                    && x.IdTarea == tarea.IdTarea) != null;
                if (existe)
                    throw new Exception($"La tarea {tarea.DescripcionTarea} ya se encuentra agregada en el trámite.");
            }

            using (var tran = uow.Context.Database.BeginTransaction())
            {
                try
                {
                    var tramiteEspecialidadEntity = await repoTramiteEspecialidades.DbSet.FirstOrDefaultAsync(x => x.IdTramite == IdTramite && x.IdEspecialidad == IdEspecialidad);
                    //Alta de Especialidad
                    if (tramiteEspecialidadEntity == null)
                    {
                        tramiteEspecialidadEntity = new TramitesEspecialidades()
                        {
                            IdTramite = IdTramite,
                            IdEspecialidad = IdEspecialidad,
                            CreateDate = DateTime.Now,
                            CreateUser = userid
                        };
                        tramiteEspecialidadEntity = await repoTramiteEspecialidades.AddAsync(tramiteEspecialidadEntity);
                    }
                    //Alta de Seccion
                    var tramiteEspecialidadSeccionEntity = await repoTramiteEspecialidadesSecciones.DbSet.FirstOrDefaultAsync(x => x.IdTramiteEspecialidad == tramiteEspecialidadEntity.IdTramiteEspecialidad && x.IdSeccion == IdSeccion);
                    //Alta de Especialidad
                    if (tramiteEspecialidadSeccionEntity == null)
                    {
                        tramiteEspecialidadSeccionEntity = new TramitesEspecialidadesSecciones()
                        {
                            IdTramiteEspecialidad = tramiteEspecialidadEntity.IdTramiteEspecialidad,
                            IdSeccion = IdSeccion,
                            CreateDate = DateTime.Now,
                            CreateUser = userid,
                        };
                        tramiteEspecialidadSeccionEntity = await repoTramiteEspecialidadesSecciones.AddAsync(tramiteEspecialidadSeccionEntity);
                    }
                    //Alta de Tareas
                    foreach (var tarea in lstTareaEntity)
                    {
                        
                        var tramiteEspecilidadTareaEntity = new TramitesEspecialidadesTareas()
                        {
                            IdTramiteEspecialidadSeccion = tramiteEspecialidadSeccionEntity.IdTramiteEspecialidadSeccion,
                            IdTarea = tarea.IdTarea,
                        };
                        tramiteEspecilidadTareaEntity = await repoTramiteEspecialidadesTareas.AddAsync(tramiteEspecilidadTareaEntity);

                        var seccionEntity = await repoEspecialidadesSecciones.FirstOrDefaultAsync(x => x.IdSeccion == tarea.IdSeccion);

                        //Alta de Equipos
                        foreach (var equipo in seccionEntity.IdEquipo.Where(x => !x.Baja))
                        {
                            var tramiteEspecialidadEquipoEntity = new TramitesEspecialidadesEquipos()
                            {
                                IdTramiteEspecialidadTarea = tramiteEspecilidadTareaEntity.IdTramiteEspecialidadTarea,
                                IdEquipo = equipo.IdEquipo
                            };
                            await repoTramiteEspecialidadesEquipos.AddAsync(tramiteEspecialidadEquipoEntity);
                        }

                    }
                    await uow.Context.SaveChangesAsync();
                    await tran.CommitAsync();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }

        }

        public async Task ActualizarEspecialidadAsync(int IdTramite, int IdEspecialidad, int IdSeccion, List<int> lstTareas)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            string userid = await _usuarioBL.GetCurrentUserid();

            var repoEspecialidadesTareas = new EspecialidadesTareasRepository(uow);
            var repoTramiteEspecialidadesSecciones = new TramitesEspecialidadesSeccionesRepository(uow);
            var repoTramiteEspecialidadesTareas = new TramitesEspecialidadesTareasRepository(uow);
            var repoTramiteEspecialidadesEquipos = new TramitesEspecialidadesEquiposRepository(uow);
            var repoEspecialidades = new EspecialidadesRepository(uow);

            var especialidadRamaC = await repoEspecialidades.FirstOrDefaultAsync(x => x.Rama == "C");
            int IdEspecialidadRamaC = (especialidadRamaC != null ? especialidadRamaC.IdEspecialidad : 0);


            var lstTareasBorrar = repoTramiteEspecialidadesTareas.Where(x => x.IdTramiteEspecialidadSeccionNavigation.IdTramiteEspecialidadNavigation.IdTramite == IdTramite
                                                                    && x.IdTramiteEspecialidadSeccionNavigation.IdTramiteEspecialidadNavigation.IdEspecialidad == IdEspecialidad
                                                                    && x.IdTramiteEspecialidadSeccionNavigation.IdSeccion == IdSeccion
                                                                    && !lstTareas.Contains(x.IdTarea)).ToList();
            var lstEquiposBorrar = repoTramiteEspecialidadesEquipos.Where(x => lstTareasBorrar.Select(s => s.IdTramiteEspecialidadTarea).Contains(x.IdTramiteEspecialidadTarea)).ToList();

            // Obtencion de datos de la Rama C si la hay
            var lstTareasRamaCBorrar = repoTramiteEspecialidadesTareas.Where(x => x.IdTramiteEspecialidadSeccionNavigation.IdTramiteEspecialidadNavigation.IdTramite == IdTramite
                                                                    && x.IdTramiteEspecialidadSeccionNavigation.IdTramiteEspecialidadNavigation.IdEspecialidad == IdEspecialidadRamaC
                                                                    && x.IdTramiteEspecialidadSeccionNavigation.IdSeccion == IdSeccion
                                                                    && !lstTareas.Contains(x.IdTarea)).ToList();

            var lstEquiposRamaCBorrar = repoTramiteEspecialidadesEquipos.Where(x => lstTareasRamaCBorrar.Select(s => s.IdTramiteEspecialidadTarea).Contains(x.IdTramiteEspecialidadTarea)).ToList();
            var entitySeccionRamaC = await repoTramiteEspecialidadesSecciones.FirstOrDefaultAsync(x => x.IdTramiteEspecialidadNavigation.IdTramite == IdTramite
                        && x.IdTramiteEspecialidadNavigation.IdEspecialidad == IdEspecialidadRamaC
                        && x.IdSeccion == IdSeccion);
            //--

            var lstTareaEntity = await repoEspecialidadesTareas.Where(x => lstTareas.Contains(x.IdTarea)).ToListAsync();
            var lstTareaAgregar = new List<EspecialidadesTareas>();

            var tramiteSeccionEntity = await repoTramiteEspecialidadesSecciones.FirstOrDefaultAsync(x => x.IdTramiteEspecialidadNavigation.IdTramite == IdTramite
                                                                                                    && x.IdTramiteEspecialidadNavigation.IdEspecialidad == IdEspecialidad
                                                                                                    && x.IdSeccion == IdSeccion);

            //Valida que las tareas que se estan agregando no se encuentrem ya en el trámite
            foreach (var tarea in lstTareaEntity)
            {
                var existe = await repoTramiteEspecialidadesTareas.FirstOrDefaultAsync(x => x.IdTramiteEspecialidadSeccionNavigation.IdTramiteEspecialidadNavigation.IdTramite == IdTramite
                                                                                        && x.IdTramiteEspecialidadSeccionNavigation.IdTramiteEspecialidadNavigation.IdEspecialidad == IdEspecialidad
                                                                                        && x.IdTramiteEspecialidadSeccionNavigation.IdSeccion == IdSeccion
                                                                                        && x.IdTarea == tarea.IdTarea) != null;
                if (!existe)
                    lstTareaAgregar.Add(tarea);
            }

            using (var uoww = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoTramiteEspecialidadesSeccionesW = new TramitesEspecialidadesSeccionesRepository(uoww);
                    var repoTramiteEspecialidadesW = new TramitesEspecialidadesRepository(uoww);
                    var repoEspecialidadesSecciones = new EspecialidadesSeccionesRepository(uoww);
                    var repoTramiteEspecialidades = new TramitesEspecialidadesRepository(uoww);
                    var repoTramitesRepresentantesTecnicosEspecialidades = new TramitesRepresentantesTecnicosEspecialidadesRepository(uoww);
                    var repoTramitesObras = new TramitesObrasRepository(uoww);
                    var repoTramitesAntecedentes = new TramitesAntecedentesRepository(uoww);


                    //Elimina los equipos relacionados a las tareas que se van a borrar
                    repoTramiteEspecialidadesEquipos = new TramitesEspecialidadesEquiposRepository(uoww);
                    await repoTramiteEspecialidadesEquipos.RemoveRangeAsync(lstEquiposBorrar);
                    if(IdEspecialidad != IdEspecialidadRamaC)
                        await repoTramiteEspecialidadesEquipos.RemoveRangeAsync(lstEquiposRamaCBorrar);

                    //Elimina las tareas que no estaban en la lista de checks
                    repoTramiteEspecialidadesTareas = new TramitesEspecialidadesTareasRepository(uoww);
                    await repoTramiteEspecialidadesTareas.RemoveRangeAsync(lstTareasBorrar);
                    if (IdEspecialidad != IdEspecialidadRamaC)
                        await repoTramiteEspecialidadesTareas.RemoveRangeAsync(lstTareasRamaCBorrar);

                    //Si todas las tareas de la rama C se eliminaron se elimina la seccion
                    if (entitySeccionRamaC != null && IdEspecialidad != IdEspecialidadRamaC)
                    {
                        var entityEspecialidadRamaC = entitySeccionRamaC.IdTramiteEspecialidadNavigation;

                        if (entitySeccionRamaC.TramitesEspecialidadesTareas.Count == 0)
                        {
                            if (repoTramitesObras.DbSet.Count(x => x.IdTramiteEspecialidadSeccion == entitySeccionRamaC.IdTramiteEspecialidadSeccion) > 0)
                                throw new Exception($"No se puede eliminar la seccion {entitySeccionRamaC.IdSeccionNavigation.DescripcionSeccion}" +
                                    $" de la especialidad {entityEspecialidadRamaC.IdEspecialidadNavigation.NombreEspecialidad} " +
                                   $", debido a que hay Obras asociadas a la misma. Debe eliminar primero las Obras que utilicen esta seccion.");

                            if (repoTramitesAntecedentes.DbSet.Count(x => x.IdTramiteEspecialidadSeccion == entitySeccionRamaC.IdTramiteEspecialidadSeccion) > 0)
                                throw new Exception($"No se puede eliminar la seccion {entitySeccionRamaC.IdSeccionNavigation.DescripcionSeccion}" +
                                    $" de la especialidad {entityEspecialidadRamaC.IdEspecialidadNavigation.NombreEspecialidad} " +
                                    $", debido a que hay Antecedentes de Producción asociados a la misma. Debe eliminar primero los Antecedentes de Producción que utilicen esta seccion.");
                            
                            await repoTramiteEspecialidadesSeccionesW.RemoveAsync(entitySeccionRamaC);
                        }


                        //Si todas las secciones de la Rama C se eliminaron se elimina la especialidad.
                        if (entityEspecialidadRamaC.TramitesEspecialidadesSecciones == null || entityEspecialidadRamaC.TramitesEspecialidadesSecciones.Count == 0)
                        {
                            if (repoTramitesRepresentantesTecnicosEspecialidades.DbSet.Count(x => x.IdTramiteEspecialidad == entityEspecialidadRamaC.IdTramiteEspecialidad) > 0)
                                throw new Exception($"No se puede eliminar la especialidad {entityEspecialidadRamaC.IdEspecialidadNavigation.NombreEspecialidad}" +
                                    $", debido a que hay representantes técnicos asociados a la misma. Debe eliminar primero los Representantes Técnicos que utilicen esta especialidad.");

                            if (repoTramitesObras.DbSet.Count(x => x.IdTramiteEspecialidad == entityEspecialidadRamaC.IdTramiteEspecialidad) > 0)
                                throw new Exception($"No se puede eliminar la especialidad {entityEspecialidadRamaC.IdEspecialidadNavigation.NombreEspecialidad}" +
                                    $", debido a que hay Obras asociadas a la misma. Debe eliminar primero las Obras que utilicen esta especialidad.");

                            if (repoTramitesAntecedentes.DbSet.Count(x => x.IdTramiteEspecialidad == entityEspecialidadRamaC.IdTramiteEspecialidad) > 0)
                                throw new Exception($"No se puede eliminar la especialidad {entityEspecialidadRamaC.IdEspecialidadNavigation.NombreEspecialidad}" +
                                    $", debido a que hay Antecedentes de producción asociados a la misma. Debe eliminar primero los Antecedentes de Producción que utilicen esta especialidad.");

                            await repoTramiteEspecialidadesW.RemoveAsync(entityEspecialidadRamaC);
                        }
                    }

                    var tramiteEspecialidadEntity = await repoTramiteEspecialidades.DbSet.FirstOrDefaultAsync(x => x.IdTramite == IdTramite && x.IdEspecialidad == IdEspecialidad);

                    //Alta de Tareas
                    foreach (var tarea in lstTareaAgregar)
                    {
                        var seccionEntity = await repoEspecialidadesSecciones.FirstOrDefaultAsync(x => x.IdSeccion == tarea.IdSeccion);

                        var tramiteEspecilidadTareaEntity = new TramitesEspecialidadesTareas()
                        {
                            IdTramiteEspecialidadSeccion = tramiteSeccionEntity.IdTramiteEspecialidadSeccion,
                            IdTarea = tarea.IdTarea,
                        };
                        tramiteEspecilidadTareaEntity = await repoTramiteEspecialidadesTareas.AddAsync(tramiteEspecilidadTareaEntity);

                        //Alta de Equipos
                        foreach (var equipo in seccionEntity.IdEquipo)
                        {
                            var tramiteEspecialidadEquipoEntity = new TramitesEspecialidadesEquipos()
                            {
                                IdTramiteEspecialidadTarea = tramiteEspecilidadTareaEntity.IdTramiteEspecialidadTarea,
                                IdEquipo = equipo.IdEquipo
                            };
                            await repoTramiteEspecialidadesEquipos.AddAsync(tramiteEspecialidadEquipoEntity);
                        }

                    }
                    await uoww.CommitAsync();
                }
                catch (Exception)
                {
                    uoww.RollBack();
                    throw;
                }
            }

        }
        public async Task ActualizarEspecialidadesRecoAsync(int IdTramite, List<EspecialidadSeccionDTO> lstEspecialidadesSecciones)
        {
            // Actualiza la lista de Especialidades y Secciones de un trámite de Registro de Consultores
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                var repoTramiteEspecialidades = new TramitesEspecialidadesRepository(uow);
                var repoTramiteEspecialidadesSecciones = new TramitesEspecialidadesSeccionesRepository(uow);
                var repoTramitesRepresentantesTecnicosEspecialidades = new TramitesRepresentantesTecnicosEspecialidadesRepository(uow);
                var repoTramitesObras = new TramitesObrasRepository(uow);
                var repoTramitesAntecedentes = new TramitesAntecedentesRepository(uow);
                var repoEspecialidadesSecciones = new EspecialidadesSeccionesRepository(uow);


                try
                {

                    // Especialidades + secciones que vienen desde la UI
                    var especialidadesDto = lstEspecialidadesSecciones
                        .GroupBy(s => s.IdEspecialidad)
                        .Select(g => new
                        {
                            IdEspecialidad = g.Key,
                            Secciones = g.Select(x => x.IdSeccion).Distinct().ToList()
                        })
                        .ToList();

                    // Especialidades y secciones actuales en el trámite
                    var especialidadesActuales = await repoTramiteEspecialidades
                        .Where(x => x.IdTramite == IdTramite)
                        .Include(x => x.TramitesEspecialidadesSecciones)
                        .ThenInclude(s => s.TramitesEspecialidadesTareas)
                        .ThenInclude(t => t.TramitesEspecialidadesEquipos)
                        .ToListAsync();

                    // 1) Eliminar especialidades/secciones que ya no estén en el DTO
                    foreach (var espActual in especialidadesActuales.ToList())
                    {
                        var dtoEsp = especialidadesDto.FirstOrDefault(e => e.IdEspecialidad == espActual.IdEspecialidad);

                        if (dtoEsp == null)
                        {
                            // Se quiere eliminar toda la especialidad: validar dependencias
                            if (repoTramitesRepresentantesTecnicosEspecialidades.DbSet.Any(x => x.IdTramiteEspecialidad == espActual.IdTramiteEspecialidad))
                            {
                                throw new Exception(
                                    $"No se puede eliminar la especialidad {espActual.IdEspecialidadNavigation.NombreEspecialidad}" +
                                    $", debido a que hay representantes técnicos asociados a la misma. Debe eliminar primero los Representantes Técnicos que utilicen esta especialidad.");
                            }

                            if (repoTramitesObras.DbSet.Any(x => x.IdTramiteEspecialidad == espActual.IdTramiteEspecialidad))
                            {
                                throw new Exception(
                                    $"No se puede eliminar la especialidad {espActual.IdEspecialidadNavigation.NombreEspecialidad}" +
                                    $", debido a que hay Obras asociadas a la misma. Debe eliminar primero las Obras que utilicen esta especialidad.");
                            }

                            if (repoTramitesAntecedentes.DbSet.Any(x => x.IdTramiteEspecialidad == espActual.IdTramiteEspecialidad))
                            {
                                throw new Exception(
                                    $"No se puede eliminar la especialidad {espActual.IdEspecialidadNavigation.NombreEspecialidad}" +
                                    $", debido a que hay Antecedentes de producción asociados a la misma. Debe eliminar primero los Antecedentes de Producción que utilicen esta especialidad.");
                            }

                            // Borrado físico de secciones/tareas/equipos
                            foreach (var seccion in espActual.TramitesEspecialidadesSecciones.ToList())
                            {
                                foreach (var tarea in seccion.TramitesEspecialidadesTareas.ToList())
                                {
                                    await new TramitesEspecialidadesEquiposRepository(uow)
                                        .RemoveRangeAsync(tarea.TramitesEspecialidadesEquipos);
                                }

                                await new TramitesEspecialidadesTareasRepository(uow)
                                    .RemoveRangeAsync(seccion.TramitesEspecialidadesTareas);

                                if (repoTramitesObras.DbSet.Any(x => x.IdTramiteEspecialidadSeccion == seccion.IdTramiteEspecialidadSeccion))
                                {
                                    throw new Exception(
                                        $"No se puede eliminar la seccion {seccion.IdSeccionNavigation.DescripcionSeccion}" +
                                        $" de la especialidad {espActual.IdEspecialidadNavigation.NombreEspecialidad} " +
                                        $", debido a que hay Obras asociadas a la misma. Debe eliminar primero las Obras que utilicen esta seccion.");
                                }

                                if (repoTramitesAntecedentes.DbSet.Any(x => x.IdTramiteEspecialidadSeccion == seccion.IdTramiteEspecialidadSeccion))
                                {
                                    throw new Exception(
                                        $"No se puede eliminar la seccion {seccion.IdSeccionNavigation.DescripcionSeccion}" +
                                        $" de la especialidad {espActual.IdEspecialidadNavigation.NombreEspecialidad} " +
                                        $", debido a que hay Antecedentes de Producción asociados a la misma. Debe eliminar primero los Antecedentes de Producción que utilicen esta seccion.");
                                }

                                await repoTramiteEspecialidadesSecciones.RemoveAsync(seccion);
                            }

                            await repoTramiteEspecialidades.RemoveAsync(espActual);
                            continue;
                        }

                        // Eliminar secciones que ya no vengan en el DTO para esa especialidad
                        foreach (var seccion in espActual.TramitesEspecialidadesSecciones.ToList())
                        {
                            if (!dtoEsp.Secciones.Contains(seccion.IdSeccion))
                            {
                                // Validaciones de dependencia
                                if (repoTramitesObras.DbSet.Any(x => x.IdTramiteEspecialidadSeccion == seccion.IdTramiteEspecialidadSeccion))
                                {
                                    throw new Exception(
                                        $"No se puede eliminar la seccion {seccion.IdSeccionNavigation.DescripcionSeccion}" +
                                        $" de la especialidad {espActual.IdEspecialidadNavigation.NombreEspecialidad} " +
                                        $", debido a que hay Obras asociadas a la misma. Debe eliminar primero las Obras que utilicen esta seccion.");
                                }

                                if (repoTramitesAntecedentes.DbSet.Any(x => x.IdTramiteEspecialidadSeccion == seccion.IdTramiteEspecialidadSeccion))
                                {
                                    throw new Exception(
                                        $"No se puede eliminar la seccion {seccion.IdSeccionNavigation.DescripcionSeccion}" +
                                        $" de la especialidad {espActual.IdEspecialidadNavigation.NombreEspecialidad} " +
                                        $", debido a que hay Antecedentes de Producción asociados a la misma. Debe eliminar primero los Antecedentes de Producción que utilicen esta seccion.");
                                }

                                // Eliminar tareas y equipos
                                foreach (var tarea in seccion.TramitesEspecialidadesTareas.ToList())
                                {
                                    await new TramitesEspecialidadesEquiposRepository(uow)
                                        .RemoveRangeAsync(tarea.TramitesEspecialidadesEquipos);
                                }

                                await new TramitesEspecialidadesTareasRepository(uow)
                                    .RemoveRangeAsync(seccion.TramitesEspecialidadesTareas);

                                await repoTramiteEspecialidadesSecciones.RemoveAsync(seccion);
                            }
                        }
                    }

                    // 2) Agregar/crear especialidades y secciones faltantes
                    foreach (var dtoEsp in especialidadesDto)
                    {
                        var espActual = especialidadesActuales
                            .FirstOrDefault(e => e.IdEspecialidad == dtoEsp.IdEspecialidad);

                        if (espActual == null)
                        {
                            // Nueva especialidad
                            espActual = new TramitesEspecialidades
                            {
                                IdTramite = IdTramite,
                                IdEspecialidad = dtoEsp.IdEspecialidad,
                                CreateDate = DateTime.Now,
                                CreateUser = userid
                            };
                            espActual = await repoTramiteEspecialidades.AddAsync(espActual);
                            especialidadesActuales.Add(espActual);
                        }

                        // Secciones nuevas para esta especialidad
                        foreach (var idSeccion in dtoEsp.Secciones)
                        {
                            var seccionActual = espActual.TramitesEspecialidadesSecciones
                                .FirstOrDefault(s => s.IdSeccion == idSeccion);

                            if (seccionActual == null)
                            {
                                seccionActual = new TramitesEspecialidadesSecciones
                                {
                                    IdTramiteEspecialidad = espActual.IdTramiteEspecialidad,
                                    IdSeccion = idSeccion,
                                    CreateDate = DateTime.Now,
                                    CreateUser = userid
                                };
                                seccionActual = await repoTramiteEspecialidadesSecciones.AddAsync(seccionActual);
                                espActual.TramitesEspecialidadesSecciones.Add(seccionActual);
                            }

                            // Para Consultores no estás manejando tareas/equipos aquí, así que se deja sólo la creación de secciones
                        }
                    }

                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }

        public async Task<List<ItemGrillaEspecialidadesDTO>> GetEspecialidadesAsync(int IdTramite)
        {
            List<ItemGrillaEspecialidadesDTO> result = new List<ItemGrillaEspecialidadesDTO>();

            using var uow = _uowFactory.GetUnitOfWork();
            var repoTramitesEspecialidades = new TramitesEspecialidadesRepository(uow);

            var lstTramiteEspecialidades = await repoTramitesEspecialidades.Where(x => x.IdTramite == IdTramite).ToListAsync();
            result = _mapper.Map<List<TramitesEspecialidades>, List<ItemGrillaEspecialidadesDTO>>(lstTramiteEspecialidades);

            return result;
        }
        
        public async Task<ItemGrillaEspecialidadesDTO> GetEspecialidadAsync(int IdTramiteEspecialidad)
        {
            ItemGrillaEspecialidadesDTO result = null;

            using var uow = _uowFactory.GetUnitOfWork();
            var repoTramitesEspecialidades = new TramitesEspecialidadesRepository(uow);

            var entity = await repoTramitesEspecialidades.FirstOrDefaultAsync(x => x.IdTramiteEspecialidad == IdTramiteEspecialidad);
            result = _mapper.Map<TramitesEspecialidades, ItemGrillaEspecialidadesDTO>(entity);

            return result;
        }

        
        public async Task EliminarEspecialidadSeccionAsync(int IdTramiteEspecialidadSeccion)
        {

        
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                var repoTramitesEspecialidades = new TramitesEspecialidadesRepository(uow);
                var repoTramitesEspecialidadesSecciones = new TramitesEspecialidadesSeccionesRepository(uow);
                var repoTramitesEspecialidadesTareas = new TramitesEspecialidadesTareasRepository(uow);
                var repoTramitesEspecialidadesEquipos = new TramitesEspecialidadesEquiposRepository(uow);
                var repoTramitesRepresentantesTecnicosEspecialidades = new TramitesRepresentantesTecnicosEspecialidadesRepository(uow);
                var repoEspecialidades = new EspecialidadesRepository(uow);
                var repoTramitesObras = new TramitesObrasRepository(uow);
                var repoTramitesAntecedentes = new TramitesAntecedentesRepository(uow);


                var especialidadRamaC = await repoEspecialidades.FirstOrDefaultAsync(x => x.Rama == "C");
                int IdEspecialidadRamaC = (especialidadRamaC  != null ? especialidadRamaC.IdEspecialidad : 0);
                
                

                var entitySeccion = await repoTramitesEspecialidadesSecciones.FirstOrDefaultAsync(x => x.IdTramiteEspecialidadSeccion == IdTramiteEspecialidadSeccion);
                var entitySeccionRamaC = await repoTramitesEspecialidadesSecciones
                                                .FirstOrDefaultAsync(x => x.IdTramiteEspecialidadNavigation.IdTramite == entitySeccion.IdTramiteEspecialidadNavigation.IdTramite
                                                && x.IdTramiteEspecialidadNavigation.IdEspecialidad == IdEspecialidadRamaC
                                                && x.IdSeccion == entitySeccion.IdSeccion);
                // Cuando es una seccion de la Rama A 0 B y el tramite tiene cargado una Rama C con la misma seccion 
                // entonces primero borra la rama C de la seccion que se está eliminando y luegi borra la seccion que se está eliminando.
                if(entitySeccionRamaC != null)
                {
                    var entityEspecialidadRamaC = entitySeccionRamaC.IdTramiteEspecialidadNavigation;
                    foreach (var tarea in entitySeccionRamaC.TramitesEspecialidadesTareas)
                    {
                        await repoTramitesEspecialidadesEquipos.RemoveRangeAsync(tarea.TramitesEspecialidadesEquipos);
                    }
                    await repoTramitesEspecialidadesTareas.RemoveRangeAsync(entitySeccionRamaC.TramitesEspecialidadesTareas);
                    
                    if (repoTramitesObras.DbSet.Count(x => x.IdTramiteEspecialidadSeccion == entitySeccionRamaC.IdTramiteEspecialidadSeccion) > 0)
                        throw new Exception($"No se puede eliminar la seccion {entitySeccionRamaC.IdSeccionNavigation.DescripcionSeccion}"  +
                            $" de la especialidad {entityEspecialidadRamaC.IdEspecialidadNavigation.NombreEspecialidad} " +
                            $", debido a que hay Obras asociadas a la misma. Debe eliminar primero las Obras que utilicen esta seccion.");

                    if (repoTramitesAntecedentes.DbSet.Count(x => x.IdTramiteEspecialidadSeccion == entitySeccionRamaC.IdTramiteEspecialidadSeccion) > 0)
                        throw new Exception($"No se puede eliminar la seccion {entitySeccionRamaC.IdSeccionNavigation.DescripcionSeccion}" +
                            $" de la especialidad {entityEspecialidadRamaC.IdEspecialidadNavigation.NombreEspecialidad} " +
                            $", debido a que hay Antecedentes de Producción asociados a la misma. Debe eliminar primero los Antecedentes de Producción que utilicen esta seccion.");

                    await repoTramitesEspecialidadesSecciones.RemoveAsync(entitySeccionRamaC);

                    if (entityEspecialidadRamaC.TramitesEspecialidadesSecciones == null || entityEspecialidadRamaC.TramitesEspecialidadesSecciones.Count == 0)
                    {
                        if (repoTramitesRepresentantesTecnicosEspecialidades.DbSet.Count(x => x.IdTramiteEspecialidad == entityEspecialidadRamaC.IdTramiteEspecialidad) > 0)
                            throw new Exception($"No se puede eliminar la especialidad {entityEspecialidadRamaC.IdEspecialidadNavigation.NombreEspecialidad}" +
                                $", debido a que hay representantes técnicos asociados a la misma. Debe eliminar primero los Representantes Técnicos que utilicen esta especialidad.");

                        if (repoTramitesObras.DbSet.Count(x => x.IdTramiteEspecialidad == entityEspecialidadRamaC.IdTramiteEspecialidad) > 0)
                            throw new Exception($"No se puede eliminar la especialidad {entityEspecialidadRamaC.IdEspecialidadNavigation.NombreEspecialidad}" +
                                $", debido a que hay Obras asociadas a la misma. Debe eliminar primero las Obras que utilicen esta especialidad.");

                        if (repoTramitesAntecedentes.DbSet.Count(x => x.IdTramiteEspecialidad == entityEspecialidadRamaC.IdTramiteEspecialidad) > 0)
                            throw new Exception($"No se puede eliminar la especialidad {entityEspecialidadRamaC.IdEspecialidadNavigation.NombreEspecialidad}" +
                                $", debido a que hay Antecedentes de producción asociados a la misma. Debe eliminar primero los Antecedentes de Producción que utilicen esta especialidad.");

                        await repoTramitesEspecialidades.RemoveAsync(entityEspecialidadRamaC);
                    }
                }
                
                entitySeccion = await repoTramitesEspecialidadesSecciones.FirstOrDefaultAsync(x => x.IdTramiteEspecialidadSeccion == IdTramiteEspecialidadSeccion);
                if (entitySeccion != null)  // Es Nulo cuando la seccion que se esta eliminando es justo de la RAMA C
                {
                    var entityEspecialidad = entitySeccion.IdTramiteEspecialidadNavigation;

                    foreach (var tarea in entitySeccion.TramitesEspecialidadesTareas)
                    {
                        await repoTramitesEspecialidadesEquipos.RemoveRangeAsync(tarea.TramitesEspecialidadesEquipos);
                    }
                    await repoTramitesEspecialidadesTareas.RemoveRangeAsync(entitySeccion.TramitesEspecialidadesTareas);

                    if (repoTramitesObras.DbSet.Count(x => x.IdTramiteEspecialidadSeccion == entitySeccion.IdTramiteEspecialidadSeccion) > 0)
                        throw new Exception($"No se puede eliminar la seccion {entitySeccion.IdSeccionNavigation.DescripcionSeccion}" +
                            $" de la especialidad {entitySeccion.IdSeccionNavigation.IdEspecialidadNavigation.NombreEspecialidad} " +
                            $", debido a que hay Obras asociadas a la misma. Debe eliminar primero las Obras que utilicen esta seccion.");

                    if (repoTramitesAntecedentes.DbSet.Count(x => x.IdTramiteEspecialidadSeccion == entitySeccion.IdTramiteEspecialidadSeccion) > 0)
                        throw new Exception($"No se puede eliminar la seccion {entitySeccion.IdSeccionNavigation.DescripcionSeccion}" +
                            $" de la especialidad {entitySeccion.IdSeccionNavigation.IdEspecialidadNavigation.NombreEspecialidad} " +
                            $", debido a que hay Antecedentes de Producción asociados a la misma. Debe eliminar primero los Antecedentes de Producción que utilicen esta seccion.");

                    await repoTramitesEspecialidadesSecciones.RemoveAsync(entitySeccion);
                    

                    if (entityEspecialidad.TramitesEspecialidadesSecciones == null || entityEspecialidad.TramitesEspecialidadesSecciones.Count == 0)
                    {
                        if (repoTramitesRepresentantesTecnicosEspecialidades.DbSet.Count(x => x.IdTramiteEspecialidad == entityEspecialidad.IdTramiteEspecialidad) > 0)
                            throw new Exception($"No se puede eliminar la especialidad {entityEspecialidad.IdEspecialidadNavigation.NombreEspecialidad}" +
                                $", debido a que hay representantes técnicos asociados a la misma. Debe eliminar primero los Representantes Técnicos que utilicen esta especialidad.");

                        if(repoTramitesObras.DbSet.Count(x=> x.IdTramiteEspecialidad == entityEspecialidad.IdTramiteEspecialidad) > 0)
                            throw new Exception($"No se puede eliminar la especialidad {entityEspecialidad.IdEspecialidadNavigation.NombreEspecialidad}" +
                                $", debido a que hay Obras asociadas a la misma. Debe eliminar primero las Obras que utilicen esta especialidad.");

                        if (repoTramitesAntecedentes.DbSet.Count(x => x.IdTramiteEspecialidad == entityEspecialidad.IdTramiteEspecialidad) > 0)
                            throw new Exception($"No se puede eliminar la especialidad {entityEspecialidad.IdEspecialidadNavigation.NombreEspecialidad}" +
                                $", debido a que hay Antecedentes de producción asociados a la misma. Debe eliminar primero los Antecedentes de Producción que utilicen esta especialidad.");

                        await repoTramitesEspecialidades.RemoveAsync(entityEspecialidad);
                    }

                }
                await uow.Context.SaveChangesAsync();
                await uow.CommitAsync();
            }
        }

        #endregion

        #region "Representantes tecnicos"

        public async Task<List<TramitesRepresentanteTecnicoDTO>> GetRepresentantesAsync(int IdTramite)
        {
            List<TramitesRepresentanteTecnicoDTO> result = new List<TramitesRepresentanteTecnicoDTO>();

            using var uow = _uowFactory.GetUnitOfWork();
            var repoRepresentantesTecnicos = new TramitesRepresentantesTecnicosRepository(uow);

            var lstRepresentantes = await repoRepresentantesTecnicos.Where(x => x.IdTramite == IdTramite).ToListAsync();
            result = _mapper.Map<List<TramitesRepresentantesTecnicos>, List<TramitesRepresentanteTecnicoDTO>>(lstRepresentantes);

            return result;
        }
        public async Task<TramitesRepresentanteTecnicoDTO> GetRepresentanteAsync(int IdRepresentanteTecnico)
        {
            TramitesRepresentanteTecnicoDTO result = null;

            using var uow = _uowFactory.GetUnitOfWork();
            var repoRepresentantesTecnicos = new TramitesRepresentantesTecnicosRepository(uow);

            var entity = await repoRepresentantesTecnicos.FirstOrDefaultAsync(x => x.IdRepresentanteTecnico == IdRepresentanteTecnico);
            result = _mapper.Map<TramitesRepresentantesTecnicos, TramitesRepresentanteTecnicoDTO>(entity);

            return result;
        }

        public async Task AgregarRepresentanteTecnicoAsync(TramitesRepresentanteTecnicoDTO representante)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork( System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoRepresentantesTecnicos = new TramitesRepresentantesTecnicosRepository(uow);
                    var repoRepresentantesTecnicosJurisdicciones = new TramitesRepresentantesTecnicosJurisdiccionesRepository(uow);


                    var representanteEntity = await repoRepresentantesTecnicos.FirstOrDefaultAsync(x => x.IdTramite == representante.IdTramite 
                                            && x.Cuit == representante.CUIT);

                    if (representanteEntity != null)
                        throw new Exception($"El representante técnico con CUIT {representante.CUIT} ya se encuentra agregado en el trámite.");

                    representanteEntity = _mapper.Map<TramitesRepresentanteTecnicoDTO, TramitesRepresentantesTecnicos>(representante);
                    representanteEntity.TramitesRepresentantesTecnicosJurisdicciones = _mapper.Map<List<TramitesRepresentanteTecnicoJurisdiccionDTO>, List<TramitesRepresentantesTecnicosJurisdicciones>>(representante.TramitesRepresentantesTecnicosJurisdicciones);
                    representanteEntity.CreateDate = DateTime.Now;
                    representanteEntity.CreateUser = userid;
                    await repoRepresentantesTecnicos.AddAsync(representanteEntity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarRepresentanteTecnicoAsync(TramitesRepresentanteTecnicoDTO representante)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repoRepresentantesTecnicos = new TramitesRepresentantesTecnicosRepository(_uowFactory.GetUnitOfWork());
            var representanteEntityVal = await repoRepresentantesTecnicos.FirstOrDefaultAsync(x => x.IdTramite == representante.IdTramite
                                            && x.Cuit == representante.CUIT
                                            && x.IdRepresentanteTecnico != representante.IdRepresentanteTecnico);

            if(representanteEntityVal != null)
            {
                throw new Exception($"El representante técnico con CUIT {representante.CUIT} ya se encuentra agregado en el trámite.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repoRepresentantesTecnicos = new TramitesRepresentantesTecnicosRepository(uow);
                    var repoRepresentantesTecnicosJurisdicciones = new TramitesRepresentantesTecnicosJurisdiccionesRepository(uow);
                    var repoRepresentantesTecnicosEspecialidades = new TramitesRepresentantesTecnicosEspecialidadesRepository(uow);


                    var representanteEntity = await repoRepresentantesTecnicos.FirstOrDefaultAsync(x => x.IdRepresentanteTecnico == representante.IdRepresentanteTecnico);
                    await repoRepresentantesTecnicosJurisdicciones.RemoveRangeAsync(representanteEntity.TramitesRepresentantesTecnicosJurisdicciones);
                    await repoRepresentantesTecnicosEspecialidades.RemoveRangeAsync(representanteEntity.TramitesRepresentantesTecnicosEspecialidades);

                    _mapper.Map<TramitesRepresentanteTecnicoDTO, TramitesRepresentantesTecnicos>(representante, representanteEntity);
                    representanteEntity.TramitesRepresentantesTecnicosJurisdicciones = _mapper.Map<List<TramitesRepresentanteTecnicoJurisdiccionDTO>, List<TramitesRepresentantesTecnicosJurisdicciones>>(representante.TramitesRepresentantesTecnicosJurisdicciones);
                    representanteEntity.CreateDate = DateTime.Now;
                    representanteEntity.CreateUser = userid;
                    await repoRepresentantesTecnicos.UpdateAsync(representanteEntity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }

        public async Task EliminarRepresentanteTecnicoAsync(int IdRepresentanteTecnico)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoRepresentantesTecnicos = new TramitesRepresentantesTecnicosRepository(uow);
                    var repoRepresentantesTecnicosJurisdicciones = new TramitesRepresentantesTecnicosJurisdiccionesRepository(uow);
                    var repoRepresentantesTecnicosEspecialidades = new TramitesRepresentantesTecnicosEspecialidadesRepository(uow);
                    var entity = await repoRepresentantesTecnicos.FirstOrDefaultAsync(x => x.IdRepresentanteTecnico == IdRepresentanteTecnico);

                    await repoRepresentantesTecnicosJurisdicciones.RemoveRangeAsync(entity.TramitesRepresentantesTecnicosJurisdicciones);
                    await repoRepresentantesTecnicosEspecialidades.RemoveRangeAsync(entity.TramitesRepresentantesTecnicosEspecialidades);
                    await repoRepresentantesTecnicos.RemoveAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
                    
            }
        }
        public async Task GuardarDesvinculacionRepresentanteTecnicoAsync(TramitesRepresentantesTecnicosDesvinculacionesDTO desvinculacion)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoDesvinculaciones = new TramitesRepresentantesTecnicosDesvinculacionesRepository(uow);

                    // Crear entidad de desvinculación
                    var desvinculacionEntity = _mapper.Map<TramitesRepresentantesTecnicosDesvinculacionesDTO, TramitesRepresentantesTecnicosDesvinculaciones>(desvinculacion);
                    desvinculacionEntity.CreateDate = DateTime.Now;
                    desvinculacionEntity.CreateUser = userid;

                    // Insertar el registro de desvinculación
                    await repoDesvinculaciones.AddAsync(desvinculacionEntity);

                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }

        
        public async Task EliminarDesvinculacionRepresentanteTecnicoAsync(int IdRepresentanteTecnico)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoDesvinculaciones = new TramitesRepresentantesTecnicosDesvinculacionesRepository(uow);
                    var desvinculacionEntity = await repoDesvinculaciones.FirstOrDefaultAsync(x => x.IdRepresentanteTecnico == IdRepresentanteTecnico);
                    if (desvinculacionEntity != null)
                    {
                        // Eliminar el registro de desvinculación
                        await repoDesvinculaciones.RemoveAsync(desvinculacionEntity);
                    }
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        #endregion

        #region Informacion Empresas

        public async Task ActualizarInfEmpAsync(TramitesInfEmpDTO dto)
        {
            
            try
            {
                string userid = await _usuarioBL.GetCurrentUserid();


                using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
                {

                    try
                    {

                        var repoInfEmp = new TramitesInfEmpRepository(uow);
                        var tramiteInfEmp = await repoInfEmp.FirstOrDefaultAsync(x => x.IdTramite == dto.IdTramite);
                        
                        if (tramiteInfEmp != null)
                        {

                            int IdTramiteInfEmp = tramiteInfEmp.IdTramiteInfEmp;
                            _mapper.Map<TramitesInfEmpDTO, TramitesInfEmp>(dto, tramiteInfEmp);
                            tramiteInfEmp.IdTramiteInfEmp = IdTramiteInfEmp;
                            tramiteInfEmp.LastUpdateDate = DateTime.Now;
                            tramiteInfEmp.LastupdateUser = userid;
                            tramiteInfEmp = await repoInfEmp.UpdateAsync(tramiteInfEmp);
                        }
                        else
                        {
                            tramiteInfEmp = new TramitesInfEmp();
                            tramiteInfEmp = _mapper.Map<TramitesInfEmp>(dto);
                            tramiteInfEmp.CreateDate = DateTime.Now;
                            tramiteInfEmp.CreateUser = userid;
                            tramiteInfEmp = await repoInfEmp.AddAsync(tramiteInfEmp);
                        }
                        
                        await uow.CommitAsync();

                    }
                    catch (Exception)
                    {
                        uow.RollBack();
                        throw;
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task ActualizarInfEmpConsAsync(TramitesInfEmpConsDto dto)
        {
            try
            {
                string userid = await _usuarioBL.GetCurrentUserid();
                using var uow = _uowFactory.GetUnitOfWork();
                var repoInfConsEmp = new TramitesInfEmpConsRepository(uow);

                var tramiteInfConsEmp = await repoInfConsEmp.FirstOrDefaultAsync(x => x.IdTramite == dto.IdTramite);

                if (tramiteInfConsEmp != null)
                {
                    var existingPersonas = tramiteInfConsEmp.TramitesInfEmpConsPersonas.ToList();
                    var dtoPersons = dto.Personas ?? new List<TramitesInfEmpConsPersonaDto>();

                    // Actualiza o añade Personas
                    foreach (var dp in dtoPersons)
                    {
                        var ent = existingPersonas.FirstOrDefault(x => x.IdPersona == dp.IdPersona);
                        if (ent != null)
                        {
                            _mapper.Map(dp, ent); // actualiza propiedades del hijo
                        }
                        else
                        {
                            var nuevo = _mapper.Map<TramitesInfEmpConsPersonas>(dp);
                            // asegurar FK si hace falta:
                            nuevo.IdTramiteInfEmpCon = tramiteInfConsEmp.IdTramiteInfEmpCon;
                            tramiteInfConsEmp.TramitesInfEmpConsPersonas.Add(nuevo);
                        }
                    }
                    // Elimina las personas borradas
                    var eliminar = existingPersonas.Where(x => !dtoPersons.Any(d => d.IdPersona == x.IdPersona)).ToList();
                    foreach (var r in eliminar)
                    {
                        tramiteInfConsEmp.TramitesInfEmpConsPersonas.Remove(r);
                        uow.Context.Remove(r); // marca para borrado en EF
                    }

                    var existingDocumentos = tramiteInfConsEmp.TramitesInfEmpConsDocumentos.ToList();
                    var dtoDocumentos = dto.Documentos ?? new List<TramitesInfEmpConsDocumentoDto>();

                    // Actualiza o añade Documentos
                    foreach (var dtoDoc in dtoDocumentos)
                    {
                        var ent = existingDocumentos.FirstOrDefault(x => x.IdTramiteInfEmpConsDocumento == dtoDoc.IdTramiteInfEmpConsDocumento);
                        if (ent != null)
                        {
                            _mapper.Map(dtoDoc, ent); // actualiza propiedades del hijo
                            ent.CreateDate = DateTime.Now;
                            ent.CreateUser = userid;
                        }
                        else
                        {
                            var nuevo = _mapper.Map<TramitesInfEmpConsDocumentos>(dtoDoc);
                            // asegurar FK si hace falta:
                            nuevo.IdTramiteInfEmpCons = tramiteInfConsEmp.IdTramiteInfEmpCon;
                            nuevo.CreateDate = DateTime.Now;
                            nuevo.CreateUser = userid;
                            tramiteInfConsEmp.TramitesInfEmpConsDocumentos.Add(nuevo);
                        }
                    }
                    // Elimina los documentos borrados
                    var eliminarDocs = existingDocumentos.Where(x => !dtoDocumentos.Any(d => d.IdTramiteInfEmpConsDocumento == x.IdTramiteInfEmpConsDocumento)).ToList();
                    foreach (var r in eliminarDocs)
                    {
                        tramiteInfConsEmp.TramitesInfEmpConsDocumentos.Remove(r);
                        uow.Context.Remove(r); // marca para borrado en EF
                    }

                    var existingDeudas = tramiteInfConsEmp.TramitesInfEmpConsDeudas.ToList();
                    var dtoDeudas = dto.Deudas ?? new List<TramitesInfEmpConsDeudaDto>();

                    // Actualiza o añade Deudas
                    foreach (var dtoDeuda in dtoDeudas)
                    {
                        var ent = existingDeudas.FirstOrDefault(x => x.IdTramiteInfEmpConsDeuda == dtoDeuda.IdTramiteInfEmpConsDeuda);
                        if (ent != null)
                        {
                            _mapper.Map(dtoDeuda, ent); // actualiza propiedades del hijo
                            ent.LastUpdateDate = DateTime.Now;
                            ent.LastUpdateUser = userid;
                        }
                        else
                        {
                            var nuevo = _mapper.Map<TramitesInfEmpConsDeudas>(dtoDeuda);
                            // asegurar FK si hace falta:
                            nuevo.IdTramiteInfEmpCons = tramiteInfConsEmp.IdTramiteInfEmpCon;
                            nuevo.CreateDate = DateTime.Now;
                            nuevo.CreateUser = userid;
                            tramiteInfConsEmp.TramitesInfEmpConsDeudas.Add(nuevo);
                        }
                    }
                    // Elimina las deudas borradas
                    var eliminarDeudas = existingDeudas.Where(x => !dtoDeudas.Any(d => d.IdTramiteInfEmpConsDeuda == x.IdTramiteInfEmpConsDeuda)).ToList();
                    foreach (var r in eliminarDeudas)
                    {
                        tramiteInfConsEmp.TramitesInfEmpConsDeudas.Remove(r);
                        uow.Context.Remove(r); // marca para borrado en EF
                    }

                    // Finalmente mapear resto de propiedades del DTO
                    _mapper.Map(dto, tramiteInfConsEmp);

                    tramiteInfConsEmp = await repoInfConsEmp.UpdateAsync(tramiteInfConsEmp);
                }
                else
                {
                    tramiteInfConsEmp = new TramitesInfEmpCons();
                    tramiteInfConsEmp = _mapper.Map<TramitesInfEmpCons>(dto);
                    tramiteInfConsEmp.TramitesInfEmpConsDocumentos = _mapper.Map<List<TramitesInfEmpConsDocumentos>>(dto.Documentos);
                    tramiteInfConsEmp.TramitesInfEmpConsPersonas = _mapper.Map<List<TramitesInfEmpConsPersonas>>(dto.Personas);
                    tramiteInfConsEmp.TramitesInfEmpConsDeudas = _mapper.Map<List<TramitesInfEmpConsDeudas>>(dto.Deudas);
                    tramiteInfConsEmp.CreateDate = DateTime.Now;
                    tramiteInfConsEmp.CreateUser = userid;
                    tramiteInfConsEmp.TramitesInfEmpConsDocumentos.ToList().ForEach(f =>
                    {
                        f.CreateDate = DateTime.Now;
                        f.CreateUser = userid;
                    });
                    tramiteInfConsEmp.TramitesInfEmpConsDeudas.ToList().ForEach(f =>
                    {
                        f.CreateDate = DateTime.Now;
                        f.CreateUser = userid;
                    });
                    tramiteInfConsEmp = await repoInfConsEmp.AddAsync(tramiteInfConsEmp);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task AgregarInfEmpDocumentoAsync( TramitesInfEmpDocumentoDTO documento)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoInfEmp = new TramitesInfEmpRepository(uow);
                    var repoInfEmpDocumentos = new TramitesInfEmpDocumentosRepository(uow);

                    
                    // Alta o consulta del registro de informacion de empresa (Cabecera)
                                        TramitesInfEmp tramiteInfEmp = null;
                    if (!documento.IdTramiteInfEmp.HasValue)
                    {
                        tramiteInfEmp = await repoInfEmp.AddAsync(new TramitesInfEmp()
                        {
                            IdTramite = documento.IdTramite,
                            CreateDate = DateTime.Now,
                            CreateUser = documento.CreateUser,
                            FechaInicioActividades = DateTime.Today
                        });
                        documento.IdTramiteInfEmp = tramiteInfEmp.IdTramiteInfEmp;
                    }
                    else
                    {
                        tramiteInfEmp = await repoInfEmp.FirstOrDefaultAsync(x => x.IdTramiteInfEmp == documento.IdTramiteInfEmp);

                        if (repoInfEmpDocumentos.DbSet.Count(x => x.IdTramiteInfEmp == tramiteInfEmp.IdTramiteInfEmp &&
                                                    x.IdTipoDocumento == documento.IdTipoDocumento && x.IdFile == documento.IdFile) > 0)
                            throw new Exception($"Ya se agregó anteriormente un documento con la misma descripción y el mismo archivo. ");

                    }
                    var documentoEntity = _mapper.Map<TramitesInfEmpDocumentoDTO, TramitesInfEmpDocumentos>(documento);
                    await repoInfEmpDocumentos.AddAsync(documentoEntity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task<TramitesInfEmpDTO> GetInfEmpAsync(int IdTramite)
        {
            
            using var uow = _uowFactory.GetUnitOfWork();
            var repoInfEmp = new TramitesInfEmpRepository(uow);
            
            var element = await repoInfEmp.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            var result = _mapper.Map<TramitesInfEmp,TramitesInfEmpDTO>(element);
            return result;

        }
        public async Task<TramitesInfEmpConsDto> GetInfEmpConsAsync(int IdTramite)
        {
            
            using var uow = _uowFactory.GetUnitOfWork();
            var repoInfEmp = new TramitesInfEmpConsRepository(uow);
            
            var element = await repoInfEmp.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            var result = _mapper.Map<TramitesInfEmpCons,TramitesInfEmpConsDto>(element);
            return result;
        }
        public async Task<List<TramitesInfEmpDocumentoDTO>> GetInfEmpDocumentosAsync(int IdTramite)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repoInfEmp = new TramitesInfEmpRepository(uow);
            var repoInfEmpDocumentos = new TramitesInfEmpDocumentosRepository(uow);

            var elements = await repoInfEmpDocumentos.Where(x => x.IdTramiteInfEmpNavigation.IdTramite == IdTramite).ToListAsync();
            var result = _mapper.Map<List<TramitesInfEmpDocumentos>, List<TramitesInfEmpDocumentoDTO>>(elements);

            return result;
        }
        public async Task<List<TramitesInfEmpConsDocumentoDto>> GetInfEmpConsDocumentosAsync(int IdTramite)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repoInfEmpDocumentos = new TramitesInfEmpConsDocumentosRepository(uow);

            var elements = await repoInfEmpDocumentos.Where(x => x.IdTramiteInfEmpConsNavigation.IdTramite == IdTramite).ToListAsync();
            var result = _mapper.Map<List<TramitesInfEmpConsDocumentos>, List<TramitesInfEmpConsDocumentoDto>>(elements);
            return result;
        }

        public async Task EliminarInfEmpDocumentoAsync(int IdTramiteInfEmpDocumento)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoInfEmp = new TramitesInfEmpRepository(uow);
                    var repoInfEmpDocumentos = new TramitesInfEmpDocumentosRepository(uow);
                    var repoInfEmpDeudas = new TramitesInfEmpDeudasRepository(uow);
                    var entityDocumento = await repoInfEmpDocumentos.FirstOrDefaultAsync(x => x.IdTramiteInfEmpDocumento == IdTramiteInfEmpDocumento);
                    var entityInfEmp = entityDocumento.IdTramiteInfEmpNavigation;
                    int CantidadDocumentos = repoInfEmpDocumentos.DbSet.Count(x => x.IdTramiteInfEmpNavigation.IdTramite == entityInfEmp.IdTramite);
                    int CantidadDeudas = repoInfEmpDeudas.DbSet.Count(x => x.IdTramiteInfEmpNavigation.IdTramite == entityInfEmp.IdTramite);

                    await repoInfEmpDocumentos.RemoveAsync(entityDocumento);
                    
                    //Se elimina la cabecera si no hay registros de deuda y este es el ultimo documento que se elimina.
                    if(CantidadDocumentos == 1 && CantidadDeudas == 0)
                        await repoInfEmp.RemoveAsync(entityInfEmp);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        
        public async Task AgregarInfEmpDeudaAsync(TramitesInfEmpDeudaDTO dto)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoInfEmp = new TramitesInfEmpRepository(uow);
                    var repoInfEmpDeudas = new TramitesInfEmpDeudasRepository(uow);

                    // Alta o consulta del registro de informacion de empresa (Cabecera)
                    TramitesInfEmp tramiteInfEmp = null;
                    if (!dto.IdTramiteInfEmp.HasValue)
                    {
                        tramiteInfEmp = await repoInfEmp.AddAsync(new TramitesInfEmp()
                        {
                            IdTramite = dto.IdTramite,
                            CreateDate = DateTime.Now,
                            CreateUser = dto.CreateUser,
                            FechaInicioActividades = DateTime.Today,
                        });
                        dto.IdTramiteInfEmp = tramiteInfEmp.IdTramiteInfEmp;
                    }
                    else
                    {
                        tramiteInfEmp = await repoInfEmp.FirstOrDefaultAsync(x => x.IdTramiteInfEmp == dto.IdTramiteInfEmp);

                        if (repoInfEmpDeudas.DbSet.Count(x => x.IdTramiteInfEmp == tramiteInfEmp.IdTramiteInfEmp 
                                && x.Entidad == dto.Entidad && x.Periodo == dto.Periodo) > 0)
                            throw new Exception($"Ya se agregó anteriormente una deuda de la entidad {dto.Entidad} en el período {dto.Periodo}.");

                    }
                    var deudaEntity = _mapper.Map<TramitesInfEmpDeudaDTO, TramitesInfEmpDeudas>(dto);
                    await repoInfEmpDeudas.AddAsync(deudaEntity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task AgregarInfEmpConsDeudaAsync(TramitesInfEmpConsDeudaDto dto)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoInfEmpCons = new TramitesInfEmpConsRepository(uow);
                    var repoInfEmpConsDeudas = new TramitesInfEmpConsDeudasRepository(uow);

                    // Alta o consulta del registro de informacion de empresa (Cabecera)
                    TramitesInfEmpCons tramiteInfEmp = null;
                    if (!dto.IdTramiteInfEmpCons.HasValue)
                    {
                        tramiteInfEmp = await repoInfEmpCons.AddAsync(new TramitesInfEmpCons()
                        {
                            IdTramite = dto.IdTramite,
                            CreateDate = DateTime.Now,
                            CreateUser = dto.CreateUser,
                        });
                        dto.IdTramiteInfEmpCons = tramiteInfEmp.IdTramiteInfEmpCon;
                    }
                    else
                    {
                        tramiteInfEmp = await repoInfEmpCons.FirstOrDefaultAsync(x => x.IdTramiteInfEmpCon == dto.IdTramiteInfEmpCons);

                        if (repoInfEmpConsDeudas.DbSet.Count(x => x.IdTramiteInfEmpCons == tramiteInfEmp.IdTramiteInfEmpCon
                                && x.Entidad == dto.Entidad && x.Periodo == dto.Periodo) > 0)
                            throw new Exception($"Ya se agregó anteriormente una deuda de la entidad {dto.Entidad} en el período {dto.Periodo}.");

                    }
                    var deudaEntity = _mapper.Map<TramitesInfEmpConsDeudaDto, TramitesInfEmpConsDeudas>(dto);
                    await repoInfEmpConsDeudas.AddAsync(deudaEntity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarInfEmpDeudaAsync(TramitesInfEmpDeudaDTO dto)
        {
            var userid = await _usuarioBL.GetCurrentUserid(); 
            
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoInfEmp = new TramitesInfEmpRepository(uow);
                    var repoInfEmpDeudas = new TramitesInfEmpDeudasRepository(uow);

                    if (repoInfEmpDeudas.DbSet.Count(x =>x.IdTramiteInfEmpDeuda != dto.IdTramiteInfEmpDeuda &&  x.IdTramiteInfEmp == dto.IdTramiteInfEmp
                             && x.Entidad == dto.Entidad && x.Periodo == dto.Periodo) > 0)
                        throw new Exception($"Ya se agregó anteriormente una deuda de la entidad {dto.Entidad} en el período {dto.Periodo}.");


                    var entity = await repoInfEmpDeudas.FirstOrDefaultAsync(x => x.IdTramiteInfEmpDeuda == dto.IdTramiteInfEmpDeuda);

                    entity.Periodo = dto.Periodo;
                    entity.Entidad = dto.Entidad;
                    entity.Monto  = dto.Monto;
                    entity.Situacion = dto.Situacion;
                    entity.DiasDeAtraso = dto.DiasDeAtraso;
                    entity.LastUpdateDate = DateTime.Now;
                    entity.LastUpdateUser = userid;
                    await repoInfEmpDeudas.UpdateAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarInfEmpConsDeudaAsync(TramitesInfEmpConsDeudaDto dto)
        {
            var userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoInfEmp = new TramitesInfEmpConsRepository(uow);
                    var repoInfEmpDeudas = new TramitesInfEmpConsDeudasRepository(uow);

                    if (repoInfEmpDeudas.DbSet.Count(x => x.IdTramiteInfEmpConsDeuda != dto.IdTramiteInfEmpConsDeuda && x.IdTramiteInfEmpCons == dto.IdTramiteInfEmpCons
                             && x.Entidad == dto.Entidad && x.Periodo == dto.Periodo) > 0)
                        throw new Exception($"Ya se agregó anteriormente una deuda de la entidad {dto.Entidad} en el período {dto.Periodo}.");


                    var entity = await repoInfEmpDeudas.FirstOrDefaultAsync(x => x.IdTramiteInfEmpConsDeuda == dto.IdTramiteInfEmpConsDeuda);

                    entity.Periodo = dto.Periodo;
                    entity.Entidad = dto.Entidad;
                    entity.Monto = dto.Monto;
                    entity.Situacion = dto.Situacion;
                    entity.DiasDeAtraso = dto.DiasDeAtraso;
                    entity.LastUpdateDate = DateTime.Now;
                    entity.LastUpdateUser = userid;
                    await repoInfEmpDeudas.UpdateAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task EliminarInfEmpDeudaAsync(int IdTramiteInfEmpDeuda)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoInfEmp = new TramitesInfEmpRepository(uow);
                    var repoInfEmpDocumentos = new TramitesInfEmpDocumentosRepository(uow);
                    var repoInfEmpDeudas = new TramitesInfEmpDeudasRepository(uow);
                    var entityDeuda = await repoInfEmpDeudas.FirstOrDefaultAsync(x => x.IdTramiteInfEmpDeuda == IdTramiteInfEmpDeuda);
                    var entityInfEmp = entityDeuda.IdTramiteInfEmpNavigation;
                    int CantidadDeudas = repoInfEmpDeudas.DbSet.Count(x => x.IdTramiteInfEmpNavigation.IdTramite == entityInfEmp.IdTramite);
                    int CantidadDocumentos = repoInfEmpDocumentos.DbSet.Count(x => x.IdTramiteInfEmpNavigation.IdTramite == entityInfEmp.IdTramite);

                    await repoInfEmpDeudas.RemoveAsync(entityDeuda);

                    //Se elimina la cabecera si no hay registros de documentos y este es el ultimo de deuda que se elimina.
                    if ( CantidadDeudas == 1 && CantidadDocumentos == 0)
                        await repoInfEmp.RemoveAsync(entityInfEmp);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        public async Task EliminarInfEmpConsDeudaAsync(int IdTramiteInfEmpConsDeuda)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoInfEmpCons = new TramitesInfEmpConsRepository(uow);
                    var repoInfEmpConsDocumentos = new TramitesInfEmpConsDocumentosRepository(uow);
                    var repoInfEmpConsDeudas = new TramitesInfEmpConsDeudasRepository(uow);
                    
                    var entityDeuda = await repoInfEmpConsDeudas.FirstOrDefaultAsync(x => x.IdTramiteInfEmpConsDeuda == IdTramiteInfEmpConsDeuda);
                    var entityInfEmp = entityDeuda.IdTramiteInfEmpConsNavigation;
                    int CantidadDeudas = repoInfEmpConsDeudas.DbSet.Count(x => x.IdTramiteInfEmpConsNavigation.IdTramite == entityInfEmp.IdTramite);
                    int CantidadDocumentos = repoInfEmpConsDocumentos.DbSet.Count(x => x.IdTramiteInfEmpConsNavigation.IdTramite == entityInfEmp.IdTramite);

                    await repoInfEmpConsDeudas.RemoveAsync(entityDeuda);

                    //Se elimina la cabecera si no hay registros de documentos y este es el ultimo de deuda que se elimina.
                    if (CantidadDeudas == 1 && CantidadDocumentos == 0)
                        await repoInfEmpCons.RemoveAsync(entityInfEmp);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        public async Task<List<TramitesInfEmpDeudaDTO>> GetInfEmpDeudasAsync(int IdTramite)
        {

            using var uow = _uowFactory.GetUnitOfWork();
            var repoInfEmp = new TramitesInfEmpRepository(uow);
            var repoInfEmpDeudas = new TramitesInfEmpDeudasRepository(uow);

            var elements = await repoInfEmpDeudas.Where(x => x.IdTramiteInfEmpNavigation.IdTramite == IdTramite).ToListAsync();
            var result = _mapper.Map<List<TramitesInfEmpDeudas>, List<TramitesInfEmpDeudaDTO>>(elements);
            return result;
        }
        public async Task<List<TramitesInfEmpConsDeudaDto>> GetInfEmpConsDeudasAsync(int IdTramite)
        {

            using var uow = _uowFactory.GetUnitOfWork();
            var repoInfEmp = new TramitesInfEmpConsRepository(uow);
            var repoInfEmpDeudas = new TramitesInfEmpConsDeudasRepository(uow);

            var elements = await repoInfEmpDeudas.Where(x => x.IdTramiteInfEmpConsNavigation.IdTramite == IdTramite).ToListAsync();
            var result = _mapper.Map<List<TramitesInfEmpConsDeudas>, List<TramitesInfEmpConsDeudaDto>>(elements);
            return result;
        }
        public async Task<TramitesInfEmpDeudaDTO> GetInfEmpDeudaAsync(int IdTramiteInfEmpDeuda)
        {

            using var uow = _uowFactory.GetUnitOfWork();
            var repoInfEmp = new TramitesInfEmpRepository(uow);
            var repoInfEmpDeudas = new TramitesInfEmpDeudasRepository(uow);

            var element = await repoInfEmpDeudas.FirstOrDefaultAsync(x => x.IdTramiteInfEmpDeuda == IdTramiteInfEmpDeuda);
            var result = _mapper.Map<TramitesInfEmpDeudas, TramitesInfEmpDeudaDTO>(element);
            return result;
        }
        public async Task<TramitesInfEmpConsDeudaDto> GetInfEmpConsDeudaAsync(int IdTramiteInfEmpConsDeuda)
        {

            using var uow = _uowFactory.GetUnitOfWork();
            var repoInfEmp = new TramitesInfEmpConsRepository(uow);
            var repoInfEmpDeudas = new TramitesInfEmpConsDeudasRepository(uow);

            var element = await repoInfEmpDeudas.FirstOrDefaultAsync(x => x.IdTramiteInfEmpConsDeuda == IdTramiteInfEmpConsDeuda);
            var result = _mapper.Map<TramitesInfEmpConsDeudas, TramitesInfEmpConsDeudaDto>(element);
            return result;
        }
        #endregion

        #region Balance General

        public async Task<TramitesBalanceGeneralDTO> GetBalanceGeneralAsync(int IdTramiteBalanceGeneral)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesBalancesGeneralesRepository(uow);

            var element = await repo.FirstOrDefaultAsync(x => x.IdTramiteBalanceGeneral == IdTramiteBalanceGeneral);
            var result = _mapper.Map<TramitesBalancesGenerales, TramitesBalanceGeneralDTO>(element);
            return result;

        }
        public async Task<List<TramitesBalanceGeneralDTO>> GetBalancesGeneralesAsync(int IdTramite)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesBalancesGeneralesRepository(uow);

            var elements = await repo.Where(x => x.IdTramite == IdTramite).ToListAsync();
            var result = _mapper.Map<List<TramitesBalancesGenerales>, List<TramitesBalanceGeneralDTO>>(elements);
            return result;

        }
        public async Task ActualizarBalanceGeneralAsync(TramitesBalanceGeneralDTO dto)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoBalance = new TramitesBalancesGeneralesRepository(uow);
                    
                    TramitesBalancesGenerales entityBalance = null;
                    if (!dto.IdTramiteBalanceGeneral.HasValue)
                    {
                        entityBalance = _mapper.Map<TramitesBalanceGeneralDTO, TramitesBalancesGenerales>(dto);
                        entityBalance.CreateDate = DateTime.Now;
                        entityBalance.CreateUser = await _usuarioBL.GetCurrentUserid();
                        await repoBalance.AddAsync(entityBalance);
                    }
                    else
                    {
                        entityBalance = await repoBalance.FirstOrDefaultAsync(x => x.IdTramiteBalanceGeneral == dto.IdTramiteBalanceGeneral);
                        _mapper.Map<TramitesBalanceGeneralDTO, TramitesBalancesGenerales>(dto,entityBalance);
                        entityBalance.LastUpdateDate = DateTime.Now;
                        entityBalance.LastUpdateUser = await _usuarioBL.GetCurrentUserid();
                        await repoBalance.UpdateAsync(entityBalance);
                    }
                    
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task EliminarBalanceGeneralAsync(int IdTramite,int Anio)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoBalance = new TramitesBalancesGeneralesRepository(uow);
                    var entityBalance = await repoBalance.FirstOrDefaultAsync(x => x.IdTramite == IdTramite && x.Anio == Anio);
                    await repoBalance.RemoveAsync(entityBalance);

                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        #endregion

        #region Equipos de la empresa
        public async Task<TramitesEquipoDTO> GetEquiposAsync(int IdTramite, bool Afectado)
        {

            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesEquiposRepository(uow);

            var element = await repo.FirstOrDefaultAsync(x => x.IdTramite == IdTramite && x.Afectado == Afectado);
            var result = _mapper.Map<TramitesEquipos, TramitesEquipoDTO>(element);
            return result;

        }
        public async Task ActualizarEquipoAsync(List<TramitesEquipoDTO> dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoEquipos = new TramitesEquiposRepository(uow);
                    
                    foreach (var item in dto)
                    {
                        if (item.IdTramiteEquipo > 0)
                        {
                            //Edicion
                            var entity = await repoEquipos.FirstOrDefaultAsync(x => x.IdTramiteEquipo == item.IdTramiteEquipo);
                            _mapper.Map<TramitesEquipoDTO, TramitesEquipos>(item, entity);
                            entity.LastUpdateDate = DateTime.Now;
                            entity.LastUpdateUser = userid;
                            await repoEquipos.UpdateAsync(entity);

                        }
                        else
                        {
                            //Alta
                            var entity = _mapper.Map<TramitesEquipoDTO, TramitesEquipos>(item);

                            entity.CreateDate = DateTime.Now;
                            entity.CreateUser = userid;
                            await repoEquipos.AddAsync(entity);
                        }
                    }

                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task EliminarEquipoAsync(int IdTramite, bool Afectado)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesEquiposRepository(uow);
                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTramite == IdTramite && x.Afectado == Afectado);
                    if(entity != null) 
                        await repo.RemoveAsync(entity);

                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        #endregion

        #region Bienes Raíces de la empresa
        public async Task<TramitesBienesRaicesDTO> GetBienesRaicesAsync(int IdTramite,bool Afectado)
        {

            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesBienesRaicesRepository(uow);

            var element = await repo.FirstOrDefaultAsync(x => x.IdTramite == IdTramite && x.Afectado == Afectado);
            var result = _mapper.Map<TramitesBienesRaices, TramitesBienesRaicesDTO>(element);
            return result;

        }
        public async Task AgregarBienRaizAsync(TramitesBienesRaicesDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesBienesRaicesRepository(uow);
                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTramite == dto.IdTramite);

                    if (entity == null)
                    {

                        entity = _mapper.Map<TramitesBienesRaicesDTO, TramitesBienesRaices>(dto);

                        entity.CreateDate = DateTime.Now;
                        entity.CreateUser = userid;
                        await repo.AddAsync(entity);
                        await uow.CommitAsync();
                    }
                    else
                        throw new Exception("Ya existe un Bien con los mismos datos.");
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarBienRaizAsync(List<TramitesBienesRaicesDTO> dto,int IdTramite, bool SinDatosForm)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesBienesRaicesRepository(uow);
                    var repoTramite = new TramitesRepository(uow);
                    
                    await repoTramite.ActualizarSinDatosForm(IdTramite, 8, SinDatosForm);

                    var lstBienes = await repo.Where(x => x.IdTramite == IdTramite).ToListAsync();
                    await repo.RemoveRangeAsync(lstBienes);

                    foreach (var item in dto)
                    {
                        
                        //Alta
                        var entity = _mapper.Map<TramitesBienesRaicesDTO, TramitesBienesRaices>(item);

                        entity.CreateDate = DateTime.Now;
                        entity.CreateUser = userid;
                        await repo.AddAsync(entity);

                    }
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task EliminarBienRaizAsync(int IdTramite, bool Afectado)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesBienesRaicesRepository(uow);
                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTramite == IdTramite && x.Afectado == Afectado);
                    if(entity != null)
                        await repo.RemoveAsync(entity);

                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        #endregion

        #region Obras
        public async Task<List<TramitesObrasDTO>> GetObrasAsync(int IdTramite)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesObrasRepository(uow);

            var elements = await repo.Where(x => x.IdTramite == IdTramite).ToListAsync();
            var result = _mapper.Map<List<TramitesObras>, List<TramitesObrasDTO>>(elements);
            return result;

        }
        public async Task<TramitesObrasDTO> GetObraAsync(int IdTramiteObra)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesObrasRepository(uow);

            var element = await repo.Where(x => x.IdTramiteObra == IdTramiteObra).FirstOrDefaultAsync();
            var result = _mapper.Map<TramitesObras, TramitesObrasDTO>(element);
            return result;
        }
        public async Task AgregarObraAsync(TramitesObrasDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesObrasRepository(uow);

                    if (repo.DbSet.Count(x => x.IdTramite == dto.IdTramite && x.IdTramiteEspecialidad == dto.IdTramiteEspecialidad &&
                                         x.IdTramiteEspecialidadSeccion == dto.IdTramiteEspecialidadSeccion ) >= 4)
                        throw new Exception($"NO es posible agregar la obra ya que la cantidad máxima de obras por especialidad son 4.");

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTramite == dto.IdTramite &&
                                                                x.IdTramiteEspecialidad == dto.IdTramiteEspecialidad &&
                                                                x.IdTramiteEspecialidadSeccion == dto.IdTramiteEspecialidadSeccion &&
                                                                x.IdObraPciaLp == dto.IdObraPciaLp);

                    if (entity == null)
                    {

                        entity = _mapper.Map<TramitesObrasDTO, TramitesObras>(dto);

                        entity.CreateDate = DateTime.Now;
                        entity.CreateUser = userid;
                        await repo.AddAsync(entity);
                        await uow.CommitAsync();
                    }
                    else
                        throw new Exception("Ya existe un obra con los mismos datos de especialidad, seccion y Obra.");
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarObraAsync(TramitesObrasDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesObrasRepository(uow);
                    var entityDuplicado = await repo.FirstOrDefaultAsync(x => x.IdTramite == dto.IdTramite &&
                                                                x.IdTramiteEspecialidad == dto.IdTramiteEspecialidad &&
                                                                x.IdTramiteEspecialidadSeccion == dto.IdTramiteEspecialidadSeccion &&
                                                                x.IdObraPciaLp == dto.IdObraPciaLp &&
                                                                x.IdTramiteObra != dto.IdTramiteObra); 

                    if (entityDuplicado != null)
                        throw new Exception($"Ya existe un registro con los mismos datos.");


                    if (repo.DbSet.Count(x => x.IdTramite == dto.IdTramite &&
                                         x.IdTramiteEspecialidad == dto.IdTramiteEspecialidad &&
                                         x.IdTramiteEspecialidadSeccion == dto.IdTramiteEspecialidadSeccion &&
                                         x.IdTramiteObra != dto.IdTramiteObra) >= 4)
                    {
                        throw new Exception($"NO es posible modificar la obra ya que la cantidad máxima de obras por especialidad son 4.");
                    }

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTramiteObra == dto.IdTramiteObra);
                    repo.Context.Entry(entity).State = EntityState.Detached;
                    if (entity != null)
                    {
                        _mapper.Map<TramitesObrasDTO, TramitesObras>(dto, entity);
                        entity.LastUpdateDate = DateTime.Now;
                        entity.LastUpdateUser = userid;

                        await repo.UpdateAsync(entity);
                        await uow.CommitAsync();
                    }
                    else
                        throw new Exception("No se encontró el registro que desaa actualizar.");
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task EliminarObraAsync(int IdTramiteObra)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesObrasRepository(uow);
                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTramiteObra == IdTramiteObra);
                    await repo.RemoveAsync(entity);

                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        #endregion

        #region Obras en Ejecucion
        public async Task<List<TramitesObraEjecucionDTO>> GetObrasEjecucionAsync(int IdTramite)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesObrasEjecucionRepository(uow);

            var elements = await repo.Where(x => x.IdTramite == IdTramite).ToListAsync();
            var result = _mapper.Map<List<TramitesObrasEjecucion>, List<TramitesObraEjecucionDTO>>(elements);
            return result;

        }
        public async Task<TramitesObraEjecucionDTO> GetObraEjecucionAsync(int IdTramiteObraEjecucion)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesObrasEjecucionRepository(uow);

            var element = await repo.Where(x => x.IdTramiteObraEjec == IdTramiteObraEjecucion).FirstOrDefaultAsync();
            var result = _mapper.Map<TramitesObrasEjecucion, TramitesObraEjecucionDTO>(element);
            return result;
        }
        public async Task AgregarObraEjecucionAsync(TramitesObraEjecucionDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesObrasEjecucionRepository(uow);
                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTramite == dto.IdTramite &&
                                                                x.IdObraPciaLp == dto.IdObraPciaLp &&
                                                                x.Ubicacion.ToLower().Trim() == dto.Ubicacion.ToLower().Trim() &&
                                                                x.Comitente.ToLower().Trim() == dto.Comitente.ToLower().Trim() 
                                                                );

                    if (entity == null)
                    {

                        entity = _mapper.Map<TramitesObraEjecucionDTO, TramitesObrasEjecucion>(dto);

                        entity.CreateDate = DateTime.Now;
                        entity.CreateUser = userid;
                        await repo.AddAsync(entity);
                        await uow.CommitAsync();
                    }
                    else
                        throw new Exception("Ya existe un registro con los mismos datos.");
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarObraEjecucionAsync(TramitesObraEjecucionDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            var repoRead = new TramitesObrasEjecucionRepository(_uowFactory.GetUnitOfWork());
            var entityDuplicado = await repoRead.FirstOrDefaultAsync(x => x.IdTramite == dto.IdTramite &&
                                            x.IdObraPciaLp == dto.IdObraPciaLp &&
                                            x.Ubicacion.ToLower().Trim() == dto.Ubicacion.ToLower().Trim() &&
                                            x.Comitente.ToLower().Trim() == dto.Comitente.ToLower().Trim() &&
                                            x.IdTramiteObraEjec != dto.IdTramiteObraEjec);

            if (entityDuplicado != null)
                throw new Exception($"Ya existe un registro con los mismos datos.");


            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesObrasEjecucionRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTramiteObraEjec == dto.IdTramiteObraEjec);

                    if (entity != null)
                    {
                        _mapper.Map<TramitesObraEjecucionDTO, TramitesObrasEjecucion>(dto, entity);

                        entity.LastUpdateDate = DateTime.Now;
                        entity.LastUpdateUser = userid;
                        await repo.UpdateAsync(entity);
                        await uow.CommitAsync();
                    }
                    else
                        throw new Exception("No se encontró el registro que desaa actualizar.");
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task EliminarObraEjecucionAsync(int IdTramiteObraEjecucion)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesObrasEjecucionRepository(uow);
                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTramiteObraEjec == IdTramiteObraEjecucion);
                    await repo.RemoveAsync(entity);

                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        #endregion

        public async Task<List<string>> ValidacionesPresentarTramite(int IdTramite)
        {

            List<string> lstMensajes = new List<string>();

            var uof = _uowFactory.GetUnitOfWork();
            var repoTramite = new TramitesRepository(uof);

            var tramite = await repoTramite.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            
            if (tramite == null)
                throw new ArgumentException($"No se encontró el trámite {IdTramite}");

            int IdGrupoTramite = tramite.IdTipoTramiteNavigation.IdGrupoTramite;

            if (tramite.TramitesEspecialidades.Count == 0) 
                lstMensajes.Add("Formulario 1: No se encontraron registros referidos a especialidades.");


            //Formulario 2 y 3 
            if (PermiteEditarFormulario(tramite.IdTipoTramite.Value, Constants.Formularios.InformacionEmpresa))
            {
                if (tramite.TramitesInfEmp == null && tramite.TramitesInfEmpCons == null)
                    lstMensajes.Add("Formulario 3: No se encontraron datos referidos a la información de la empresa.");
                else
                {
                    var lstDocumentosObligatorios = await _tablasBL.GetTiposDeDocumentosObligatoriosAsync(IdGrupoTramite);


                    var tiposDeDocumentosFaltantes = IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores
                                                                ? lstDocumentosObligatorios.Select(s => s.IdTipoDocumento)
                                                                .Except(tramite.TramitesInfEmp.TramitesInfEmpDocumentos
                                                                .Select(s => s.IdTipoDocumento))
                                                                .ToList()
                                                                : lstDocumentosObligatorios.Select(s => s.IdTipoDocumento)
                                                                .Except(tramite.TramitesInfEmpCons.TramitesInfEmpConsDocumentos
                                                                .Select(s => s.IdTipoDocumento))
                                                                .ToList()
                                                                ;

                    var lstDocumentosFaltantes = lstDocumentosObligatorios.Where(x => tiposDeDocumentosFaltantes.Contains(x.IdTipoDocumento))
                                                                          .Select(s => s.Descripcion).ToList();

                    if (lstDocumentosFaltantes.Count > 0)
                    {
                        lstMensajes.Add($"Formulario 3: Documento/s Faltante/s:");
                        lstDocumentosFaltantes.ForEach(f => lstMensajes.Add($"- {f}"));
                        
                    }

                }
            }
            //--

            //Formulario 6
            if(PermiteEditarFormulario(tramite.IdTipoTramite.Value, Constants.Formularios.BalanceGeneral) 
                && tramite.TramitesBalancesGenerales.Count == 0)
                lstMensajes.Add("Formulario 6: No se encontraron datos referidos a la información del Balance General.");

            //Formulario 7
            if (IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores 
                && PermiteEditarFormulario(tramite.IdTipoTramite.Value, Constants.Formularios.Equipos))
            {
                if (!tramite.TramitesEquipos.Any())
                    lstMensajes.Add("Formulario 7: No se encontraron datos referidos a los Equipos.");
                else if (!tramite.TramitesEquipos.Any(x => x.Afectado))
                    lstMensajes.Add("Formulario 7: No se cargaron Equipos (Afectados).");
            }
            //Formulario 8
            if (IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores 
                && PermiteEditarFormulario(tramite.IdTipoTramite.Value, Constants.Formularios.BienesRaices))
            {
                if (tramite.TramitesBienesRaices == null)
                    lstMensajes.Add("Formulario 8: No se encontraron datos referidos a los Inmuebles.");
                else if (!tramite.TramitesBienesRaices.Any(x => x.Afectado) && !tramite.SinDatosForm8)
                    lstMensajes.Add("Formulario 8: No se cargaron Inmuebles (Afectados).");
            }

            #region Representantes técnicos

            if (IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores && 
                (
                 PermiteEditarFormulario(tramite.IdTipoTramite.Value, Constants.Formularios.RepresentantesTecnicos) ||
                 PermiteEditarFormulario(tramite.IdTipoTramite.Value, Constants.Formularios.Especialidades))
                )
            {
                // Obtener todas las especialidades del trámite
                var especialidades = tramite.TramitesEspecialidades
                .Select(s => new { s.IdEspecialidad, s.IdEspecialidadNavigation.NombreEspecialidad })
                .Distinct()
                .ToList();

                // Obtener todas las especialidades que tienen representante técnico
                List<int> especialidadesConRepresentante = new();
                foreach (var representante in tramite.TramitesRepresentantesTecnicos)
                {
                    especialidadesConRepresentante.AddRange(
                        representante.TramitesRepresentantesTecnicosEspecialidades
                            .Select(s => s.IdTramiteEspecialidadNavigation.IdEspecialidad)
                    );
                }

                // Quitar duplicados
                especialidadesConRepresentante = especialidadesConRepresentante.Distinct().ToList();

                // Encontrar especialidades sin representante
                var especialidadesSinRepresentante = especialidades
                    .Where(e => !especialidadesConRepresentante.Contains(e.IdEspecialidad))
                    .ToList();

                // Generar mensajes de error
                if (especialidadesSinRepresentante.Any())
                {
                    if (especialidadesSinRepresentante.Count == 1)
                    {
                        lstMensajes.Add(
                            $"Formulario 9: La especialidad \"{especialidadesSinRepresentante[0].NombreEspecialidad}\" no posee un representante técnico asignado."
                        );
                    }
                    else
                    {
                        var nombres = string.Join(", ", especialidadesSinRepresentante.Select(e => e.NombreEspecialidad));
                        lstMensajes.Add(
                            $"Formulario 9: Las siguientes especialidades no poseen representante técnico asignado: {nombres}"
                        );
                    }
                }
            
            }
            #endregion

            //Formulario 10
            if (IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores 
                && PermiteEditarFormulario(tramite.IdTipoTramite.Value, Constants.Formularios.Obras) &&
                tramite.TramitesObras.Count == 0 && !tramite.SinDatosForm10)
                lstMensajes.Add("Formulario 10: No se encontraron datos referidos a las Obras (Capacidad técnica).");

            //Formulario 11
            if (PermiteEditarFormulario(tramite.IdTipoTramite.Value, Constants.Formularios.AntecedentesDeProduccion)
                && tramite.TramitesAntecedentes.Count == 0 && !tramite.SinDatosForm11)
                lstMensajes.Add("Formulario 11: No se encontraron datos referidos a los Antecedentes de Producción.");

            //Formulario 12
            if (IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores 
                && PermiteEditarFormulario(tramite.IdTipoTramite.Value, Constants.Formularios.ObrasEnEjecucion) 
                && tramite.TramitesObrasEjecucion.Count == 0 && !tramite.SinDatosForm12)
                lstMensajes.Add("Formulario 12: No se encontraron datos referidos a las Obras en Ejecución.");


            return lstMensajes;
        }
        public async Task<TramitesDTO> GetTramitePendienteAsync(int IdGrupoTramite, int IdEmpresa)
        {
            TramitesDTO result = null;
            var uof = _uowFactory.GetUnitOfWork();
            var repoTramite = new TramitesRepository(uof);

            int[] EstadosExcluir = new int[] { Constants.TramitesEstados.Aprobado,
                                               Constants.TramitesEstados.Rechazado,
                                               Constants.TramitesEstados.Anulado };
            
            var tramite = await repoTramite.FirstOrDefaultAsync(x=> x.IdTipoTramiteNavigation.IdGrupoTramite == IdGrupoTramite 
                                                                && x.IdEmpresa == IdEmpresa 
                                                                && !EstadosExcluir.Contains(x.IdEstado));

            if (tramite != null)
                result = _mapper.Map<Tramites,TramitesDTO>(tramite);

            return result;
        }
        public async Task<TramitesDTO> ThrowIfValidacionesActualizacionAnual(int IdTipoTramite, int IdEmpresa)
        {
            TramitesDTO result = null;
            var uof = _uowFactory.GetUnitOfWork();
            var repoTramite = new TramitesRepository(uof);
            var repoEmpresa = new EmpresasRepository(uof);

            //Obtiene la ultima fecha de creacion del trámte de actualización
            var TramiteUltimaActualizacion = await repoTramite.Where(x => x.IdTipoTramite == IdTipoTramite
                                                                    && x.IdEmpresa == IdEmpresa
                                                                    && x.IdEstado == Constants.TramitesEstados.Aprobado
                                                                    ).Select(s=> new
                                                                    {
                                                                        IdTramite = s.IdTramite,
                                                                        FechaUltimaActualizacionCompleta = s.TramitesBalancesGenerales.OrderByDescending(o=> o.Anio).Select(s=> s.FechaBalance).FirstOrDefault(),
                                                                        CreateDate = s.CreateDate
                                                                    }).FirstOrDefaultAsync();

            //Si no se encontró un trámite de actualizacion toma la ultima fecha de balance de la empresa.
            if (TramiteUltimaActualizacion != null)
            {
                if(DateTime.Today <= TramiteUltimaActualizacion.FechaUltimaActualizacionCompleta.AddYears(1))
                    throw new ArgumentException($"La empresa ya realizó esta actualización en el último año. Tramite Nro. {TramiteUltimaActualizacion.IdTramite}, se podría volver a actualizar a partir del {TramiteUltimaActualizacion.FechaUltimaActualizacionCompleta.AddYears(1).ToString("dd/MM/yyyy")}.");
            }
            else
            {
                var empresa = await repoEmpresa.FirstOrDefaultAsync(x=> x.IdEmpresa == IdEmpresa);
                if (!empresa.FechaBalance.HasValue)
                    throw new ArgumentException($"No se encontró la fecha de balance de la empresa.");  //siempre que hay una registración aprobada debe haber una fecha de balance.

                if (empresa.FechaBalance.HasValue && DateTime.Today <= empresa.FechaBalance.Value.AddYears(1))
                    throw new ArgumentException($"Para poder realizar esta actualización, debe haber transcurrido al menos 1 año de la fecha de balance {empresa.FechaBalance.Value.ToString("dd/MM/yyyy")}.");

            }


            return result;
        }

        public async Task ThrowIfValidacionesActualizacionCapacidadRespEquipos(int IdTipoTramite, int IdEmpresa)
        {
            var uof = _uowFactory.GetUnitOfWork();
            var repoTramite = new TramitesRepository(uof);
            var repoEmpresa = new EmpresasRepository(uof);
            DateTime? FechaVencimiento;


            var empresa = await repoEmpresa.FirstOrDefaultAsync(x => x.IdEmpresa == IdEmpresa);

            //si no se encontró la empresa lanza error
            if (empresa == null)
                throw new ArgumentException($"No se encontró la empresa {IdEmpresa}.");

            FechaVencimiento = empresa.Vencimiento;
            if (!FechaVencimiento.HasValue)
            {
                DateTime? fechaBalance = repoTramite.Where(x=> x.IdEmpresa == IdEmpresa
                                            && x.IdEstado == Constants.TramitesEstados.Aprobado)
                                          .OrderByDescending(o => o.IdTramite)
                                          .SelectMany(s=> s.TramitesBalancesGenerales)
                                          .OrderByDescending(o=> o.FechaBalance)
                                          .Select(s=> s.FechaBalance)
                                          .FirstOrDefault();
                //Si no se encontró la fecha de balance lanza error
                if (!fechaBalance.HasValue)
                    throw new ArgumentException($"No se encontró la fecha de balance de la empresa.");  //siempre que hay una registración aprobada debe haber una fecha de balance.
                else
                    FechaVencimiento = fechaBalance.Value.AddDays(540); //Fecha de vencimiento = fecha de balance + 540 días (18 meses)
            }
            
            //Si la fecha de vencimiento es mayor a la fecha actual lanza error
            if (FechaVencimiento.Value < DateTime.Today)
                throw new ArgumentException($"La empresa no puede realizar el trámite de actualización ya que su capacidad se encuentra vencida. Fecha de vencimiento: {FechaVencimiento.Value.ToString("dd/MM/yyyy")}.");

            
            //Obtiene la ultima fecha de creacion del trámite de actualización
            var TramiteUltimaActualizacion = await repoTramite.Where(x => x.IdTipoTramite == IdTipoTramite
                                                                    && x.IdEmpresa == IdEmpresa
                                                                    && x.IdEstado == Constants.TramitesEstados.Aprobado
                                                                    ).Select(s => new
                                                                    {
                                                                        IdTramite = s.IdTramite,
                                                                        CreateDate = s.CreateDate
                                                                    }).FirstOrDefaultAsync();

            //Si se encontró un trámite de actualizacion verifica que haya pasado un año desde la ultima actualización
            // de lo contrario lanza un error.
            if (TramiteUltimaActualizacion != null && DateTime.Today <= TramiteUltimaActualizacion.CreateDate.AddYears(1))
                throw new ArgumentException($"La empresa ya realizó esta actualización en el último año. Tramite Nro. {TramiteUltimaActualizacion.IdTramite}, se podría volver a actualizar a partir del {TramiteUltimaActualizacion.CreateDate.AddYears(1).ToString("dd/MM/yyyy")}.");

         
        }
        public async Task<TramitesDTO> GetUltimoTramiteAprobadoAsync(int IdTipoTramite, int IdEmpresa)
        {
            TramitesDTO result = null;
            var uof = _uowFactory.GetUnitOfWork();
            var repoTramite = new TramitesRepository(uof);

            var tramite = await repoTramite.Where(x => x.IdTipoTramite == IdTipoTramite && x.IdEmpresa == IdEmpresa 
                                                  && x.IdEstado == Constants.TramitesEstados.Aprobado)
                                            .OrderByDescending(o=> o.IdTramite)
                                            .FirstOrDefaultAsync();
                
            if (tramite != null)
                result = _mapper.Map<Tramites, TramitesDTO>(tramite);

            return result;
        }

        public async Task<TramitesDTO> GetUltimoTramiteAprobadoAsync(int IdEmpresa)
        {
            TramitesDTO result = null;
            var uof = _uowFactory.GetUnitOfWork();
            var repoTramite = new TramitesRepository(uof);

            var tramite = await repoTramite.Where(x => x.IdEmpresa == IdEmpresa
                                                  && x.IdEstado == Constants.TramitesEstados.Aprobado)
                                            .OrderByDescending(o => o.IdTramite)
                                            .FirstOrDefaultAsync();

            if (tramite != null)
                result = _mapper.Map<Tramites, TramitesDTO>(tramite);

            return result;
        }

        public async Task<TramitesRepresentanteTecnicoDTO> GetRepresentanteTecnicoByCuitAsync(decimal cuit)
        {
            TramitesRepresentanteTecnicoDTO result = null;
            var uof = _uowFactory.GetUnitOfWork();
            var repo = new TramitesRepresentantesTecnicosRepository(uof);

            var element = await repo.Where(x => x.Cuit == cuit).OrderByDescending(x=> x.IdRepresentanteTecnico).FirstOrDefaultAsync();
            if (element != null)
                result = _mapper.Map<TramitesRepresentantesTecnicos, TramitesRepresentanteTecnicoDTO>(element);

            return result;
        }
        public async Task TomarTramiteAsync(int IdTramite)
        {
            var uof = _uowFactory.GetUnitOfWork();
            var repo = new TramitesRepository(uof);
            var userid = await _usuarioBL.GetCurrentUserid();
            
            var element = await repo.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            if (element != null)
            {
                element.UsuarioAsignado = userid;
                element.FechaAsignacion = DateTime.Now;
                await repo.UpdateAsync(element);
            }
            else
                throw new Exception($"No se encontró el trámite {IdTramite}");
            
        }
        public async Task<bool> ComprobarSeguridad(string IdentificadorUnico)
        {
            bool result = false;
            
            string userid = await _usuarioBL.GetCurrentUserid();
            var lstPerfiles = await _usuarioBL.GetPerfilesForUserAsync(userid);
            var uof = _uowFactory.GetUnitOfWork();
            var repo = new TramitesRepository(uof);
            var tramiteEntity = await repo.FirstOrDefaultAsync(x => x.IdentificadorUnico == IdentificadorUnico);

            if (tramiteEntity != null)
            {
                if ( lstPerfiles.Select(s=> s.IdPerfil).Contains(Constants.Perfiles.Administrador) ||
                     lstPerfiles.Select(s => s.IdPerfil).Contains(Constants.Perfiles.Evaluador) )
                {
                        result = true;
                }
                else
                {
                    //Cuando es perfil empresa 
                    int userIdEmpresa = 0;
                    int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(),out userIdEmpresa);

                    //permite acceder si es el usuario que lo creo
                    result = (tramiteEntity.IdEmpresa == userIdEmpresa);

                }
            }
            return result;

        }
        public async Task<bool> isEvaluandoTramite(string IdentificadorUnico)
        {
            bool result = false;
            string userid = await _usuarioBL.GetCurrentUserid();
            var lstPerfiles = await _usuarioBL.GetPerfilesForUserAsync(userid);
            var uof = _uowFactory.GetUnitOfWork();
            var repo = new TramitesRepository(uof);
            var tramiteEntity = await repo.FirstOrDefaultAsync(x => x.IdentificadorUnico == IdentificadorUnico);
            if (tramiteEntity != null)
            {
                if (tramiteEntity.IdEstado == Constants.TramitesEstados.EnEvaluacion && tramiteEntity.UsuarioAsignado == userid 
                    && (lstPerfiles.Select(s => s.IdPerfil).Contains(Constants.Perfiles.Evaluador) ||
                         lstPerfiles.Select(s => s.IdPerfil).Contains(Constants.Perfiles.Administrador)
                        ))
                {
                    result = true;
                }
            }
            return result;
        }
        public async Task<List<InformeRepresentanteTecnicoDTO>> GetInformeRepresentantesTecnicos1Async(FiltroInformeResponsablesTecnicosDTO filtro, CancellationToken cancellationToken = default)
        {
            var repo = new TramitesRepresentantesTecnicosRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.GetInforme1(filtro).ToListAsync(cancellationToken);
            return _mapper.Map<List<InformeRepresentanteTecnicoDTO>>(elements);
        }
        public async Task<DateTime> GetFechaVencimientoConstanciaInscripcion(int IdTramite)
        {
            //devuelve la fecha ed vencimiento de la constancia de inscripcion del tramite actual si es de tipo Inscripcion
            //o el inmediato anterior aprobado.
            DateTime result = DateTime.Now.Date.AddMonths(18);

            var uof = _uowFactory.GetUnitOfWork();
            var repoTramites = new TramitesRepository(uof);
            var repoBalances = new TramitesBalancesGeneralesRepository(uof);

            var tramiteEntity = await repoTramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);

            //Se busca el tramite de inscipcion actual o inmediato anterior de la misma empresa
            tramiteEntity = await repoTramites.Where(x => x.IdTramite <= tramiteEntity.IdTramite && 
                                                     (x.IdTipoTramite == Constants.TiposDeTramite.Reli_Inscripcion 
                                                     || x.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCompleta)
                                                     && x.IdEmpresa == tramiteEntity.IdEmpresa &&
                                                     x.IdEstado == Constants.TramitesEstados.Aprobado
                                                     ).OrderByDescending(o=> o.IdTramite)
                                                   .FirstOrDefaultAsync();

            if (tramiteEntity != null)
            {
                //Busca la ultima fecha de los balances y le suma 540 dias (18 meses)
                var BalanceEntity = await repoBalances.Where(x => x.IdTramite == tramiteEntity.IdTramite)
                                                      .OrderByDescending(o => o.FechaBalance)
                                                      .FirstOrDefaultAsync();
                if (BalanceEntity != null)
                    result = BalanceEntity.FechaBalance.AddMonths(18);
            }

            return result;
        }
        public async Task<DateTime?> GetFechaPresentacionAsync(int IdTramite)
        {
            var uof = _uowFactory.GetUnitOfWork();
            var repoTramites = new TramitesRepository(uof);

            return await repoTramites.GetFechaPresentacionAsync(IdTramite);
        }
        public async Task<DateTime?> GetFechaCertificacionAsync(int IdTramite)
        {
            //edvuelve el ultimo dia del mes anterior a la fecha de presentacion de la solicitud.
            DateTime? result = null;
            using var uow = _uowFactory.GetUnitOfWork();
            var repoHistorial = new TramitesHistorialEstadosRepository(uow);


            var entity = await repoHistorial.Where(x => x.IdTramite == IdTramite && x.CodEstadoNuevo == Constants.TramitesEstadosCodigos.EnEvaluacion)
                                             .OrderBy(o => o.CreateDate)
                                             .FirstOrDefaultAsync();

            if (entity != null)
            {
                result = new DateTime(entity.CreateDate.Year, entity.CreateDate.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(-1);
            }
            return result;
        }
        public bool PermiteEditarFormulario(int IdTipoTramite, Constants.Formularios form)
        {
            bool result = false;

            if (IdTipoTramite == Constants.TiposDeTramite.Reli_Inscripcion ||
                IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCompleta ||
                IdTipoTramite == Constants.TiposDeTramite.Reco_Inscripcion
                )
                result = true;
            else if (IdTipoTramite == Constants.TiposDeTramite.Reli_Licitar &&
                (form == Constants.Formularios.BoletaPago || form == Constants.Formularios.ObrasEnEjecucion))
                result = true;
            else if (IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionSoloTecnicos &&
                (form == Constants.Formularios.RepresentantesTecnicos))
                result = true;
            else if (IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCapacidadTecnica &&
                (form == Constants.Formularios.RepresentantesTecnicos || form == Constants.Formularios.Equipos
                    || form == Constants.Formularios.Obras))
                result = true;

            return result;

        }

        private async Task CopiarEstructuraTramiteAEmpresaAsync(IUnitOfWork uow,
                                                                Tramites tramite,
                                                                Empresas empresa,
                                                                string userid,
                                                                bool esTramiteLicitadores)
        {
            // Crear todos los repositorios necesarios dentro del método
            var repoEmpresas = new EmpresasRepository(uow);
            var repoDeudas = new EmpresasDeudasRepository(uow);
            var repoEspecialidades = new EmpresasEspecialidadesRepository(uow);
            var repoEspecialidadesSecciones = new EmpresasEspecialidadesSeccionesRepository(uow);
            var repoEspecialidadesTareas = new EmpresasEspecialidadesTareasRepository(uow);
            var repoEspecialidadesEquipos = new EmpresasEspecialidadesEquiposRepository(uow);
            var repoRepresentantesTecnicos = new EmpresasRepresentantesTecnicosRepository(uow);
            var repoRepresentantesTecnicosEsp = new EmpresasRepresentantesTecnicosEspecialidadesRepository(uow);
            var repoRepresentantesTecnicosJur = new EmpresasRepresentantesTecnicosJurisdiccionesRepository(uow);
            var repoRepresentantesTecnicosDesvinculaciones = new EmpresasRepresentantesTecnicosDesvinculacionesRepository(uow);
            var repoEmpresasBalances = new EmpresasBalancesGeneralesRepository(uow);
            var repoEmpresasBalancesEvaluar = new EmpresasBalancesGeneralesEvaluarRepository(uow);
            var repoEmpresasConstanciaCap = new EmpresasConstanciaCapRepository(uow);
            var repoEmpresasCapTecnica = new EmpresasCapTecnicaRepository(uow);
            var repoEmpresasCapEjec = new EmpresasCapEjecRepository(uow);
            var repoEmpresasCapProd = new EmpresasCapProdRepository(uow);
            var repoEmpresasCapTecxEquipo = new EmpresasCapTecxEquipoRepository(uow);
            var repoEmpresasDetalleObras = new EmpresasDetalleObrasEjecucionRepository(uow);
            var repoEmpresasInfDocumentos = new EmpresasInfDocumentosRepository(uow);
            var repoEmpresasEquipos = new EmpresasEquiposRepository(uow);
            var repoEmpresasBienesRaices = new EmpresasBienesRaicesRepository(uow);
            var repoEmpresasObras = new EmpresasObrasRepository(uow);
            var repoEmpresasAntecedentes = new EmpresasAntecedentesRepository(uow);
            var repoEmpresasAntecedentesDdjj = new EmpresasAntecedentesDdjjmensualRepository(uow);
            var repoEmpresasObrasEjecucion = new EmpresasObrasEjecucionRepository(uow);
            var repoEmpresasInf = new EmpresasInfRepository(uow);

            // Actualizar datos básicos de la empresa según el tipo de trámite
            if (esTramiteLicitadores)
            {
                // Licitadores
                if (tramite.FechaVencimiento.HasValue)
                    empresa.Vencimiento = tramite.FechaVencimiento;

                if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_Inscripcion)
                    empresa.FechaInscripcion = DateTime.Now;

                empresa.IdTramiteOrigen = tramite.IdTramite;
                empresa.FechaBalance = tramite.TramitesBalancesGenerales.Max(m => m.FechaBalance);
                empresa.LastUpdateDate = DateTime.Now;
                empresa.LastUpdateUser = userid;
            }
            else
            {
                // Consultores
                if (tramite.FechaVencimiento.HasValue)
                    empresa.VencimientoReco = tramite.FechaVencimiento;

                if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reco_Inscripcion)
                    empresa.FechaInscripcionReco = DateTime.Now;

                empresa.AniosAntiguedadReco = tramite.TramitesInfEmpCons.AniosDuracionSoc;
                empresa.IdTramiteOrigenReco = tramite.IdTramite;
                empresa.LastUpdateDate = DateTime.Now;
                empresa.LastUpdateUser = userid;
            }


            // Eliminar datos anteriores de la empresa
            await repoEmpresasInf.RemoveRangeAsync(empresa.EmpresasInf);
            await repoDeudas.RemoveRangeAsync(empresa.EmpresasDeudas);

            // Eliminar representantes técnicos
            foreach (var item in empresa.EmpresasRepresentantesTecnicos)
            {
                await repoRepresentantesTecnicosEsp.RemoveRangeAsync(item.EmpresasRepresentantesTecnicosEspecialidades);
                await repoRepresentantesTecnicosJur.RemoveRangeAsync(item.EmpresasRepresentantesTecnicosJurisdicciones);
            }
            await repoRepresentantesTecnicos.RemoveRangeAsync(empresa.EmpresasRepresentantesTecnicos);

            //Eliminar Obras
            await repoEmpresasObras.RemoveRangeAsync(empresa.EmpresasObras);

            // Eliminar antecedentes y sus DDJJ mensuales
            foreach (var antecedente in empresa.EmpresasAntecedentes)
            {
                await repoEmpresasAntecedentesDdjj.RemoveRangeAsync(antecedente.EmpresasAntecedentesDdjjmensual);
            }
            await repoEmpresasAntecedentes.RemoveRangeAsync(empresa.EmpresasAntecedentes);
            //Eliminar Obras en Ejecucion
            await repoEmpresasObrasEjecucion.RemoveRangeAsync(empresa.EmpresasObrasEjecucion);

            // Eliminar Especialidades de la Empresa
            foreach (var especialidad in empresa.EmpresasEspecialidades)
            {
                foreach (var seccion in especialidad.EmpresasEspecialidadesSecciones)
                {
                    foreach (var tarea in seccion.EmpresasEspecialidadesTareas)
                    {
                        await repoEspecialidadesEquipos.RemoveRangeAsync(tarea.EmpresasEspecialidadesEquipos);
                    }
                    await repoEspecialidadesTareas.RemoveRangeAsync(seccion.EmpresasEspecialidadesTareas);
                }
                await repoEspecialidadesSecciones.RemoveRangeAsync(especialidad.EmpresasEspecialidadesSecciones);
            }
            await repoEspecialidades.RemoveRangeAsync(empresa.EmpresasEspecialidades);

            // Eliminar Balances Generales anteriores de la Empresa
            await repoEmpresasBalances.RemoveRangeAsync(empresa.EmpresasBalancesGenerales);

            // Eliminar EmpresasBalancesGeneralesEvaluar anteriores
            await repoEmpresasBalancesEvaluar.RemoveRangeAsync(empresa.EmpresasBalancesGeneralesEvaluar);

            // Eliminar tablas hijas de evaluación (Constancia, CapTecnica, etc.)
            await repoEmpresasConstanciaCap.RemoveRangeAsync(empresa.EmpresasConstanciaCap);
            await repoEmpresasCapTecnica.RemoveRangeAsync(empresa.EmpresasCapTecnica);
            await repoEmpresasCapEjec.RemoveRangeAsync(empresa.EmpresasCapEjec);
            await repoEmpresasCapProd.RemoveRangeAsync(empresa.EmpresasCapProd);
            await repoEmpresasCapTecxEquipo.RemoveRangeAsync(empresa.EmpresasCapTecxEquipo);
            await repoEmpresasDetalleObras.RemoveRangeAsync(empresa.EmpresasDetalleObrasEjecucion);

            // Eliminar documentos, equipos y bienes raíces
            await repoEmpresasInfDocumentos.RemoveRangeAsync(empresa.EmpresasInfDocumentos);
            await repoEmpresasEquipos.RemoveRangeAsync(empresa.EmpresasEquipos);
            await repoEmpresasBienesRaices.RemoveRangeAsync(empresa.EmpresasBienesRaices);
           

            // Copiar datos de las deudas a la empresa
            if (tramite.TramitesInfEmp != null)
            {
                var EmpresaInfEntity = _mapper.Map<EmpresasInf>(tramite.TramitesInfEmp);
                EmpresaInfEntity.CreateDate = DateTime.Now;
                EmpresaInfEntity.CreateUser = userid;
                empresa.EmpresasInf = new List<EmpresasInf> { EmpresaInfEntity };

                empresa.EmpresasDeudas = tramite.TramitesInfEmp.TramitesInfEmpDeudas.Select(s => new EmpresasDeudas
                {
                    IdGrupoTramite = esTramiteLicitadores ? Constants.GruposDeTramite.RegistroLicitadores : Constants.GruposDeTramite.RegistroConsultores,
                    Entidad = s.Entidad,
                    Periodo = s.Periodo,
                    Monto = s.Monto,
                    DiasDeAtraso = s.DiasDeAtraso,
                    CreateDate = DateTime.Now,
                    CreateUser = userid
                }).ToList();
            }

            // Copiar las especialidades del trámite a la empresa
            empresa.EmpresasEspecialidades = tramite.TramitesEspecialidades.Select(s => new EmpresasEspecialidades
            {
                IdGrupoTramite = esTramiteLicitadores ? Constants.GruposDeTramite.RegistroLicitadores : Constants.GruposDeTramite.RegistroConsultores,
                IdEspecialidad = s.IdEspecialidad,
                CreateDate = DateTime.Now,
                EmpresasEspecialidadesSecciones = s.TramitesEspecialidadesSecciones.Select(ss => new EmpresasEspecialidadesSecciones
                {
                    IdSeccion = ss.IdSeccion,
                    CreateDate = DateTime.Now,
                    EmpresasEspecialidadesTareas = ss.TramitesEspecialidadesTareas.Select(st => new EmpresasEspecialidadesTareas
                    {
                        IdTarea = st.IdTarea,
                        EmpresasEspecialidadesEquipos = st.TramitesEspecialidadesEquipos.Select(se => new EmpresasEspecialidadesEquipos
                        {
                            IdEquipo = se.IdEquipo,
                            CreateDate = DateTime.Now,
                        }).ToList(),
                    }).ToList(),
                }).ToList(),
            }).ToList();

            // Se necesita actualizar para obtener los ids que luego serán utilizados en Representantes técnicos
            await repoEmpresas.UpdateAsync(empresa);

            // Copiar los representantes técnicos del trámite a la empresa
            empresa.EmpresasRepresentantesTecnicos = tramite.TramitesRepresentantesTecnicos
                .Where(x => x.TramitesRepresentantesTecnicosDesvinculaciones == null) // desestima los que se están desvinculando en el trámite
                .Select(s => new EmpresasRepresentantesTecnicos
                {
                    IdGrupoTramite = esTramiteLicitadores ? Constants.GruposDeTramite.RegistroLicitadores : Constants.GruposDeTramite.RegistroConsultores,
                    Apellido = s.Apellido,
                    Nombres = s.Nombres,
                    Cuit = s.Cuit,
                    Cargo = s.Cargo,
                    Matricula = s.Matricula,
                    FechaVencimientoMatricula = s.FechaVencimientoMatricula,
                    FechaVencimientoContrato = s.FechaVencimientoContrato,
                    IdFileContrato = s.IdFileContrato,
                    FilenameContrato = s.FilenameContrato,
                    IdFileBoleta = s.IdFileBoleta,
                    FilenameBoleta = s.FilenameBoleta,
                    IdFileMatricula = s.IdFileMatricula,
                    FilenameMatricula = s.FilenameMatricula,
                    CreateDate = DateTime.Now,
                    CreateUser = userid,
                    EmpresasRepresentantesTecnicosEspecialidades = s.TramitesRepresentantesTecnicosEspecialidades.Select(se => new EmpresasRepresentantesTecnicosEspecialidades
                    {
                        IdEmpresaEspecialidad = empresa.EmpresasEspecialidades.First(x => x.IdEspecialidad == se.IdTramiteEspecialidadNavigation.IdEspecialidad).IdEmpresaEspecialidad,
                    }).ToList(),
                    EmpresasRepresentantesTecnicosJurisdicciones = s.TramitesRepresentantesTecnicosJurisdicciones.Select(sj => new EmpresasRepresentantesTecnicosJurisdicciones
                    {
                        IdProvincia = sj.IdProvincia,
                    }).ToList(),
                }).ToList();

            // Copiar los representantes técnicos que se desvincularon en el trámite a la tabla de desvinculaciones de la empresa para mantener el historial
            if (tramite.TramitesRepresentantesTecnicos.Any() && tramite.TramitesRepresentantesTecnicos.Where(x => x.TramitesRepresentantesTecnicosDesvinculaciones != null).Any())
            {
                await repoRepresentantesTecnicosDesvinculaciones.AddARangeAsync(tramite.TramitesRepresentantesTecnicos
                    .Where(x => x.TramitesRepresentantesTecnicosDesvinculaciones != null)
                    .Select(s => new EmpresasRepresentantesTecnicosDesvinculaciones
                    {
                        IdGrupoTramite = esTramiteLicitadores ? Constants.GruposDeTramite.RegistroLicitadores : Constants.GruposDeTramite.RegistroConsultores,
                        Apellido = s.Apellido,
                        Nombres = s.Nombres,
                        Cuit = s.Cuit,
                        Cargo = s.Cargo,
                        Matricula = s.Matricula,
                        FechaVencimientoMatricula = s.FechaVencimientoMatricula,
                        FechaVencimientoContrato = s.FechaVencimientoContrato,
                        IdFileContrato = s.IdFileContrato,
                        FilenameContrato = s.FilenameContrato,
                        IdFileBoleta = s.IdFileBoleta,
                        FilenameBoleta = s.FilenameBoleta,
                        IdFileMatricula = s.IdFileMatricula,
                        FilenameMatricula = s.FilenameMatricula,
                        IdEmpresa = empresa.IdEmpresa,
                        IdFileDesvinculacion = s.TramitesRepresentantesTecnicosDesvinculaciones.IdFileDesvinculacion,
                        FilenameDesvinculacion = s.TramitesRepresentantesTecnicosDesvinculaciones.FilenameDesvinculacion,
                        CreateDate = DateTime.Now,
                        CreateUser = userid
                    }).ToList());
            }

            // Copiar el último Balance General del trámite a la empresa
            var ultimoBalanceTramite = tramite.TramitesBalancesGenerales
                .OrderByDescending(b => b.Anio)
                .ThenByDescending(b => b.FechaBalance)
                .FirstOrDefault();

            if (ultimoBalanceTramite != null)
            {
                // Mapear el balance principal usando AutoMapper
                var empresaBalance = _mapper.Map<EmpresasBalancesGenerales>(ultimoBalanceTramite);
                empresaBalance.CreateDate = DateTime.Now;
                empresaBalance.CreateUser = userid;

                empresa.EmpresasBalancesGenerales = new List<EmpresasBalancesGenerales> { empresaBalance };
            }

            // Copiar el último TramitesBalancesGeneralesEvaluar
            var ultimaEvaluacionTramite = ultimoBalanceTramite?.TramitesBalancesGeneralesEvaluar;

            if (ultimaEvaluacionTramite != null)
            {
                // Mapear la evaluación usando AutoMapper
                var empresaEvaluacion = _mapper.Map<EmpresasBalancesGeneralesEvaluar>(ultimaEvaluacionTramite);
                empresaEvaluacion.CreateDate = DateTime.Now;
                empresaEvaluacion.CreateUser = userid;

                empresa.EmpresasBalancesGeneralesEvaluar = new List<EmpresasBalancesGeneralesEvaluar> { empresaEvaluacion };

                // Copiar las tablas hijas de la evaluación

                // Copiar Constancia de Capacidad
                if (ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarConstanciaCap?.Any() == true)
                {
                    empresa.EmpresasConstanciaCap = ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarConstanciaCap
                        .Select(cc => _mapper.Map<EmpresasConstanciaCap>(cc))
                        .ToList();
                }

                // Copiar Capacidad Técnica
                if (ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarCapTecnica?.Any() == true)
                {
                    empresa.EmpresasCapTecnica = ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarCapTecnica
                        .Select(ct => _mapper.Map<EmpresasCapTecnica>(ct))
                        .ToList();
                }

                // Copiar Capacidad de Ejecución
                if (ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarCapEjec?.Any() == true)
                {
                    empresa.EmpresasCapEjec = ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarCapEjec
                        .Select(ce => _mapper.Map<EmpresasCapEjec>(ce))
                        .ToList();
                }

                // Copiar Capacidad de Producción
                if (ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarCapProd?.Any() == true)
                {
                    empresa.EmpresasCapProd = ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarCapProd
                        .Select(cp => _mapper.Map<EmpresasCapProd>(cp))
                        .ToList();
                }

                // Copiar Capacidad Técnica por Equipo
                if (ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarCapTecxEquipo?.Any() == true)
                {
                    empresa.EmpresasCapTecxEquipo = ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarCapTecxEquipo
                        .Select(cte => _mapper.Map<EmpresasCapTecxEquipo>(cte))
                        .ToList();
                }

                // Copiar Detalle de Obras en Ejecución
                if (ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarDetalleObrasEjecucion?.Any() == true)
                {
                    empresa.EmpresasDetalleObrasEjecucion = ultimaEvaluacionTramite.TramitesBalancesGeneralesEvaluarDetalleObrasEjecucion
                        .Select(doe => _mapper.Map<EmpresasDetalleObrasEjecucion>(doe))
                        .ToList();
                }
            }

            // Copiar Documentos de Información de Empresa
            if (tramite.TramitesInfEmp?.TramitesInfEmpDocumentos?.Any() == true)
            {
                empresa.EmpresasInfDocumentos = tramite.TramitesInfEmp.TramitesInfEmpDocumentos
                    .Select(doc =>
                    {
                        var empresaDoc = _mapper.Map<EmpresasInfDocumentos>(doc);
                        empresaDoc.CreateDate = DateTime.Now;
                        empresaDoc.CreateUser = userid;
                        return empresaDoc;
                    })
                    .ToList();
            }

            // Copiar Equipos
            if (tramite.TramitesEquipos?.Any() == true)
            {
                empresa.EmpresasEquipos = tramite.TramitesEquipos
                    .Select(eq =>
                    {
                        var empresaEquipo = _mapper.Map<EmpresasEquipos>(eq);
                        empresaEquipo.CreateDate = DateTime.Now;
                        empresaEquipo.CreateUser = userid;
                        return empresaEquipo;
                    })
                    .ToList();
            }

            // Copiar Bienes Raíces
            if (tramite.TramitesBienesRaices?.Any() == true)
            {
                empresa.EmpresasBienesRaices = tramite.TramitesBienesRaices
                    .Select(br =>
                    {
                        var empresaBienRaiz = _mapper.Map<EmpresasBienesRaices>(br);
                        empresaBienRaiz.CreateDate = DateTime.Now;
                        empresaBienRaiz.CreateUser = userid;
                        return empresaBienRaiz;
                    })
                    .ToList();
            }

            // Copiar Obras
            if (tramite.TramitesObras?.Any() == true)
            {
                empresa.EmpresasObras = tramite.TramitesObras
                    .Select(obra =>
                    {
                        var empresaObra = _mapper.Map<EmpresasObras>(obra);

                        // Asignar manualmente las FK buscando en el contexto de la empresa
                        empresaObra.IdEmpresaEspecialidad = empresa.EmpresasEspecialidades
                            .First(x => x.IdEspecialidad == obra.IdTramiteEspecialidadNavigation.IdEspecialidad)
                            .IdEmpresaEspecialidad;

                        empresaObra.IdEmpresaEspecialidadSeccion = empresa.EmpresasEspecialidades
                            .SelectMany(sm => sm.EmpresasEspecialidadesSecciones)
                            .First(x => x.IdSeccion == obra.IdTramiteEspecialidadSeccionNavigation.IdSeccion)
                            .IdEmpresaEspecialidadSeccion;

                        empresaObra.CreateDate = DateTime.Now;
                        empresaObra.CreateUser = userid;

                        return empresaObra;
                    })
                    .ToList();
            }

            // Copiar Antecedentes de Producción
            if (tramite.TramitesAntecedentes?.Any() == true)
            {
                empresa.EmpresasAntecedentes = tramite.TramitesAntecedentes
                    .Select(ant =>
                    {
                        var empresaAntecedente = _mapper.Map<EmpresasAntecedentes>(ant);

                        // Asignar manualmente las FK buscando en el contexto de la empresa
                        empresaAntecedente.IdEmpresaEspecialidad = empresa.EmpresasEspecialidades
                            .First(x => x.IdEspecialidad == ant.IdTramiteEspecialidadNavigation.IdEspecialidad)
                            .IdEmpresaEspecialidad;

                        empresaAntecedente.IdEmpresaEspecialidadSeccion = empresa.EmpresasEspecialidades
                            .SelectMany(sm => sm.EmpresasEspecialidadesSecciones)
                            .First(x => x.IdSeccion == ant.IdTramiteEspecialidadSeccionNavigation.IdSeccion)
                            .IdEmpresaEspecialidadSeccion;

                        empresaAntecedente.CreateDate = DateTime.Now;
                        empresaAntecedente.CreateUser = userid;

                        return empresaAntecedente;
                    })
                    .ToList();
            }

            // Copiar Obras en Ejecución
            if (tramite.TramitesObrasEjecucion?.Any() == true)
            {
                empresa.EmpresasObrasEjecucion = tramite.TramitesObrasEjecucion
                    .Select(oe =>
                    {
                        var empresaObraEjec = _mapper.Map<EmpresasObrasEjecucion>(oe);
                        empresaObraEjec.CreateDate = DateTime.Now;
                        empresaObraEjec.CreateUser = userid;
                        return empresaObraEjec;
                    })
                    .ToList();
            }
        }

        public async Task ActualizarEnUte(int IdTramite,bool SePresentaEnUte, decimal? PorcParticipUte)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            var repoTramites = new TramitesRepository(_uowFactory.GetUnitOfWork());
            var entityTramite = await repoTramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);

            if (entityTramite != null)
            {
                entityTramite.SePresentaEnUte = SePresentaEnUte;
                entityTramite.PorcParticipUte = PorcParticipUte;
                entityTramite.LastUpdateDate = DateTime.Now;
                entityTramite.LastUpdateUser = userid;
                await repoTramites.UpdateAsync(entityTramite);
            }
            else
            {     
                throw new Exception($"No se encontró el trámite {IdTramite}");
            }

        }
    }
}
