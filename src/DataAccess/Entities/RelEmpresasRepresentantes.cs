using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class RelEmpresasRepresentantes
{
    public int IdEmpresaRepresentante { get; set; }

    public decimal CuitEmpresa { get; set; }

    public decimal CuitRepresentante { get; set; }

    public DateTime FechaVigenciaDesde { get; set; }

    public DateTime? FechaVigenciaHasta { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateUser { get; set; } = null!;

    public DateTime? LastUpdateDate { get; set; }

    public string? LastUpdateUser { get; set; }
}
