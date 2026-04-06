using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class ReferenciaDisponibleRecepcionValidationRule : IValidationRule
    {

        protected readonly int _idEmpresa;
        protected readonly string _codigoInternoAgente;
        protected readonly string _tipoRecepcionExterno;
        protected readonly string _idPredio;
        protected readonly int _idReferencia;
        protected readonly IUnitOfWork _uow;

        public ReferenciaDisponibleRecepcionValidationRule(IUnitOfWork uow, string idEmpresa, string codigoInternoAgente, string tipoRecepcionExterno, string predio, string idReferencia)
        {
            this._idEmpresa = int.Parse(idEmpresa);
            this._codigoInternoAgente = codigoInternoAgente;
            this._tipoRecepcionExterno = tipoRecepcionExterno;
            this._idPredio = predio;
            this._uow = uow;
            //TODO Mejorar, cambio por funcionalidad que no soportaba el null cuando se instanciaban las clases
            int valueParsed;
            int.TryParse(idReferencia, out valueParsed);
            this._idReferencia = valueParsed;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var tipoRecepcion = _uow.RecepcionTipoRepository.GetRecepcionTipoByExterno(_idEmpresa, _tipoRecepcionExterno);

            var dbQuery = new GetReferenciasDisponiblesQuery(_idEmpresa, tipoRecepcion.TipoReferencia, _codigoInternoAgente, _idPredio);
            _uow.HandleQuery(dbQuery);

            if (!dbQuery.AnyReferenciaDisponible(_idReferencia))
                errors.Add(new ValidationError("General_Sec0_Error_ReferenciaNoDisponible"));

            return errors;
        }
    }
}