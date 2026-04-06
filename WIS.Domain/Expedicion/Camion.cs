using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion.Enums;

namespace WIS.Domain.Expedicion
{
    public class Camion
    {
        public int Id { get; set; }                                     //CD_CAMION
        public string Matricula { get; set; }                           //CD_PLACA_CARRO
        public short? Ruta { get; set; }                                //CD_ROTA
        public short? Puerta { get; set; }                              //CD_PORTA
        public bool RespetaOrdenCarga { get; set; }                     //ID_RESPETA_ORD_CARGA
        public int Transportista { get; set; }                          //CD_TRANSPORTADORA
        public DateTime? FechaCreacion { get; set; }                    //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }                //DT_UPDROW
        public int? Empresa { get; set; }                               //CD_EMPRESA
        public DateTime? FechaProgramado { get; set; }                  //DT_PROGRAMADO
        public DateTime? FechaArriboCamion { get; set; }                //DT_ARRIBO_CAMION
        public string Descripcion { get; set; }                         //DS_CAMION
        public int? Vehiculo { get; set; }                              //CD_VEICULO
        public DateTime? FechaFacturacion { get; set; }                 //DT_FACTURACION
        public int? FuncionarioCierre { get; set; }                     //CD_FUNC_CIERRE
        public DateTime? FechaCierre { get; set; }                      //DT_CIERRE
        public long? NumeroInterfazEjecucionCierre { get; set; }        //NU_INTERFAZ_EJECUCION
        public long? NumeroInterfazEjecucionFactura { get; set; }       //NU_INTERFAZ_EJECUCION_FACT
        public bool IsTrackingHabilitado { get; set; }                  //FL_TRACKING
        public bool IsRuteoHabilitado { get; set; }                     //FL_RUTEO
        public int? NumeroOrtOrden { get; set; }                        //NU_ORT_ORDEN
        public string Predio { get; set; }                              //NU_PREDIO
        public bool IsSincronizacionRealizada { get; set; }             //FL_SYNC_REALIZADA
        public bool ConfirmacionViajeRealizada { get; set; }            //FL_CONF_VIAJE_REALIZADA
        public string ArmadoEgreso { get; set; }                        //ND_ARMADO_EGRESO
        public bool IsCargaHabilitada { get; set; }                     //FL_HABILITADO_CARGA
        public bool IsCierreHabilitado { get; set; }                    //FL_HABILITADO_CIERRE
        public bool ArmadoHabilitado { get; set; }                      //FL_HABILITADO_ARMADO
        public string Documento { get; set; }                           //DS_DOCUMENTO
        public bool IsCierreParcialHabilitado { get; set; }             //FL_CIERRE_PARCIAL
        public bool IsCierreAutomaticoHabilitado { get; set; }          //FL_CIERRE_AUTO
        public bool IsCargaAutomaticaHabilitada { get; set; }           //FL_AUTO_CARGA
        public bool IsControlContenedoresHabilitado { get; set; }       //FL_CTRL_CONTENEDORES
        public long? NumeroTransaccion { get; set; }                    //NU_TRANSACCION
        public CamionEstado Estado { get; set; }                        //CD_SITUACAO
        public string TipoArmadoEgreso { get; set; }                    //TP_ARMADO_EGRESO
        public string IdExterno { get; set; }                           //ID_EXTERNO
        public int? EmpresaExterna { get; set; }                        //CD_EMPRESA_EXTERNA
        public List<CargaCamion> Cargas { get; set; }

        #region WMS_API                                              
        public short EstadoId { get; set; }                           //CD_SITUACAO
        public string RespetaOrdenCargaId { get; set; }               //ID_RESPETA_ORD_CARGA
        public string TrackingHabilitadoId { get; set; }              //FL_TRACKING
        public string RuteoHabilitadoId { get; set; }                 //FL_RUTEO
        public string SincronizacionRealizadaId { get; set; }         //FL_SYNC_REALIZADA
        public string ConfirmacionViajeRealizadaId { get; set; }      //FL_CONF_VIAJE_REALIZADA
        public string CargaHabilitadaId { get; set; }                 //FL_HABILITADO_CARGA
        public string CierreHabilitadoId { get; set; }                //FL_HABILITADO_CIERRE
        public string ArmadoHabilitadoId { get; set; }                //FL_HABILITADO_ARMADO
        public string CierreParcialHabilitadoId { get; set; }         //FL_CIERRE_PARCIAL
        public string CierreAutomaticoHabilitadoId { get; set; }      //FL_CIERRE_AUTO
        public string CargaAutomaticaHabilitadaId { get; set; }       //FL_AUTO_CARGA
        public string ControlContenedoresHabilitadoId { get; set; }   //FL_CTRL_CONTENEDORES
        public string HabilitarUsoCargaAsignada { get; set; }         //Para funcionalidad de mover cargas de la api
        public string ProgramacionHoraInicio { get; set; }            //Futuro nuevo campo
        public string ProgramacionHoraFin { get; set; }               //Futuro nuevo campo
        public string Necesidades { get; set; }                       //Futuro nuevo campo
        public string PredioExterno { get; set; }                     //ID_EXTERNO - T_PREDIO  Funcionalidad Api
        public ArmadoEgresoDetalle DetalleArmadoEgreso { get; set; }  //Funcionalidad Api
        public bool IgnorarCargasInexistentes { get; set; }           //Funcionalidad Api

        #endregion

        public Camion()
        {
            this.Cargas = new List<CargaCamion>();
        }

        public Camion(short estadoId, string respetaOrdenCargaId, string trackingHabilitadoId, string ruteoHabilitadoId, string sincronizacionRealizadaId, string confirmacionViajeRealizadaId, string cargaHabilitadaId, string cierreHabilitadoId, string armadoHabilitadoId, string cierreParcialHabilitadoId, string cierreAutomaticoHabilitadoId, string cargaAutomaticaHabilitadaId, string controlContenedoresHabilitadoId, string habilitarUsoCargaAsignada, string programacionHoraInicio, string programacionHoraFin, string necesidades, string predioExterno)
        {
            EstadoId = estadoId;
            RespetaOrdenCargaId = respetaOrdenCargaId;
            TrackingHabilitadoId = trackingHabilitadoId;
            RuteoHabilitadoId = ruteoHabilitadoId;
            SincronizacionRealizadaId = sincronizacionRealizadaId;
            ConfirmacionViajeRealizadaId = confirmacionViajeRealizadaId;
            CargaHabilitadaId = cargaHabilitadaId;
            CierreHabilitadoId = cierreHabilitadoId;
            ArmadoHabilitadoId = armadoHabilitadoId;
            CierreParcialHabilitadoId = cierreParcialHabilitadoId;
            CierreAutomaticoHabilitadoId = cierreAutomaticoHabilitadoId;
            CargaAutomaticaHabilitadaId = cargaAutomaticaHabilitadaId;
            ControlContenedoresHabilitadoId = controlContenedoresHabilitadoId;
            HabilitarUsoCargaAsignada = habilitarUsoCargaAsignada;
            ProgramacionHoraInicio = programacionHoraInicio;
            ProgramacionHoraFin = programacionHoraFin;
            Necesidades = necesidades;
            PredioExterno = predioExterno;
            DetalleArmadoEgreso = new ArmadoEgresoDetalle();
        }

        #region Auxs

        public virtual bool IsCerrado()
        {
            return this.Estado == CamionEstado.Cerrado;
        }
        public virtual bool IsCargando()
        {
            return this.Estado == CamionEstado.Cargando;
        }
        public virtual bool IsFacturado()
        {
            return this.NumeroInterfazEjecucionFactura != null && this.NumeroInterfazEjecucionFactura >= 0;
        }
        public virtual bool PuedeFacturarse()
        {
            return ((this.NumeroInterfazEjecucionFactura == null) && (this.Estado == CamionEstado.AguardandoCarga || this.Estado == CamionEstado.Cargando));
        }
        public virtual bool IsAguarandoCarga()
        {
            return this.Estado == CamionEstado.AguardandoCarga;
        }
        public virtual bool IsPendienteFacturar()
        {
            return this.NumeroInterfazEjecucionFactura == null;
        }
        public virtual bool TieneMultiplesCargas()
        {
            return this.Cargas.GroupBy(d => d.Carga).Count() > 1;
        }
        public virtual bool IsFacturacionEnProceso()
        {
            return this.NumeroInterfazEjecucionFactura == -1;
        }
        public virtual bool PuedeSincronizarTracking()
        {
            //return this.Estado == CamionEstado.Cerrado && this.IsTrackingHabilitado && this.IsSincronizacionRealizada;
            return this.IsTrackingHabilitado && this.Estado != CamionEstado.Cerrado;
        }
        public virtual bool TieneDocumentacionCargada()
        {
            return !string.IsNullOrEmpty(this.Documento);
        }
        public virtual bool PuedeReSincronizarTracking()
        {
            //return this.Estado == CamionEstado.Cerrado && this.IsTrackingHabilitado && this.IsSincronizacionRealizada;
            return this.IsTrackingHabilitado && this.Estado == CamionEstado.Cerrado && !this.ConfirmacionViajeRealizada;
        }
        public virtual bool ExisteCarga(long carga, string cliente)
        {
            return this.Cargas.Any(d => d.Carga == carga && d.Cliente == cliente);
        }

        public virtual bool PuedeArmarse(bool manejoDocumentalActivo, bool egresoDocumentalEditable, bool armadoPorContenedor = false)
        {
            var armadoCamionSituacion = new List<CamionEstado>
            {
                CamionEstado.AguardandoCarga
            };

            if (armadoPorContenedor)
                armadoCamionSituacion.Add(CamionEstado.Cargando);

            if (this.NumeroInterfazEjecucionFactura == -1)
                return false;

            if (!armadoCamionSituacion.Contains(this.Estado))
                return false;

            if (manejoDocumentalActivo && !egresoDocumentalEditable)
                return false;

            return true;
        }
        public virtual bool PuedeArmarsePorCarga(bool manejoDocumentalActivo, bool egresoDocumentalEditable)
        {
            return this.PuedeArmarse(manejoDocumentalActivo, egresoDocumentalEditable) && this.FechaFacturacion == null;
        }
        public virtual bool PuedeGenerarseEgresoDocumental(bool manejoDocumentalActivo, bool egresoDocumentalEditable)
        {
            List<CamionEstado> armadoCamionSituacion = new List<CamionEstado>
            {
                CamionEstado.AguardandoCarga
            };

            if (!armadoCamionSituacion.Contains(this.Estado))
                return false;

            return manejoDocumentalActivo && egresoDocumentalEditable;
        }

        public virtual void Cerrar()
        {
            this.Estado = CamionEstado.Cerrado;
            this.FechaModificacion = DateTime.Now;
            this.FechaCierre = DateTime.Now;
            this.NumeroInterfazEjecucionCierre = -1;
        }
        public virtual void IniciarCierre()
        {
            this.Estado = CamionEstado.IniciandoCierre;
            this.FechaModificacion = DateTime.Now;
        }
        public virtual List<int> GetEmpresas()
        {
            return this.Cargas.GroupBy(d => d.Empresa).Select(d => d.Key).ToList();
        }
        public virtual void PrepararFacturacion()
        {
            this.NumeroInterfazEjecucionFactura = -1;
            this.FechaFacturacion = DateTime.Now;
        }
        public virtual void MarcarComoNoFacturable()
        {
            if (this.NumeroInterfazEjecucionFactura != -1)
                this.NumeroInterfazEjecucionFactura = 0;
        }
        public virtual List<string> GetClientes(int empresa)
        {
            return this.Cargas.Where(d => d.Empresa == empresa).GroupBy(d => d.Cliente).Select(d => d.Key).ToList();
        }


        //Api
        public virtual void SetPedidoCamion(string nuPedido, string cdAgente, string tpAgente, int empresa)
        {
            if (DetalleArmadoEgreso.Pedidos == null)
                DetalleArmadoEgreso.Pedidos = new List<ArmadoEgresoCamionPedido>();

            DetalleArmadoEgreso.Pedidos.Add(new ArmadoEgresoCamionPedido
            {
                NroPedido = nuPedido,
                CodigoAgente = cdAgente,
                TipoAgente = tpAgente,
                Empresa = empresa,
                Existe = true,
            });
        }
        public virtual void SetCargaCamion(long carga, string cdAgente, string tpAgente, int empresa)
        {
            if (DetalleArmadoEgreso.Cargas == null)
                DetalleArmadoEgreso.Cargas = new List<ArmadoEgresoCamionCarga>();

            DetalleArmadoEgreso.Cargas.Add(new ArmadoEgresoCamionCarga
            {
                Carga = carga,
                CodigoAgente = cdAgente,
                TipoAgente = tpAgente,
                Empresa = empresa,
                Existe = true,
            });
        }
        public virtual void SetContenedorCamion(string idExterno, string tipoContenedor, int empresa)
        {
            if (DetalleArmadoEgreso.Contenedores == null)
                DetalleArmadoEgreso.Contenedores = new List<ArmadoEgresoCamionContenedor>();

            DetalleArmadoEgreso.Contenedores.Add(new ArmadoEgresoCamionContenedor
            {
                IdExternoContenedor = idExterno,
                TipoContenedor = string.IsNullOrEmpty(tipoContenedor) ? BarcodeDb.TIPO_CONTENEDOR_W : tipoContenedor,
                Empresa = empresa,
                Existe = true,
            });
        }

        public virtual int GetCountPedidos()
        {
            return DetalleArmadoEgreso?.Pedidos?.Count ?? 0;
        }
        public virtual int GetCountCargas()
        {
            return DetalleArmadoEgreso?.Cargas?.Count ?? 0;
        }
        public virtual int GetCountContenedores()
        {
            return DetalleArmadoEgreso?.Contenedores?.Count ?? 0;
        }

        public virtual bool ForzarHabilitarArmado()
        {
            return !(DetalleArmadoEgreso?.Pedidos?.Any(p => p.Existe) ?? false)
                && !(DetalleArmadoEgreso?.Cargas?.Any(ca => ca.Existe) ?? false)
                && !(DetalleArmadoEgreso?.Contenedores?.Any(co => co.Existe) ?? false);
        }
        #endregion
    }

    public class ArmadoEgresoDetalle
    {
        public List<ArmadoEgresoCamionPedido> Pedidos { get; set; }
        public List<ArmadoEgresoCamionCarga> Cargas { get; set; }
        public List<ArmadoEgresoCamionContenedor> Contenedores { get; set; }

        public ArmadoEgresoDetalle()
        {
            Pedidos = new List<ArmadoEgresoCamionPedido>();
            Cargas = new List<ArmadoEgresoCamionCarga>();
            Contenedores = new List<ArmadoEgresoCamionContenedor>();
        }
    }

    public class ArmadoEgresoCamionPedido
    {
        public string NroPedido { get; set; }
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public int Empresa { get; set; }
        public bool Existe { get; set; }
    }

    public class ArmadoEgresoCamionCarga
    {
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public int Empresa { get; set; }
        public long Carga { get; set; }
        public bool Existe { get; set; }

    }

    public class ArmadoEgresoCamionContenedor
    {
        public string IdExternoContenedor { get; set; }
        public string TipoContenedor { get; set; }
        public int Empresa { get; set; }
        public bool Existe { get; set; }

    }
}
