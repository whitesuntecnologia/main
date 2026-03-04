using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public  class IndicesCostoConstruccionRepository : BaseRepository<IndicesCostoConstruccion>
    {
        
        public IndicesCostoConstruccionRepository(IUnitOfWork unit) : base(unit)
        {
        }

        public decimal? GetIndice(int Anio,int Mes)
        {
            return Context.IndicesCostoConstruccion.FirstOrDefault(x => x.Anio == Anio && x.Mes == Mes)?.Valor;
        }
        public decimal? GetIndice(string Periodo)
        {
            int Mes = Convert.ToInt32(Periodo.Split("/")[0]);
            int Anio = Convert.ToInt32(Periodo.Split("/")[1]);
            return Context.IndicesCostoConstruccion.FirstOrDefault(x => x.Anio == Anio && x.Mes == Mes)?.Valor;
        }
    }
}
