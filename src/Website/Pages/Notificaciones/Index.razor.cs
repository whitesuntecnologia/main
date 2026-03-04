using AutoMapper;
using Business;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System;
using static StaticClass.Constants;
using Website.Pages.Shared;

namespace Website.Pages.Notificaciones
{
    public partial class Index : ComponentBase
    {
        [Inject] private INotificacionesBL notificacionesBL { get; set; } = null!;
        [Inject] private IUsuariosBL usuariosBL { get; set; } = null!;
        [Inject] private ITramitesBL tramitesBL { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        private List<NotificacionDTO> lstNotificaciones { get; set; } = new();
        private RadzenDataGrid<NotificacionDTO> grdNotificaciones = null!;
        private string userid { get; set; } = null!;
        private bool isBusyInitialLoading {get;set;} = true;
        protected override async Task OnInitializedAsync()
        {
            userid = await usuariosBL.GetCurrentUserid();
            var lstPerfiles = await usuariosBL.GetPerfilesForUserAsync(userid);

            bool EsAdministrador = lstPerfiles.Select(s => s.IdPerfil).Contains((int)StaticClass.Constants.Perfiles.Administrador);
            
            if(EsAdministrador)
                lstNotificaciones = await notificacionesBL.GetNotificaciones();
            else
                lstNotificaciones = await notificacionesBL.GetNotificacionesPorUsuario(userid);
            
            await base.OnInitializedAsync();
            isBusyInitialLoading = false;
        }

        protected async Task LeerClick(NotificacionDTO row)
        {
            if (!row.Leido)
            {
                await notificacionesBL.MarcarComoLeido(row.IdNotificacion);
                row.Leido = true;
            }

            await dialogService.OpenAsync<ModalNotificaciones>("Notificación",
                        new Dictionary<string, object>() { { "Mensaje", row.Mensaje }, { "Title", row.Titulo } },
                                           new DialogOptions() { Width = "60%", Height = "auto", Resizable = false });
            
        }

        protected async Task navigateClick(int IdTramite)
        {
            var tramite = await tramitesBL.GetTramiteByIdAsync(IdTramite);
            navigationManager.NavigateTo($"/Tramites/Visualizar/{tramite.IdentificadorUnico}");
        }
    }
}
