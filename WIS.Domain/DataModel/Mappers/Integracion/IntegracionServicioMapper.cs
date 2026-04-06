using WIS.Domain.Integracion;
using WIS.Domain.Integracion.Factories;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Integracion
{
    public class IntegracionServicioMapper : Mapper
    {
        protected AutenticacionFactory _autenticacionFactory;

        public IntegracionServicioMapper()
        {
            this._autenticacionFactory = new AutenticacionFactory();
        }

        public virtual IntegracionServicio Map(T_INTEGRACION_SERVICIO entity)
        {
            if (entity == null)
                return null;

            var obj = new IntegracionServicio()
            {
                Numero = entity.NU_INTEGRACION,
                Codigo = entity.CD_INTEGRACION,
                Descripcion = entity.DS_INTEGRACION,
                FechaRegistro = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UDPROW,
                Habilitado = MapStringToBoolean(entity.FL_HABILITADO),
                TipoAutenticacion = entity.ND_TP_AUTH_SRV,
                Scope = entity.VL_SCOPE,
                Secret = entity.VL_SECRET,
                TipoComunicacion = entity.ND_TP_COMUNICACION,
                Transaccion = entity.NU_TRANSACCION,
                SecretFormat = entity.VL_SECRETFORMAT,
                SecretSalt = entity.VL_SECRETSALT,
                UrlAuthServer = entity.VL_URL_AUTH_SERVER,
                UrlIntegracion = entity.VL_URL_INTEGRACION,
                User = entity.VL_USER,
            };

            obj.Authorization = this._autenticacionFactory.Create(obj);

            return obj;
        }

        public virtual T_INTEGRACION_SERVICIO MapToEntity(IntegracionServicio obj)
        {
            return new T_INTEGRACION_SERVICIO()
            {
                NU_INTEGRACION = obj.Numero,
                CD_INTEGRACION = obj.Codigo,
                DS_INTEGRACION = obj.Descripcion,
                DT_ADDROW = obj.FechaRegistro,
                DT_UDPROW = obj.FechaModificacion,
                FL_HABILITADO = MapBooleanToString(obj.Habilitado),
                ND_TP_AUTH_SRV = obj.TipoAutenticacion,
                VL_SCOPE = obj.Scope,
                VL_SECRET = obj.Secret,
                ND_TP_COMUNICACION = obj.TipoComunicacion,
                NU_TRANSACCION = obj.Transaccion,
                VL_SECRETFORMAT = obj.SecretFormat,
                VL_SECRETSALT = obj.SecretSalt,
                VL_URL_AUTH_SERVER = obj.UrlAuthServer,
                VL_URL_INTEGRACION = obj.UrlIntegracion,
                VL_USER = obj.User
            };
        }
    }
}
