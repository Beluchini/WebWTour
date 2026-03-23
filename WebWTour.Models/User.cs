using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebWTour.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Имя обязательно")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 50 символов")]
    [Display(Name = "Имя")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Фамилия обязательна")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Фамилия должна содержать от 2 до 50 символов")]
    [Display(Name = "Фамилия")]
    public string Surname { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Введите корректный email адрес")]
    [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Пароль обязателен")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать от 6 до 100 символов")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;
    
    [NotMapped]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение пароля")]
    public string ConfirmPassword { get; set; } = string.Empty;
    
    [Display(Name = "Дата регистрации")]
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    
    [Display(Name = "Последний вход")]
    public DateTime? LastLoginDate { get; set; }
    
    [Display(Name = "Активен")]
    public bool IsActive { get; set; } = true;
}