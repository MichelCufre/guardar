using System;
using System.Collections.Generic;
using WIS.Domain.Documento;

namespace WIS.Domain.Picking
{
    public class Preparacion
    {
        public int Id { get; set; }                                   //NU_PREPARACION
        public string Descripcion { get; set; }                       //DS_PREPARACION
        public DateTime? FechaInicio { get; set; }                    //DT_INICIO
        public DateTime? FechaFin { get; set; }                       //DT_FIN
        public int? Usuario { get; set; }                             //CD_FUNCIONARIO
        public int? Empresa { get; set; }                             //CD_EMPRESA
        public string IdAviso { get; set; }                           //ID_AVISO
        public string Tipo { get; set; }                            //TP_PREPARACION
        public short? Situacion { get; set; }                         //CD_SITUACAO
        public string CodigoContenedorValidado { get; set; }         //CD_CONTENEDOR_VALIDACION
        public bool PrepararSoloConCamion { get; set; }               //FL_PREPARAR_SOLO_CON_CAMION
        public bool PickingEsAgrupadoPorCamion { get; set; }          //FL_PICK_AGRUPADO_POR_CAMION
        public bool RespetarFifoEnLoteAUTO { get; set; }              //FL_RESPETAR_FIFO_EN_LOTE_AUTO
        public bool AceptaMercaderiaAveriada { get; set; }            //FL_MERCADERIA_AVERIADA
        public bool DebeLiberarPorUnidades { get; set; }              //FL_LIBERAR_POR_UNIDADES
        public bool DebeLiberarPorCurvas { get; set; }                //FL_LIBERAR_POR_CURVAS
        public bool ControlaStockDocumental { get; set; }             //FL_CONTROLA_STOCK_DOCUMENTO
        public string ModalPalletCompleto { get; set; }               //FL_MODAL_PALLET_COMPLETO
        public string ModalPalletIncompleto { get; set; }             //FL_MODAL_PALLET_INCOMPLETO
        public string Predio { get; set; }                            //NU_PREDIO
        public string RepartirEscasez { get; set; }                   //VL_REPARTIR_ESCASEZ
        public decimal? PorcentajeRepartoComun { get; set; }          //VL_PORCENTAJE_REPARTO_COMUN
        public short? Onda { get; set; }                              //CD_ONDA
        public decimal? CantidadRechazo { get; set; }                 //QT_RECHAZOS
        public short? CodigoDestino { get; set; }                     //CD_DESTINO
        public string TipoDocumento { get; set; }                     //TP_DOCUMENTO
        public string NumeroDocumemto { get; set; }                   //NU_DOCUMENTO
        public int? UsuarioAsignado { get; set; }                     //CD_FUNCIONARIO_ASIGNADO
        public long? Transaccion { get; set; }                        //NU_TRANSACCION
        public long? TransaccionDelete { get; set; }                  //NU_TRANSACCION_DELETE
        public string Agrupacion { get; set; }                        //ID_AGRUPACION
        public string CursorStock { get; set; }                       //VL_CURSOR_STOCK
        public string CursorPedido { get; set; }                      //VL_CURSOR_PEDIDO
        public bool PriozarDesborde { get; set; }                     //FL_PRIORIZAR_DESBORDE
        public bool ManejaVidaUtil { get; set; }                      //FL_VENTANA_POR_CLIENTE
        public short? ValorVidaUtil { get; set; }                     //VL_PORCENTAJE_VENTANA
        public bool RequiereUbicacion { get; set; }                   //FL_REQUIERE_UBICACION
        public bool Simulacro { get; set; }                           //FL_SIMULACRO
        public bool PriorizarLotePick { get; set; }                   //FL_PRIORIZAR_LOTE_PICK
        public bool UsarSoloStkPicking { get; set; }                  //FL_USAR_SOLO_STK_PICKING
        public bool ExcluirUbicacionesPicking { get; set; }           //FL_EXCLUIR_UBICACIONES_PICKING
        public bool PermitePickVencido { get; set; }            //FL_PICK_MANUAL_VENCIDO
        public bool ValidarProductoProveedor { get; set; }            //FL_VALIDAR_PRODUCTO_PROVEEDOR
        public string FlPermitePickVencido { get; set; }
        public string FlAceptaMercaderiaAveriada { get;  set; }

        public List<DocumentoLiberable> Documentos { get; set; }

        public List<DetallePreparacion> Lineas { get; set; }

        public Preparacion()
        {
            this.Documentos = new List<DocumentoLiberable>();
            this.Lineas = new List<DetallePreparacion>();
        }
    }
}
