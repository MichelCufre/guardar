using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Picking;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Produccion
{
    public class EnsambladoFormulaContenedor
    {
        protected IUnitOfWork uow;
        protected Pedido pedidoEnsamblado;
        protected Pedido pedidoOriginal;
        protected Contenedor contenedor;
        protected int userId;
        protected string predio;
        protected readonly IBarcodeService _barcodeService;

        public EnsambladoFormulaContenedor(IUnitOfWork uow, Pedido pedidoEnsamblado, Pedido pedidoOriginal, Contenedor contenedor, int userId, string predio, IBarcodeService barcodeService)
        {
            this.uow = uow;
            this.pedidoEnsamblado = pedidoEnsamblado;
            this.pedidoOriginal = pedidoOriginal;
            this.contenedor = contenedor;
            this.userId = userId;
            this.predio = predio;
            this._barcodeService = barcodeService;
        }

        public virtual void ProcesarInsumos(List<ProductoEnsamblado> colInsumos)
        {
            var carga = new Carga
            {
                Descripcion = "Carga creada para ensamblado de formula",
                Preparacion = contenedor.NumeroPreparacion,
                Ruta = (short)pedidoOriginal.Ruta,
                FechaAlta = DateTime.Now
            };

            uow.CargaRepository.AddCarga(carga);

            var tipoContenedor = BarcodeDb.TIPO_CONTENEDOR_W;
            string idExterno;
            string codigoBarras;
            do
            {
                idExterno = uow.ContenedorRepository.GetUltimaSecuenciaTipoContenedor(tipoContenedor).ToString();
                codigoBarras = _barcodeService.GenerateBarcode(idExterno, tipoContenedor);
            }
            while (uow.ContenedorRepository.ExisteContenedorActivoByCodigoBarras(codigoBarras));

            var newContenedor = new Contenedor
            {
                NumeroPreparacion = contenedor.NumeroPreparacion,
                Numero = uow.ContenedorRepository.GetNextNuContenedor(),
                TipoContenedor = tipoContenedor,
                Estado = EstadoContenedor.EnsambladoKit,
                Ubicacion = contenedor.Ubicacion,
                CodigoSubClase = contenedor.CodigoSubClase,
                FechaAgregado = DateTime.Now,
                NumeroTransaccion = uow.GetTransactionNumber(),
                IdExterno = idExterno,
                CodigoBarras = codigoBarras,
            };

            uow.ContenedorRepository.AddContenedor(newContenedor);

            foreach (var insumo in colInsumos)
            {
                decimal saldo = ((insumo.QT_SALIDA * insumo.QT_COMPLETA));
                var colDetPicking = uow.PreparacionRepository.GetDetallesPreparacionPorInsumo(contenedor.NumeroPreparacion, contenedor.Numero, pedidoEnsamblado.Id, pedidoEnsamblado.Cliente, pedidoEnsamblado.Empresa, insumo.CD_PRODUTO);

                foreach (DetallePreparacion pik in colDetPicking)
                {
                    if (saldo == 0)
                        break;

                    decimal qtAjustar = saldo;

                    if (saldo >= pik.CantidadPreparada)
                    {
                        qtAjustar = (pik.CantidadPreparada ?? 0);
                        saldo -= qtAjustar;
                        pik.Contenedor = newContenedor;
                        pik.Transaccion = uow.GetTransactionNumber();
                    }
                    else if (pik.CantidadPreparada > saldo)
                    {
                        qtAjustar = saldo;

                        DetallePreparacion detalle = new DetallePreparacion();
                        detalle.NumeroPreparacion = pik.NumeroPreparacion;
                        detalle.Producto = pik.Producto;
                        detalle.Faixa = 1;
                        detalle.Lote = pik.Lote;
                        detalle.Empresa = pik.Empresa;
                        detalle.Ubicacion = pik.Ubicacion;
                        detalle.NumeroSecuencia = pik.NumeroSecuencia;
                        detalle.Pedido = pik.Pedido;
                        detalle.Cliente = pik.Cliente;
                        detalle.EspecificaLote = pik.EspecificaLote;
                        detalle.Agrupacion = Agrupacion.Pedido;
                        detalle.Cantidad = saldo;
                        detalle.CantidadPreparada = saldo;
                        detalle.FechaAlta = DateTime.Now;
                        detalle.Carga = carga.Id;
                        detalle.Contenedor = newContenedor;
                        detalle.Usuario = userId;
                        detalle.CantidadPickeo = saldo;
                        detalle.FechaPickeo = DateTime.Now;
                        detalle.NumeroContenedorPickeo = newContenedor.Numero;
                        detalle.UsuarioPickeo = userId;
                        detalle.FechaModificacion = null;
                        detalle.Transaccion = uow.GetTransactionNumber();

                        uow.PreparacionRepository.AddDetallePreparacion(detalle);

                        pik.CantidadPreparada = (pik.CantidadPreparada - saldo);
                        pik.Cantidad = (pik.Cantidad - saldo);
                        pik.Transaccion = uow.GetTransactionNumber();

                        saldo = 0;
                    }

                    uow.PreparacionRepository.UpdateDetallePreparacion(pik);

                    Stock stockBaja = uow.StockRepository.GetStock(pik.Empresa, pik.Producto, pik.Faixa, contenedor.Ubicacion, pik.Lote);

                    if (stockBaja == null)
                        throw new Exception("PRD096_Sec0_Error_Error02");

                    stockBaja.Cantidad -= qtAjustar;
                    stockBaja.ReservaSalida -= qtAjustar;
                    stockBaja.NumeroTransaccion = uow.GetTransactionNumber();

                    uow.StockRepository.UpdateStock(stockBaja);

                    string nombreFormula = uow.FormulaRepository.GetFormula(insumo.CD_PRDC_DEFINICION).Nombre;
                    AjusteStock ajuste = new AjusteStock
                    {
                        NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                        Producto = insumo.CD_PRODUTO,
                        Ubicacion = contenedor.Ubicacion,
                        Faixa = pik.Faixa,
                        Identificador = pik.Lote,
                        Empresa = pik.Empresa,
                        FechaRealizado = DateTime.Now,
                        TipoAjuste = TipoAjusteDb.Produccion,
                        QtMovimiento = (-1) * qtAjustar,
                        DescMotivo = insumo.CD_PRDC_DEFINICION,
                        CdMotivoAjuste = MotivoAjusteDb.Formula,
                        Predio = predio,
                        NuTransaccion = uow.GetTransactionNumber(),
                        NuDocumento = pedidoOriginal.Id,
                        Serializado = nombreFormula
                    };

                    uow.AjusteRepository.Add(ajuste);

                    uow.SaveChanges();
                }
            }
        }

        public virtual void ProcesarProductosFinales(List<ProductoEnsamblado> colProductosFinales, int cargaOriginal)
        {
            foreach (ProductoEnsamblado productoFinal in colProductosFinales)
            {
                decimal saldo = (productoFinal.QT_SALIDA);

                DetallePreparacion detalle = new DetallePreparacion();
                detalle.NumeroPreparacion = contenedor.NumeroPreparacion;
                detalle.Producto = productoFinal.CD_PRODUTO;
                detalle.Faixa = 1;
                detalle.Lote = productoFinal.NU_IDENTIFICADOR;
                detalle.Empresa = productoFinal.CD_EMPRESA;
                detalle.Ubicacion = contenedor.Ubicacion;
                detalle.Pedido = pedidoOriginal.Id;
                detalle.Cliente = pedidoOriginal.Cliente;
                detalle.EspecificaLote = "S";
                detalle.Agrupacion = Agrupacion.Pedido;
                detalle.Cantidad = saldo;
                detalle.CantidadPreparada = saldo;
                detalle.FechaAlta = DateTime.Now;
                detalle.Carga = cargaOriginal;
                detalle.Contenedor = contenedor;
                detalle.Usuario = userId;
                detalle.CantidadPickeo = saldo;
                detalle.FechaPickeo = DateTime.Now;
                detalle.NumeroContenedorPickeo = contenedor.Numero;
                detalle.UsuarioPickeo = userId;
                detalle.FechaModificacion = null;
                detalle.Transaccion = uow.GetTransactionNumber();

                uow.PreparacionRepository.AddDetallePreparacion(detalle);

                Stock stockAlta = uow.StockRepository.GetStock(productoFinal.CD_EMPRESA, productoFinal.CD_PRODUTO, productoFinal.CD_FAIXA, contenedor.Ubicacion, productoFinal.NU_IDENTIFICADOR);

                if (stockAlta == null)
                {
                    stockAlta = new Stock
                    {
                        Ubicacion = contenedor.Ubicacion,
                        Empresa = productoFinal.CD_EMPRESA,
                        Producto = productoFinal.CD_PRODUTO,
                        Faixa = productoFinal.CD_FAIXA,
                        Identificador = productoFinal.NU_IDENTIFICADOR,
                        CantidadTransitoEntrada = 0,
                        ReservaSalida = saldo,
                        Cantidad = saldo,
                        Averia = "N",
                        Inventario = "R",
                        ControlCalidad = EstadoControlCalidad.Controlado,
                        FechaInventario = DateTime.Now,
                        NumeroTransaccion = uow.GetTransactionNumber()
                    };

                    uow.StockRepository.AddStock(stockAlta);
                }
                else
                {
                    stockAlta.Cantidad += saldo;
                    stockAlta.ReservaSalida += saldo;
                    stockAlta.NumeroTransaccion = uow.GetTransactionNumber();
                    stockAlta.FechaModificacion = DateTime.Now;

                    uow.StockRepository.UpdateStock(stockAlta);
                }

                string nombreFormula = uow.FormulaRepository.GetFormula(productoFinal.CD_PRDC_DEFINICION).Nombre;
                AjusteStock ajuste = new AjusteStock
                {
                    NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                    Producto = productoFinal.CD_PRODUTO,
                    Ubicacion = contenedor.Ubicacion,
                    Faixa = productoFinal.CD_FAIXA,
                    Identificador = productoFinal.NU_IDENTIFICADOR,
                    Empresa = productoFinal.CD_EMPRESA,
                    FechaRealizado = DateTime.Now,
                    TipoAjuste = TipoAjusteDb.Produccion,
                    QtMovimiento = saldo,
                    DescMotivo = productoFinal.CD_PRDC_DEFINICION,
                    CdMotivoAjuste = MotivoAjusteDb.Formula,
                    Predio = predio,
                    NuTransaccion = uow.GetTransactionNumber(),
                    NuDocumento = pedidoOriginal.Id,
                    Serializado = nombreFormula
                };

                uow.AjusteRepository.Add(ajuste);
                uow.SaveChanges();
            }
        }

        public virtual void FinalizarProduccionEnsamblado()
        {
            var ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(pedidoEnsamblado.IngresoProduccion);

            if (ingreso == null)
                throw new Exception("General_Sec0_Error_IngresoNoExiste");

            if (ingreso.Situacion == SituacionDb.PRODUCCION_FINALIZADA)
                throw new Exception("General_Sec0_Error_ProdnoExisteOFinalizada");

            if (uow.PreparacionRepository.AnyPendientesEnsamblarProduccion(pedidoEnsamblado.IngresoProduccion))
                throw new Exception("PRD400_Sec0_Error_HayPendientesEnsamblar");

            ingreso.FinalizarIngreso();
            uow.ProduccionRepository.UpdateIngresoWhiteBox(ingreso);
        }
    }
}
