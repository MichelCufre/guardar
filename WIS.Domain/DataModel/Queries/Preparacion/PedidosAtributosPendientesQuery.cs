using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PedidosAtributosPendientesQuery : QueryObject<V_PRE100_ATRIBUTOS_DEFINIDOS, WISDB>
    {

        protected string _pedido;
        protected int _empresa;
        protected string _cliente;
        protected string _idEspecificaIdentificador;
        protected string _idLpnExteno;
        protected string _lpnTipo;
        protected string _producto;
        protected string _identificador;
        protected long _nuDetPedSaiAtrib;
        protected decimal _faixa;
        public PedidosAtributosPendientesQuery(string pedido, int empresa, string cliente, string idEspecificaIdentificador, string producto, string identificador, decimal faixa, long nuDetPedSaiAtrib)
        {
            this._pedido = pedido;
            this._empresa = empresa;
            this._cliente = cliente;
            this._idEspecificaIdentificador = idEspecificaIdentificador;

            this._producto = producto;
            this._identificador = identificador;
            this._faixa = faixa;
            this._nuDetPedSaiAtrib = nuDetPedSaiAtrib;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE100_ATRIBUTOS_DEFINIDOS.AsNoTracking();

            this.Query = this.Query.Where(x => x.NU_PEDIDO == _pedido && x.CD_EMPRESA == _empresa && x.CD_CLIENTE == _cliente && x.ID_ESPECIFICA_IDENTIFICADOR == _idEspecificaIdentificador
                                               && x.CD_PRODUTO == _producto && x.NU_IDENTIFICADOR == _identificador && x.CD_FAIXA == _faixa && x.NU_DET_PED_SAI_ATRIB == _nuDetPedSaiAtrib);


        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
