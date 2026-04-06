using NLog;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Exceptions;

namespace WIS.Domain.Registro
{
    public class ZonasDeUbicacion
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWork _uow;
        protected string _aplicacion;
        protected int _usuario;
        protected List<string> _zonas;

        public ZonasDeUbicacion(IUnitOfWork uow, int usuario, string aplicacion, List<string> zonas = null)
        {
            this._uow = uow;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._zonas = zonas;
        }

        public virtual void Eliminar()
        {
            foreach (var zonaAux in this._zonas)
            {
                ZonaUbicacion zona = this._uow.ZonaUbicacionRepository.GetZona(zonaAux);
                if (zona == null)
                    throw new EntityNotFoundException("REG070_Sec0_Error_ZonaEliminarNoExiste", new string[] { zonaAux });
                else
                    this._uow.ZonaUbicacionRepository.RemoveZona(zona);
            }
        }
    }
}
