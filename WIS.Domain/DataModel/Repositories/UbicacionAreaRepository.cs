using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class UbicacionAreaRepository
    {
        protected readonly int _userId;
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly UbicacionAreaMapper _mapper;
        protected readonly UbicacionAreaMapper _ubicacionAreaMapper;

        public UbicacionAreaRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new UbicacionAreaMapper();
            this._ubicacionAreaMapper = new UbicacionAreaMapper();
        }

        #region Any

        public virtual bool AnyUbicacionArea(short id)
        {
            return this._context.T_TIPO_AREA
                .AsNoTracking()
                .Any(d => d.CD_AREA_ARMAZ == id);
        }

        public virtual bool AnyUbicacionAreaMantenible(short id)
        {
            return this._context.T_TIPO_AREA
                .AsNoTracking()
                .Any(d => d.ID_PERMITE_MANTENIMIENTO == "S" 
                    && d.CD_AREA_ARMAZ == id);
        }

        public virtual bool AnyUbicacionAreaAlmacenable(short id)
        {
            return this._context.T_TIPO_AREA
                .AsNoTracking()
                .Any(d => d.ID_PERMITE_ALMACENAR == "S" 
                    && d.ID_AREA_PICKING == "N"
                    && d.CD_AREA_ARMAZ == id);
        }

        #endregion

        #region Get
        public virtual UbicacionArea GetUbicacionArea(short codigo)
        {
            var entity = this._context.T_TIPO_AREA
                .AsNoTracking()
                .Where(d => d.CD_AREA_ARMAZ == codigo)
                .FirstOrDefault();

            return this._mapper.MapToObject(entity);
        }

        public virtual List<UbicacionArea> GetUbicacionAreas()
        {
            var entities = this._context.T_TIPO_AREA
                .AsNoTracking()
                .ToList();

            var areas = new List<UbicacionArea>();

            foreach (var entity in entities)
            {
                areas.Add(this._mapper.MapToObject(entity));
            }

            return areas;
        }

        public virtual List<UbicacionArea> GetUbicacionAreasMantenibles()
        {
            var entities = this._context.T_TIPO_AREA
                .AsNoTracking()
                .Where(s => s.ID_PERMITE_MANTENIMIENTO == "S")
                .ToList();

            var areas = new List<UbicacionArea>();

            foreach (var entity in entities)
            {
                areas.Add(this._mapper.MapToObject(entity));
            }

            return areas;
        }

        public virtual List<UbicacionArea> GetUbicacionAreasPermiteAlmacenar()
        {
            var entities = this._context.T_TIPO_AREA
                .AsNoTracking()
                .Where(s => s.ID_PERMITE_ALMACENAR == "S"
                    && s.ID_AREA_PICKING == "N")
                .ToList();

            var areas = new List<UbicacionArea>();

            foreach (var entity in entities)
            {
                areas.Add(this._mapper.MapToObject(entity));
            }

            return areas;
        }

        public virtual List<short> GetAreasPermitidasParaCambiarVencimiento()
        {
            return this._context.T_TIPO_AREA.AsNoTracking().Where(s => s.ID_AREA_AVARIA == "S" || s.ID_AREA_PICKING == "S" || s.ID_ESTOQUE_GERAL == "S").Select(s => s.CD_AREA_ARMAZ).ToList();
        }

        public virtual short? GetAreaByUbicacion(string ubicacion)
        {
            return this._context.T_ENDERECO_ESTOQUE.FirstOrDefault(d => d.CD_ENDERECO == ubicacion)?.CD_AREA_ARMAZ;
        }

        public virtual UbicacionArea GetTipoAreaByUbicacion(string cdubicacion)
        {
            var ubicacion = this._context.T_ENDERECO_ESTOQUE.Include("T_TIPO_AREA").FirstOrDefault(d => d.CD_ENDERECO == cdubicacion);
            if (ubicacion != null)
            {
                return this._ubicacionAreaMapper.MapToObject(ubicacion.T_TIPO_AREA);
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
