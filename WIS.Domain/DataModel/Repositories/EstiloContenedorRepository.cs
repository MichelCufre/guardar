using System.Linq;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class EstiloContenedorRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;

        public EstiloContenedorRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
        }

        #region Any

        public virtual bool ExisteTipoContenedor(string idEstiloContenedor, string tipoContenedor)
        {
            return this._context.T_REL_LABELESTILO_TIPOCONT.Any(a => a.CD_LABEL_ESTILO == idEstiloContenedor && a.TP_CONTENEDOR == tipoContenedor);
        }

        #endregion

        #region Get
        
        #endregion

        #region Add

        public virtual void AddTipoContenedor(string idEstiloContenedor, string tipoContenedor)
        {
            T_LABEL_ESTILO estilo = _context.T_LABEL_ESTILO.FirstOrDefault(e => e.CD_LABEL_ESTILO == idEstiloContenedor);
            T_TIPO_CONTENEDOR contenedor = _context.T_TIPO_CONTENEDOR.FirstOrDefault(tc => tc.TP_CONTENEDOR == tipoContenedor);

            if (estilo != null && contenedor != null)
            {
                _context.T_REL_LABELESTILO_TIPOCONT.Add(new T_REL_LABELESTILO_TIPOCONT()
                {
                    T_LABEL_ESTILO = estilo,
                    T_TIPO_CONTENEDOR = contenedor
                });
            }
        }
        
        #endregion

        #region Update

        #endregion

        #region Remove

        public virtual void DeleteTipoContenedor(string idEstiloContenedor, string tipoContenedor)
        {
            T_REL_LABELESTILO_TIPOCONT entity = this._context.T_REL_LABELESTILO_TIPOCONT
                .FirstOrDefault(x => x.CD_LABEL_ESTILO == idEstiloContenedor && x.TP_CONTENEDOR == tipoContenedor);
            T_REL_LABELESTILO_TIPOCONT attachedEntity = _context.T_REL_LABELESTILO_TIPOCONT.Local
                .FirstOrDefault(w => w.CD_LABEL_ESTILO == entity.CD_LABEL_ESTILO && w.TP_CONTENEDOR == entity.TP_CONTENEDOR);

            if (attachedEntity != null)
            {
                _context.T_REL_LABELESTILO_TIPOCONT.Remove(attachedEntity);
            }
            else
            {
                _context.T_REL_LABELESTILO_TIPOCONT.Remove(entity);
            }
        }

        #endregion       
    }
}
