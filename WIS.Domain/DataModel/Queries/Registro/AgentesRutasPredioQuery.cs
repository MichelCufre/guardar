using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class AgentesRutasPredioQuery : QueryObject<V_REG220_CLIENTE_RUTA_PREDIO, WISDB>
    {
        protected readonly string _codigoInternoCliente;
        protected readonly int _idEmpresa;
        protected readonly int _idUsuario;


        public AgentesRutasPredioQuery(string codigoInternoCliente, int idEmpresa, int idUsuario)
        {
            this._codigoInternoCliente = codigoInternoCliente;
            this._idEmpresa = idEmpresa;
            this._idUsuario = idUsuario;
        }

        public override void BuildQuery(WISDB context)
        {
            var predios = context.V_PREDIO_USUARIO.Where(s => s.USERID == _idUsuario).Select(s => s.NU_PREDIO).ToList();

            this.Query = context.V_REG220_CLIENTE_RUTA_PREDIO.Where(d => d.CD_EMPRESA == _idEmpresa
                                                                      && d.CD_CLIENTE == _codigoInternoCliente
                                                                      && (d.NU_PREDIO == null || predios.Contains(d.NU_PREDIO))).Select(d => d);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }

}