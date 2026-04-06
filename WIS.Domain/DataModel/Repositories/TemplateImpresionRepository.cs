using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Impresiones;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class TemplateImpresionRepository
    {
        protected WISDB _context;
        protected string application;
        protected int userId;
        protected readonly TemplateImpresionMapper _mapper;

        public TemplateImpresionRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this.application = application;
            this.userId = userId;
            this._mapper = new TemplateImpresionMapper();

        }

        #region Add



        #endregion

        #region Get

        public virtual TemplateImpresion GetTemplateImpresion(string estilo, string lenguajeImpresion)
        {
            var template = this._context.T_LABEL_TEMPLATE.FirstOrDefault(x => x.CD_LABEL_ESTILO == estilo && x.CD_LENGUAJE_IMPRESION == lenguajeImpresion);

            return template == null ? null : this._mapper.MapToObject(template);

        }
        #endregion

    }
}
