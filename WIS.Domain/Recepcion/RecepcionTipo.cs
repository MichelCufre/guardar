using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Reportes;

namespace WIS.Domain.Recepcion
{
    public class RecepcionTipo
    {
        public string Tipo { get; set; }                                    //TP_RECEPCION
        public string Descripcion { get; set; }                             //DS_TIPO_RECEPCION
        public string TipoAgente { get; set; }                              //TP_AGENTE
        public bool AceptaProductosNoEsperados { get; set; }                //FL_PRODUCTOS_NO_ESPERADOS
        public bool IngresaFactura { get; set; }                            //FL_INGRESO_FACTURA
        public bool PermiteRecepcionAutomatica { get; set; }                //FL_PERMITE_AUTO_RECEPCION
        public bool PermiteDigitacion { get; set; }                         //FL_DIGITACION_HABILITADA
        public bool CargaDetalleAutomaticamente { get; set; }               //FL_CARGA_AUTO_DETALLE
        public bool HabilitaAsociacionAlCrearEmpresa { get; set; }          //FL_HABILITAR_EMPRESA_DEFAULT
        public bool CancelarSaldosReferenciasAlCierreDeAgenda { get; set; } //FL_CANCELAR_SALDO_AL_CIERRE
        public string TipoSeleccionReferencia { get; set; }                 //TP_SELECCION_REFERENCIA
        public string TipoReferencia { get; set; }                          //TP_REFERENCIA
        public string TipoManejoNumeroDocumento { get; set; }               //TP_MANEJO_NU_DOCUMENTO
        public string OrdenCompraSinSaldoAgenda { get; set; }               //FL_OC_SIN_SALDO_AGENDA
        public string EspecificarPredio { get; set; }                       //FL_ESPECIFICAR_PREDIO
        public string CierreAgenda { get; set; }                            //VL_CIERRE_AGENDA
        public string VlCrossDocking { get; set; }                          //VL_CROSS_DOCKING
        public string AdmiteProductosActivos { get; set; }                  //FL_PRODUCTOS_ACTIVOS
        public string PermiteRecibirLotesNoEsperados { get; set; }          //FL_RECIBIR_LOTES_NO_ESPERADOS
        public string ControlarVencimiento { get; set; }                    //FL_CONTROLAR_VENCIMIENTO
        public string VlEtiquetaRecepcion { get; set; }                     //VL_ETIQUETAS_RECEPCION
        public string VlEspecificaLotes { get; set; }                       //VL_ESPECIFICAR_LOTES
        public string RequiereAgendarHorarioPuerta { get; set; }            //FL_AGENDAR_HORARIO_PUERTA
        public string VlEstadoEtiqueta { get; set; }                        //VL_ESTADO_ETIQUETA
        public string PermiteAlmacenarEnAveria { get; set; }                //FL_PERMITIR_ALMACENAR_AVERIA
        public string VlInterfazEnCierreAgenda { get; set; }                //VL_INTERFAZ_EN_CIERRE_AGENDA
        public string OrdenCompraSinSaldoRecepcion { get; set; }            //FL_OC_SIN_SALDO_RECEPCION
        public string AceptaCantidadMayorAgendado { get; set; }             //FL_ACEPTA_QT_MAYOR_A_AGENDADO
        public string ManipuleoTarea { get; set; }                          //FL_MANIPULEO_TAREA
        public string ControlPeso { get; set; }                             //FL_CTRL_PESO
        public string ControlVencimiento { get; set; }                      //FL_CTRL_VENCIMIENTO
        public string ControlVolumen { get; set; }                          //FL_CTRL_VOLUMEN
        public string AdmiteMonoReferencia { get; set; }                    //FL_MONO_REFERENCIA
        public string MotivoRequerido { get; set; }                         //FL_MOTIVO_REQUERIDO
        public string PermitePlanificarLpn { get; set; }                    //FL_PERMITE_PLANIFICAR_LPN
        public string PermiteRecibirLpn { get; set; }                       //FL_PERMITE_RECIBIR_LPN
        public string PermiteLpnNoEsperado { get; set; }                    //FL_PERMITE_LPN_NO_ESPERADO

        public List<ReporteDefinicion> Reportes { get; set; }

        public RecepcionTipo()
        {
            Reportes = new List<ReporteDefinicion>();
        }
    }
}
