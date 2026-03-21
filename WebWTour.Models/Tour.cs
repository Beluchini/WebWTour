using System.ComponentModel.DataAnnotations;

namespace WebWTour.Models;

public class Tour
{
    [Key]
    public int Id { get; set; }
    public string Tittle { get; set; }
    public string Description { get; set; }
    public string FullDecription { get; set; }
    public string ImageLink { get; set; }
    public int Price { get; set; }
    public string Place { get; set; }
    public string Season { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime CloseDate { get; set; }
    public string BookType { get; set; }
}