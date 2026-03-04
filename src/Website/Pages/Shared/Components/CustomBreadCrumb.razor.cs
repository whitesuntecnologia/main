using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Website.Pages.Shared.Components
{
    public partial class CustomBreadCrumb : ComponentBase
    {
        [Parameter] public Dictionary<string,string> Path { get; set; } = new Dictionary<string,string>();

        protected override async Task OnInitializedAsync()
        {
            

            await base.OnInitializedAsync();
        }
    }
}
