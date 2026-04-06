using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class SituacionDb
    {
        public const short EnProceso = 0;
        public const short AgendaAbierta = 3;
        public const short AgendaCerrada = 4;
        public const short AgendaCancelada = 5;
        public const short DocumentoAbierto = 6;
        public const short AgendaIngresandoFactura = 7;
        public const short AgendaConferidaSinDiferencia = 9;
        public const short AgendaConferidaConDiferencia = 10;
        public const short AgendaAguardandoDesembarque = 11;
        public const short AgendaNfCoincidenAgenda= 12;
        public const short AnuladoCompletamente = 14;
        public const short Activo = 15;
        public const short Inactivo = 16;
        public const short AgendaRecepcionEnProgreso= 18;
        public const short PalletGeneradoAutomaticamente = 20;
        public const short Emitida = 21;
        public const short PalletConferido = 23;
        public const short PalletAlmacenado = 25;
        public const short PalletSinProductos = 29;
        public const short TransferidoAContenedores = 30;

        public const short UbicacionVacia = 42;
        public const short UbicacionEntrando = 45;
        public const short PrepOndaNoIniciada = 54;
        public const short PedidoParcialmenteEnviado = 55;
        public const short PrepOndaIniciada = 57;
        public const short PedidoAnulado = 58;
        public const short PedidoEnviadoExceso = 59;
        public const short PedidoCompletamenteEnviado = 60;
        public const short PedidoAbierto = 64;
        public const short PreparacionExpedido = 68;
        public const short CamionSinOrdenDeTrabajo = 70;
        public const short VehiculoLiberado = 73;
        public const short PreparacionPendiente = 109;
        public const short PreparacionIniciada = 110;
        public const short PreparacionFinalizada = 111;
        public const short HabilitadoParaPickear = 112;
        public const short PENDIENTE_EJECUTAR_CALCULO = 300;
        public const short CALCULO_EJECUTADO = 301;
        public const short CALCULO_CON_ERRORES = 302;
        public const short CALCULO_ACEPTADO = 303;
        public const short CALCULO_RECHAZADO = 304;
        public const short EJECUCION_ENVIADA = 305;
        public const short EJECUCION_REALIZADA = 306;
        public const short EJECUCION_PENDIENTE = 307;
        public const short CALCULO_OMITIDO = 308;
        public const short CALCULO_ENVIADO = 309;
        public const short CALCULO_FACTURADO = 310;
        public const short EJECUCION_EN_PROGRAMACION = 311;
        public const short EJECUCION_ANULADA = 312;
        public const short ContenedorVacio = 600;
        public const short ContenedorEnPreparacion = 601;
        public const short ContenedorEnCamion = 602;
        public const short ContenedorEnviado = 603;
        public const short ContenedorContabilizado = 604;
        public const short ContenedorEnsambladoKit = 605;
        public const short ContenedorTransferido = 606;
        public const short CamionAguardandoCarga = 650;
        public const short CamionCargando = 651;
        public const short CamionCerrado = 652;
        public const short CamionIniciandoCierre = 653;
        public const short SITUACION_PROCESO = 681;
        public const short ProcesamientoIniciado = 740;
        public const short ProcesadoPendiente = 741;
        public const short ProcesadoConError = 742;
        public const short ProcesadoOK = 743;
        public const short ErrorNotificacionAutomatismo = 745;
        public const short EnProcesoNotificacionAutomatismo = 746;
        public const short EjecucionIniciada = 750;
        public const short ArchivoProcesado = 751;
        public const short ProcesandoInterfaz = 752;
        public const short ArchivoRespaldado = 753;
        public const short InterfazEnAprobacionRespaldado = 754;
        public const short ProcesamientoCancelado = 755;
        public const short EjecucionCancelada = 761;
        public const short TransferenciaRealizada = 800;
        public const short EnTransferencia = 801;

        public const short PRODUCCION_CREADA = 620;
        public const short PEDIDO_GENERADO = 621;
        public const short PREPARANDO_PEDIDOS = 622;
        public const short INSUMOS_EN_ESP_DE_PRODUCCION = 623;
        public const short PRODUCCION_INICIADA = 624;
        public const short PRODUCCION_FINALIZADA = 625;
        public const short PRODUCCION_PARCIALMENTE_NOTIF = 626;
        public const short PRODUCCION_PENDIENTE_NOTIFICACION_PARCIAL = 627;
        public const short PRODUCCION_PENDIENTE_NOTIFICACION_FINAL = 628;
        public const short INICIO_ALTA_PRODUCTOS_FINALES = 629;
        public const short PRODUCIENDO = 630;

        public const short UBICACION_ACTIVA = 41;

        public static List<short> SITUACIONES_PRODUCCION_ACTIVA = new List<short>() { PRODUCCION_INICIADA, PRODUCCION_PARCIALMENTE_NOTIF, PRODUCCION_PENDIENTE_NOTIFICACION_PARCIAL, PRODUCIENDO };
        public static bool IsAPI(int? status)
        {
            if (status == null)
                return false;

            switch (status.Value)
            {
                case SituacionDb.ProcesamientoIniciado:
                case SituacionDb.ProcesadoPendiente:
                case SituacionDb.ProcesadoConError:
                case SituacionDb.ProcesadoOK:
                case SituacionDb.ErrorNotificacionAutomatismo:
                case SituacionDb.EnProcesoNotificacionAutomatismo:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsAPI(string status)
        {
            if (int.TryParse(status, out var value))
                return IsAPI(value);
            else
                return false;
        }


    }
}
