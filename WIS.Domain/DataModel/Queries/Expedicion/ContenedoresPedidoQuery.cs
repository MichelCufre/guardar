using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class ContenedoresPedidoQuery : QueryObject<V_CONT_SIN_EMBARCAR_PED_SINCAM, WISDB>
    {
        protected readonly List<int> _contenedores = new List<int>();
        protected readonly string _nuPedido;
        protected readonly string _cdCliente;
        protected readonly int _empresa = -1;

        public ContenedoresPedidoQuery(int? nuContenedorOrigen, int? nuContenedorDestino, string nuPedido, string cdCliente, int empresa)
        {
            if (nuContenedorOrigen != null && nuContenedorOrigen != -1)
                _contenedores.Add(nuContenedorOrigen.Value);

            if (nuContenedorDestino != null)
                _contenedores.Add(nuContenedorDestino.Value);

            _nuPedido = nuPedido;
            _cdCliente = cdCliente;
            _empresa = empresa;
        }

        public ContenedoresPedidoQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CONT_SIN_EMBARCAR_PED_SINCAM.AsNoTracking();

            if (_contenedores.Count > 0)
                this.Query = this.Query.Where(x => !_contenedores.Contains(x.NU_CONTENEDOR));

            if (!string.IsNullOrEmpty(_nuPedido))
                this.Query = this.Query.Where(x => x.NU_PEDIDO == _nuPedido);

            if (!string.IsNullOrEmpty(_cdCliente))
                this.Query = this.Query.Where(x => x.CD_CLIENTE == _cdCliente);

            if (_empresa != -1)
                this.Query = this.Query.Where(x => x.CD_EMPRESA == _empresa);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
