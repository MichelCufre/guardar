using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class ColasTrabajoDb
    {
        public const string Cliente = "CD_CLIENTE";
        public const string CondicionLiberacion = "CD_CONDICION_LIBERACION";
        public const string CondicionEmpresa = "CD_EMPRESA";
        public const string CondicionFechaEmitido = "DT_EMITIDO";
        public const string CondicionFechaEntrega = "DT_ENTREGA";
        public const string CondicionFechaLiberado = "DT_LIBERADO";
        public const string CondicionGemeroca = "VL_PONDERACION_GENERICA";
        public const string CondicionRota = "CD_ROTA";
        public const string CondicionZona = "CD_ZONA";
        public const string CondicionPedido = "TP_PEDIDO";
        public const string CondicionExpedicion = "TP_EXPEDICION";
        public const string OperacionIgual = "=";
        public const string OperacionMayor = ">";
        public const string OperacionMayorIgual = ">=";
        public const string OperacionMenor = "<";
        public const string OperacionMenorIgual = "<=";
        public const string OperacionEntre = "entre";
        public const string OperadorAnd = " and ";
    }
}
