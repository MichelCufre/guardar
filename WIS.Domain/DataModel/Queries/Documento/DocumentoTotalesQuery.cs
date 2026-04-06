using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoTotalesQuery : QueryObject<V_DOC_SALDO_TEMP_AUX, WISDB>
    {
        protected readonly string _nuDocumento;
        protected readonly string _tpDocumento;

        public DocumentoTotalesQuery(string nuDocumento, string tpDocumento)
        {
            this._nuDocumento = nuDocumento;
            this._tpDocumento = tpDocumento;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOC_SALDO_TEMP_AUX
                .Where(d => d.NU_DOCUMENTO == this._nuDocumento && d.TP_DOCUMENTO == this._tpDocumento);
        }

        public virtual TotalesDocumento GetTotales()
        {
            if (this.Query != null)
            {
                var result = this.Query.FirstOrDefault();
                if (result != null)
                {
                    return new TotalesDocumento
                    {
                        NumeroDocumento = result.NU_DOCUMENTO,
                        TipoDocumento = result.TP_DOCUMENTO,
                        Lineas = result.QT_LINEAS ?? 0,
                        Productos = result.QT_PRODUCTOS ?? 0,
                        Desafectada = result.QT_DESAFECTADA ?? 0,
                        CIF = result.VL_CIF ?? 0
                    };
                }
                else {
                    return null;
                }
            }
            throw new InvalidOperationException("Query no esta cargada. Utilizar HandleQuery antes de ejecutar esta operación");
        }
    }
}
