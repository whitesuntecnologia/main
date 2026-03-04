using Business.Interfaces;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Website.Pages.Shared.Components
{
    public partial class SQLCon : ComponentBase
    {
        private string sentencias { get; set; } = "";
        private List<dynamic> lstResults = new List<dynamic>();
        [Inject] private ITablasBL _TablasBL { get; set; } = null!;
        
        private System.Data.DataTable results = null!;
        
        protected async Task ExecuteClick()
        {
            try
            {
                results = await _TablasBL.Execute(sentencias);
                
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
