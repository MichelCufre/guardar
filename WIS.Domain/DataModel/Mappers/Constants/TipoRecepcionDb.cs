using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class TipoRecepcionDb
    {
        public const string MultipleOC= "MOC";                              //Múltiple (OC)
        public const string MultipleOD = "SOD";                             //Múltiple (OD)
        public const string SeleccionOC = "SOC";                            //Mono (OC)
        public const string SeleccionOD = "DEVOD";                          //Devolución mono (OD)
        public const string DigitacionLibre = "DIGLIB";                     //Digitación Libre
        public const string DocumentosAduaneros = "DOCADU";                 //Documentos aduaneros
        public const string ReferenciaDeRecepcionRR = "REFREC";             //Referencia de recepción mono (RR)                                                                            
        public const string DevolucionDigitadaLibre = "DDIGLI";             //Devolución Digitada Libre

        public const string DigitacionLibreDeposito = "DIGLID";             //Digitación Libre Deposito
        public const string DevolucionDigitadaLibreTraspaso = "DIGLIT";     //Devolución Digitada Libre Traspaso
    }
}
