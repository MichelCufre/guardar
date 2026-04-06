using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.Eventos.Enums;
using WIS.Extension;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Evento
{
    public class AdjuntarArchivoQuery : QueryObject<V_ARCHIVO_ADJUNTO, WISDB>
    {
        protected int? _cdEmpresa;
        protected string _referencia1;
        protected string _referencia2;
        protected string _tpDocumento;
        protected string _vlFilter;
        protected string _flAux;
        protected List<string> _averias;

        private List<string> _ManejosProducto = new List<string>() { "AVE", "PRO", "TAR" };

        public AdjuntarArchivoQuery(int cdEmpresa, string referencia1, string referencia2)
        {
            this._cdEmpresa = cdEmpresa;
            this._referencia1 = referencia1;
            this._referencia2 = referencia2;
            _flAux = "I";
        }

        public AdjuntarArchivoQuery(string cdEmpresa, string referencia1, string tpDocumento, string vlFilter)
        {

            this._cdEmpresa = cdEmpresa.ToNumber<int?>();
            this._referencia1 = referencia1;
            this._tpDocumento = tpDocumento;
            this._vlFilter = vlFilter;

            this._flAux = "F";
        }

        public AdjuntarArchivoQuery(List<string> averias)
        {
            this._averias = averias;
            _flAux = "A";
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ARCHIVO_ADJUNTO.AsNoTracking().Where(w => w.CD_SITUACAO == (short)EstadoArchivo.Activo);

            if (_flAux == "F")
            {

                this.Query = this.Query.Where(w => _ManejosProducto.Contains(w.CD_MANEJO));

                if (_cdEmpresa != null)
                {
                    this.Query = this.Query.Where(w => w.CD_EMPRESA == _cdEmpresa);
                }

                if (!string.IsNullOrEmpty(_referencia1))
                {
                    this.Query = this.Query.Where(w => w.CD_MANEJO == _referencia1);
                }

                if (!string.IsNullOrEmpty(_tpDocumento))
                {
                    this.Query = this.Query.Where(w => w.TP_DOCUMENTO == _tpDocumento);
                }

                if (!string.IsNullOrEmpty(_vlFilter))
                {
                    this.Query = this.Query.Where(w =>
                                                    w.DS_REFERENCIA.ToLower().Contains(_vlFilter.ToLower())
                                                    || w.DS_REFERENCIA2.ToLower().Contains(_vlFilter.ToLower())
                                                  );
                }
            }
            else if (_flAux == "A")
            {
                this.Query = this.Query.Where(w =>

                _averias.Contains(w.DS_REFERENCIA)

                                                            );
            }
            else if (_flAux == "I")
            {
                this.Query = this.Query.Where(w =>
                                                    w.CD_EMPRESA == _cdEmpresa
                                                    && w.CD_MANEJO == _referencia1
                                                    && w.DS_REFERENCIA == _referencia2
                                              );
            }

        }

        public virtual List<EVT000SelectionQuery> GetKeysRowsSelected(bool allSelected, List<string> keys)
        {

            if (allSelected)
            {
                return this.GetResult()
                    .Select(s => string.Join("$", new string[] { s.NU_ARCHIVO_ADJUNTO.ToString(), s.CD_EMPRESA.ToString() }))
                    .Except(keys)
                    .Select(s => new EVT000SelectionQuery(s.Split('$')))
                    .ToList();
            }
            else
            {
                return this.GetResult()
                    .Select(s => string.Join("$", new string[] { s.NU_ARCHIVO_ADJUNTO.ToString(), s.CD_EMPRESA.ToString() }))
                    .Intersect(keys)
                    .Select(s => new EVT000SelectionQuery(s.Split('$')))
                    .ToList();
            }
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }

    public class EVT000SelectionQuery
    {
        public EVT000SelectionQuery(string[] split)
        {
            NU_ARCHIVO_ADJUNTO = split[0].ToNumber<long>();
            CD_EMPRESA = split[1].ToNumber<int>();

        }

        public long NU_ARCHIVO_ADJUNTO { get; set; }
        public int CD_EMPRESA { get; set; }

    }
}
