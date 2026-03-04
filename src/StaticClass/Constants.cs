using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticClass
{
    public static class Constants
    {
        // En esta clase se deben definir las constantes (const, enum, struct, etc)
        public const int SessionJsTimeout = 1800;  //seconds
        public const int ProvLaPampa = 11;  // Id de la provincia de la pampa en la BD.
        public const string DatagridNoRecords = "No se encontraron registros.";
        public const string LocalStorageAuthKey = "RELI.Auth";
        public const decimal CoefCapacTecnicaxEquipoRamaC = 4m; //Coeficiente para todos los de Rama C. Mantis 104
        public struct SqlErroNumbers
        {
            public const int ErrorRegistroRelacionado = 547;    // Error de ForeignKeys
        }
        public struct NotifDuration
        {
            public const int Fast = 6_000;
            public const int Normal = 10_000;
            public const int Slow = 15_000;
            public const int VerySlow = 25_000;
            public const int Endless = 300_000;
        }

        public struct TramitesEstados
        {
            public const int EditarInformacion = 0;
            public const int EnEvaluacion = 1;
            public const int Observado = 2;
            public const int Aprobado = 3;
            public const int Rechazado = 4;
            public const int ElevadoParaLaFirma = 5;
            public const int Anulado = 6;
        }
        public struct TramitesEstadosCodigos
        {
            public const string EditarInformacion = "EDIT_INF";
            public const string EnEvaluacion = "EVAL";
            public const string Observado = "OBS";
            public const string Aprobado = "APRO";
            public const string ElevadoParaLaFirma = "FIRMA";
            public const string Anulado = "ANU";
        }

        public struct UsuariosEstados
        {
            public const int Activo = 0;
            public const int Bloqueado = 1;
            public const int Baja = 2;
        }
        public struct Perfiles
        {
            public const int Empresa = 1;
            public const int Evaluador = 2;
            public const int Administrador = 3;
        }
        public enum ExportType
        {
            Excel = 1,
            Csv = 2,
            Pdf = 3,
        }

        public enum EstadosEvaluacion
        {
            Aprobar = 1,
            Notificar = 2
        }

        public enum Formularios
        {
            BoletaPago = 0,
            Especialidades = 1,
            RepresentantesTecnicos = 9,
            InformacionEmpresa = 2,
            BalanceGeneral = 6,
            Equipos = 7,
            BienesRaices = 8,
            Obras = 10,
            AntecedentesDeProduccion = 11,
            ObrasEnEjecucion = 12,
        }
        public struct TiposDeTramite
        {
            public const int Reli_Inscripcion = 1;
            public const int Reli_Licitar = 2;
            public const int Reli_ActualizacionSoloTecnicos = 3;
            public const int Reli_ActualizacionCapacidadTecnica = 4;
            public const int Reli_ActualizacionCompleta = 5;
            public const int Reco_Inscripcion = 6;
        }
        public struct GruposDeTramite
        {
            public const int RegistroLicitadores = 1;
            public const int RegistroConsultores = 2;
        }

        public struct EstadosObrasLP
        {
            public const string Finalizada = "Finalizada";
            public const string EnEjecucion = "En Ejecución";
            public const string EjecucionDelProyecto = "Ejecución del Proyecto";
            public const string Licitada = "Licitada";
            public const string ParaAdjudicar = "Para Adjudicar";
            public const string ParaLicitar = "Para Licitar";
        }
    }
}
