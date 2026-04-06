using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.Documento.Reserva;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class DocumentoEntradaProduccionReservaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly DocumentoEntradaProduccionReservaMapper _mapper;
        protected readonly IDapper _dapper;

        public DocumentoEntradaProduccionReservaRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new DocumentoEntradaProduccionReservaMapper();
            this._dapper = dapper;
        }

        #region Any

        #endregion

        #region Get

        public virtual List<DocumentoProduccionEntrada> GetEntradasReservas(int empresa, string producto, string identificador)
        {
            List<DocumentoProduccionEntrada> entradas = new List<DocumentoProduccionEntrada>();

            var entradasEntity = this._context.T_DOCUMENTO_PRDC_ENTRADA
                                         .Where(pa => pa.CD_EMPRESA == empresa
                                                && pa.CD_PRODUTO == producto
                                                && pa.NU_IDENTIFICADOR == identificador
                                                && pa.QT_PRODUTO > 0)
                                         .AsNoTracking()
                                         .ToList();

            foreach (var entity in entradasEntity)
            {
                entradas.Add(this._mapper.MapToEntradaProduccionReserva(entity));
            }

            return entradas;
        }

        public virtual string GetIdAnulacion()
        {
            return this._context.GetNextSequenceValueDecimal(_dapper, "S_DOC_ID_ANULACION").ToString();
        }

        #endregion

        #region Add

        #endregion

        #region Update

        public virtual void UpdateDocumentoProduccionEntrada(DocumentoProduccionEntrada produccionEntradaReserva, long nuTransaccion)
        {
            var entity = this._mapper.MapFromEntradaProduccionReserva(produccionEntradaReserva);
            var attachedEntity = _context.T_DOCUMENTO_PRDC_ENTRADA.Local
                .FirstOrDefault(w => w.NU_PREPARACION == entity.NU_PREPARACION
                    && w.NU_PEDIDO == entity.NU_PEDIDO
                    && w.NU_CONTENEDOR == entity.NU_CONTENEDOR
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_PRODUTO == entity.CD_PRODUTO
                    && w.CD_FAIXA == entity.CD_FAIXA
                    && w.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR);

            entity.VL_DATO_AUDITORIA = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO_PRDC_ENTRADA.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void ProcesarEntradasModificadas(DocumentoProduccionEntradaSaldos entradasSaldos, long nuTransaccion)
        {
            foreach (var update in entradasSaldos.LineasModificadas)
            {
                this.UpdateDocumentoProduccionEntrada(update, nuTransaccion);
            }
            foreach (var remove in entradasSaldos.LineasEliminadas)
            {
                this.RemoveDocumentoProduccionEntrada(remove, nuTransaccion);
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveDocumentoProduccionEntrada(DocumentoProduccionEntrada produccionEntradaReserva, long nuTransaccion)
        {
            var entity = this._mapper.MapFromEntradaProduccionReserva(produccionEntradaReserva);
            var attachedEntity = _context.T_DOCUMENTO_PRDC_ENTRADA.Local
                .FirstOrDefault(w => w.NU_PREPARACION == entity.NU_PREPARACION
                    && w.NU_PEDIDO == entity.NU_PEDIDO
                    && w.NU_CONTENEDOR == entity.NU_CONTENEDOR
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_PRODUTO == entity.CD_PRODUTO
                    && w.CD_FAIXA == entity.CD_FAIXA
                    && w.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR);

            string datoAuditoria = string.Format("{0}${1}${2}", this._application, this._userId, nuTransaccion.ToString());
            entity.VL_DATO_AUDITORIA = datoAuditoria;

            if (attachedEntity != null)
            {
                this._context.T_DOCUMENTO_PRDC_ENTRADA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DOCUMENTO_PRDC_ENTRADA.Attach(entity);
                this._context.T_DOCUMENTO_PRDC_ENTRADA.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #endregion
    }

}
