using System.ComponentModel.DataAnnotations;

namespace WebWTour.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    public string TitleOrder { get; set; }
    public int PriceOrder { get; set; }
    public string PlaceOrder { get; set; }
    public DateTime OpenDateOrder { get; set; }
    public DateTime CloseDateOrder { get; set; }
}