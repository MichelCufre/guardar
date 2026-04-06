using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class PickeosPendientesQuery : QueryObject<V_PRODUTOS_SIN_PREP_PED_SINCAM, WISDB>
    {
        protected readonly string _nuPedido;
        protected readonly string _cdCliente;
        private readonly int _empresa = -1;

        public PickeosPendientesQuery(string nuPedido, string cdCliente, int empresa)
        {
            _nuPedido = nuPedido;
            _cdCliente = cdCliente;
            _empresa = empresa;
        }

        public PickeosPendientesQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRODUTOS_SIN_PREP_PED_SINCAM.AsNoTracking();

            if (!string.IsNullOrEmpty(_nuPedido) &&
                !string.IsNullOrEmpty(_cdCliente) &&
                _empresa != -1)
                this.Query = this.Query.Where(x => x.NU_PEDIDO == _nuPedido && x.CD_CLIENTE == _cdCliente && x.CD_EMPRESA == _empresa);

        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
