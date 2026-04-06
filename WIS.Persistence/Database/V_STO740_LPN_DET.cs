namespace WIS.Persistence.Database
{
    using System;

    public class V_STO740_LPN_DET
    {
        public long NU_LPN { get; set; }

        public string ID_LPN_EXTERNO { get; set; }

        public string TP_LPN_TIPO { get; set; }

        public string NM_LPN_TIPO { get; set; }

        public int ID_LPN_DET { get; set; }

        public string ID_LINEA_SISTEMA_EXTERNO { get; set; }

        public string CD_PRODUTO { get; set; }

        public string DS_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        public int CD_EMPRESA { get; set; }

        public string NM_EMPRESA { get; set; }

        public string NU_IDENTIFICADOR { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public decimal QT_ESTOQUE { get; set; }

        public decimal? QT_DECLARADA { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_EXPEDIDA { get; set; }

        public string ID_AVERIA { get; set; }

        public string ID_INVENTARIO { get; set; }

        public string ID_CTRL_CALIDAD { get; set; }

        public string ID_PACKING { get; set; }

        public int? NU_AGENDA { get; set; }

        public string CD_ENDERECO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ACTIVACION { get; set; }

        public DateTime? DT_FIN { get; set; }

        public string ID_ESTADO { get; set; }
    }
}