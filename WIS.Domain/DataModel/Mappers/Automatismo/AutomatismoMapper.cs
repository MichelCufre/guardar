using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Automatismo
{
    public class AutomatismoMapper : Mapper
    {
        protected readonly AutomatismoPosicionMapper _mapperPosicion;
        protected readonly AutomatismoInterfazMapper _mapperInterfaz;
        protected readonly AutomatismoCaracteristicaMapper _mapperCaracteristica;
        protected readonly AutomatismoPuestoMapper _mapperPuesto;
        protected readonly IAutomatismoFactory _automatismoFactory;

        public AutomatismoMapper(IAutomatismoFactory automatismoFactory)
        {
            this._mapperPuesto = new AutomatismoPuestoMapper();
            this._mapperPosicion = new AutomatismoPosicionMapper();
            this._mapperInterfaz = new AutomatismoInterfazMapper();
            this._mapperCaracteristica = new AutomatismoCaracteristicaMapper();
            this._automatismoFactory = automatismoFactory;
        }

        public virtual IAutomatismo Map(T_AUTOMATISMO entity, bool properties = true)
        {
            if (entity == null) return null;
            var automatismo = this._automatismoFactory.Create(entity.ND_TP_AUTOMATISMO);

            if (automatismo != null)
            {
                automatismo.Numero = entity.NU_AUTOMATISMO;
                automatismo.Codigo = entity.CD_AUTOMATISMO;
                automatismo.Descripcion = entity.DS_AUTOMATISMO;
                automatismo.Tipo = entity.ND_TP_AUTOMATISMO;
                automatismo.Predio = entity.NU_PREDIO;
                automatismo.ZonaUbicacion = entity.CD_ZONA_UBICACION;
                automatismo.IsEnabled = MapStringToBoolean(entity.FL_HABILITADO);
                automatismo.FechaRegistro = entity.DT_ADDROW;
                automatismo.FechaModificacion = entity.DT_UDPROW;
                automatismo.Transaccion = entity.NU_TRANSACCION;

                if (properties)
                {
                    automatismo.Puestos = this._mapperPuesto.Map(entity.T_AUTOMATISMO_PUESTO.ToList());
                    automatismo.Posiciones = this._mapperPosicion.Map(entity.T_AUTOMATISMO_POSICION.ToList());
                    automatismo.Interfaces = this._mapperInterfaz.Map(entity.T_AUTOMATISMO_INTERFAZ.ToList());
                    automatismo.Caracteristicas = this._mapperCaracteristica.Map(entity.T_AUTOMATISMO_CARACTERISTICA.ToList());
                }
            }
            return automatismo;
        }
        public virtual T_AUTOMATISMO Map(IAutomatismo obj)
        {
            if (obj == null) return null;

            return new T_AUTOMATISMO()
            {
                NU_AUTOMATISMO = obj.Numero,
                CD_AUTOMATISMO = obj.Codigo?.ToUpper(),
                DS_AUTOMATISMO = obj.Descripcion,
                CD_ZONA_UBICACION = obj.ZonaUbicacion,
                DT_ADDROW = obj.FechaRegistro,
                DT_UDPROW = obj.FechaModificacion,
                NU_PREDIO = obj.Predio,
                NU_TRANSACCION = obj.Transaccion,
                FL_HABILITADO = MapBooleanToString(obj.IsEnabled),
                ND_TP_AUTOMATISMO = obj.Tipo,
            };
        }

        public virtual List<IAutomatismo> Map(List<T_AUTOMATISMO> entities)
        {
            List<IAutomatismo> result = null;
            if (entities != null)
            {
                result = new List<IAutomatismo>();
                entities.ForEach(ent =>
                {
                    IAutomatismo elemnt = this.Map(ent);
                    if (elemnt != null)
                    {
                        result.Add(elemnt);
                    }
                });
            }
            return result;
        }
        public virtual List<T_AUTOMATISMO> Map(List<IAutomatismo> objs)
        {
            List<T_AUTOMATISMO> entities = new List<T_AUTOMATISMO>();

            foreach (var item in objs) entities.Add(Map(item));

            return entities;
        }

        public virtual IAutomatismo MapToObject(T_AUTOMATISMO entity, bool properties = true)
        {
            var automatismo = this._automatismoFactory.Create(entity.ND_TP_AUTOMATISMO);

            if (automatismo != null)
            {
                automatismo.Numero = entity.NU_AUTOMATISMO;
                automatismo.Codigo = entity.CD_AUTOMATISMO;
                automatismo.Descripcion = entity.DS_AUTOMATISMO;
                automatismo.Tipo = entity.ND_TP_AUTOMATISMO;
                automatismo.Predio = entity.NU_PREDIO;
                automatismo.ZonaUbicacion = entity.CD_ZONA_UBICACION;
                automatismo.IsEnabled = MapStringToBoolean(entity.FL_HABILITADO);
                automatismo.FechaRegistro = entity.DT_ADDROW;
                automatismo.FechaModificacion = entity.DT_UDPROW;
                automatismo.Transaccion = entity.NU_TRANSACCION;

                if (properties)
                {
                    automatismo.Puestos = this._mapperPuesto.Map(entity.T_AUTOMATISMO_PUESTO.ToList());
                    automatismo.Posiciones = this._mapperPosicion.Map(entity.T_AUTOMATISMO_POSICION.ToList());
                    automatismo.Interfaces = this._mapperInterfaz.Map(entity.T_AUTOMATISMO_INTERFAZ.ToList());
                    automatismo.Caracteristicas = this._mapperCaracteristica.Map(entity.T_AUTOMATISMO_CARACTERISTICA.ToList());
                }
            }
            return automatismo;
        }
        public virtual UbicacionPickingZonaAutomatismo MapToObject(V_PRODUCTOS_ASOCIADOS_AUTOMATISMO entity)
        {
            if (entity == null)
                return null;

            return new UbicacionPickingZonaAutomatismo()
            {
                Ubicacion = entity.CD_ENDERECO_SEPARACAO,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Zona = entity.CD_ZONA_UBICACION,
                ConfirmarCodigoBarrasAut = MapStringToBoolean(entity.FL_CONF_CD_BARRAS_AUT),
                UnidadCajaAut = entity.CD_UNIDAD_CAJA_AUT,
                CantidadUnidadCajaAut = entity.QT_UNIDAD_CAJA_AUT
            };

        }
    }
}
