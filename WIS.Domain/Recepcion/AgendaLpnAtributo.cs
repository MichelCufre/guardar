using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Domain.Tracking.Models;
using WIS.TrafficOfficer;

namespace WIS.Domain.Recepcion
{
    public class AgendaLpnAtributo
    {
        public int NroAgenda { get; set; }                  //NU_AGENDA
        public long NroLPN { get; set; }                    //NU_LPN
        public int IdAtributo { get; set; }                 //ID_ATRIBUTO
        public string Nombre { get; set; }                  //NM_ATRIBUTO
        public string Valor { get; set; }                   //VL_LPN_ATRIBUTO
        public DateTime? FechaInsercion { get; set; }       //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
    }
}