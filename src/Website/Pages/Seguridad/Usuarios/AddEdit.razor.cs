using AutoMapper;
using Business;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;
using Website.Models.Seguridad;

namespace Website.Pages.Seguridad.Usuarios
{
    public partial class AddEdit: ComponentBase
    {
        [Parameter] public string? userid { get; set; }

        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;

        [Inject] private IUsuariosBL usuarioBL { get; set; } = null!;
        [Inject] private ICombosBL combosBL { get; set; } = null!;
        private IEnumerable<GenericComboDTO> lstPerfiles { get; set; } = null!;
        private IEnumerable<GenericComboDTO> lstEstados { get; set; } = null!;
        private string Accion { get; set; } = "Agregar";
        private UsuarioModel Model { get; set; } = new();
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UsuarioModel, UsuarioDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--


            lstPerfiles = await combosBL.GetPerfilesAsync();
            lstEstados = new List<GenericComboDTO>()
            {
                new GenericComboDTO{ Id = (int) Constants.UsuariosEstados.Activo, Descripcion = nameof(Constants.UsuariosEstados.Activo)},
                new GenericComboDTO{ Id = (int) Constants.UsuariosEstados.Bloqueado, Descripcion = nameof(Constants.UsuariosEstados.Bloqueado) },
                new GenericComboDTO{ Id = (int) Constants.UsuariosEstados.Baja, Descripcion = nameof(Constants.UsuariosEstados.Baja) },
            };
            
            if (!string.IsNullOrWhiteSpace(userid)) 
            {
                Accion = "Editar";
                Model = _mapper.Map<UsuarioModel>(await usuarioBL.GetUserByIdAsync(userid));
                Model.CheckedValues = Model.Perfiles.Select(s=> s.IdPerfil).ToList();
            }
            
            await base.OnInitializedAsync();
        }
        protected async Task OnClickAceptar(EditContext Ed)
        {
            if (isBusyAceptar)
                return;

            isBusyAceptar = true;
            if (Ed != null && Ed.Validate())
            {

                try
                {
                    var dto = _mapper.Map<UsuarioModel, UsuarioDTO>(Model);

                    dto.Perfiles = (from p in lstPerfiles
                                    join chk in Model.CheckedValues on p.Id equals chk
                                    select new PerfilDTO
                                    {
                                        IdPerfil = chk,
                                        Nombre = p.Descripcion,
                                    }).ToList();
                    
                    if (!string.IsNullOrWhiteSpace(dto.Id))
                        await usuarioBL.ActualizarUsuarioAsync(dto);
                    else
                        await usuarioBL.AgregarUsuarioAsync(dto);

                    navigationManager.NavigateTo($"/Seguridad/Usuarios", true);
                }
                catch (Exception ex)
                {
                    isBusyAceptar = false;
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }
            else
                isBusyAceptar = false;
        
        }
        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/Seguridad/Usuarios", true);
        }
    }
}
