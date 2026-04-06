namespace WIS.Domain.Produccion.DTOs
{
    public class DTOIngreso
    {
        public string NU_PRDC_INGRESO { get; set; }
        public string CD_PRDC_DEFINICION { get; set; }
        public string NM_PRDC_DEFINICION { get; set; }
        public string CD_SITUACAO { get; set; }
        public string DS_SITUACAO { get; set; }
        public string CD_EMPRESA { get; set; }
        public string NM_EMPRESA { get; set; }
        public string ID_GENERAR_PEDIDO { get; set; }
        public string DT_ADDROW { get; set; }
        public string CD_FUNCIONARIO { get; set; }
        public string NM_FUNCIONARIO { get; set; }
        public string QT_FORMULA { get; set; }
        public string QT_PEDIDO { get; set; }
        public string QT_LIBERADO { get; set; }
        public string QT_PREPARADO { get; set; }
        public string QT_LINEA { get; set; }
        public string QT_ELABORADO { get; set; }
        public string DS_ANEXO1 { get; set; }
        public string DS_ANEXO2 { get; set; }
        public string DS_ANEXO3 { get; set; }
        public string DS_ANEXO4 { get; set; }
        public string DS_TIPO_INGRESO { get; set; }
        public string ND_TIPO { get; set; }
    }
}
