using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class DominioDetalle
    {
        public string Id { get; set; }              //NU_DOMINIO
        public string Codigo { get; set; }          //CD_DOMINIO
        public string Valor { get; set; }           //CD_DOMINIO_VALOR
        public string Descripcion { get; set; }     //DS_DOMINIO_VALOR
    }
}
