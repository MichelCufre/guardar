namespace WIS.Domain.Parametrizacion
{
    public class LpnTipo
    {
        public string Tipo { get; set; }                            //TP_LPN_TIPO
        public string Nombre { get; set; }                          //NM_LPN_TIPO
        public string Descripcion { get; set; }                     //DS_LPN_TIPO
        public string PermiteConsolidar { get; set; }               //FL_PERMITE_CONSOLIDAR
        public string PermiteExtraerLineas { get; set; }            //FL_PERMITE_EXTRAER_LINEAS
        public string PermiteAgregarLineas { get; set; }            //FL_PERMITE_AGREGAR_LINEAS
        public string CrearSoloAlIngreso { get; set; }              //FL_CREAR_SOLO_AL_INGRESO
        public string MultiProducto { get; set; }                   //FL_MULTIPRODUCTO
        public string MultiLote { get; set; }                       //FL_MULTI_LOTE
        public string PermiteAnidacion { get; set; }                //FL_PERMITE_ANIDACION
        public string NumeroTemplate { get; set; }                  //NU_TEMPLATE_ETIQUETA
        public string NumeroComponente { get; set; }                //NU_COMPONENTE
        public string ContenedorLPN { get; set; }                   //FL_CONTENEDOR_LPN
        public long? NumeroSecuencia { get; set; }                  //NU_SEQ_LPN
        public string PermiteGenerar { get; set; }                  //FL_PERMITE_GENERAR
        public string IngresoRecepcionAtributo { get;  set; }       //FL_INGRESO_RECEPCION_ATRIBUTO
        public string IngresoPickingAtributo { get;  set; }         //FL_INGRESO_PICKING_ATRIBUTO
        public string Prefijo { get;  set; }                        //VL_PREFIJO
        public string EtiquetaRecepcion { get;  set; }              //TP_ETIQUETA_RECEPCION
        public string PermiteDestruirAlmacenaje { get; set; }       //FL_PERMITE_DESTRUIR_ALM
        public LpnTipoPuntajePicking PuntajePicking { get; set; }
    }
}
