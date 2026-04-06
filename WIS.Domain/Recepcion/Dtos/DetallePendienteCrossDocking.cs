using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion.Dtos
{
    public class DetallePendienteCrossDocking
    {
        public int Id { get; set; }                       //NU_AGENDA
        public int Empresa { get; set; }                  //CD_EMPRESA
        public string Producto { get; set; }              //CD_PRODUTO
        public string Identificador { get; set; }              //NU_IDENTIFICADOR
        public string Cliente { get; set; }              //CD_CLIENTE
        public decimal CantidadPendiente { get; set; }              //QT_PEND_XD
    }
}
