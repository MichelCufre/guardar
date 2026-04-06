using NLog;
using WIS.Domain.DataModel;
using WIS.Domain.Picking;
using WIS.Security;

namespace WIS.Domain.Logic
{
    public class LColasDeTrabajo
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly Logger _logger;

        public LColasDeTrabajo(IUnitOfWork uow, IIdentityService identity, Logger logger)
        {
            this._uow = uow;
            this._identity = identity;
            this._logger = logger;
        }

        public virtual void EditarPonderadorDetalle(int nuColaDeTrabajo, int nuPonderacion, string ponderador, string operacion, string key)
        {

            ColaDeTrabajoPonderador colaTrabajoPonderador = null;
            colaTrabajoPonderador = _uow.ColaDeTrabajoRepository.GetPonderador(nuColaDeTrabajo, ponderador);

            if (colaTrabajoPonderador == null)
            {
                PonderadorInstancia pondDefault = _uow.ColaDeTrabajoRepository.GetPonderadorDefault(ponderador);

                colaTrabajoPonderador = new ColaDeTrabajoPonderador
                {
                    Habilitado = pondDefault.Habilitado,
                    Incremento = pondDefault.IncrementoDefault,
                    Numero = nuColaDeTrabajo,
                    Ponderador = ponderador
                };

                _uow.ColaDeTrabajoRepository.AddPonderador(colaTrabajoPonderador);
            }

            _uow.SaveChanges();

            var det = _uow.ColaDeTrabajoRepository.GetPonderadorDetalle(nuColaDeTrabajo, ponderador, key);

            if (det == null)
            {
                det = new ColaDeTrabajoPonderadorDetalle
                {
                    Ponderador = ponderador,
                    Numero = nuColaDeTrabajo,
                    Instancia = key,
                    NuPonderacion = nuPonderacion,
                    Operacion = operacion
                };

                _uow.ColaDeTrabajoRepository.AddPonderadorDetalle(det);
            }
            else
            {
                det.NuPonderacion = nuPonderacion;
                _uow.ColaDeTrabajoRepository.UpdatePonderadorDetalle(det);
            }
        }

        public virtual void DeletePonderadorDetalle(IUnitOfWork uow, int nuColaDeTrabajo, string instanciaPonderador, string ponderador)
        {
            ColaDeTrabajoPonderadorDetalle det = null;

            det = uow.ColaDeTrabajoRepository.GetPonderadorDetalle(nuColaDeTrabajo, ponderador, instanciaPonderador);

            if (det != null)
            {
                uow.ColaDeTrabajoRepository.RemovePonderadorDetalle(det);
            }

            uow.SaveChanges();
        }
    }
}
