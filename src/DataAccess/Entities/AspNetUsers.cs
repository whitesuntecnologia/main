using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class AspNetUsers
{
    public string Id { get; set; } = null!;

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

    public int Estado { get; set; }

    public virtual ICollection<Especialidades> EspecialidadesCreateUserNavigation { get; set; } = new List<Especialidades>();

    public virtual ICollection<EspecialidadesEquipos> EspecialidadesEquiposCreateUserNavigation { get; set; } = new List<EspecialidadesEquipos>();

    public virtual ICollection<EspecialidadesEquipos> EspecialidadesEquiposLastUpdateUserNavigation { get; set; } = new List<EspecialidadesEquipos>();

    public virtual ICollection<Especialidades> EspecialidadesLastUpdateUserNavigation { get; set; } = new List<Especialidades>();

    public virtual ICollection<EspecialidadesSecciones> EspecialidadesSeccionesCreateUserNavigation { get; set; } = new List<EspecialidadesSecciones>();

    public virtual ICollection<EspecialidadesSecciones> EspecialidadesSeccionesLastUpdateUserNavigation { get; set; } = new List<EspecialidadesSecciones>();

    public virtual ICollection<EspecialidadesTareas> EspecialidadesTareasCreateUserNavigation { get; set; } = new List<EspecialidadesTareas>();

    public virtual ICollection<EspecialidadesTareas> EspecialidadesTareasLastUpdateUserNavigation { get; set; } = new List<EspecialidadesTareas>();

    public virtual ICollection<Files> FilesCreateUserNavigation { get; set; } = new List<Files>();

    public virtual ICollection<Files> FilesUpdateUserNavigation { get; set; } = new List<Files>();

    public virtual ICollection<IndicesBancoCentral> IndicesBancoCentralCreateUserNavigation { get; set; } = new List<IndicesBancoCentral>();

    public virtual ICollection<IndicesBancoCentral> IndicesBancoCentralLastUpdateUserNavigation { get; set; } = new List<IndicesBancoCentral>();

    public virtual ICollection<IndicesCostoConstruccion> IndicesCostoConstruccion { get; set; } = new List<IndicesCostoConstruccion>();

    public virtual ICollection<IndicesUnidadVivienda> IndicesUnidadViviendaCreateUserNavigation { get; set; } = new List<IndicesUnidadVivienda>();

    public virtual ICollection<IndicesUnidadVivienda> IndicesUnidadViviendaLastUpdateUserNavigation { get; set; } = new List<IndicesUnidadVivienda>();

    public virtual ICollection<RelEmpresasRepresentantes> RelEmpresasRepresentantesCreateUserNavigation { get; set; } = new List<RelEmpresasRepresentantes>();

    public virtual ICollection<RelEmpresasRepresentantes> RelEmpresasRepresentantesLastUpdateUserNavigation { get; set; } = new List<RelEmpresasRepresentantes>();

    public virtual ICollection<RelUsuariosPerfiles> RelUsuariosPerfiles { get; set; } = new List<RelUsuariosPerfiles>();

    public virtual ICollection<TiposDeDocumentos> TiposDeDocumentos { get; set; } = new List<TiposDeDocumentos>();

    public virtual ICollection<TramitesAntecedentes> TramitesAntecedentesCreateUserNavigation { get; set; } = new List<TramitesAntecedentes>();

    public virtual ICollection<TramitesAntecedentes> TramitesAntecedentesLastUpdateUserNavigation { get; set; } = new List<TramitesAntecedentes>();

    public virtual ICollection<TramitesBalancesGenerales> TramitesBalancesGeneralesCreateUserNavigation { get; set; } = new List<TramitesBalancesGenerales>();

    public virtual ICollection<TramitesBalancesGenerales> TramitesBalancesGeneralesLastUpdateUserNavigation { get; set; } = new List<TramitesBalancesGenerales>();

    public virtual ICollection<TramitesBienesRaices> TramitesBienesRaicesCreateUserNavigation { get; set; } = new List<TramitesBienesRaices>();

    public virtual ICollection<TramitesBienesRaices> TramitesBienesRaicesLastUpdateUserNavigation { get; set; } = new List<TramitesBienesRaices>();

    public virtual ICollection<Tramites> TramitesCreateUserNavigation { get; set; } = new List<Tramites>();

    public virtual ICollection<TramitesEquipos> TramitesEquiposCreateUserNavigation { get; set; } = new List<TramitesEquipos>();

    public virtual ICollection<TramitesEquipos> TramitesEquiposLastUpdateUserNavigation { get; set; } = new List<TramitesEquipos>();

    public virtual ICollection<TramitesEspecialidades> TramitesEspecialidades { get; set; } = new List<TramitesEspecialidades>();

    public virtual ICollection<Tramites> TramitesEvaluadorUserNavigation { get; set; } = new List<Tramites>();

    public virtual ICollection<TramitesInfEmp> TramitesInfEmpCreateUserNavigation { get; set; } = new List<TramitesInfEmp>();

    public virtual ICollection<TramitesInfEmpDeudas> TramitesInfEmpDeudasCreateUserNavigation { get; set; } = new List<TramitesInfEmpDeudas>();

    public virtual ICollection<TramitesInfEmpDeudas> TramitesInfEmpDeudasLastUpdateUserNavigation { get; set; } = new List<TramitesInfEmpDeudas>();

    public virtual ICollection<TramitesInfEmpDocumentos> TramitesInfEmpDocumentos { get; set; } = new List<TramitesInfEmpDocumentos>();

    public virtual ICollection<TramitesInfEmp> TramitesInfEmpLastupdateUserNavigation { get; set; } = new List<TramitesInfEmp>();

    public virtual ICollection<Tramites> TramitesLastUpdateUserNavigation { get; set; } = new List<Tramites>();

    public virtual ICollection<TramitesObras> TramitesObrasCreateUserNavigation { get; set; } = new List<TramitesObras>();

    public virtual ICollection<TramitesObrasEjecucion> TramitesObrasEjecucionCreateUserNavigation { get; set; } = new List<TramitesObrasEjecucion>();

    public virtual ICollection<TramitesObrasEjecucion> TramitesObrasEjecucionLastUpdateUserNavigation { get; set; } = new List<TramitesObrasEjecucion>();

    public virtual ICollection<TramitesObras> TramitesObrasLastUpdateUserNavigation { get; set; } = new List<TramitesObras>();

    public virtual ICollection<TramitesRepresentantesTecnicos> TramitesRepresentantesTecnicosCreateUserNavigation { get; set; } = new List<TramitesRepresentantesTecnicos>();

    public virtual ICollection<TramitesRepresentantesTecnicos> TramitesRepresentantesTecnicosLastUpdateUserNavigation { get; set; } = new List<TramitesRepresentantesTecnicos>();
}
