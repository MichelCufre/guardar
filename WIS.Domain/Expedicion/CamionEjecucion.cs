using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion
{
    public class CamionEjecucion
    {
        public int Camion { get; set; }                     //CD_CAMION
        public long NumeroInterfazEjecucion { get; set; }   //NU_INTERFAZ_EJECUCION
        public string InterfazDeFacturacion { get; set; }   //FL_FACTURACION
        public DateTime FechaCreacion { get; set; }         //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
    }
}
