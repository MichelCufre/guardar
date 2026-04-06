using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class ProductoProveedor
    {
        public ProductoProveedor()
        {
        }
        public ProductoProveedor(string tipoOperacionId, string tipoAgente, string codigoAgente)
        {
            TipoOperacionId = tipoOperacionId;
            TipoAgente = tipoAgente;
            CodigoAgente = codigoAgente;
        }

        public string CodigoProducto { get; set; }  //CD_PRODUTO
        public int Empresa { get; set; }            //CD_EMPRESA
        public string Cliente { get; set; }         //CD_CLIENTE
        public string CodigoExterno { get; set; }   //CD_EXTERNO
        public DateTime? FechaIngreso { get; set; }   //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }   //DT_UPDROW

        #region WMS_API
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public string TipoOperacionId { get; set; }
        #endregion

    }
}
