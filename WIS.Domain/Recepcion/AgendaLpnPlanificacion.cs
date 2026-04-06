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
    public class AgendaLpnPlanificacion
    {
        public int NroAgenda { get; set; }                  //NU_AGENDA
        public long NroLPN { get; set; }                    //NU_LPN
        public string Planificado { get; set; }             //FL_PLANIFICADO
        public string Recibido { get; set; }                //FL_RECIBIDO
        public int? Funcionario { get; set; }               //CD_FUNCIONARIO
        public int? FuncionarioRecepcion { get; set; }      //CD_FUNCIONARIO_RECEPCION
        public DateTime? FechaRecepcion{ get; set; }        //DT_RECEPCION
        public DateTime? FechaInsercion { get; set; }       //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW

        public long? NumeroTransaccion { get; set; }        
    }
}