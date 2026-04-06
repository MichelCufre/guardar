using NLog;
using System.Collections.Generic;
using WIS.Domain.DataModel;

namespace WIS.Domain.Liberacion
{
    public class MantenimientoLiberacion
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public MantenimientoLiberacion(IUnitOfWork uow, int userId, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = userId;
            this._aplicacion = aplicacion;

        }

        public virtual void AgregarReglaClientes(List<ReglaCliente> reglas)
        {
            foreach (var regla in reglas)
            {
                _uow.LiberacionRepository.AddReglaCliente(regla);
            }

        }

        public virtual void DesasociarReglaClientes(List<ReglaCliente> reglas)
        {
            _uow.LiberacionRepository.DeleteReglaCliente(reglas);
        }
    }
}
