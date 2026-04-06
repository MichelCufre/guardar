using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.Validation;

namespace WIS.Domain.Parametrizacion
{
    public interface IAtributoValidacionService
    {
        public void Validar(IUnitOfWork uow, int idAtributo, short idValidacion, string valorAValidar, string valorComparativo, IFormatProvider culture, bool invocarAPICustom, out Error error);
    }
}
