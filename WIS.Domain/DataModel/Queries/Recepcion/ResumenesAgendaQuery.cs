using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
	public class ResumenesAgendaQuery : QueryObject<V_RESUMEN_AGENDA_WREC250, WISDB>
    {
        protected readonly FiltrosAgendaResumen _filtros;

        public ResumenesAgendaQuery(FiltrosAgendaResumen filtro)
        {
            _filtros = filtro;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_RESUMEN_AGENDA_WREC250.AsNoTracking();
            int user = -1;
            int empresa = -1;
            if (!string.IsNullOrEmpty(_filtros.USERID))
            {
                user = int.Parse(_filtros.USERID);
            }
            if (!string.IsNullOrEmpty(_filtros.CD_EMPRESA))
            {
                empresa = int.Parse(_filtros.CD_EMPRESA);
            }

            if (user >= 0 && string.IsNullOrEmpty(_filtros.CD_EMPRESA))
            {
                var empresas = context.T_EMPRESA_FUNCIONARIO.AsNoTracking().Where(d => d.USERID == user)
               .Select(d => d.CD_EMPRESA).ToList();
                this.Query = this.Query.Where(x => empresas.Contains(x.CD_EMPRESA ?? 0));
            }
            else if (empresa >= 0)
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == empresa);
            }

            if (!string.IsNullOrEmpty(_filtros.NU_PREDIO) && !_filtros.NU_PREDIO.Equals(GeneralDb.PredioSinDefinir))
            {

                this.Query = this.Query.Where(x => x.NU_PREDIO == _filtros.NU_PREDIO);
            }
            if (string.IsNullOrEmpty(_filtros.FL_CERRADO) || (!string.IsNullOrEmpty(_filtros.FL_CERRADO) && _filtros.FL_CERRADO.Equals("N")))
            {
                this.Query = this.Query.Where(x => x.NU_COLOR < 6);

            }
        }
        public virtual List<CantidadResumenAgenda> GetCantidad(IUnitOfWork uow, string campo = null)
        {

            return this.Query.GroupBy(w => w.NU_COLOR).Select(s => new CantidadResumenAgenda
            {
                NU_COLOR = s.Key,
                CANTIDAD = s.Count()

            }).ToList();



        }
        public class CantidadResumenAgenda
        {
            public decimal? NU_COLOR;
            public int CANTIDAD;
        }

        /*  public virtual string GetCantidad(IUnitOfWork uow, string campo)
          {
              try
              {
                  switch (campo)
                  {
                      case "QT_ABIERTAS": return this.Query.Where(w => w.NU_COLOR == 1).ToList().Count().ToString();
                      case "QT_LIBERADAS": return this.Query.Where(w => w.NU_COLOR == 2).ToList().Count().ToString();
                      case "QT_RECEPCION": return this.Query.Where(w => w.NU_COLOR == 3).ToList().Count().ToString();
                      case "QT_RECIBIDAS": return this.Query.Where(w => w.NU_COLOR == 4).ToList().Count().ToString();
                      case "QT_CON_RESPON": return this.Query.Where(w => w.NU_COLOR == 5).ToList().Count().ToString();
                      default: return "0";
                  }

              }
              catch (Exception ex)
              {
              }
              return "0";
          }*/

        public virtual List<Agenda> GetAgendas()
        {
            return this.Query.Where(w => w.NU_COLOR == 1 || w.NU_COLOR == 5).Select(r => new Agenda
            {
                Id = r.NU_AGENDA,
                FuncionarioResponsable = r.CD_FUN_RESP,

            }).ToList();
        }

        public virtual List<string[]> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_AGENDA.ToString() }))
                .Intersect(keysToSelect)
                .Select(w => w.Split('$'))
                .ToList();
        }

        public virtual List<string[]> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.GetResult()
                .Select(r => string.Join("$", new string[] { r.NU_AGENDA.ToString() }))
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
}
