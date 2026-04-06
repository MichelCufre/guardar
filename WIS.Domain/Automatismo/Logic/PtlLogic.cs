using System;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Security;

namespace WIS.Domain.Automatismo.Logic
{
    public class PtlLogic
    {
        protected readonly IBarcodeService _barcodeService;
        protected readonly IIdentityService _identity;

        public PtlLogic(IBarcodeService barcodeService,IIdentityService identity)
        {
            _barcodeService = barcodeService;
            _identity = identity;
        }

        public virtual bool TieneMasDeUnConjuntoDeAgrupacionContenedor(IUnitOfWork uow, int preparacion, int numeroContenedor)
        {
            return uow.PreparacionRepository.TieneMasDeUnConjuntoDeAgrupacionContenedor(preparacion, numeroContenedor);
        }

        public virtual bool EsCompatibleContenedorConAgrupaciones(IUnitOfWork uow, int preparacion, int numeroContenedor, string vlComparteContenedor, string subClase, string cliente)
        {
            if (uow.PreparacionRepository.TieneAgrupaciones(preparacion, numeroContenedor))
            {
                return !uow.PreparacionRepository.TieneOtraAgrupacion(preparacion, numeroContenedor, vlComparteContenedor, subClase, cliente);
            }

            return true;
        }

        public virtual bool AnyPickingPendienteConLoteAuto(IUnitOfWork uow, int preparacion, string vlComparteContenedor, string subClase)
        {
            return uow.PreparacionRepository.AnyPickingPendienteConLoteAuto(preparacion, vlComparteContenedor, subClase);
        }

        public virtual PtlAgrupacionContenedor GetAgrupacionContenedor(IUnitOfWork uow, int numeroPreparacion, int numeroContenedor)
        {
            return uow.PreparacionRepository.GetAgrupacionContenedor(numeroPreparacion, numeroContenedor);
        }

        public virtual (bool, Contenedor) ExisteContenedor(IUnitOfWork uow, string codigoBarras)
        {
            _barcodeService.ValidarEtiquetaContenedor(codigoBarras, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

            if (datosContenedor.NuPreparacion == -1 && datosContenedor.Estado == EstadoContenedor.Unknown)
                return (false, null);

            if (datosContenedor.NuPreparacion == -1 && (datosContenedor.Estado != EstadoContenedor.EnPreparacion || datosContenedor.Estado != EstadoContenedor.EnCamion))
                return (false, null);

            return (true, uow.ContenedorRepository.GetContenedor(datosContenedor.NuPreparacion, datosContenedor.NuContenedor));
        }

        public virtual bool TienePickingPendienteEnUbicacionesAutomatismo(IUnitOfWork uow, int preparacion, int empresa, string cliente)
        {
            return uow.PtlRepository.AnyPickingPendienteEnUbicacionesAutomatismo(preparacion, empresa, cliente);
        }

        public virtual bool TieneUbicacionesAutomatismoPendLiberar(IUnitOfWork uow, int preparacion, int empresa, string cliente)
        {
            return uow.PtlRepository.AnyDetallePickingPendienteAutomatismo(preparacion, empresa, cliente);
        }

        public virtual void LiberarLineasAgrupacion(IUnitOfWork uow, int preparacion, int empresa, string cliente, string estado, string estadoAnterior)
        {
            uow.PreparacionRepository.UpdateDetallesPreparacion(preparacion, empresa, cliente, estado, estadoAnterior, uow.GetTransactionNumber());
        }

        public virtual void TomarLineasAgrupacion(IUnitOfWork uow, int preparacion, int empresa, string cliente, string estado, string estadoAnterior)
        {
            uow.PreparacionRepository.UpdateDetallesPreparacion(preparacion, empresa, cliente, estado, estadoAnterior, uow.GetTransactionNumber());
        }

        public virtual (string entityToLock, string key) CrearKeyEntidadBloqueo(string aplicacion, int userIdConfirmation, string subClase, string cliente, int preparacion, string vlComparteContenedorPicking)
        {
            return ($"{aplicacion}", $"{preparacion}${cliente}${vlComparteContenedorPicking}${subClase}");
        }


        public virtual Contenedor CrearContenedor(IUnitOfWork uow, int numeroContenedor, int preparacion, string ubicacion, string codigoSubClase, string tipoContenedor)
        {
            var contenedor = new Contenedor
            {
                Numero = numeroContenedor,
                NumeroPreparacion = preparacion,
                Estado = EstadoContenedor.EnPreparacion,
                SegundaFase = "N",
                Ubicacion = ubicacion,
                CodigoSubClase = codigoSubClase,
                TipoContenedor = tipoContenedor,
                NumeroTransaccion = uow.GetTransactionNumber(),
                FechaAgregado = DateTime.Now
            };

            uow.ContenedorPtlRepository.AddContenedor(contenedor);

            return contenedor;
        }

        public virtual string CreatePtlReferencia(int preparacion, string codigoCliente, int empresa)
        {
            return $"{preparacion}#{codigoCliente}#{empresa}";
        }
    }
}
