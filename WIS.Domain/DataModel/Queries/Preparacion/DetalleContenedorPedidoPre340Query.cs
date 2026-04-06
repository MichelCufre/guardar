using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class DetalleContenedorPedidoPre340Query : QueryObject<V_CONTENEDOR_PEDIDO_CONSULTA, WISDB>
    {

        private readonly int _empresa = -1;
        protected readonly string _cdCliente;
        protected readonly string _nuPedido;


        public DetalleContenedorPedidoPre340Query(string nuPedido, string cdCliente, int empresa)
        {
            _nuPedido = nuPedido;
            _cdCliente = cdCliente;
            _empresa = empresa;
        }

        public DetalleContenedorPedidoPre340Query()
        {

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CONTENEDOR_PEDIDO_CONSULTA.AsNoTracking();

            this.Query = this.Query.Where(x => x.NU_PEDIDO == _nuPedido &&

                                               x.CD_EMPRESA == _empresa &&
                                               x.CD_CLIENTE == _cdCliente
                                              );

        }
    }
}
