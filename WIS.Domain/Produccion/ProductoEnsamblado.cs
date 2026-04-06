namespace WIS.Domain.Produccion
{
    public class ProductoEnsamblado
    {
        public string CD_PRDC_DEFINICION { get; set; }

        public int CD_EMPRESA { get; set; }

        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }
        
        public string NU_IDENTIFICADOR { get; set; }

        public decimal QT_PREPARADO { get; set; }

        public decimal QT_COMPLETA { get; set; }

        public decimal QT_SALIDA { get; set; }

        public decimal QT_SOBRANTE { get; set; }
    }
}
