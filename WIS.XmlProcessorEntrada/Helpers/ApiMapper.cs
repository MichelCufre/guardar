using System.Globalization;
using System.Xml;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.XmlProcessorEntrada.Models;

namespace WIS.XmlProcessorEntrada.Helpers
{
    public class ApiMapper
    {
        public const string WISIE_ID_REQUEST_FORMAT = "WISIE{0}_{1}";

        public virtual List<ApiRequest> Map(int interfazExterna, int empresa, long numeroEjecucion, string xml, out object extraData)
        {
            var requests = new List<ApiRequest>();

            extraData = null;

            switch (interfazExterna)
            {
                case CInterfazExterna.Empresas:
                    Add(interfazExterna, MapEmpresas(xml, empresa, numeroEjecucion, out List<AgentesRequest> agentes, out extraData), requests);

                    foreach (var request in agentes)
                    {
                        Add(CInterfazExterna.Agentes, request, requests);
                    }

                    break;
                case CInterfazExterna.Producto:
                    Add(interfazExterna, MapProductos(xml, empresa, numeroEjecucion, out CodigosBarrasRequest codigosBarras, out ProductosProveedorRequest productosProveedor, out extraData), requests);

                    if (codigosBarras.CodigosDeBarras.Count > 0)
                    {
                        Add(CInterfazExterna.CodigoDeBarras, codigosBarras, requests);
                    }

                    if (productosProveedor.Productos.Count > 0)
                    {
                        Add(CInterfazExterna.ProductoProveedor, productosProveedor, requests);
                    }

                    break;
                case CInterfazExterna.CodigoDeBarras:
                    Add(interfazExterna, MapCodigosDeBarra(xml, empresa, numeroEjecucion, out extraData), requests);
                    break;
                case CInterfazExterna.ProductoProveedor:
                    Add(interfazExterna, MapProductosProvedor(xml, empresa, numeroEjecucion, out extraData), requests);
                    break;
                case CInterfazExterna.Agentes:
                    Add(interfazExterna, MapAgentes(xml, empresa, numeroEjecucion, out extraData), requests);
                    break;
                case CInterfazExterna.AnulacionReferenciaRecepcion:
                    Add(interfazExterna, MapAnulacionReferenciaRecepcion(xml, empresa, numeroEjecucion, out extraData), requests);
                    break;
                case CInterfazExterna.ModificarDetalleReferenciaRecepcion:
                    Add(interfazExterna, MapModificarDetalleReferenciaRecepcion(xml, empresa, numeroEjecucion, out extraData), requests);
                    break;
                case CInterfazExterna.Pedidos:
                    Add(interfazExterna, MapPedidos(xml, empresa, numeroEjecucion, out extraData), requests);
                    break;
                case CInterfazExterna.ReferenciaDeRecepcion:
                    Add(interfazExterna, MapReferenciaRecepcion(xml, empresa, numeroEjecucion, out extraData), requests);
                    break;
                case CInterfazExterna.ModificarPedido:
                    Add(interfazExterna, MapModificarPedido(xml, empresa, numeroEjecucion, out extraData), requests);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return requests;
        }

        #region Mappers

        protected virtual EmpresasRequest MapEmpresas(string xml, int numeroEmpresa, long numeroEjecucion, out List<AgentesRequest> agentesRequests, out object extraData)
        {
            var ordenEjecucion = 1;
            var empresasRequest = new EmpresasRequest();

            agentesRequests = new List<AgentesRequest>();
            extraData = null;

            empresasRequest.Empresa = numeroEmpresa;
            empresasRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            empresasRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, ordenEjecucion);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            var empresas = xmlDoc.SelectNodes("/MAESTRO_EMPRESA/EMPRESAS/EMPRESA");

            foreach (XmlNode empresa in empresas)
            {
                var empresaRequest = new EmpresaRequest();

                empresaRequest.Codigo = ParsearValorNoNull<int>(empresa.SelectSingleNode("CD_EMPRESA")?.InnerText);
                empresaRequest.Nombre = empresa.SelectSingleNode("NM_EMPRESA")?.InnerText;
                empresaRequest.Estado = ParsearValor<short>(empresa.SelectSingleNode("CD_SITUACAO")?.InnerText);
                empresaRequest.Direccion = empresa.SelectSingleNode("DS_ENDERECO")?.InnerText;
                empresaRequest.Telefono = empresa.SelectSingleNode("NU_TELEFONE")?.InnerText;
                empresaRequest.Subdivision = empresa.SelectSingleNode("CD_UF")?.InnerText;
                empresaRequest.NumeroFiscal = empresa.SelectSingleNode("CD_CGC_EMPRESA")?.InnerText;
                empresaRequest.ProveedorDevolucion = ParsearValor<int>(empresa.SelectSingleNode("CD_FORN_DEVOLUCAO")?.InnerText);
                empresaRequest.ValorPallet = ParsearValor<decimal>(empresa.SelectSingleNode("VL_POS_PALETE")?.InnerText);
                empresaRequest.ValorPalletDia = ParsearValor<decimal>(empresa.SelectSingleNode("VL_POS_PALETE_DIA")?.InnerText);
                empresaRequest.CantidadDiasPeriodo = ParsearValor<short>(empresa.SelectSingleNode("QT_DIAS_POR_PERIODO")?.InnerText);
                empresaRequest.CodigoPostal = empresa.SelectSingleNode("DS_CP_POSTAL")?.InnerText;
                empresaRequest.ClienteArmadoKit = empresa.SelectSingleNode("CD_CLIENTE_ARMADO_KIT")?.InnerText;
                empresaRequest.Pais = empresa.SelectSingleNode("CD_PAIS")?.InnerText;
                empresaRequest.Anexo1 = empresa.SelectSingleNode("DS_ANEXO1")?.InnerText;
                empresaRequest.Anexo2 = empresa.SelectSingleNode("DS_ANEXO2")?.InnerText;
                empresaRequest.Anexo3 = empresa.SelectSingleNode("DS_ANEXO3")?.InnerText;
                empresaRequest.Anexo4 = empresa.SelectSingleNode("DS_ANEXO4")?.InnerText;
                empresaRequest.ValorMinimoStock = ParsearValor<decimal>(empresa.SelectSingleNode("IM_MINIMO_STOCK")?.InnerText);
                empresaRequest.IdUnidadFactura = empresa.SelectSingleNode("ID_UND_FACT_EMPRESA")?.InnerText;
                empresaRequest.IdOperativo = empresa.SelectSingleNode("ID_OPERATIVO")?.InnerText;
                empresaRequest.TipoDeAlmacenajeYSeguro = ParsearValor<short>(empresa.SelectSingleNode("TP_ALMACENAJE_Y_SEGURO")?.InnerText);
                empresaRequest.IdDAP = empresa.SelectSingleNode("ID_DAP")?.InnerText;
                empresaRequest.EmpresaConsolidado = ParsearValor<int>(empresa.SelectSingleNode("CD_EMPRESA_DE_CONSOLIDADO")?.InnerText);
                empresaRequest.ListaPrecio = ParsearValor<int>(empresa.SelectSingleNode("CD_LISTA_PRECIO")?.InnerText);
                empresaRequest.TipoFiscal = empresa.SelectSingleNode("TP_EMPRESA")?.InnerText;

                XmlNodeList agentes = empresa.SelectNodes("AGENTES/AGENTE");

                if (agentes != null && agentes.Count > 0)
                {
                    ordenEjecucion++;

                    var agentesRequest = new AgentesRequest();
                    agentesRequest.Empresa = empresaRequest.Codigo;
                    agentesRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
                    agentesRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, ordenEjecucion);

                    foreach (XmlNode agente in agentes)
                    {
                        var agenteRequest = new AgenteRequest();

                        agenteRequest.CodigoAgente = agente.SelectSingleNode("CD_AGENTE")?.InnerText;
                        agenteRequest.Ruta = ParsearValor<short>(agente.SelectSingleNode("CD_ROTA")?.InnerText);
                        agenteRequest.Descripcion = agente.SelectSingleNode("DS_AGENTE")?.InnerText;
                        agenteRequest.Direccion = agente.SelectSingleNode("DS_ENDERECO")?.InnerText;
                        agenteRequest.Barrio = agente.SelectSingleNode("DS_BAIRRO")?.InnerText;
                        agenteRequest.CodigoPostal = agente.SelectSingleNode("CD_CEP")?.InnerText;
                        agenteRequest.TelefonoPrincipal = agente.SelectSingleNode("NU_TELEFONE")?.InnerText;
                        agenteRequest.TelefonoSecundario = agente.SelectSingleNode("NU_FAX")?.InnerText;
                        agenteRequest.OtroDatoFiscal = agente.SelectSingleNode("NU_INSCRICAO")?.InnerText;
                        agenteRequest.NumeroFiscal = agente.SelectSingleNode("CD_CGC_CLIENTE")?.InnerText;
                        agenteRequest.Estado = ParsearValor<short>(agente.SelectSingleNode("CD_SITUACAO")?.InnerText);
                        agenteRequest.Anexo1 = agente.SelectSingleNode("DS_ANEXO1")?.InnerText;
                        agenteRequest.Anexo2 = agente.SelectSingleNode("DS_ANEXO2")?.InnerText;
                        agenteRequest.Anexo3 = agente.SelectSingleNode("DS_ANEXO3")?.InnerText;
                        agenteRequest.Anexo4 = agente.SelectSingleNode("DS_ANEXO4")?.InnerText;
                        agenteRequest.Tipo = agente.SelectSingleNode("TP_AGENTE")?.InnerText;
                        agenteRequest.NumeroLocalizacionGlobal = ParsearValor<long>(agente.SelectSingleNode("CD_GLN")?.InnerText);

                        agentesRequest.Agentes.Add(agenteRequest);
                    }

                    agentesRequests.Add(agentesRequest);
                }

                empresasRequest.Empresas.Add(empresaRequest);
            }

            /*
				 Campos 2.0 sin mapeo:
				 
				 - DT_UPDROW
                 - DT_ADDROW
                 - CD_MUNICIPIO
                 - FG_QUEBRA_PEDIDO
            */

            return empresasRequest;
        }

        protected virtual ProductosRequest MapProductos(string xml, int empresa, long numeroEjecucion, out CodigosBarrasRequest codigosBarrasRequest, out ProductosProveedorRequest productosProveedorRequest, out object extraData)
        {
            var productosRequest = new ProductosRequest();
            var productosExtraData = new ProductosExtraData();

            codigosBarrasRequest = new CodigosBarrasRequest();
            productosProveedorRequest = new ProductosProveedorRequest();
            extraData = productosExtraData;

            productosRequest.Empresa = empresa;
            productosRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            productosRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 1);

            codigosBarrasRequest.Empresa = empresa;
            codigosBarrasRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            codigosBarrasRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 2);

            productosProveedorRequest.Empresa = empresa;
            productosProveedorRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            productosProveedorRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 3);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList productos = xmlDoc.SelectNodes("/MAESTRO_PRODUCTO/PRODUCTOS/PRODUCTO");

            foreach (XmlNode producto in productos)
            {
                var productoRequest = new ProductoRequest();

                productoRequest.Codigo = producto.SelectSingleNode("CD_PRODUTO")?.InnerText;
                productoRequest.Descripcion = producto.SelectSingleNode("DS_PRODUTO")?.InnerText;
                productoRequest.UnidadDistribucion = ParsearValor<decimal>(producto.SelectSingleNode("QT_UND_DISTRIBUCION")?.InnerText);
                productoRequest.UnidadBulto = ParsearValor<decimal>(producto.SelectSingleNode("QT_UND_BULTO")?.InnerText);
                productoRequest.CodigoClase = producto.SelectSingleNode("CD_CLASSE")?.InnerText;
                productoRequest.Ramo = ParsearValor<short>(producto.SelectSingleNode("CD_RAMO_PRODUTO")?.InnerText);
                productoRequest.Situacion = ParsearValor<short>(producto.SelectSingleNode("CD_SITUACAO")?.InnerText);
                productoRequest.CodigoMercadologico = producto.SelectSingleNode("CD_MERCADOLOGICO")?.InnerText;
                productoRequest.ManejoIdentificador = producto.SelectSingleNode("ID_MANEJO_IDENTIFICADOR")?.InnerText;
                productoRequest.UnidadMedida = producto.SelectSingleNode("CD_UNIDADE_MEDIDA")?.InnerText;
                productoRequest.CodigoProductoEmpresa = producto.SelectSingleNode("CD_PRODUTO_EMPRESA")?.InnerText;
                productoRequest.GrupoConsulta = producto.SelectSingleNode("CD_GRUPO_CONSULTA")?.InnerText;
                productoRequest.CodigoFamilia = ParsearValor<int>(producto.SelectSingleNode("CD_FAMILIA_PRODUTO")?.InnerText);
                productoRequest.CodigoRotatividad = ParsearValor<short>(producto.SelectSingleNode("CD_ROTATIVIDADE")?.InnerText);
                productoRequest.StockMinimo = ParsearValor<int>(producto.SelectSingleNode("QT_ESTOQUE_MINIMO")?.InnerText);
                productoRequest.StockMaximo = ParsearValor<int>(producto.SelectSingleNode("QT_ESTOQUE_MAXIMO")?.InnerText);
                productoRequest.PesoNeto = ParsearValor<decimal>(producto.SelectSingleNode("PS_LIQUIDO")?.InnerText);
                productoRequest.PesoBruto = ParsearValor<decimal>(producto.SelectSingleNode("PS_BRUTO")?.InnerText);
                productoRequest.VolumenCC = ParsearValor<decimal>(producto.SelectSingleNode("VL_CUBAGEM")?.InnerText);
                productoRequest.PrecioVenta = ParsearValor<decimal>(producto.SelectSingleNode("VL_PRECO_VENDA")?.InnerText);
                productoRequest.UltimoCosto = ParsearValor<decimal>(producto.SelectSingleNode("VL_CUSTO_ULT_ENT")?.InnerText);
                productoRequest.UndEmb = producto.SelectSingleNode("CD_UNID_EMB")?.InnerText;
                productoRequest.DiasValidez = ParsearValor<short>(producto.SelectSingleNode("QT_DIAS_VALIDADE")?.InnerText);
                productoRequest.DiasDuracion = ParsearValor<short>(producto.SelectSingleNode("QT_DIAS_DURACAO")?.InnerText);
                productoRequest.AceptaDecimales = producto.SelectSingleNode("FL_ACEPTA_DECIMALES")?.InnerText;
                productoRequest.TipoManejoFecha = producto.SelectSingleNode("TP_MANEJO_FECHA")?.InnerText;
                productoRequest.NAM = producto.SelectSingleNode("CD_NAM")?.InnerText;
                productoRequest.DescripcionReducida = producto.SelectSingleNode("DS_REDUZIDA")?.InnerText;
                productoRequest.Agrupacion = producto.SelectSingleNode("ID_AGRUPACION")?.InnerText;
                productoRequest.Anexo1 = producto.SelectSingleNode("DS_ANEXO1")?.InnerText;
                productoRequest.Anexo2 = producto.SelectSingleNode("DS_ANEXO2")?.InnerText;
                productoRequest.Anexo3 = producto.SelectSingleNode("DS_ANEXO3")?.InnerText;
                productoRequest.Anexo4 = producto.SelectSingleNode("DS_ANEXO4")?.InnerText;
                productoRequest.Anexo5 = producto.SelectSingleNode("DS_ANEXO5")?.InnerText;
                productoRequest.Altura = ParsearValor<decimal>(producto.SelectSingleNode("VL_ALTURA")?.InnerText);
                productoRequest.Ancho = ParsearValor<decimal>(producto.SelectSingleNode("VL_LARGURA")?.InnerText);
                productoRequest.Profundidad = ParsearValor<decimal>(producto.SelectSingleNode("VL_PROFUNDIDADE")?.InnerText);
                productoRequest.AvisoAjusteInventario = ParsearValor<decimal>(producto.SelectSingleNode("VL_AVISO_AJUSTE")?.InnerText);
                productoRequest.DiasLiberacion = ParsearValor<short>(producto.SelectSingleNode("QT_DIAS_VALIDADE_LIBERACION")?.InnerText);

                var cdFamilia = productoRequest.CodigoFamilia;
                var dsFamilia = producto.SelectSingleNode("DS_FAMILIA_PRODUTO")?.InnerText; 
                if (cdFamilia.HasValue && !string.IsNullOrEmpty(dsFamilia))
                {
                    productosExtraData.Familias[cdFamilia.Value] = dsFamilia;
                }

                var cdRamo = productoRequest.Ramo;
                var dsRamo = producto.SelectSingleNode("DS_RAMO")?.InnerText;
                if (cdRamo.HasValue && !string.IsNullOrEmpty(dsRamo))
                {
                    productosExtraData.Ramos[cdRamo.Value] = dsRamo;
                }

                var cdClase = productoRequest.CodigoClase;
                var dsClase = producto.SelectSingleNode("DS_CLASSE")?.InnerText;
                if (!string.IsNullOrEmpty(cdClase) && !string.IsNullOrEmpty(dsClase))
                {
                    productosExtraData.Clases[cdClase] = dsClase;
                }

                XmlNodeList codigosDeBarra = producto.SelectNodes("BARCODES/BARCODE");

                if (codigosDeBarra != null)
                {
                    foreach (XmlNode codigoBarra in codigosDeBarra)
                    {
                        var codigoRequest = new CodigoBarraRequest();

                        codigoRequest.Producto = productoRequest.Codigo;
                        codigoRequest.Codigo = codigoBarra.SelectSingleNode("CD_BARRAS")?.InnerText;
                        codigoRequest.TipoCodigo = ParsearValor<int>(codigoBarra.SelectSingleNode("TP_CODIGO_BARRAS")?.InnerText);
                        codigoRequest.CantidadEmbalaje = ParsearValor<decimal>(codigoBarra.SelectSingleNode("QT_EMBALAGEM")?.InnerText);
                        codigoRequest.PrioridadUso = ParsearValor<short>(codigoBarra.SelectSingleNode("NU_PRIORIDADE_USO")?.InnerText);
                        codigoRequest.TipoOperacion = codigoBarra.SelectSingleNode("TP_OPERACION")?.InnerText;

                        codigosBarrasRequest.CodigosDeBarras.Add(codigoRequest);
                    }
                }

                XmlNodeList productosProveedor = producto.SelectNodes("CONVERTEDORES/CONVERTEDOR");

                if (productosProveedor != null)
                {
                    foreach (XmlNode productoProvedor in productosProveedor)
                    {
                        var productoProveedorRequest = new ProductoProveedorRequest();

                        productoProveedorRequest.CodigoProducto = productoRequest.Codigo;
                        productoProveedorRequest.CodigoExterno = productoProvedor.SelectSingleNode("CD_EXTERNO")?.InnerText;
                        productoProveedorRequest.CodigoAgente = productoProvedor.SelectSingleNode("CD_AGENTE")?.InnerText;
                        productoProveedorRequest.TipoAgente = productoProvedor.SelectSingleNode("TP_AGENTE")?.InnerText;
                        productoProveedorRequest.TipoOperacion = productoProvedor.SelectSingleNode("TP_OPERACION")?.InnerText;

                        productosProveedorRequest.Productos.Add(productoProveedorRequest);
                    }
                }

                productosRequest.Productos.Add(productoRequest);
            }

            /*
				 Campos 2.0 sin mapeo:
				 
				 - TP_CARGA
				 - DS_CLASSE
				 - DS_RAMO
				 - DS_FAMILIA_PRODUTO
				 - ID_MANEJA_TOMA_DATO
				 - ID_CROSS_DOCKING
				 - DT_ADDROW
            */

            return productosRequest;
        }

        protected virtual CodigosBarrasRequest MapCodigosDeBarra(string xml, int empresa, long numeroEjecucion, out object extraData)
        {
            CodigosBarrasRequest codigosBarrasRequest = new CodigosBarrasRequest();

            extraData = null;

            codigosBarrasRequest.Empresa = empresa;
            codigosBarrasRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            codigosBarrasRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 1);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList codigosDeBarra = xmlDoc.SelectNodes("/MAESTRO_BARCODE/BARCODES/BARCODE");

            foreach (XmlNode codigo in codigosDeBarra)
            {
                var codigoBarraRequest = new CodigoBarraRequest();

                codigoBarraRequest.Producto = codigo.SelectSingleNode("CD_PRODUTO")?.InnerText;
                codigoBarraRequest.Codigo = codigo.SelectSingleNode("CD_BARRAS")?.InnerText;
                codigoBarraRequest.TipoCodigo = ParsearValor<int>(codigo.SelectSingleNode("TP_CODIGO_BARRAS")?.InnerText);
                codigoBarraRequest.CantidadEmbalaje = ParsearValor<decimal>(codigo.SelectSingleNode("QT_EMBALAGEM")?.InnerText);
                codigoBarraRequest.PrioridadUso = ParsearValor<short>(codigo.SelectSingleNode("NU_PRIORIDADE_USO")?.InnerText);
                codigoBarraRequest.TipoOperacion = codigo.SelectSingleNode("TP_OPERACION")?.InnerText;

                codigosBarrasRequest.CodigosDeBarras.Add(codigoBarraRequest);
            }

            return codigosBarrasRequest;
        }

        protected virtual ProductosProveedorRequest MapProductosProvedor(string xml, int empresa, long numeroEjecucion, out object extraData)
        {
            ProductosProveedorRequest productosProveedorRequest = new ProductosProveedorRequest();

            extraData = null;

            productosProveedorRequest.Empresa = empresa;
            productosProveedorRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            productosProveedorRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 1);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList productosProveedor = xmlDoc.SelectNodes("/MAESTRO_CONVERTEDORES/CONVERTEDORES/CONVERTEDOR");

            foreach (XmlNode productoProveedor in productosProveedor)
            {
                var productoProveedorRequest = new ProductoProveedorRequest();

                productoProveedorRequest.CodigoProducto = productoProveedor.SelectSingleNode("CD_PRODUTO")?.InnerText;
                productoProveedorRequest.CodigoExterno = productoProveedor.SelectSingleNode("CD_EXTERNO")?.InnerText;
                productoProveedorRequest.CodigoAgente = productoProveedor.SelectSingleNode("CD_AGENTE")?.InnerText;
                productoProveedorRequest.TipoAgente = productoProveedor.SelectSingleNode("TP_AGENTE")?.InnerText;
                productoProveedorRequest.TipoOperacion = productoProveedor.SelectSingleNode("TP_OPERACION")?.InnerText;

                productosProveedorRequest.Productos.Add(productoProveedorRequest);
            }

            return productosProveedorRequest;
        }

        protected virtual PedidosRequest MapPedidos(string xml, int empresa, long numeroEjecucion, out object extraData)
        {
            PedidosRequest pedidosRequest = new PedidosRequest();

            extraData = null;

            pedidosRequest.Empresa = empresa;
            pedidosRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            pedidosRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 1);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList pedidos = xmlDoc.SelectNodes("/PEDIDOS/PEDIDO");

            foreach (XmlNode pedido in pedidos)
            {
                var pedidoRequest = new PedidoRequest();

                pedidoRequest.NroPedido = pedido.SelectSingleNode("NU_PEDIDO")?.InnerText;
                pedidoRequest.CodigoAgente = pedido.SelectSingleNode("CD_CLIENTE")?.InnerText;
                pedidoRequest.TipoAgente = pedido.SelectSingleNode("TP_AGENTE")?.InnerText;
                pedidoRequest.CondicionLiberacion = pedido.SelectSingleNode("CD_CONDICION_LIBERACION")?.InnerText;
                pedidoRequest.PuntoEntrega = pedido.SelectSingleNode("CD_PUNTO_ENTREGA")?.InnerText;
                pedidoRequest.Ruta = ParsearValor<short>(pedido.SelectSingleNode("CD_ROTA")?.InnerText);
                pedidoRequest.CodigoTransportadora = ParsearValor<int>(pedido.SelectSingleNode("CD_TRANSPORTADORA")?.InnerText);
                pedidoRequest.Zona = pedido.SelectSingleNode("CD_ZONA")?.InnerText;
                pedidoRequest.Anexo1 = pedido.SelectSingleNode("DS_ANEXO1")?.InnerText;
                pedidoRequest.Anexo2 = pedido.SelectSingleNode("DS_ANEXO2")?.InnerText;
                pedidoRequest.Anexo3 = pedido.SelectSingleNode("DS_ANEXO3")?.InnerText;
                pedidoRequest.Anexo4 = pedido.SelectSingleNode("DS_ANEXO4")?.InnerText;
                pedidoRequest.Direccion = pedido.SelectSingleNode("DS_ENDERECO")?.InnerText;
                pedidoRequest.Memo = pedido.SelectSingleNode("DS_MEMO")?.InnerText;
                pedidoRequest.Memo1 = pedido.SelectSingleNode("DS_MEMO_1")?.InnerText;
                pedidoRequest.FechaEmision = ParsearValor<DateTime>(pedido.SelectSingleNode("DT_EMITIDO")?.InnerText);
                pedidoRequest.FechaEntrega = ParsearValor<DateTime>(pedido.SelectSingleNode("DT_ENTREGA")?.InnerText);
                pedidoRequest.FechaLiberarDesde = ParsearValor<DateTime>(pedido.SelectSingleNode("DT_LIBERAR_DESDE")?.InnerText);
                pedidoRequest.FechaLiberarHasta = ParsearValor<DateTime>(pedido.SelectSingleNode("DT_LIBERAR_HASTA")?.InnerText);
                pedidoRequest.FechaGenerica = ParsearValor<DateTime>(pedido.SelectSingleNode("DT_GENERICO_1")?.InnerText);
                pedidoRequest.Agrupacion = pedido.SelectSingleNode("ID_AGRUPACION")?.InnerText;
                pedidoRequest.NuGenerico = ParsearValor<decimal>(pedido.SelectSingleNode("NU_GENERICO_1")?.InnerText);
                pedidoRequest.OrdenEntrega = ParsearValor<int>(pedido.SelectSingleNode("NU_ORDEN_ENTREGA")?.InnerText);
                pedidoRequest.Predio = pedido.SelectSingleNode("NU_PREDIO")?.InnerText;
                pedidoRequest.TipoExpedicion = pedido.SelectSingleNode("TP_EXPEDICION")?.InnerText;
                pedidoRequest.TipoPedido = pedido.SelectSingleNode("TP_PEDIDO")?.InnerText;
                pedidoRequest.ComparteContenedorEntrega = pedido.SelectSingleNode("VL_COMPARTE_CONTENEDOR_ENTREGA")?.InnerText;
                pedidoRequest.ComparteContenedorPicking = pedido.SelectSingleNode("VL_COMPARTE_CONTENEDOR_PICKING")?.InnerText;
                pedidoRequest.DsGenerico = pedido.SelectSingleNode("VL_GENERICO_1")?.InnerText;
                pedidoRequest.Serializado = pedido.SelectSingleNode("VL_SERIALIZADO_1")?.InnerText;
                pedidoRequest.Telefono = pedido.SelectSingleNode("NU_TELEFONE")?.InnerText;
                pedidoRequest.TelefonoSecundario = pedido.SelectSingleNode("NU_TELEFONE2")?.InnerText;
                pedidoRequest.Longitud = ParsearValor<decimal>(pedido.SelectSingleNode("VL_LONGITUD")?.InnerText);
                pedidoRequest.Latitud = ParsearValor<decimal>(pedido.SelectSingleNode("VL_LATITUD")?.InnerText);

                XmlNodeList detallesPedidos = pedido.SelectNodes("DETALLE/LINEA_PEDIDO");

                foreach (XmlNode detalle in detallesPedidos)
                {
                    var detallePedido = new DetallePedidoRequest();

                    detallePedido.CodigoProducto = detalle.SelectSingleNode("CD_PRODUTO")?.InnerText;
                    detallePedido.Identificador = detalle.SelectSingleNode("NU_IDENTIFICADOR")?.InnerText;
                    detallePedido.Cantidad = ParsearValorNoNull<decimal>(detalle.SelectSingleNode("QT_PEDIDO")?.InnerText);
                    detallePedido.Memo = detalle.SelectSingleNode("DS_MEMO")?.InnerText;
                    detallePedido.FechaGenerica = ParsearValor<DateTime>(detalle.SelectSingleNode("DT_GENERICO_1")?.InnerText);
                    detallePedido.NuGenerico = ParsearValor<decimal>(detalle.SelectSingleNode("NU_GENERICO_1")?.InnerText);
                    detallePedido.DsGenerico = detalle.SelectSingleNode("VL_GENERICO_1")?.InnerText;
                    detallePedido.Serializado = detalle.SelectSingleNode("VL_SERIALIZADO_1")?.InnerText;

                    pedidoRequest.Detalles.Add(detallePedido);
                }

                pedidosRequest.Pedidos.Add(pedidoRequest);
            }

            /*
				 Campos 2.0 sin mapeo:
				 
				Cabezal

				 - ID_MODO_PEDIDO_NRO

				Detalle 
				 - ID_AGRUPACION
            */

            return pedidosRequest;
        }

        protected virtual AgentesRequest MapAgentes(string xml, int empresa, long numeroEjecucion, out object extraData)
        {
            AgentesRequest agentesRequest = new AgentesRequest();

            extraData = null;

            agentesRequest.Empresa = empresa;
            agentesRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            agentesRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 1);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList agentes = xmlDoc.SelectNodes("/AGENTES/AGENTE");

            foreach (XmlNode agente in agentes)
            {
                var agenteRequest = new AgenteRequest();

                agenteRequest.CodigoAgente = agente.SelectSingleNode("CD_AGENTE")?.InnerText;
                agenteRequest.Tipo = agente.SelectSingleNode("TP_AGENTE")?.InnerText;
                agenteRequest.Ruta = ParsearValor<short>(agente.SelectSingleNode("CD_ROTA")?.InnerText);
                agenteRequest.Direccion = agente.SelectSingleNode("DS_ENDERECO")?.InnerText;
                agenteRequest.Barrio = agente.SelectSingleNode("DS_BAIRRO")?.InnerText;
                agenteRequest.CodigoPostal = agente.SelectSingleNode("CD_CEP")?.InnerText;
                agenteRequest.TelefonoPrincipal = agente.SelectSingleNode("NU_TELEFONE")?.InnerText;
                agenteRequest.TelefonoSecundario = agente.SelectSingleNode("NU_FAX")?.InnerText;
                agenteRequest.OtroDatoFiscal = agente.SelectSingleNode("NU_INSCRICAO")?.InnerText;
                agenteRequest.NumeroFiscal = agente.SelectSingleNode("CD_CGC_CLIENTE")?.InnerText;
                agenteRequest.Estado = ParsearValor<short>(agente.SelectSingleNode("CD_SITUACAO")?.InnerText);
                agenteRequest.Anexo1 = agente.SelectSingleNode("DS_ANEXO1")?.InnerText;
                agenteRequest.Anexo2 = agente.SelectSingleNode("DS_ANEXO2")?.InnerText;
                agenteRequest.Anexo3 = agente.SelectSingleNode("DS_ANEXO3")?.InnerText;
                agenteRequest.Anexo4 = agente.SelectSingleNode("DS_ANEXO4")?.InnerText;
                agenteRequest.Categoria = agente.SelectSingleNode("CD_CATEGORIA")?.InnerText;
                agenteRequest.Descripcion = agente.SelectSingleNode("DS_AGENTE")?.InnerText;

                agentesRequest.Agentes.Add(agenteRequest);
            }

            /*
				 Campos 2.0 sin mapeo:
				 
				 - CD_LUGAR

            */

            return agentesRequest;
        }

        protected virtual ReferenciasRecepcionRequest MapReferenciaRecepcion(string xml, int empresa, long numeroEjecucion, out object extraData)
        {
            ReferenciasRecepcionRequest referenciasRecepcionRequest = new ReferenciasRecepcionRequest();

            extraData = null;

            referenciasRecepcionRequest.Empresa = empresa;
            referenciasRecepcionRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            referenciasRecepcionRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 1);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList referencias = xmlDoc.SelectNodes("/REFERENCIAS_RECEPCION/REFERENCIAS/REFERENCIA");

            foreach (XmlNode referencia in referencias)
            {
                var referenciaRecepcionRequest = new ReferenciaRecepcionRequest();

                referenciaRecepcionRequest.Referencia = referencia.SelectSingleNode("NU_REFERENCIA")?.InnerText;
                referenciaRecepcionRequest.TipoReferencia = referencia.SelectSingleNode("TP_REFERENCIA")?.InnerText;
                referenciaRecepcionRequest.TipoAgente = referencia.SelectSingleNode("TP_AGENTE")?.InnerText;
                referenciaRecepcionRequest.CodigoAgente = referencia.SelectSingleNode("CD_AGENTE")?.InnerText;
                referenciaRecepcionRequest.FechaEntrega = ParsearValor<DateTime>(referencia.SelectSingleNode("DT_ENTREGA")?.InnerText);
                referenciaRecepcionRequest.FechaVencimientoOrden = ParsearValor<DateTime>(referencia.SelectSingleNode("DT_VENCIMIENTO_ORDEN")?.InnerText);
                referenciaRecepcionRequest.FechaEmitida = ParsearValor<DateTime>(referencia.SelectSingleNode("DT_EMITIDA")?.InnerText);
                referenciaRecepcionRequest.Memo = referencia.SelectSingleNode("DS_MEMO")?.InnerText;
                referenciaRecepcionRequest.Predio = referencia.SelectSingleNode("NU_PREDIO")?.InnerText;
                referenciaRecepcionRequest.Serializado = referencia.SelectSingleNode("VL_SERIALIZADO")?.InnerText;

                XmlNodeList detalles = referencia.SelectNodes("REFERENCIA_DETALLES/REFERENCIA_DETALLE");

                foreach (XmlNode detalle in detalles)
                {
                    var detallereferenciaRecepcionRequest = new ReferenciaRecepcionDetalleRequest();

                    detallereferenciaRecepcionRequest.IdLineaSistemaExterno = detalle.SelectSingleNode("ID_LINEA_SISTEMA_EXTERNO")?.InnerText;
                    detallereferenciaRecepcionRequest.CodigoProducto = detalle.SelectSingleNode("CD_PRODUTO")?.InnerText;
                    detallereferenciaRecepcionRequest.Identificador = detalle.SelectSingleNode("NU_IDENTIFICADOR")?.InnerText;
                    detallereferenciaRecepcionRequest.CantidadReferencia = ParsearValorNoNull<decimal>(detalle.SelectSingleNode("QT_REFERENCIA")?.InnerText);
                    detallereferenciaRecepcionRequest.FechaVencimiento = ParsearValor<DateTime>(detalle.SelectSingleNode("DT_VENCIMIENTO")?.InnerText);
                    detallereferenciaRecepcionRequest.Anexo1 = detalle.SelectSingleNode("DS_ANEXO")?.InnerText;

                    referenciaRecepcionRequest.Detalles.Add(detallereferenciaRecepcionRequest);
                }

                referenciasRecepcionRequest.Referencias.Add(referenciaRecepcionRequest);
            }

            /*
				 Campos 2.0 sin mapeo:
				 
				 - FL_AUTO_AGENDABLE
				 - TP_RECEPCION_EXTERNO

            */

            return referenciasRecepcionRequest;
        }

        protected virtual AnularReferenciasRequest MapAnulacionReferenciaRecepcion(string xml, int empresa, long numeroEjecucion, out object extraData)
        {
            AnularReferenciasRequest anularReferenciasRequest = new AnularReferenciasRequest();

            extraData = null;

            anularReferenciasRequest.Empresa = empresa;
            anularReferenciasRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            anularReferenciasRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 1);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList anulacionesReferencias = xmlDoc.SelectNodes("/ANULACION_REFERENCIAS/REFERENCIA_ENTRADA");

            foreach (XmlNode anulacion in anulacionesReferencias)
            {
                var anulacionReferenciaRequest = new AnularReferenciaRequest();

                anulacionReferenciaRequest.Referencia = anulacion.SelectSingleNode("NU_REFERENCIA")?.InnerText;
                anulacionReferenciaRequest.TipoReferencia = anulacion.SelectSingleNode("TP_REFERENCIA")?.InnerText;
                anulacionReferenciaRequest.CodigoAgente = anulacion.SelectSingleNode("CD_AGENTE")?.InnerText;
                anulacionReferenciaRequest.TipoAgente = anulacion.SelectSingleNode("TP_AGENTE")?.InnerText;

                anularReferenciasRequest.Referencias.Add(anulacionReferenciaRequest);
            }

            return anularReferenciasRequest;
        }

        protected virtual ModificacionDetalleReferenciaRequest MapModificarDetalleReferenciaRecepcion(string xml, int empresa, long numeroEjecucion, out object extraData)
        {
            ModificacionDetalleReferenciaRequest modificacionReferenciasRequest = new ModificacionDetalleReferenciaRequest();

            extraData = null;

            modificacionReferenciasRequest.Empresa = empresa;
            modificacionReferenciasRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            modificacionReferenciasRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 1);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList modificacionesReferencias = xmlDoc.SelectNodes("/MODIFICAR_DETALLE_REFERENCIAS/REFERENCIAS/ANUL_REFERENCIA");

            foreach (XmlNode modificacion in modificacionesReferencias)
            {
                var modificacionReferenciaRequest = new ReferenciaModificacionRequest();

                modificacionReferenciaRequest.Referencia = modificacion.SelectSingleNode("NU_REFERENCIA")?.InnerText;
                modificacionReferenciaRequest.TipoReferencia = modificacion.SelectSingleNode("TP_REFERENCIA")?.InnerText;
                modificacionReferenciaRequest.CodigoAgente = modificacion.SelectSingleNode("CD_AGENTE")?.InnerText;
                modificacionReferenciaRequest.TipoAgente = modificacion.SelectSingleNode("TP_AGENTE")?.InnerText;

                XmlNodeList detallesModificacionesReferencias = modificacion.SelectNodes("ANUL_REFERENCIA_DETALLES/ANUL_REFERENCIA_DETALLE");

                foreach (XmlNode detalle in detallesModificacionesReferencias)
                {
                    var detalleModificacionReferenciaRequest = new DetalleModificacionRequest();

                    var entradaTipoOperacion = detalle.SelectSingleNode("TP_OPERACION")?.InnerText;
                    string tipoOperacion = "";

                    switch (entradaTipoOperacion)
                    {
                        case "A":
                            tipoOperacion = "A";
                            break;
                        case "D":
                            tipoOperacion = "M";
                            break;
                        case "U":
                            tipoOperacion = "R";
                            break;
                        default:
                            throw new InvalidMappingException("TP_OPERACION");
                    }

                    detalleModificacionReferenciaRequest.TipoOperacion = tipoOperacion;
                    detalleModificacionReferenciaRequest.IdLineaSistemaExterno = detalle.SelectSingleNode("ID_LINEA_SISTEMA_EXTERNO")?.InnerText;
                    detalleModificacionReferenciaRequest.CodigoProducto = detalle.SelectSingleNode("CD_PRODUTO")?.InnerText;
                    detalleModificacionReferenciaRequest.Identificador = detalle.SelectSingleNode("NU_IDENTIFICADOR")?.InnerText;
                    detalleModificacionReferenciaRequest.CantidadOperacion = ParsearValorNoNull<decimal>(detalle.SelectSingleNode("QT_REFERENCIA")?.InnerText);

                    modificacionReferenciaRequest.Detalles.Add(detalleModificacionReferenciaRequest);
                }

                modificacionReferenciasRequest.Referencias.Add(modificacionReferenciaRequest);
            }

            return modificacionReferenciasRequest;
        }

        protected virtual ModificarPedidosRequest MapModificarPedido(string xml, int empresa, long numeroEjecucion, out object extraData)
        {
            ModificarPedidosRequest modificacionPedidoRequest = new ModificarPedidosRequest();

            extraData = null;

            modificacionPedidoRequest.Empresa = empresa;
            modificacionPedidoRequest.DsReferencia = $"Interfaz XML: {numeroEjecucion}";
            modificacionPedidoRequest.IdRequest = string.Format(WISIE_ID_REQUEST_FORMAT, numeroEjecucion, 1);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList modificacionesPedidos = xmlDoc.SelectNodes("/MODIFICACION_PEDIDOS/PEDIDOS/PEDIDO");

            foreach (XmlNode modificacion in modificacionesPedidos)
            {
                var modificacionPedido = new ModificarPedidoRequest();

                modificacionPedido.NroPedido = modificacion.SelectSingleNode("NU_PEDIDO")?.InnerText;
				modificacionPedido.CodigoAgente = modificacion.SelectSingleNode("CD_CLIENTE")?.InnerText;
				modificacionPedido.TipoAgente = modificacion.SelectSingleNode("TP_AGENTE")?.InnerText;
                modificacionPedido.CondicionLiberacion = modificacion.SelectSingleNode("CD_CONDICION_LIBERACION")?.InnerText;
                modificacionPedido.PuntoEntrega = modificacion.SelectSingleNode("CD_PUNTO_ENTREGA")?.InnerText;
                modificacionPedido.Ruta = ParsearValor<short>(modificacion.SelectSingleNode("CD_ROTA")?.InnerText);
                modificacionPedido.CodigoTransportadora = ParsearValor<int>(modificacion.SelectSingleNode("CD_TRANSPORTADORA")?.InnerText);
                modificacionPedido.Zona = modificacion.SelectSingleNode("DS_ZONA")?.InnerText;
                modificacionPedido.Anexo1 = modificacion.SelectSingleNode("DS_ANEXO1")?.InnerText;
                modificacionPedido.Anexo2 = modificacion.SelectSingleNode("DS_ANEXO2")?.InnerText;
                modificacionPedido.Anexo3 = modificacion.SelectSingleNode("DS_ANEXO3")?.InnerText;
                modificacionPedido.Anexo4 = modificacion.SelectSingleNode("DS_ANEXO4")?.InnerText;
                modificacionPedido.Direccion = modificacion.SelectSingleNode("DS_ENDERECO")?.InnerText;
                modificacionPedido.Memo = modificacion.SelectSingleNode("DS_MEMO")?.InnerText;
                modificacionPedido.Memo1 = modificacion.SelectSingleNode("DS_MEMO_1")?.InnerText;
                modificacionPedido.FechaEmision = ParsearValor<DateTime>(modificacion.SelectSingleNode("DT_EMITIDO")?.InnerText);
                modificacionPedido.FechaEntrega = ParsearValor<DateTime>(modificacion.SelectSingleNode("DT_ENTREGA")?.InnerText);
                modificacionPedido.FechaLiberarDesde = ParsearValor<DateTime>(modificacion.SelectSingleNode("DT_LIBERAR_DESDE")?.InnerText);
                modificacionPedido.FechaLiberarHasta = ParsearValor<DateTime>(modificacion.SelectSingleNode("DT_LIBERAR_HASTA")?.InnerText);
                modificacionPedido.FechaGenerica = ParsearValor<DateTime>(modificacion.SelectSingleNode("DT_GENERICO_1")?.InnerText);
                modificacionPedido.Agrupacion = modificacion.SelectSingleNode("ID_AGRUPACION")?.InnerText;
                modificacionPedido.NuGenerico = ParsearValor<decimal>(modificacion.SelectSingleNode("NU_GENERICO_1")?.InnerText);
                modificacionPedido.OrdenEntrega = ParsearValor<int>(modificacion.SelectSingleNode("NU_ORDEN_ENTREGA")?.InnerText);
                modificacionPedido.Predio = modificacion.SelectSingleNode("NU_PREDIO")?.InnerText;
                modificacionPedido.TipoExpedicion = modificacion.SelectSingleNode("TP_EXPEDICION")?.InnerText;
                modificacionPedido.TipoPedido = modificacion.SelectSingleNode("TP_PEDIDO")?.InnerText;
                modificacionPedido.ComparteContenedorEntrega = modificacion.SelectSingleNode("VL_COMPARTE_CONTENEDOR_ENTREGA")?.InnerText;
                modificacionPedido.ComparteContenedorPicking = modificacion.SelectSingleNode("VL_COMPARTE_CONTENEDOR_PICKING")?.InnerText;
                modificacionPedido.DsGenerico = modificacion.SelectSingleNode("VL_GENERICO_1")?.InnerText;
                modificacionPedido.Serializado = modificacion.SelectSingleNode("VL_SERIALIZADO_1")?.InnerText;
                modificacionPedido.Telefono = modificacion.SelectSingleNode("NU_TELEFONE")?.InnerText;
                modificacionPedido.TelefonoSecundario = modificacion.SelectSingleNode("NU_TELEFONE2")?.InnerText;
                modificacionPedido.Longitud = ParsearValor<decimal>(modificacion.SelectSingleNode("VL_LONGITUD")?.InnerText);
                modificacionPedido.Latitud = ParsearValor<decimal>(modificacion.SelectSingleNode("VL_LATITUD")?.InnerText);

                XmlNodeList detallesPedidos = modificacion.SelectNodes("DETALLES/DETALLE_PEDIDO");

                foreach (XmlNode detalle in detallesPedidos)
                {
                    var detallePedido = new ModificarDetallePedidoRequest();

                    detallePedido.CodigoProducto = detalle.SelectSingleNode("CD_PRODUTO")?.InnerText;
                    detallePedido.Identificador = detalle.SelectSingleNode("NU_IDENTIFICADOR")?.InnerText;
                    detallePedido.Cantidad = ParsearValorNoNull<decimal>(detalle.SelectSingleNode("QT_PEDIDO")?.InnerText);
                    detallePedido.Memo = detalle.SelectSingleNode("DS_MEMO")?.InnerText;
                    detallePedido.FechaGenerica = ParsearValor<DateTime>(detalle.SelectSingleNode("DT_GENERICO_1")?.InnerText);
                    detallePedido.NuGenerico = ParsearValor<decimal>(detalle.SelectSingleNode("NU_GENERICO_1")?.InnerText);
                    detallePedido.DsGenerico = detalle.SelectSingleNode("VL_GENERICO_1")?.InnerText;
                    detallePedido.Serializado = detalle.SelectSingleNode("VL_SERIALIZADO_1")?.InnerText;

                    modificacionPedido.Detalles.Add(detallePedido);
                }

                modificacionPedidoRequest.Pedidos.Add(modificacionPedido);
            }

            return modificacionPedidoRequest;
        }

        #endregion

        #region Utils

        protected virtual T? ParsearValor<T>(string value) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                return null;

            Type type = typeof(T);

            if (type == typeof(decimal))
            {
                if (decimal.TryParse(value, CultureInfo.InvariantCulture, out decimal decimalResult))
                    return (T)(object)decimalResult;
            }
            else if (type == typeof(long))
            {
                if (long.TryParse(value, out long longResult))
                    return (T)(object)longResult;
            }
            else if (type == typeof(int))
            {
                if (int.TryParse(value, out int intResult))
                    return (T)(object)intResult;
            }
            else if (type == typeof(short))
            {
                if (short.TryParse(value, out short shortResult))
                    return (T)(object)shortResult;
            }
            else if (type == typeof(DateTime))
            {
                string[] formatosValidos = {
                    "dd/MM/yyyy HH:mm:ss",
                    "dd/MM/yyyy H:mm:ss",
                    "dd/MM/yyyy HH:mm",
                    "dd/MM/yyyy"
                };

                if (DateTime.TryParseExact(value, formatosValidos, null, System.Globalization.DateTimeStyles.None, out DateTime dateResult))
                    return (T)(object)dateResult;
            }

            return null;
        }

        protected virtual T ParsearValorNoNull<T>(string value) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                throw new NullReferenceException();

            return ParsearValor<T>(value).Value;
        }

        protected virtual void Add(int interfazExterna, IApiEntradaRequest payload, List<ApiRequest> requests)
        {
            if (payload == null)
                return;

            requests.Add(new ApiRequest
            {
                InterfazExterna = interfazExterna,
                Empresa = payload.Empresa,
                IdRequest = payload.IdRequest,
                Payload = payload
            });
        }

        #endregion
    }
}
