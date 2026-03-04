using Business;
using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using StaticClass;

namespace Website.Pages.Informes.Empresas
{
    public partial class Detalle: ComponentBase
    {
        [Parameter] public int IdEmpresa{ get; set; }

        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IEmpresasBL _empresaBL { get; set; } = null!;

        private EmpresaDTO Empresa { get; set; } = null!;
        private bool isBusyInitialLoading { get; set; } = true;
        protected override async Task OnInitializedAsync()
        {
            Empresa = await _empresaBL.GetEmpresaAsync(IdEmpresa);
            Empresa.Especialidades  = Empresa.Especialidades.Where(x=> x.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores).ToList();
            Empresa.Deudas = Empresa.Deudas.Where(x => x.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores).ToList();
            Empresa.RepresentantesTecnicos = Empresa.RepresentantesTecnicos.Where(x => x.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores).ToList();
            isBusyInitialLoading = false;
        }
    }
}
