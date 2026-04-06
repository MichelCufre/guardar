using WIS.Domain.Impresiones;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
	public class EstiloEtiquetaMapper : Mapper
	{
		public EstiloEtiquetaMapper()
		{
		}

		public virtual EtiquetaEstilo MapToObject(T_LABEL_ESTILO entity)
		{
			if (entity == null) return null;

			return new EtiquetaEstilo
			{
				Id = entity.CD_LABEL_ESTILO,
				Descripcion = entity.DS_LABEL_ESTILO,
				Tipo = entity.TP_LABEL,
				Habilitado = entity.FL_HABILITADO
			};
		}

		public virtual T_LABEL_ESTILO MapToEntity(EtiquetaEstilo obj)
		{
			if (obj == null) return null;

			return new T_LABEL_ESTILO
			{
				CD_LABEL_ESTILO = obj.Id,
				DS_LABEL_ESTILO = obj.Descripcion,
				TP_LABEL = obj.Tipo,
				FL_HABILITADO = obj.Habilitado
			};
		}
	}
}
