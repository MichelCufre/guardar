using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class AnulacionMapper
    {
        public virtual T_ANULACION_PREPARACION MapToEntity(AnulacionPreparacion obj)
        {
            return new T_ANULACION_PREPARACION
            {
                NU_ANULACION_PREPARACION = obj.NroAnulacionPreparacion,
                NU_PREPARACION = obj.Preparacion,
                ND_ESTADO = obj.Estado,
                DS_ANULACION = obj.Descripcion,
                DT_ADDROW = obj.Alta,
                DT_UPDROW = obj.Modificacion,
                TP_ANULACION = obj.TipoAnulacion,
                TP_AGRUPACION = obj.TipoAgrupacion,
                USERID = obj.UserId                
            };
        }
    }
}
