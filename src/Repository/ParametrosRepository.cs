using DataAccess.Entities;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class ParametrosRepository : BaseRepository<Parametros>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ParametrosRepository(IUnitOfWork unit) : base(unit)
        {
            _unitOfWork = unit;
        }

        public string GetParametroChar(string CodParam)
        {

            var entity = _unitOfWork.Context.Parametros.FirstOrDefault(x => x.CodigoParametro == CodParam);
            if (entity == null)
                throw new ArgumentException(string.Format("No se encuentra configurado el parámetro {0}", CodParam));
            
            return entity.ValorChar;
        }
        public decimal? GetParametroNum(string CodParam)
        {

            var entity = _unitOfWork.Context.Parametros.FirstOrDefault(x => x.CodigoParametro == CodParam);
            if (entity == null)
                throw new ArgumentException(string.Format("No se encuentra configurado el parámetro {0}", CodParam));

            return entity.ValorNum;
        }
    }
}
