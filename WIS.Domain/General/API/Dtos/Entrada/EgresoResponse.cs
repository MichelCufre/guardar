using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class EgresoResponse
    {
        public int Camion { get; set; }                             //CD_CAMION
        public string Descripcion { get; set; }                     //DS_CAMION
        public string Predio { get; set; }                          //NU_PREDIO
        public int? Empresa { get; set; }                           //CD_EMPRESA
        public short? Ruta { get; set; }                            //CD_ROTA
        public short? Puerta { get; set; }                          //CD_PORTA
        public short Estado { get; set; }                           //CD_SITUACAO
        public string Matricula { get; set; }                       //CD_PLACA_CARRO
        public int Transportista { get; set; }                      //CD_TRANSPORTADORA
        public int? Vehiculo { get; set; }                          //CD_VEICULO
        public string FechaCreacion { get; set; }                   //DT_ADDROW
        public string FechaProgramado { get; set; }                 //DT_PROGRAMADO
        //public string FechaArriboCamion { get; set; }             //DT_ARRIBO_CAMION
        public string FechaFacturacion { get; set; }                //DT_FACTURACION
        public string FechaCierre { get; set; }                     //DT_CIERRE
        //public string FechaModificacion { get; set; }             //DT_UPDROW
        public int? FuncionarioCierre { get; set; }                 //CD_FUNC_CIERRE
        public long? NumeroInterfazEjecucionCierre { get; set; }    //NU_INTERFAZ_EJECUCION
        public long? NumeroInterfazEjecucionFactura { get; set; }   //NU_INTERFAZ_EJECUCION_FACT
        public string ArmadoEgreso { get; set; }                    //ND_ARMADO_EGRESO        
        public string TipoArmadoEgreso { get; set; }                //TP_ARMADO_EGRESO
        public string Documento { get; set; }                       //DS_DOCUMENTO                
        public string RespetaOrdenCarga { get; set; }               //ID_RESPETA_ORD_CARGA        
        public string TrackingHabilitado { get; set; }              //FL_TRACKING        
        public string RuteoHabilitado { get; set; }                 //FL_RUTEO        
        public string SincronizacionRealizada { get; set; }         //FL_SYNC_REALIZADA        
        public string ConfirmacionViajeRealizada { get; set; }      //FL_CONF_VIAJE_REALIZADA        
        public string CargaHabilitada { get; set; }                 //FL_HABILITADO_CARGA        
        public string CierreHabilitado { get; set; }                //FL_HABILITADO_CIERRE        
        public string ArmadoHabilitado { get; set; }                //FL_HABILITADO_ARMADO        
        public string CierreParcialHabilitado { get; set; }         //FL_CIERRE_PARCIAL        
        public string CierreAutomaticoHabilitado { get; set; }      //FL_CIERRE_AUTO        
        public string CargaAutomaticaHabilitada { get; set; }       //FL_AUTO_CARGA        
        public string ControlContenedoresHabilitado { get; set; }   //FL_CTRL_CONTENEDORES
        public long? NumeroTransaccion { get; set; }                //NU_TRANSACCION
        public int? NumeroOrtOrden { get; set; }                    //NU_ORT_ORDEN
        public string IdExterno { get; set; }                       //ID_EXTERNO
        public int? EmpresaExterna { get; set; }                    //CD_EMPRESA_EXTERNA

        public List<CargasAsociadasResponse> CargasAsociadas { get; set; }

        public EgresoResponse()
        {
            CargasAsociadas = new List<CargasAsociadasResponse>();
        }
    }
    public class CargasAsociadasResponse
    {
        public long Carga { get; set; }                     //NU_CARGA
        public string CodigoAgente { get; set; }            //CD_AGENTE
        public string TipoAgente { get; set; }              //TP_AGENTE
        public int Empresa { get; set; }                    //CD_EMPRESA
        public string ParaCargar { get; set; }              //ID_CARGAR
        public string ModalidadDeCarga { get; set; }        //TP_MODALIDAD
        public string SincronizacionRealizada { get; set; } //FL_SYNC_REALIZADA

        //public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
        //public DateTime? FechaAlta { get; set; }            //DT_ADDROW
    }
}
