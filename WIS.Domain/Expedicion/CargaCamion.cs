using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Expedicion
{
    public class CargaCamion
    {
        public int Camion { get; set; }                     //CD_CAMION
        public long Carga { get; set; }                     //NU_CARGA
        public string Cliente { get; set; }                 //CD_CLIENTE
        public int Empresa { get; set; }                    //CD_EMPRESA
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
        public DateTime? FechaAlta { get; set; }            //DT_ADDROW
        public string IdCargar { get; set; }                //ID_CARGAR
        public string TipoModalidad { get; set; }           //TP_MODALIDAD
        public bool SincronizacionRealizada { get; set; }   //FL_SYNC_REALIZADA

        #region Api
        public string SincronizacionRealizadaId { get; set; }  //FL_SYNC_REALIZADA
        #endregion

    }
}
