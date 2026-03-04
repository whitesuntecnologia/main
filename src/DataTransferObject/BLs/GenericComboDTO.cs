using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.BLs
{
    public class GenericComboDTO
    {
        public GenericComboDTO()
        {
        }

        public GenericComboDTO(int id, string description)
        {
            Id = id;
            Descripcion = description;
        }

        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool IsDisabled { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    public class GenericComboStrDTO
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
    }
    public class GenericComboBooleanDTO
    {
        public bool Id { get; set; }
        public string Descripcion { get; set; }
    }
}
