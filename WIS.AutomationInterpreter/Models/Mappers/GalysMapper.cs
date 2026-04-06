using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Automation.Galys;
using WIS.AutomationInterpreter.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Security;
using static WIS.Domain.Automatismo.Dtos.ConfirmacionMovimientoStockRequest;

namespace WIS.AutomationInterpreter.Models.Mappers
{
	public class GalysMapper : IGalysMapper
	{
		protected string _secret;

		public GalysMapper(IConfiguration config)
		{
			_secret = config.GetSection("IntegrationSettings:Secret")?.Value;
		}

		public virtual ProductoGalysRequest Map(ProductoAutomatismoRequest request)
		{
            return new ProductoGalysRequest()
            {
                estado = request.TipoOperacion == TipoOperacionDb.Sobrescritura ? "A" : request.TipoOperacion,
                codAlmacen = request.Predio,
                codArticulo = request.Codigo,
                denomArticulo = request.Descripcion,
                dimensionXEnvase = (int)(request.Ancho ?? 0),
                dimensionYEnvase = (int)(request.Altura ?? 0),
                dimensionZEnvase = (int)(request.Profundidad ?? 0),
                gestionCaducidadEntrada = request.TipoManejoFecha == ManejoFechaProductoDb.Expirable || request.TipoManejoFecha == ManejoFechaProductoDb.Fifo,
                gestionLoteEntrada = request.ManejoIdentificador != ManejoIdentificadorDb.Producto,
                peso = request.PesoBruto ?? 0,
                udc = request.UnidadCaja,
                udsUdc = request.CantidadUnidadCaja,
                UdsUde = (int)(request.UnidadBulto ?? 1),
                leerCdBSalidas = request.ConfirmarCodigoBarras == "S" ? true : false
            };
        }

		public virtual CodigoBarraGalysRequest Map(CodigoBarraAutomatismoRequest request)
		{
			return new CodigoBarraGalysRequest
			{
                codAlmacen = request.Predio,
                codArticulo = request.Producto,
                codBarras = request.Codigo,
            };
		}

		public virtual EntradaStockGalysRequest Map(EntradaStockAutomatismoRequest cabezal, List<EntradaStockLineaAutomatismoRequest> detalles)
		{
			var nuDetalle = 1;
			var firstDet = detalles.FirstOrDefault();
			var request = new EntradaStockGalysRequest
			{
                codAlmacen = cabezal.Predio,
                idEntrada = $"{cabezal.Empresa}~{cabezal.Ejecucion}",
                codProveedor = $"{firstDet?.TipoAgente}~{firstDet?.CodigoAgente}",
                denomProveedor = firstDet?.DescripcionAgente,
                fechaEntradaPrevista = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                listaLineas = new List<EntradaStockLineaGalysRequest>(),
            };

			foreach (var det in detalles)
			{
                request.listaLineas.Add(Map(nuDetalle, det));
                nuDetalle++;
            }

			return request;
		}

		protected virtual EntradaStockLineaGalysRequest Map(int nuDetalle, EntradaStockLineaAutomatismoRequest request)
		{
			return new EntradaStockLineaGalysRequest
			{
                fechaCaducidad = request.ManejoVencimiento == ManejoFechaProductoDb.Expirable ? request.FechaVencimiento?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                fechaEntrada = request.ManejoVencimiento == ManejoFechaProductoDb.Fifo ? request.FechaVencimiento?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                cantidad = (int)request.Cantidad,
                codArticulo = request.Producto,
                idCarro = request.EtiquetaCarro,
                lineaEntrada = request.LineaEntrada,
                lote = request.Identificador == "*" ? string.Empty : request.Identificador,
            };


		}

        public virtual SalidaStockGalysRequest Map(SalidaStockAutomatismoRequest cabezal)
        {
            var nuDetalle = 1;
            var firstDet = cabezal.Detalles.FirstOrDefault();
            var request = new SalidaStockGalysRequest
            {
                codAlmacen = firstDet?.Predio,
                codDestinatario = string.IsNullOrEmpty(firstDet?.CodigoAgente) ? "VARIOS" : $"{firstDet?.TipoAgente}~{firstDet?.CodigoAgente}",
                denomDestinatario = string.IsNullOrEmpty(firstDet?.DescripcionAgente) ? "Varios" : firstDet?.DescripcionAgente,
                fechaServicio = firstDet?.FechaEntrega?.ToString("yyyy-MM-dd hh:mm:ss"),
                tipoPedido = firstDet.TipoSalida,
                numeroPedidoCliente = $"{cabezal.Empresa}~{cabezal.Ejecucion}",//GetIdSalida(cabezal, firstDet),
                prioridad = firstDet?.Prioridad ?? 1,
                listaLineas = new List<SalidaStockLineaGalysRequest>()
            };

            foreach (var det in cabezal.Detalles)
            {
                request.listaLineas.Add(Map(nuDetalle, det));
                nuDetalle++;
            }

            return request;
        }

		protected virtual SalidaStockLineaGalysRequest Map(int nuDetalle, SalidaStockLineaAutomatismoRequest request)
		{
			return new SalidaStockLineaGalysRequest
			{
                cantidadSolicitada = (int)request.Cantidad,
                codArticulo = request.Producto,
                lineaSalida = nuDetalle,
                lote = (request.Identificador == "*" ? string.Empty : request.Identificador),
			};
		}

		public virtual NotificacionAjustesStockRequest Map(NotificacionAjusteStockGalysRequest request)
		{
			DateTime? fechaVencimiento = null;

			if (DateTime.TryParseExact(request.caducidad, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime vencimiento))
			{
				fechaVencimiento = vencimiento;
			}

			return new NotificacionAjustesStockRequest
			{
				Empresa = -1,
				Puesto = request.puesto?.ToUpper(),
				DsReferencia = $"Notificación de ajuste de stock",
				Usuario = new UsuarioRequest
				{
					LoginName = request.usuario,
					Hash = Signer.ComputeHash(_secret, request.usuario),
				},
				Ajustes = new List<AutomatismoAjusteStockRequest>
				{
					new AutomatismoAjusteStockRequest
					{
						Cantidad = request.cantidad,
						Causa = request.codigoCausa?.ToUpper(),
						FechaVencimiento = fechaVencimiento,
						Identificador = (string.IsNullOrEmpty(request.lote) ? "*": request.lote)?.Trim()?.ToUpper(),
						Predio = request.codAlmacen,
						Producto = request.codArticulo?.ToUpper(),
					},
				}
			};
		}

		public virtual ConfirmacionEntradaStockRequest Map(ConfirmacionEntradaStockGalysRequest request)
		{
			List<ConfirmacionEntradaStockLineaRequest> detalles = null;
			var idEntradaArgs = request.idEntrada.Split('~');

			if (request.listaLineas != null)
			{
				detalles = new List<ConfirmacionEntradaStockLineaRequest>();
				foreach (var linea in request.listaLineas)
				{
					DateTime? fechaVencimiento = null;

					detalles.Add(new ConfirmacionEntradaStockLineaRequest
					{
                        Cantidad = linea.cantidadEnMatricula,
                        CantidadSolicitada = linea.cantidadSolicitada,
                        FechaVencimiento = fechaVencimiento,
                        IdLinea = linea.lineaEntrada,
                        Producto = linea.codArticulo,
                    });
				}
			}

			return new ConfirmacionEntradaStockRequest
			{
                IdEntrada = idEntradaArgs[1],
                DsReferencia = $"Confirmación de entrada de stock {request.idEntrada}",
				Empresa = int.Parse(idEntradaArgs[0]),
				Predio = request.codAlmacen,
				Puesto = request.puesto,
                EstadoEntrada = request.estadoEntrada,
                Usuario = new UsuarioRequest
				{
					LoginName = request.usuario,
					Hash = Signer.ComputeHash(_secret, request.usuario)
				},
                Detalles = detalles,
            };
		}

		public virtual ConfirmacionSalidaStockRequest Map(ConfirmacionSalidaStockGalysRequest request)
		{
            var detalles = new List<ConfirmacionSalidaStockLineaRequest>();
            var contenedores = new List<ConfirmacionSalidaStockContenedorRequest>();
            var idSalidaArgs = request.numeroPedidoCliente.Split('~');

            if (request.listaLineas != null && request.listaLineas.Count > 0)
            {
                foreach (var linea in request.listaLineas)
                {

                    detalles.Add(new ConfirmacionSalidaStockLineaRequest
                    {
                        IdLinea = linea.lineaSalida,
                        Producto = linea.codArticulo,
                        CantidadSolicitada = linea.cantidadSolicitada,
                        CantidadPreparada = linea.cantidadEnBulto
                    });
                }
            }

            if (request.listaMatriculas != null && request.listaMatriculas.Count > 0)
            {
                foreach (var lineaMatricula in request.listaMatriculas)
                {
                    var contenedor = new ConfirmacionSalidaStockContenedorRequest()
                    {
                        IdMatricula = lineaMatricula.IdMatricula
                    };

                    foreach (var lineaProducto in lineaMatricula.listaProductos)
                    {
                        DateTime? fechaVencimiento = null;

                        if (DateTime.TryParseExact(lineaProducto.fechaCaducidad, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime vencimiento))
                        {
                            fechaVencimiento = vencimiento;
                        }

                        DateTime? fechaEntrada = null;

                        if (DateTime.TryParseExact(lineaProducto.fechaEntrada, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime entrada))
                        {
                            fechaEntrada = entrada;
                        }

                        contenedor.Productos.Add(new ConfirmacionSalidaStockContenedorDetalleRequest
                        {
                            Producto = lineaProducto.codArticulo,
                            Cantidad = lineaProducto.cantidad,
                            FechaVencimiento = fechaVencimiento,
                            FechaEntrada = fechaEntrada,
                            IdLinea = lineaProducto.lineaSalida,
                            Identificador = (string.IsNullOrEmpty(lineaProducto.lote) ? "*" : lineaProducto.lote),
                        });
                    }

                    contenedores.Add(contenedor);
                }
            }

            var confirmacionSalida = new ConfirmacionSalidaStockRequest
            {
                IdSalida = idSalidaArgs[1],
                Detalles = detalles,
                Contenedores = contenedores,
                DsReferencia = $"Confirmación órden de salida: {request.numeroPedidoCliente}",
                Empresa = int.Parse(idSalidaArgs[0]),
                Predio = request.codAlmacen,
                Puesto = request.puesto,
                EstadoSalida = request.estadoSalida,
                Usuario = new UsuarioRequest
                {
                    LoginName = request.usuario,
                    Hash = Signer.ComputeHash(_secret, request.usuario),
                },
            };

            return confirmacionSalida;
        }

        public virtual ConfirmacionMovimientoStockRequest Map(ConfirmacionMovimientoStockGalysRequest request)
        {
            var detalles = new List<ConfirmacionMovimientoStockLineaRequest>();

            int empresa = -1;
            string idPeticion = "";

            if (!string.IsNullOrEmpty(request.listaProductos.FirstOrDefault()?.idPeticion))
            {
                string[] listSplit = request.listaProductos.FirstOrDefault()?.idPeticion?.Split("~");

                if (listSplit.Count() > 1)
                {
                    empresa = int.Parse(listSplit[0]);
                    idPeticion = listSplit[1];
                }
                else
                    idPeticion = request.listaProductos.FirstOrDefault()?.idPeticion;
            }

            foreach (var linea in request.listaProductos)
            {
                DateTime? fechaVencimiento = null;

                if (DateTime.TryParseExact(linea?.fechaCaducidad, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime vencimiento))
                {
                    fechaVencimiento = vencimiento;
                }

                detalles.Add(new ConfirmacionMovimientoStockLineaRequest
                {
                    IdLinea = linea.lineaOrden,
                    IdPeticion = idPeticion,
                    Cantidad = linea.cantidad,
                    CodigoCausa = linea.codigoCausa,
                    FechaVencimiento = fechaVencimiento,
                    Identificador = (string.IsNullOrEmpty(linea.lote) ? "*" : linea.lote),
                    Producto = linea.codArticulo,
                });
            }

            return new ConfirmacionMovimientoStockRequest
            {
                Automatismo = request.codAlmacen,
                IdEntrada = idPeticion,
                Empresa = empresa,
                DsReferencia = $"Movimiento de stock. Tipo {request.tipoMovimiento}. Id: {request.listaProductos.FirstOrDefault().idPeticion}",
                TipoMovimiento = request.tipoMovimiento,
                Puesto = request.puesto,
                Usuario = new UsuarioRequest
                {
                    LoginName = request.usuario,
                    Hash = Signer.ComputeHash(_secret, request.usuario),
                },

                Detalles = detalles,
            };
        }
    }
}
