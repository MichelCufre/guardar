using System;
using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Interfaces;
using WIS.Domain.StockEntities;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class StockMapper : IStockMapper
    {
        public virtual FiltrosStock Map(StockRequest request)
        {
            FiltrosStock model = new FiltrosStock();

            model.Empresa = request.Empresa;
            model.Pagina = request.Pagina;

            model.Ubicacion = request.Filtros?.Ubicacion;
            model.Producto = request.Filtros?.Producto;
            model.Clase = request.Filtros?.CodigoClase;
            model.Familia = request.Filtros?.CodigoFamilia;
            model.Ramo = request.Filtros?.Ramo;
            model.TipoManejoFecha = request.Filtros?.TipoManejoFecha;
            model.ManejoIdentificador = request.Filtros?.ManejoIdentificador;
            model.Predio = request.Filtros?.Predio;
            model.Averia = request.Filtros?.Averia == null ? false : (bool)request.Filtros?.Averia;
            model.GrupoConsulta = request.Filtros?.GrupoConsulta;

            return model;
        }

        public virtual List<AjusteStock> Map(AjustesDeStockRequest request, InterfazEjecucion ejecucion)
        {
            List<AjusteStock> models = new List<AjusteStock>();

            foreach (var reg in request.Ajustes)
            {
                AjusteStock model = new AjusteStock
                {
                    Ubicacion = reg.Ubicacion,
                    Empresa = request.Empresa,
					Producto = reg.Producto,
                    Faixa = 1,
					Identificador = (string.IsNullOrEmpty(reg.Identificador) ? ManejoIdentificadorDb.IdentificadorProducto : reg.Identificador)?.Trim()?.ToUpper(),
                    FechaVencimiento = reg.FechaVencimiento,
                    QtMovimiento = reg.Cantidad,
					TipoAjuste = string.IsNullOrEmpty(reg.TipoAjuste) ? TipoAjusteDb.Stock : reg.TipoAjuste,
                    CdMotivoAjuste = reg.MotivoAjuste,
                    DescMotivo = reg.DescripcionMotivo,
                    FechaMotivo = reg.FechaMotivo,
                    Serializado = reg.Serializado,
                    Funcionario = ejecucion.UserId,
					NuInterfazEjecucion = null,
					IdProcesar = "N",
                    IdProcesado = "N",
					FechaRealizado = DateTime.Now
                };

                models.Add(model);
            }
            return models;
        }

        public virtual List<TransferenciaStock> Map(TransferenciaStockRequest request)
        {
            var models = new List<TransferenciaStock>();

            foreach (var r in request.Transferencias)
            {
                models.Add(new TransferenciaStock
                {
                    Ubicacion = r.Ubicacion,
                    UbicacionDestino = r.UbicacionDestino,
                    Producto = r.CodigoProducto,
                    Identificador = r.Identificador?.Trim()?.ToUpper(),
                    Faixa = 1,
                    Cantidad = r.Cantidad,
					Empresa = request.Empresa,
					EtiquetaInterna = r.EtiquetaInterna,
					EtiquetaOperacion = r.EtiquetaOperacion
				});
            }
            return models;
        }
    }
}
