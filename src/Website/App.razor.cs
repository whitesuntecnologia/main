using Business.Extensions;
using DataAccess.Entities;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client.Extensions.Msal;
using Website.Services;

namespace Website
{
    public partial class App : ComponentBase
    {
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;
        [Inject] private AppState AppStateService { get; set; } = null!;
        private void OnNavigateAsync(NavigationContext args)
        {
            routeHistory.AddHistoryItem(args.Path);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                AppStateService.SetAppLoaded();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
