using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ValidezMapper : Mapper
    {
        public virtual Validez MapToObject(T_VALIDEZ valida)
        {
            return new Validez
            {
                Id = valida.CD_VALIDEZ,
                Descripcion = valida.DS_VALIDEZ,
                DiasDuracion = valida.QT_DIAS_DURACAO,
                DiasValidez = valida.QT_DIAS_VALIDADE,
                DiasValidezLibracion = valida.QT_DIAS_VALIDADE_LIBERACION
            };
        }

        public virtual T_VALIDEZ MapToEntity(Validez validez)
        {
            return new T_VALIDEZ
            {
                CD_VALIDEZ = validez.Id,
                DS_VALIDEZ = validez.Descripcion,
                QT_DIAS_DURACAO = validez.DiasDuracion,
                QT_DIAS_VALIDADE = validez.DiasValidez,
                QT_DIAS_VALIDADE_LIBERACION = validez.DiasValidezLibracion
            };
        }
        

    }
}
