using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.Parametrizacion;

namespace WIS.Domain.StockEntities
{
    public class Lpn
    {
        public long NumeroLPN { get; set; }                 //NU_LPN
        public string IdExterno { get; set; }               //ID_LPN_EXTERNO
        public int Empresa { get; set; }                    //CD_EMPRESA
        public string Tipo { get; set; }                    //TP_LPN_TIPO
        public string Estado { get; set; }                  //ID_ESTADO
        public string Ubicacion { get; set; }               //CD_ENDERECO
        public DateTime FechaAdicion { get; set; }          //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
        public DateTime? FechaActivacion { get; set; }      //DT_ACTIVACION
        public DateTime? FechaFin { get; set; }             //DT_FIN
        public long? NumeroTransaccion { get; set; }        //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }  //NU_TRANSACCION_DELETE
        public string IdPacking { get; set; }               //ID_PACKING
        public int? NroAgenda { get; set; }                 //NU_AGENDA
        public string DisponibleLiberacion { get; set; }    //FL_DISPONIBLE_LIBERACION

        public List<LpnDetalle> Detalles { get; set; }

        #region Api
        public List<AtributoValor> AtributosSinDefinir { get; set; }

        public List<LpnBarras> BarrasSinDefinir { get; set; }

        public string Predio { get; set; }
        #endregion

        public Lpn()
        {
            Detalles = new List<LpnDetalle>();
        }
    }
}
