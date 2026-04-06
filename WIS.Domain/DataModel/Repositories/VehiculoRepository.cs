using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Expedicion;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
	public class VehiculoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly VehiculoMapper _mapper;
        protected readonly DominioMapper _mapperDominio;
        protected readonly IDapper _dapper;

        public VehiculoRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new VehiculoMapper(new TipoVehiculoMapper());
            this._mapperDominio = new DominioMapper();
            this._dapper = dapper;
        }

        public virtual void Add(Vehiculo vehiculo)
        {
            vehiculo.Id = this._context.GetNextSequenceValueInt(_dapper, "S_VEHICULO");

            var entity = this._mapper.MapVehiculoObjectToEntity(vehiculo);

            this._context.T_VEICULO.Add(entity);
        }

        public virtual void Update(Vehiculo vehiculo)
        {
            var entity = this._mapper.MapVehiculoObjectToEntity(vehiculo);
            T_VEICULO attachedEntity = _context.T_VEICULO.Local.FirstOrDefault(x => x.CD_VEICULO == entity.CD_VEICULO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_VEICULO.Attach(entity);
                _context.Entry<T_VEICULO>(entity).State = EntityState.Modified;
            }
        }

        public virtual Vehiculo GetVehiculo(int id)
        {
            return this._mapper.MapVehiculoEntityToObject(this._context.T_VEICULO
                .Include("T_TIPO_VEICULO")
                .AsNoTracking()
                .Where(d => d.CD_VEICULO == id)
                .FirstOrDefault());
        }

        public virtual List<Vehiculo> GetVehiculos(List<int> vehiculoIds)
        {
            List<T_VEICULO> entities = this._context.T_VEICULO
                .Include("T_TIPO_VEICULO")
                .AsNoTracking()
                .Where(d => vehiculoIds.Contains(d.CD_VEICULO))
                .ToList();

            List<Vehiculo> vehiculos = new List<Vehiculo>();

            foreach (var entity in entities)
            {
                vehiculos.Add(this._mapper.MapVehiculoEntityToObject(entity));
            }

            return vehiculos;
        }

        public virtual List<Vehiculo> GetVehiculos()
        {
            List<T_VEICULO> entities = this._context.T_VEICULO.Include("T_TIPO_VEICULO").AsNoTracking().ToList();

            List<Vehiculo> vehiculos = new List<Vehiculo>();

            foreach (var entity in entities)
            {
                vehiculos.Add(this._mapper.MapVehiculoEntityToObject(entity));
            }

            return vehiculos;
        }
        public virtual List<Vehiculo> GetVehiculosNoSincronizados()
        {
            return _context.T_VEICULO.Include("T_TIPO_VEICULO")
                .AsNoTracking().Where(v => v.FL_SYNC_REALIZADA != "S")
                .Select(v => _mapper.MapVehiculoEntityToObject(v)).ToList();
        }
        public virtual List<DominioDetalle> GetEstadosEditables()
        {
            List<DominioDetalle> result = new List<DominioDetalle>();

            var dominios = this._context.T_DET_DOMINIO.Where(d => d.CD_DOMINIO == "SVEHI").ToList();

            foreach (var dominio in dominios)
            {
                if (dominio.NU_DOMINIO != "SVEHIENUSO")
                    result.Add(this._mapperDominio.MapToObject(dominio));
            }

            return result;
        }

        public virtual List<Vehiculo> GetByDescripcionOrCodePartial(string value, string predio)
        {
            List<T_VEICULO> entities;
            List<Vehiculo> vehiculos = new List<Vehiculo>();

            if (int.TryParse(value, out int vehiculoId))
                entities = this._context.T_VEICULO
                    .AsNoTracking()
                    .Include("T_TIPO_VEICULO")
                    .Where(d => (d.NU_PREDIO == predio || d.NU_PREDIO == null)
                        && (d.CD_VEICULO == vehiculoId
                            || (d.CD_VEICULO.ToString().Contains(vehiculoId.ToString()))
                            || d.DS_VEICULO.ToLower().Contains(value.ToLower())))
                    .ToList();
            else
                entities = this._context.T_VEICULO
                    .AsNoTracking()
                    .Include("T_TIPO_VEICULO")
                    .Where(d => (d.NU_PREDIO == predio || d.NU_PREDIO == null)
                        && (d.DS_VEICULO.ToLower().Contains(value.ToLower())))
                    .ToList();

            foreach (var entity in entities)
            {
                vehiculos.Add(this._mapper.MapVehiculoEntityToObject(entity));
            }

            return vehiculos;
        }

        public virtual void Delete(Vehiculo vehiculo)
        {
            var entity = this._mapper.MapVehiculoObjectToEntity(vehiculo);
            var attachedEntity = this._context.T_VEICULO.Local
                .FirstOrDefault(v => v.CD_VEICULO == entity.CD_VEICULO);

            if (attachedEntity != null)
            {
                this._context.T_VEICULO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_VEICULO.Attach(entity);
                this._context.T_VEICULO.Remove(entity);
            }
        }

        public virtual bool AnyVehiculo(int id)
        {
            return this._context.T_VEICULO.Any(d => d.CD_VEICULO == id);
        }

        public virtual bool AnyVehiculoConTipo(List<int> tipos)
        {
            return this._context.T_VEICULO.Any(d => tipos.Contains(d.CD_TIPO_VEICULO ?? -1));
        }
    }
}
