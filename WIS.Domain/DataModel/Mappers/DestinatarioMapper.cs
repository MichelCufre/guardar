using WIS.Domain.Eventos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class DestinatarioMapper : Mapper
    {
        public DestinatarioMapper()
        {

        }

        public virtual Contacto MapContactoToObject(T_CONTACTO entity)
        {
            var obj = new Contacto
            {
                Id = entity.NU_CONTACTO,
                CodigoCliente = entity.CD_CLIENTE,
                CodigoEmpresa = entity.CD_EMPRESA,
                Email = entity.DS_EMAIL,
                Nombre = entity.NM_CONTACTO,
                Descripcion = entity.DS_CONTACTO,
                Telefono = entity.NU_TELEFONO,
                IdUsuario = entity.USERID,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
            return obj;
        }

        public virtual Contacto MapContactoToObject(V_CONTACTOS_INSTANCIA entity)
        {
            var obj = new Contacto
            {
                Id = entity.NU_CONTACTO,
                CodigoCliente = entity.CD_CLIENTE,
                CodigoEmpresa = entity.CD_EMPRESA,
                Email = entity.DS_EMAIL,
                Nombre = entity.NM_CONTACTO,
                Descripcion = entity.DS_CONTACTO,
                Telefono = entity.NU_TELEFONO,
                IdUsuario = entity.USERID,
            };
            return obj;
        }

        public virtual T_CONTACTO MapContactoToEntity(Contacto obj)
        {
            var entity = new T_CONTACTO
            {
                NU_CONTACTO = obj.Id,
                CD_CLIENTE = obj.CodigoCliente,
                CD_EMPRESA = obj.CodigoEmpresa,
                DS_CONTACTO = obj.Descripcion,
                NM_CONTACTO = obj.Nombre,
                DS_EMAIL = obj.Email,
                NU_TELEFONO = NullIfEmpty(obj.Telefono),
                USERID = obj.IdUsuario,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
            };

            return entity;
        }

        public virtual GrupoContacto MapGrupoToObject(T_CONTACTO_GRUPO entity)
        {
            var obj = new GrupoContacto
            {
                Id = entity.NU_CONTACTO_GRUPO,
                Nombre = entity.NM_GRUPO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };

            return obj;
        }

        public virtual ContactoGrupo MapToObject(T_CONTACTO_GRUPO_REL obj)
        {
            var entity = new ContactoGrupo
            {
                NumeroContactoGrupo = obj.NU_CONTACTO_GRUPO,
                NumeroContacto = obj.NU_CONTACTO,
                FechaAlta = obj.DT_ADDROW,
                FechaModificacion = obj.DT_UPDROW,
                NumeroTransaccion = obj.NU_TRANSACCION,
                NumeroTransaccionDelete = obj.NU_TRANSACCION_DELETE,
            };

            return entity;
        }

        public virtual T_CONTACTO_GRUPO MapGrupoToEntity(GrupoContacto obj)
        {
            var entity = new T_CONTACTO_GRUPO
            {
                NU_CONTACTO_GRUPO = obj.Id,
                NM_GRUPO = obj.Nombre,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
            };

            return entity;
        }

        public virtual T_CONTACTO_GRUPO_REL MapToEntity(ContactoGrupo obj)
        {
            var entity = new T_CONTACTO_GRUPO_REL
            {
                NU_CONTACTO_GRUPO = obj.NumeroContactoGrupo,
                NU_CONTACTO = obj.NumeroContacto,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
            };

            return entity;
        }

        public virtual DestinatarioInstancia MapDestinatarioInstanciaToObject(T_EVENTO_GRUPO_INSTANCIA_REL entity)
        {
            var obj = new DestinatarioInstancia
            {
                Id = entity.NU_INSTANCIA_GRUPO,
                NumeroInstancia = entity.NU_EVENTO_INSTANCIA,
                NumeroContacto = entity.NU_CONTACTO,
                NumeroGrupo = entity.NU_CONTACTO_GRUPO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE
            };

            return obj;
        }

        public virtual T_EVENTO_GRUPO_INSTANCIA_REL MapDestinatarioInstanciaToEntity(DestinatarioInstancia obj)
        {
            var entity = new T_EVENTO_GRUPO_INSTANCIA_REL
            {
                NU_INSTANCIA_GRUPO = obj.Id,
                NU_EVENTO_INSTANCIA = obj.NumeroInstancia,
                NU_CONTACTO = obj.NumeroContacto,
                NU_CONTACTO_GRUPO = obj.NumeroGrupo,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete
            };

            return entity;
        }

    }
}
