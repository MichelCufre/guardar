using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Documento
{
    public class DocumentoPreparacion
    {
        public int NroDocumentoPreparacion { get; set; }        //NU_DOCUMENTO_PREPARACION
        public int Preparacion { get; set; }                    //NU_PREPARACION               
        public int EmpresaIngreso { get; set; }                 //CD_EMPRESA_INGRESO
        public int EmpresaEgreso { get; set; }                  //CD_EMPRESA_Egreso                                                     
        public string TpOperativa { get; set; }                 //TP_OPERATIVA
        public bool Activa { get; set; }                        //FL_ACTIVE
        public string NroDocumentoIngreso { get; set; }         //NU_DOCUMENTO_INGRESO
        public string TpDocumentoIngreso { get; set; }          //TP_DOCUMENTO_INGRESO
        public string NroDocumentoEgreso { get; set; }          //NU_DOCUMENTO_EGRESO
        public string TpDocumentoEgreso { get; set; }           //TP_DOCUMENTO_EGRESO
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public int? Funcionario { get; set; }                   //CD_FUNCIONARIO

        public virtual void Enable()
        {
            this.Activa = true;
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void Disable()
        {
            this.Activa = false;
            this.FechaModificacion = DateTime.Now;
        }
    }
}
