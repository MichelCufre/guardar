using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Picking;
using WIS.Persistence.Database;
using WIS.Security;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class ResumenesPedidos : QueryObject<V_SEG_PED_PRE640, WISDB>
    {
        protected readonly FiltrosPedidosResumen _filtros;
        protected readonly IIdentityService _identity;

        public ResumenesPedidos(FiltrosPedidosResumen filtro, IIdentityService identity)
        {
            _filtros = filtro;
            _identity = identity;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_SEG_PED_PRE640.AsNoTracking();
            int responsable = -1;
            int empresa = -1;
            
            if (!string.IsNullOrEmpty(_filtros.USERID))
            {
                responsable = int.Parse(_filtros.USERID);
            }
            
            if (!string.IsNullOrEmpty(_filtros.CD_EMPRESA))
            {
                empresa = int.Parse(_filtros.CD_EMPRESA);
            }

            //if (responsable >= 0)
            //{
            //    this.Query = this.Query.Where(x => x.CD_FUN_RESP == responsable);
            //}

            if (empresa < 0)
            {
                var empresas = context.T_EMPRESA_FUNCIONARIO.AsNoTracking().Where(d => d.USERID == _identity.UserId)
               .Select(d => d.CD_EMPRESA).ToList();
                this.Query = this.Query.Where(x => empresas.Contains(x.CD_EMPRESA));
            }
            else
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == empresa);
            }

            if (!string.IsNullOrEmpty(_filtros.NU_PREDIO) && !_filtros.NU_PREDIO.Equals(GeneralDb.PredioSinDefinir))
            {
                string predio = _filtros.NU_PREDIO;
                this.Query = this.Query.Where(x => x.NU_PREDIO == predio || !_identity.Predio.Equals(GeneralDb.PredioSinDefinir) && x.NU_PREDIO == null);
            }

            if (string.IsNullOrEmpty(_filtros.FL_FINALIZADOS) || (!string.IsNullOrEmpty(_filtros.FL_FINALIZADOS) && _filtros.FL_FINALIZADOS.Equals("N")))
            {
                this.Query = this.Query.Where(x => x.ND_ACTIVIDAD.Equals("N"));
            }
        }

        public virtual List<CantidadResumenPedido> GetCantidad(IUnitOfWork uow, string campo = null)
        {
            return this.Query.GroupBy(w => w.STATUS_PEOR).Select(s => new CantidadResumenPedido
            {
                STATUS_PEOR = s.Key,
                CANTIDAD = s.Count()

            }).ToList();
        }

        public virtual List<Pedido> GetPedidos()
        {
            return this.Query.Where(w => w.STATUS_PEOR.Equals("Nuevo") || w.STATUS_PEOR.Equals("Nuevo Con Responsable")).Select(r => new Pedido
            {
                Id = r.NU_PEDIDO,
                Empresa = r.CD_EMPRESA,
                Cliente = r.CD_CLIENTE,
                //FuncionarioResponsable = r.CD_FUN_RESP,
            }).ToList();
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_PEDIDO, r.CD_CLIENTE, r.CD_EMPRESA.ToString() }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }

    public class CantidadResumenPedido
    {
        public string STATUS_PEOR;
        public int CANTIDAD;
    }

}
