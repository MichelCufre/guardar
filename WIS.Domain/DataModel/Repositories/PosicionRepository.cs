using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Persistence.InMemory;

namespace WIS.Domain.DataModel.Repositories
{
    public class PosicionRepository
    {
        protected readonly WISDBInMemory _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly ColorRepository _ColorRepository;
        protected readonly PtlMapper _mapper;
        protected static long secuenciaLuces = 0; //ToDo

        public PosicionRepository(WISDBInMemory _context, string cdAplicacion, int userId)
        {
            this._context = _context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._ColorRepository = new ColorRepository(_context, cdAplicacion, userId);
            this._mapper = new PtlMapper();
        }

        #region >> Add



        public virtual void AddPtlUbicacionPrendida(PtlPosicionEnUso ubic, bool colorReservado = true, long? orden = null)
        {
            var entity = _mapper.Map(ubic);
            lock (_context.PtlPosicionEnUsoEntity)
            {
                entity.DT_ADDROW = DateTime.Now;
                secuenciaLuces++;
                entity.NU_ORDEN = orden ?? secuenciaLuces;
                entity.Transaccion = this._context.GetNumberTransaction();

                _context.PtlPosicionEnUsoEntity.Add(entity);
                _context.SaveChanges();

                this._ColorRepository.UpdateUltimaAccion(entity.NU_PTL, entity.NU_COLOR, colorReservado);
                _context.SaveChanges();
            }
        }


        #endregion << Add

        #region >> Remove

        public virtual void RemoveUbicacionesPrendidasByPtl(int ptl)
        {
            lock (_context.PtlPosicionEnUsoEntity)
            {
                _context.PtlPosicionEnUsoEntity.RemoveRange(_context.PtlPosicionEnUsoEntity.Where(s => s.NU_PTL == ptl));
                _context.SaveChanges();
            }
        }
        public virtual void RemoveUbicacionesPrendidas(List<PtlPosicionEnUsoEntity> posicionesPrendidas)
        {
            lock (_context.PtlPosicionEnUsoEntity)
            {
                _context.PtlPosicionEnUsoEntity.RemoveRange(posicionesPrendidas);
                _context.SaveChanges();
            }
        }

        public virtual void RemovePtlUbicacionPrendida(int nuAddress, string nuColor, bool colorReservado = true)
        {
            lock (_context.PtlPosicionEnUsoEntity)
            {
                var posicionEnUso = _context.PtlPosicionEnUsoEntity.FirstOrDefault(s => s.NU_ADDRESS == nuAddress && s.NU_COLOR == nuColor);

                if (posicionEnUso != null)
                {
                    _context.PtlPosicionEnUsoEntity.Remove(posicionEnUso);
                    this._ColorRepository.UpdateUltimaAccion(posicionEnUso.NU_PTL, nuColor, colorReservado);
                    _context.SaveChanges();
                }
            }
        }

        #endregion << Remove

        #region >> Any




        public virtual bool AnyUbicacionesPrendidasByUbicacion(int posicion)
        {
            return _context.PtlPosicionEnUsoEntity.Any(s => s.NU_ADDRESS == posicion);
        }
        public virtual bool AnyUbicacionesPrendidasByPocision(List<int> colUbicacion)
        {
            return _context.PtlPosicionEnUsoEntity.Any(s => colUbicacion.Contains(s.NU_ADDRESS));
        }

        public virtual bool AnyUbicacionesPrendidasByColor(int ptl, string color)
        {
            return _context.PtlPosicionEnUsoEntity.Any(x => x.NU_PTL == ptl && x.NU_COLOR == color);
        }
        public virtual bool AnyPtlReferenciaEnUso(int ptl, string referencia)
        {
            return _context.PtlPosicionEnUsoEntity.Any(x => x.NU_PTL == ptl && x.Referencia == referencia);
        }
        #endregion << Any


        #region >> Get
        public virtual List<PtlPosicionEnUso> GetUbicacionesPrendidasByPtl(int ptl)
        {
            return _context.PtlPosicionEnUsoEntity.Where(s => s.NU_PTL == ptl).Select(x => _mapper.Map(x)).ToList();
        }
        public virtual List<PtlPosicionEnUso> GetUbicacionesPrendidasByPtlAndColor(int ptl, string color)
        {
            return _context.PtlPosicionEnUsoEntity.Where(s => s.NU_PTL == ptl && s.NU_COLOR == color).Select(x => _mapper.Map(x)).ToList();
        }

        public virtual List<PtlPosicionEnUso> GetUbicacionesPrendidasByPosicion(int posicion)
        {
            return _context.PtlPosicionEnUsoEntity.Where(s => s.NU_ADDRESS == posicion).Select(x => _mapper.Map(x)).ToList();
        }
        public virtual List<PtlPosicionEnUso> GetUbicacionesPrendidasByPosicionColor(int ptl, string color)
        {
            return _context.PtlPosicionEnUsoEntity.Where(s => s.NU_PTL == ptl && s.NU_COLOR == color).Select(x => _mapper.Map(x)).ToList();
        }

        public virtual List<PtlPosicionEnUso> GetUbicacionesPrendidas()
        {
            return _context.PtlPosicionEnUsoEntity.Select(x => _mapper.Map(x)).ToList();
        }

        public virtual List<int> GetPtlUbicacionesPrendidasByProducto(int ptl, int cdEmpresa, string cdProduto)
        {
            return _context.PtlPosicionEnUsoEntity.Where(d => d.CD_EMPRESA == cdEmpresa && d.CD_PRODUCTO == cdProduto && d.NU_PTL == ptl).Select(a => a.NU_ADDRESS).Distinct().ToList();
        }

        public virtual List<PtlPosicionEnUso> GetUbicacionesPrendidasByColor(int ptl, string color)
        {
            return _context.PtlPosicionEnUsoEntity.Where(x => x.NU_PTL == ptl && x.NU_COLOR == color)
                .Select(x => _mapper.Map(x)).ToList();
        }


        public virtual PtlPosicionEnUso GetPtlUbicacionPrendida(int posicion, string color)
        {
            return _mapper.Map(_context.PtlPosicionEnUsoEntity.Where(s => s.NU_ADDRESS == posicion && s.NU_COLOR == color).OrderBy(s => s.NU_ORDEN).FirstOrDefault());
        }



        #endregion << Get


    }
}
