using WIS.Domain.OrdenTarea;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class InsumoManipuleoMapper
    {
        public InsumoManipuleoMapper()
        {

        }

        public virtual InsumoManipuleo MapToObject(T_ORT_INSUMO_MANIPULEO entity)
        {
            return new InsumoManipuleo
            {
                Id = entity.CD_INSUMO_MANIPULEO,
                Descripcion = entity.DS_INSUMO_MANIPULEO,
                NumeroComponente = entity.NU_COMPONENTE,
                Producto = entity.CD_PRODUTO,
                Empresa = entity.CD_EMPRESA,
                FlTodaEmpresa = entity.FL_TODA_EMPRESA,
                Tipo = entity.TP_INSUMO_MANIPULEO,
            };
        }
        public virtual T_ORT_INSUMO_MANIPULEO MapToEntity(InsumoManipuleo insumo)
        {
            return new T_ORT_INSUMO_MANIPULEO
            {
                CD_INSUMO_MANIPULEO = insumo.Id,
                DS_INSUMO_MANIPULEO = insumo.Descripcion,
                NU_COMPONENTE = insumo.NumeroComponente,
                CD_PRODUTO = insumo.Producto,
                CD_EMPRESA = insumo.Empresa,
                FL_TODA_EMPRESA = insumo.FlTodaEmpresa,
                TP_INSUMO_MANIPULEO = insumo.Tipo
            };
        }
    }
}
