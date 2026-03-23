using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using WebWTour.Models;

namespace WebWTour
{
    public class AuthService
    {
        private User? _currentUser;
        private readonly ProtectedLocalStorage _localStorage;
        
        public event Action? OnAuthStateChanged;
        
        public User? CurrentUser => _currentUser;
        
        public bool IsAuthenticated => _currentUser != null;
        
        public AuthService(ProtectedLocalStorage localStorage)
        {
            _localStorage = localStorage;
        }
        
        public async Task InitializeAsync()
        {
            try
            {
                var result = await _localStorage.GetAsync<User>("currentUser");
                if (result.Success && result.Value != null)
                {
                    _currentUser = result.Value;
                    NotifyStateChanged();
                }
            }
            catch (Exception)
            {
                // Обработка ошибки
            }
        }
        
        public async Task Login(User user)
        {
            _currentUser = user;
            await _localStorage.SetAsync("currentUser", user);
            NotifyStateChanged();
        }
        
        public async Task Logout()
        {
            _currentUser = null;
            await _localStorage.DeleteAsync("currentUser");
            NotifyStateChanged();
        }
        
        private void NotifyStateChanged()
        {
            OnAuthStateChanged?.Invoke();
        }
    }
}