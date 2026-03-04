namespace Website.Models.Formulario
{
    public class AntecedentesDeclaracionMensualModel
    {
        public int? IdTramiteAntecedenteDDJJMensual { get; set; }
        public int IdTramiteAntecedente { get; set; }
        public int Mes {  get; set; }
        public int Anio { get;set; }
        public decimal? Monto { get; set; }
    }
    public partial class HeaderGrillaDdjjmensualModel
    {
        public int IdTramiteAntecedente { get; set; }
        public string ColumnName { get; set; } = string.Empty;
    }
}
