using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class AgendaDetalleQuery : QueryObject<V_REC171_AGENDA_DETALLE, WISDB>
    {
        protected readonly int? _idAgenda;
        protected readonly int? _idEmpresa;
        protected readonly string _idProducto;
        protected readonly bool _sinEtiqueta;

        public AgendaDetalleQuery(bool sinEtiqueta = false)
        {
            this._sinEtiqueta = sinEtiqueta;
        }
        public AgendaDetalleQuery(int idAgenda, int idEmpresa, bool sinEtiqueta = false)
        {
            this._idAgenda = idAgenda;
            this._idEmpresa = idEmpresa;
            this._sinEtiqueta = sinEtiqueta;
        }
        public AgendaDetalleQuery(int idAgenda, int idEmpresa, string idProducto, bool sinEtiqueta = false)
        {
            this._idAgenda = idAgenda;
            this._idEmpresa = idEmpresa;
            this._idProducto = idProducto;
            this._sinEtiqueta = sinEtiqueta;
        }

        public AgendaDetalleQuery(int agenda, bool sinEtiqueta = false)
        {
            this._idAgenda = agenda;
            this._sinEtiqueta = sinEtiqueta;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC171_AGENDA_DETALLE.AsNoTracking();

            if ((_idAgenda != null) && (_idEmpresa != null) && (_idProducto != null))
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == _idEmpresa && x.NU_AGENDA == _idAgenda && x.CD_PRODUTO == _idProducto);
            }
            else if ((_idAgenda != null) && (_idEmpresa != null))
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == _idEmpresa && x.NU_AGENDA == _idAgenda);
            }
            else if (_idAgenda != null)
            {
                this.Query = this.Query.Where(x => x.NU_AGENDA == this._idAgenda);
            }

            if (this._sinEtiqueta)
                this.Query = this.Query.Where(w => w.TP_BARRAS_DISTINTO_CERO == "N");

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }


        public virtual List<string> GetKeysAndExclude(List<string> ExcludeKeys)
        {
             return GetResult().Where(w => !ExcludeKeys.Contains($"{w.QT_ETIQUETA_IMPRIMIR}${w.NU_AGENDA}${w.CD_FAIXA}${w.NU_IDENTIFICADOR}${w.CD_PRODUTO}${w.CD_EMPRESA}${w.QT_AGENDADO}${w.QT_UNIDADES_BULTO}"))
                .Select(x =>
                $"{x.QT_ETIQUETA_IMPRIMIR}$" +
                $"{x.NU_AGENDA}$" +
                $"{x.CD_FAIXA}$" +
                $"{x.NU_IDENTIFICADOR}$" +
                $"{x.CD_PRODUTO}$" +
                $"{x.CD_EMPRESA}$" +
                $"{x.QT_AGENDADO}$" +
                $"{x.QT_UNIDADES_BULTO}").ToList();
        }  
    }
}