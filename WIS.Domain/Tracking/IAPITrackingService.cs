using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Tracking.Validation;
using WIS.Validation;

namespace WIS.Domain.Tracking
{
    public interface IAPITrackingService
    {
        public TrackingValidationResult AddAgente(AgenteRequest agente);

        public TrackingValidationResult AddTipoVehiculo(TipoVehiculoRequest tipo);
        public TrackingValidationResult UpdateTipoVehiculo(TipoVehiculoRequest tipo);

        public TrackingValidationResult AddVehiculo(VehiculoRequest vehiculo);
        public TrackingValidationResult UpdateVehiculo(VehiculoRequest vehiculo);

        public TrackingValidationResult AddPuntoEntrega(PuntoDeEntregaRequest p, out string cdPuntoEntrega, out string cdZona);

        public TrackingValidationResult AddTarea(TareaRequest tarea);

        public TrackingValidationResult AddUser(UserRequest usuario);
        public TrackingValidationResult UpdateUser(UserRequest usuario);

        public TrackingValidationResult CerrarPedido(AnularPedidoRequest tareaReferencia);

        public TrackingValidationResult CrearViaje(ViajeTeoricoRequest viaje);
        public TrackingValidationResult ConfirmarViaje(ViajeRealRequest viaje);

        public bool GetAgenteTest(Dictionary<string, string> config);
        public bool TrackingHabilitado();
        public Dictionary<string, string> GetConfig();
        public TrackingValidationResult EnviarTareaConAcciones(List<PlanificacionRequest> request);
        public TrackingValidationResult ModificarObjeto(ModificarObjetosRequest r, out ModificarObjetosResponse res);
    }
}
