using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class UbicacionTipo
    {
        public short Id { get; set; }                           //CD_TIPO_ENDERECO
        public string Descripcion { get; set; }                 //DS_TIPO_ENDERECO
        public int? CapacidadPallets { get; set; }              //QT_CAPAC_PALETES
        public bool PermiteVariosProductos { get; set; }        //ID_VARIOS_PRODUTOS
        public bool PermiteVariosLotes { get; set; }            //ID_VARIOS_LOTES
        public short IdTipoEstatura { get; set; }               //CD_TIPO_ESTRUTURA
        public decimal? Altura { get; set; }                    //VL_ALTURA
        public decimal? Ancho { get; set; }                     //VL_COMPRIMENTO
        public decimal? Largo { get; set; }                     //VL_LARGURA
        public decimal? PesoMaximo { get; set; }                //VL_PESO_MAXIMO
        public decimal? VolumenUnidadFacturacion { get; set; }  //QT_VOLUMEN_UNIDAD_FACTURACION
        public DateTime FechaInsercion { get; set; }            //DT_ADDROW
        public DateTime FechaModificacion { get; set; }         //DT_UPDROW
        public short? IdUbicacionArea { get; set; }              //CD_AREA_ARMAZ
        public bool RespetaClase { get; set; }                  //FL_RESPETA_CLASE                                                          
    }
}
