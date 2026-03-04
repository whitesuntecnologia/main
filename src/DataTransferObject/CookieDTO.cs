using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class CookieDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime? Expires { get; set; }
        public string Domain { get; set; }
    }
}
