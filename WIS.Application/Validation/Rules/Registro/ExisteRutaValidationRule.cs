using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteRutaValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _idUsuairo;
        protected readonly string _predio;
        protected readonly IUnitOfWork _uow;

        public ExisteRutaValidationRule(IUnitOfWork uow, string codigoRuta, int idUsuario)
        {
            this._value = codigoRuta;
            this._uow = uow;
            this._idUsuairo = idUsuario;
        }

        public ExisteRutaValidationRule(IUnitOfWork uow, string codigoRuta, int idUsuario, string predio)
        {
            this._value = codigoRuta;
            this._uow = uow;
            this._idUsuairo = idUsuario;
            this._predio = predio;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var dbQuery = new RutaOndaQuery(this._idUsuairo);
            this._uow.HandleQuery(dbQuery);

            List<Ruta> rutas = dbQuery.GetRutasDisponibles();

            if (!short.TryParse(this._value, out short ruta))
                return errors;

            if (!string.IsNullOrEmpty(this._predio))
            {
                var rutaSeleccionada = rutas.FirstOrDefault(s => s.Id == ruta);

                if (rutaSeleccionada.Onda.Predio != null && rutaSeleccionada.Onda.Predio != this._predio)
                {
                    errors.Add(new ValidationError("General_Sec0_Error_RutaPredioNoExistente"));
                }
            }

            if (!rutas.Any(s => s.Id == ruta))
                errors.Add(new ValidationError("General_Sec0_Error_RutaNoExistente"));

            return errors;
        }
    }
}