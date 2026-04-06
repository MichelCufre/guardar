using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General.Enums;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CaracteresAdmitidosCodigoProductoValidationRule : IValidationRule
    {
        protected readonly string _field;
        protected readonly IUnitOfWork _uow;
        protected readonly ProductoMapper _mapper;

        public CaracteresAdmitidosCodigoProductoValidationRule(IUnitOfWork uow, string field)
        {
            this._field = field;
            this._uow = uow;
            this._mapper = new ProductoMapper();
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            //TODO: VER DE MEJORAR LA VALIDACION
            string codigoProducto = this._field.ToUpper();

            string caracteresNoPermitidos = string.Empty;

            string lstCaracteres = _uow.ParametroRepository.GetParameter("LISTA_CARACTERES_COD_PROD");
            Char[] arrCaracteres = lstCaracteres.ToCharArray();

            for (int i = 0; i < codigoProducto.Length; i++)
            {
                if (!arrCaracteres.Contains(codigoProducto[i]))
                    caracteresNoPermitidos += codigoProducto[i] + " ";
            }

            if (!string.IsNullOrEmpty(caracteresNoPermitidos))
                errors.Add(new ValidationError("REG009_Sec0_Error_NoAdmiteXCaracteres", new List<string> { caracteresNoPermitidos }));

            return errors;
        }
    }
}
