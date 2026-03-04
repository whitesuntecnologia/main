using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess;
using DataAccess.Entities;
using DataTransferObject;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class NotificacionesBL : INotificacionesBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;
        public NotificacionesBL(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));
            _usuarioBL = usuarioBL;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TramitesNotificaciones, NotificacionDTO>();
                cfg.CreateMap<NotificacionDTO, TramitesNotificaciones>();

            });
            _mapper = config.CreateMapper();
        }
        
        public async Task<List<NotificacionDTO>> GetNotificacionesPorUsuario(string userid)
        {
            List<NotificacionDTO> lstResult = new List<NotificacionDTO>();
            var uow = _uowFactory.GetUnitOfWork();
            var repoNotificaciones = new TramitesNotificacionesRepository(uow);
            if(userid != null)
            {
                var lstEntity = await repoNotificaciones.GetNotificacionPorUsuario(userid).OrderByDescending(o=> o.IdNotificacion).ToListAsync();
                 lstResult =_mapper.Map<List<NotificacionDTO>>(lstEntity);
            }

            return lstResult;
        }

        public async Task<List<NotificacionDTO>> GetNotificaciones()
        {
            List<NotificacionDTO> lstResult = new List<NotificacionDTO>();
            var uow = _uowFactory.GetUnitOfWork();
            var repoNotificaciones = new TramitesNotificacionesRepository(uow);
            var lstEntity = await repoNotificaciones.DbSet.OrderByDescending(o=> o.IdNotificacion).ToListAsync();
            lstResult = _mapper.Map<List<NotificacionDTO>>(lstEntity);
            
            return lstResult;
        }
        public async Task MarcarComoLeido(int IdNotificacion)
        {
            var userid = await _usuarioBL.GetCurrentUserid();
            var uow = _uowFactory.GetUnitOfWork();
            var repoNotificaciones = new TramitesNotificacionesRepository(uow);

            var entity = await repoNotificaciones.FirstOrDefaultAsync(x => x.IdNotificacion == IdNotificacion);

            entity.Leido = true;
            entity.LastUpdateDate = DateTime.Now;
            entity.LastupdateUser = userid;

            await repoNotificaciones.UpdateAsync(entity);
        }
        public async Task Notificar(int IdTramite, string Titulo, string Mensaje)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            string userid = await _usuarioBL.GetCurrentUserid();
            var repoNotificaciones = new TramitesNotificacionesRepository(uow);

            var notificacion = new TramitesNotificaciones
            {
                IdTramite = IdTramite,
                Titulo = Titulo,
                Mensaje = Mensaje,
                CreateDate = DateTime.Now,
                CreateUser = userid
            };
            await repoNotificaciones.AddAsync(notificacion);
        }

        public async Task<List<AlertaDTO>> GetAlertas( int? IdEmpresa = null)
        {
            List<AlertaDTO> lstResult = new List<AlertaDTO>();
            var uow = _uowFactory.GetUnitOfWork();
            var repoEmpresas = new EmpresasRepository(uow);


            var q = repoEmpresas.Where(x => x.Vencimiento.HasValue
                               && x.Vencimiento.Value <= DateTime.Now.Date.AddDays(60));

            if (IdEmpresa.HasValue) 
            {
                q = q.Where(x => x.IdEmpresa == IdEmpresa.Value);
            }

            lstResult = await q.Select(s => new AlertaDTO
            {
                IdEmpresa = s.IdEmpresa,
                RazonSocial = s.RazonSocial,
                Mensaje = (s.Vencimiento < DateTime.Now.Date ? "El registro de empresa se encuentra vencido." : $"El registro de empresa está próximo a vencerse."),
                Vencimiento = s.Vencimiento.Value
            }).ToListAsync();
            
            return lstResult;
        }
    }
}
