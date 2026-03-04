using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Extends
{
    public class GenericComboEntity
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    public class GenericStringComboEntity
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
    }
}
