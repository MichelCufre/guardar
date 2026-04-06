using System.Linq;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Produccion
{
    public class FormulaAccionMapper : Mapper
    {
        public virtual FormulaAccion MapEntityToObject(T_PRDC_ACCION_INSTANCIA entity)
        {
            if (entity == null)
                return null;

            var accion = new FormulaAccion
            {
                Id = entity.CD_ACCION_INSTANCIA,
                Descripcion = entity.DS_ACCION_INSTANCIA,
                Tipo = this.MapAccionTipo(entity.CD_ACCION),
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };

            accion.Parametros.Add(entity.VL_PARAMETRO1);
            accion.Parametros.Add(entity.VL_PARAMETRO2);
            accion.Parametros.Add(entity.VL_PARAMETRO3);

            return accion;
        }

        public virtual T_PRDC_ACCION_INSTANCIA MapObjetcToEntity(FormulaAccion accion)
        {
            return new T_PRDC_ACCION_INSTANCIA()
            {
                CD_ACCION = this.MapAccionTipo(accion.Tipo),
                CD_ACCION_INSTANCIA = accion.Id,
                DS_ACCION_INSTANCIA = accion.Descripcion,
                DT_ADDROW = accion.FechaAlta,
                DT_UPDROW = accion.FechaModificacion,
                VL_PARAMETRO1 = accion.Parametros.ElementAtOrDefault(0) != null ? accion.Parametros[0] : null,
                VL_PARAMETRO2 = accion.Parametros.ElementAtOrDefault(1) != null ? accion.Parametros[1] : null,
                VL_PARAMETRO3 = accion.Parametros.ElementAtOrDefault(2) != null ? accion.Parametros[2] : null
            };
        }

        public virtual FormulaAccionTipo MapAccionTipo(string tipo)
        {
            switch (tipo)
            {
                case "LEERDATO": return FormulaAccionTipo.LeerDato;
                case "LEERVALUN": return FormulaAccionTipo.LeerValorUnico;
                case "EMITREP": return FormulaAccionTipo.EmitirReporte;
                case "EJEC_PRC": return FormulaAccionTipo.EjecutarProcedimiento;
                case "QTPASADAS": return FormulaAccionTipo.LeerCantidadPasadas;
            }

            return FormulaAccionTipo.Unknown;
        }

        public virtual string MapAccionTipo(FormulaAccionTipo tipo)
        {
            switch (tipo)
            {
                case FormulaAccionTipo.LeerDato: return "LEERDATO";
                case FormulaAccionTipo.LeerValorUnico: return "LEERVALUN";
                case FormulaAccionTipo.EmitirReporte: return "EMITREP";
                case FormulaAccionTipo.EjecutarProcedimiento: return "EJEC_PRC";
                case FormulaAccionTipo.LeerCantidadPasadas: return "QTPASADAS";
            }

            return "";
        }
    }
}
