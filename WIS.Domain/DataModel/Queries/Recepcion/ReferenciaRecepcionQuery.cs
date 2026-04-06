using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class ReferenciaRecepcionQuery : QueryObject<V_REC010_RECEPCION_REFERENCIA, WISDB>
    {
        protected readonly int? _agenda;

        public ReferenciaRecepcionQuery()
        {
        }
        public ReferenciaRecepcionQuery(int? agenda)
        {
            this._agenda = agenda;
        }

        public override void BuildQuery(WISDB context)
        {
            if (this._agenda != null)
            {
                var referencias = context.T_RECEPC_AGENDA_REFERENCIA_REL.Where(x => x.NU_AGENDA == this._agenda).Select(s => s.NU_RECEPCION_REFERENCIA).ToArray();

                this.Query = context.V_REC010_RECEPCION_REFERENCIA.Where(d => referencias.Contains(d.NU_RECEPCION_REFERENCIA)).Select(d => d);
            }
            else
            {
                this.Query = context.V_REC010_RECEPCION_REFERENCIA.Select(d => d);
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<ReferenciaRecepcion> GetReferenciasAsociadas()
        {
            var referencias = new List<ReferenciaRecepcion>();

            var entries = this.Query.ToList();

            foreach (var entry in entries)
            {
                referencias.Add(new ReferenciaRecepcion()
                {
                    Id = entry.NU_RECEPCION_REFERENCIA,
                    Numero = entry.NU_REFERENCIA,
                    Memo = entry.DS_MEMO
                });
            }

            return referencias;

        }

        public virtual List<ReferenciaRecepcion> GetByMemoOrNumeroPartial(string searchValue)
        {
            var referencias = new List<ReferenciaRecepcion>();

            var entries = this.Query
                .Where(d => d.DS_MEMO.ToLower().Contains(searchValue.ToLower()) || d.NU_REFERENCIA.ToLower().Contains(searchValue.ToLower()))
                .ToList();

            foreach (var entry in entries)
            {
                referencias.Add(new ReferenciaRecepcion()
                {
                    Id = entry.NU_RECEPCION_REFERENCIA,
                    Numero = entry.NU_REFERENCIA,
                    Memo = entry.DS_MEMO
                });
            }

            return referencias;

        }

    }
}
