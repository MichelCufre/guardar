using WIS.Domain.DataModel;
using WIS.Exceptions;

namespace WIS.Domain.Impresiones
{
    public class EstiloTemplate : IEstiloTemplate
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _estilo;
        protected readonly string _lenguaje;

        public EstiloTemplate(IUnitOfWork uow, string estilo)
        {
            this._uow = uow;
            this._estilo = estilo;
        }
 
        public virtual TemplateImpresion GetTemplate(Impresora impresora)
        {
            //TODO: VER COMO PASARLO A LENGUAJE DE IMPRESION Y NO CODIGO
            //return this._uow.TemplateImpresionRepository.GetTemplateImpresion(this._estilo, impresora.GetLenguaje().Id);

            var template = this._uow.TemplateImpresionRepository.GetTemplateImpresion(this._estilo, impresora.CodigoLenguajeImpresion);

            if (template == null)
                throw new EntityNotFoundException("General_Sec0_Error_ImpModule_NoHayTemplateCompatible");

            return template;
        }
    }
}
