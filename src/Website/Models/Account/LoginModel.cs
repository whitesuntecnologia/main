using System.ComponentModel.DataAnnotations;

namespace Website.Models.Account
{
    public class LoginModel
    {
        public string ReturnUrl { get; set; } = null!;
        [Required( ErrorMessage ="El Nombre de usuario es requerido")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = null!;
    }
}
