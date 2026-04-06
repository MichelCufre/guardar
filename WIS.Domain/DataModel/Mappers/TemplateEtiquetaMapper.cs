using WIS.Domain.General.Configuracion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class TemplateEtiquetaMapper : Mapper
    {
        public TemplateEtiquetaMapper()
        {
        }

        public virtual TemplateEtiqueta MapToObject(T_LABEL_TEMPLATE item)
        {
            return new TemplateEtiqueta
            {
                estilo = item.CD_LABEL_ESTILO,
                lenguaje = item.CD_LENGUAJE_IMPRESION,
                preCuerpo = item.VL_PRE_CUERPO,
                cuerpo = item.VL_CUERPO,
                postCuerpo = item.VL_POST_CUERPO,
                altura = item.VL_LABEL_ALTURA,
                largura = item.VL_LABEL_LARGURA
            };
        }

        public virtual T_LABEL_TEMPLATE MapToEntity(TemplateEtiqueta item)
        {
            return new T_LABEL_TEMPLATE
            {
                CD_LABEL_ESTILO = item.estilo,
                CD_LENGUAJE_IMPRESION = item.lenguaje,
                VL_PRE_CUERPO = item.preCuerpo,
                VL_CUERPO = item.cuerpo,
                VL_POST_CUERPO = item.postCuerpo,
                VL_LABEL_ALTURA = item.altura,
                VL_LABEL_LARGURA = item.largura
            };
        }

    }
}
