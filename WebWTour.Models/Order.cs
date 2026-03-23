using System.ComponentModel.DataAnnotations;

namespace WebWTour.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public int TourId { get; set; }
    
    [Required]
    public string TitleOrder { get; set; } = string.Empty;
    
    [Required]
    public int PriceOrder { get; set; }
    
    [Required]
    public string PlaceOrder { get; set; } = string.Empty;
    
    [Required]
    public DateTime OpenDateOrder { get; set; }
    
    [Required]
    public DateTime CloseDateOrder { get; set; }
    
    public int Quantity { get; set; } = 1;
    
    public DateTime OrderDate { get; set; } = DateTime.Now;
    
    public string? ImageLink { get; set; }
    
    public string? Season { get; set; }
    
    public string? BookType { get; set; }
    
    public bool IsCompleted { get; set; } = false;
    
    // Навигационное свойство
    public User? User { get; set; }
    public Tour? Tour { get; set; }
}