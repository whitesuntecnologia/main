using DataTransferObject;
using FluentValidation;
using StaticClass;
using System.ComponentModel.DataAnnotations;
using Website.Models.Formulario;
using Website.Models.Validators;

namespace Website.Models.Seguridad
{
    public class UsuarioModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public List<PerfilDTO> Perfiles { get; set; } = new();
        public int Estado { get; set; }
        public string? NombreyApellido { get; set; }
        public IEnumerable<int> CheckedValues { get; set; } = new List<int>();
    }

    public class UsuarioModelValidator : AbstractValidator<UsuarioModel>
    {
        public UsuarioModelValidator()
        {

            RuleFor(p => p.UserName)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.UserName)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.UserName)
                .Must(username => Functions.ValidateCUIT(username))
                .When(x=> x.CheckedValues.Contains(Constants.Perfiles.Empresa) || x.CheckedValues.Contains(Constants.Perfiles.Evaluador))
                .WithMessage("El CUIT es inválido");
            
            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Email)
                .EmailAddress()
                .WithMessage("el valor ingrasado no es una dirección de correo válida.");

            RuleFor(p => p.CheckedValues)
                .NotEmpty()
                .WithMessage("Debe tildar algún perfil");

            RuleFor(p => p.NombreyApellido)
                .NotEmpty()
                //.When(x=> x.CheckedValues.Contains(Constants.Perfiles.Empresa))
                .WithMessage("El campo es requerido");
            
        }
    }
}
