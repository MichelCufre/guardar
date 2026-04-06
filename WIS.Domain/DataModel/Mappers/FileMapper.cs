using System;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class FileMapper : Mapper
    {

        public virtual T_ARCHIVO MapToEntity(General.File file)
        {
            return new T_ARCHIVO
            {
                CD_ARCHIVO = file.FileId,
                DT_ADDROW = DateTime.Now,
                NM_ARCHIVO = file.FileName,
                VL_SIZE = file.Size,
                TP_ENTIDAD = file.TipoEntidad,
                CD_ENTIDAD = file.CodigoEntidad,
                CD_FUNCIONARIO = file.CodigoFuncionario
            };
        }

        public General.File MapToObject(T_ARCHIVO entity)
        {
            return new General.File()
            {
                FileId = entity.CD_ARCHIVO,
                FileName = entity.NM_ARCHIVO,
                Size = entity.VL_SIZE,
                TipoEntidad = entity.TP_ENTIDAD,
                CodigoEntidad = entity.CD_ENTIDAD,
                CodigoFuncionario = entity.CD_FUNCIONARIO
            };
        }
    }
}
