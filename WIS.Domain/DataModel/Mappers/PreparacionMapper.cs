using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class PreparacionMapper : Mapper
    {
        public PreparacionMapper()
        {

        }

        public virtual Preparacion MapToObjectWhitoutDetail(T_PICKING entity)
        {
            if (entity == null)
                return null;

            return new Preparacion
            {
                Id = entity.NU_PREPARACION,
                Descripcion = entity.DS_PREPARACION,
                FechaInicio = entity.DT_INICIO,
                FechaFin = entity.DT_FIN,
                Usuario = entity.CD_FUNCIONARIO,
                Empresa = entity.CD_EMPRESA,
                IdAviso = entity.ID_AVISO,
                Tipo = entity.TP_PREPARACION,
                Situacion = entity.CD_SITUACAO,
                CodigoContenedorValidado = entity.CD_CONTENEDOR_VALIDACION,
                PrepararSoloConCamion = this.MapStringToBoolean(entity.FL_PREPARAR_SOLO_CON_CAMION),
                PickingEsAgrupadoPorCamion = this.MapStringToBoolean(entity.FL_PICK_AGRUPADO_POR_CAMION),
                RespetarFifoEnLoteAUTO = this.MapStringToBoolean(entity.FL_RESPETAR_FIFO_EN_LOTE_AUTO),
                AceptaMercaderiaAveriada = this.MapStringToBoolean(entity.FL_MERCADERIA_AVERIADA),
                DebeLiberarPorUnidades = this.MapStringToBoolean(entity.FL_LIBERAR_POR_UNIDADES),
                DebeLiberarPorCurvas = this.MapStringToBoolean(entity.FL_LIBERAR_POR_CURVAS),
                Predio = entity.NU_PREDIO,
                ControlaStockDocumental = this.MapStringToBoolean(entity.FL_CONTROLA_STOCK_DOCUMENTO),
                ModalPalletCompleto = entity.FL_MODAL_PALLET_COMPLETO,
                ModalPalletIncompleto = entity.FL_MODAL_PALLET_INCOMPLETO,
                RepartirEscasez = entity.VL_REPARTIR_ESCASEZ,
                PorcentajeRepartoComun = entity.VL_PORCENTAJE_REPARTO_COMUN,
                Onda = entity.CD_ONDA,
                CantidadRechazo = entity.QT_RECHAZOS,
                CodigoDestino = entity.CD_DESTINO,
                TipoDocumento = entity.TP_DOCUMENTO,
                NumeroDocumemto = entity.NU_DOCUMENTO,
                UsuarioAsignado = entity.CD_FUNCIONARIO_ASIGNADO,
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE,
                Agrupacion = entity.ID_AGRUPACION,
                CursorPedido = entity.VL_CURSOR_PEDIDO,
                CursorStock = entity.VL_CURSOR_STOCK,
                PriozarDesborde = this.MapStringToBoolean(entity.FL_PRIORIZAR_DESBORDE),
                ManejaVidaUtil = this.MapStringToBoolean(entity.FL_VENTANA_POR_CLIENTE),
                ValorVidaUtil = entity.VL_PORCENTAJE_VENTANA,
                RequiereUbicacion = this.MapStringToBoolean(entity.FL_REQUIERE_UBICACION),
                Simulacro = this.MapStringToBoolean(entity.FL_SIMULACRO),
                PriorizarLotePick = this.MapStringToBoolean(entity.FL_PRIORIZAR_LOTE_PICK),
                UsarSoloStkPicking = this.MapStringToBoolean(entity.FL_USAR_SOLO_STK_PICKING),
                ExcluirUbicacionesPicking = this.MapStringToBoolean(entity.FL_EXCLUIR_UBICACIONES_PICKING),
                PermitePickVencido = this.MapStringToBoolean(entity.FL_PICK_VENCIDO),
                ValidarProductoProveedor = this.MapStringToBoolean(entity.FL_VALIDAR_PRODUCTO_PROVEEDOR),
                FlPermitePickVencido = entity.FL_PICK_VENCIDO,
                FlAceptaMercaderiaAveriada = entity.FL_MERCADERIA_AVERIADA,
            };
        }

        public virtual Preparacion MapToObject(T_PICKING entity)
        {
            if (entity == null)
                return null;

            var preparacion = MapToObjectWhitoutDetail(entity);

            foreach (var linea in entity.T_DET_PICKING)
            {
                preparacion.Lineas.Add(MapToObject(linea));
            }

            return preparacion;
        }

        public virtual DetallePreparacion MapToObject(T_DET_PICKING entity)
        {
            if (entity == null)
                return null;

            return new DetallePreparacion
            {
                NumeroPreparacion = entity.NU_PREPARACION,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Lote = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Empresa = entity.CD_EMPRESA,
                Ubicacion = entity.CD_ENDERECO,
                Pedido = entity.NU_PEDIDO,
                Cliente = entity.CD_CLIENTE,
                NumeroSecuencia = entity.NU_SEQ_PREPARACION,
                EspecificaLote = entity.ID_ESPECIFICA_IDENTIFICADOR,
                Carga = entity.NU_CARGA,
                Agrupacion = entity.ID_AGRUPACION,
                Cantidad = entity.QT_PRODUTO ?? 0,
                CantidadPreparada = entity.QT_PREPARADO,
                Usuario = entity.CD_FUNCIONARIO,
                NroContenedor = entity.NU_CONTENEDOR,
                NumeroContenedorSys = entity.NU_CONTENEDOR_SYS,
                CantidadPickeo = entity.QT_PICKEO,
                FechaPickeo = entity.DT_PICKEO,
                NumeroContenedorPickeo = entity.NU_CONTENEDOR_PICKEO,
                UsuarioPickeo = entity.CD_FUNC_PICKEO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                VencimientoPickeo = entity.DT_FABRICACAO_PICKEO,
                AveriaPickeo = entity.ID_AVERIA_PICKEO,
                Proveedor = entity.CD_FORNECEDOR,
                AreaAveria = entity.ID_AREA_AVERIA,
                Cancelado = this.MapStringToBoolean(entity.FL_CANCELADO),
                CantidadControlada = entity.QT_CONTROLADO,
                CantidadControl = entity.QT_CONTROL,
                ErrorControl = entity.FL_ERROR_CONTROL,
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE,
                Estado = entity.ND_ESTADO,
                ReferenciaEstado = entity.VL_ESTADO_REFERENCIA,
                FechaSeparacion = entity.DT_SEPARACION,
                IdDetallePickingLpn = entity.ID_DET_PICKING_LPN
            };
        }

        public virtual T_PICKING MapToEntityWithoutDetail(Preparacion obj)
        {
            if (obj == null) return null;

            return new T_PICKING
            {
                NU_PREPARACION = obj.Id,
                DS_PREPARACION = obj.Descripcion,
                DT_INICIO = obj.FechaInicio,
                DT_FIN = obj.FechaFin,
                CD_FUNCIONARIO = obj.Usuario,
                CD_EMPRESA = obj.Empresa,
                ID_AVISO = obj.IdAviso,
                TP_PREPARACION = obj.Tipo,
                CD_SITUACAO = obj.Situacion,
                CD_CONTENEDOR_VALIDACION = obj.CodigoContenedorValidado,
                FL_PREPARAR_SOLO_CON_CAMION = this.MapBooleanToString(obj.PrepararSoloConCamion),
                FL_PICK_AGRUPADO_POR_CAMION = this.MapBooleanToString(obj.PickingEsAgrupadoPorCamion),
                FL_RESPETAR_FIFO_EN_LOTE_AUTO = this.MapBooleanToString(obj.RespetarFifoEnLoteAUTO),
                FL_MERCADERIA_AVERIADA = this.MapBooleanToString(obj.AceptaMercaderiaAveriada),
                FL_LIBERAR_POR_UNIDADES = this.MapBooleanToString(obj.DebeLiberarPorUnidades),
                FL_LIBERAR_POR_CURVAS = this.MapBooleanToString(obj.DebeLiberarPorCurvas),
                NU_PREDIO = obj.Predio,
                FL_CONTROLA_STOCK_DOCUMENTO = this.MapBooleanToString(obj.ControlaStockDocumental),
                FL_MODAL_PALLET_COMPLETO = obj.ModalPalletCompleto,
                FL_MODAL_PALLET_INCOMPLETO = obj.ModalPalletIncompleto,
                VL_REPARTIR_ESCASEZ = obj.RepartirEscasez,
                VL_PORCENTAJE_REPARTO_COMUN = obj.PorcentajeRepartoComun,
                CD_ONDA = obj.Onda,
                QT_RECHAZOS = obj.CantidadRechazo,
                CD_DESTINO = obj.CodigoDestino,
                TP_DOCUMENTO = obj.TipoDocumento,
                NU_DOCUMENTO = obj.NumeroDocumemto,
                CD_FUNCIONARIO_ASIGNADO = obj.UsuarioAsignado,
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
                ID_AGRUPACION = obj.Agrupacion,
                VL_CURSOR_STOCK = obj.CursorStock,
                VL_CURSOR_PEDIDO = obj.CursorPedido,
                VL_PORCENTAJE_VENTANA = obj.ValorVidaUtil,
                FL_VENTANA_POR_CLIENTE = this.MapBooleanToString(obj.ManejaVidaUtil),
                FL_PRIORIZAR_DESBORDE = this.MapBooleanToString(obj.PriozarDesborde),
                FL_REQUIERE_UBICACION = this.MapBooleanToString(obj.RequiereUbicacion),
                FL_SIMULACRO = this.MapBooleanToString(obj.Simulacro),
                FL_PRIORIZAR_LOTE_PICK = this.MapBooleanToString(obj.PriorizarLotePick),
                FL_USAR_SOLO_STK_PICKING = this.MapBooleanToString(obj.UsarSoloStkPicking),
                FL_EXCLUIR_UBICACIONES_PICKING = this.MapBooleanToString(obj.ExcluirUbicacionesPicking),
                FL_PICK_VENCIDO = this.MapBooleanToString(obj.PermitePickVencido),
                FL_VALIDAR_PRODUCTO_PROVEEDOR = this.MapBooleanToString(obj.ValidarProductoProveedor)
            };
        }

        public virtual T_PICKING MapToEntity(Preparacion obj)
        {
            var picking = MapToEntityWithoutDetail(obj);

            foreach (var linea in obj.Lineas)
            {
                linea.Transaccion = obj.Transaccion;
                picking.T_DET_PICKING.Add(MapToEntity(linea));
            }

            return picking;
        }

        public virtual T_DET_PICKING MapToEntity(DetallePreparacion obj)
        {
            if (obj == null) return null;

            return new T_DET_PICKING
            {
                NU_PREPARACION = obj.NumeroPreparacion,
                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                NU_IDENTIFICADOR = obj.Lote?.Trim()?.ToUpper(),
                CD_EMPRESA = obj.Empresa,
                CD_ENDERECO = obj.Ubicacion,
                NU_PEDIDO = obj.Pedido,
                CD_CLIENTE = obj.Cliente,
                NU_SEQ_PREPARACION = obj.NumeroSecuencia,
                ID_ESPECIFICA_IDENTIFICADOR = obj.EspecificaLote,
                NU_CARGA = obj.Carga,
                ID_AGRUPACION = obj.Agrupacion,
                QT_PRODUTO = obj.Cantidad,
                QT_PREPARADO = obj.CantidadPreparada,
                NU_CONTENEDOR = obj.Contenedor?.Numero,
                CD_FUNCIONARIO = obj.Usuario,
                NU_CONTENEDOR_SYS = obj.NumeroContenedorSys,
                QT_PICKEO = obj.CantidadPickeo,
                DT_PICKEO = obj.FechaPickeo,
                NU_CONTENEDOR_PICKEO = obj.NumeroContenedorPickeo,
                CD_FUNC_PICKEO = obj.UsuarioPickeo,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                DT_FABRICACAO_PICKEO = obj.VencimientoPickeo,
                ID_AVERIA_PICKEO = obj.AveriaPickeo,
                CD_FORNECEDOR = obj.Proveedor,
                ID_AREA_AVERIA = obj.AreaAveria,
                FL_CANCELADO = this.MapBooleanToString(obj.Cancelado),
                QT_CONTROLADO = obj.CantidadControlada,
                QT_CONTROL = obj.CantidadControl,
                FL_ERROR_CONTROL = obj.ErrorControl,
                NU_TRANSACCION = obj.Transaccion,
                NU_TRANSACCION_DELETE = obj.TransaccionDelete,
                ND_ESTADO = obj.Estado,
                VL_ESTADO_REFERENCIA = obj.ReferenciaEstado,
                DT_SEPARACION = obj.FechaSeparacion,
                ID_DET_PICKING_LPN = obj.IdDetallePickingLpn
            };
        }

        public virtual List<T_PICKING> MapToEntity(List<Preparacion> preparaciones)
        {
            return preparaciones
                .Select(MapToEntityWithoutDetail)
                .ToList();
        }

        public virtual T_PREP_NO_ANULAR MapToEntity(PrepNoAnular obj)
        {
            return new T_PREP_NO_ANULAR
            {
                NU_PREPARACION = obj.nuPreparacion,
                NU_PEDIDO = obj.nuPedido,
                CD_EMPRESA = obj.cdEmpresa,
                CD_CLIENTE = obj.cdCliente,
                DT_FIN = obj.dtFin
            };
        }

        public virtual AnulacionPendienteCamion MapToObject(V_ANULACIONES_PENDIENT_CAMION entity)
        {
            return new AnulacionPendienteCamion
            {
                Camion = entity.CD_CAMION,
                Carga = entity.NU_CARGA,
                Empresa = entity.CD_EMPRESA,
                Cliente = entity.CD_CLIENTE,
                Contenedor = entity.NU_CONTENEDOR,
                Pedido = entity.NU_PEDIDO,
                Preparacion = entity.NU_PREPARACION,
                Producto = entity.CD_PRODUTO
            };
        }

        public virtual Preparacion MapToObject(V_PREP_DISP_ASOCIAR entity)
        {
            if (entity == null)
                return null;

            return new Preparacion
            {
                Id = entity.NU_PREPARACION,
                Descripcion = entity.DS_PREPARACION,
                Empresa = entity.CD_EMPRESA,
            };
        }
    }
}
