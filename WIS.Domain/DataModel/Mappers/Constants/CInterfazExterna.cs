using System;
using System.Collections.Generic;
using System.Linq;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class CInterfazExterna
    {
        public const int SinEspecificar = -1;
        public const int Tracking = 10;

        //Entrada
        public const int Facturas = 425;
        public const int Producto = 500;
        public const int Pedidos = 503;
        public const int CodigoDeBarras = 505;
        public const int ProductoProveedor = 506;
        public const int Agentes = 507;
        public const int ReferenciaDeRecepcion = 510;
        public const int AnulacionReferenciaRecepcion = 513;
        public const int ModificarDetalleReferenciaRecepcion = 520;
        public const int Empresas = 522;
        public const int Agendas = 525;
        public const int Egresos = 530;
        public const int Lpn = 535;
        public const int ControlDeCalidad = 540;
        public const int PickingProducto = 570;
        public const int ModificarPedido = 585;
        public const int Produccion = 700;
        public const int ProducirProduccion = 701;
        public const int ConsumirProduccion = 702;
        public const int CambiarContenedor = 980;
        public const int AnularPickingPedidoPendiente = 990;
        public const int CrossDockingUnaFase = 994;
        public const int SepararPicking = 995;
        public const int AnulacionPicking = 996;
        public const int Picking = 997;
        public const int AjustarStock = 998;
        public const int TransferenciaStock = 999;

        //Salida
        public const int ConfirmacionDeRecepcion = 502;
        public const int AjustesDeStock = 508;
        public const int PedidosAnulados = 509;
        public const int ConfirmacionDePedido = 512;
        public const int Facturacion = 516;
        public const int ConsultaDeStock = 601;
        public const int Almacenamiento = 526;
        public const int ConfirmacionProduccion = 790;

        public static List<int> GetConstantValues()
        {
            Type type = typeof(CInterfazExterna);
            return type.GetFields().Select(x => (int)x.GetValue(null)).ToList();
        }

        public static bool IsSalida(int interfazExterna)
        {
            switch (interfazExterna)
            {
                case CInterfazExterna.ConfirmacionDeRecepcion:
                case CInterfazExterna.AjustesDeStock:
                case CInterfazExterna.PedidosAnulados:
                case CInterfazExterna.ConfirmacionDePedido:
                case CInterfazExterna.Facturacion:
                case CInterfazExterna.ConsultaDeStock:
                case CInterfazExterna.Almacenamiento:
                case CInterfazExterna.ConfirmacionProduccion:
                    return true;
                default:
                    return false;
            }
        }
    }
}
