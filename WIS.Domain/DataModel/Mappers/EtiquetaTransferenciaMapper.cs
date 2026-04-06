using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class EtiquetaTransferenciaMapper : Mapper
    {
        public EtiquetaTransferenciaMapper()
        {

        }

        public virtual EtiquetaTransferencia MapToObject(T_PALLET_TRANSFERENCIA entity)
        {
            if (entity == null)
                return null;

            return new EtiquetaTransferencia
            {
                NumeroEtiqueta = entity.NU_ETIQUETA,
                NumeroSecEtiqueta = entity.NU_SEC_ETIQUETA,
                UbicacionReal = entity.CD_ENDERECO_REAL,
                UbicacionDestino = entity.CD_ENDERECO_DESTINO,
                Estado = entity.CD_SITUACAO,
                Predio = entity.NU_PREDIO,
                FechaFinalizacion = entity.DT_FINALIZACION,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                AplicacionOrigen = entity.CD_APLICACAO_ORIGEN,
                TpModalidadUso = entity.TP_MODALIDAD_USO,
                NumeroTransaccion = entity.NU_TRANSACCION,
                IdExternoEtiqueta = entity.ID_EXTERNO_ETIQUETA,
                TipoEtiquetaTransferencia = entity.TP_ETIQUETA_TRANSFERENCIA,
                NroLpn = entity.NU_LPN
            };
        }

        public virtual T_PALLET_TRANSFERENCIA MapToEntity(EtiquetaTransferencia obj)
        {
            return new T_PALLET_TRANSFERENCIA
            {
                NU_ETIQUETA = obj.NumeroEtiqueta,
                NU_SEC_ETIQUETA = obj.NumeroSecEtiqueta,
                CD_ENDERECO_REAL = obj.UbicacionReal,
                CD_ENDERECO_DESTINO = obj.UbicacionDestino,
                CD_SITUACAO = obj.Estado,
                NU_PREDIO = obj.Predio,
                DT_FINALIZACION = obj.FechaFinalizacion,
                DT_UPDROW = obj.FechaModificacion,
                DT_ADDROW = obj.FechaInsercion,
                CD_APLICACAO_ORIGEN = obj.AplicacionOrigen,
                TP_MODALIDAD_USO = obj.TpModalidadUso,
                NU_TRANSACCION = obj.NumeroTransaccion,
                ID_EXTERNO_ETIQUETA = obj.IdExternoEtiqueta,
                TP_ETIQUETA_TRANSFERENCIA = obj.TipoEtiquetaTransferencia,
                NU_LPN = obj.NroLpn,
            };
        }


        public virtual T_DET_PALLET_TRANSFERENCIA MapToEntity(DetalleEtiquetaTransferencia entity)
        {
            return new T_DET_PALLET_TRANSFERENCIA
            {
                CD_EMPRESA = entity.Empresa,
                CD_ENDERECO_DESTINO = entity.UbicacionDestino,
                CD_ENDERECO_ORIGEN = entity.UbicacionOrigen,
                CD_FAIXA = entity.Faixa,
                CD_FUNCIONARIO = entity.Funcionario,
                CD_PRODUTO = entity.Producto,
                CD_SITUACAO = entity.Estado,
                DT_ADDROW = entity.FechaRegistro,
                DT_FABRICACAO = entity.Vencimiento,
                DT_ULT_INVENTARIO = entity.FechaUltimoInventario,
                DT_UPDROW = entity.FechaModificacion,
                ID_AREA_AVERIA = entity.AreaAveria,
                ID_AVERIA = entity.Averia,
                ID_CTRL_CALI_PEND = entity.ControlCalidadPendiente,
                ID_INVENTARIO_CICLICO = entity.InventarioCiclico,
                NU_ETIQUETA = entity.NumeroEtiqueta,
                NU_IDENTIFICADOR = entity.Identificador?.Trim()?.ToUpper(),
                NU_SEC_DETALLE = entity.NumeroSecDetalle,
                NU_SEC_ETIQUETA = entity.NumeroSecEtiqueta,
                NU_TRANSACCION = entity.Transaccion,
                QT_PRODUTO = entity.Cantidad,
                VL_METADATA = entity.Metadata,
                NU_LPN = entity.NroLpn,
            };
        }

        public virtual DetalleEtiquetaTransferencia MapToObject(T_DET_PALLET_TRANSFERENCIA entity)
        {
            return new DetalleEtiquetaTransferencia
            {
                Empresa = entity.CD_EMPRESA,
                UbicacionDestino = entity.CD_ENDERECO_DESTINO,
                UbicacionOrigen = entity.CD_ENDERECO_ORIGEN,
                Faixa = entity.CD_FAIXA,
                Funcionario = entity.CD_FUNCIONARIO,
                Producto = entity.CD_PRODUTO,
                Estado = entity.CD_SITUACAO,
                FechaRegistro = entity.DT_ADDROW,
                Vencimiento = entity.DT_FABRICACAO,
                FechaUltimoInventario = entity.DT_ULT_INVENTARIO,
                FechaModificacion = entity.DT_UPDROW,
                AreaAveria = entity.ID_AREA_AVERIA,
                Averia = entity.ID_AVERIA,
                ControlCalidadPendiente = entity.ID_CTRL_CALI_PEND,
                InventarioCiclico = entity.ID_INVENTARIO_CICLICO,
                NumeroEtiqueta = entity.NU_ETIQUETA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                NumeroSecDetalle = entity.NU_SEC_DETALLE,
                NumeroSecEtiqueta = entity.NU_SEC_ETIQUETA,
                Transaccion = entity.NU_TRANSACCION,
                Cantidad = entity.QT_PRODUTO,
                Metadata = entity.VL_METADATA,
                NroLpn = entity.NU_LPN,
            };
        }
    }
}
