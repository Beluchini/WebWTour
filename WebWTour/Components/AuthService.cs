using WebWTour.Models;

namespace WebWTour.Components

{
    public class AuthService
    {
        private User? _currentUser;
        
        public event Action? OnAuthStateChanged;
        
        public User? CurrentUser => _currentUser;
        
        public bool IsAuthenticated => _currentUser != null;
        
        public void Login(User user)
        {
            _currentUser = user;
            NotifyStateChanged();
        }
        
        public void Logout()
        {
            _currentUser = null;
            NotifyStateChanged();
        }
        
        private void NotifyStateChanged() => OnAuthStateChanged?.Invoke();
    }
}