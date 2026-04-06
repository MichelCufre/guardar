using System.Text.Json;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Picking;
using WIS.Domain.Picking;
using WIS.PageComponent.Execution;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PRE811Configuracion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly PreferenciaMapper _mapper;

        public PRE811Configuracion(IUnitOfWorkFactory uowFactory, IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _mapper = new PreferenciaMapper();
        }

        public override PageContext PageLoad(PageContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuPreferencia = context.GetParameter("keyPreferencia");

            Preferencia preferencia = uow.PreferenciaRepository.GetPreferencia(int.Parse(nuPreferencia));

            var tabs = new Tabs
            {
                ControlAcceso = _mapper.MapStringToBoolean(preferencia.FL_HABILITADO_CONT_ACCESO),
                Classe = _mapper.MapStringToBoolean(preferencia.FL_HABILITADO_CLASE),
                Familia = _mapper.MapStringToBoolean(preferencia.FL_HABILITADO_FAMILIA),
                Empresa = _mapper.MapStringToBoolean(preferencia.FL_HABILITADO_EMPRESA),
                Cliente = _mapper.MapStringToBoolean(preferencia.FL_HABILITADO_CLIENTE),
                Ruta = _mapper.MapStringToBoolean(preferencia.FL_HABILITADO_RUTA),
                Zona = _mapper.MapStringToBoolean(preferencia.FL_HABILITADO_ZONA),
                CondLiberacion = _mapper.MapStringToBoolean(preferencia.FL_HABILITADO_COND_LIBERACION),
                TpPedido = _mapper.MapStringToBoolean(preferencia.FL_HABILITADO_TP_PEDIDO),
                TpExpedicion = _mapper.MapStringToBoolean(preferencia.FL_HABILITADO_TP_EXPEDICION)
            };

            context.AddParameter("TabsEnabled", JsonSerializer.Serialize(tabs));

            return context;
        }

    }

    public class Tabs
    {
        public bool ControlAcceso { get; set; }
        public bool Classe { get; set; }
        public bool Familia { get; set; }
        public bool Empresa { get; set; }
        public bool Cliente { get; set; }
        public bool Ruta { get; set; }
        public bool Zona { get; set; }
        public bool CondLiberacion { get; set; }
        public bool TpPedido { get; set; }
        public bool TpExpedicion { get; set; }
    }
}
