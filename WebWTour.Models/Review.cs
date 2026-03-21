using System.ComponentModel.DataAnnotations;

namespace WebWTour.Models;

public class Review
{
    [Key]
    public int Id { get; set; }
    public string Text { get; set; }
    public string Author { get; set; }
    public int Rate { get; set; }
}