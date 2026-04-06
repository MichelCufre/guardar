using System.Collections.Generic;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class ConfirmacionProduccionResponse
    {
        [ApiDtoExample("1")]
        public string IdProduccionExterno { get; set; }     //ID_PRODUCCION_EXTERNO

        [ApiDtoExample("11")]
        public int Empresa { get; set; }                    // CD_EMPRESA

        [ApiDtoExample("TPINGPR_BLACKBOX")]
        public string Tipo { get; set; }                    //ND_TIPO

        [ApiDtoExample("1")]
        public string Predio { get; set; }                  //NU_PREDIO

        [ApiDtoExample("1")]
        public string EspacioProduccion { get; set; }       //CD_PRDC_LINEA

        [ApiDtoExample("12/06/2016")]
        public string FechaInicioProduccion { get; set; }   //DT_INICIO_PRODUCCION

        [ApiDtoExample("12/06/2026")]
        public string FechaFinProduccion { get; set; }      //DT_FIN_PRODUCCION

        [ApiDtoExample("Anexo1")]
        public string Anexo1 { get; set; }                  //DS_ANEXO1

        [ApiDtoExample("Anexo2")]
        public string Anexo2 { get; set; }                  //DS_ANEXO2

        [ApiDtoExample("Anexo3")]
        public string Anexo3 { get; set; }                  //DS_ANEXO3

        [ApiDtoExample("Anexo4")]
        public string Anexo4 { get; set; }                  //DS_ANEXO4

        [ApiDtoExample("Anexo5")]
        public string Anexo5 { get; set; }                  //DS_ANEXO5

        [ApiDtoExample("S")]
        public string FinProduccion { get; set; }

        public List<InsumoProduccionResponse> Insumos { get; set; }
        public List<ProductoProduccionResponse> Productos { get; set; }

        public ConfirmacionProduccionResponse()
        {
            Insumos = new List<InsumoProduccionResponse>();
            Productos = new List<ProductoProduccionResponse>();
        }
    }
}
