using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
    public class Preferencia
    {
        public int NU_PREFERENCIA { get; set; }
        public string DS_PREFERENCIA { get; set; }
        public string NU_PREDIO { get; set; }
        public string ID_BLOQUE_MIN { get; set; }
        public string ID_BLOQUE_MAX { get; set; }
        public string ID_CALLE_MIN { get; set; }
        public string ID_CALLE_MAX { get; set; }
        public int? NU_COLUMNA_MIN { get; set; }
        public int? NU_COLUMNA_MAX { get; set; }
        public int? NU_ALTURA_MIN { get; set; }
        public int? NU_ALTURA_MAX { get; set; }
        public decimal? PS_BRUTO_MAXIMO { get; set; }
        public decimal? VL_CUBAGEM_MAXIMO { get; set; }
        public int? QT_CLIENTES { get; set; }
        public int? QT_PEDIDOS { get; set; }
        public int? QT_MAXIMO_PICKEOS { get; set; }
        public int? QT_MAXIMO_UNIDADES { get; set; }
        public string FL_HABILITADO_EMPRESA { get; set; }
        public string FL_HABILITADO_CLIENTE { get; set; }
        public string FL_HABILITADO_RUTA { get; set; }
        public string FL_HABILITADO_ZONA { get; set; }
        public string FL_HABILITADO_COND_LIBERACION { get; set; }
        public string FL_HABILITADO_TP_PEDIDO { get; set; }
        public string FL_HABILITADO_TP_EXPEDICION { get; set; }
        public string FL_HABILITADO_CLASE { get; set; }
        public string FL_HABILITADO_FAMILIA { get; set; }
        public string FL_HABILITADO_CONT_ACCESO { get; set; }
        public string FL_HABILITADO_RANGO_UBIC { get; set; }
        public string FL_HABILITADO_PEDIDO_COMPLETO { get; set; }
        public string FL_HABILITADO_LIB_COMPLETO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

    }
}
