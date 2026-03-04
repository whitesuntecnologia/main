using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataTransferObject
{
    public class UsuarioDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public List<PerfilDTO> Perfiles { get; set; }
        public int Estado { get; set; }
        public string NombreEstado 
        { 
            get
            {
                var nombreEstado = (from d in typeof(Constants.UsuariosEstados).GetFields()
                        where  (int) d.GetRawConstantValue() == this.Estado
                        select d.Name).FirstOrDefault();
                
                return nombreEstado;
            }
        }
        public string Perfiles1Linea
        {
            get
            {
                return string.Join(", ", this.Perfiles.Select(s => s.Nombre).ToList());
            }
        }
        public string NombreyApellido { get; set; }

    }
}
