using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class MenuesBL: IMenuesBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;

        public MenuesBL(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Menues, MenuDTO>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => "M"))
                .ForMember(dest => dest.MenuPadre, opt => opt.MapFrom(src => src.IdMenuPadreNavigation))
                .ForMember(dest=> dest.SubMenues, opt => opt.MapFrom( src => src.InverseIdMenuPadreNavigation))
                .ForMember(dest=> dest.Permisos, opt => opt.MapFrom( src => src.MenuesPermisos.Select(s=> s.IdPermisoNavigation)))

                ;
                cfg.CreateMap<Permisos, PermisoDTO>().ReverseMap();

            });
            _mapper = config.CreateMapper();
        }

        public async Task<List<MenuDTO>> GetMenuesAsync(int IdMenuPadre, CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new MenuesRepository(uow);
            var elements = await repo.DbSet.Where(x=> x.IdMenuPadre == IdMenuPadre && x.Visible)
                                           .OrderBy(x=>x.Orden.GetValueOrDefault()) 
                                           .ToListAsync(cancellationToken);
            return _mapper.Map<List<Menues>, List<MenuDTO>>(elements);
        }

        public async Task<List<MenuDTO>> GetMenuesAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new MenuesRepository(uow);
            var elements = await repo.DbSet.Where(x=> x.Visible && x.IdMenuPadre == 0)
                                           .OrderBy(x => x.Orden.GetValueOrDefault()) 
                                           .ToListAsync(cancellationToken);

            var menues = _mapper.Map<List<Menues>, List<MenuDTO>>(elements);

            return menues;
        }


    }
}
