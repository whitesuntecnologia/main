using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;

namespace Website.Pages.Seguridad.Perfiles
{
    public partial class Index : ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IPerfilesBL perfilesBL { get; set;} = null!;
        private List<PerfilDTO> lstPerfiles { get; set; } = new();
        private RadzenDataGrid<PerfilDTO>? grdPerfiles = null!;

        protected override async Task OnInitializedAsync()
        {
            lstPerfiles = await perfilesBL.GetPerfilesAsync();
            await base.OnInitializedAsync();
        }
        private  Task EditarClick(PerfilDTO row)
        {
            navigationManager.NavigateTo($"/Seguridad/Perfiles/Editar/{row.IdPerfil}");
            return Task.CompletedTask;
        }
    }
}
