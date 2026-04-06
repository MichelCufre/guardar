using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class PedidoMapper : Mapper
    {
        public PedidoMapper()
        {

        }

        //Pedido
        public virtual Pedido Map(T_PEDIDO_SAIDA entity)
        {
            Pedido pedido = new Pedido();

            pedido.Cliente = entity.CD_CLIENTE;
            pedido.CondicionLiberacion = entity.CD_CONDICION_LIBERACION;
            pedido.Empresa = entity.CD_EMPRESA;
            pedido.FuncionarioResponsable = entity.CD_FUN_RESP;
            pedido.Origen = entity.CD_ORIGEN;
            pedido.PuntoEntrega = entity.CD_PUNTO_ENTREGA;
            pedido.Ruta = entity.CD_ROTA;
            pedido.Estado = (entity.CD_SITUACAO ?? SituacionDb.PedidoAbierto);
            pedido.CodigoTransportadora = entity.CD_TRANSPORTADORA;
            pedido.CodigoUF = entity.CD_UF;
            pedido.Zona = NullIfEmpty(entity.CD_ZONA);
            pedido.Anexo = entity.DS_ANEXO1;
            pedido.Anexo2 = entity.DS_ANEXO2;
            pedido.Anexo3 = entity.DS_ANEXO3;
            pedido.Anexo4 = entity.DS_ANEXO4;
            pedido.DireccionEntrega = entity.DS_ENDERECO;
            pedido.Memo = entity.DS_MEMO;
            pedido.Memo1 = entity.DS_MEMO_1;
            pedido.FechaAlta = entity.DT_ADDROW;
            pedido.FechaEmision = entity.DT_EMITIDO;
            pedido.FechaEntrega = entity.DT_ENTREGA;
            pedido.FechaFuncionarioResponsable = entity.DT_FUN_RESP;
            pedido.FechaGenerica_1 = entity.DT_GENERICO_1;
            pedido.FechaLiberarDesde = entity.DT_LIBERAR_DESDE;
            pedido.FechaLiberarHasta = entity.DT_LIBERAR_HASTA;
            pedido.FechaUltimaPreparacion = entity.DT_ULT_PREPARACION;
            pedido.FechaModificacion = entity.DT_UPDROW;
            pedido.Agrupacion = entity.ID_AGRUPACION;
            pedido.IsManual = this.MapStringToBoolean(entity.ID_MANUAL);
            pedido.Actividad = entity.ND_ACTIVIDAD;
            pedido.NuGenerico_1 = entity.NU_GENERICO_1;
            pedido.NroIntzFacturacion = entity.NU_INTERFAZ_FACTURACION;
            pedido.OrdenEntrega = entity.NU_ORDEN_ENTREGA;
            pedido.NumeroOrdenLiberacion = entity.NU_ORDEN_LIBERACION;
            pedido.Id = entity.NU_PEDIDO;
            pedido.IngresoProduccion = entity.NU_PRDC_INGRESO;
            pedido.Predio = entity.NU_PREDIO;
            pedido.NroPrepManual = entity.NU_PREPARACION_MANUAL;
            pedido.PreparacionProgramada = entity.NU_PREPARACION_PROGRAMADA;
            pedido.Transaccion = entity.NU_TRANSACCION;
            pedido.TransaccionDelete = entity.NU_TRANSACCION_DELETE;
            pedido.NumeroUltimaPreparacion = entity.NU_ULT_PREPARACION;
            pedido.Tipo = entity.TP_PEDIDO;
            pedido.ComparteContenedorPicking = entity.VL_COMPARTE_CONTENEDOR_PICKING;
            pedido.ComparteContenedorEntrega = entity.VL_COMPARTE_CONTENEDOR_ENTREGA;
            pedido.VlGenerico_1 = entity.VL_GENERICO_1;
            pedido.VlSerealizado_1 = entity.VL_SERIALIZADO_1;
            pedido.IsSincronizacionRealizada = this.MapStringToBoolean(entity.FL_SYNC_REALIZADA);
            pedido.NuCarga = entity.NU_CARGA;
            pedido.Telefono = entity.NU_TELEFONE;
            pedido.TelefonoSecundario = entity.NU_TELEFONE2;
            pedido.Latitud = entity.VL_LATITUD;
            pedido.Longitud = entity.VL_LONGITUD;
            pedido.TipoExpedicionId = entity.TP_EXPEDICION;

            pedido.Lineas = new List<DetallePedido>();
            foreach (var detalle in entity.T_DET_PEDIDO_SAIDA)
            {
                pedido.Lineas.Add(this.MapDetalle(detalle));
            }

            return pedido;
        }

        public virtual T_PEDIDO_SAIDA Map(Pedido pedido)
        {
            return new T_PEDIDO_SAIDA
            {
                CD_CLIENTE = pedido.Cliente,
                CD_CONDICION_LIBERACION = pedido.CondicionLiberacion,
                CD_EMPRESA = pedido.Empresa,
                CD_FUN_RESP = pedido.FuncionarioResponsable,
                CD_ORIGEN = pedido.Origen,
                CD_PUNTO_ENTREGA = pedido.PuntoEntrega,
                CD_ROTA = pedido.Ruta,
                CD_SITUACAO = pedido.Estado,
                CD_TRANSPORTADORA = pedido.CodigoTransportadora,
                CD_UF = pedido.CodigoUF,
                CD_ZONA = NullIfEmpty(pedido.Zona),
                DS_ANEXO1 = pedido.Anexo,
                DS_ANEXO2 = pedido.Anexo2,
                DS_ANEXO3 = pedido.Anexo3,
                DS_ANEXO4 = pedido.Anexo4,
                DS_ENDERECO = pedido.DireccionEntrega,
                DS_MEMO = pedido.Memo,
                DS_MEMO_1 = pedido.Memo1,
                DT_ADDROW = pedido.FechaAlta,
                DT_EMITIDO = pedido.FechaEmision,
                DT_ENTREGA = pedido.FechaEntrega,
                DT_FUN_RESP = pedido.FechaFuncionarioResponsable,
                DT_GENERICO_1 = pedido.FechaGenerica_1,
                DT_LIBERAR_DESDE = pedido.FechaLiberarDesde,
                DT_LIBERAR_HASTA = pedido.FechaLiberarHasta,
                DT_ULT_PREPARACION = pedido.FechaUltimaPreparacion,
                DT_UPDROW = pedido.FechaModificacion,
                ID_AGRUPACION = pedido.Agrupacion,
                ID_MANUAL = this.MapBooleanToString(pedido.IsManual),
                ND_ACTIVIDAD = pedido.Actividad,
                NU_GENERICO_1 = pedido.NuGenerico_1,
                NU_INTERFAZ_FACTURACION = pedido.NroIntzFacturacion,
                NU_ORDEN_ENTREGA = pedido.OrdenEntrega,
                NU_ORDEN_LIBERACION = pedido.NumeroOrdenLiberacion,
                NU_PEDIDO = pedido.Id.Trim(),
                NU_PRDC_INGRESO = pedido.IngresoProduccion,
                NU_PREDIO = pedido.Predio,
                NU_PREPARACION_MANUAL = pedido.NroPrepManual,
                NU_PREPARACION_PROGRAMADA = pedido.PreparacionProgramada,
                NU_TRANSACCION = pedido.Transaccion,
                NU_TRANSACCION_DELETE = pedido.TransaccionDelete,
                NU_ULT_PREPARACION = pedido.NumeroUltimaPreparacion,
                TP_EXPEDICION = pedido.ConfiguracionExpedicion.Tipo,
                TP_PEDIDO = pedido.Tipo,
                VL_COMPARTE_CONTENEDOR_ENTREGA = NullIfEmpty(pedido.ComparteContenedorEntrega),
                VL_COMPARTE_CONTENEDOR_PICKING = NullIfEmpty(pedido.ComparteContenedorPicking),
                VL_GENERICO_1 = pedido.VlGenerico_1,
                VL_SERIALIZADO_1 = pedido.VlSerealizado_1,
                FL_SYNC_REALIZADA = this.MapBooleanToString(pedido.IsSincronizacionRealizada),
                NU_CARGA = pedido.NuCarga,
                NU_TELEFONE = pedido.Telefono,
                NU_TELEFONE2 = pedido.TelefonoSecundario,
                VL_LATITUD = pedido.Latitud,
                VL_LONGITUD = pedido.Longitud,

            };
        }

        public virtual Pedido Map(V_PED_NUEVAS_CARGAS entity)
        {
            Pedido pedido = new Pedido();

            pedido.Cliente = entity.CD_CLIENTE;
            pedido.CondicionLiberacion = entity.CD_CONDICION_LIBERACION;
            pedido.Empresa = entity.CD_EMPRESA;
            pedido.FuncionarioResponsable = entity.CD_FUN_RESP;
            pedido.Origen = entity.CD_ORIGEN;
            pedido.PuntoEntrega = entity.CD_PUNTO_ENTREGA;
            pedido.Ruta = entity.CD_ROTA;
            pedido.Estado = (entity.CD_SITUACAO ?? SituacionDb.PedidoAbierto);
            pedido.CodigoTransportadora = entity.CD_TRANSPORTADORA;
            pedido.CodigoUF = entity.CD_UF;
            pedido.Zona = entity.CD_ZONA;
            pedido.Anexo = entity.DS_ANEXO1;
            pedido.Anexo2 = entity.DS_ANEXO2;
            pedido.Anexo3 = entity.DS_ANEXO3;
            pedido.Anexo4 = entity.DS_ANEXO4;
            pedido.DireccionEntrega = entity.DS_ENDERECO;
            pedido.Memo = entity.DS_MEMO;
            pedido.Memo1 = entity.DS_MEMO_1;
            pedido.FechaAlta = entity.DT_ADDROW;
            pedido.FechaEmision = entity.DT_EMITIDO;
            pedido.FechaEntrega = entity.DT_ENTREGA;
            pedido.FechaFuncionarioResponsable = entity.DT_FUN_RESP;
            pedido.FechaGenerica_1 = entity.DT_GENERICO_1;
            pedido.FechaLiberarDesde = entity.DT_LIBERAR_DESDE;
            pedido.FechaLiberarHasta = entity.DT_LIBERAR_HASTA;
            pedido.FechaUltimaPreparacion = entity.DT_ULT_PREPARACION;
            pedido.FechaModificacion = entity.DT_UPDROW;
            pedido.Agrupacion = entity.ID_AGRUPACION;
            pedido.IsManual = this.MapStringToBoolean(entity.ID_MANUAL);
            pedido.Actividad = entity.ND_ACTIVIDAD;
            pedido.NuGenerico_1 = entity.NU_GENERICO_1;
            pedido.NroIntzFacturacion = entity.NU_INTERFAZ_FACTURACION;
            pedido.OrdenEntrega = entity.NU_ORDEN_ENTREGA;
            pedido.NumeroOrdenLiberacion = entity.NU_ORDEN_LIBERACION;
            pedido.Id = entity.NU_PEDIDO;
            pedido.IngresoProduccion = entity.NU_PRDC_INGRESO;
            pedido.Predio = entity.NU_PREDIO;
            pedido.NroPrepManual = entity.NU_PREPARACION_MANUAL;
            pedido.PreparacionProgramada = entity.NU_PREPARACION_PROGRAMADA;
            pedido.Transaccion = entity.NU_TRANSACCION;
            pedido.NumeroUltimaPreparacion = entity.NU_ULT_PREPARACION;
            //TP_EXPEDICION - Lo hace despues.
            pedido.Tipo = entity.TP_PEDIDO;
            pedido.ComparteContenedorPicking = entity.VL_COMPARTE_CONTENEDOR_PICKING;
            pedido.ComparteContenedorEntrega = entity.VL_COMPARTE_CONTENEDOR_ENTREGA;
            pedido.VlGenerico_1 = entity.VL_GENERICO_1;
            pedido.VlSerealizado_1 = entity.VL_SERIALIZADO_1;
            pedido.IsSincronizacionRealizada = this.MapStringToBoolean(entity.FL_SYNC_REALIZADA);
            pedido.NuCarga = entity.NU_CARGA;

            return pedido;
        }

        //Detalle
        public virtual DetallePedido MapDetalle(T_DET_PEDIDO_SAIDA entity)
        {
            if (entity == null) return null;

            return new DetallePedido
            {
                Id = entity.NU_PEDIDO,
                Cliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                Faixa = entity.CD_FAIXA,
                Producto = entity.CD_PRODUTO,
                Memo = entity.DS_MEMO,
                FechaAlta = entity.DT_ADDROW,
                FechaGenerica_1 = entity.DT_GENERICO_1,
                FechaModificacion = entity.DT_UPDROW,
                Agrupacion = entity.ID_AGRUPACION,
                EspecificaIdentificador = this.MapStringToBoolean(entity.ID_ESPECIFICA_IDENTIFICADOR),
                NuGenerico_1 = entity.NU_GENERICO_1,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Transaccion = entity.NU_TRANSACCION,
                TransaccionDelete = entity.NU_TRANSACCION_DELETE,
                CantidadAbastecida = entity.QT_ABASTECIDO,
                CantidadAnulada = entity.QT_ANULADO,
                CantidadAnuladaFactura = entity.QT_ANULADO_FACTURA,
                CantidadCargada = entity.QT_CARGADO,
                CantidadControlada = entity.QT_CONTROLADO,
                CantidadCrossDocking = entity.QT_CROSS_DOCK,
                CantidadExpedida = entity.QT_EXPEDIDO,
                CantidadFacturada = entity.QT_FACTURADO,
                CantidadLiberada = entity.QT_LIBERADO,
                Cantidad = entity.QT_PEDIDO,
                CantidadOriginal = entity.QT_PEDIDO_ORIGINAL,
                CantidadPreparada = entity.QT_PREPARADO,
                CantidadTransferida = entity.QT_TRANSFERIDO,
                CantUndAsociadoCamion = entity.QT_UND_ASOCIADO_CAMION,
                VlGenerico_1 = entity.VL_GENERICO_1,
                PorcentajeTolerancia = entity.VL_PORCENTAJE_TOLERANCIA,
                DatosSerializados = entity.VL_SERIALIZADO_1,
                SemiAcabado = entity.FL_SEMIACABADO,
                Consumible = entity.FL_CONSUMIBLE,
            };
        }

        public virtual T_DET_PEDIDO_SAIDA MapDetalle(DetallePedido detalle)
        {
            return new T_DET_PEDIDO_SAIDA
            {
                CD_CLIENTE = detalle.Cliente,
                CD_EMPRESA = detalle.Empresa,
                CD_FAIXA = detalle.Faixa,
                CD_PRODUTO = detalle.Producto,
                DS_MEMO = detalle.Memo,
                DT_ADDROW = detalle.FechaAlta,
                DT_GENERICO_1 = detalle.FechaGenerica_1,
                DT_UPDROW = detalle.FechaModificacion,
                ID_AGRUPACION = detalle.Agrupacion,
                ID_ESPECIFICA_IDENTIFICADOR = this.MapBooleanToString(detalle.EspecificaIdentificador),
                NU_GENERICO_1 = detalle.NuGenerico_1,
                NU_IDENTIFICADOR = detalle.Identificador?.Trim()?.ToUpper(),
                NU_PEDIDO = detalle.Id,
                NU_TRANSACCION = detalle.Transaccion,
                NU_TRANSACCION_DELETE = detalle.TransaccionDelete,
                QT_ABASTECIDO = detalle.CantidadAbastecida,
                QT_ANULADO = detalle.CantidadAnulada,
                QT_ANULADO_FACTURA = detalle.CantidadAnuladaFactura,
                QT_CARGADO = detalle.CantidadCargada,
                QT_CONTROLADO = detalle.CantidadControlada,
                QT_CROSS_DOCK = detalle.CantidadCrossDocking,
                QT_EXPEDIDO = detalle.CantidadExpedida,
                QT_FACTURADO = detalle.CantidadFacturada,
                QT_LIBERADO = detalle.CantidadLiberada,
                QT_PEDIDO = detalle.Cantidad,
                QT_PEDIDO_ORIGINAL = detalle.CantidadOriginal,
                QT_PREPARADO = detalle.CantidadPreparada,
                QT_TRANSFERIDO = detalle.CantidadTransferida,
                QT_UND_ASOCIADO_CAMION = detalle.CantUndAsociadoCamion,
                VL_GENERICO_1 = detalle.VlGenerico_1,
                VL_PORCENTAJE_TOLERANCIA = detalle.PorcentajeTolerancia,
                VL_SERIALIZADO_1 = detalle.DatosSerializados,
                FL_SEMIACABADO = detalle.SemiAcabado,
                FL_CONSUMIBLE = detalle.Consumible,
            };
        }

		public virtual T_DET_PEDIDO_SAIDA MapDetalle(Pedido pedido, DetallePedido detalle)
		{
			return new T_DET_PEDIDO_SAIDA
			{
				CD_CLIENTE = pedido.Cliente,
				CD_EMPRESA = pedido.Empresa,
				CD_FAIXA = detalle.Faixa,
				CD_PRODUTO = detalle.Producto,
				DS_MEMO = detalle.Memo,
				DT_ADDROW = detalle.FechaAlta,
				DT_GENERICO_1 = detalle.FechaGenerica_1,
				DT_UPDROW = detalle.FechaModificacion,
				ID_AGRUPACION = detalle.Agrupacion,
				ID_ESPECIFICA_IDENTIFICADOR = MapBooleanToString(detalle.EspecificaIdentificador),
				NU_GENERICO_1 = detalle.NuGenerico_1,
				NU_IDENTIFICADOR = detalle.Identificador?.Trim()?.ToUpper(),
				NU_PEDIDO = pedido.Id,
				NU_TRANSACCION = detalle.Transaccion,
				NU_TRANSACCION_DELETE = detalle.TransaccionDelete,
				QT_ABASTECIDO = detalle.CantidadAbastecida,
				QT_ANULADO = detalle.CantidadAnulada,
				QT_ANULADO_FACTURA = detalle.CantidadAnuladaFactura,
				QT_CARGADO = detalle.CantidadCargada,
				QT_CONTROLADO = detalle.CantidadControlada,
				QT_CROSS_DOCK = detalle.CantidadCrossDocking,
				QT_EXPEDIDO = detalle.CantidadExpedida,
				QT_FACTURADO = detalle.CantidadFacturada,
				QT_LIBERADO = detalle.CantidadLiberada,
				QT_PEDIDO = detalle.Cantidad,
				QT_PEDIDO_ORIGINAL = detalle.CantidadOriginal,
				QT_PREPARADO = detalle.CantidadPreparada,
				QT_TRANSFERIDO = detalle.CantidadTransferida,
				QT_UND_ASOCIADO_CAMION = detalle.CantUndAsociadoCamion,
				VL_GENERICO_1 = detalle.VlGenerico_1,
				VL_PORCENTAJE_TOLERANCIA = detalle.PorcentajeTolerancia,
				VL_SERIALIZADO_1 = detalle.DatosSerializados,
				FL_SEMIACABADO = detalle.SemiAcabado,
				FL_CONSUMIBLE = detalle.Consumible
			};
		}

		//Tipo Expedicion
		public virtual ConfiguracionExpedicionPedido MapConfiguracionExpedicion(T_TIPO_EXPEDICION tipoExpEntity)
        {
            return new ConfiguracionExpedicionPedido
            {
                Tipo = tipoExpEntity.TP_EXPEDICION,
                Descripcion = tipoExpEntity.NM_EXPEDICION,
                RequiereBox = this.MapStringToBoolean(tipoExpEntity.FL_REQUIERE_BOX),
                TipoArmadoEgreso = tipoExpEntity.TP_ARMADO_EGRESO,
                DebeFacturarEnEmpaquetado = this.MapStringToBoolean(tipoExpEntity.FL_FACTURAR_EN_EMPAQUETADO),
                CierreCamionEnBox = this.MapStringToBoolean(tipoExpEntity.FL_CIERRE_CAMION_EN_BOX),
                DebeExpedirContenedor = this.MapStringToBoolean(tipoExpEntity.FL_EXPEDIR_CONTENEDOR),
                DebeEmpaquetarContenedor = this.MapStringToBoolean(tipoExpEntity.FL_EMPAQUETA_CONTENEDOR),
                TraspasoEmpresa = this.MapStringToBoolean(tipoExpEntity.FL_TRASPASO_EMPRESA),
                FlProduccion = this.MapStringToBoolean(tipoExpEntity.FL_PRODUCCION),
                IsConsumoInterno = this.MapStringToBoolean(tipoExpEntity.FL_CONSUMO_INTERNO),
                PermiteFacturacionParcial = this.MapStringToBoolean(tipoExpEntity.FL_PERMITE_FACTURACION_PARCIAL),
                FlCierreCamionEnEmpaque = this.MapStringToBoolean(tipoExpEntity.FL_CIERRE_CAMION_EN_EMPAQUE),
                FlFacturaAutoCompletar = this.MapStringToBoolean(tipoExpEntity.FL_FACTURA_AUTO_COMPLETAR),
                FlPermiteAnulacionParcial = this.MapStringToBoolean(tipoExpEntity.FL_PERMITE_ANULACION_PARCIAL),
                FlModalidadEmpaquetado = this.MapStringToBoolean(tipoExpEntity.FL_MODALIDAD_EMPAQUETADO),
                CodigoGrupoExpedicion = tipoExpEntity.CD_GRUPO_EXPEDICION,
                IsDisponible = this.MapStringToBoolean(tipoExpEntity.ID_DISPONIBLE),
                FlAnulaPendientesAlLiberar = this.MapStringToBoolean(tipoExpEntity.FL_ANULA_PENDIENTES_AL_LIBERAR),
                DebeExpedirCompleto = this.MapStringToBoolean(tipoExpEntity.FL_EXPEDIR_COMPLETO),
                PermiteFacturacionSinPrecinto = this.MapStringToBoolean(tipoExpEntity.FL_PERMITE_FACT_SIN_PRECINTO),
                CantidadPrecintos = tipoExpEntity.VL_CANTIDAD_PRECINTOS,
                IsFacturacionRequerida = this.MapStringToBoolean(tipoExpEntity.FL_FACTURACION_REQUERIDA),
                IsFacturacionParcial = this.MapStringToBoolean(tipoExpEntity.FL_FACTURACION_PARCIAL),
                IsFacturacionPorPedido = this.MapStringToBoolean(tipoExpEntity.FL_FACTURACION_POR_PEDIDO),
                RequiereLiberacionTotal = this.MapStringToBoolean(tipoExpEntity.FL_REQUIERE_LIBERACION_TOTAL),
                FlTracking = this.MapStringToBoolean(tipoExpEntity.FL_TRACKING),
                FlPreparable = this.MapStringToBoolean(tipoExpEntity.FL_PREPARABLE),
                FlEmpaquetaSoloUnProducto = this.MapStringToBoolean(tipoExpEntity.FL_EMPAQUETA_SOLO_UN_PRODUCTO),
            };
        }

        public virtual T_TIPO_EXPEDICION MapConfiguracionExpedicion(ConfiguracionExpedicionPedido tipoExpedicion)
        {
            return new T_TIPO_EXPEDICION
            {
                TP_EXPEDICION = tipoExpedicion.Tipo,
                NM_EXPEDICION = tipoExpedicion.Descripcion,
                FL_REQUIERE_BOX = this.MapBooleanToString(tipoExpedicion.RequiereBox),
                TP_ARMADO_EGRESO = tipoExpedicion.TipoArmadoEgreso,
                FL_FACTURAR_EN_EMPAQUETADO = this.MapBooleanToString(tipoExpedicion.DebeFacturarEnEmpaquetado),
                FL_CIERRE_CAMION_EN_BOX = this.MapBooleanToString(tipoExpedicion.CierreCamionEnBox),
                FL_EXPEDIR_CONTENEDOR = this.MapBooleanToString(tipoExpedicion.DebeExpedirContenedor),
                FL_EMPAQUETA_CONTENEDOR = this.MapBooleanToString(tipoExpedicion.DebeEmpaquetarContenedor),
                FL_TRASPASO_EMPRESA = this.MapBooleanToString(tipoExpedicion.TraspasoEmpresa),
                FL_PRODUCCION = this.MapBooleanToString(tipoExpedicion.FlProduccion),
                FL_CONSUMO_INTERNO = this.MapBooleanToString(tipoExpedicion.IsConsumoInterno),
                FL_PERMITE_FACTURACION_PARCIAL = this.MapBooleanToString(tipoExpedicion.PermiteFacturacionParcial),
                FL_CIERRE_CAMION_EN_EMPAQUE = this.MapBooleanToString(tipoExpedicion.FlCierreCamionEnEmpaque),
                FL_FACTURA_AUTO_COMPLETAR = this.MapBooleanToString(tipoExpedicion.FlFacturaAutoCompletar),
                FL_PERMITE_ANULACION_PARCIAL = this.MapBooleanToString(tipoExpedicion.FlPermiteAnulacionParcial),
                FL_MODALIDAD_EMPAQUETADO = this.MapBooleanToString(tipoExpedicion.FlModalidadEmpaquetado),
                CD_GRUPO_EXPEDICION = tipoExpedicion.CodigoGrupoExpedicion,
                ID_DISPONIBLE = this.MapBooleanToString(tipoExpedicion.IsDisponible),
                FL_ANULA_PENDIENTES_AL_LIBERAR = this.MapBooleanToString(tipoExpedicion.FlAnulaPendientesAlLiberar),
                FL_EXPEDIR_COMPLETO = this.MapBooleanToString(tipoExpedicion.DebeExpedirCompleto),
                FL_PERMITE_FACT_SIN_PRECINTO = this.MapBooleanToString(tipoExpedicion.PermiteFacturacionSinPrecinto),
                VL_CANTIDAD_PRECINTOS = tipoExpedicion.CantidadPrecintos,
                FL_FACTURACION_REQUERIDA = this.MapBooleanToString(tipoExpedicion.IsFacturacionRequerida),
                FL_FACTURACION_PARCIAL = this.MapBooleanToString(tipoExpedicion.IsFacturacionParcial),
                FL_FACTURACION_POR_PEDIDO = this.MapBooleanToString(tipoExpedicion.IsFacturacionPorPedido),
                FL_REQUIERE_LIBERACION_TOTAL = this.MapBooleanToString(tipoExpedicion.RequiereLiberacionTotal),
                FL_TRACKING = this.MapBooleanToString(tipoExpedicion.FlTracking),
                FL_PREPARABLE = this.MapBooleanToString(tipoExpedicion.FlPreparable),
                FL_EMPAQUETA_SOLO_UN_PRODUCTO = this.MapBooleanToString(tipoExpedicion.FlEmpaquetaSoloUnProducto),
            };
        }

        //Pedido Excel
        public virtual PedidosExpedidosExel MapToPedidosExpedidosExelObject(V_PEDIDOS_EXPEDIDOS_EXEL entity)
        {
            return new PedidosExpedidosExel
            {
                CantidadExpedida = entity.QT_PRODUTO,
                CodigoAgente = entity.CD_AGENTE,
                CodigoProductoExterno = entity.CD_EXTERNO,
                Pedido = entity.NU_PEDIDO,
                TipoAgente = entity.TP_AGENTE,
                Cliente = entity.CD_CLIENTE,
                Empresa = entity.CD_EMPRESA,
                Camion = entity.CD_CAMION,
                CodigoProducto = entity.CD_PRODUTO,
                //Contenedor = entity.NU_CONTENEDOR,
                //DescripcionCliente = entity.DS_CLIENTE,
                DescripcionContenedor = entity.DS_CONTENEDOR,
                DescripcionProducto = entity.DS_PRODUTO,
                Predio = entity.NU_PREDIO,
                Ruta = entity.CD_ROTA,
                Transportadora = entity.CD_TRANSPORTADORA,
            };
        }

        //Pedido Expedido
        public virtual DetallePedidoExpedido MapToDetalleExpedicionObject(T_DET_PEDIDO_EXPEDIDO detExpedicionEntity)
        {
            return new DetallePedidoExpedido
            {
                Pedido = detExpedicionEntity.NU_PEDIDO,
                Cliente = detExpedicionEntity.CD_CLIENTE,
                Empresa = detExpedicionEntity.CD_EMPRESA,
                Producto = detExpedicionEntity.CD_PRODUTO,
                Faixa = detExpedicionEntity.CD_FAIXA,
                Identificador = detExpedicionEntity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Cantidad = detExpedicionEntity.QT_PRODUTO,
                FechaExpedicion = detExpedicionEntity.DT_EXPEDICION,
                Camion = detExpedicionEntity.CD_CAMION,
                EspecificaLote = this.MapStringToBoolean(detExpedicionEntity.ID_ESPECIFICA_IDENTIFICADOR),


            };
        }

        public virtual T_DET_PEDIDO_EXPEDIDO MapToDetalleExpedicionEntity(DetallePedidoExpedido detallePedidoExpedido)
        {
            return new T_DET_PEDIDO_EXPEDIDO
            {
                NU_PEDIDO = detallePedidoExpedido.Pedido,
                CD_CLIENTE = detallePedidoExpedido.Cliente,
                CD_EMPRESA = detallePedidoExpedido.Empresa,
                CD_PRODUTO = detallePedidoExpedido.Producto,
                CD_FAIXA = detallePedidoExpedido.Faixa,
                NU_IDENTIFICADOR = detallePedidoExpedido.Identificador?.Trim()?.ToUpper(),
                QT_PRODUTO = detallePedidoExpedido.Cantidad,
                DT_EXPEDICION = detallePedidoExpedido.FechaExpedicion,
                CD_CAMION = detallePedidoExpedido.Camion,
                ID_ESPECIFICA_IDENTIFICADOR = this.MapBooleanToString(detallePedidoExpedido.EspecificaLote),


            };
        }

        //Pedido Mostrador
        public virtual TempPedidoMostrador MapToPedidoMostradorObject(T_TEMP_PEDIDO_MOSTRADOR pedidoMostradorEntity)
        {
            return new TempPedidoMostrador
            {

                NumeroPreparacion = pedidoMostradorEntity.NU_PREPARACION,
                NumeroContenedor = pedidoMostradorEntity.NU_CONTENEDOR,
                NumeroPedido = pedidoMostradorEntity.NU_PEDIDO,
                CodigoEmpresa = pedidoMostradorEntity.CD_EMPRESA,
                CodigoCliente = pedidoMostradorEntity.CD_CLIENTE,
                NumeroCarga = pedidoMostradorEntity.NU_CARGA,
                CodigoCamionFacturado = pedidoMostradorEntity.CD_CAMION_FACTURADO,
                CodigoUbicacion = pedidoMostradorEntity.CD_ENDERECO,


            };
        }

        public virtual T_TEMP_PEDIDO_MOSTRADOR MapToPedidoMostradorEntity(TempPedidoMostrador tempPedidoMostrador)
        {
            return new T_TEMP_PEDIDO_MOSTRADOR
            {

                NU_PREPARACION = tempPedidoMostrador.NumeroPreparacion,
                NU_CONTENEDOR = tempPedidoMostrador.NumeroContenedor,
                NU_PEDIDO = tempPedidoMostrador.NumeroPedido,
                CD_EMPRESA = tempPedidoMostrador.CodigoEmpresa,
                CD_CLIENTE = tempPedidoMostrador.CodigoCliente,
                NU_CARGA = tempPedidoMostrador.NumeroCarga,
                CD_CAMION_FACTURADO = tempPedidoMostrador.CodigoCamionFacturado,
                CD_ENDERECO = tempPedidoMostrador.CodigoUbicacion,

            };
        }

        //Log Pedido anulado
        public virtual PedidoAnulado MapToObject(T_LOG_PEDIDO_ANULADO entity)
        {
            return new PedidoAnulado
            {
                Id = entity.NU_LOG_PEDIDO_ANULADO,
                Empresa = entity.CD_EMPRESA,
                Cliente = entity.CD_CLIENTE,
                Producto = entity.CD_PRODUTO,
                Pedido = entity.NU_PEDIDO,
                Embalaje = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                EspecificaIdentificador = this.MapStringToBoolean(entity.ID_ESPECIFICA_IDENTIFICADOR),
                CantidadAnulada = entity.QT_ANULADO,
                Motivo = entity.DS_MOTIVO,
                Funcionario = entity.CD_FUNCIONARIO,
                FechaInsercion = entity.DT_ADDROW,
                Aplicacion = entity.CD_APLICACAO,
                InterfazEjecucion = entity.NU_INTERFAZ_EJECUCION

            };
        }

        public virtual T_LOG_PEDIDO_ANULADO MapToEntity(PedidoAnulado obj)
        {
            return new T_LOG_PEDIDO_ANULADO
            {
                NU_LOG_PEDIDO_ANULADO = obj.Id,
                CD_CLIENTE = obj.Cliente,
                CD_EMPRESA = obj.Empresa,
                CD_PRODUTO = obj.Producto,
                NU_PEDIDO = obj.Pedido,
                CD_FAIXA = obj.Embalaje,
                NU_IDENTIFICADOR = obj.Identificador?.Trim()?.ToUpper(),
                ID_ESPECIFICA_IDENTIFICADOR = this.MapBooleanToString(obj.EspecificaIdentificador),
                QT_ANULADO = obj.CantidadAnulada,
                DS_MOTIVO = obj.Motivo,
                CD_FUNCIONARIO = obj.Funcionario,
                DT_ADDROW = obj.FechaInsercion,
                CD_APLICACAO = obj.Aplicacion,
                NU_INTERFAZ_EJECUCION = obj.InterfazEjecucion

            };
        }

        public virtual PedidoAnuladoLpn MapToObject(T_LOG_PEDIDO_ANULADO_LPN entity)
        {
            return new PedidoAnuladoLpn
            {
                Id = entity.NU_LOG_PEDIDO_ANULADO_LPN,
                IdLogPedidoAnulado = entity.NU_LOG_PEDIDO_ANULADO,
                TipoOperacion = entity.TP_OPERACION,
                IdExternoLpn = entity.ID_LPN_EXTERNO,
                TipoLpn = entity.TP_LPN_TIPO,
                IdConfiguracion = entity.NU_DET_PED_SAI_ATRIB,
                CantidadAnulada = entity.QT_ANULADO,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };
        }

        public virtual T_LOG_PEDIDO_ANULADO_LPN MapToEntity(PedidoAnuladoLpn obj)
        {
            return new T_LOG_PEDIDO_ANULADO_LPN
            {
                NU_LOG_PEDIDO_ANULADO_LPN = obj.Id,
                NU_LOG_PEDIDO_ANULADO = obj.IdLogPedidoAnulado,
                TP_OPERACION = obj.TipoOperacion,
                ID_LPN_EXTERNO = obj.IdExternoLpn,
                TP_LPN_TIPO = obj.TipoLpn,
                NU_DET_PED_SAI_ATRIB = obj.IdConfiguracion,
                QT_ANULADO = obj.CantidadAnulada,
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion,
            };
        }
    }
}
