using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class DocumentoAnulacionPreparacionReservaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly DocumentoAnulacionReservaMapper _mapper;

        public DocumentoAnulacionPreparacionReservaRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new DocumentoAnulacionReservaMapper();
        }

        #region Any

        #endregion

        #region Get

        public virtual DocumentoAnulacionPreparacionReserva GetAnulacion(string idAnulacion)
        {
            var anulacionEntity = this._context.T_DOCUMENTO_ANU_PREP_RESERVA
                .Where(pa => pa.ID_ANULACION == idAnulacion)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapToAnulacionPreparacionReserva(anulacionEntity);
        }

        public virtual List<DocumentoAnulacionPreparacionReserva> GetAnulaciones(List<string> idAnulaciones)
        {
            List<DocumentoAnulacionPreparacionReserva> anulaciones = new List<DocumentoAnulacionPreparacionReserva>();

            foreach (string idAnulacion in idAnulaciones)
            {
                var anulacion = this.GetAnulacion(idAnulacion);

                if (anulacion != null)
                    anulaciones.Add(anulacion);
            }

            return anulaciones;
        }

        public virtual List<DocumentoAnulacionPreparacionReserva> GetAnulacionesPendientes(int cantidad)
        {
            List<DocumentoAnulacionPreparacionReserva> anulaciones = new List<DocumentoAnulacionPreparacionReserva>();

            var anulacionesEntity = this._context.T_DOCUMENTO_ANU_PREP_RESERVA
                .Where(pa => pa.ID_ESTADO == EstadoAnularReservaPreparacion.PENDIENTE)
                .AsNoTracking()
                .OrderBy(pa => pa.DT_ADDROW)
                .Take(cantidad)
                .ToList();

            foreach (var entity in anulacionesEntity)
            {
                anulaciones.Add(this._mapper.MapToAnulacionPreparacionReserva(entity));
            }

            return anulaciones;
        }

        #endregion

        #region Add

        public virtual void AddAnulacion(DocumentoAnulacionPreparacionReserva anulacionPreparacionReserva)
        {
            var anulacionEntity = this._mapper.MapFromAnulacionPreparacionReserva(anulacionPreparacionReserva);

            this._context.T_DOCUMENTO_ANU_PREP_RESERVA.Add(anulacionEntity);
        }

        public virtual void AddAnulaciones(List<DocumentoAnulacionPreparacionReserva> anulaciones)
        {
            foreach (var anulacion in anulaciones)
            {
                this.AddAnulacion(anulacion);
            }
        }

        #endregion

        #region Update

        public virtual void UpdateAnulacion(DocumentoAnulacionPreparacionReserva anulacionPreparacionReserva)
        {
            var entity = this._mapper.MapFromAnulacionPreparacionReserva(anulacionPreparacionReserva);
            var attachedEntity = _context.T_DOCUMENTO_ANU_PREP_RESERVA.Local
               .FirstOrDefault(w => w.ID_ANULACION == entity.ID_ANULACION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO_ANU_PREP_RESERVA.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
