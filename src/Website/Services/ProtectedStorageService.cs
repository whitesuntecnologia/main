using Business.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Website.Services
{
    public class ProtectedStorageService : IStorageService
    {
        private readonly ProtectedLocalStorage _protectedLocalStorage;

        public ProtectedStorageService(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }

        public async Task SaveAsync(string key, string value)
        {
            await _protectedLocalStorage.SetAsync(key, value);
        }

        public async Task<string?> GetAsync(string key)
        {
            string? result = null!;
            TimeSpan timeout = TimeSpan.FromSeconds(5);
            var ValueStorageresult = await Task.Run(() => _protectedLocalStorage?.GetAsync<string>(key).Result).WaitAsync(timeout);

            if (ValueStorageresult != null && ValueStorageresult.Value.Success)
                result = (ValueStorageresult.Value.Value ?? "");

            return result;
        }
    }
}
