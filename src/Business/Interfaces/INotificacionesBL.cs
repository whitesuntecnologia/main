using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface INotificacionesBL
    {
        Task<List<NotificacionDTO>> GetNotificacionesPorUsuario(string userid = null);
        Task MarcarComoLeido(int IdNotificacion);
        Task Notificar(int IdTramite, string Titulo, string Mensaje);
        Task<List<NotificacionDTO>> GetNotificaciones();
        Task<List<AlertaDTO>> GetAlertas(int? IdEmpresa = null);
    }
}
