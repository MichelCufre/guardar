using WIS.Domain.Impresiones;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class TemplateImpresionMapper : Mapper
    {
        public TemplateImpresionMapper()
        {
        }

        public virtual T_LABEL_TEMPLATE MapToEntity(TemplateImpresion template)
        {
            return new T_LABEL_TEMPLATE
            {
                CD_LENGUAJE_IMPRESION = template.CodigoLenguajeImpresion,
                CD_LABEL_ESTILO = template.EstiloEtiqueta,
                VL_CUERPO = template.GetContenido(),
                VL_POST_CUERPO = template.PostCuerpo,
                VL_PRE_CUERPO = template.PreCuerpo,
                VL_LABEL_ALTURA = template.Altura,
                VL_LABEL_LARGURA = template.Ancho

            };
        }

        public virtual TemplateImpresion MapToObject(T_LABEL_TEMPLATE template)
        {
            return new TemplateImpresion(template.VL_CUERPO)
            {
                EstiloEtiqueta = template.CD_LABEL_ESTILO,
                CodigoLenguajeImpresion = template.CD_LENGUAJE_IMPRESION,
                Altura = template.VL_LABEL_ALTURA,
                Ancho = template.VL_LABEL_LARGURA,
                PostCuerpo = template.VL_POST_CUERPO,
                PreCuerpo = template.VL_PRE_CUERPO,

            };
        }

    }
}
