using AutoMapper;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using System.Runtime.CompilerServices;
using Website.Models.Seguridad;

namespace Website.Pages.Seguridad.Empresas
{
    public partial class AddEditEmpresa : ComponentBase
    {
        [Parameter] public int? IdEmpresa { get; set; } = null!;
        [Parameter] public string Accion { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;

        [Inject] private IEmpresasBL _EmpresasBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuariosBL { get; set; } = null!;
        private EmpresaModel Model { get; set; } = new();
        private List<GenericComboStrDTO> lstUsuarios { get; set; } = new();
        private RadzenDataGrid<EmpresaSancionModel>? grdSanciones = null;
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; }
        private EditContext? context;

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EmpresaModel, EmpresaDTO>().ReverseMap();
                cfg.CreateMap<EmpresaSancionModel, EmpresaSancionDTO>().ReverseMap();
                cfg.CreateMap<EmpresaSancionModel, EmpresaSancionModel>(); // Mapeo para clonar
                cfg.CreateMap<UsuarioDTO, GenericComboStrDTO>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.UserName + " - " + src.NombreyApellido))
                ;
            });
            _mapper = config.CreateMapper();
            //--

            lstUsuarios = _mapper.Map<List<GenericComboStrDTO>>(await _usuariosBL.GetUsersAsync(null, Constants.Perfiles.Empresa, null));


            if (IdEmpresa.HasValue)
            {
                var dto = await _EmpresasBL.GetEmpresaAsync(IdEmpresa.Value);
                Model = _mapper.Map<EmpresaModel>(dto);
            }

            await base.OnInitializedAsync();
        }

      

        private async Task OnClickAgregarSancion()
        {
            var nuevaSancion = new EmpresaSancionModel();
            
            var sancion = await dialogService.OpenAsync<AddEditSancion>("Agregar Sanción",
                new Dictionary<string, object> { { "Model", nuevaSancion } },
                new DialogOptions()
                {
                    Width = "1000px",
                    Height = "auto",
                    Resizable = true,
                    Draggable = true
                });

            if (sancion != null && sancion is EmpresaSancionModel)
            {
                Model.Sanciones.Add(sancion);
                await ReloadGridSanciones();
            }
        }

        private async Task OnClickEditarSancion(EmpresaSancionModel sancion)
        {
            // Crear una copia del modelo para edición
            var sancionCopia = _mapper.Map<EmpresaSancionModel>(sancion);

            var sancionEditada = await dialogService.OpenAsync<AddEditSancion>("Editar Sanción",
                new Dictionary<string, object> { { "Model", sancionCopia } },
                new DialogOptions()
                {
                    Width = "700px",
                    Height = "auto",
                    Resizable = true,
                    Draggable = true
                });

            // Solo actualizar el original si se confirmó la edición
            if (sancionEditada != null && sancionEditada is EmpresaSancionModel)
            {
                // Copiar los valores editados al modelo original
                sancion.Nombre = sancionEditada!.Nombre;
                sancion.FechaDesdeSancion = sancionEditada.FechaDesdeSancion;
                sancion.FechaHastaSancion = sancionEditada.FechaHastaSancion;
                sancion.IdFileSancion = sancionEditada.IdFileSancion;
                sancion.FilenameSancion = sancionEditada.FilenameSancion;

                await ReloadGridSanciones();
            }
            // Si result es null (canceló), no se hace nada y la sanción original queda intacta
        }

        private void OnClickEliminarSancion(EmpresaSancionModel sancion)
        {
            Model.Sanciones.Remove(sancion);
            ReloadGridSanciones().Wait();
        }

        private async Task ReloadGridSanciones()
        {
            if (grdSanciones != null)
                await grdSanciones.Reload();
            else
                StateHasChanged();
        }
        protected async Task OnClickAceptar(EditContext Ed)
        {
            if (isBusyAceptar)
                return;

            context = Ed;
            isBusyAceptar = true;

            if (Ed != null && Ed.Validate())
            {

                try
                {
                    var dto = _mapper.Map<EmpresaModel, EmpresaDTO>(Model);

                    if (Model.IdEmpresa.HasValue)
                        await _EmpresasBL.ActualizarEmpresaAsync(dto);
                    else
                        await _EmpresasBL.AgregarEmpresaAsync(dto);

                    navigationManager.NavigateTo("/Seguridad/Empresas", true);
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }

            isBusyAceptar = false;
        }
        protected async Task OnClickCancelar()
        {
            navigationManager.NavigateTo("/Seguridad/Empresas", true);
        }
    }
}