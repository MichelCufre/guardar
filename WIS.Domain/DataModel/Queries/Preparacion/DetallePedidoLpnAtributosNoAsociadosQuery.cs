using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Picking.Dtos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class DetallePedidoLpnAtributosNoAsociadosQuery : QueryObject<V_PRE100_ATRIBUTOS_TIPO_SIN_DEFINIR, WISDB>
    {
        protected readonly DetallePedidoLpnEspecifico _datos;

        public DetallePedidoLpnAtributosNoAsociadosQuery(DetallePedidoLpnEspecifico datos)
        {
            this._datos = datos;
        }

        public override void BuildQuery(WISDB context)
        {
            var idAtributosDefinidos = context.V_PRE100_ATRIBUTOS_LPN_DEFINIDOS
                .AsNoTracking()
                .Where(a => a.NU_PEDIDO == this._datos.Pedido
                    && a.CD_CLIENTE == this._datos.Cliente
                    && a.CD_EMPRESA == this._datos.Empresa
                    && a.CD_PRODUTO == this._datos.Producto
                    && a.CD_FAIXA == this._datos.Faixa
                    && a.NU_IDENTIFICADOR == this._datos.Identificador
                    && a.ID_ESPECIFICA_IDENTIFICADOR == this._datos.IdEspecificaIdentificador
                    && a.TP_LPN_TIPO == this._datos.TipoLpn
                    && a.ID_LPN_EXTERNO == this._datos.IdExternoLpn
                    && a.FL_CABEZAL == (!_datos.Detalle ? "S" : "N")
                    && ((_datos.IdConfiguracion.HasValue) ? (a.NU_DET_PED_SAI_ATRIB == _datos.IdConfiguracion.Value || a.NU_DET_PED_SAI_ATRIB == -1) : a.NU_DET_PED_SAI_ATRIB == -1)
                    && (a.USERID == _datos.UserId.Value || a.USERID == null))
                .Select(a => a.ID_ATRIBUTO)
                .Distinct();

            this.Query = context.V_PRE100_ATRIBUTOS_TIPO_SIN_DEFINIR
                .Where(a => a.FL_CABEZAL == (!_datos.Detalle ? "S" : "N")
                    && a.TP_LPN_TIPO == this._datos.TipoLpn
                    && !idAtributosDefinidos.Contains(a.ID_ATRIBUTO))
                .OrderBy(a => a.NU_ORDEN);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.TP_LPN_TIPO, r.ID_ATRIBUTO.ToString(), r.FL_CABEZAL }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.TP_LPN_TIPO, r.ID_ATRIBUTO.ToString(), r.FL_CABEZAL }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList();
        }
    }
}
