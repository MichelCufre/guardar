using System.Globalization;
using System.Xml;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Salida;

namespace WIS.XmlProcessorSalida.Helpers
{
    public class ApiMapper
    {
        public virtual string Map(int interfazExterna, object response)
        {
            switch (interfazExterna)
            {
                case CInterfazExterna.ConfirmacionDeRecepcion:
                    return MapConfirmacionDeRecepcion((ConfirmacionRecepcionResponse)response);
                case CInterfazExterna.ConfirmacionDePedido:
                    return MapConfirmacionDePedidos((ConfirmacionPedidoResponse)response);
                case CInterfazExterna.PedidosAnulados:
                    return MapPedidosAnulados((PedidosAnuladosResponse)response);
                case CInterfazExterna.Facturacion:
                    return MapConfirmacionMercaderiaPreparada((FacturacionResponse)response);
                case CInterfazExterna.AjustesDeStock:
                    return MapAjustes((AjustesResponse)response);
                default:
                    throw new NotImplementedException();
            }
        }

        #region Mappers

        protected virtual string MapConfirmacionDeRecepcion(ConfirmacionRecepcionResponse response)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("AGENDA");

            root.AppendChild(CreateElement(xmlDoc, "NU_AGENDA", response.Agenda.ToString()));
            root.AppendChild(CreateElement(xmlDoc, "CD_EMPRESA", response.Empresa.ToString()));
            root.AppendChild(CreateElement(xmlDoc, "TP_AGENTE", response.TipoAgente));
            root.AppendChild(CreateElement(xmlDoc, "CD_AGENTE", response.CodigoAgente));
            root.AppendChild(CreateElement(xmlDoc, "TP_RECEPCION", response.TipoRecepcion));
            root.AppendChild(CreateElement(xmlDoc, "NU_DOCUMENTO", response.NumeroDocumento));
            root.AppendChild(CreateElement(xmlDoc, "DT_ADDROW", response.FechaIngreso));
            root.AppendChild(CreateElement(xmlDoc, "DT_CIERRE", response.FechaCierre));
            root.AppendChild(CreateElement(xmlDoc, "DS_ANEXO1", response.Anexo1));

            XmlElement detallesAgendaElement = xmlDoc.CreateElement("AGENDA_DET");

            foreach (var detalle in response.Detalles)
            {
                XmlElement detalleElement = xmlDoc.CreateElement("LINEA");

                detalleElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", detalle.Producto));
                detalleElement.AppendChild(CreateElement(xmlDoc, "NU_IDENTIFICADOR", detalle.Identificador));
                detalleElement.AppendChild(CreateElement(xmlDoc, "QT_AGENDADO_ORIGINAL", detalle.CantidadTeorica.ToString(CultureInfo.InvariantCulture)));
                detalleElement.AppendChild(CreateElement(xmlDoc, "QT_RECIBIDO", detalle.CantidadRecibida?.ToString(CultureInfo.InvariantCulture)));

                detallesAgendaElement.AppendChild(detalleElement);
            }

            root.AppendChild(detallesAgendaElement);

            XmlElement referenciasElement = xmlDoc.CreateElement("REFERENCIAS");

            foreach (var referencia in response.Referencias)
            {
                XmlElement referenciaElement = xmlDoc.CreateElement("REFERENCIA");

                referenciaElement.AppendChild(CreateElement(xmlDoc, "NU_REFERENCIA", referencia.NumeroReferencia));
                referenciaElement.AppendChild(CreateElement(xmlDoc, "TP_REFERENCIA", referencia.TipoReferencia));
                referenciaElement.AppendChild(CreateElement(xmlDoc, "TP_AGENTE", referencia.TipoAgente));
                referenciaElement.AppendChild(CreateElement(xmlDoc, "CD_AGENTE", referencia.CodigoAgente));
                referenciaElement.AppendChild(CreateElement(xmlDoc, "DS_MEMO", referencia.Memo));
                referenciaElement.AppendChild(CreateElement(xmlDoc, "NU_PREDIO", referencia.Predio));

                XmlElement detallesReferenciaElement = xmlDoc.CreateElement("REFERENCIA_DET");

                foreach (var detalle in referencia.Detalles)
                {
                    XmlElement detalleReferenciaElement = xmlDoc.CreateElement("LINEA");

                    detalleReferenciaElement.AppendChild(CreateElement(xmlDoc, "ID_LINEA_SISTEMA_EXTERNO", detalle.IdLineaSistemaExterno));
                    detalleReferenciaElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", detalle.Producto));
                    detalleReferenciaElement.AppendChild(CreateElement(xmlDoc, "NU_IDENTIFICADOR", detalle.Identificador));
                    detalleReferenciaElement.AppendChild(CreateElement(xmlDoc, "QT_REFERENCIA", detalle.CantidadReferencia.ToString(CultureInfo.InvariantCulture)));
                    detalleReferenciaElement.AppendChild(CreateElement(xmlDoc, "QT_CONSUMIDA", detalle.CantidadConsumida?.ToString(CultureInfo.InvariantCulture)));
                    detalleReferenciaElement.AppendChild(CreateElement(xmlDoc, "DS_ANEXO1", detalle.Anexo));
                    detalleReferenciaElement.AppendChild(CreateElement(xmlDoc, "QT_CONSUMIDA_AGENDA", detalle.CantidadConsumidaAgenda?.ToString(CultureInfo.InvariantCulture)));

                    detallesReferenciaElement.AppendChild(detalleReferenciaElement);
                }

                referenciaElement.AppendChild(detallesReferenciaElement);
                referenciasElement.AppendChild(referenciaElement);
            }

            root.AppendChild(referenciasElement);

            XmlElement facturasElement = xmlDoc.CreateElement("FACTURAS");

            foreach (var factura in response.Facturas)
            {
                XmlElement facturaElement = xmlDoc.CreateElement("FACTURA");

                facturaElement.AppendChild(CreateElement(xmlDoc, "NU_SERIE", factura.Serie));
                facturaElement.AppendChild(CreateElement(xmlDoc, "NU_FACTURA", factura.Factura));
                facturaElement.AppendChild(CreateElement(xmlDoc, "TP_FACTURA", factura.TipoFactura));
                facturaElement.AppendChild(CreateElement(xmlDoc, "DT_EMISION", factura.FechaEmision));
                facturaElement.AppendChild(CreateElement(xmlDoc, "IM_TOTAL_DIGITADO", factura.TotalDigitado?.ToString(CultureInfo.InvariantCulture)));
                facturaElement.AppendChild(CreateElement(xmlDoc, "ID_ORIGEN", factura.Origen));

                XmlElement detallesFacturasElement = xmlDoc.CreateElement("FACTURA_DET");

                foreach (var detalle in factura.Detalles)
                {
                    XmlElement detalleFacturaElement = xmlDoc.CreateElement("LINEA");

                    detalleFacturaElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", detalle.Producto));
                    detalleFacturaElement.AppendChild(CreateElement(xmlDoc, "NU_IDENTIFICADOR", detalle.Identificador));
                    detalleFacturaElement.AppendChild(CreateElement(xmlDoc, "QT_FACTURADA", detalle.CantidadFacturada.ToString(CultureInfo.InvariantCulture)));
                    detalleFacturaElement.AppendChild(CreateElement(xmlDoc, "QT_VALIDADA", detalle.CantidadValidada.ToString(CultureInfo.InvariantCulture)));
                    detalleFacturaElement.AppendChild(CreateElement(xmlDoc, "QT_RECIBIDA", detalle.CantidadRecibida.ToString(CultureInfo.InvariantCulture)));
                    detalleFacturaElement.AppendChild(CreateElement(xmlDoc, "DT_VENCIMIENTO", detalle.FechaVencimiento));

                    detallesFacturasElement.AppendChild(detalleFacturaElement);
                }

                facturaElement.AppendChild(detallesFacturasElement);
                facturasElement.AppendChild(facturaElement);
            }

            root.AppendChild(facturasElement);

            xmlDoc.AppendChild(root);

			/*
				 Campos 2.0 sin mapeo:
				 
				 - TP_RECEPCION_EXTERNO
            */

			return xmlDoc.OuterXml;
        }

        protected virtual string MapConfirmacionDePedidos(ConfirmacionPedidoResponse response)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("CONFIRMACION_PEDIDOS");

            root.AppendChild(CreateElement(xmlDoc, "CD_CAMION", response.Camion));
            root.AppendChild(CreateElement(xmlDoc, "CD_PLACA_CARRO", response.Matricula));
            root.AppendChild(CreateElement(xmlDoc, "CD_ROTA", response.Ruta?.ToString()));
            root.AppendChild(CreateElement(xmlDoc, "CD_TRANSPORTADORA", response.Transportadora?.ToString()));
            root.AppendChild(CreateElement(xmlDoc, "CD_VEICULO", response.Vehiculo?.ToString()));
            root.AppendChild(CreateElement(xmlDoc, "DS_CAMION", response.DescripcionCamion));
            root.AppendChild(CreateElement(xmlDoc, "DT_FACTURACION", response.FechaFacturacion));
            root.AppendChild(CreateElement(xmlDoc, "NU_PREDIO", response.Predio));

            XmlElement pedidosElement = xmlDoc.CreateElement("PEDIDOS");

            foreach (var pedido in response.Pedidos)
            {
                XmlElement pedidoElement = xmlDoc.CreateElement("PEDIDO");

                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_EMPRESA", pedido.Empresa.ToString()));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "TP_AGENTE", pedido.TipoAgente));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_AGENTE", pedido.CodigoAgente));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "NU_PEDIDO", pedido.Pedido));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_ORIGEN", pedido.CodigoOrigen));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "DS_MEMO", pedido.Memo));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "DS_MEMO_1", pedido.Memo1));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_CONDICION_LIBERACION", pedido.CondicionLiberacion));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "NU_PREDIO", pedido.Predio));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "TP_PEDIDO", pedido.TipoPedido));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "TP_EXPEDICION", pedido.TipoExpedicion));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "DS_ENDERECO", pedido.Direccion));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_PUNTO_ENTREGA", pedido.PuntoEntrega));

                XmlElement detallesPedidoElement = xmlDoc.CreateElement("PEDIDO_LINEAS");

                foreach (var detalle in pedido.Detalles)
                {
                    XmlElement detalleElement = xmlDoc.CreateElement("PEDIDO_LINEA");

                    detalleElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", detalle.Producto));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "NU_IDENTIFICADOR", detalle.Identificador));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "ID_ESPECIFICA_IDENTIFICADOR", detalle.EspecificaIdentificador));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "QT_PRODUTO", detalle.CantidadProducto.ToString(CultureInfo.InvariantCulture)));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "DS_MEMO", detalle.Memo));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "VL_SERIALIZADO_1", detalle.Serializado));

                    detallesPedidoElement.AppendChild(detalleElement);
                }

                pedidoElement.AppendChild(detallesPedidoElement);
                pedidosElement.AppendChild(pedidoElement);
            }

            root.AppendChild(pedidosElement);

            XmlElement contenedoresElement = xmlDoc.CreateElement("CONTENEDORES");

            foreach (var contenedor in response.Contenedores)
            {
                XmlElement contenedorElement = xmlDoc.CreateElement("CONTENEDOR");

                contenedorElement.AppendChild(CreateElement(xmlDoc, "NU_PREPARACION", contenedor.Preparacion.ToString()));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "NU_CONTENEDOR", contenedor.NumeroContenedor.ToString()));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "TP_CONTENEDOR", contenedor.TipoContenedor));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "CD_SUB_CLASSE", contenedor.SubClase));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "DT_EXPEDIDO", contenedor.FechaExpedicion));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "PS_REAL", contenedor.PesoReal?.ToString(CultureInfo.InvariantCulture)));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "VL_ALTURA", contenedor.Altura?.ToString(CultureInfo.InvariantCulture)));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "VL_LARGURA", contenedor.Largo?.ToString(CultureInfo.InvariantCulture)));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "VL_CUBAGEM", contenedor.Volumen?.ToString(CultureInfo.InvariantCulture)));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "VL_PROFUNDIDADE", contenedor.Profundidad?.ToString(CultureInfo.InvariantCulture)));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "CD_UNIDAD_BULTO", contenedor.CodigoUnidadBulto));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "QT_BULTO", contenedor.CantidadBultos?.ToString()));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "DS_CONTENEDOR", contenedor.DescripcionContenedor));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "ID_PRECINTO_1", contenedor.Precinto1));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "ID_PRECINTO_2", contenedor.Precinto2));

                XmlElement detallesContenedoresElement = xmlDoc.CreateElement("CONTENEDOR_DETALLES");

                foreach (var detalle in contenedor.Detalles)
                {
                    XmlElement detalleElement = xmlDoc.CreateElement("CONTENEDOR_DETALLE");

                    detalleElement.AppendChild(CreateElement(xmlDoc, "NU_PEDIDO", detalle.Pedido));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "CD_AGENTE", detalle.CodigoAgente));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "TP_AGENTE", detalle.TipoAgente));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", detalle.Producto));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "NU_IDENTIFICADOR", detalle.Identificador));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "DT_VENCIMIENTO_PICKEO", detalle.FechaVencimientoPickeo));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "ID_AVERIA_PICKEO", detalle.AveriaPickeo));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "QT_PREPARADO", detalle.CantidadPreparada.ToString(CultureInfo.InvariantCulture)));

                    detallesContenedoresElement.AppendChild(detalleElement);
                }

                contenedorElement.AppendChild(detallesContenedoresElement);
                contenedoresElement.AppendChild(contenedorElement);
            }

            root.AppendChild(contenedoresElement);

            // Entregas
            //XmlElement entregasElement = xmlDoc.CreateElement("ENTREGAS");
            //foreach (var entrega in response.Entregas)
            //{
            //	XmlElement entregaElement = xmlDoc.CreateElement("ENTREGA");
            //	entregaElement.AppendChild(CreateElement(xmlDoc, "NU_ENTREGA", entrega.NumeroEntrega.ToString()));
            //	entregaElement.AppendChild(CreateElement(xmlDoc, "TP_ENTREGA", entrega.TipoEntrega));
            //	entregaElement.AppendChild(CreateElement(xmlDoc, "DS_ENTREGA", entrega.DescripcionEntrega));
            //	entregaElement.AppendChild(CreateElement(xmlDoc, "CD_CLIENTE", entrega.CodigoAgente));
            //	entregaElement.AppendChild(CreateElement(xmlDoc, "CD_BARRAS", entrega.CodigoBarras));
            //	entregaElement.AppendChild(CreateElement(xmlDoc, "NU_CONTENEDOR", entrega.NumeroContenedor.ToString()));
            //	entregaElement.AppendChild(CreateElement(xmlDoc, "NU_PREPARACION", entrega.NumeroPreparacion.ToString()));
            //	entregaElement.AppendChild(CreateElement(xmlDoc, "VL_AGRUPACION_ENTREGA", entrega.Agrupacion));
            //	entregaElement.AppendChild(CreateElement(xmlDoc, "CD_PUNTO_ENTREGA", entrega.PuntoEntrega));
            //	entregaElement.AppendChild(CreateElement(xmlDoc, "DS_ANEXO", entrega.Anexo));
            //	entregasElement.AppendChild(entregaElement);
            //}

            //root.AppendChild(entregasElement);

            xmlDoc.AppendChild(root);

			/*
				 Campos 2.0 sin mapeo:
				 
                Contenedor
				 - CD_PORTA
                 - CD_FUNCIONARIO_EXPEDICION
            */

			return xmlDoc.OuterXml;
        }

        protected virtual string MapPedidosAnulados(PedidosAnuladosResponse response)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("PEDIDOS_ANULADOS");

            foreach (var pedidoAnulado in response.PedidosAnulados)
            {
                XmlElement pedidoElement = xmlDoc.CreateElement("PEDIDO_ANULADO");

                // CABEZAL
                pedidoElement.AppendChild(CreateElement(xmlDoc, "NU_PEDIDO", pedidoAnulado.Pedido));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_EMPRESA", pedidoAnulado.Empresa?.ToString()));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_CLIENTE", pedidoAnulado.CodigoAgente));

                // DETALLE
                foreach (var detalle in pedidoAnulado.Detalles)
                {
                    XmlElement detalleElement = xmlDoc.CreateElement("DETALLE");

                    detalleElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", detalle.Producto));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "NU_IDENTIFICADOR", detalle.Identificador));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "QT_ANULADO", detalle.CantidadAnulada?.ToString(CultureInfo.InvariantCulture)));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "CD_FUNCIONARIO", detalle.Funcionario?.ToString()));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "DS_MOTIVO", detalle.Motivo));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "CD_APLICACAO", detalle.Aplicacion));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "DT_ADDROW", detalle.FechaAlta));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "ID_ESPECIFICA_IDENTIFICADOR", detalle.EspecificaIdentificador));

                    pedidoElement.AppendChild(detalleElement);
                }

                root.AppendChild(pedidoElement);
            }

            xmlDoc.AppendChild(root);

            return xmlDoc.OuterXml;
        }

		protected virtual string MapAjustes(AjustesResponse ajustesResponse)
		{
			XmlDocument xmlDoc = new XmlDocument();

			XmlElement root = xmlDoc.CreateElement("AJUSTES");
			xmlDoc.AppendChild(root);

			foreach (var ajuste in ajustesResponse.Ajustes)
			{
				XmlElement ajusteElement = xmlDoc.CreateElement("AJUSTE");

				ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_APLICACAO", ajuste.Aplicacion));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_EMPRESA", ajuste.Empresa.ToString()));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_ENDERECO", ajuste.Ubicacion));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_FAIXA", ajuste.Faixa.ToString(CultureInfo.InvariantCulture)));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_FUNCIONARIO", ajuste.Funcionario?.ToString()));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_MOTIVO_AJUSTE", ajuste.Motivo));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", ajuste.Producto));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "DS_MOTIVO", ajuste.DescripcionMotivo));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "DT_MOTIVO", ajuste.FechaMotivo));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "DT_REALIZADO", ajuste.FechaRealizacion));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "DT_UPDROW", ajuste.FechaUltimaModificacion));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "ID_AREA_AVERIA", ajuste.AreaAveria));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "NU_AJUSTE_STOCK", ajuste.NumeroAjusteStock.ToString()));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "NU_IDENTIFICADOR", ajuste.Identificador));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "NU_INTERFAZ_EJECUCION", ajuste.InterfazEjecucion?.ToString()));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "NU_LOG_INVENTARIO", ajuste.NroLogInventario?.ToString()));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "NU_PREDIO", ajuste.Predio));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "QT_MOVIMIENTO", ajuste.CantidadMovimiento?.ToString(CultureInfo.InvariantCulture)));
				ajusteElement.AppendChild(CreateElement(xmlDoc, "TP_AJUSTE", ajuste.TipoAjuste));

				root.AppendChild(ajusteElement);
			}

			/*
				 Campos 2.0 sin mapeo:
				 
				 - CD_FUNC_MOTIVO
                 - ID_PROCESADO
                 - ID_PROCESAR
                 - NU_DOCUMENTO
                 - NU_INVENTARIO_ENDERECO_DET
                 - NU_REGISTRO
                 - NU_TRANSACCION
                 - TP_DOCUMENTO
            */

			return xmlDoc.OuterXml;
		}

        protected virtual string MapConfirmacionMercaderiaPreparada(FacturacionResponse response)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("CONF_MERC_PREPARADA");

            root.AppendChild(CreateElement(xmlDoc, "CD_CAMION", response.CodigoCamion.ToString()));
            root.AppendChild(CreateElement(xmlDoc, "CD_PLACA_CARRO", response.MatriculaCamion));
            root.AppendChild(CreateElement(xmlDoc, "CD_ROTA", response.Ruta?.ToString()));
            root.AppendChild(CreateElement(xmlDoc, "CD_TRANSPORTADORA", response.Transportadora?.ToString()));
            root.AppendChild(CreateElement(xmlDoc, "CD_VEICULO", response.Vehiculo?.ToString()));
            root.AppendChild(CreateElement(xmlDoc, "DS_CAMION", response.DescripcionCamion));
            root.AppendChild(CreateElement(xmlDoc, "DT_FACTURACION", response.FechaFacturacion));
            root.AppendChild(CreateElement(xmlDoc, "NU_PREDIO", response.Predio));

            XmlElement pedidosElement = xmlDoc.CreateElement("PEDIDOS");

            foreach (var pedido in response.Pedidos)
            {
                XmlElement pedidoElement = xmlDoc.CreateElement("PEDIDO");

                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_EMPRESA", pedido.Empresa.ToString()));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "TP_AGENTE", pedido.TipoAgente));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_AGENTE", pedido.CodigoAgente));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "NU_PEDIDO", pedido.Pedido));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_ORIGEN", pedido.CodigoOrigen));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "DS_MEMO", pedido.Memo));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "DS_MEMO_1", pedido.Memo1));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_CONDICION_LIBERACION", pedido.CondicionLiberacion));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "NU_PREDIO", pedido.Predio));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "TP_PEDIDO", pedido.TipoPedido));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "TP_EXPEDICION", pedido.TipoExpedicion));
                pedidoElement.AppendChild(CreateElement(xmlDoc, "CD_PUNTO_ENTREGA", pedido.PuntoEntrega));

                XmlElement pedidoLineasElement = xmlDoc.CreateElement("PEDIDO_LINEAS");

                foreach (var linea in pedido.Detalles)
                {
                    XmlElement lineaElement = xmlDoc.CreateElement("PEDIDO_LINEA");

                    lineaElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", linea.Producto));
                    lineaElement.AppendChild(CreateElement(xmlDoc, "NU_IDENTIFICADOR", linea.Identificador));
                    lineaElement.AppendChild(CreateElement(xmlDoc, "ID_ESPECIFICA_IDENTIFICADOR", linea.EspecificaIdentificador));
                    lineaElement.AppendChild(CreateElement(xmlDoc, "QT_PRODUTO", linea.CantidadProducto.ToString(CultureInfo.InvariantCulture)));
                    lineaElement.AppendChild(CreateElement(xmlDoc, "DS_MEMO", linea.Memo));
                    lineaElement.AppendChild(CreateElement(xmlDoc, "VL_SERIALIZADO_1", linea.Serializado));

                    pedidoLineasElement.AppendChild(lineaElement);
                }

                pedidoElement.AppendChild(pedidoLineasElement);
                pedidosElement.AppendChild(pedidoElement);
            }

            root.AppendChild(pedidosElement);

            XmlElement contenedoresElement = xmlDoc.CreateElement("CONTENEDORES");

            foreach (var contenedor in response.Contenedores)
            {
                XmlElement contenedorElement = xmlDoc.CreateElement("CONTENEDOR");

                contenedorElement.AppendChild(CreateElement(xmlDoc, "NU_PREPARACION", contenedor.Preparacion.ToString()));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "NU_CONTENEDOR", contenedor.NumeroContenedor.ToString()));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "TP_CONTENEDOR", contenedor.TipoContenedor));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "CD_SUB_CLASSE", contenedor.SubClase));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "DT_EXPEDIDO", contenedor.FechaExpedicion));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "PS_REAL", contenedor.PesoReal?.ToString(CultureInfo.InvariantCulture)));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "VL_ALTURA", contenedor.Altura?.ToString(CultureInfo.InvariantCulture)));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "VL_LARGURA", contenedor.Largo?.ToString(CultureInfo.InvariantCulture)));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "VL_CUBAGEM", contenedor.Volumen?.ToString(CultureInfo.InvariantCulture)));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "VL_PROFUNDIDADE", contenedor.Profundidad?.ToString(CultureInfo.InvariantCulture)));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "CD_UNIDAD_BULTO", contenedor.CodigoUnidadBulto));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "QT_BULTO", contenedor.CantidadBultos?.ToString()));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "DS_CONTENEDOR", contenedor.DescripcionContenedor));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "ID_PRECINTO_1", contenedor.Precinto1));
                contenedorElement.AppendChild(CreateElement(xmlDoc, "ID_PRECINTO_2", contenedor.Precinto2));

                XmlElement contenedorDetallesElement = xmlDoc.CreateElement("CONTENEDOR_DETALLES");

                foreach (var detalle in contenedor.Detalles)
                {
                    XmlElement detalleElement = xmlDoc.CreateElement("CONTENEDOR_DETALLE");

                    detalleElement.AppendChild(CreateElement(xmlDoc, "NU_PEDIDO", detalle.Pedido));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "CD_AGENTE", detalle.CodigoAgente));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "TP_AGENTE", detalle.TipoAgente));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", detalle.Producto));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "NU_IDENTIFICADOR", detalle.Identificador));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "DT_VENCIMIENTO_PICKEO", detalle.FechaVencimientoPickeo));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "ID_AVERIA_PICKEO", detalle.AveriaPickeo));
                    detalleElement.AppendChild(CreateElement(xmlDoc, "QT_PREPARADO", detalle.CantidadPreparada.ToString(CultureInfo.InvariantCulture)));

					contenedorDetallesElement.AppendChild(detalleElement);
                }

                contenedorElement.AppendChild(contenedorDetallesElement);
                contenedoresElement.AppendChild(contenedorElement);
            }

            root.AppendChild(contenedoresElement);

            xmlDoc.AppendChild(root);

			/*
				 Campos 2.0 sin mapeo:
				 
                Pedido
				 - DS_ENDERECO
                 - CD_CATEGORIA_CLIENTE

               Contenedor
                - CD_PORTA
                - CD_FUNCIONARIO_EXPEDICION
                - DETALLE_CONTENEDOR
                - TP_PEDIDO
                - CD_CATEGORIA_CLIENTE
            */

			return xmlDoc.OuterXml;
        }

		#endregion

		#region Utils

        protected virtual string ConvertToXml(AjustesResponse response)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("AJUSTES");

            foreach (var ajuste in response.Ajustes)
            {
                XmlElement ajusteElement = xmlDoc.CreateElement("AJUSTE");

                ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_APLICACAO", ajuste.Aplicacion));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_EMPRESA", ajuste.Empresa.ToString()));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_ENDERECO", ajuste.Ubicacion));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_FAIXA", ajuste.Faixa.ToString(CultureInfo.InvariantCulture)));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_FUNCIONARIO", ajuste.Funcionario?.ToString()));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_MOTIVO_AJUSTE", ajuste.Motivo));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", ajuste.Producto));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "DS_MOTIVO", ajuste.DescripcionMotivo));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "DT_REALIZADO", ajuste.FechaRealizacion));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "DT_UPDROW", ajuste.FechaUltimaModificacion));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "ID_AREA_AVERIA", ajuste.AreaAveria));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "NU_AJUSTE_STOCK", ajuste.NumeroAjusteStock.ToString()));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "NU_IDENTIFICADOR", ajuste.Identificador));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "NU_INTERFAZ_EJECUCION", ajuste.InterfazEjecucion?.ToString()));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "NU_LOG_INVENTARIO", ajuste.NroLogInventario?.ToString()));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "NU_PREDIO", ajuste.Predio));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "QT_MOVIMIENTO", ajuste.CantidadMovimiento?.ToString(CultureInfo.InvariantCulture)));
                ajusteElement.AppendChild(CreateElement(xmlDoc, "TP_AJUSTE", ajuste.TipoAjuste));

                root.AppendChild(ajusteElement);
            }

            xmlDoc.AppendChild(root);

            return xmlDoc.OuterXml;
        }

        protected virtual XmlElement CreateElement(XmlDocument doc, string name, string value)
        {
            XmlElement element = doc.CreateElement(name);

			element.InnerText = value;

			return element;
        }

        #endregion
    }
}
