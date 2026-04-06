using WIS.Domain.Automatismo;
using WIS.Persistence.InMemory;

namespace WIS.Domain.DataModel.Mappers.Automatismo
{
    public class PtlMapper : Mapper
    {
        public virtual PtlColor MapColor(PtlColorEnUsoEntity obj)
        {
            if (obj == null) return null;

            return new PtlColor
            {
                Code = obj.NU_COLOR,
                UserId = obj.UserId
            };

        }

        public virtual PtlPosicionEnUso Map(PtlPosicionEnUsoEntity entity)
        {
            if (entity == null)
                return null;
            return new PtlPosicionEnUso()
            {
                Key = entity.Key,
                Referencia = entity.Referencia,
                Id = entity.Id,
                Ptl = entity.NU_PTL,
                Ubicacion = entity.NU_ADDRESS,
                Orden = entity.NU_ORDEN,
                Color = entity.NU_COLOR,
                Display = entity.Display,
                DisplayFn = entity.DisplayFn,
                FechaRegistro = entity.DT_ADDROW,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUCTO,
                UserId = entity.UserId,
                Estado = entity.CD_ESTADO,
                Detalle = entity.Detail,
                Transaccion = entity.Transaccion,
                Agrupacion = entity.Agrupacion,
            };

        }

        public virtual PtlPosicionEnUsoEntity Map(PtlPosicionEnUso obj)
        {
            return new PtlPosicionEnUsoEntity()
            {
                Referencia = obj.Referencia,
                Id = obj.Id,
                NU_PTL = obj.Ptl,
                NU_ADDRESS = obj.Ubicacion,
                NU_ORDEN = obj.Orden,
                NU_COLOR = obj.Color,
                Display = obj.Display,
                DisplayFn = obj.DisplayFn,
                CD_EMPRESA = obj.Empresa,
                CD_PRODUCTO = obj.Producto,
                UserId = obj.UserId,
                CD_ESTADO = obj.Estado,
                Detail = obj.Detalle,
                Agrupacion = obj.Agrupacion,
            };
        }

        public virtual PtlColorEnUsoEntity Map(PtlColorEnUso color)
        {
            return new PtlColorEnUsoEntity()
            {
                UserId = color.UserId,
                NU_PTL = color.Ptl,
                NU_COLOR = color.Color,
            };
        }

        public virtual PtlColorEnUso Map(PtlColorEnUsoEntity entity)
        {
            if (entity == null)
                return null;
            return new PtlColorEnUso()
            {
                Color = entity.NU_COLOR,
                Ptl = entity.NU_PTL,
                UserId = entity.UserId,
                FechaRegistro = entity.DT_ADDROW,
                FechaUltimaAccion = entity.DT_ULTIMA_ACCION,
                Transaccion = entity.Transaccion,

            };

        }
    }
}
