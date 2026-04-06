using System;

namespace WIS.Domain.Recorridos
{
    public class AplicacionRecorridoUsuario
    {
        public int IdRecorrido { get; set; }                //NU_RECORRIDO

        public string IdAplicacion { get; set; }            //CD_APLICACION

        public int UserId { get; set; }                     //USERID

        public bool EsPredeterminado { get; set; }          //FL_PREDETERMINADO

        public long? NuTransaccion { get; set; }            //NU_TRANSACCION

        public DateTime? FechaAlta { get; set; }            //DT_ADDROW

        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW

        public long? NuTransaccionDelete { get; set; }      //NU_TRANSACCION_DELETE

        public int NumeroRecorridoDefault { get; set; }
    }
}
