using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Exceptions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class PuedeIngresoCambiarEstadoValidationRule : IValidationRule
    {
        protected readonly string _nuDocumentoValue;
        protected readonly string _tpDocumentoValue;
        protected readonly string _nuevoEstadoValue;
        protected readonly int _userId;
        protected readonly IUnitOfWork _uow;

        public PuedeIngresoCambiarEstadoValidationRule(IUnitOfWork uow, string nuDocumento, string tpDocumento, string nuevoEstado, int userId)
        {
            this._uow = uow;
            this._nuDocumentoValue = nuDocumento;
            this._tpDocumentoValue = tpDocumento;
            this._nuevoEstadoValue = nuevoEstado;
            this._userId = userId;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            IDocumentoIngreso documento = this._uow.DocumentoRepository.GetIngreso(this._nuDocumentoValue, this._tpDocumentoValue);
            List<string> estadosHabiltiados = documento.GetEstadosHabilitadosParaCambio(this._uow)
                .Select(a => a.Destino.Id)
                .ToList();

            if (!estadosHabiltiados.Contains(this._nuevoEstadoValue))
                errors.Add(new ValidationError("General_Sec0_Error_Error30"));

            if (!(_nuevoEstadoValue == AccionDocumento.Cancelar && (documento.Lineas == null || documento.Lineas.Count() == 0)))
            {
                if (this._uow.DocumentoTipoRepository.RequiereValidacion(this._tpDocumentoValue, this._nuevoEstadoValue) && !documento.Validado)
                    errors.Add(new ValidationError("General_Sec0_Error_Error31"));
            }

            if (this._uow.DocumentoTipoRepository.RequiereLineas(this._tpDocumentoValue, this._nuevoEstadoValue)
                && documento.Lineas.Count == 0)
                errors.Add(new ValidationError("General_Sec0_Error_Error33"));

            if (documento.Agenda != null && this._uow.DocumentoTipoRepository.PermiteEdicion(documento.Tipo, this._nuevoEstadoValue))
                errors.Add(new ValidationError("DOC260_Sec0_Error_DocumentoAgendado"));

            return errors;
        }
    }
}
