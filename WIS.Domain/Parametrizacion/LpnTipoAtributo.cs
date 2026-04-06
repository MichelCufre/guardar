namespace WIS.Domain.Parametrizacion
{
    public class LpnTipoAtributo
    {
        public string TipoLpn { get;  set; }            //TP_LPN_TIPO
        public int IdAtributo { get;  set; }            //ID_ATRIBUTO
        public string NombreAtributo { get; set; }      //NM_ATRIBUTO
        public string ValorInicial { get;  set; }       //VL_INICIAL
        public string Requerido { get;  set; }          //FL_REQUERIDO
        public string ValidoInterfaz { get;  set; }     //VL_VALIDO_INTERFAZ
        public short? Orden { get;  set; }              //NU_ORDEN
        public int? IdConsolidador { get;  set; }       //ID_CONSOLIDACION_TIPO
        public string EstadoInicial { get; set; }       //ID_ESTADO_INICIAL

    }
}
