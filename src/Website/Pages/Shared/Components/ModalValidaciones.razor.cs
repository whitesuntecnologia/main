using Microsoft.AspNetCore.Components;
using Radzen;

namespace Website.Pages.Shared.Components
{
    public partial class ModalValidaciones: ComponentBase
    {
        [Parameter] public List<string> Mensajes { get; set; } = new List<string>();
        [Parameter] public string? Title { get; set; }
        [Inject] private DialogService dialogService { get; set; } = null!;
    }
}
