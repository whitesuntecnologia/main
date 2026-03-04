using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class ResponseConsultarDocumentoPdfDto
    {
        public string ErrorOut { get; set; }
        public byte[] RespuestaDocPDF { get; set; }
    }
}
