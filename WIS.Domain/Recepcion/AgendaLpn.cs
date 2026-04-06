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
    public class AgendaLpn
    {
        public int NroAgenda { get; set; }                  //NU_AGENDA
        public long NroLPN { get; set; }                    //NU_LPN
        public int Empresa { get; set; }                    //CD_EMPRESA
        public string IdExterno { get; set; }               //ID_LPN_EXTERNO
        public string Tipo { get; set; }                    //TP_LPN_TIPO
        public string IdPacking { get; set; }               //ID_PACKING
        public DateTime? FechaInsercion { get; set; }       //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
    }
}