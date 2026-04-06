using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Picking
{
    public class AnulacionPreparacion
    {
        public int NroAnulacionPreparacion { get; set; }    //NU_ANULACION_PREPARACION
        public int Preparacion { get; set; }                //NU_PREPARACION
        public string Estado { get; set; }                  //ND_ESTADO
        public string Descripcion { get; set; }             //DS_ANULACION
        public int TipoAnulacion { get; set; }              //TP_ANULACION
        public string TipoAgrupacion { get; set; }          //TP_AGRUPACION
        public int? UserId { get; set; }                    //USERID
        public DateTime Alta { get; set; }                  //DT_ADDROW
        public DateTime? Modificacion { get; set; }         //DT_UPDROW
    }
}
