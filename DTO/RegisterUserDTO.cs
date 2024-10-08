using System.ComponentModel.DataAnnotations;

namespace WebApplication2.DTO
{
    public class RegisterUserDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

    }
}
