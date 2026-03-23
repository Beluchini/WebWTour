using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using WebWTour.Database;
using WebWTour.Models;

namespace WebWTour
{
    public class AuthService
    {
        private User? _currentUser;
        private List<CartItem> _cartItems = new();
        private List<Order> _bookedOrders = new();
        private readonly ProtectedLocalStorage _localStorage;
        private readonly TourContext _context;
        
        public event Action? OnAuthStateChanged;
        public event Action? OnCartChanged;
        public event Action? OnBookingsChanged;
        
        public User? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public IReadOnlyList<CartItem> CartItems => _cartItems;
        public IReadOnlyList<Order> BookedOrders => _bookedOrders;
        public int CartCount => _cartItems.Sum(x => x.Quantity);
        public decimal CartTotal => _cartItems.Sum(x => (x.Tour?.Price ?? 0) * x.Quantity);
        
        public AuthService(ProtectedLocalStorage localStorage, TourContext context)
        {
            _localStorage = localStorage;
            _context = context;
        }
        
        public async Task InitializeAsync()
        {
            try
            {
                var userResult = await _localStorage.GetAsync<User>("currentUser");
                if (userResult.Success && userResult.Value != null)
                {
                    _currentUser = userResult.Value;
                    await LoadCartFromDatabase();
                    await LoadBookingsFromDatabase();
                }
                
                NotifyStateChanged();
            }
            catch (Exception)
            {
                // Обработка ошибки
            }
        }
        
        private async Task LoadCartFromDatabase()
        {
            if (_currentUser == null) return;
            
            var orders = await _context.Orders
                .Where(o => o.UserId == _currentUser.Id && o.Status == "cart")
                .Include(o => o.Tour)
                .ToListAsync();
                
            _cartItems = orders.Select(o => new CartItem
            {
                OrderId = o.Id,
                Tour = o.Tour ?? new Tour 
                { 
                    Id = o.TourId,
                    Tittle = o.TitleOrder,
                    Price = o.PriceOrder,
                    Place = o.PlaceOrder,
                    OpenDate = o.OpenDateOrder,
                    CloseDate = o.CloseDateOrder,
                    ImageLink = o.ImageLink ?? "",
                    Season = o.Season ?? "",
                    BookType = o.BookType ?? "",
                    Description = "",
                    FullDecription = ""
                },
                Quantity = o.Quantity
            }).ToList();
        }
        
        private async Task LoadBookingsFromDatabase()
        {
            if (_currentUser == null) return;
            
            _bookedOrders = await _context.Orders
                .Where(o => o.UserId == _currentUser.Id && o.Status == "booked")
                .OrderByDescending(o => o.BookingDate)
                .ToListAsync();
        }
        
        public async Task Login(User user)
        {
            _currentUser = user;
            await _localStorage.SetAsync("currentUser", user);
            await LoadCartFromDatabase();
            await LoadBookingsFromDatabase();
            NotifyStateChanged();
            NotifyCartChanged();
            NotifyBookingsChanged();
        }
        
        public async Task Logout()
        {
            _currentUser = null;
            _cartItems.Clear();
            _bookedOrders.Clear();
            await _localStorage.DeleteAsync("currentUser");
            NotifyStateChanged();
            NotifyCartChanged();
            NotifyBookingsChanged();
        }
        
        public async Task AddToCart(Tour tour, int quantity = 1)
        {
            if (_currentUser == null) return;
            
            var existingOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.UserId == _currentUser.Id && 
                                          o.TourId == tour.Id && 
                                          o.Status == "cart");
                                          
            if (existingOrder != null)
            {
                existingOrder.Quantity += quantity;
                await _context.SaveChangesAsync();
            }
            else
            {
                var order = new Order
                {
                    UserId = _currentUser.Id,
                    TourId = tour.Id,
                    TitleOrder = tour.Tittle,
                    PriceOrder = tour.Price,
                    PlaceOrder = tour.Place,
                    OpenDateOrder = tour.OpenDate,
                    CloseDateOrder = tour.CloseDate,
                    Quantity = quantity,
                    OrderDate = DateTime.Now,
                    ImageLink = tour.ImageLink,
                    Season = tour.Season,
                    BookType = tour.BookType,
                    Status = "cart"
                };
                
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }
            
            await LoadCartFromDatabase();
            NotifyCartChanged();
        }
        
        public async Task RemoveFromCart(int orderId)
        {
            if (_currentUser == null) return;
            
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == _currentUser.Id && o.Status == "cart");
                
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                await LoadCartFromDatabase();
                NotifyCartChanged();
            }
        }
        
        public async Task UpdateQuantity(int orderId, int quantity)
        {
            if (_currentUser == null) return;
            
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == _currentUser.Id && o.Status == "cart");
                
            if (order != null)
            {
                if (quantity <= 0)
                {
                    await RemoveFromCart(orderId);
                }
                else
                {
                    order.Quantity = quantity;
                    await _context.SaveChangesAsync();
                    await LoadCartFromDatabase();
                    NotifyCartChanged();
                }
            }
        }
        
        public async Task ClearCart()
        {
            if (_currentUser == null) return;
            
            var orders = await _context.Orders
                .Where(o => o.UserId == _currentUser.Id && o.Status == "cart")
                .ToListAsync();
                
            _context.Orders.RemoveRange(orders);
            await _context.SaveChangesAsync();
            
            _cartItems.Clear();
            NotifyCartChanged();
        }
        
        public async Task BookOrder(int orderId)
        {
            if (_currentUser == null) return;
            
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == _currentUser.Id && o.Status == "cart");
                
            if (order != null)
            {
                order.Status = "booked";
                order.BookingDate = DateTime.Now;
                await _context.SaveChangesAsync();
                
                await LoadCartFromDatabase();
                await LoadBookingsFromDatabase();
                NotifyCartChanged();
                NotifyBookingsChanged();
            }
        }
        
        public async Task BookAllOrders()
        {
            if (_currentUser == null) return;
            
            var orders = await _context.Orders
                .Where(o => o.UserId == _currentUser.Id && o.Status == "cart")
                .ToListAsync();
                
            foreach (var order in orders)
            {
                order.Status = "booked";
                order.BookingDate = DateTime.Now;
            }
            
            await _context.SaveChangesAsync();
            
            await LoadCartFromDatabase();
            await LoadBookingsFromDatabase();
            NotifyCartChanged();
            NotifyBookingsChanged();
        }
        
        public async Task CancelBooking(int orderId)
        {
            if (_currentUser == null) return;
            
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == _currentUser.Id && o.Status == "booked");
                
            if (order != null)
            {
                order.Status = "cancelled";
                await _context.SaveChangesAsync();
                await LoadBookingsFromDatabase();
                NotifyBookingsChanged();
            }
        }
        
        private void NotifyStateChanged()
        {
            OnAuthStateChanged?.Invoke();
        }
        
        private void NotifyCartChanged()
        {
            OnCartChanged?.Invoke();
        }
        
        private void NotifyBookingsChanged()
        {
            OnBookingsChanged?.Invoke();
        }
    }
    
    public class CartItem
    {
        public int OrderId { get; set; }
        public Tour Tour { get; set; } = new();
        public int Quantity { get; set; }
    }
}