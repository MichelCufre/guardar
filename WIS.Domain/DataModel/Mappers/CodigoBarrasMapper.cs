using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class CodigoBarrasMapper : Mapper
    {

        public CodigoBarrasMapper()
        {


        }

        public virtual T_CODIGO_BARRAS MapToEntity(CodigoBarras codigo)
        {
            return new T_CODIGO_BARRAS
            {
                CD_BARRAS = codigo.Codigo,
                CD_EMPRESA = codigo.Empresa,
                CD_PRODUTO = codigo.Producto,
                DT_ADDROW = codigo.FechaInsercion,
                DT_UPDROW = codigo.FechaModificacion,
                NU_PRIORIDADE_USO = codigo.PrioridadUso,
                QT_EMBALAGEM = codigo.CantidadEmbalaje,
                TP_CODIGO_BARRAS = codigo.TipoCodigo,
                NU_TRANSACCION = codigo.NumeroTransaccion,
                NU_TRANSACCION_DELETE = codigo.NumeroTransaccionDelete,
            };
        }

        public virtual CodigoBarras MapToObject(T_CODIGO_BARRAS codigo)
        {
            return new CodigoBarras
            {
                Codigo = codigo.CD_BARRAS,
                Empresa = codigo.CD_EMPRESA,
                Producto = codigo.CD_PRODUTO,
                FechaInsercion = codigo.DT_ADDROW,
                FechaModificacion = codigo.DT_UPDROW,
                PrioridadUso = codigo.NU_PRIORIDADE_USO,
                CantidadEmbalaje = codigo.QT_EMBALAGEM,
                TipoCodigo = codigo.TP_CODIGO_BARRAS,
                NumeroTransaccion = codigo.NU_TRANSACCION,
                NumeroTransaccionDelete = codigo.NU_TRANSACCION_DELETE
            };
        }
        public virtual List<CodigoBarras> Map(CodigosBarrasRequest request)
        {
            List<CodigoBarras> codigos = new List<CodigoBarras>();

            foreach (var c in request.CodigosDeBarras)
            {
                CodigoBarras codigo = new CodigoBarras(c.TipoOperacion);
                codigo.Codigo = c.Codigo;
                codigo.Producto = c.Producto;
                codigo.TipoCodigo = c.TipoCodigo;
                codigo.PrioridadUso = c.PrioridadUso;
                codigo.CantidadEmbalaje = c.CantidadEmbalaje;
                codigo.Empresa = request.Empresa;
                codigos.Add(codigo);
            }
            return codigos;
        }
    }
}
