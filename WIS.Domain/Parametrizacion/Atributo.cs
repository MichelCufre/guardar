using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Parametrizacion
{
	public class Atributo
	{
		public int Id { get; set; }                     //ID_ATRIBUTO
		public string Nombre { get; set; }              //NM_ATRIBUTO
		public string Descripcion { get; set; }         //DS_ATRIBUTO
		public string IdTipo { get; set; }              //ID_ATRIBUTO_TIPO
		public string CodigoDominio { get; set; }       //CD_DOMINIO
		public string Campo { get; set; }               //NM_CAMPO
		public short? Largo { get; set; }               //NU_LARGO
		public short? Decimales { get; set; }           //NU_DECIMALES
		public string MascaraDisplay { get; set; }      //VL_MASCARA_DISPLAY
		public string MascaraIngreso { get; set; }      //VL_MASCARA_INGRESO
		public string Separador { get; set; }           //VL_SEPARADOR
	}
}
