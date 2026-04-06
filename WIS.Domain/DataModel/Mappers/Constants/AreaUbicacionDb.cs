using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class AreaUbicacionDb
    {
        public const short StockGeneral = 1;
        public const short Averias = 10;
        public const short Transferencia = 20;
        public const short Equipamiento = 21;
        public const short Picking = 30;
        public const short PuertaEmbarque = 90;
        public const short BlackBox = 70;
        public const short ProduccionSalida = 71;
        public const short Clasificacion = 40;

        public const short AutomatismoEntrada = 50;
        public const short AutomatismoPicking = 51;
        public const short AutomatismoSalida = 52;
        public const short AutomatismoRechazo = 53;
        public const short AutomatismoAjuste = 53;
        public const short AutomatismoTransferencia = 54;
        public const int SEMIACABADO = 44;
        public const int CONSUMIBLE = 45;

        public static List<short> GetAreasAutomatismo()
        {
            return new List<short>() {
                AutomatismoEntrada,
                AutomatismoPicking,
                AutomatismoSalida,
                AutomatismoRechazo,
                AutomatismoAjuste,
                AutomatismoTransferencia
            };
        }
    }
}
