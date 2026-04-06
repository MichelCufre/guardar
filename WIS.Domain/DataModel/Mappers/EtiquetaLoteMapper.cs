using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class EtiquetaLoteMapper : Mapper
    {
        public virtual EtiquetaLote MapToObject(T_ETIQUETA_LOTE entity)
        {
            if (entity == null)
                return null;

            return new EtiquetaLote
            {
                Numero = entity.NU_ETIQUETA_LOTE,
                Cliente = entity.CD_CLIENTE,
                IdUbicacion = entity.CD_ENDERECO,
                UbicacionMovimiento = entity.CD_ENDERECO_MOVTO_PARCIAL,
                IdUbicacionSugerida = entity.CD_ENDERECO_SUGERIDO,
                FuncionarioAlmacenamiento = entity.CD_FUNC_ALMACENAMIENTO,
                FuncionarioRecepcion = entity.CD_FUNC_RECEPCION,
                CodigoGrupo = entity.CD_GRUPO,
                CodigoPallet = entity.CD_PALLET,
                Estado = entity.CD_SITUACAO,
                EstadoPallet = entity.CD_SITUACAO_PALLET,
                FechaAlmacenamiento = entity.DT_ALMACENAMIENTO,
                FechaRecepcion = entity.DT_RECEPCION,
                FechaModificacion = entity.DT_UPDROW,
                NumeroAgenda = entity.NU_AGENDA,
                NumeroExterno = entity.NU_EXTERNO_ETIQUETA,
                TipoEtiqueta = entity.TP_ETIQUETA,
                CodigoUnidadTransporte = entity.NU_UNIDAD_TRANSPORTE,
                CodigoBarras = entity.CD_BARRAS,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                NroLpn = entity.NU_LPN,
            };
        }
        public virtual T_ETIQUETA_LOTE MapToEntity(EtiquetaLote obj)
        {
            return new T_ETIQUETA_LOTE
            {
                NU_ETIQUETA_LOTE = obj.Numero,
                CD_CLIENTE = obj.Cliente,
                CD_ENDERECO = NullIfEmpty(obj.IdUbicacion),
                CD_ENDERECO_MOVTO_PARCIAL = obj.UbicacionMovimiento,
                CD_ENDERECO_SUGERIDO = NullIfEmpty(obj.IdUbicacionSugerida),
                CD_FUNC_ALMACENAMIENTO = obj.FuncionarioAlmacenamiento,
                CD_FUNC_RECEPCION = obj.FuncionarioRecepcion,
                CD_GRUPO = obj.CodigoGrupo,
                CD_PALLET = obj.CodigoPallet,
                CD_SITUACAO = obj.Estado,
                CD_SITUACAO_PALLET = obj.EstadoPallet,
                DT_ALMACENAMIENTO = obj.FechaAlmacenamiento,
                DT_RECEPCION = obj.FechaRecepcion,
                DT_UPDROW = obj.FechaModificacion,
                NU_AGENDA = obj.NumeroAgenda,
                NU_EXTERNO_ETIQUETA = obj.NumeroExterno,
                NU_UNIDAD_TRANSPORTE = obj.CodigoUnidadTransporte,
                TP_ETIQUETA = obj.TipoEtiqueta,
                CD_BARRAS = obj.CodigoBarras,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
                NU_LPN = obj.NroLpn
            };
        }

        public virtual EtiquetaLoteDetalle MapDetalleConCabezalToObject(T_DET_ETIQUETA_LOTE entity)
        {
            if (entity == null)
                return null;

            var detalle = new EtiquetaLoteDetalle
            {
                IdEtiquetaLote = entity.NU_ETIQUETA_LOTE,
                CodigoProducto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                IdEmpresa = entity.CD_EMPRESA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                CantidadRecibida = entity.QT_PRODUTO_RECIBIDO,
                Cantidad = entity.QT_PRODUTO,
                CantidadAjusteRecibido = entity.QT_AJUSTE_RECIBIDO,
                CantidadEtiquetaGenerada = entity.QT_ETIQUETA_GENERADA,
                CantidadAlmacenada = entity.QT_ALMACENADO,
                Vencimiento = entity.DT_FABRICACAO,
                Insercion = entity.DT_ADDROW,
                Modificacion = entity.DT_UPDROW,
                CantidadRastreoPallet = entity.QT_RASTREO_PALLET,
                CantidadMovilizado = entity.QT_MOVILIZADO,
                Entrada = entity.DT_ENTRADA,
                PesoRecibido = entity.PS_PRODUTO_RECIBIDO,
                Peso = entity.PS_PRODUTO,
                DescripcionMotivo = entity.DS_MOTIVO,
                NumeroTransaccion = entity.NU_TRANSACCION,
            };

            if (entity.T_ETIQUETA_LOTE != null)
            {
                detalle.EtiquetaLote = this.MapToObject(entity.T_ETIQUETA_LOTE);
            }
            return detalle;
        }

        public virtual EtiquetaLoteDetalle MapToObject(T_DET_ETIQUETA_LOTE entity)
        {
            if (entity == null)
                return null;

            var detalle = new EtiquetaLoteDetalle
            {
                IdEtiquetaLote = entity.NU_ETIQUETA_LOTE,
                CodigoProducto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                IdEmpresa = entity.CD_EMPRESA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                CantidadRecibida = entity.QT_PRODUTO_RECIBIDO,
                Cantidad = entity.QT_PRODUTO,
                CantidadAjusteRecibido = entity.QT_AJUSTE_RECIBIDO,
                CantidadEtiquetaGenerada = entity.QT_ETIQUETA_GENERADA,
                CantidadAlmacenada = entity.QT_ALMACENADO,
                Vencimiento = entity.DT_FABRICACAO,
                Insercion = entity.DT_ADDROW,
                Modificacion = entity.DT_UPDROW,
                CantidadRastreoPallet = entity.QT_RASTREO_PALLET,
                CantidadMovilizado = entity.QT_MOVILIZADO,
                Entrada = entity.DT_ENTRADA,
                PesoRecibido = entity.PS_PRODUTO_RECIBIDO,
                Peso = entity.PS_PRODUTO,
                DescripcionMotivo = entity.DS_MOTIVO,
                NumeroTransaccion = entity.NU_TRANSACCION,
            };

            return detalle;
        }

        public virtual T_DET_ETIQUETA_LOTE MapToEntity(EtiquetaLoteDetalle objeto)
        {
            return new T_DET_ETIQUETA_LOTE
            {
                NU_ETIQUETA_LOTE = objeto.IdEtiquetaLote,
                CD_PRODUTO = objeto.CodigoProducto,
                CD_FAIXA = objeto.Faixa,
                CD_EMPRESA = objeto.IdEmpresa,
                NU_IDENTIFICADOR = objeto.Identificador?.Trim()?.ToUpper(),
                QT_PRODUTO_RECIBIDO = objeto.CantidadRecibida,
                QT_PRODUTO = objeto.Cantidad,
                QT_AJUSTE_RECIBIDO = objeto.CantidadAjusteRecibido,
                QT_ETIQUETA_GENERADA = objeto.CantidadEtiquetaGenerada,
                QT_ALMACENADO = objeto.CantidadAlmacenada,
                DT_FABRICACAO = objeto.Vencimiento,
                DT_ADDROW = objeto.Insercion,
                DT_UPDROW = objeto.Modificacion,
                QT_RASTREO_PALLET = objeto.CantidadRastreoPallet,
                QT_MOVILIZADO = objeto.CantidadMovilizado,
                DT_ENTRADA = objeto.Entrada,
                PS_PRODUTO_RECIBIDO = objeto.PesoRecibido,
                PS_PRODUTO = objeto.Peso,
                DS_MOTIVO = objeto.DescripcionMotivo,
                NU_TRANSACCION = objeto.NumeroTransaccion,
            };
        }

        public virtual LogEtiqueta MapToObject(T_LOG_ETIQUETA entity)
        {
            if (entity == null)
                return null;

            return new LogEtiqueta
            {
                Id = entity.NU_LOG_ETIQUETA,
                Agenda = entity.NU_AGENDA,
                NumeroEtiqueta = entity.NU_ETIQUETA,
                CodigoProducto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Empresa = entity.CD_EMPRESA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Cantidad = entity.QT_MOVIMIENTO,
                Ubicacion = entity.CD_ENDERECO,
                FechaOperacion = entity.DT_OPERACION,
                NroTransaccion = entity.NU_TRANSACCION,
                NroInterfazEjecucion = entity.NU_INTERFAZ_EJECUCION,
                Vencimiento = entity.DT_FABRICACAO,
                TipoMovimiento = entity.TP_MOVIMIENTO,
                Aplicacion = entity.CD_APLICACAO,
                Funcionario = entity.CD_FUNCIONARIO,
            };
        }

        public virtual T_LOG_ETIQUETA MapToEntity(LogEtiqueta objeto)
        {
            return new T_LOG_ETIQUETA
            {
                NU_LOG_ETIQUETA = objeto.Id,
                NU_AGENDA = objeto.Agenda,
                NU_ETIQUETA = objeto.NumeroEtiqueta,
                CD_PRODUTO = objeto.CodigoProducto,
                CD_FAIXA = objeto.Faixa,
                CD_EMPRESA = objeto.Empresa,
                NU_IDENTIFICADOR = objeto.Identificador?.Trim()?.ToUpper(),
                QT_MOVIMIENTO = objeto.Cantidad,
                CD_ENDERECO = objeto.Ubicacion,
                DT_OPERACION = objeto.FechaOperacion,
                NU_TRANSACCION = objeto.NroTransaccion,
                NU_INTERFAZ_EJECUCION = objeto.NroInterfazEjecucion,
                DT_FABRICACAO = objeto.Vencimiento,
                TP_MOVIMIENTO = objeto.TipoMovimiento,
                CD_APLICACAO = objeto.Aplicacion,
                CD_FUNCIONARIO = objeto.Funcionario,
            };
        }
    }
}
