using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Domain.General.Enums;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.Security;

namespace WIS.Domain.Expedicion
{
    public class ArmadoEgresoPedido
    {
        protected readonly Camion _camion;
        protected readonly IUnitOfWork _uow;
        protected readonly List<PedidoAsociarUnidad> _pedidosAsociar;
        protected readonly ExpedicionConfiguracionService _expedicionService;
        protected readonly IFactoryService _factoryService;
        protected readonly IIdentityService _identity;

        public ArmadoEgresoPedido(IUnitOfWork uow,
            IFactoryService factoryService,
            IIdentityService identity,
            ExpedicionConfiguracionService expedicionService,
            Camion camion,
            List<PedidoAsociarUnidad> pedido)
        {
            this._uow = uow;
            this._factoryService = factoryService;
            this._identity = identity;
            this._expedicionService = expedicionService;
            this._camion = camion;
            this._pedidosAsociar = pedido;
        }

        public virtual void Armar()
        {
            var documentoEgreso = this.Validar();

            _uow.CamionRepository.AsociarPedidoCamion(_uow, _camion, _pedidosAsociar);

            if (documentoEgreso != null)
            {
                var egresoDocumental = new EgresoDocumental(_factoryService);
                egresoDocumental.GenerarEgresoDocumental(_uow, _identity.UserId, _camion.Id, documentoEgreso.Tipo, documentoEgreso.Numero);
            }
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

            if (!this._camion.PuedeArmarse(manejoDocumentalActivo, egresoDocumentalEditable))
            {
                if (this._camion.NumeroInterfazEjecucionFactura == -1)
                    throw new ValidationFailedException("WEXP012_Sec0_Error_Er002_CamionPendienteFacturar");
                else if (manejoDocumentalActivo && !egresoDocumentalEditable)
                    throw new ValidationFailedException("General_Sec0_Error_EgresoDocumentalNoEditable");
                else
                    throw new ValidationFailedException("General_Sec0_Error_EstadoCamionNoArmable");
            }

            return egresoDocumentalEditable ? documentoEgreso : null;
        }
    }
}
