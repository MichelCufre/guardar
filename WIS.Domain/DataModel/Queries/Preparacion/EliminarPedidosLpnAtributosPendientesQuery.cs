using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class EliminarPedidosLpnAtributosPendientesQuery : QueryObject<V_PRE110_DET_PEDIDO_LPN_ATR, WISDB>
    {

        protected string _pedido;
        protected int _empresa;
        protected string _cliente;
        protected string _idEspecificaIdentificador;
        protected string _idLpnExteno;
        protected string _lpnTipo;
        protected string _producto;
        protected string _identificador;
        protected decimal _faixa;
        public EliminarPedidosLpnAtributosPendientesQuery(string pedido, int empresa, string cliente, string idEspecificaIdentificador, string idLpnExteno, string lpnTipo, string producto, string identificador, decimal faixa)
        {
            this._pedido = pedido;
            this._empresa = empresa;
            this._cliente = cliente;
            this._idEspecificaIdentificador = idEspecificaIdentificador;
            this._idLpnExteno = idLpnExteno;
            this._lpnTipo = lpnTipo;
            this._producto = producto;
            this._identificador = identificador;
            this._faixa = faixa;
        }
        public EliminarPedidosLpnAtributosPendientesQuery()
        {
            _pedido = "";
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE110_DET_PEDIDO_LPN_ATR.AsNoTracking();


            if (!string.IsNullOrEmpty(_pedido))
            {
                this.Query = this.Query.Where(x => x.NU_PEDIDO == _pedido && x.CD_EMPRESA == _empresa && x.CD_CLIENTE == _cliente && x.ID_ESPECIFICA_IDENTIFICADOR == _idEspecificaIdentificador
                                                    && x.ID_LPN_EXTERNO == _idLpnExteno && x.TP_LPN_TIPO == _lpnTipo && x.CD_PRODUTO == _producto && x.NU_IDENTIFICADOR == _identificador && x.CD_FAIXA == _faixa);
            }

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString(), r.CD_PRODUTO, r.CD_FAIXA.ToString(formatProvider), r.NU_IDENTIFICADOR, r.ID_ESPECIFICA_IDENTIFICADOR, r.TP_LPN_TIPO, r.ID_LPN_EXTERNO,r.NU_DET_PED_SAI_ATRIB.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude, IFormatProvider formatProvider)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString(), r.CD_PRODUTO, r.CD_FAIXA.ToString(formatProvider), r.NU_IDENTIFICADOR, r.ID_ESPECIFICA_IDENTIFICADOR, r.TP_LPN_TIPO, r.ID_LPN_EXTERNO, r.NU_DET_PED_SAI_ATRIB .ToString()}))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList(); //TODO: Ver de cambiar
        }

    }
}
