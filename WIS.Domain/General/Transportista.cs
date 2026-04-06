using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General
{
    /// <summary>
    /// TODO: Agregar tipo fiscal, cambiar CodigoCiudad y CodigoDepartamento por ID_LOCALIDAD
    /// </summary>
    public class Transportista
    {
        public int Id { get; set; }                       //CD_TRANSPORTADORA
        public short? Estado { get; set; }                //CD_SITUACAO
        public string Descripcion { get; set; }           //DS_TRANSPORTADORA
        public string DireccionFiscal { get; set; }       //DS_ENDERECO
        public string CodigoCiudad { get; set; }          //CD_UF
        public int? CodigoDepartamento { get; set; }      //CD_MUNICIPIO
        public DateTime? FechaSituacion { get; set; }     //DT_SITUACAO
        public DateTime? FechaAlta { get; set; }          //DT_CADASTRAMENTO
        public DateTime? FechaModificacion { get; set; }  //DT_ALTERACAO
        public long? NumeroFiscal { get; set; }           //CD_CGC_TRANSP
        public string OtroDatoFiscal { get; set; }        //CD_INSCRICAO_TRANSP
        public string Contacto { get; set; }              //NM_CONTACTO
        public string TelefonoPrincipal { get; set; }     //NU_TELEFONE
        public string TelefonoSecundario { get; set; }    //NU_FAX


        public virtual void Enable()
        {
            this.Estado = SituacionDb.Activo;
            this.FechaSituacion = DateTime.Now;
        }

        public virtual void Disable()
        {
            this.Estado = SituacionDb.Inactivo;
            this.FechaSituacion = DateTime.Now;
        }
    }
}
