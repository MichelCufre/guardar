using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    class ExcelColumn : Attribute
    {
        public string Descripcion { get; set; }
    }
    public class AUXCOR
    {
        [ExcelColumn(Descripcion = "Cliente")]
        public string CD_CLIENTE { get; set; }
        [ExcelColumn(Descripcion = "Empresa")]
        public int CD_EMPRESA { get; set; }
        [ExcelColumn(Descripcion = "Agenda")]
        public Nullable<int> NU_AGENDA { get; set; }
        [ExcelColumn(Descripcion = "Pedido")]
        public string NU_PEDIDO { get; set; }
        [ExcelColumn(Descripcion = "Lote")]
        public string NU_IDENTIFICADOR { get; set; }
        [ExcelColumn(Descripcion = "Nro. Pallet")]
        public String NU_PALLET { get; set; }
        [ExcelColumn(Descripcion = "Fecha Ingreso")]
        public Nullable<System.DateTime> DT_INGRESO { get; set; }
        [ExcelColumn(Descripcion = "Fecha de Retiro")]
        public Nullable<System.DateTime> DT_RETIRO { get; set; }
        [ExcelColumn(Descripcion = "Componente")]
        public string NU_COMPONENTE { get; set; }
        [ExcelColumn(Descripcion = "Unidad x Bulto")]
        public decimal QT_UND_BULTO { get; set; }
        [ExcelColumn(Descripcion = "Unidad de Manejo")]
        public string CD_UNIDADE_MEDIDA { get; set; }
        [ExcelColumn(Descripcion = "Envase")]
        public string DS_ENVASE { get; set; }
        [ExcelColumn(Descripcion = "Mercadería")]
        public string DS_PRODUTO { get; set; }
        [ExcelColumn(Descripcion = "Cantidad")]
        public decimal QT_CANTIDAD { get; set; }
        [ExcelColumn(Descripcion = "Peso (kilos)")]
        public decimal PS_BULTO { get; set; }
        [ExcelColumn(Descripcion = "Días estadía")]
        public int QT_ESTADIA { get; set; }
        [ExcelColumn(Descripcion = "Tarifa")]
        public Nullable<decimal> IM_TARIFADIA { get; set; }
        [ExcelColumn(Descripcion = "Importe")]
        public Nullable<decimal> IM_IMPORTE { get; set; }
        [ExcelColumn(Descripcion = "Tp. Resultado")]
        public string TP_RESULTADO { get; set; }
        [ExcelColumn(Descripcion = "DSPRUEBA")]
        public decimal PRUEBA { get; set; }

        [ExcelColumn(Descripcion = "Etiquete Lote")]
        public int NU_ETIQUETA_LOTE { get; set; }
        [ExcelColumn(Descripcion = "Cod. Producto")]
        public string CD_PRODUTO { get; set; }
        [ExcelColumn(Descripcion = "Cód. Proceso")]
        public string CD_PROCESO { get; set; }
        [ExcelColumn(Descripcion = "Precio Unitario")]
        public Nullable<decimal> VL_PRECIO_UNITARIO { get; set; }
        [ExcelColumn(Descripcion = "Total")]
        public Nullable<decimal> QT_TOTAL { get; set; }
        [ExcelColumn(Descripcion = "Unidad Embebido")]
        public string CD_UNID_EMB { get; set; }
        [ExcelColumn(Descripcion = "Unidad Embebido")]
        public string NU_FOTO { get; set; }
        [ExcelColumn(Descripcion = "CD_FAIXA")]
        public decimal CD_FAIXA { get; set; }
        [ExcelColumn(Descripcion = "Predio")]
        public string NU_PREDIO { get; set; }
        [ExcelColumn(Descripcion = "Cantidad Producto")]
        public Nullable<decimal> QT_PRODUTO { get; set; }
    }
}
