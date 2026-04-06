using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class TransportistaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly TransportistaMapper _mapper;

        public TransportistaRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new TransportistaMapper();
        }

        public virtual Transportista GetTransportista(int idTransportista)
        {
            var entity = this._context.T_TRANSPORTADORA
                .FirstOrDefault(d => d.CD_TRANSPORTADORA == idTransportista);
            return this._mapper.MapToObject(entity);
        }
        public virtual Transportista GetFirstTransportista()
        {
            var entity = this._context.T_TRANSPORTADORA.AsNoTracking().Where(t => t.CD_SITUACAO == SituacionDb.Activo).OrderBy(t => t.CD_TRANSPORTADORA).FirstOrDefault();
            return this._mapper.MapToObject(entity);
        }

        public virtual string GetDescripcion(int idTransportista)
        {
            return this._context.T_TRANSPORTADORA
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_TRANSPORTADORA == idTransportista)?.DS_TRANSPORTADORA;
        }

        public virtual List<Transportista> GetByDescripcionOrCodePartial(string value)
        {
            if (int.TryParse(value, out int idTransportista))
            {
                return this._context.T_TRANSPORTADORA.AsNoTracking()
                    .AsNoTracking()
                    .Where(d => d.CD_TRANSPORTADORA == idTransportista || d.DS_TRANSPORTADORA.ToLower().Contains(value.ToLower()))
                    .ToList().Select(d => this._mapper.MapToObject(d))
                    .ToList();
            }
            else
            {
                return this._context.T_TRANSPORTADORA.AsNoTracking()
                    .AsNoTracking()
                    .Where(d => d.DS_TRANSPORTADORA.ToLower().Contains(value.ToLower()))
                    .ToList().Select(d => this._mapper.MapToObject(d))
                    .ToList();
            }
        }

        public virtual bool AnyTransportista(int id)
        {
            return this._context.T_TRANSPORTADORA
                .AsNoTracking()
                .Any(d => d.CD_TRANSPORTADORA == id);
        }

        public virtual IEnumerable<Transportista> GetTransportistas()
        {
            return this._context.T_TRANSPORTADORA.AsNoTracking()
                .Select(t => this._mapper.MapToObject(t));
        }
    }
}
