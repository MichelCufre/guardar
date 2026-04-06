using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Exceptions;
using WIS.Security;

namespace WIS.Domain.Expedicion
{
    public class ArmadoEgresoContenedor : IArmadoEgreso
    {
        protected readonly Camion _camion;
        protected readonly IUnitOfWork _uow;
        protected readonly ExpedicionConfiguracionService _expedicionService;
        protected readonly List<ContenedorAsociarUnidad> _contenedoresAsociar;
        protected readonly IFactoryService _factoryService;
        protected readonly IIdentityService _identity;

        public ArmadoEgresoContenedor(IUnitOfWork uow,
            IFactoryService factoryService,
            IIdentityService identity,
            Camion camion,
            ExpedicionConfiguracionService expedicionService,
            List<ContenedorAsociarUnidad> contenedores)
        {
            this._uow = uow;
            this._factoryService = factoryService;
            this._identity = identity;
            this._expedicionService = expedicionService;
            this._camion = camion;
            this._contenedoresAsociar = contenedores;
        }

        public virtual void Armar()
        {
            var documentoEgreso = this.Validar();

            this._uow.CamionRepository.AsociarContenedorCamion(_uow, _camion, _contenedoresAsociar);

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

            foreach (var contenedor in this._contenedoresAsociar)
            {
                if (contenedor.GrupoExpedicion == "*")
                {
                    var cont = _uow.ContenedorRepository.GetContenedor(contenedor.Preparacion, contenedor.Contenedor);
                    throw new ValidationFailedException("WEXP_grid1_Error_ContenedorConDiferenteGrupoExpedicionCamion", new string[] { cont.TipoContenedor, cont.IdExterno });
                }
            }

            return egresoDocumentalEditable ? documentoEgreso : null;
        }
    }
}
