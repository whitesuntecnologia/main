using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Extensions
{
    public class RouteHistoryService
    {
        private List<RouteHistoryItem> history = new List<RouteHistoryItem>();
        
        public IReadOnlyList<RouteHistoryItem> History => history;

        public void AddHistoryItem(string path)
        {
            var item = new RouteHistoryItem
            {
                Path = path,
                Timestamp = DateTime.Now
            };

            history.Add(item);
        }
        public bool ProvieneBandejaTramites()
        {
            bool result = false;

            var ultimoItem = history.Where(x => x.Path.Contains("ConsultarTramites") || x.Path.Contains("BandejaTramites") || x.Path.Contains("Consultas/Tramites"))
                                             .OrderByDescending(o => o.Timestamp)
                                             .FirstOrDefault()?.Path;

            if (ultimoItem == "BandejaTramites")
                result = true;

            return result;
        }

        public bool ProvieneConsultaTramites()
        {
            bool result = false;

            var ultimoItem = history.Where(x => x.Path.Contains("ConsultarTramites") || x.Path.Contains("BandejaTramites") || x.Path.Contains("Consultas/Tramites"))
                                             .OrderByDescending(o => o.Timestamp)
                                             .FirstOrDefault()?.Path;

            if (ultimoItem == "Consultas/Tramites")
                result = true;

            return result;
        }

    }

    public class RouteHistoryItem
    {
        public string Path { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
