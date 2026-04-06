using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.General.Configuracion
{
    public class SistemaConfiguración
    {
        protected readonly IUnitOfWork _uow;

        public bool ManejoDocumentalActivo { get; set; }


        public SistemaConfiguración(IUnitOfWork uow)
        {
            _uow = uow;

            this.CargarParametros();
        }

        public virtual void CargarParametros()
        {
            ManejoDocumentalActivo = _uow.ParametroRepository.GetParameter(ParamManager.MANEJO_DOCUMENTAL) == "S" ? true : false;
        }

    }
}
