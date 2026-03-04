using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class TokensLoginSuu
{
    public int IdLoginSuu { get; set; }

    public string Tokenregreso { get; set; } = null!;

    public string Tokenvalidacion { get; set; } = null!;

    public string? UsuarioLogueado { get; set; }

    public int? Respuesta1 { get; set; }

    public int? Respuesta2 { get; set; }

    public int? Respuesta3 { get; set; }

    public DateTime CreateDate { get; set; }
}
