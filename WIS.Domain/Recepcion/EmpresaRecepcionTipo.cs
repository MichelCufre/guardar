using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Recepcion.Enums;

namespace WIS.Domain.Recepcion
{
    public class EmpresaRecepcionTipo
    {
        public int Id { get; set; }                             //NU_RECEPCION_REL_EMPRESA_TIPO
        public int? IdEmpresa { get; set; }                     //CD_EMPRESA
        public bool Habilitado { get; set; }                    //FL_HABILITADO
        public string TipoExterno { get; set; }                 //TP_RECEPCION_EXTERNO
        public int? InterfazExterna { get; set; }               //CD_INTERFAZ_EXTERNA
        public string DescripcionExterna { get; set; }          //DS_RECEPCION_EXTERNO
        public string ManejoDeInterfaz { get; set; }            //FL_MANEJO_INTERFAZ

        public Empresa Empresa { get; set; }
        public RecepcionTipo RecepcionTipoInterno { get; set; } //TP_RECEPCION


    }
}
