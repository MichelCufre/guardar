using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class SuperClaseMapper : Mapper
    {
        protected readonly ClaseMapper _claseMapper;

        public SuperClaseMapper(ClaseMapper claseMapper)
        {
            this._claseMapper = claseMapper;
        }
        public virtual SuperClase MapToObject(T_SUB_CLASSE superClase)
        {
            if (superClase == null)
                return null;

            var clases = new List<Clase>();

            foreach (var clase in superClase.T_CLASSE)
            {
                clases.Add(this._claseMapper.MapToObject(clase));
            }

            return new SuperClase
            {
                Id = superClase.CD_SUB_CLASSE,
                Descripcion = superClase.DS_SUB_CLASSE,
                FechaInsercion = superClase.DT_ADDROW,
                FechaModificacion = superClase.DT_UPDROW,
                Clases = clases
            };
        }
        public virtual T_SUB_CLASSE MapToEntity(SuperClase superClase)
        {
            var entity = new T_SUB_CLASSE
            {
                CD_SUB_CLASSE = superClase.Id,
                DS_SUB_CLASSE = superClase.Descripcion,
                DT_ADDROW = superClase.FechaInsercion,
                DT_UPDROW = superClase.FechaModificacion
            };
            return entity;
        }
    }
}
