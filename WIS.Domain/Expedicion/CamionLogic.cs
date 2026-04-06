using System;
using WIS.Domain.DataModel;
using WIS.Exceptions;

namespace WIS.Domain.Expedicion
{
    public class CamionLogic
    {
        public CamionLogic()
        {
        }

        public virtual void ProcesarCerrarCamion(IUnitOfWork uow, Camion camion, ProcesoExpedicion exp)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var sincZFM = uow.ParametroRepository.GetParameter("SINCRONIZAR_ZFM") ?? "N";

            CierreCamionPermitido(uow, camion.Id, sincZFM);

            camion.NumeroTransaccion = nuTransaccion;
            camion.FechaModificacion = DateTime.Now;
            camion.Estado = Enums.CamionEstado.IniciandoCierre;

            string cdPorta = camion.Puerta.ToString();

            if (string.IsNullOrEmpty(cdPorta))
                throw new EntityNotFoundException("General_Sec0_Error_Er197_CamionSinPuerta");

            var puertaEmbarque = uow.PuertaEmbarqueRepository.GetUbicacionPuertaEmbarque(short.Parse(cdPorta));
            var productos = uow.StockRepository.GetStockContePuertaCamion(camion.Id, puertaEmbarque);

            int aux = 0;

            foreach (var producto in productos)
            {
                aux++;

                uow.StockRepository.UpdateStock(producto.CodigoUbicacion, producto.CodigoEmpresa, producto.CodigoProducto, producto.CodigoFaixa, producto.Lote, (producto.CantidadPreparada ?? 0), alta: false, modificarReserva: true);

                exp.GrabarExpedicion(uow, (int)producto.CodigoCamion, producto.NumeroPedido, producto.CodigoCliente, producto.CodigoEmpresa, producto.CodigoProducto, producto.CodigoFaixa, producto.Lote, producto.EspecificaLote, producto.CantidadPreparada);
                uow.SaveChanges();
            }

            DeleteCargaCamion(uow, camion.Id);
            var logic = new ContenedorLogic();

            //SE PUEDE OPTIMIZAR
            logic.UpdateContenedorCamion(uow, camion.Id, puertaEmbarque);

            camion.Cerrar();
            uow.CamionRepository.UpdateCamion(camion);
        }

        public virtual void CierreCamionPermitido(IUnitOfWork uow, int cdCamion, string valorParametro)
        {
            var cam = uow.CamionRepository.GetCamion(cdCamion);

            if (cam == null)
                throw new InvalidOperationException("General_Sec0_Error_Er198_camionNoEncontrado");

            if (cam.IsCerrado())
                throw new EntityNotFoundException("General_Sec0_Error_Er196_CamionCerrado");

            if (valorParametro.Equals("S"))
            {
                int countEmbarcados = uow.PreparacionRepository.GetCantidadContenedoresEmbargados(cdCamion);

                if (countEmbarcados == 0)
                    throw new EntityNotFoundException("General_Sec0_Error_Er200_ContNoCargadosCamion");

                int countSinEmparcar = uow.PreparacionRepository.GetContenedoresSinEmbarcar(cdCamion);

                if (countSinEmparcar > 0)
                    throw new EntityNotFoundException("General_Sec0_Error_Er201_ContPendientesEnvios");

                int countProdSinPrep = uow.PreparacionRepository.GetContenedoresSinPreparar(cdCamion);

                if (countProdSinPrep > 0)
                    throw new EntityNotFoundException("General_Sec0_Error_Er202_PickeosPendientes");

                //TODO Manejo Control Documental 
            }
        }

        public virtual void DeleteCargaCamion(IUnitOfWork uow, int cdCamion)
        {
            var cargas = uow.CargaCamionRepository.GetsCargasCamion(cdCamion);

            foreach (var carga in cargas)
            {
                uow.CargaCamionRepository.DeleteCargaCamiones(carga);
            }
        }

        public virtual void AgregarCargaCamion(IUnitOfWork uow, ContenedorFacturar contenedor)
        {
            long? nuCarga = uow.PreparacionRepository.GetCargaContenedor(contenedor);

            if (nuCarga != null)
            {
                contenedor.NumeroCarga = nuCarga ?? -1;

                if (!uow.CargaCamionRepository.ExisteCargaCamion(nuCarga, contenedor.CodigoCliente, contenedor.CodigoCamion))
                {
                    var carga = new CargaCamion();

                    carga.Camion = contenedor.CodigoCamion;
                    carga.Carga = contenedor.NumeroCarga;
                    carga.Empresa = contenedor.CodigoEmpresa;
                    carga.Cliente = contenedor.CodigoCliente;
                    carga.FechaAlta = DateTime.Now;

                    uow.CargaCamionRepository.AddCargaCamion(carga);
                }
            }
        }
    }
}
