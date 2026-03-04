using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class UsuarioCookieDataDto
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? NombreyApellido { get; set; }
        public string? Email { get; set; }
        public int? IdEmpresa { get; set; }
        public decimal? CuitEmpresa { get; set; }
        public string? NombreEmpresa { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
