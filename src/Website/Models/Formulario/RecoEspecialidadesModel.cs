namespace Website.Models.Formulario
{
    public class RecoEspecialidadesModel
    {
        public int IdSeccion { get; set; }
        public string DescripcionSeccion { get; set; } = null!;
        public int IdEspecialidad { get; set; }
        public string DescripcionEspecialidad { get; set; } = null!;
        public bool Baja { get; set; }
        public decimal? CoefCapacTecnicaxEquipo { get; set; }
        public decimal? MultiplicadorCapEconomica { get; set; }
        public bool Seleccionada { get; set; }
    }
}
