using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
	public class CargaMapper
	{
		public CargaMapper()
		{

		}

		public virtual Carga MapToObject(T_CARGA cargaEntity)
		{
			if (cargaEntity == null) return null;

			return new Carga
			{
				Id = cargaEntity.NU_CARGA,
				Descripcion = cargaEntity.DS_CARGA,
				Ruta = cargaEntity.CD_ROTA,
				FechaAlta = cargaEntity.DT_ADDROW,
				Preparacion = cargaEntity.NU_PREPARACION,
			};
		}

		public virtual T_CARGA MapToEntity(Carga carga)
		{
			return new T_CARGA
			{
				NU_CARGA = carga.Id,
				DS_CARGA = carga.Descripcion,
				CD_ROTA = carga.Ruta,
				DT_ADDROW = carga.FechaAlta,
				NU_PREPARACION = carga.Preparacion
			};
		}
	}
}
