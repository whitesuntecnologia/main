namespace Website.Models.Formulario
{
    public class InfEmpresaDocumentoModel
    {
        public int IdTipoDocumento { get; set; }

        public string Descripcion { get; set; } = null!;

        public int? TamanioMaximoMb { get; set; }

        public string Extension { get; set; } = null!;

        public string? AcronimoGde { get; set; }

        public bool Obligatorio { get; set; }
        public bool SePermiteVariasVeces { get; set; }

        public int? IdTramiteInfEmpDocumento { get; set; }
        public int? IdFile { get; set; }
        public string? Filename { get; set; }

        //propiedades solo para leer en las grillas
        public long? Size { get; set; }
        public string? SizeStr
        {
            get
            {
                string? result = null;
                if (this.Size.HasValue)
                {
                    decimal kb = this.Size.Value / 1024.0m;
                    decimal mb = kb / 1024.0m;
                    result = (mb < 1 ? Math.Ceiling(kb).ToString("N0") + " Kb." : mb.ToString("N2") + " Mb.");
                }
                return result;
            }
        }
    }
}
