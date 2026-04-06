namespace WIS.Domain.Parametrizacion
{
    public class LpnDetalleAtributo
    {
        public long NumeroLpn { get; set; }                    //NU_LPN     
        public int IdLpnDetalle { get; set; }                  //ID_LPN_DET
        public string IdLpnExterno { get; set; }               //ID_LPN_EXTERNO
        public string Tipo { get; set; }                       //TP_LPN_TIPO
        public int IdAtributo { get; set; }                    //ID_ATRIBUTO
        public string NombreAtributo { get; set; }             //NM_ATRIBUTO
        public string ValorAtributo { get; set; }              //VL_LPN_DET_ATRIBUTO
        public string Producto { get; set; }                   //CD_PRODUTO
        public decimal Faixa { get; set; }                     //CD_FAIXA
        public int Empresa { get; set; }                       //CD_EMPRESA
        public string Lote { get; set; }                       //NU_IDENTIFICADOR
        public string Estado { get; set; }                     //ID_ESTADO
        public long? NumeroTransaccion { get; set; }           //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }     //NU_TRANSACCION_DELETE
    }
}
