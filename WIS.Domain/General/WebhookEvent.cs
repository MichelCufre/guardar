using Humanizer;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API;
using WIS.Domain.General.API.Dtos.Salida;

namespace WIS.Domain.General
{
    public class SkipPropertyAttribute : Attribute
    {
    }

    public static class TypeExtensions
    {
        public static PropertyInfo[] GetFilteredProperties(this Type type)
        {
            return type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(SkipPropertyAttribute), true).Length == 0).ToArray();
        }
    }

    public class WebhookEvent
    {
        public string Id { get; set; }

        [SkipProperty]
        public long NumeroInterfazEjecucion { get; set; }

        public ConfirmacionRecepcionResponse ConfirmacionRecepcion { get; set; }

        public ConfirmacionPedidoResponse ConfirmacionPedido { get; set; }

        public PedidosAnuladosResponse PedidosAnulados { get; set; }

        public AjustesResponse Ajustes { get; set; }

        public ConsultaStockResponse ConsultaStock { get; set; }

        public FacturacionResponse ConfirmacionMercaderiaPreparada { get; set; }

        public AlmacenamientoResponse Almacenamiento { get; set; }

        public ConfirmacionProduccionResponse ConfirmacionProduccion { get; set; }

        public TestResponse Test { get; set; }

        public WebhookEvent(long numeroInterfazEjecucion)
        {
            NumeroInterfazEjecucion = numeroInterfazEjecucion;
        }

        public static WebhookEvent GetNewInstance(string jsonContent, bool camelCaseEnabled)
        {
            return GetNewInstance(-1, -1, jsonContent, camelCaseEnabled);
        }

        public static WebhookEvent GetNewInstance(long nroInterfazEjecucion, int interfazExterna, string jsonContent, bool camelCaseEnabled)
        {
            WebhookEvent e = new WebhookEvent(nroInterfazEjecucion);

            switch (interfazExterna)
            {
                case CInterfazExterna.ConfirmacionDeRecepcion:
                    e.ConfirmacionRecepcion = JsonConvert.DeserializeObject<ConfirmacionRecepcionResponse>(jsonContent);
                    break;
                case CInterfazExterna.AjustesDeStock:
                    e.Ajustes = JsonConvert.DeserializeObject<AjustesResponse>(jsonContent);
                    break;
                case CInterfazExterna.PedidosAnulados:
                    e.PedidosAnulados = JsonConvert.DeserializeObject<PedidosAnuladosResponse>(jsonContent);
                    break;
                case CInterfazExterna.ConfirmacionDePedido:
                    e.ConfirmacionPedido = JsonConvert.DeserializeObject<ConfirmacionPedidoResponse>(jsonContent);
                    break;
                case CInterfazExterna.Facturacion:
                    e.ConfirmacionMercaderiaPreparada = JsonConvert.DeserializeObject<FacturacionResponse>(jsonContent);
                    break;
                case CInterfazExterna.ConsultaDeStock:
                    e.ConsultaStock = JsonConvert.DeserializeObject<ConsultaStockResponse>(jsonContent);
                    break;
                case CInterfazExterna.Almacenamiento:
                    e.Almacenamiento = JsonConvert.DeserializeObject<AlmacenamientoResponse>(jsonContent);
                    break;
                case CInterfazExterna.ConfirmacionProduccion:
                    e.ConfirmacionProduccion = JsonConvert.DeserializeObject<ConfirmacionProduccionResponse>(jsonContent);
                    break;
                default:
                    e.Test = JsonConvert.DeserializeObject<TestResponse>(jsonContent);
                    break;
            }

            foreach (PropertyInfo pi in e.GetType().GetFilteredProperties())
            {
                if (pi.GetValue(e) != null)
                {
                    var id = pi.Name;
                    if (camelCaseEnabled)
                        id = id.Camelize();

                    e.Id = id;
                    break;
                }
            }

            return e;
        }
    }
}
