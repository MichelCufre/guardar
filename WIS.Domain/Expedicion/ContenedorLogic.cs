using System;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Extension;

namespace WIS.Domain.Expedicion
{
    public class ContenedorLogic
    {
        protected int _userId;
        protected string _nuPredio;

        public ContenedorLogic()
        {
        }

        public ContenedorLogic(int userId, string nuPredio)
        {
            this._userId = userId;
            this._nuPredio = nuPredio;
        }

        public virtual void MoverContenedor(IUnitOfWork uow, Contenedor contenedor, string ubicacionDestino)
        {
            try
            {
                if (contenedor.Ubicacion != ubicacionDestino)
                {
                    var productos = uow.PreparacionRepository.GetsProductoContenedor(contenedor.Numero, contenedor.NumeroPreparacion);
                    foreach (var producto in productos)
                    {
                        //Stock origen
                        var stockContenedor = uow.StockRepository.GetStock(producto.Ubicacion, producto.CodigoEmpresa, producto.CodigoProducto, producto.CodigoFaixa, producto.Lote);

                        if (stockContenedor != null && (stockContenedor.Cantidad - producto.CantidadPreparada) >= 0)
                            uow.StockRepository.UpdateStock(producto.Ubicacion, producto.CodigoEmpresa, producto.CodigoProducto, producto.CodigoFaixa, producto.Lote, (producto.CantidadPreparada ?? 0), alta: false, modificarReserva: true);
                        else
                            throw new ValidationFailedException("General_msg_Error_InconsistenciaDeSTock", new string[] { producto.Ubicacion, producto.CodigoEmpresa.ToString(), producto.CodigoProducto, producto.Lote });

                        //Stock destino
                        uow.StockRepository.UpdateOrInsertStock(ubicacionDestino, producto.CodigoEmpresa, producto.CodigoProducto, producto.CodigoFaixa, producto.Lote, (producto.CantidadPreparada ?? 0), alta: true, modificarReserva: true);

                        uow.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                var args = new string[]
                {
                    $"{contenedor.TipoContenedor} - {contenedor.IdExterno}",
                    contenedor.NumeroPreparacion.ToString(),
                    ubicacionDestino
                };

                throw new ValidationFailedException("EXP330_msg_Error_MoverContenedor", args);
            }
        }

        public virtual void UpdateContenedorCamion(IUnitOfWork uow, int cdCamion, string UbicacionPuerta)
        {
            var list = uow.ContenedorRepository.GetContenedoreEnPuerta(cdCamion, UbicacionPuerta);

            foreach (var contenedor in list)
            {
                contenedor.Estado = EstadoContenedor.Enviado;
                contenedor.NumeroTransaccion = uow.GetTransactionNumber();
                contenedor.FechaModificado = DateTime.Now;
                contenedor.FechaExpedido = DateTime.Now;

                uow.ContenedorRepository.UpdateContenedor(contenedor);

                if (contenedor.NroLpn != null)
                    uow.ManejoLpnRepository.ExpedirLpn(contenedor.NroLpn.Value, contenedor.Ubicacion, uow.GetTransactionNumber());

                if (uow.ContenedorRepository.TipoContenedorEnvase(contenedor.TipoContenedor))
                {
                    Envase envase = uow.EnvaseRepository.GetEnvase(contenedor.IdExterno, contenedor.TipoContenedor);
                    Agente agente = uow.AgenteRepository.GetAgenteContenedor(contenedor.Numero, contenedor.NumeroPreparacion);
                    string pedidosDelContenedor = string.Join("-", uow.PreparacionRepository.GetNumerosPedidosDeUnContenedor(contenedor.Numero, contenedor.NumeroPreparacion));

                    if (envase == null)
                    {
                        envase = new Envase
                        {
                            Id = contenedor.IdExterno,
                            TipoEnvase = contenedor.TipoContenedor,
                            Estado = EstadoEnvase.Expedido,
                            CodigoBarras = contenedor.CodigoBarras,
                            CodigoAgente = agente.Codigo,
                            TipoAgente = agente.Tipo,
                            Empresa = agente.Empresa,
                            NumeroTransaccion = uow.GetTransactionNumber(),
                            FechaUltimaExpedicion = DateTime.Now,
                            UsuarioUltimaExpedicion = _userId,
                            DescripcionUltimoMovimiento = ($"Expedido => Camión: {cdCamion}; Preparación: {contenedor.NumeroPreparacion}; Pedidos: {pedidosDelContenedor};").Truncate(200)
                        };

                        uow.EnvaseRepository.AddEnvase(envase);
                    }
                    else
                    {
                        envase.NumeroTransaccion = uow.GetTransactionNumber();
                        envase.Estado = EstadoEnvase.Expedido;
                        envase.FechaUltimaExpedicion = DateTime.Now;
                        envase.UsuarioUltimaExpedicion = _userId;
                        envase.DescripcionUltimoMovimiento = ($"Expedido => Camión: {cdCamion}; Preparación: {contenedor.NumeroPreparacion}; Pedidos: {pedidosDelContenedor};").Truncate(200);
                        uow.EnvaseRepository.UpdateEnvase(envase);
                    }
                }
            }
        }

        public virtual void CargarContenedoresCamion(IUnitOfWork uow, Camion camion, ContenedorExpedir datosContenedor)
        {
            var ubicacionDestino = uow.PuertaEmbarqueRepository.GetUbicacionPuertaEmbarque(camion.Puerta);
            var contenedor = uow.ContenedorRepository.GetContenedoresEnPreparacion(datosContenedor.NumeroPreparacion, datosContenedor.NumeroContenedor);

            MoverContenedor(uow, contenedor, ubicacionDestino);

            if (camion != null && (camion.Estado == Enums.CamionEstado.AguardandoCarga || camion.Estado == Enums.CamionEstado.SinOrdenDeTrabajo))
            {
                camion.Estado = Enums.CamionEstado.Cargando;
                camion.NumeroTransaccion = uow.GetTransactionNumber();
                camion.FechaModificacion = DateTime.Now;
                uow.CamionRepository.UpdateCamion(camion);
            }

            if (contenedor != null)
            {
                var ut = contenedor.NumeroUnidadTransporte;

                contenedor.NumeroUnidadTransporte = null;
                contenedor.Estado = EstadoContenedor.EnCamion;
                contenedor.Ubicacion = ubicacionDestino;
                contenedor.CodigoCamion = camion.Id;
                contenedor.CodigoFuncionarioExpedicion = _userId;
                contenedor.NumeroTransaccion = uow.GetTransactionNumber();
                contenedor.FechaModificado = DateTime.Now;

                uow.ContenedorRepository.UpdateContenedor(contenedor);
                uow.SaveChanges();

                if (!uow.ContenedorRepository.AnyContenedorEnUt(ut))
                    uow.UnidadMedidaRepository.RemoveUt(ut);

                if (uow.ContenedorRepository.TipoContenedorEnvase(contenedor.TipoContenedor))
                {
                    var envase = uow.EnvaseRepository.GetEnvase(contenedor.IdExterno, contenedor.TipoContenedor);

                    var agente = uow.AgenteRepository.GetAgente(datosContenedor.CodigoEmpresa, datosContenedor.CodigoCliente);

                    if (envase == null)
                    {
                        envase = new Envase
                        {
                            Id = contenedor.IdExterno,
                            TipoEnvase = contenedor.TipoContenedor,
                            Estado = EstadoEnvase.EnCamion,
                            CodigoBarras = contenedor.CodigoBarras,
                            CodigoAgente = _nuPredio,
                            TipoAgente = TipoAgenteDb.Deposito,
                            Empresa = null,
                            NumeroTransaccion = uow.GetTransactionNumber(),
                            FechaUltimaCargaEnCamion = DateTime.Now,
                            UsuarioUltimaCargaEnCamion = _userId,
                            DescripcionUltimoMovimiento = $"Cargado => Camión : {camion.Id} ; Preparación : {contenedor.NumeroPreparacion} ;",
                        };

                        uow.EnvaseRepository.AddEnvase(envase);
                    }
                    else
                    {
                        envase.CodigoAgente = agente.Codigo;
                        envase.TipoAgente = agente.Tipo;
                        envase.Empresa = agente.Empresa;
                        envase.Estado = EstadoEnvase.EnCamion;
                        envase.NumeroTransaccion = uow.GetTransactionNumber();
                        envase.FechaUltimaCargaEnCamion = DateTime.Now;
                        envase.UsuarioUltimaCargaEnCamion = _userId;
                        envase.DescripcionUltimoMovimiento = $"Cargado => Camión : {camion.Id} ; Preparación : {contenedor.NumeroPreparacion} ;";

                        uow.EnvaseRepository.UpdateEnvase(envase);
                    }
                }
            }
        }

        public virtual bool FacturarContenedores(IUnitOfWork uow, Camion camion, int _userId)
        {
            var tieneFacturacion = false;            
            var cargas = uow.CargaCamionRepository.GetsCargasCamion(camion);

            foreach (var carga in cargas)
            {
                var contenedores = uow.ContenedorRepository.GetContenedoresCarga(carga);

                foreach (var contenedor in contenedores)
                {
                    var detPick = uow.PreparacionRepository.GetDetallePreparacion(contenedor.NumeroPreparacion, contenedor.Numero);
                    var pedido = uow.PedidoRepository.GetPedido(detPick.Empresa, detPick.Cliente, detPick.Pedido);

                    if (pedido.ConfiguracionExpedicion.IsFacturacionRequerida)
                    {
                        tieneFacturacion = true;
                        contenedor.CamionFacturado = camion.Id;
                    }
                    else
                        contenedor.CamionFacturado = 0;

                    contenedor.NumeroTransaccion = uow.GetTransactionNumber();
                    uow.ContenedorRepository.UpdateContenedor(contenedor);
                }
            }

            return tieneFacturacion;
        }
    }
}
