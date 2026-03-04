using AutoMapper;
using Business;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using System;
using System.Linq;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Website.Pages.Seguridad.Perfiles
{
    public partial class Editar: ComponentBase
    {
        [Parameter] public int IdPerfil { get; set; }
        
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;

        [Inject] private IMenuesBL menuesBL { get; set; } = null!;
        [Inject] private IPerfilesBL perfilesBL { get; set; } = null!;
        private List<MenuTreeDTO> lstMenues { get; set; } = new();
        private IEnumerable<object> CheckedValues { get; set; } = null!;
        private PerfilDTO? perfilDTO { get; set; } = null!;
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MenuDTO, MenuTreeDTO>()
                    .ForMember(dest => dest.Id , opt => opt.MapFrom( src => src.IdMenu))
                    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => "M"))
                ;
            });
            _mapper = config.CreateMapper();
            //--

            perfilDTO = await perfilesBL.GetPerfilAsync(IdPerfil);

            //Obtiene los menues de la DB
            var menues = (await menuesBL.GetMenuesAsync()).ToList();
            lstMenues = _mapper.Map<List<MenuDTO>, List<MenuTreeDTO>>(menues);
            
            // Obtiene los permisos del perfil
            var lstPermisos = await perfilesBL.GetPermisosAsync();
            AgregarPermisosEnMenues(lstMenues, lstPermisos);

            // Obtiene los permisos guardados anteriormente
            var lstPermisosxPerfil = await perfilesBL.GetPermisosByPerfilAsync(IdPerfil);

            //Tilda los permsos guardados anteriormente en los puntos de menú
            lstMenues = TildarPermisosEnMenu(lstMenues, lstPermisosxPerfil).ToList();

            //Genera los valores tildados como los necesita el Arbol
            CheckedValues = GetMenuesChecked(lstMenues);

            await base.OnInitializedAsync();
        }
        
     
        private void AgregarPermisosEnMenues(List<MenuTreeDTO> menues,List<PermisoDTO> permisos)
        {
            foreach(var permiso in permisos)
            {
                foreach (var IdMenu in permiso.Menues)
                {
                    var menu = BuscarMenu(IdMenu,"M", menues);
                    if (menu != null)
                    {
                        if (!menu.SubMenues.Exists(x => x.Id == permiso.IdPermiso))
                        {
                            menu.SubMenues.Add(new MenuTreeDTO()
                            {
                                Id = permiso.IdPermiso,
                                Descripcion = permiso.Nombre + " (" + permiso.Codigo + ")",
                                Tipo = "P"
                            });
                        }
                    }
                }
            }

        }

        private MenuTreeDTO? BuscarMenu(int IdMenu, string Tipo, List<MenuTreeDTO> lst)
        {
            MenuTreeDTO? result = null;
            foreach(var menu in lst) {
                if (menu.Id == IdMenu && menu.Tipo == Tipo)
                {
                    result = menu;
                    if (result != null) break;
                }
                else if (menu.SubMenues.Count > 0)
                {
                    result = BuscarMenu(IdMenu, Tipo, menu.SubMenues);
                    if (result != null) break;
                }
            }

            return result;
        }

    
        private List<MenuTreeDTO> TildarPermisosEnMenu(List<MenuTreeDTO> menues, List<PermisoDTO> permisosValidos)
        {
            List<MenuTreeDTO> result = new List<MenuTreeDTO>();

            foreach (var menu in menues)
            {

                if (menu.SubMenues.Count > 0)
                {
                    menu.SubMenues = TildarPermisosEnMenu(menu.SubMenues, permisosValidos);
                    
                    if(!menu.SubMenues.Exists(x=> !x.Checked))
                        menu.Checked = true;
                    
                }
                else
                {
                    if (menu.Tipo == "P" && permisosValidos.Exists(x => x.IdPermiso == menu.Id))
                    {
                        menu.Checked = true;
                    }
                }

            }
            return menues;
        }
        private List<MenuTreeDTO> GetMenuesChecked(List<MenuTreeDTO> menues)
        {
            List<MenuTreeDTO> result = new List<MenuTreeDTO>();

            foreach (var menu in menues)
            {
                if (menu.SubMenues.Count > 0)
                {
                    result.AddRange( GetMenuesChecked(menu.SubMenues));
                }
                if (menu.Checked)
                    result.Add(menu);


            }

            return result;
        }

        private async Task<bool> ValidacionesAlGuardarAsync()
        {
            List<string> mensajes = new List<string>();

            var lstPermisos = new List<int>();
            foreach (MenuTreeDTO item in CheckedValues)
            {
                if (item.Tipo == "P")
                    lstPermisos.Add(item.Id);
            }

            if(lstPermisos.Count == 0)
            {
                mensajes.Add("Debe seleccionar algún permiso para poder guardar.");
            }

            if (mensajes.Count > 0)
            {
                await DS.OpenAsync<ModalValidaciones>("Validaciones",
                                       new Dictionary<string, object>() { { "Mensajes", mensajes }, { "Title", "Se han encontrado validaciones a revisar:" } },
                                       new DialogOptions() { Width = "auto", Height = "auto", Resizable = false });

            }
            return mensajes.Count == 0;
        }

        protected async Task OnClickGuardar()
        {
            if (isBusyAceptar)
                return;

            isBusyAceptar = true;
            try
            {
                if (await ValidacionesAlGuardarAsync())
                {
                    if (CheckedValues != null)
                    {
                        var lstPermisos = new List<int>();
                        foreach (MenuTreeDTO item in CheckedValues)
                        {
                            if (item.Tipo == "P")
                                lstPermisos.Add(item.Id);
                        }

                        await perfilesBL.ActualizarPermisosAsync(IdPerfil, lstPermisos);

                    }

                    navigationManager.NavigateTo($"/Seguridad/Perfiles", true);
                }
                else
                    isBusyAceptar = false;
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                isBusyAceptar = false;
            }
            

        }
        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/Seguridad/Perfiles", true);
        }
    }
}
