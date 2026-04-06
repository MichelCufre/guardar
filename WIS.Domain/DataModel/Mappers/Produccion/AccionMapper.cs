using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Produccion
{
    public class AccionMapper : Mapper
    {
        public virtual Accion MapEntityToObject(T_PRDC_ACCION entity)
        {
            var accion = new Accion
            {
                Id = entity.CD_ACCION,
                Descripcion = entity.DS_ACCION,
                Tipo = this.MapAccionTipo(entity.TP_ACCION)
            };

            return accion;
        }

        public virtual AccionTipo MapAccionTipo(string tipo)
        {
            switch (tipo)
            {
                case "ING_X_PANT": return AccionTipo.IngresoPorPantalla;
                case "AUTO": return AccionTipo.Automatica;
            }

            return AccionTipo.Unknown;
        }
    }
}
