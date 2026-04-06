using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Interfaces
{
   public class InterfazError
    {
        public long Id { get; set; } //NU_INTERFAZ_EJECUCION
        public int NroError { get; set; } //NU_ERROR
        public int? Registro { get; set; } //NU_REGISTRO
        public string Referencia { get; set; } //DS_REFERENCIA
        public int? Parametro { get; set; } //CD_PARAMETRO
        public string CodigoError { get; set; } //CD_ERROR
        public string Descripcion { get; set; } //DS_ERROR
    }
}
