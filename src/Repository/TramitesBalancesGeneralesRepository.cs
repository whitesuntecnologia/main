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
    public class TramitesBalancesGeneralesRepository : BaseRepository<TramitesBalancesGenerales>
    {
        public TramitesBalancesGeneralesRepository(IUnitOfWork unit) : base(unit)
        {
        }

        public async Task CopiarDatosEmpresaToEvaluador(int IdTramite)
        {
            //Solo se copia la primera vez  que se presenta el tramite
            if (!await Context.TramitesFormulariosEvaluados.AnyAsync(x => x.IdTramite == IdTramite))
            {
                var lstEntity = await Context.TramitesBalancesGenerales.Where(x => x.IdTramite == IdTramite).ToListAsync();
                lstEntity.ForEach(x =>
                {
                    x.OoficDepositosCortoPlazo = x.OoficDepositosCortoPlazoEmp;
                    x.OoficDepositosLargoPlazo = x.OoficDepositosLargoPlazoEmp;
                    x.OoficInversionesCortoPlazo = x.OoficInversionesCortoPlazoEmp;
                    x.OoficInversionesLargoPlazo = x.OoficInversionesLargoPlazoEmp;
                    x.OoficOtrosConceptos = x.OoficOtrosConceptosEmp;
                    x.OpartDepositosCortoPlazo = x.OpartDepositosCortoPlazoEmp;
                    x.OpartDepositosLargoPlazo = x.OpartDepositosLargoPlazoEmp;
                    x.OpartInversionesCortoPlazo = x.OpartInversionesCortoPlazoEmp;
                    x.OpartInversionesLargoPlazo = x.OpartInversionesLargoPlazoEmp;
                    x.OpartInversiones = x.OpartInversionesEmp;
                    x.OpartOtrosConceptos = x.OpartOtrosConceptosEmp;
                    x.DispCajayBancos = x.DispCajayBancosEmp;
                    x.BusoInversiones = x.BusoInversionesEmp;
                    x.BusoMaqUtilAfec = x.BusoMaqUtilAfecEmp;
                    x.BusoMaqUtilNoAfec = x.BusoMaqUtilNoAfecEmp;
                    x.BusoBienesRaicesAfec = x.BusoBienesRaicesAfecEmp;
                    x.BusoBienesRaicesNoAfec = x.BusoBienesRaicesNoAfecEmp;
                    x.BusoOtrosConceptos = x.BusoOtrosConceptosEmp;
                    x.BcamMateriales = x.BcamMaterialesEmp;
                    x.BcamOtrosConceptos = x.BcamOtrosConceptosEmp;
                    x.DeuCortoPlazo = x.DeuCortoPlazoEmp;
                    x.DeuLargoPlazo = x.DeuLargoPlazoEmp;
                });
                await UpdateARangeAsync(lstEntity);
            }
        }
    }
}

