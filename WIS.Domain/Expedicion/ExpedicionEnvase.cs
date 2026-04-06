using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Extension;
using WIS.Security;

namespace WIS.Domain.Expedicion
{
    public class ExpedicionEnvase
    {
        protected readonly IIdentityService _securityService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IUnitOfWork _uow;
        protected readonly Camion _camion;

        public ExpedicionEnvase(IIdentityService securityService, IBarcodeService barcodeService, IUnitOfWork uow, Camion camion)
        {
            this._securityService = securityService;
            this._barcodeService = barcodeService;
            this._uow = uow;
            this._camion = camion;
        }

        public virtual void Expedir()
        {
            List<EnvaseCamion> envases = this._uow.EnvaseRepository.GetEnvasesExpedicion(this._camion.Id);

            foreach (var envase in envases)
            {
                string pedidos = this.GetPedidos(envase);

                this.MarcarEnvase(envase, pedidos);
            }
        }

        public virtual string GetPedidos(EnvaseCamion envase)
        {
            List<string> listaPedidos = this._uow.PreparacionRepository.GetNumerosPedidosDeUnContenedor(envase.Contenedor, envase.Preparacion);

            return string.Join("-", listaPedidos);
        }

        public virtual void MarcarEnvase(EnvaseCamion envaseCamion, string pedidos)
        {
            Envase envase = this._uow.EnvaseRepository.GetEnvase(envaseCamion.IdExterno, envaseCamion.Tipo);

            if (envase == null)
            {
                this.CrearEnvase(envaseCamion, pedidos);
                return;
            }

            envase = this.SetProperties(envase, envaseCamion, pedidos);

            this._uow.EnvaseRepository.UpdateEnvase(envase);
        }

        public virtual Envase CrearEnvase(EnvaseCamion envaseCamion, string pedidos)
        {
            var contenedor = _uow.ContenedorRepository.GetContenedor(envaseCamion.Preparacion, envaseCamion.Contenedor);

            var envase = new Envase
            {
                Id = contenedor.IdExterno,
                TipoEnvase = envaseCamion.Tipo,
                FechaAlta = DateTime.Now,
                CodigoBarras = contenedor.CodigoBarras
            };

            envase = this.SetProperties(envase, envaseCamion, pedidos);

            this._uow.EnvaseRepository.AddEnvase(envase);

            return envase;
        }

        public virtual Envase SetProperties(Envase envase, EnvaseCamion envaseCamion, string pedidos)
        {
            envase.Estado = EstadoEnvase.Expedido;
            envase.FechaModificacion = DateTime.Now;

            if (this._uow.GetTransactionNumber() == 0)
                envase.NumeroTransaccion = _camion.NumeroTransaccion;
            else
                envase.NumeroTransaccion = this._uow.GetTransactionNumber();

            envase.CodigoAgente = envaseCamion.CodigoAgente;
            envase.TipoAgente = envaseCamion.TipoAgente;
            envase.Empresa = envaseCamion.Empresa;
            envase.FechaUltimaExpedicion = DateTime.Now;
            envase.UsuarioUltimaExpedicion = this._securityService.UserId;
            envase.DescripcionUltimoMovimiento = ($"Expedido => Camión: {this._camion.Id}; Preparación: {envaseCamion.Preparacion}; Pedidos: {pedidos};").Truncate(200);

            return envase;
        }
    }
}
