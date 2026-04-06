using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.AutomationManager.Interfaces
{
    public interface IAutomatismoService
    {
        IAutomatismo GetByZona(IUnitOfWork uow, string zona);
        IAutomatismo GetByCodigo(IUnitOfWork uow, string codigo);
        List<IAutomatismo> GetAllByTipo(IUnitOfWork uow, string tipo);
        Task<ValidationsResult> NotificarProductos(ProductosAutomatismoRequest request, AutomatismoEjecucion ejecucion);
        Task<ValidationsResult> NotificarCodigosBarras(CodigosBarrasAutomatismoRequest request, AutomatismoEjecucion ejecucion);
        Task<ValidationsResult> NotificarEntrada(EntradaStockAutomatismoRequest request, AutomatismoEjecucion ejecucion);
        Task<ValidationsResult> NotificarSalida(SalidaStockAutomatismoRequest request, AutomatismoEjecucion ejecucion);
        Task<ValidationsResult> ProcesarNotificacionAjusteStock(string puesto, AjustesDeStockRequest request, AutomatismoEjecucion ejecucion);
        Task<ValidationsResult> ProcesarConfirmacionEntrada(string puesto, TransferenciaStockRequest request, AutomatismoEjecucion ejecucion);
        Task<ValidationsResult> ProcesarConfirmacionSalida(string puesto, PickingRequest request, AutomatismoEjecucion ejecucion);
        Task<ValidationsResult> NotificarUbicacionesPicking(UbicacionesPickingAutomatismoRequest request, AutomatismoEjecucion ejecucion);
        Task<ValidationsResult> ProcesarConfirmacionMovimiento(string puesto, ConfirmacionMovimientoStockRequest request, AutomatismoEjecucion ejecucion);
        bool IsValidUser(UsuarioRequest usuario, string puesto);
    }
}
