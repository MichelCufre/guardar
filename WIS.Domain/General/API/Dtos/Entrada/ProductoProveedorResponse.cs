using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ProductoProveedorResponse
    {
        [StringLength(40)]
        public string CodigoProducto { get; set; }  //CD_PRODUTO
        public int Empresa { get; set; }            //CD_EMPRESA
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public string CodigoExterno { get; set; }   //CD_EXTERNO
    }
}
