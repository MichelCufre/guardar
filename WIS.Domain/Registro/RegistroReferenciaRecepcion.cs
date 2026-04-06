using NLog;
using System;
using WIS.Domain.DataModel;
using WIS.Domain.Recepcion;

namespace WIS.Domain.Registro
{
    public class RegistroReferenciaRecepcion
    {
        protected readonly IUnitOfWork _uow;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly int _idUsuario;
        protected readonly string _aplicacion;

        public RegistroReferenciaRecepcion(IUnitOfWork uow, int idUsuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._idUsuario = idUsuarioLogueado;
            this._aplicacion = aplicacion;
        }

        public virtual ReferenciaRecepcion RegistrarReferencia(ReferenciaRecepcion referencia)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", this._idUsuario))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", this._aplicacion))
            {

                referencia.FechaModificacion = DateTime.Now;
                referencia.FechaInsercion = DateTime.Now;
                referencia.NumeroTransaccion = _uow.GetTransactionNumber();

                this._uow.ReferenciaRecepcionRepository.AddReferencia(referencia);

                if (referencia.Detalles != null)
                {
                    foreach (var detalle in referencia.Detalles)
                    {
                        detalle.IdReferencia = referencia.Id;

                        detalle.FechaModificacion = DateTime.Now;
                        detalle.FechaInsercion = DateTime.Now;
                        detalle.NumeroTransaccion = referencia.NumeroTransaccion;

                        this._uow.ReferenciaRecepcionRepository.AddReferenciaDetalle(detalle);
                    }
                }

                return referencia;

            }

        }

    }
}
