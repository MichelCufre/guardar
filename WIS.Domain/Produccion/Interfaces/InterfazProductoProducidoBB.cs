using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Interfaces;

namespace WIS.Domain.Produccion.Interfaces
{
    public class InterfazProductoProducidoBB
    {
        protected readonly IUnitOfWork _uow;

        public InterfazProductoProducidoBB(IUnitOfWork uow)
        {
            this._uow = uow;
        }

        public virtual void RechazarInterfazProductoProducido(long nroEjecucion, string motivoRechazo)
        {
            var ejecucion = this._uow.EjecucionRepository.GetEjecucion(nroEjecucion).GetAwaiter().GetResult();

            //Modificar Ejecucion
            ejecucion.ErrorCarga = "S";
            ejecucion.Situacion = SituacionDb.ProcesadoConError;

            this._uow.EjecucionRepository.Update(ejecucion).GetAwaiter().GetResult();

            var error = new InterfazError()
            {
                Id = nroEjecucion,
                CodigoError = "Rechazo",               
                Descripcion = motivoRechazo,
                Referencia = "Mod.Doc PRD260",
                NroError = 1,
                Registro = 1
            };

            this._uow.EjecucionRepository.AddEjecucionError(error).GetAwaiter().GetResult();
        }

        public virtual void AprobarInterfazProductoProducido(long nroEjecucion)
        {
            var ejecucion = this._uow.EjecucionRepository.GetEjecucion(nroEjecucion).GetAwaiter().GetResult();

            ejecucion.Situacion = SituacionDb.ProcesadoOK;

            this._uow.EjecucionRepository.Update(ejecucion).GetAwaiter().GetResult();
        }
    }
}
