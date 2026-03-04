using ElmahCore;
using Microsoft.AspNetCore.Components.Web;

namespace Website.Services
{
    public class GlobalErrorHandler : ErrorBoundary
    {

        protected override Task OnErrorAsync(Exception exception)
        {
            // log to ELMAH here
            ElmahExtensions.RaiseError(exception);
            return base.OnErrorAsync(exception);

        }
    }
}
