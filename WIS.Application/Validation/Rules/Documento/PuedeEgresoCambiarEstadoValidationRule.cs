using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Documento;
using WIS.Exceptions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class PuedeEgresoCambiarEstadoValidationRule : IValidationRule
    {
        protected readonly string _nuDocumentoValue;
        protected readonly string _tpDocumentoValue;
        protected readonly string _nuevoEstadoValue;
        protected readonly int _userId;
        protected readonly IUnitOfWork _uow;

        public PuedeEgresoCambiarEstadoValidationRule(IUnitOfWork uow, string nuDocumento, string tpDocumento, string nuevoEstado, int userId)
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
            IDocumentoEgreso documento = this._uow.DocumentoRepository.GetEgreso(this._nuDocumentoValue, this._tpDocumentoValue);

            List<string> estadosHabiltiados = documento.GetEstadosHabilitadosParaCambio(this._uow)
                .Select(a => a.Destino.Id)
                .ToList();

            if (!estadosHabiltiados.Contains(this._nuevoEstadoValue))
                errors.Add(new ValidationError("General_Sec0_Error_Error30"));

            if (documento.Agenda != null && this._uow.DocumentoTipoRepository.PermiteEdicion(documento.Tipo, this._nuevoEstadoValue))
                throw new ValidationFailedException("DOC260_Sec0_Error_DocumentoAgendado");

            return errors;
        }
    }
}
