using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Exceptions;
using WIS.Security;

namespace WIS.Domain.Expedicion
{
    public class DesarmadoEgresoContenedor
    {
        protected readonly Camion _camion;
        protected readonly IUnitOfWork _uow;
        protected readonly ExpedicionConfiguracionService _expedicionService;
        protected readonly List<ContenedorAsociarUnidad> _contenedoresDesasociar;
        protected readonly IFactoryService _factoryService;
        protected readonly IIdentityService _identity;

        public DesarmadoEgresoContenedor(IUnitOfWork uow,
            IFactoryService factoryService,
            IIdentityService identity,
            Camion camion,
            ExpedicionConfiguracionService expedicionService,
            List<ContenedorAsociarUnidad> contenedores)
        {
            this._uow = uow;
            this._factoryService = factoryService;
            this._identity = identity;
            this._camion = camion;
            this._expedicionService = expedicionService;
            this._contenedoresDesasociar = contenedores;
        }

        public virtual void Desarmar()
        {
            var documentoEgreso = this.Validar();

            this._uow.CamionRepository.DesarmarContenedorCamion(_uow, _camion, _contenedoresDesasociar);

            if (documentoEgreso != null)
            {
                var egresoDocumental = new EgresoDocumental(_factoryService);
                egresoDocumental.GenerarEgresoDocumental(_uow, _identity.UserId, _camion.Id, documentoEgreso.Tipo, documentoEgreso.Numero);
            }

            this._uow.SaveChanges();
        }

        public virtual IDocumentoEgreso Validar()
        {
            if (this._camion == null)
                throw new ValidationFailedException("General_Sec0_Error_Er100_CamionNoExiste");

            var manejoDocumentalActivo = this._expedicionService.IsManejoDocumentalHabilitado(this._camion.Empresa ?? -1);
            var egresoDocumentalEditable = false;
            var documentoEgreso = (IDocumentoEgreso)null;

            if (manejoDocumentalActivo)
            {
                documentoEgreso = this._uow.DocumentoRepository.GetEgresoPorCamion(this._camion.Id);

                if (documentoEgreso == null)
                    egresoDocumentalEditable = true;
                else
                    egresoDocumentalEditable = this._uow.DocumentoTipoRepository.PermiteEditarCamion(documentoEgreso.Tipo, documentoEgreso.Estado);
            }

            if (!this._camion.PuedeArmarse(manejoDocumentalActivo, egresoDocumentalEditable, armadoPorContenedor: true))
            {
                if (this._camion.NumeroInterfazEjecucionFactura == -1)
                    throw new ValidationFailedException("WEXP010_Sec0_Error_Er004_CamionPendienteFacturarInmodificable");
                else if (manejoDocumentalActivo && !egresoDocumentalEditable)
                    throw new ValidationFailedException("General_Sec0_Error_EgresoDocumentalNoEditable");
                else
                    throw new ValidationFailedException("General_Sec0_Error_EstadoCamionNoArmable");
            }

            //Cargas asignadas al camion por la modaliad Pedido
            var cargasModPed = _camion.Cargas
                .Join(_contenedoresDesasociar,
                    cc => new { cc.Carga, cc.Cliente, cc.Empresa },
                    ccc => new { ccc.Carga, ccc.Cliente, ccc.Empresa },
                    (cc, ccc) => new { CargaCamion = cc, CargaContenedo = ccc })
                .Any(x => x.CargaCamion.TipoModalidad == TipoModalidadArmado.Pedido);

            if (cargasModPed)
                throw new ValidationFailedException("General_Sec0_Error_DesarmarContenedorArmadoPEdido");

            return egresoDocumentalEditable ? documentoEgreso : null;
        }
    }
}
