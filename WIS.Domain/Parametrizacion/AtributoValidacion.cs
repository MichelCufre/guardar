using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Parametrizacion
{
    public class AtributoValidacion
    {
        public short Id { get;  set; }                  //ID_VALIDACION
        public string NombreValidacion { get;  set; }   //NM_VALIDACION
        public string Descripcion { get;  set; }        //DS_VALIDACION
        public string AtributoTipo { get;  set; }       //ID_ATRIBUTO_TIPO
        public string NombreArgumento { get;  set; }    //NM_ARGUMENTO
        public string TipoArgumento { get;  set; }      //TP_ARGUMENTO
        public string Error { get;  set; }              //DS_ERROR
    }
}
