using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
  public class ProductoCodigoBarraMapper
    {
        public ProductoCodigoBarraMapper()
        {

        }

        public virtual ProductoCodigoBarra MapToObject(T_CODIGO_BARRAS barraEntity)
        {
            return new ProductoCodigoBarra
            {
                CodigoBarra = barraEntity.CD_BARRAS,
                IdEmpresa = barraEntity.CD_EMPRESA,
                IdProducto = barraEntity.CD_PRODUTO,
                IdTipoCodigoBarra = barraEntity.TP_CODIGO_BARRAS ?? 0,
                FechaInsercion = barraEntity.DT_ADDROW,
                FechaModificacion = barraEntity.DT_UPDROW,
                CantEmbalaje = barraEntity.QT_EMBALAGEM ?? 0,
                NumPrioridadeUso = barraEntity.NU_PRIORIDADE_USO ?? 0,
                TipoCodigoDeBarra = MapToObject(barraEntity.T_TIPO_CODIGO_BARRAS),
                NumeroTransaccion = barraEntity.NU_TRANSACCION,
                NumeroTransaccionDelete = barraEntity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual ProductoCodigoBarraTipo MapToObject(T_TIPO_CODIGO_BARRAS tpBarrasEntity)
        {
            return new ProductoCodigoBarraTipo
            {
                Id = tpBarrasEntity.TP_CODIGO_BARRAS,
                Descripcion = tpBarrasEntity.DS_CODIGO_BARRAS,
                FechaInsercion = tpBarrasEntity.DT_ADDROW,
                FechaModificacion = tpBarrasEntity.DT_UPDROW
            };
        }

        public virtual T_CODIGO_BARRAS MapToEntity(ProductoCodigoBarra prodCodigoBarra)
        {
            return new T_CODIGO_BARRAS
            {
              CD_BARRAS = prodCodigoBarra.CodigoBarra,
              CD_EMPRESA = prodCodigoBarra.IdEmpresa,
              CD_PRODUTO = prodCodigoBarra.IdProducto,
              TP_CODIGO_BARRAS = prodCodigoBarra.IdTipoCodigoBarra,
              DT_ADDROW = prodCodigoBarra.FechaInsercion,
              DT_UPDROW = prodCodigoBarra.FechaModificacion,
              QT_EMBALAGEM = prodCodigoBarra.CantEmbalaje,
              NU_PRIORIDADE_USO = prodCodigoBarra.NumPrioridadeUso,
              NU_TRANSACCION = prodCodigoBarra.NumeroTransaccion,
              NU_TRANSACCION_DELETE = prodCodigoBarra.NumeroTransaccionDelete
            };
        }

        public virtual T_TIPO_CODIGO_BARRAS MapToEntity(ProductoCodigoBarraTipo tpProdCodigoBarra)
        {
            return new T_TIPO_CODIGO_BARRAS
            {
                TP_CODIGO_BARRAS = tpProdCodigoBarra.Id,
                DS_CODIGO_BARRAS = tpProdCodigoBarra.Descripcion,
                DT_ADDROW = tpProdCodigoBarra.FechaInsercion,
                DT_UPDROW = tpProdCodigoBarra.FechaModificacion
            };
        }
    }
}
