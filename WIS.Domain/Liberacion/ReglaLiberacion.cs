using System;
using System.Collections.Generic;

namespace WIS.Domain.Liberacion
{
    public partial class ReglaLiberacion
    {
        public ReglaLiberacion()
        {
            LstReglaCliente = new HashSet<ReglaCliente>();
            LstReglaCondicionLiberacion = new HashSet<ReglaCondicionLiberacion>();
            LstReglaTipoExpedicion = new HashSet<ReglaTipoExpedicion>();
            LstReglaTipoPedido = new HashSet<ReglaTipoPedido>();
        }
        public int NuRegla { get; set; }                  //NU_REGLA
        public string DsRegla { get; set; }               //DS_REGLA
        public bool FlActiva { get; set; }                //FL_ACTIVE
        public short? NuOrden { get; set; }               //NU_ORDEN
        public DateTime? DtInicio { get; set; }           //DT_INICIO
        public DateTime? DtFin { get; set; }              //DT_FIN
        public string DsDias { get; set; }                //DS_DIAS
        public TimeSpan? HrInicio { get; set; }           //HR_INICIO
        public TimeSpan? HrFin { get; set; }              //HR_FIN
        public int? NuFrecuencia { get; set; }            //NU_FRECUENCIA
        public string TpFrecuencia { get; set; }          //TP_FRECUENCIA
        public string CdPalletCompeto { get; set; }       //CD_PALLET_COMPLETO
        public string CdStock { get; set; }               //CD_STOCK
        public string CdControlaStock { get; set; }       //CD_CONTROLA_STOCK
        public string CdpalletIncompleto { get; set; }    //CD_PALLET_INCOMPLETO
        public string CdOrdenPedidos { get; set; }        //CD_ORDEN_PEDIDOS
        public string CdRespetarFifo { get; set; }        //CD_RESPETAR_FIFO
        public string CdPrepararSoloCamion { get; set; }  //CD_PREPARAR_SOLO_CAMION
        public string CdRepartirEscasez { get; set; }     //CD_REPARTIR_ESCASEZ
        public string CdLiberarPorUnidad { get; set; }    //CD_LIBERAR_POR_UNIDAD
        public string CdAgruparPorCamion { get; set; }    //CD_AGRUPAR_POR_CAMION
        public string CdAgrupacion { get; set; }          //CD_AGRUPACION
        public string CdLiberarPorCurvas { get; set; }    //CD_LIBERAR_POR_CURVAS
        public DateTime? DtAddRow { get; set; }           //DT_ADDROW
        public DateTime? DtUpdRow { get; set; }           //DT_UPDROW
        public int CdEmpresa { get; set; }                //CD_EMPRESA
        public short CdOnda { get; set; }                 //CD_ONDA
        public string TpAgente { get; set; }              //TP_AGENTE
        public string NuPredio { get; set; }              //NU_PREDIO
        public DateTime? DtUltimaEjecucion { get; set; }  //DT_ULTIMA_EJECUCION
        public string CdOrdenPedidosAuto { get; set; }    //CD_ORDEN_PEDIDOS_AUTO
        public short? NuClisPorPreparacion { get; set; }  //NU_CLIS_POR_PREPARACION
        public bool FlSoloPedidosNuevos { get; set; }     //FL_SOLO_PEDIDOS_NUEVOS
        public bool ManejaVidaUtil { get; set; }          //FL_VENTANA_POR_CLIENTE
        public short? ValorVidaUtil { get; set; }         //VL_PORCENTAJE_VENTANA
        public bool PriozarDesborde { get; set; }         //FL_PRIORIZAR_DESBORDE
        public bool RespetarIntervalo { get; set; }       //FL_RESPETAR_INTERVALO
        public bool ExcluirUbicacionesPicking { get; set; } //FL_EXCLUIR_UBICACIONES_PICKING
        public bool UsarSoloStkPicking { get; set; }      //FL_USAR_SOLO_STK_PICKING
        public short? NuDiasColaTrabajo { get; set; }     // NU_DIAS_COLA_TRABAJO

        public virtual ICollection<ReglaCondicionLiberacion> LstReglaCondicionLiberacion { get; set; }

        public virtual ICollection<ReglaTipoExpedicion> LstReglaTipoExpedicion { get; set; }

        public virtual ICollection<ReglaTipoPedido> LstReglaTipoPedido { get; set; }

        public virtual ICollection<ReglaCliente> LstReglaCliente { get; set; }

        public virtual void Enable()
        {
            this.FlActiva = true;
            this.DtUpdRow = DateTime.Now;
        }

        public virtual void Disable()
        {
            this.FlActiva = false;
            this.DtUpdRow = DateTime.Now;
        }
    }
}