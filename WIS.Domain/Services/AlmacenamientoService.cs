using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class AlmacenamientoService : IAlmacenamientoService
    {
        protected readonly IBarcodeService _barcodeService;
        protected readonly IGrupoService _grupoService;
        protected readonly IIdentityService _identity;


        public AlmacenamientoService(IBarcodeService barcodeService,
            IGrupoService grupoService,
            IIdentityService identity)
        {
            this._barcodeService = barcodeService;
            this._grupoService = grupoService;
            this._identity = identity;
        }

        public virtual SugerenciaAlmacenamiento SugerirUbicacionParaProducto(IUnitOfWork uow, string predio, int nuEtiqueta, string agrupador, string producto, decimal? faixa, string lote, decimal cantidadSeparar, decimal cantidadClasificada, decimal cantidadOriginal, DateTime? vencimiento)
        {
            var inicioCalculo = DateTime.Now;
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLote(nuEtiqueta);

            if (etiqueta != null)
            {
                var criterio = GetCriterioAlmacenaje(uow, agrupador, predio, producto, faixa, lote, cantidadSeparar, cantidadClasificada, cantidadOriginal, vencimiento, etiqueta);

                var sugerencia = uow.AlmacenamientoRepository.SugerirUbicacion(criterio, inicioCalculo, false, true, uow.GetTransactionNumber(), nuEtiqueta, true);

                DeleteCriterioAlmacenajeTemp(uow, nuEtiqueta, _identity.UserId);

                return sugerencia;

            }
            else
                return null;
        }

        public virtual SugerenciaAlmacenamiento SugerirUbicacionParaEtiqueta(IUnitOfWork uow, string predio, int nuEtiqueta, string ubicacionInicio)
        {
            var inicioCalculo = DateTime.Now;
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLote(nuEtiqueta);

            if (etiqueta != null)
            {
                var criterio = GetCriterioAlmacenaje(uow, predio, nuEtiqueta, etiqueta);

                var sugerencia = uow.AlmacenamientoRepository.SugerirUbicacion(criterio, inicioCalculo, false, false, uow.GetTransactionNumber(), nuEtiqueta, true);

                DeleteCriterioAlmacenajeTemp(uow, nuEtiqueta, _identity.UserId);

                etiqueta.IdUbicacionSugerida = sugerencia?.UbicacionSugerida;

                uow.EtiquetaLoteRepository.UpdateEtiquetaLote(etiqueta);

                return sugerencia;
            }
            else
                return null;
        }

        public virtual CriterioAlmacenaje GetCriterioAlmacenaje(IUnitOfWork uow, string predio, int nuEtiqueta, EtiquetaLote etiqueta)
        {
            var agenda = uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);
            var productos = uow.AlmacenamientoRepository.GetProductosAlmacenajeFromEtiqueta(etiqueta.Numero, _identity.UserId);
            var clase = (string)null;

            if (productos.Select(p => p.Clase).Distinct().Count() == 1)
                clase = productos.FirstOrDefault()?.Clase;

            var grupo = uow.GrupoRepository.GetGrupo(etiqueta.CodigoGrupo);

            var agrupador = new AgrupadorAlmacenaje
            {
                Id = etiqueta.CodigoBarras,
            };

            var operativa = new OperativaAlmacenaje()
            {
                Codigo = agenda.TipoRecepcionInterno,
                Tipo = AlmacenamientoDb.TIPO_OPERATIVA_CLASIFICACION
            };

            var criterio = new CriterioAlmacenaje
            {
                Agrupador = agrupador,
                Clase = clase ?? grupo?.CodigoClase,
                Grupo = etiqueta.CodigoGrupo,
                Operativa = operativa,
                Predio = predio ?? agenda.Predio,
                Productos = productos,
                Referencia = etiqueta.NumeroAgenda.ToString(),
                Pallet = etiqueta.CodigoPallet,
            };
            return criterio;
        }

        protected virtual CriterioAlmacenaje GetCriterioAlmacenaje(IUnitOfWork uow, string agrupador, string predio, string producto, decimal? faixa, string lote, decimal cantidadSeparar, decimal cantidadClasificada, decimal cantidadOriginal, DateTime? vencimiento, EtiquetaLote etiqueta)
        {
            var agenda = uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);
            var productos = uow.AlmacenamientoRepository.GetProductosAlmacenajeFromEtiqueta(etiqueta.Numero, _identity.UserId, producto, lote, vencimiento, cantidadSeparar);
            var clase = (string)null;

            if (productos.Select(p => p.Clase).Distinct().Count() == 1)
                clase = productos.FirstOrDefault().Clase;

            clase = clase ?? uow.ProductoRepository.GetClaseProducto(producto, agenda.IdEmpresa);

            var productoGrupo = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(agenda.IdEmpresa, producto);
            var grupo = _grupoService.GetGrupo(productoGrupo);

            var agrupadorAlmacenaje = new AgrupadorAlmacenaje
            {
                Id = string.IsNullOrEmpty(agrupador) ? (Guid.NewGuid().ToString()) : agrupador,
            };

            var operativa = new OperativaAlmacenaje()
            {
                Codigo = agenda.TipoRecepcionInterno,
                Tipo = AlmacenamientoDb.TIPO_OPERATIVA_CLASIFICACION
            };

            var criterio = new CriterioAlmacenaje
            {
                Agrupador = agrupadorAlmacenaje,
                Clase = clase ?? grupo?.CodigoClase,
                Grupo = grupo.Id,
                Operativa = operativa,
                Predio = predio ?? agenda.Predio,
                Productos = productos,
                Referencia = etiqueta.CodigoBarras,
                Pallet = etiqueta.CodigoPallet,
            };

            return criterio;
        }

        public virtual void DeleteCriterioAlmacenajeTemp(IUnitOfWork uow, int nuEtiqueta, int userId)
        {
            uow.AlmacenamientoRepository.DeleteCriterioAlmacenajeTemp(nuEtiqueta, _identity.UserId);
        }

        public virtual void RechazarSugerenciaParaEtiqueta(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, string cdMotivoRechazo)
        {
            var etiqueta = _barcodeService.GetEtiquetaLote(sugerencia.Agrupador);

            etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(etiqueta.TipoEtiqueta, etiqueta.NumeroExterno);

            uow.AlmacenamientoRepository.RechazarSugerenciaParaEtiqueta(sugerencia, etiqueta.Numero, cdMotivoRechazo, uow.GetTransactionNumber());
        }

        public virtual void AprobarSugerenciaParaEtiqueta(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia)
        {
            var etiqueta = _barcodeService.GetEtiquetaLote(sugerencia.Agrupador);

            etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(etiqueta.TipoEtiqueta, etiqueta.NumeroExterno);

            uow.AlmacenamientoRepository.AprobarSugerenciaParaEtiqueta(sugerencia, etiqueta.Numero, uow.GetTransactionNumber());
        }

        public virtual void CancelarSugerenciaParaProducto(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, int nuEtiqueta, string producto, decimal? faixa, string lote, decimal cantidad)
        {
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLote(nuEtiqueta);
            var agenda = uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);
            var productos = uow.AlmacenamientoRepository.GetProductosAlmacenajeFromEtiqueta(nuEtiqueta);
            var productoEtiqueta = productos
                .FirstOrDefault(p => p.Codigo == sugerencia.Producto
                    && p.Lote == lote);

            faixa = faixa ?? productoEtiqueta?.Faixa;

            uow.AlmacenamientoRepository.CancelarSugerenciaParaProducto(sugerencia, agenda.IdEmpresa, producto, faixa ?? 1, lote, cantidad, uow.GetTransactionNumber());
        }

        public virtual void AprobarSugerenciaParaProducto(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, int nuEtiqueta, string producto, decimal? faixa, string lote, decimal cantidad, DateTime? vencimiento)
        {
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLote(nuEtiqueta);
            var agenda = uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);
            var productos = uow.AlmacenamientoRepository.GetProductosAlmacenajeFromEtiqueta(nuEtiqueta);
            var productoEtiqueta = productos
                .FirstOrDefault(p => p.Codigo == sugerencia.Producto
                    && p.Lote == lote);

            faixa = faixa ?? productoEtiqueta?.Faixa;

            uow.AlmacenamientoRepository.AprobarSugerenciaParaProducto(sugerencia, agenda.IdEmpresa, producto, faixa ?? 1, lote, cantidad, uow.GetTransactionNumber(), vencimiento);
        }

        public virtual SugerenciaAlmacenamiento SugerirUbicacionParaReabastecer(IUnitOfWork uow, string predio, int nuEtiqueta, string producto, int cdEmpresa, decimal? faixa, string lote, DateTime? vencimiento, bool ignorarStock, decimal cantidadSeparar, decimal cantidadClasificada, decimal cantidadOriginal)
        {
            var inicioCalculo = DateTime.Now;
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLote(nuEtiqueta);
            if (etiqueta != null)
            {
                return uow.AlmacenamientoRepository.SugerirUbicacionReabastecimiento(inicioCalculo, uow.GetTransactionNumber(), predio, nuEtiqueta, producto, cdEmpresa, faixa, lote, ignorarStock, cantidadSeparar, cantidadClasificada, cantidadOriginal, vencimiento);
            }
            else
                return null;
        }

        public virtual void CancelarSugerenciaParaReabastecimiento(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, int nuEtiqueta, string producto, int IdEmpresa, decimal? faixa, string lote, decimal cantidad)
        {
            var productos = uow.AlmacenamientoRepository.GetProductosAlmacenajeFromEtiqueta(nuEtiqueta);
            var productoEtiqueta = productos
                .FirstOrDefault(p => p.Codigo == sugerencia.Producto
                    && p.Lote == lote);

            faixa = faixa ?? productoEtiqueta?.Faixa;

            uow.AlmacenamientoRepository.CancelarSugerenciaParaReabastecimiento(sugerencia, IdEmpresa, producto, faixa ?? 1, lote, cantidad, uow.GetTransactionNumber());
        }

        public virtual void AprobarSugerenciaParaReabastecimiento(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, int numero, string producto, int IdEmpresa, decimal? faixa, DateTime? vencimiento, string lote, decimal cantidad)
        {
            uow.AlmacenamientoRepository.AprobarSugerenciaParaReabastecimiento(sugerencia, IdEmpresa, producto, faixa ?? 1, lote, cantidad, uow.GetTransactionNumber(), vencimiento);
        }
    }
}
