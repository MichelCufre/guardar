using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class OprerativaAsociableQuery : QueryObject<V_REC275_OPERATIVAS_ASOCIADAS, WISDB>
    {
        protected readonly string _predio;
        protected readonly string _tpEntidad;
        protected readonly string _cdEntidad;
        protected readonly string _dsEntidad;
        protected readonly string _cdEmpresa;
        protected readonly string _nmEmpresa;
        protected readonly int? _nuEstrategia;

        public OprerativaAsociableQuery(string predio, string tpEntidad, string cdEntidad, string dsEntidad, string cdEmpresa, string nmEmpresa, int nuEstrategia )
        {
            _predio = predio;
            _tpEntidad = tpEntidad;
            _cdEntidad = cdEntidad;
            _dsEntidad = dsEntidad;
            _cdEmpresa = cdEmpresa;
            _nmEmpresa = nmEmpresa;
            _nuEstrategia = nuEstrategia;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC275_OPERATIVAS_ASOCIADAS
                .AsNoTracking()
                .Where(oa => (oa.NU_ALM_ESTRATEGIA == null || oa.NU_ALM_ESTRATEGIA == _nuEstrategia)
                    && (oa.TP_ENTIDAD == "-" || oa.TP_ENTIDAD == _tpEntidad)
                    && (oa.CD_ENTIDAD == "-" || oa.CD_ENTIDAD == _cdEntidad)
                    && (oa.NU_PREDIO == "-" || oa.NU_PREDIO == _predio)
                    && (oa.CD_EMPRESA == "-" || oa.CD_EMPRESA == _cdEmpresa));

            this.Query = this.Query
                .GroupBy(oa => new { oa.TP_RECEPCION, oa.TP_ALM_OPERATIVA_ASOCIABLE,oa.DS_ALM_OPERATIVA_ASOCIABLE })
                .Select(g => new V_REC275_OPERATIVAS_ASOCIADAS
                {
                    TP_RECEPCION = g.Key.TP_RECEPCION,
                    CD_ENTIDAD = _cdEntidad,
                    DS_ALM_ESTRATEGIA = g.Max(oa => oa.DS_ALM_ESTRATEGIA),
                    DS_TIPO_RECEPCION = g.Max(oa => oa.DS_TIPO_RECEPCION),
                    DS_ENTIDAD = _dsEntidad,
                    NU_ALM_ESTRATEGIA = g.Max(oa => oa.NU_ALM_ESTRATEGIA),
                    NU_PREDIO = _predio,
                    TP_ALM_OPERATIVA_ASOCIABLE = g.Key.TP_ALM_OPERATIVA_ASOCIABLE,
                    DS_ALM_OPERATIVA_ASOCIABLE = g.Key.DS_ALM_OPERATIVA_ASOCIABLE,
                    TP_ENTIDAD = _tpEntidad,
                    CD_EMPRESA = _cdEmpresa,
                    NM_EMPRESA = _nmEmpresa,
                });
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
