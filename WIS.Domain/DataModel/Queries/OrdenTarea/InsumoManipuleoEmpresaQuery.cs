using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.OrdenTarea
{
    public class InsumoManipuleoEmpresaQuery : QueryObject<V_REG100_EMPRESAS, WISDB>
    {
        protected readonly int? _idUsuario;
        protected readonly string _cdInsumoManipuleo;
        protected readonly bool _asociado;

        public InsumoManipuleoEmpresaQuery()
        {
        }

        public InsumoManipuleoEmpresaQuery(int? idUsuario, string cdInsumoManipuleo, bool asociado)
        {
            this._idUsuario = idUsuario;
            this._cdInsumoManipuleo = cdInsumoManipuleo;
            this._asociado = asociado;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG100_EMPRESAS
                .Where(e => context.T_EMPRESA_FUNCIONARIO
                    .Any(f => f.CD_EMPRESA == e.CD_EMPRESA && f.USERID == this._idUsuario));

            if (this._cdInsumoManipuleo != null)
            {
                if (this._asociado)
                    this.Query = this.Query
                        .Where(x => context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Any(ime => ime.CD_INSUMO_MANIPULEO == _cdInsumoManipuleo && ime.CD_EMPRESA == x.CD_EMPRESA));
                else
                    this.Query = this.Query
                        .Where(x => !context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Any(ime => ime.CD_INSUMO_MANIPULEO == _cdInsumoManipuleo && ime.CD_EMPRESA == x.CD_EMPRESA));
            }
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<int> GetEmpresas()
        {
            return this.Query.Select(x => x.CD_EMPRESA).ToList();
        }
    }
}
