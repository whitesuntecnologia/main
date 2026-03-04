using AutoMapper;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Website.Pages
{
    public partial class Index : ComponentBase
    {
        [Parameter] public int? IdMenu { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IMenuesBL menuesBL { get; set; } = null!;
        [Inject] private IPerfilesBL perfilesBL { get; set; } = null!;

        [Inject] private IUsuariosBL usuarioBL { get; set; } = null!;
        [Inject] private IJSRuntime jsRuntime { get; set; } = null!;

        private List<MenuDTO> lstMenues { get; set; } = new();
        private string? Titulo { get; set; }
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyInitialLoading = true;

        protected override async Task OnInitializedAsync()
        {
            
            string userid = await usuarioBL.GetCurrentUserid();

            //Obtiene los menues de la DB
            
            
            if (IdMenu.HasValue)
            {
                lstMenues = await menuesBL.GetMenuesAsync(IdMenu.Value);
            }
            else
            {
                lstMenues = await menuesBL.GetMenuesAsync();
            }

            // Obtiene los permisos del perfil
            var lstPermisos = await perfilesBL.GetPermisosAsync(userid);
            await AgregarPermisosEnMenues(lstMenues, lstPermisos);
            lstMenues = await QuitarMenuesSinPermiso(lstMenues);


            if (lstMenues != null && lstMenues.Count() > 0)
                Titulo = lstMenues.First().MenuPadre?.TituloHijos;
            

            await base.OnInitializedAsync();
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender) 
            {
                isBusyInitialLoading = false;
            }
            return base.OnAfterRenderAsync(firstRender);
        }
        private Task MenuClick(MenuDTO menu)
        {
            if (!string.IsNullOrWhiteSpace(menu.Url))
                if (menu.Url.StartsWith("http"))
                    jsRuntime.InvokeVoidAsync("open", menu.Url, "_blank");
                else
                    navigationManager.NavigateTo(menu.Url, true);

            return Task.CompletedTask;
        }

        private Task BreadClick()
        {
            navigationManager.NavigateTo("/", true);

            return Task.CompletedTask;
        }

        private async Task<MenuDTO?> BuscarMenu(int IdMenu, string Tipo, List<MenuDTO> lst)
        {
            MenuDTO? result = null;
            foreach (var menu in lst)
            {
                if (menu.IdMenu == IdMenu && menu.Tipo == Tipo)
                {
                    result = menu;
                    if (result != null) break;
                }
                else if (menu.SubMenues.Count > 0)
                {
                    result = await BuscarMenu(IdMenu, Tipo, menu.SubMenues);
                    if (result != null) break;
                }
            }

            return result;
        }

        private async Task AgregarPermisosEnMenues(List<MenuDTO> menues, List<PermisoDTO> permisos)
        {
            foreach (var permiso in permisos)
            {
                foreach (var IdMenu in permiso.Menues)
                {
                    var menu = await BuscarMenu(IdMenu, "M", menues);
                    if (menu != null)
                    {
                        if (!menu.SubMenues.Exists(x => x.IdMenu == permiso.IdPermiso))
                        {
                            menu.SubMenues.Add(new MenuDTO()
                            {
                                IdMenu = permiso.IdPermiso,
                                Descripcion = permiso.Nombre + " (" + permiso.Codigo + ")",
                                Tipo = "P"
                            });
                        }
                    }
                }
            }

        }
        private async Task<List<MenuDTO>> QuitarMenuesSinPermiso(List<MenuDTO> menues)
        {
            foreach (var menu in menues.ToList())
            {
                if (menu.SubMenues.Count > 0)
                {
                    menu.SubMenues = await QuitarMenuesSinPermiso(menu.SubMenues);
                    if(menu.SubMenues.Count == 0)
                        menues.Remove(menu);
                }
                else if (menu.Tipo == "M")
                    menues.Remove(menu);
            }
            return menues;
        }
       
    }
}
