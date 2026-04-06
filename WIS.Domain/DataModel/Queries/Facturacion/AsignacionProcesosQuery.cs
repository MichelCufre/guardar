using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class AsignacionProcesosQuery : QueryObject<V_FACTURACION_PROC_WFAC010, WISDB>
    {
        protected string _parcial;
        protected bool _asociar;
        protected string _nuEjecucion;

        public AsignacionProcesosQuery(string parcial, bool asociar, string nuEjecucion)
        {
            this._parcial = parcial;
            this._asociar = asociar;
            this._nuEjecucion = nuEjecucion;
        }

        public override void BuildQuery(WISDB context)
        {
            //Si el proceso es parcial solo ve los parciales, de lo contario con totales muestra todos
            //Si es la grid de asociar se muestra solo los procesos asociados, de lo contrario muestra los disponibles a asociar
            if (this._asociar)
            {
                this.Query = context.V_FACTURACION_PROC_WFAC010
                    .GroupJoin(context.T_FACTURACION_EJEC_EMPRESA
                        .Where(fee => fee.NU_EJECUCION == int.Parse(this._nuEjecucion)),
                        fp => new { fp.CD_EMPRESA, fp.CD_PROCESO },
                        fee => new { fee.CD_EMPRESA, fee.CD_PROCESO },
                        (fp, fees) => new { Fp = fp, Fees = fees })
                    .SelectMany(fpfees => fpfees.Fees.DefaultIfEmpty(), (fpfees, fee) => new { Fp = fpfees.Fp, Fee = fee })
                    .Where(fpfee => fpfee.Fee == null
                        && (this._parcial != "S" || fpfee.Fp.FL_EJEC_POR_HORA == "S"))
                    .Select(s => s.Fp);
            }
            else
            {
                this.Query = context.V_FACTURACION_PROC_WFAC010
                    .Join(context.T_FACTURACION_EJEC_EMPRESA
                        .Where(fee => fee.NU_EJECUCION == int.Parse(this._nuEjecucion)),
                        fp => new { fp.CD_EMPRESA, fp.CD_PROCESO },
                        fee => new { fee.CD_EMPRESA, fee.CD_PROCESO },
                        (fp, fee) => new { Fp = fp, Fee = fee })
                    .Where(fpfee => this._parcial != "S" || fpfee.Fp.FL_EJEC_POR_HORA == "S")
                    .Select(fpfee => fpfee.Fp);
            }
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
                .Select(r => string.Join("$", new string[] { r.CD_EMPRESA.ToString(), r.CD_PROCESO }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.CD_EMPRESA.ToString(), r.CD_PROCESO }))
                .Except(keysToExclude)
                .Select(w => w.Split('$'))
                .ToList();
        }
    }
}
