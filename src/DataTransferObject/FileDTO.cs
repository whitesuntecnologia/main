using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class FileDTO
    {
        public int IdFile { get; set; }

        public Guid Rowid { get; set; }

        public byte[] ContentFile { get; set; } = null!;

        public string FileName { get; set; }

        public string Extension { get; set; }

        public string ContentType { get; set; }

        public string Md5 { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUser { get; set; } = null!;

        public DateTime? UpdateDate { get; set; }

        public string UpdateUser { get; set; }
        public string FilePath { get; set; }
        
        public long Size { get;set; }
        public string SizeStr
        {
            get {
                decimal kb = this.ContentFile.Length / 1024.0m;
                decimal mb = kb / 1024.0m;
                string result = (mb < 1 ? Math.Ceiling(kb).ToString("N0") + " Kb." : mb.ToString("N2") + " Mb.");
                return result;
            }
        }
        
    }
}
