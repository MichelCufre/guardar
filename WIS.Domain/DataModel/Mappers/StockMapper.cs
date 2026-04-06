using WIS.Domain.General;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class StockMapper : Mapper
    {
        protected readonly AgenteMapper _mapperAgente;

        public StockMapper()
        {
            this._mapperAgente = new AgenteMapper();
        }

        public virtual Stock MapToStock(T_STOCK stockEntity)
        {
            return new Stock
            {
                Ubicacion = stockEntity.CD_ENDERECO,
                Empresa = stockEntity.CD_EMPRESA,
                Producto = stockEntity.CD_PRODUTO,
                Faixa = stockEntity.CD_FAIXA,
                Identificador = stockEntity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Cantidad = stockEntity.QT_ESTOQUE,
                ReservaSalida = stockEntity.QT_RESERVA_SAIDA,
                CantidadTransitoEntrada = stockEntity.QT_TRANSITO_ENTRADA,
                Vencimiento = stockEntity.DT_FABRICACAO,
                Averia = stockEntity.ID_AVERIA,
                Inventario = stockEntity.ID_INVENTARIO,
                FechaInventario = stockEntity.DT_INVENTARIO,
                ControlCalidad = stockEntity.ID_CTRL_CALIDAD,
                FechaModificacion = stockEntity.DT_UPDROW,
                NumeroTransaccion = stockEntity.NU_TRANSACCION,
                NumeroTransaccionDelete = stockEntity.NU_TRANSACCION_DELETE,
                MotivoAveria = stockEntity.CD_MOTIVO_AVERIA
            };
        }

        public virtual T_STOCK MapFromStock(Stock stock)
        {
            return new T_STOCK
            {
                CD_ENDERECO = stock.Ubicacion,
                CD_EMPRESA = stock.Empresa,
                CD_PRODUTO = stock.Producto,
                CD_FAIXA = stock.Faixa,
                NU_IDENTIFICADOR = stock.Identificador?.Trim()?.ToUpper(),
                QT_ESTOQUE = stock.Cantidad,
                QT_RESERVA_SAIDA = stock.ReservaSalida,
                QT_TRANSITO_ENTRADA = stock.CantidadTransitoEntrada,
                DT_FABRICACAO = stock.Vencimiento,
                ID_AVERIA = stock.Averia,
                ID_INVENTARIO = stock.Inventario,
                DT_INVENTARIO = stock.FechaInventario,
                ID_CTRL_CALIDAD = stock.ControlCalidad,
                DT_UPDROW = stock.FechaModificacion,
                NU_TRANSACCION = stock.NumeroTransaccion,
                NU_TRANSACCION_DELETE = stock.NumeroTransaccionDelete,
                CD_MOTIVO_AVERIA = stock.MotivoAveria
            };
        }


        public virtual Envase Map(T_STOCK_ENVASE entity)
        {
            if (entity == null) return null;

            return new Envase
            {
                Id = entity.ID_ENVASE,
                CodigoAgente = entity.CD_AGENTE,
                TipoEnvase = entity.ND_TP_ENVASE,
                CodigoBarras = entity.CD_BARRAS,
                Estado = entity.ND_ESTADO_ENVASE,
                DescripcionUltimoMovimiento = entity.DS_ULTIMO_MOVIMIENTO,
                Observaciones = entity.DS_OBSERVACIONES,
                NumeroInterfaz = entity.NU_INTERFAZ_EJECUCION,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UDPROW,
                Empresa = entity.CD_EMPRESA,
                TipoAgente = entity.TP_AGENTE,
                FechaUltimaRecepcion = entity.DT_ULT_RECEPCION,
                FechaUltimaCargaEnCamion = entity.DT_ULT_CARGA_CAMION,
                FechaUltimaExpedicion = entity.DT_ULT_EXPEDICION,
                UsuarioUltimaRecepcion = entity.CD_USUARIO_ULT_RECEPCION,
                UsuarioUltimaCargaEnCamion = entity.CD_USUARIO_ULT_CARGA_CAMION,
                UsuarioUltimaExpedicion = entity.CD_USUARIO_ULT_EXPEDICION,
                NumeroTransaccion = entity.NU_TRANSACCION
            };
        }

        public virtual T_STOCK_ENVASE Map(Envase obj)
        {
            if (obj == null) return null;

            return new T_STOCK_ENVASE
            {
                ID_ENVASE = obj.Id,
                CD_AGENTE = obj.CodigoAgente,
                ND_TP_ENVASE = obj.TipoEnvase,
                CD_BARRAS = obj.CodigoBarras,
                ND_ESTADO_ENVASE = obj.Estado,
                DS_ULTIMO_MOVIMIENTO = obj.DescripcionUltimoMovimiento,
                DS_OBSERVACIONES = obj.Observaciones,
                NU_INTERFAZ_EJECUCION = obj.NumeroInterfaz,
                DT_ADDROW = obj.FechaAlta,
                DT_UDPROW = obj.FechaModificacion,
                CD_EMPRESA = obj.Empresa,
                TP_AGENTE = obj.TipoAgente,
                DT_ULT_RECEPCION = obj.FechaUltimaRecepcion,
                DT_ULT_CARGA_CAMION = obj.FechaUltimaCargaEnCamion,
                DT_ULT_EXPEDICION = obj.FechaUltimaExpedicion,
                CD_USUARIO_ULT_RECEPCION = obj.UsuarioUltimaRecepcion,
                CD_USUARIO_ULT_CARGA_CAMION = obj.UsuarioUltimaCargaEnCamion,
                CD_USUARIO_ULT_EXPEDICION = obj.UsuarioUltimaExpedicion,
                NU_TRANSACCION = obj.NumeroTransaccion
            };
        }

        public virtual TipoMovimiento Map(T_TIPO_MOVIMIENTO entity)
        {
            if (entity == null) return null;

            return new TipoMovimiento
            {
                Id = entity.CD_MOVIMIENTO,
                Descripcion = entity.DS_MOVIMIENTO,
                Categoria = entity.CD_CATEGORIA
            };
        }

        public virtual T_TIPO_MOVIMIENTO Map(TipoMovimiento obj)
        {
            if (obj == null) return null;

            return new T_TIPO_MOVIMIENTO
            {
                CD_MOVIMIENTO = obj.Id,
                DS_MOVIMIENTO = obj.Descripcion,
                CD_CATEGORIA = obj.Categoria
            };
        }
    }
}
