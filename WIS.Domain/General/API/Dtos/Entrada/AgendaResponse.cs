using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class AgendaResponse
    {
        public int NroAgenda { get; set; }                      //NU_AGENDA
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        /// <summary>
        /// Número de referencia asociada a la agenda
        /// </summary>
        /// <example>238</example>
        [ApiDtoExample("238")]
        public string NumeroDocumento { get; set; }             //NU_DOCUMENTO
        public string FechaInsercion { get; set; }              //DT_ADDROW
        public string FechaModificacion { get; set; }           //DT_UPDROW
        public string Predio { get; set; }                      //NU_PREDIO

        /// <summary>
        ///     Los estados posibles para la agenda son:
        ///     3 Agenda abierta En Edición.
        ///     4 Agenda cerrada (Ya se finalizó y aceptó la recepción).
        ///     11 Agenda habilitada para recepción.
        ///     9 Agenda en proceso de recepción, sin problemas de recepción o con problemas ya aceptados.
        ///     10 Agenda en proceso de recepción, con problemas de recepción.
        /// </summary>
        public short Estado { get; set; }                       //CD_SITUACAO
        public string TipoDocumento { get; set; }               //CD_TIPO_DOCUMENTO

        /// <summary>
        /// Número de puerta donde se recibió o se agendó la recepción.
        /// </summary>
        public short? NroPuerta { get; set; }                //CD_PORTA
        public string FechaInicio { get; set; }                 //DT_INICIO
        public string FechaFin { get; set; }                    //DT_FIN
        public string PlacaVehiculo { get; set; }               //DS_PLACA

        /// <summary>
        ///     DUA o documento aduanero asociado al ingreso de recepción. 
        ///     Por lo general solo aplica en aquellos sistemas que cuentan con algún tipo de integración con Aduanas.
        /// </summary>
        public string DUA { get; set; }                         //NU_DUA
        public string Anexo1 { get; set; }                      //DS_ANEXO1
        public string Anexo2 { get; set; }                      //DS_ANEXO2
        public string Anexo3 { get; set; }                      //DS_ANEXO3
        public string Anexo4 { get; set; }                      //DS_ANEXO4
        public string Averiado { get; set; }                    //ID_AVERIA

        /// <summary>
        /// Marca de tiempo en el momento en que se cerró o finalizó la agenda
        /// </summary>
        public string FechaCierre { get; set; }                 //DT_CIERRE
        /// <summary>
        /// Fecha estimada de entrega de la agenda
        /// </summary>
        public string FechaEntrega { get; set; }                //DT_ENTREGA
        /// <summary>
        ///     Tipo de recepción con el que se creo la agenda.
        /// </summary>
        public string TipoRecepcion { get; set; }               //TP_RECEPCION
        /// <summary>
        ///     Corresponde al número de interfaz generado al procesar el cierre de la agenda.
        /// </summary>
        public long? NumeroInterfazEjecucion { get; set; }      //NU_INTERFAZ_EJECUCION

        public List<AgendaDetalleResponse> Detalles { get; set; }
        public AgendaResponse()
        {
            Detalles = new List<AgendaDetalleResponse>();
        }
    }

    public class AgendaDetalleResponse
    {
        /// <summary>
        ///     Código del producto a recibir
        /// </summary>
        public string CodigoProducto { get; set; }

        //public decimal Faixa { get; set; }

        /// <summary>
        ///     Lote o número que identifica al producto.
        /// </summary>
        public string Identificador { get; set; }

        /// <summary>
        ///     El estado actual del detalle, el mismo varía a medida que la operativa avanza.
        /// </summary>
        public short Estado { get; set; }

        /// <summary>
        ///     Cantidad a recibir en la recepción.
        /// </summary>
        public decimal CantidadAgendada { get; set; }

        /// <summary>
        ///     Cantidad que se recibió para el producto especificado
        /// </summary>
        public decimal CantidadRecibida { get; set; }

        /// <summary>
        ///     Cantidad equivalente a la cantidad recibida cuando se aceptan las diferencias de recepción.
        /// </summary>
        public decimal CantidadAceptada { get; set; }

        /// <summary>
        ///     Cantidad a recibir en una primera instancia más allá de las posibles modificaciones que la cantidad agendada pueda tener.
        /// </summary>
        public decimal CantidadAgendadaOriginal { get; set; }

        /// <summary>
        ///     Cantidad usada para atender los pedidos en cross-docking
        /// </summary>
        public decimal CantidadCrossDocking { get; set; }

        /// <summary>
        ///     Fecha en que vence el producto
        /// </summary>
        public string Vencimiento { get; set; }

        /// <summary>
        ///     Fecha en que se hizo la inserción
        /// </summary>
        public string FechaAlta { get; set; }

        /// <summary>
        ///     La última fecha en que se modificó el detalle.
        /// </summary>
        public string FechaModificacion { get; set; }

        /// <summary>
        ///     Corresponde a la fecha de cuando se aceptan los problemas de la agenda, no siempre puede tener un valor, solo cuando se aceptaron diferencias.
        /// </summary>
        public string FechaAceptacionProblema { get; set; }
        public decimal? PrecioCIF { get; set; }
        public decimal? PrecioFOB { get; set; }
    }
}
