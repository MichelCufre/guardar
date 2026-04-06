using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class RutaMapper : Mapper
    {
        protected readonly OndaMapper _ondaMapper;
        protected readonly PuertaEmbarqueMapper _puertaEmbarque;
        protected readonly TransportistaMapper _transportistaMapper;

        public RutaMapper() {
            _ondaMapper = new OndaMapper();
            _puertaEmbarque = new PuertaEmbarqueMapper();
            _transportistaMapper = new TransportistaMapper();
        }

        public RutaMapper(OndaMapper ondaMapper, PuertaEmbarqueMapper puertaEmbarque, TransportistaMapper transportistaMapper)
        {
            _ondaMapper = ondaMapper;
            _puertaEmbarque = puertaEmbarque;
            _transportistaMapper = transportistaMapper;
        }

        public virtual Ruta MapToObject(T_ROTA entity)
        {
            if (entity == null)
                return null;

            return new Ruta
            {
                Id = entity.CD_ROTA,
                Descripcion = entity.DS_ROTA,
                Estado = this.MapEstado(entity.CD_SITUACAO ?? 0),
                ControlaOrdenDeCarga = this.MapStringToBoolean(entity.ID_ORDEM_CARGA),
                FechaAlta = entity.DT_CADASTRAMENTO,
                FechaModificacion = entity.DT_ALTERACAO,
                FechaSituacion = entity.DT_SITUACAO,
                Onda = (_ondaMapper == null ? null : (entity.T_ONDA == null ? null : _ondaMapper.MapToObject(entity.T_ONDA))),
                PuertaEmbarque = (_puertaEmbarque == null ? null : (entity.T_PORTA_EMBARQUE == null ? null : _puertaEmbarque.MapToObject(entity.T_PORTA_EMBARQUE))),
                Transportista = entity.CD_TRANSPORTADORA,
                Zona = entity.CD_ZONA
            };
        }

        public virtual T_ROTA MapToEntity(Ruta ruta)
        {
            return new T_ROTA
            {
                CD_ROTA = ruta.Id,
                DS_ROTA = ruta.Descripcion,
                CD_ONDA = (ruta.Onda == null ? null : (short?)ruta.Onda.Id),
                CD_PORTA = (ruta.PuertaEmbarque == null ? null : (short?)ruta.PuertaEmbarque.Id),
                CD_SITUACAO = this.MapEstado(ruta.Estado),
                CD_TRANSPORTADORA = ruta.Transportista,
                DT_ALTERACAO = ruta.FechaModificacion,
                DT_CADASTRAMENTO = ruta.FechaAlta,
                DT_SITUACAO = ruta.FechaSituacion,
                ID_ORDEM_CARGA = this.MapBooleanToString(ruta.ControlaOrdenDeCarga),
                CD_ZONA = ruta.Zona
            };
        }
        public virtual EstadoRutaDeEntrega MapEstadoBooleanToEnum(bool value)
        {
            return (EstadoRutaDeEntrega)(value ? EstadoRutaDeEntrega.Activo : EstadoRutaDeEntrega.Inactivo);
        }
        public virtual EstadoRutaDeEntrega MapEstado(short? estado)
        {
            switch (estado)
            {
                case SituacionDb.Activo: return EstadoRutaDeEntrega.Activo;
                case SituacionDb.Inactivo: return EstadoRutaDeEntrega.Inactivo;
            }

            return EstadoRutaDeEntrega.Unknown;
        }
        public virtual short? MapEstado(EstadoRutaDeEntrega estado)
        {
            switch (estado)
            {
                case EstadoRutaDeEntrega.Activo: return SituacionDb.Activo;
                case EstadoRutaDeEntrega.Inactivo: return SituacionDb.Inactivo;
            }

            return null;
        }

    }
}
