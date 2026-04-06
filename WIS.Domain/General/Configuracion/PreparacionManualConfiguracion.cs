using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General.Configuracion
{
    public class PreparacionManualConfiguracion
    {
        public bool ControlTotal { get; set; }                      //WPRE300_Control_Picking_Total

        public bool PermitePickearVencido { get; set; }             //PRE052_DEFAULT_PICK_VENCIDO
        public bool PermitePickearVencidoHabilitado { get; set; }   //PRE052_ENABLED_PICK_VENCIDO

        public bool PermitePickearAveriado { get; set; }            //PRE052_DEFAULT_PICK_AVERIADO
        public bool PermitePickearAveriadoHabilitado { get; set; }  //PRE052_ENABLED_PICK_AVERIADO
        
        public bool ValidarProductoProveedor { get; set; }          //PRE052_DEFAULT_PICK_AVERIADO
        public bool ValidarProductoProveedorHabilitado { get; set; }//PRE052_ENABLED_PICK_AVERIADO       

    }
}
