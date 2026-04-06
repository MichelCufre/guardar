using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking.Dtos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class DetallePedidoLpnAtributosAsociadosQuery : QueryObject<V_PRE100_ATRIBUTOS_LPN_DEFINIDOS, WISDB>
    {
        protected readonly DetallePedidoLpnEspecifico _datos;

        public DetallePedidoLpnAtributosAsociadosQuery(DetallePedidoLpnEspecifico datos)
        {
            this._datos = datos;
        }


        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE100_ATRIBUTOS_LPN_DEFINIDOS
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
                    && (a.USERID == _datos.UserId.Value || a.USERID == null));

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }


        public virtual List<KeyAtributoAsignado> GetSelectedKeys(List<string> keysToSelect, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[]
                {
                    r.NU_PEDIDO,
                    r.CD_CLIENTE,
                    r.CD_EMPRESA.ToString(),
                    r.CD_PRODUTO,
                    r.CD_FAIXA.ToString(formatProvider),
                    r.NU_IDENTIFICADOR,
                    r.ID_ESPECIFICA_IDENTIFICADOR,
                    r.TP_LPN_TIPO,
                    r.ID_LPN_EXTERNO,
                    r.NU_DET_PED_SAI_ATRIB.ToString(),
                    r.ID_ATRIBUTO.ToString(),
                    r.FL_CABEZAL
                }))
                .Intersect(keysToSelect)
                .Select(w => SelectionQuery(w, formatProvider))
                .ToList();
        }

        public virtual List<KeyAtributoAsignado> GetSelectedKeysAndExclude(List<string> keysToExclude, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[]
                {
                    r.NU_PEDIDO,
                    r.CD_CLIENTE,
                    r.CD_EMPRESA.ToString(),
                    r.CD_PRODUTO,
                    r.CD_FAIXA.ToString(formatProvider),
                    r.NU_IDENTIFICADOR,
                    r.ID_ESPECIFICA_IDENTIFICADOR,
                    r.TP_LPN_TIPO,
                    r.ID_LPN_EXTERNO,
                    r.NU_DET_PED_SAI_ATRIB.ToString(),
                    r.ID_ATRIBUTO.ToString(),
                    r.FL_CABEZAL
                }))
                .Except(keysToExclude)
                .Select(w => SelectionQuery(w, formatProvider))
                .ToList();
        }

        public virtual KeyAtributoAsignado SelectionQuery(string key, IFormatProvider formatProvider)
        {
            var keys = key.Split('$');
            return new KeyAtributoAsignado()
            {
                Pedido = keys[0],
                Cliente = keys[1],
                Empresa = int.Parse(keys[2]),
                Producto = keys[3],
                Faixa = decimal.Parse(keys[4], formatProvider),
                Identificador = keys[5],
                IdEspecificaIdentificador = keys[6],
                TipoLpn = keys[7],
                IdExternoLpn = keys[8],
                IdConfiguracion = long.Parse(keys[9]),
                IdAtributo = int.Parse(keys[10]),
                IdCabezal = keys[11],
            };
        }
    }
}
