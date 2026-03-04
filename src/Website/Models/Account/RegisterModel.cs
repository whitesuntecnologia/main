using FluentValidation;
using StaticClass;
using System.ComponentModel.DataAnnotations;
using Website.Models.Formulario;

namespace Website.Models.Account
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "El Nombre de usuario es requerido")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "El Email es requerido")]
        [EmailAddress (ErrorMessage = "El email no es válido")]
        public string Email { get; set; } = null!;
    }

}
