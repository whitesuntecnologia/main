namespace Website.Services
{
    public class AppState
    {
        public bool IsAppLoaded { get; private set; } = false;
        public event Action? OnAppLoaded;

        public void SetAppLoaded()
        {
            IsAppLoaded = true;
            OnAppLoaded?.Invoke();
        }
    }
}
