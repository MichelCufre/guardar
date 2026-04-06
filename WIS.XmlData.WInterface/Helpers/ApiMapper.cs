
using System.Globalization;
using System.Xml;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Salida;
using static WIS.Domain.General.API.Dtos.Salida.StockRequest;
using Irony.Parsing;

namespace WIS.XmlData.WInterface.Helpers
{
    public class ApiMapper
    {
        public virtual List<StockRequest> MapConsultaStockRequests(int empresa, string xml)
        {
            var requests = new List<StockRequest>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNodeList filtros = xmlDoc.SelectNodes("/CONSULTA_STOCK/STOCK");

            foreach (XmlNode filtro in filtros)
            {
                var request = new StockRequest
                {
                    Empresa = empresa,
                    Pagina = 1,
                    Filtros = new FiltrosStockRequest()
                };

                var averia = filtro.SelectSingleNode("FL_AVERIA")?.InnerText ?? "N";

                request.Filtros.Averia = (averia.ToUpper() == "S");
                request.Filtros.CodigoClase = filtro.SelectSingleNode("CD_CLASSE")?.InnerText;
                request.Filtros.CodigoFamilia = ParsearValor<int>(filtro.SelectSingleNode("CD_FAMILIA_PRODUTO")?.InnerText);
                request.Filtros.GrupoConsulta = filtro.SelectSingleNode("CD_GRUPO_CONSULTA")?.InnerText;
                request.Filtros.ManejoIdentificador = filtro.SelectSingleNode("ID_MANEJO_IDENTIFICADOR")?.InnerText;
                request.Filtros.Predio = filtro.SelectSingleNode("NU_PREDIO")?.InnerText;
                request.Filtros.Producto = filtro.SelectSingleNode("CD_PRODUTO")?.InnerText;
                request.Filtros.Ramo = ParsearValor<short>(filtro.SelectSingleNode("CD_RAMO_PRODUTO")?.InnerText);
                request.Filtros.Ubicacion = filtro.SelectSingleNode("CD_ENDERECO")?.InnerText;

                /*
                    Campos 2.0 sin mapeo:
                    
                    - CD_MERCADOLOGICO
                    - CD_NAM
                    - CD_ORIGEM
                    - ID_AGRUPACION
                 */

                requests.Add(request);
            }

            return requests;
        }

        public virtual string MapConsultaStockResponse(ConsultaStockResponse response)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlElement root = xmlDoc.CreateElement("CONSULTA_STOCK");
            xmlDoc.AppendChild(root);

            foreach (var stock in response.Stock)
            {
                XmlElement stockElement = xmlDoc.CreateElement("STOCK");

                stockElement.AppendChild(CreateElement(xmlDoc, "CD_PRODUTO", stock.Producto));
                stockElement.AppendChild(CreateElement(xmlDoc, "STOCK_DISPONIBLE", stock.StockDisponible.ToString(CultureInfo.InvariantCulture)));
                stockElement.AppendChild(CreateElement(xmlDoc, "STOCK_GENERAL", stock.StockGeneral.ToString(CultureInfo.InvariantCulture)));

                root.AppendChild(stockElement);
            }

            return xmlDoc.OuterXml;
        }

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

        protected virtual XmlElement CreateElement(XmlDocument doc, string name, string value)
        {
            XmlElement element = doc.CreateElement(name);

            element.InnerText = value;

            return element;
        }
    }
}
