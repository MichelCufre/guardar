using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Factories;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AutomatismoProductosAsociadosRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly AutomatismoMapper _mapper;

        public AutomatismoProductosAsociadosRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new AutomatismoMapper(new AutomatismoFactory());
        }

        #region Any
        public virtual bool HasZonaAutomatismo(string zona)
        {
            return this._context.V_PRODUCTOS_ASOCIADOS_AUTOMATISMO.AsNoTracking().Any(w => w.CD_ZONA_UBICACION == zona);
        }
        #endregion

        #region Get
        public virtual List<UbicacionPickingZonaAutomatismo> GetZonasProductosAsociadoAutomatismo(string producto, int empresa)
        {
            List<UbicacionPickingZonaAutomatismo> listObj = new List<UbicacionPickingZonaAutomatismo>();

            var listEntity = this._context.V_PRODUCTOS_ASOCIADOS_AUTOMATISMO
                                          .AsNoTracking()
                                          .Where(paa => paa.CD_EMPRESA == empresa
                                                     && paa.CD_PRODUTO == producto)
                                          .ToList();

            if (listEntity != null)
            {
                foreach (var item in listEntity)
                    listObj.Add(_mapper.MapToObject(item));
            }

            return listObj;
        }


        #endregion

        #region Add

        #endregion

        #region Update
        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
