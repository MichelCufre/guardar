using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class SituacionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly SituacionMapper _mapper;

        public SituacionRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new SituacionMapper();
        }

        public virtual List<Situacion> GetSituaciones(List<int> situaciones)
        {
            List<Situacion> lstSituaciones = new List<Situacion>();

            List<T_SITUACAO> lstEntity = _context.T_SITUACAO.Where(x => situaciones.Contains(x.CD_SITUACAO)).ToList();

            foreach(T_SITUACAO sit in lstEntity)
            {
                Situacion objSituacion = _mapper.MapToObject(sit);
                lstSituaciones.Add(objSituacion);
            }
            return lstSituaciones;
        }

        public virtual string GetSituacionDescripcion(short? cd_situacion)
        {
            return this._context.T_SITUACAO.FirstOrDefault(x => x.CD_SITUACAO == cd_situacion).DS_SITUACAO;
        }


    }
}