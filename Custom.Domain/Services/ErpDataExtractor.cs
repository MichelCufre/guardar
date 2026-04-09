using Custom.Domain.DataModel.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Custom.Domain.Services.Configuration;
using WIS.Domain.General.API.Dtos.Entrada;

namespace Custom.Domain.Services
{
    /// <summary>
    /// ObtenerXml*()  — Fase 1: consulta WsGenQuery y devuelve el XML crudo del cliente para encolar.
    /// ToWms*()       — Fase 2: traduce el XML crudo de la cola al JSON que espera la WMS API.
    /// Los nombres de elementos XML son los que devuelve el WsGenQuery del cliente — ajustar segun cada implementacion.
    /// </summary>
    public class ErpDataExtractor
    {
        private readonly WsGenQueryClient _wsClient;
        private readonly IOptions<ErpClientSettings> _settings;
        private readonly ILogger<ErpDataExtractor> _logger;

        // Claves del WsGenQuery — ajustar segun el cliente
        private const string KeyProductos = "ITEMS";
        private const string KeyAgentes = "AGENTES";
        private const string KeyCodigoBarras = "CODIGOS_BARRA";

        public ErpDataExtractor(
            WsGenQueryClient wsClient,
            IOptions<ErpClientSettings> settings,
            ILogger<ErpDataExtractor> logger)
        {
            _wsClient = wsClient;
            _settings = settings;
            _logger   = logger;
        }

        // FASE 1 — Consultar WsGenQuery y devolver XML crudo para encolar
       
        public string ObtenerXmlProductos()
        {
            _logger.LogInformation("Consultando productos al WsGenQuery");
            var xml = _wsClient.GetData(KeyProductos);
            _logger.LogInformation("{Count} productos obtenidos del ERP", xml.Descendants("Table").Count());
            return xml.ToString();
        }

        public string ObtenerXmlAgentes()
        {
            _logger.LogInformation("Consultando agentes al WsGenQuery");
            var xml = _wsClient.GetData(KeyAgentes);
            _logger.LogInformation("{Count} agentes obtenidos del ERP", xml.Descendants("Table").Count());
            return xml.ToString();
        }

        public string ObtenerXmlCodigosBarras()
        {
            _logger.LogInformation("Consultando codigos de barras al WsGenQuery");
            var xml = _wsClient.GetData(KeyCodigoBarras);
            _logger.LogInformation("{Count} codigos de barras obtenidos del ERP", xml.Descendants("Table").Count());
            return xml.ToString();
        }

        // FASE 2 — Traducir XML crudo de la cola al JSON que espera la WMS API

        public string ToWmsProductos(string xmlPayload)
        {
            var xml = XDocument.Parse(xmlPayload);
            var productos = xml.Descendants("Table").Select(t => new ProductoRequest
            {
                Codigo = (string)t.Element("articulo"),
                Descripcion = (string)t.Element("descripcion"),
                UnidadMedida = (string)t.Element("unidad"),
                TipoManejoFecha = (string)t.Element("tipo_fecha"),
                Situacion = ParseShort(t.Element("situacion")),
                ManejoIdentificador = (string)t.Element("manejo_lote"),
                CodigoProductoEmpresa = (string)t.Element("cod_empresa"),
                DescripcionReducida = (string)t.Element("desc_corta"),
            }).ToList();

            return JsonConvert.SerializeObject(new ProductosRequest
            {
                Empresa = _settings.Value.Empresa,
                DsReferencia = _settings.Value.Referencia ?? "Middleware - Productos",
                Productos = productos
            });
        }

        public string ToWmsAgentes(string xmlPayload)
        {
            var xml = XDocument.Parse(xmlPayload);
            var agentes = xml.Descendants("Table").Select(t => new AgenteRequest
            {
                CodigoAgente = (string)t.Element("codigo"),
                Tipo = (string)t.Element("tipo_cliente"),
                Descripcion = (string)t.Element("nombre"),
                Estado = ParseShort(t.Element("estado")),
                Ruta = ParseShort(t.Element("ruta")),
                Direccion = (string)t.Element("direccion"),
                TelefonoPrincipal = (string)t.Element("telefono"),
                Email = (string)t.Element("email"),
            }).ToList();

            return JsonConvert.SerializeObject(new AgentesRequest
            {
                Empresa = _settings.Value.Empresa,
                DsReferencia = _settings.Value.Referencia ?? "Middleware - Agentes",
                Agentes = agentes
            });
        }

        public string ToWmsCodigosBarras(string xmlPayload)
        {
            var xml = XDocument.Parse(xmlPayload);
            var codigos = xml.Descendants("Table").Select(t => new CodigoBarraRequest
            {
                Codigo = (string)t.Element("codigo_barra"),
                Producto = (string)t.Element("articulo"),
                TipoCodigo = ParseInt(t.Element("tipo_codigo")),
                PrioridadUso = ParseShort(t.Element("prioridad")),
                CantidadEmbalaje = ParseDecimal(t.Element("cantidad")),
                TipoOperacion = (string)t.Element("operacion"),
            }).ToList();

            return JsonConvert.SerializeObject(new CodigosBarrasRequest
            {
                Empresa = _settings.Value.Empresa,
                DsReferencia = _settings.Value.Referencia ?? "Middleware - Codigos de Barras",
                CodigosDeBarras = codigos
            });
        }

        // ─── Helpers

        private static short? ParseShort(XElement el)
            => el != null && short.TryParse((string)el, out var v) ? v : (short?)null;

        private static int? ParseInt(XElement el)
            => el != null && int.TryParse((string)el, out var v) ? v : (int?)null;

        private static decimal? ParseDecimal(XElement el)
            => el != null && decimal.TryParse((string)el, out var v) ? v : (decimal?)null;
    }
}
