using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class FileRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly FileMapper _mapper;

        public FileRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new FileMapper();
        }

        #region Any

        public virtual bool AnyFileByName(string tpEntidad, string cdCodigoEntidad, string fileName)
        {
            return this._context.T_ARCHIVO
                .AsNoTracking()
                .Any(x => x.TP_ENTIDAD == tpEntidad 
                    && x.CD_ENTIDAD == cdCodigoEntidad 
                    && x.NM_ARCHIVO.Trim().ToLower() == fileName.Trim().ToLower());
        }

        public virtual bool AnyFileById(string fileId)
        {
            return this._context.T_ARCHIVO
                .AsNoTracking()
                .Any(x => x.CD_ARCHIVO == fileId);
        }

        #endregion

        #region Get

        public virtual General.File GetFileById(string fileId)
        {
            var file = this._context.T_ARCHIVO
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_ARCHIVO == fileId);

            return this._mapper.MapToObject(file);
        }

        #endregion

        #region Add

        public virtual void AddFile(General.File file)
        {
            T_ARCHIVO entity = this._mapper.MapToEntity(file);
            this._context.T_ARCHIVO.Add(entity);
        }

        #endregion

        #region Update

        #endregion
        
        #region Remove
        public virtual void RemoveFileById(string fileId)
        {
            T_ARCHIVO entity = this._context.T_ARCHIVO.FirstOrDefault(x => x.CD_ARCHIVO == fileId);
            T_ARCHIVO attachedEntity = _context.T_ARCHIVO.Local.FirstOrDefault(w => w.CD_ARCHIVO == entity.CD_ARCHIVO);

            if (attachedEntity != null)
            {
                _context.T_ARCHIVO.Remove(attachedEntity);
            }
            else
            {
                _context.T_ARCHIVO.Attach(entity);
                _context.T_ARCHIVO.Remove(entity);
            }

        }
        #endregion

    }
}
