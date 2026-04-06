using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Persistence.InMemory;

namespace WIS.Domain.DataModel.Repositories
{
    public class ColorRepository
    {
        protected readonly WISDBInMemory _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly PtlMapper _mapper; //ToDo

        public ColorRepository(WISDBInMemory _context, string cdAplicacion, int userId)
        {
            this._context = _context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new PtlMapper();
        }

        #region >> Add

        public void AddPtlColor(PtlColorEnUso color)
        {
            var entity = _mapper.Map(color);
            lock (_context.PtlColorEnUsoEntity)
            {
                entity.DT_ADDROW = DateTime.Now;
                entity.Transaccion = this._context.GetNumberTransaction();

                _context.PtlColorEnUsoEntity.Add(entity);
                _context.SaveChanges();
            }
        }


        #endregion << Add

        #region >> Remove


        public void RemovePtlColor(int ptl, string nuColor)
        {
            lock (_context.PtlColorEnUsoEntity)
            {
                var color = _context.PtlColorEnUsoEntity.FirstOrDefault(s => s.NU_PTL == ptl && s.NU_COLOR == nuColor);
                _context.PtlColorEnUsoEntity.Remove(color);
                _context.SaveChanges();
            }
        }
        public void RemoveColorsByPtl(int ptl)
        {
            lock (_context.PtlColorEnUsoEntity)
            {
                _context.PtlColorEnUsoEntity.RemoveRange(_context.PtlColorEnUsoEntity.Where(s => s.NU_PTL == ptl));
                _context.SaveChanges();
            }
        }




        #endregion << Remove

        #region >> Update


        public void UpdateUltimaAccion(int idPtl, string nuColor, bool colorReservado = true)
        {
            if (colorReservado)
            {
                var accion = _context.PtlColorEnUsoEntity.FirstOrDefault(s => s.NU_PTL == idPtl && s.NU_COLOR == nuColor);

                if (accion != null)
                    accion.DT_ULTIMA_ACCION = DateTime.Now;
            }

        }



        #endregion << Update


        #region >> Sequence



        #endregion << Sequence

        #region >> Get        
        public PtlColorEnUso GetPtlColor(int ptl, int userId)
        {
            return _mapper.Map(_context.PtlColorEnUsoEntity.FirstOrDefault(s => s.UserId == userId && s.NU_PTL == ptl));
        }

        public PtlColorEnUso GetColorEnUso(int ptl, string color)
        {
            return _mapper.Map(_context.PtlColorEnUsoEntity.FirstOrDefault(s => s.NU_COLOR == color && s.NU_PTL == ptl));
        }

        public List<PtlColorEnUso> GetPtlColoresByPtl(int ptl)
        {
            return _context.PtlColorEnUsoEntity.Where(s => s.NU_PTL == ptl).Select(x => _mapper.Map(x)).ToList();
        }

        public List<PtlColor> GetPtlColoresEnUsoByPtl(int ptl)
        {
            return _context.PtlColorEnUsoEntity.Where(s => s.NU_PTL == ptl).Select(x => _mapper.MapColor(x)).ToList();
        }


        #endregion << Get

    }
}
