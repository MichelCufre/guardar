using WIS.Persistence.Database;
using WIS.XmlProcessorEntrada.Models;

namespace WIS.XmlProcessorEntrada.Helpers
{
    public class ExtraDataProcessor
    {
        public virtual void ProcessExtraData(WISDB context, object extraData)
        {
            if (extraData is ProductosExtraData)
            {
                ProcessProductosExtraData(context, (ProductosExtraData)extraData);
            }
        }

        protected virtual void ProcessProductosExtraData(WISDB context, ProductosExtraData extraData)
        {
            if (extraData.Familias.Count > 0)
            {
                var familias = context.T_FAMILIA_PRODUTO.ToList();
                foreach (var familia in familias)
                {
                    var cdFamilia = familia.CD_FAMILIA_PRODUTO;
                    if (extraData.Familias.ContainsKey(cdFamilia))
                    {
                        familia.DS_FAMILIA_PRODUTO = extraData.Familias[cdFamilia];
                    }
                }
            }

            if (extraData.Ramos.Count > 0)
            {
                var ramos = context.T_RAMO_PRODUTO.ToList();
                foreach (var ramo in ramos)
                {
                    var cdRamo = ramo.CD_RAMO_PRODUTO;
                    if (extraData.Ramos.ContainsKey(cdRamo))
                    {
                        ramo.DS_RAMO_PRODUTO = extraData.Ramos[cdRamo];
                    }
                }
            }

            if (extraData.Clases.Count > 0)
            {
                var clases = context.T_CLASSE.ToList();
                foreach (var clase in clases)
                {
                    var cdClase = clase.CD_CLASSE;
                    if (extraData.Clases.ContainsKey(cdClase))
                    {
                        clase.DS_CLASSE = extraData.Clases[cdClase];
                    }
                }
            }
        }
    }
}
