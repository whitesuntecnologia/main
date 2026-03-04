using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using Radzen.Blazor;
using Website.Extensions;
using Website.Models.Seguridad;
using static StaticClass.Constants;

namespace Website.Pages.Seguridad.Usuarios
{
    public partial class Index: ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IUsuariosBL _usuariosBL { get; set; } = null!;
        [Inject] private ICombosBL _combosBL { get; set; } = null!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;
        
        private List<UsuarioDTO> lstUsuarios { get; set; } = new();
        private IEnumerable<GenericComboDTO> lstPerfiles { get; set; } = null!;
        private List<GenericComboDTO> lstEstados { get; set; } = new();
        private RadzenDataGrid<UsuarioDTO>? grdUsuarios = null!;
        private BusquedaUsuarioModel Model { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            lstUsuarios = await _usuariosBL.GetUsersAsync();
            lstPerfiles = await _combosBL.GetPerfilesAsync();
            lstEstados = await _combosBL.GetEstados();
            
            await base.OnInitializedAsync();
        }
        private Task AgregarClick()
        {
            navigationManager.NavigateTo($"/Seguridad/Usuarios/Agregar");
            return Task.CompletedTask;
        }
        private Task EditarClick(UsuarioDTO row)
        {
            navigationManager.NavigateTo($"/Seguridad/Usuarios/Editar/{row.Id}");
            return Task.CompletedTask;
        }
        private async Task ExportarExcel()
        {
            if (grdUsuarios != null)
            {
                await grdUsuarios.ExportGridDataAsync(JSRuntime, ExportType.Excel, "Usuarios", new List<string>() { "Acciones" });
            }
        }
        private async Task BuscarClick()
        {
            lstUsuarios = await _usuariosBL.GetUsersAsync(Model.usuario, Model.IdPerfil, Model.IdEstado);
        }
        private async Task LimpiarClick()
        {
            Model = new();
            lstUsuarios = await _usuariosBL.GetUsersAsync();
        }
    }
}
