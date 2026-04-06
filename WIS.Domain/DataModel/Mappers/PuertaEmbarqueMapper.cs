using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class PuertaEmbarqueMapper : Mapper
    {
        protected readonly UbicacionMapper _ubicacionMapper;

        public PuertaEmbarqueMapper()
        {
            _ubicacionMapper = new UbicacionMapper();
        }

        public virtual PuertaEmbarque MapToObject(T_PORTA_EMBARQUE entity)
        {
            if (entity == null)
                return null;

            PuertaEmbarque porta = new PuertaEmbarque
            {
                Id = entity.CD_PORTA,
                Descripcion = entity.DS_PORTA,
                Estado = entity.CD_SITUACAO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                CodigoUbicacion = entity.CD_ENDERECO,
                Tipo = entity.TP_PUERTA,
                NumPredio = entity.NU_PREDIO,
                Ubicacion = null
            };

            if (this._ubicacionMapper != null)
                porta.Ubicacion = this._ubicacionMapper.MapToObject(entity.T_ENDERECO_ESTOQUE);

            return porta;
        }
        public virtual T_PORTA_EMBARQUE MapToEntity(PuertaEmbarque puertaEmbarque)
        {
            return new T_PORTA_EMBARQUE
            {
                CD_PORTA = puertaEmbarque.Id,
                DS_PORTA = puertaEmbarque.Descripcion,
                CD_SITUACAO = puertaEmbarque.Estado,
                CD_ENDERECO = NullIfEmpty(puertaEmbarque.Ubicacion == null ? puertaEmbarque.CodigoUbicacion : puertaEmbarque.Ubicacion.Id),
                NU_PREDIO = puertaEmbarque.NumPredio,
                DT_ADDROW = puertaEmbarque.FechaAlta,
                DT_UPDROW = puertaEmbarque.FechaModificacion,
                TP_PUERTA = puertaEmbarque.Tipo,
            };
        }
    }
}
