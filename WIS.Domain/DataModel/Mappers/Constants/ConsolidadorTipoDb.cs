using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class ConsolidadorTipoDb
    {
        public const int USAR_VALOR_DE_LPN_MÁS_ANTIGUO = 1;
        public const int USAR_VALOR_DE_LPN_MÁS_NUEVO = 2;
        public const int USAR_EL_ÚLTIMO_VALOR_CARGADO = 3;
        public const int USAR_EL_MAYOR = 4;
        public const int USAR_EL_MENOR = 5;
        public const int USAR_EL_VALOR_DEL_LPN_DESTINO = 6;
        public const int USAR_EL_VALOR_DEL_LPN_ORIGEN = 7;
        public const int SOLICITAR_AL_USUARIO = 8;
        public const int BLOQUEAR_CONSOLIDACIÓN_SI_EL_VALOR_ES_DISTINTO = 9;
        public const int SUMAR_VALOR_SÓLO_NUMERICOS = 10;
    }
}
