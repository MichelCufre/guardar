using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class Predio
    {
        public string Numero { get; set; }                  //NU_PREDIO
        public string Descripcion { get; set; }             //DS_PREDIO
        public string Direccion { get; set; }               //DS_ENDERECO
        public string PuntoEntrega { get; set; }            //CD_PUNTO_ENTREGA
        public bool SincronizacionRealizada { get; set; }   //FL_SYNC_REALIZADA
        public string IdExterno { get; set; }               //ID_EXTERNO
        public DateTime? Alta { get; set; }                 //DT_ADDROW
        public DateTime? Modificacion { get; set; }         //DT_UPDROW

    }
}
