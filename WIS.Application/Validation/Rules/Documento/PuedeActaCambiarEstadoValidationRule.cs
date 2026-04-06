using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Documento;
using WIS.Domain.Documento;
using WIS.Exceptions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class PuedeActaCambiarEstadoValidationRule : IValidationRule
    {
        protected readonly string _nuDocumentoValue;
        protected readonly string _tpDocumentoValue;
        protected readonly string _nuevoEstadoValue;
        protected readonly int _userId;
        protected readonly IUnitOfWork _uow;
        protected readonly DocumentoTipoMapper _mapper;

        public PuedeActaCambiarEstadoValidationRule(IUnitOfWork uow, string nuDocumento, string tpDocumento, string nuevoEstado, int userId)
        {
            this._uow = uow;
            this._nuDocumentoValue = nuDocumento;
            this._tpDocumentoValue = tpDocumento;
            this._nuevoEstadoValue = nuevoEstado;
            this._userId = userId;
            this._mapper = new DocumentoTipoMapper();
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            IDocumentoActa documento = this._uow.DocumentoRepository.GetActa(this._nuDocumentoValue, this._tpDocumentoValue);
            List<string> estadosHabiltiados = documento.GetEstadosHabilitadosParaCambio(this._uow)
                .Select(a => a.Destino.Id)
                .ToList();

            if (!estadosHabiltiados.Contains(this._nuevoEstadoValue))
                errors.Add(new ValidationError("General_Sec0_Error_Error30"));

            if (this._uow.DocumentoTipoRepository.RequiereValidacion(this._tpDocumentoValue, this._nuevoEstadoValue) && !documento.Validado)
                errors.Add(new ValidationError("General_Sec0_Error_Error31"));

            if (documento.Agenda != null && this._uow.DocumentoTipoRepository.PermiteEdicion(documento.Tipo, this._nuevoEstadoValue))
                throw new ValidationFailedException("DOC260_Sec0_Error_DocumentoAgendado");

            return errors;
        }
    }
}
