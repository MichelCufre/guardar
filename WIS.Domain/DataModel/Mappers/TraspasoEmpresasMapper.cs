using WIS.Domain.StockEntities;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class TraspasoEmpresasMapper : Mapper
    {
        public TraspasoEmpresasMapper()
        {
        }

        public virtual TraspasoEmpresas MapToObject(T_TRASPASO entity)
        {
            if (entity == null)
                return null;

            return new TraspasoEmpresas
            {
                Id = entity.NU_TRASPASO,
                Descripcion = entity.DS_TRASPASO,
                IdExterno = entity.ID_TRASPASO_EXTERNO,
                EmpresaOrigen = entity.CD_EMPRESA,
                EmpresaDestino = entity.CD_EMPRESA_DESTINO,
                TipoTraspaso = entity.TP_TRASPASO,
                Estado = entity.ID_ESTADO,
                DocumentoIngreso = entity.NU_DOCUMENTO_INGRESO,
                TipoDocumentoIngreso = entity.TP_DOCUMENTO_INGRESO,
                DocumentoEgreso = entity.NU_DOCUMENTO_EGRESO,
                TipoDocumentoEgreso = entity.TP_DOCUMENTO_EGRESO,
                Preparacion = entity.NU_PREPARACION,
                FinalizarConPreparacion = MapStringToBoolean(entity.FL_FINALIZAR_CON_PREPARACION),
                PropagarLPN = MapStringToBoolean(entity.FL_PROPAGAR_LPN),
                GeneraCabezalAuto = MapStringToBoolean(entity.FL_GENERACION_AUTO_CABEZAL),
                ReplicaProductos = MapStringToBoolean(entity.FL_REPLICA_PRODUCTOS),
                ReplicaCodBarras = MapStringToBoolean(entity.FL_REPLICA_CODIGOS_BARRAS),
                ControlaCaractIguales = MapStringToBoolean(entity.FL_CTRL_CARACT_IGUALES),
                ReplicaAgentes = MapStringToBoolean(entity.FL_REPLICA_AGENTES),
                ConfigPedidoDestino = entity.DS_CONFIG_PEDIDO_DESTINO,
                PreparacionDestino = entity.NU_PREPARACION_DESTINO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
            };
        }

        public virtual TraspasoEmpresasPreparacionPendiente MapToObject(V_STO820_PREPARACION_PENDIENTE entity)
        {
            if (entity == null)
                return null;

            return new TraspasoEmpresasPreparacionPendiente
            {
                Preparacion = entity.NU_PREPARACION,
                Descripcion = entity.DS_PREPARACION,
                Empresa = entity.CD_EMPRESA,
                CantidadLoteAuto = entity.QT_LOTE_AUTO,
                CantidadDetallePickingPorAtributo = entity.QT_LPN_ATR,
                CantidadDetallleLpn = entity.QT_LPN_DET,
                CantidadDetallePickingPorLpn = entity.QT_LPN_DET_PICKING,
                CantidadPendiente = entity.QT_PENDIENTE,
                CantidadPickingMayorSuelto = entity.QT_PICKING_MAYOR_SUELTO,
                CantidadPreparada = entity.QT_PREPARADO,
                CantidadSaldoSinTrabajar = entity.QT_SALDO_SIN_TRABAJAR,
                CantidadPedidoNoTraspaso = entity.QT_TIPO_EXP_NO_TRASPASO,
                CantidadPickingReabastecimientoUbicacion = entity.QT_REAB_UBIC_BAJAS,
            };
        }

        public virtual TraspasoEmpresasPreparacionPreparado MapToObject(V_STO820_PREPARACION_PREPARADO entity)
        {
            if (entity == null)
                return null;

            return new TraspasoEmpresasPreparacionPreparado
            {
                Preparacion = entity.NU_PREPARACION,
                Descripcion = entity.DS_PREPARACION,
                Empresa = entity.CD_EMPRESA,
                CantidadPendiente = entity.QT_PENDIENTE,
                CantidadPreparada = entity.QT_PREPARADO,
                CantidadSaldoSinTrabajar = entity.QT_SALDO_SIN_TRABAJAR,
                CantidadPedidoNoTraspaso = entity.QT_TIPO_EXP_NO_TRASPASO
            };
        }

        public virtual TraspasoEmpresasConfiguracion MapConfigToObject(T_TRASPASO_CONFIGURACION entity)
        {
            if (entity == null)
                return null;

            return new TraspasoEmpresasConfiguracion
            {
                Id = entity.NU_TRASPASO_CONFIGURACION,
                EmpresaOrigen = entity.CD_EMPRESA,
                TodaEmpresa = MapStringToBoolean(entity.FL_TODA_EMPRESA),
                TodoTipoTraspaso = MapStringToBoolean(entity.FL_TODO_TIPO_TRASPASO),
                GeneraCabezalAuto = MapStringToBoolean(entity.FL_GENERACION_AUTO_CABEZAL),
                ReplicaProductos = MapStringToBoolean(entity.FL_REPLICA_PRODUCTOS),
                ReplicaCodBarras = MapStringToBoolean(entity.FL_REPLICA_CODIGOS_BARRAS),
                ControlaCaractIguales = MapStringToBoolean(entity.FL_CTRL_CARACT_IGUALES),
                ReplicaAgentes = MapStringToBoolean(entity.FL_REPLICA_AGENTES),
                TipoDocumentoIngreso = entity.CD_TIPO_DOCUMENTO_INGRESO,
                TipoDocumentoEgreso = entity.CD_TIPO_DOCUMENTO_EGRESO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
            };
        }

        public virtual MapeoProducto MapMapeoToObject(T_TRASPASO_MAPEO_PRODUTO entity)
        {
            if (entity == null)
                return null;

            return new MapeoProducto
            {
                EmpresaOrigen = entity.CD_EMPRESA,
                EmpresaDestino = entity.CD_EMPRESA_DESTINO,
                ProductoOrigen = entity.CD_PRODUTO,
                ProductoDestino = entity.CD_PRODUTO_DESTINO,
                FaixaOrigen = entity.CD_FAIXA,
                FaixaDestino = entity.CD_FAIXA_DESTINO,
                CantidadOrigen = entity.QT_ORIGEN,
                CantidadDestino = entity.QT_DESTINO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
            };
        }

        public virtual TraspasoEmpresasDetallePedido MapDetPedidoToObject(T_TRASPASO_DET_PEDIDO entity)
        {
            if (entity == null)
                return null;

            return new TraspasoEmpresasDetallePedido
            {
                Id = entity.NU_TRASPASO_DET_PEDIDO,
                Traspaso = entity.NU_TRASPASO,
                PedidoOrigen = entity.NU_PEDIDO,
                ClienteOrigen = entity.CD_CLIENTE,
                EmpresaOrigen = entity.CD_EMPRESA,
                PedidoDestino = entity.NU_PEDIDO_DESTINO,
                ClienteDestino = entity.CD_CLIENTE_DESTINO,
                EmpresaDestino = entity.CD_EMPRESA_DESTINO,
                TipoPedidoDestino = entity.TP_PEDIDO_DESTINO,
                TipoExpedicionDestino = entity.TP_EXPEDICION_DESTINO,
                FechaAlta = entity.DT_ADDROW,
            };
        }

        public virtual T_TRASPASO MapToEntity(TraspasoEmpresas obj)
        {
            return new T_TRASPASO
            {
                NU_TRASPASO = obj.Id,
                DS_TRASPASO = obj.Descripcion,
                ID_TRASPASO_EXTERNO = obj.IdExterno,
                CD_EMPRESA = obj.EmpresaOrigen,
                CD_EMPRESA_DESTINO = obj.EmpresaDestino,
                TP_TRASPASO = obj.TipoTraspaso,
                ID_ESTADO = obj.Estado,
                NU_DOCUMENTO_INGRESO = obj.DocumentoIngreso,
                TP_DOCUMENTO_INGRESO = obj.TipoDocumentoIngreso,
                NU_DOCUMENTO_EGRESO = obj.DocumentoEgreso,
                TP_DOCUMENTO_EGRESO = obj.TipoDocumentoEgreso,
                NU_PREPARACION = obj.Preparacion,
                FL_FINALIZAR_CON_PREPARACION = MapBooleanToString(obj.FinalizarConPreparacion),
                FL_PROPAGAR_LPN = MapBooleanToString(obj.PropagarLPN),
                FL_GENERACION_AUTO_CABEZAL = MapBooleanToString(obj.GeneraCabezalAuto),
                FL_REPLICA_PRODUCTOS = MapBooleanToString(obj.ReplicaProductos),
                FL_REPLICA_CODIGOS_BARRAS = MapBooleanToString(obj.ReplicaCodBarras),
                FL_CTRL_CARACT_IGUALES = MapBooleanToString(obj.ControlaCaractIguales),
                FL_REPLICA_AGENTES = MapBooleanToString(obj.ReplicaAgentes),
                DS_CONFIG_PEDIDO_DESTINO = obj.ConfigPedidoDestino,
                NU_PREPARACION_DESTINO = obj.PreparacionDestino,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
            };
        }

        public virtual T_TRASPASO_CONFIGURACION MapConfigToEntity(TraspasoEmpresasConfiguracion obj)
        {
            return new T_TRASPASO_CONFIGURACION
            {
                NU_TRASPASO_CONFIGURACION = obj.Id,
                CD_EMPRESA = obj.EmpresaOrigen,
                FL_TODA_EMPRESA = MapBooleanToString(obj.TodaEmpresa),
                FL_TODO_TIPO_TRASPASO = MapBooleanToString(obj.TodoTipoTraspaso),
                FL_GENERACION_AUTO_CABEZAL = MapBooleanToString(obj.GeneraCabezalAuto),
                FL_REPLICA_PRODUCTOS = MapBooleanToString(obj.ReplicaProductos),
                FL_REPLICA_CODIGOS_BARRAS = MapBooleanToString(obj.ReplicaCodBarras),
                FL_CTRL_CARACT_IGUALES = MapBooleanToString(obj.ControlaCaractIguales),
                FL_REPLICA_AGENTES = MapBooleanToString(obj.ReplicaAgentes),
                CD_TIPO_DOCUMENTO_INGRESO = NullIfEmpty(obj.TipoDocumentoIngreso),
                CD_TIPO_DOCUMENTO_EGRESO = NullIfEmpty(obj.TipoDocumentoEgreso),
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
            };
        }

        public virtual T_TRASPASO_MAPEO_PRODUTO MapMapeoToEntity(MapeoProducto obj)
        {
            return new T_TRASPASO_MAPEO_PRODUTO
            {
                CD_EMPRESA = obj.EmpresaOrigen,
                CD_EMPRESA_DESTINO = obj.EmpresaDestino,
                CD_PRODUTO = obj.ProductoOrigen,
                CD_PRODUTO_DESTINO = obj.ProductoDestino,
                CD_FAIXA = obj.FaixaOrigen,
                CD_FAIXA_DESTINO = obj.FaixaDestino,
                QT_ORIGEN = obj.CantidadOrigen,
                QT_DESTINO = obj.CantidadDestino,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
            };
        }

        public virtual T_TRASPASO_DET_PEDIDO MapDetPedidoToEntity(TraspasoEmpresasDetallePedido obj)
        {
            return new T_TRASPASO_DET_PEDIDO
            {
                NU_TRASPASO_DET_PEDIDO = obj.Id,
                NU_TRASPASO = obj.Traspaso,
                NU_PEDIDO = obj.PedidoOrigen,
                CD_CLIENTE = obj.ClienteOrigen,
                CD_EMPRESA = obj.EmpresaOrigen,
                NU_PEDIDO_DESTINO = NullIfEmpty(obj.PedidoDestino),
                CD_CLIENTE_DESTINO = NullIfEmpty(obj.ClienteDestino),
                CD_EMPRESA_DESTINO = obj.EmpresaDestino,
                TP_PEDIDO_DESTINO = obj.TipoPedidoDestino,
                TP_EXPEDICION_DESTINO = obj.TipoExpedicionDestino,
                DT_ADDROW = obj.FechaAlta,
            };
        }
    }
}
