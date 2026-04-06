using System;
using WIS.Domain.Expedicion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class VehiculoMapper : Mapper
    {
        protected readonly TipoVehiculoMapper _tipoMapper;

        public VehiculoMapper(TipoVehiculoMapper tipoMapper)
        {
            this._tipoMapper = tipoMapper;
        }

        public virtual Vehiculo MapVehiculoEntityToObject(T_VEICULO entity)
        {
            return new Vehiculo
            {
                Id = entity.CD_VEICULO,
                Descripcion = entity.DS_VEICULO,
                Marca = entity.DS_MARCA,
                Matricula = entity.DS_PLACA,
                Estado = entity.ND_ESTADO,
                HoraDisponibilidadDesde = TimeSpan.FromMilliseconds(entity.HR_DISPONIBILIDAD_DESDE),
                HoraDisponibilidadHasta = TimeSpan.FromMilliseconds(entity.HR_DISPONIBILIDAD_HASTA),
                Predio = entity.NU_PREDIO,
                Transportista = entity.CD_TRANSPORTADORA,
                Caracteristicas = this._tipoMapper.MapToObject(entity.T_TIPO_VEICULO),
                SincronizacionRealizada = this.MapStringToBoolean(entity.FL_SYNC_REALIZADA),
            };
        }

        public virtual T_VEICULO MapVehiculoObjectToEntity(Vehiculo vehiculo)
        {
            return new T_VEICULO
            {
                CD_VEICULO = vehiculo.Id,
                DS_VEICULO = vehiculo.Descripcion,
                DS_MARCA = vehiculo.Marca,
                DS_PLACA = vehiculo.Matricula,
                ND_ESTADO = vehiculo.Estado,
                HR_DISPONIBILIDAD_DESDE = (long)vehiculo.HoraDisponibilidadDesde.TotalMilliseconds,
                HR_DISPONIBILIDAD_HASTA = (long)vehiculo.HoraDisponibilidadHasta.TotalMilliseconds,
                NU_PREDIO = vehiculo.Predio,
                CD_TRANSPORTADORA = vehiculo.Transportista,
                CD_TIPO_VEICULO = vehiculo.Caracteristicas.Id,
                FL_SYNC_REALIZADA = this.MapBooleanToString(vehiculo.SincronizacionRealizada)
            };
        }
    }
}
