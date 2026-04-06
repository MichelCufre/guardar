using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Expedicion;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class TipoVehiculoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly TipoVehiculoMapper _mapper;
        protected readonly IDapper _dapper;

        public TipoVehiculoRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new TipoVehiculoMapper();
            this._dapper = dapper;
        }

        public virtual void Add(VehiculoEspecificacion especificacion)
        {
            especificacion.Id = this._context.GetNextSequenceValueInt(_dapper, "S_TIPO_VEHICULO");

            T_TIPO_VEICULO entity = this._mapper.MapToEntity(especificacion);

            this._context.T_TIPO_VEICULO.Add(entity);
        }

        public virtual VehiculoEspecificacion GetTipo(int id)
        {
            return this._mapper.MapToObject(this._context.T_TIPO_VEICULO.AsNoTracking().Where(d => d.CD_TIPO_VEICULO == id).FirstOrDefault());
        }

        public virtual List<VehiculoEspecificacion> GetTipos(List<int> ids)
        {
            List<T_TIPO_VEICULO> entities = this._context.T_TIPO_VEICULO.AsNoTracking().Where(d => ids.Contains(d.CD_TIPO_VEICULO)).ToList();

            List<VehiculoEspecificacion> especificaciones = new List<VehiculoEspecificacion>();

            foreach (var entity in entities)
            {
                especificaciones.Add(this._mapper.MapToObject(entity));
            }

            return especificaciones;
        }

        public virtual List<VehiculoEspecificacion> GetTipos()
        {
            return this._context.T_TIPO_VEICULO.AsNoTracking().OrderBy(d => d.CD_TIPO_VEICULO).ToList().Select(d=>this._mapper.MapToObject(d)).ToList();
        }
        public virtual List<VehiculoEspecificacion> GetTiposNoSincronizados()
        {
            return this._context.T_TIPO_VEICULO.AsNoTracking()
            .Where(v => v.FL_SYNC_REALIZADA != "S").OrderBy(d => d.CD_TIPO_VEICULO)
            .Select(d => this._mapper.MapToObject(d)).ToList();
        }
        public virtual List<VehiculoEspecificacion> GetTipoByDescripcionOrCodePartial(string value)
        {
            if (int.TryParse(value, out int tipoVehiculo))
            {
                return this._context.T_TIPO_VEICULO.AsNoTracking()
                    .Where(d => d.CD_TIPO_VEICULO == tipoVehiculo || d.DS_TIPO_VEICULO.ToLower().Contains(value.ToLower()))
                    .ToList().Select(d => this._mapper.MapToObject(d))
                    .ToList();
            }
            else
            {
                return this._context.T_TIPO_VEICULO.AsNoTracking()
                    .Where(d => d.DS_TIPO_VEICULO.ToLower().Contains(value.ToLower()))
                    .ToList().Select(d => this._mapper.MapToObject(d))
                    .ToList();
            }
        }

        public virtual void Update(VehiculoEspecificacion especificacion)
        {
            T_TIPO_VEICULO entity = this._mapper.MapToEntity(especificacion);
            T_TIPO_VEICULO attachedEntity = _context.T_TIPO_VEICULO.Local
                .FirstOrDefault(x => x.CD_TIPO_VEICULO == entity.CD_TIPO_VEICULO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_TIPO_VEICULO.Attach(entity);
                _context.Entry<T_TIPO_VEICULO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void Delete(VehiculoEspecificacion especificacion)
        {
            T_TIPO_VEICULO entity = this._mapper.MapToEntity(especificacion);
            T_TIPO_VEICULO attachedEntity = _context.T_TIPO_VEICULO.Local
                .FirstOrDefault(x => x.CD_TIPO_VEICULO == entity.CD_TIPO_VEICULO);

            if (attachedEntity != null)
            {
                this._context.T_TIPO_VEICULO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_TIPO_VEICULO.Attach(entity);
                this._context.T_TIPO_VEICULO.Remove(entity);
            }
        }

        public virtual bool AnyTipo(int id)
        {
            return this._context.T_TIPO_VEICULO.Any(d => d.CD_TIPO_VEICULO == id);
        }
    }
}
