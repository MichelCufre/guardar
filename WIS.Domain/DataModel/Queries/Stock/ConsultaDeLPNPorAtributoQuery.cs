using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;


namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaDeLPNPorAtributoQuery : QueryObject<V_STO710_CONSULTA_LPN_ATRIBUTOS, WISDB>
    {
        protected long _numeroLPN;
        protected string _detalle;
        protected string _idDetalle;

        public ConsultaDeLPNPorAtributoQuery(long numeroLPN = 0, string detalle = "", string idDetalle = null)
        {
            this._numeroLPN = numeroLPN;
            this._detalle = detalle;
            this._idDetalle = idDetalle;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO710_CONSULTA_LPN_ATRIBUTOS.AsNoTracking();

            if (this._numeroLPN != 0)
            {
                this.Query = this.Query.Where(d => d.NU_LPN == this._numeroLPN);
            }

            if (this._detalle == "true")
            {
                this.Query = this.Query.Where(d => d.TP_ATRIBUTO_ASOCIADO == TipoAtributoAsociadoDb.DETALLE);
            }
            else if (this._detalle == "false")
			{
                this.Query = this.Query.Where(d => d.TP_ATRIBUTO_ASOCIADO == TipoAtributoAsociadoDb.CABEZAL);
            }

            if (int.TryParse(this._idDetalle, out int idLpnDet)) 
            {
                this.Query = this.Query.Where(d => d.ID_LPN_DET == idLpnDet);
            }
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}