using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;
using WIS.Domain.Security;
using WIS.Domain.Tracking.Models;

namespace WIS.Domain.Services.Interfaces
{
    public interface ITrackingService
    {
        void Sync();
        bool TrackingHabilitado();
        void SincronizacionInicial(IUnitOfWork uow);
        Dictionary<string, string> GetConfig();
        bool TestearConexion(Dictionary<string, string> config);
        void SincronizacionInicialPedidos(IUnitOfWork uow, bool bloquear = false);
        void GuardarErrores(IUnitOfWorkFactory uowFactory, string dsReferencia, string error);
        void SincronizarPedido(IUnitOfWork uow, Pedido pedido, Agente agente, bool bloquear);
        void SincronizarUsuario(Usuario usuario, bool bloquear);
        void CerrarPedido(IUnitOfWork uow, Pedido pedido, Agente agente, bool bloquear);
        void SincronizarAgente(Agente agente, bool bloquear);
        void SincronizarVehiculo(IUnitOfWork uow, Vehiculo vehiculo, bool bloquear);
        void SincronizarTipoVehiculo(VehiculoEspecificacion tipoVehiculo, bool bloquear);
        List<ValidacionCamionResultado> ValidarSincronizacion(IUnitOfWork uow, Camion camion);
        void SincronizarEgreso(IUnitOfWork uow, Camion camion, bool confirmarViaje);
        void SincronizarPlanificacion(IUnitOfWork uow, Camion camion);
        void SincronizarPredio(Predio predio);
        void SincronizarDevolucion(IUnitOfWork uow, Agenda agenda, string puntoDeEntrega);
        void CambiarEstadoSincronizacion(IUnitOfWork uow, Camion camion, bool sincronizado);
        void CambiarEstadoSincronizacion(IUnitOfWork uow, IEnumerable<Camion> camiones, bool sincronizado);
        void CambiarEstadoSincronizacion(IUnitOfWork uow, int cdCamion, bool sincronizado);
        void RegularizarEgresosMesaEmpaque(IUnitOfWork uow, EgresoContenedorTracking datosOrigen, EgresoContenedorTracking datosDestino);
        void RegularizarBultosAnulados(IUnitOfWork uow, List<long> cargas);
    }
}
