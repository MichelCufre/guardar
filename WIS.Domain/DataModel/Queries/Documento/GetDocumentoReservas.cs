using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetDocumentoReservas : QueryObject<T_DOCUMENTO_PREPARACION_RESERV, WISDB>
    {
        protected readonly string _producto;
        protected readonly string _identificador;
        protected readonly int _preparacion;
        protected readonly int _empresa;


        public GetDocumentoReservas(int preparacion, string producto, string identificador,int empresa)
        {
            this._identificador = identificador;
            this._producto = producto;
            this._preparacion = preparacion;
            this._empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_DOCUMENTO_PREPARACION_RESERV
                .Where(r => r.NU_IDENTIFICADOR_PICKING_DET == this._identificador 
                    && r.CD_PRODUTO == this._producto 
                    && r.NU_PREPARACION == this._preparacion 
                    && r.CD_EMPRESA == this._empresa);
        }
    }
}
