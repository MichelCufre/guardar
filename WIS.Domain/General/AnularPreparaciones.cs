using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Picking;

namespace WIS.Domain.General
{
    public class AnularPreparaciones
    {
        protected readonly List<DetallePreparacion> _detalles;
        protected readonly IEnumerable<int> _preparaciones;

        protected readonly IUnitOfWork _uow;
        protected readonly int _userId;
        protected readonly int _tipo;
        protected readonly string _modo;
        protected readonly bool _anulacionParcial;
        protected readonly long _nuTransaccion;

        public AnularPreparaciones(IUnitOfWork uow, int userId, int tipo, string modo, List<DetallePreparacion> detalles, IEnumerable<int> preparaciones, long nuTransaccion)
        {
            this._uow = uow;
            this._detalles = detalles;
            this._userId = userId;
            this._tipo = tipo;
            this._modo = modo;
            this._preparaciones = preparaciones;
            this._nuTransaccion = nuTransaccion;
            this._anulacionParcial = _modo == TipoAnulacion.Seleccion;
        }

        public virtual void ProcesarAnulacion(out string prepPendientes, out string preparacionesEnTraspaso)
        {
            var empresasDocumentales = this._uow.EmpresaRepository.GetEmpresasDocumentales();
            prepPendientes = string.Empty;
            preparacionesEnTraspaso = string.Empty;
            (List<int> pendientes, List<int> enTraspaso) = _uow.AnulacionRepository.ProcesarAnulacion(_detalles, _preparaciones.ToList(), _anulacionParcial, _nuTransaccion, empresasDocumentales.ToList(), _tipo, _modo);


            if (pendientes != null && pendientes.Count > 0)
            {
                foreach (var p in pendientes)
                {
                    if (string.IsNullOrEmpty(prepPendientes))
                        prepPendientes = p.ToString();
                    else
                        prepPendientes += " - " + p.ToString();
                }
            }
            if (enTraspaso != null && enTraspaso.Count > 0)
            {
                foreach (var p in enTraspaso)
                {
                    if (string.IsNullOrEmpty(prepPendientes))
                        preparacionesEnTraspaso = p.ToString();
                    else
                        preparacionesEnTraspaso += " - " + p.ToString();
                }
            }
        }
    }
}
