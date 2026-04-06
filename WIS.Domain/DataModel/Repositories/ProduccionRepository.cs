using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.DTOs;
using WIS.Domain.Produccion.Enums;
using WIS.Domain.Produccion.Interfaces.Entrada;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class ProduccionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly ProduccionMapper _mapper;
        protected readonly IDapper _dapper;

        public ProduccionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new ProduccionMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyIngreso(string nroIngreso)
        {
            return this._context.T_PRDC_INGRESO
                .AsNoTracking()
                .Any(d => d.NU_PRDC_INGRESO == nroIngreso);
        }

        public virtual bool AnyProductoProducido(string nroIngreso, int empresa, string producto, string identificador, decimal faixa)
        {
            return this._context.T_PRDC_LINEA_PRODUCIDO
                .AsNoTracking()
                .Any(d => d.NU_PRDC_INGRESO == nroIngreso
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.NU_IDENTIFICADOR == identificador
                    && d.CD_FAIXA == faixa);
        }

        public virtual bool AnyProduccionParaFormula(string formula)
        {
            return this._context.T_PRDC_INGRESO.AsNoTracking().Any(d => d.CD_PRDC_DEFINICION == formula);
        }

        public virtual bool AnyProduccionActivaParaFormula(string formula)
        {
            return this._context.T_PRDC_INGRESO.AsNoTracking().Any(d => d.CD_PRDC_DEFINICION == formula && d.CD_SITUACAO != SituacionDb.PRODUCCION_FINALIZADA);
        }

        public virtual bool LineaOcupadaPorIngreso(ILinea linea, TipoProduccionIngreso tipoIngreso)
        {
            var tipo = this._mapper.MapTipoIngresoToString(tipoIngreso);

            return this._context.T_PRDC_INGRESO
                .Any(d => d.NU_PRDC_INGRESO == linea.NumeroIngreso
                    && d.CD_SITUACAO == SituacionDb.PRODUCCION_INICIADA
                    && d.ND_TIPO == tipo);
        }

        public virtual bool ExisteIngresoActivoEnLinea(string numeroLinea)
        {
            return _context.T_PRDC_INGRESO
                .Any(x => x.CD_PRDC_LINEA == numeroLinea
                    && x.CD_SITUACAO != SituacionDb.PRODUCCION_FINALIZADA);
        }

        public virtual void AnyDetallesSalidaManejaLoteOVencimiento(string nuIngresoProduccion, out bool manejaLote, out bool manjecaVencimiento)
        {
            string[] manejaIdentificador = new string[] { ManejoIdentificadorDb.Lote, ManejoIdentificadorDb.Serie };
            var productoDetallesSalida = _context.T_PRDC_DET_INGRESO_TEORICO.Where(x => x.NU_PRDC_INGRESO == nuIngresoProduccion && x.TP_REGISTRO == CIngresoProduccionDetalleTeorico.TipoDetalleSalida)
                 .Join(_context.T_PRODUTO,
                 pdit => new { pdit.CD_EMPRESA, pdit.CD_PRODUTO },
                 p => new { CD_EMPRESA = (int?)p.CD_EMPRESA, p.CD_PRODUTO },
                 (pdit, p) => p).Where(x => manejaIdentificador.Contains(x.ID_MANEJO_IDENTIFICADOR) || x.TP_MANEJO_FECHA == ManejoFechaProductoDb.Expirable);

            manejaLote = productoDetallesSalida.Any(x => manejaIdentificador.Contains(x.ID_MANEJO_IDENTIFICADOR));
            manjecaVencimiento = productoDetallesSalida.Any(x => x.TP_MANEJO_FECHA == ManejoFechaProductoDb.Expirable);

        }

        public virtual bool AnyIngresoByIdExternoEmpresa(string idExterno, int empresa)
        {
            return this._context.T_PRDC_INGRESO
                .AsNoTracking()
                .Any(d => d.ID_PRODUCCION_EXTERNO == idExterno && d.CD_EMPRESA == empresa);
        }

        public virtual bool AnyInsumoTeorico(string idIngreso, int empresa, string producto, string tipoRegistro, string identificador = null)
        {
            return this._context.T_PRDC_DET_INGRESO_TEORICO
                .AsNoTracking()
                .Any(d => d.NU_PRDC_INGRESO == idIngreso
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.TP_REGISTRO == tipoRegistro
                    && (!string.IsNullOrEmpty(identificador) ? d.NU_IDENTIFICADOR == identificador : true));
        }

        public virtual bool AnyInsumoPlanificacion(string idIngreso, int empresa, string producto, bool planificacionPedido, string identificador = null)
        {
            if (planificacionPedido)
            {
                return this._context.V_PRDC_PLANIFICACION_PEDIDO
                    .AsNoTracking()
                    .Any(d => d.NU_PRDC_INGRESO == idIngreso
                        && d.CD_EMPRESA == empresa
                        && d.CD_PRODUTO == producto
                        && (!string.IsNullOrEmpty(identificador) ? d.NU_IDENTIFICADOR == identificador : true));
            }
            else
            {
                return this._context.V_PRDC_PLANIFICACION_INSUMO
                    .AsNoTracking()
                    .Any(d => d.NU_PRDC_INGRESO == idIngreso
                        && d.CD_EMPRESA == empresa
                        && d.CD_PRODUTO == producto
                        && (!string.IsNullOrEmpty(identificador) ? d.NU_IDENTIFICADOR == identificador : true));
            }
        }
        #endregion

        #region Get

        public virtual IIngreso GetIngreso(string nroIngreso)
        {
            T_PRDC_INGRESO ingreso = this._context.T_PRDC_INGRESO
                .Include("T_PRDC_DEFINICION")
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == nroIngreso)
                .FirstOrDefault();

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

            var consumido = this._context.T_PRDC_LINEA_CONSUMIDO
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            var producido = this._context.T_PRDC_LINEA_PRODUCIDO
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            return this._mapper.MapIngresoEntityToObject(ingreso, consumido, producido);
        }

        public virtual IIngreso GetIngresoByIdExterno(string idProduccionExterno)
        {
            T_PRDC_INGRESO ingreso = this._context.T_PRDC_INGRESO
                .AsNoTracking()
                .Where(d => d.ID_PRODUCCION_EXTERNO == idProduccionExterno)
                .FirstOrDefault();

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

            var consumido = this._context.T_PRDC_LINEA_CONSUMIDO
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            var producido = this._context.T_PRDC_LINEA_PRODUCIDO
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            return this._mapper.MapIngresoEntityToObject(ingreso, consumido, producido);
        }

        public virtual IIngresoPanel GetIngresoPanel(string nroIngreso)
        {
            T_PRDC_INGRESO ingreso = this._context.T_PRDC_INGRESO
                .Include("T_PRDC_DEFINICION")
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == nroIngreso
                    && d.ND_TIPO != TipoIngresoProduccion.Colector).FirstOrDefault();

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

            var linea = this._context.T_PRDC_LINEA
                .AsNoTracking()
                .Where(d => d.CD_PRDC_LINEA == ingreso.CD_PRDC_LINEA)
                .FirstOrDefault();

            var consumido = this._context.T_PRDC_LINEA_CONSUMIDO
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            var producido = this._context.T_PRDC_LINEA_PRODUCIDO
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            var pasadas = this._context.T_PRDC_INGRESO_PASADA
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            return this._mapper.MapIngresoPanelEntityToObject(ingreso, linea, consumido, producido, pasadas);
        }

        public virtual IngresoWhiteBox GetIngresoWhiteBox(string nroIngreso)
        {
            T_PRDC_INGRESO ingreso = this._context.T_PRDC_INGRESO
                .Include("T_PRDC_DEFINICION")
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == nroIngreso && d.ND_TIPO == TipoIngresoProduccion.PanelWeb)
                .FirstOrDefault();

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

            var linea = this._context.T_PRDC_LINEA
                .AsNoTracking()
                .Where(d => d.CD_PRDC_LINEA == ingreso.CD_PRDC_LINEA)
                .FirstOrDefault();

            var consumido = this._context.T_PRDC_LINEA_CONSUMIDO
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            var producido = this._context.T_PRDC_LINEA_PRODUCIDO
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            var pasadas = this._context.T_PRDC_INGRESO_PASADA
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            var documento = this._context.T_PRDC_INGRESO_DOCUMENTO
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO);

            return this._mapper.MapIngresoWhiteBoxEntityToObject(ingreso, linea, consumido, producido, pasadas, documento);
        }

        public virtual IngresoBlackBox GetIngresoBlackBox(string nroIngreso)
        {
            T_PRDC_INGRESO ingreso = this._context.T_PRDC_INGRESO
                .Include("T_PRDC_DEFINICION")
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == nroIngreso)
                .FirstOrDefault();

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

            var linea = this._context.T_PRDC_LINEA
                .AsNoTracking()
                .Where(d => d.CD_PRDC_LINEA == ingreso.CD_PRDC_LINEA)
                .FirstOrDefault();

            var consumido = this._context.T_PRDC_LINEA_CONSUMIDO
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            var producido = this._context.T_PRDC_LINEA_PRODUCIDO
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            var pasadas = this._context.T_PRDC_INGRESO_PASADA
                .AsNoTracking()
                .Where(d => d.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO)
                .ToList();

            return this._mapper.MapIngresoBlackBoxEntityToObject(ingreso, linea, consumido, producido, pasadas);
        }

        public virtual string GetUbicacionSemiacabado(string predio)
        {
            var ubicacion = this._context.T_ENDERECO_ESTOQUE
                .FirstOrDefault(x => x.NU_PREDIO == predio
                    && x.CD_AREA_ARMAZ == AreaUbicacionDb.SEMIACABADO);

            return ubicacion?.CD_ENDERECO;
        }

        public virtual string GetUbicacionConsumible(string predio)
        {
            var ubicacion = this._context.T_ENDERECO_ESTOQUE
                .FirstOrDefault(x => x.NU_PREDIO == predio
                    && x.CD_AREA_ARMAZ == AreaUbicacionDb.CONSUMIBLE);

            return ubicacion?.CD_ENDERECO;
        }

        public virtual string GetPedidoProduccion(string ingreso)
        {
            return this._context.T_PEDIDO_SAIDA.AsNoTracking().FirstOrDefault(l => l.NU_PEDIDO == ingreso).NU_PREDIO;
        }

        public virtual long ObtenerNumeroLineaConsumoHistorica()
        {
            return this._context.GetNextSequenceValueLong(_dapper, "S_HIST_PRDC_ING_CONS");
        }

        public virtual long ObtenerNumeroLineaProducidoHistorica()
        {
            return this._context.GetNextSequenceValueLong(_dapper, "S_HIST_PRDC_ING_PROD");
        }

        public virtual long ObtenerNumeroPasadaHistorica()
        {
            return this._context.GetNextSequenceValueLong(_dapper, "S_HIST_PRDC_ING_PASADA");
        }

        public virtual string GetNumeroIngreso()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_INGRESO_PRODUCCION").ToString();
        }

        public virtual List<string> GetNumerosIngresos()
        {
            return this._context.T_PRDC_INGRESO.Select(x => x.NU_PRDC_ORIGINAL).ToList();
        }

        public virtual InterfazEntradaProduccion GetInterfazEntradaProduccionPorEjecucion(long nroEjecucion)
        {
            var ejecucionEntity = this._context.I_E_PRDC_SALIDA_PRD
                .Where(d => d.NU_INTERFAZ_EJECUCION == nroEjecucion)
                .AsNoTracking().FirstOrDefault();

            return this._mapper.MapToInterfazEntradaProduccion(ejecucionEntity);
        }

        public virtual List<InterfazEntradaProduccionInsumo> GetInsumoConsumidoInterfaceEntradaProduccionPorEjecucion(long nroEjecucion, string movimientoConsumo)
        {
            List<InterfazEntradaProduccionInsumo> detalles = new List<InterfazEntradaProduccionInsumo>();

            var insumosEntity = this._context.I_E_PRDC_SALIDA_PRD_INSUMO
                .Where(d => d.NU_INTERFAZ_EJECUCION == nroEjecucion && d.ND_ACCION_MOVIMIENTO == movimientoConsumo)
                .AsNoTracking().ToList();

            foreach (var entity in insumosEntity)
            {
                detalles.Add(this._mapper.MapToInterfazEntradaProduccionInsumo(entity));
            }

            return detalles;
        }

        public virtual List<DetallePedidoE> GetCantidadDetallePedidosSumados(string numeroingreso)
        {
            List<DetallePedidoE> detalles = new List<DetallePedidoE>();

            var insumosEntity = this._context.V_PEDIDO_SAIDA_KIT260
                .Where(d => d.NU_PRDC_INGRESO == numeroingreso)
                .GroupBy(x => new { x.CD_PRODUTO, x.CD_EMPRESA, x.NU_IDENTIFICADOR })
                .Select(d => new { d.Key.CD_PRODUTO, d.Key.CD_EMPRESA, d.Key.NU_IDENTIFICADOR, Cantidad = d.Sum(e => e.QT_LIBERADO) })
                .ToList();

            foreach (var entity in insumosEntity)
            {
                DetallePedidoE detalle = new DetallePedidoE();
                detalle.Producto = entity.CD_PRODUTO;
                detalle.Empresa = entity.CD_EMPRESA;
                detalle.Identificador = entity.NU_IDENTIFICADOR;
                detalle.cantidad = (entity.Cantidad) ?? 0;
                detalles.Add(detalle);
            }

            return detalles;
        }

        public virtual List<InterfazEntradaProduccionProducido> GetProducidoInterfaceEntradaProduccionPorEjecucion(long nroEjecucion)
        {
            List<InterfazEntradaProduccionProducido> detalles = new List<InterfazEntradaProduccionProducido>();

            var producidoEntities = this._context.I_E_PRDC_SALIDA_PRD_PRODUCIDO
                .Where(d => d.NU_INTERFAZ_EJECUCION == nroEjecucion)
                .AsNoTracking().ToList();

            foreach (var entity in producidoEntities)
            {
                detalles.Add(this._mapper.MapToInterfazEntradaProduccionProducido(entity));
            }

            return detalles;
        }

        public virtual decimal GetQtProducido(string nroIngreso, string cdProducto, int cdEmpresa, decimal cdFaixa)
        {
            return this._context.T_PRDC_LINEA_PRODUCIDO
                .Where(d => d.NU_PRDC_INGRESO == nroIngreso
                    && d.CD_PRODUTO == cdProducto
                    && d.CD_EMPRESA == cdEmpresa
                    && d.CD_FAIXA == cdFaixa)
                .GroupBy(d => new { d.NU_PRDC_INGRESO, d.CD_PRODUTO, d.CD_EMPRESA, d.CD_FAIXA })
                .AsNoTracking()
                .Select(d => d.Sum(e => e.QT_PRODUCIDO ?? 0))
                .FirstOrDefault();
        }

        public virtual decimal GetQtLineaIngreso(string nuPrdcIngreso, string cdProducto, decimal cdFaixa)
        {
            return this._context.T_PRDC_LINEA
                    .Join(this._context.T_STOCK,
                            pl => pl.CD_ENDERECO_ENTRADA,
                            st => st.CD_ENDERECO,
                            (pl, st) => new { Linea = pl, Stock = st })
                    .Where(d => d.Linea.NU_PRDC_INGRESO == nuPrdcIngreso
                        && d.Stock.CD_PRODUTO == cdProducto
                        && d.Stock.CD_FAIXA == cdFaixa)
                    .GroupBy(d => new { d.Linea.NU_PRDC_INGRESO, d.Stock.CD_PRODUTO, d.Stock.CD_EMPRESA, d.Stock.CD_FAIXA })
                    .AsNoTracking()
                    .Select(d => d.Sum(e => e.Stock.QT_ESTOQUE ?? 0))
                    .FirstOrDefault();
        }

        public virtual decimal GetQtPreparadoIngreso(string nuPrdcIngreso, string cdProducto, decimal cdFaixa)
        {
            return this._context.T_DET_PICKING
                .Join(this._context.T_PEDIDO_SAIDA,
                        pk => new { pk.NU_PEDIDO, pk.CD_CLIENTE, pk.CD_EMPRESA },
                        ps => new { ps.NU_PEDIDO, ps.CD_CLIENTE, ps.CD_EMPRESA },
                        (pk, ps) => new { DetPicking = pk, Pedido = ps })
                .Join(this._context.T_CONTENEDOR,
                        pks => new { pks.DetPicking.NU_PREPARACION, pks.DetPicking.NU_CONTENEDOR },
                        co => new { co.NU_PREPARACION, NU_CONTENEDOR = (int?)co.NU_CONTENEDOR },
                        (pks, co) => new { DetPicking = pks.DetPicking, Pedido = pks.Pedido, Contenedor = co })
                .Where(d => d.Contenedor.CD_SITUACAO != SituacionDb.ContenedorTransferido
                    && d.Pedido.NU_PRDC_INGRESO == nuPrdcIngreso
                    && d.DetPicking.CD_PRODUTO == cdProducto
                    && d.DetPicking.CD_FAIXA == cdFaixa)
                .GroupBy(d => new { d.Pedido.NU_PRDC_INGRESO, d.DetPicking.CD_PRODUTO, d.DetPicking.CD_EMPRESA, d.DetPicking.CD_FAIXA })
                .AsNoTracking()
                .Select(d => d.Sum(e => e.DetPicking.QT_PREPARADO ?? 0))
                .FirstOrDefault();
        }

        public virtual void GetQtPedidoIngreso(string nuPrdcIngreso, string cdProducto, int cdEmpresa, decimal cdFaixa, out decimal qtPedido, out decimal qtLiberado)
        {
            qtPedido = 0;
            qtLiberado = 0;

            var datosPedido = this._context.T_PEDIDO_SAIDA
                .Join(this._context.T_DET_PEDIDO_SAIDA,
                        ps => new { ps.NU_PEDIDO, ps.CD_CLIENTE, ps.CD_EMPRESA },
                        dps => new { dps.NU_PEDIDO, dps.CD_CLIENTE, dps.CD_EMPRESA },
                        (ps, dps) => new { Pedido = ps, DetPedido = dps })
                .Where(d => d.Pedido.NU_PRDC_INGRESO == nuPrdcIngreso
                    && d.DetPedido.CD_PRODUTO == cdProducto
                    && d.DetPedido.CD_EMPRESA == cdEmpresa
                    && d.DetPedido.CD_FAIXA == cdFaixa)
                .GroupBy(d => new { d.Pedido.NU_PRDC_INGRESO, d.DetPedido.CD_EMPRESA, d.DetPedido.CD_PRODUTO, d.DetPedido.CD_FAIXA })
                .AsNoTracking()
                .Select(d => new
                {
                    QT_PEDIDO_TOTAL = d.Sum(e => e.DetPedido.QT_PEDIDO ?? 0),
                    QT_LIBERADO_TOTAL = d.Sum(e => e.DetPedido.QT_LIBERADO ?? 0)
                })
                .FirstOrDefault();

            if (datosPedido != null)
            {
                qtPedido = datosPedido.QT_PEDIDO_TOTAL;
                qtLiberado = datosPedido.QT_LIBERADO_TOTAL;
            }
        }

        public virtual DTOIngreso GetIngresoPRD150(string nroIngreso)
        {
            V_PRDC_INGRESO_KIT150 ingreso = this._context.V_PRDC_INGRESO_KIT150
                .FirstOrDefault(i => i.NU_PRDC_INGRESO == nroIngreso);

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

            string tipo = this._context.T_DET_DOMINIO.FirstOrDefault(d => d.NU_DOMINIO == ingreso.ND_TIPO).DS_DOMINIO_VALOR;

            return new DTOIngreso
            {
                NU_PRDC_INGRESO = ingreso.NU_PRDC_INGRESO,
                CD_PRDC_DEFINICION = ingreso.CD_PRDC_DEFINICION,
                NM_PRDC_DEFINICION = ingreso.NM_PRDC_DEFINICION,
                CD_SITUACAO = Convert.ToString(ingreso.CD_SITUACAO),
                DS_SITUACAO = ingreso.DS_SITUACAO,
                CD_EMPRESA = Convert.ToString(ingreso.CD_EMPRESA),
                NM_EMPRESA = ingreso.NM_EMPRESA,
                ID_GENERAR_PEDIDO = ingreso.ID_GENERAR_PEDIDO,
                DT_ADDROW = ingreso.DT_ADDROW.HasValue ? ingreso.DT_ADDROW.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                CD_FUNCIONARIO = Convert.ToString(ingreso.CD_FUNCIONARIO),
                NM_FUNCIONARIO = ingreso.NM_FUNCIONARIO,
                DS_ANEXO1 = ingreso.DS_ANEXO1,
                DS_ANEXO2 = ingreso.DS_ANEXO2,
                DS_ANEXO3 = ingreso.DS_ANEXO3,
                DS_ANEXO4 = ingreso.DS_ANEXO4,
                DS_TIPO_INGRESO = tipo,
                ND_TIPO = ingreso.ND_TIPO
            };
        }

        public virtual List<ProductoEnsamblado> GetInsumosPorContenedor(int preparacion, int contenedor)
        {
            return _context.V_PRDC_PROD_ENTRADA_PREP
                .AsNoTracking()
                .Where(w => w.NU_PREPARACION == preparacion
                    && w.NU_CONTENEDOR == contenedor)
                .ToList()
                .Select(w => new ProductoEnsamblado
                {
                    CD_PRDC_DEFINICION = w.CD_PRDC_DEFINICION,
                    CD_EMPRESA = w.CD_EMPRESA,
                    CD_PRODUTO = w.CD_PRODUTO,
                    CD_FAIXA = w.CD_FAIXA,
                    NU_IDENTIFICADOR = ManejoIdentificadorDb.IdentificadorProducto,
                    QT_PREPARADO = w.QT_PREPARADO ?? 0,
                    QT_COMPLETA = w.QT_COMPLETA ?? 0,
                    QT_SALIDA = w.QT_SALIDA ?? 0,
                    QT_SOBRANTE = w.QT_SOBRANTE ?? 0,
                }).ToList();
        }

        public virtual List<ProductoEnsamblado> GetProductosFinalesPorContenedor(int preparacion, int contenedor, string defFormula)
        {
            return _context.V_PRDC_PROD_SALIDA_PREP
                .AsNoTracking()
                .Where(w => w.NU_PREPARACION == preparacion
                    && w.NU_CONTENEDOR == contenedor
                    && (w.QT_SALIDA ?? 0) > 0)
                .ToList()
                .Select(w => new ProductoEnsamblado
                {
                    CD_PRDC_DEFINICION = w.CD_PRDC_DEFINICION,
                    CD_EMPRESA = w.CD_EMPRESA,
                    CD_PRODUTO = w.CD_PRODUTO,
                    CD_FAIXA = w.CD_FAIXA,
                    NU_IDENTIFICADOR = ManejoIdentificadorDb.IdentificadorProducto,
                    QT_PREPARADO = 0,
                    QT_COMPLETA = 0,
                    QT_SALIDA = w.QT_SALIDA ?? 0,
                    QT_SOBRANTE = 0,

                }).ToList();
        }

        public virtual string GetUbicacionProduccion(string idEspacioProducion)
        {
            string ubicacionProduccion = "";
            var linea = this._context.T_PRDC_LINEA.FirstOrDefault(x => x.CD_PRDC_LINEA == idEspacioProducion);

            if (linea != null)
                ubicacionProduccion = linea.CD_ENDERECO_PRODUCCION == null ? "" : linea.CD_ENDERECO_PRODUCCION;

            return ubicacionProduccion;
        }

        public virtual decimal GetCantidadReservaInsumosSumados(string idIngreso, string producto, string lote, int empresa, decimal faixa, string ubicacion)
        {
            decimal cantidadReservada = 0;
            var detallesRealInsumo = _context.T_PRDC_DET_INGRESO_REAL.Where(x => x.NU_PRDC_INGRESO == idIngreso
                   && x.CD_EMPRESA == empresa
                   && x.CD_PRODUTO == producto
                   && x.CD_FAIXA == faixa
                   && (x.NU_IDENTIFICADOR == lote || lote == ManejoIdentificadorDb.IdentificadorAuto)).ToList();

            if (detallesRealInsumo.Count > 0)
            {
                cantidadReservada = detallesRealInsumo.Sum(x => x.QT_REAL ?? 0);
            }

            return cantidadReservada;
        }

        public virtual string GetIdExternoByIdIngreso(string idIngreso)
        {
            T_PRDC_INGRESO ingreso = this._context.T_PRDC_INGRESO
               .AsNoTracking()
               .Where(d => d.NU_PRDC_INGRESO == idIngreso)
               .FirstOrDefault();
            if (ingreso != null)
                return ingreso.ID_PRODUCCION_EXTERNO;

            return string.Empty;
        }

        public virtual List<string> GetAllVentanasConfiguracionLiberacion(int empresa)
        {
            return _context.T_PRODUTO
                .Where(x => x.CD_EMPRESA == empresa && x.CD_VENTANA_LIBERACION != null)
                .Select(x => x.CD_VENTANA_LIBERACION).Distinct().ToList();
        }
        public virtual List<ClienteDiasValidezVentana> GetAllVentanasConfiguracionLiberacionByEmpresa(int empresa)
        {
            return _context.T_PRODUTO
                .Where(x => x.CD_EMPRESA == empresa && x.CD_VENTANA_LIBERACION != null)
                .Select(x => x.CD_VENTANA_LIBERACION).Distinct().Select( x => new ClienteDiasValidezVentana() { VentanaLiberacion =  x , Empresa = empresa }).ToList();
        }
        #endregion

        #region Add

        public virtual void AddPasada(IIngresoPanel ingreso, Pasada pasada)
        {
            T_PRDC_INGRESO_PASADA entity = this._mapper.MapPasadaObjectToEntity(pasada);

            entity.NU_PRDC_INGRESO = ingreso.Id;
            entity.CD_PRDC_LINEA = ingreso.Linea.Id;

            this._context.T_PRDC_INGRESO_PASADA.Add(entity);
        }

        public virtual void AddIngreso(IIngreso ingreso)
        {
            T_PRDC_INGRESO entity = this._mapper.MapObjectToIngreso(ingreso);

            this._context.T_PRDC_INGRESO.Add(entity);
        }

        public virtual void AddIngresoPanel(IIngresoPanel ingreso)
        {
            T_PRDC_INGRESO entity = this._mapper.MapObjectToIngreso(ingreso);

            this._context.T_PRDC_INGRESO.Add(entity);
        }

        public virtual void AddConsumido(IIngreso ingreso, LineaConsumida linea)
        {
            AddConsumido(ingreso, linea, linea.NumeroTransaccion ?? 0);
        }

        public virtual void AddConsumido(IIngreso ingreso, LineaConsumida linea, long transaccion)
        {
            T_PRDC_LINEA_CONSUMIDO entity = this._mapper.MapObjectToLineaConsumidaEntity(ingreso, linea);

            entity.NU_TRANSACCION = transaccion;
            entity.DT_ADDROW = linea.FechaAlta;

            this._context.T_PRDC_LINEA_CONSUMIDO.Add(entity);
        }

        public virtual void AddProducido(IIngreso ingreso, LineaProducida linea)
        {
            AddProducido(ingreso, linea, linea.NumeroTransaccion ?? 0);
        }

        public virtual void AddProducido(IIngreso ingreso, LineaProducida linea, long transaccion)
        {
            string semi = "N";

            if (!string.IsNullOrEmpty(linea.Semiacabado))
            {
                semi = linea.Semiacabado;
            }

            var entity = this._context.T_PRDC_LINEA_PRODUCIDO
                .FirstOrDefault(x => x.CD_PRODUTO == linea.Producto
                    && x.CD_EMPRESA == linea.Empresa
                    && x.CD_FAIXA == linea.Faixa
                    && x.NU_IDENTIFICADOR == linea.Identificador
                    && x.NU_PRDC_INGRESO == ingreso.Id
                    && x.CD_PRDC_DEFINICION == ingreso.Formula.Id
                    && x.NU_FORMULA == linea.Iteracion
                    && x.NU_PASADA == linea.Pasada
                    && x.FL_SEMIACABADO == semi);

            if (entity == null)
            {
                entity = this._mapper.MapObjectToLineaProducidaEntity(ingreso, linea);
                entity.NU_TRANSACCION = transaccion;
                entity.DT_ADDROW = linea.FechaAlta;
                this._context.T_PRDC_LINEA_PRODUCIDO.Add(entity);
            }
            else
            {
                entity.QT_PRODUCIDO = entity.QT_PRODUCIDO + linea.Cantidad;
            }
        }

        public virtual void AddConsumoHistorico(IIngreso ingreso, LineaConsumidaHistorica lineaHistorica, long transaccion)
        {
            T_HIST_PRDC_LINEA_CONSUMIDO entity = this._mapper.MapObjectToLineaConsumidaHistoricoEntity(ingreso, lineaHistorica);

            entity.NU_TRANSACCION = transaccion;
            entity.DT_ADDROW = lineaHistorica.FechaAlta;
            entity.NU_HISTORICO = lineaHistorica.NumeroHistorico;

            this._context.T_HIST_PRDC_LINEA_CONSUMIDO.Add(entity);
        }

        public virtual void AddProducidoHistorico(IIngreso ingreso, LineaProducidaHistorica lineaHistorica, long transaccion)
        {
            T_HIST_PRDC_LINEA_PRODUCIDO entity = this._mapper.MapObjectToLineaProducidaHistoricoEntity(ingreso, lineaHistorica);

            entity.NU_TRANSACCION = transaccion;
            entity.DT_ADDROW = lineaHistorica.FechaAlta;
            entity.NU_HISTORICO = lineaHistorica.NumeroHistorico;

            this._context.T_HIST_PRDC_LINEA_PRODUCIDO.Add(entity);
        }

        public virtual void AddPasadaHistorico(PasadaHistorica pasadaHistorica, long transaccion)
        {
            T_HIST_PRDC_INGRESO_PASADA entity = this._mapper.MapObjectToPasdaHistoricoEntity(pasadaHistorica);

            entity.NU_TRANSACCION = transaccion;

            this._context.T_HIST_PRDC_INGRESO_PASADA.Add(entity);
        }

        public virtual void AddDocumentosIngreso(IIngreso ingreso, Produccion.Documento documento)
        {
            T_PRDC_INGRESO_DOCUMENTO entity = this._mapper.MapObjectDocumentoEntity(ingreso, documento);

            this._context.T_PRDC_INGRESO_DOCUMENTO.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateIngresoWhiteBox(IngresoWhiteBox ingreso)
        {
            var entity = this._mapper.MapObjectToIngresoPanel(ingreso);
            var attachedEntity = _context.T_PRDC_INGRESO.Local
                .FirstOrDefault(w => w.NU_PRDC_INGRESO == entity.NU_PRDC_INGRESO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRDC_INGRESO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateIngresoBlackBox(IngresoBlackBox ingreso)
        {
            var entity = this._mapper.MapObjectToIngreso(ingreso);
            var attachedEntity = _context.T_PRDC_INGRESO.Local
                .FirstOrDefault(w => w.NU_PRDC_INGRESO == entity.NU_PRDC_INGRESO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRDC_INGRESO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateConsumido(IIngreso ingreso, LineaConsumida linea, long transaccion)
        {
            var entity = this._mapper.MapObjectToLineaConsumidaEntity(ingreso, linea);
            var attachedEntity = _context.T_PRDC_LINEA_CONSUMIDO.Local
                .FirstOrDefault(w => w.CD_PRDC_DEFINICION == entity.CD_PRDC_DEFINICION
                    && w.NU_PRDC_INGRESO == entity.NU_PRDC_INGRESO
                    && w.NU_FORMULA == entity.NU_FORMULA
                    && w.NU_PASADA == entity.NU_PASADA
                    && w.CD_PRODUTO == entity.CD_PRODUTO
                    && w.CD_FAIXA == entity.CD_FAIXA
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR);

            entity.NU_TRANSACCION = transaccion;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRDC_LINEA_CONSUMIDO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateProducido(IIngreso ingreso, LineaProducida linea, long transaccion)
        {
            var entity = this._mapper.MapObjectToLineaProducidaEntity(ingreso, linea);
            var attachedEntity = _context.T_PRDC_LINEA_PRODUCIDO.Local
                .FirstOrDefault(w => w.CD_PRDC_DEFINICION == entity.CD_PRDC_DEFINICION
                    && w.NU_PRDC_INGRESO == entity.NU_PRDC_INGRESO
                    && w.NU_FORMULA == entity.NU_FORMULA
                    && w.NU_PASADA == entity.NU_PASADA
                    && w.CD_PRODUTO == entity.CD_PRODUTO
                    && w.CD_FAIXA == entity.CD_FAIXA
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR);

            entity.NU_TRANSACCION = transaccion;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRDC_LINEA_PRODUCIDO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Delete

        public virtual void RemoveConsumo(IIngreso ingreso, LineaConsumida linea)
        {
            var entity = this._context.T_PRDC_LINEA_CONSUMIDO
                .FirstOrDefault(x => x.CD_PRODUTO == linea.Producto
                    && x.CD_EMPRESA == linea.Empresa
                    && x.CD_FAIXA == linea.Faixa
                    && x.NU_IDENTIFICADOR == linea.Identificador
                    && x.NU_PRDC_INGRESO == ingreso.Id
                    && x.CD_PRDC_DEFINICION == ingreso.Formula.Id
                    && x.NU_FORMULA == linea.Iteracion
                    && x.NU_PASADA == linea.Pasada);
            var attachedEntity = this._context.T_PRDC_LINEA_CONSUMIDO.Local
                .FirstOrDefault(x => x.CD_PRODUTO == linea.Producto
                    && x.CD_EMPRESA == linea.Empresa
                    && x.CD_FAIXA == linea.Faixa
                    && x.NU_IDENTIFICADOR == linea.Identificador
                    && x.NU_PRDC_INGRESO == ingreso.Id
                    && x.CD_PRDC_DEFINICION == ingreso.Formula.Id
                    && x.NU_FORMULA == linea.Iteracion
                    && x.NU_PASADA == linea.Pasada);

            if (attachedEntity != null)
            {
                this._context.T_PRDC_LINEA_CONSUMIDO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_PRDC_LINEA_CONSUMIDO.Remove(entity);
            }
        }

        public virtual void RemoveProducido(IIngreso ingreso, LineaProducida linea)
        {
            var entity = this._context.T_PRDC_LINEA_PRODUCIDO
                .FirstOrDefault(x => x.CD_PRODUTO == linea.Producto
                    && x.CD_EMPRESA == linea.Empresa
                    && x.CD_FAIXA == linea.Faixa
                    && x.NU_IDENTIFICADOR == linea.Identificador
                    && x.NU_PRDC_INGRESO == ingreso.Id
                    && x.CD_PRDC_DEFINICION == ingreso.Formula.Id
                    && x.NU_FORMULA == linea.Iteracion
                    && x.NU_PASADA == linea.Pasada);
            var attachedEntity = this._context.T_PRDC_LINEA_PRODUCIDO.Local
                .FirstOrDefault(x => x.CD_PRODUTO == linea.Producto
                    && x.CD_EMPRESA == linea.Empresa
                    && x.CD_FAIXA == linea.Faixa
                    && x.NU_IDENTIFICADOR == linea.Identificador
                    && x.NU_PRDC_INGRESO == ingreso.Id
                    && x.CD_PRDC_DEFINICION == ingreso.Formula.Id
                    && x.NU_FORMULA == linea.Iteracion
                    && x.NU_PASADA == linea.Pasada);

            if (attachedEntity != null)
            {
                this._context.T_PRDC_LINEA_PRODUCIDO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_PRDC_LINEA_PRODUCIDO.Remove(entity);
            }
        }

        public virtual void RemovePasada(IIngreso ingreso, Pasada pasada)
        {
            var entity = this._mapper.MapPasadaObjectToEntity(pasada);
            var attachedEntity = _context.T_PRDC_INGRESO_PASADA.Local
                .FirstOrDefault(w => w.NU_PRDC_INGRESO == entity.NU_PRDC_INGRESO
                    && w.QT_PASADAS == entity.QT_PASADAS
                    && w.NU_ORDEN == entity.NU_ORDEN);

            if (attachedEntity != null)
            {
                this._context.T_PRDC_INGRESO_PASADA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_PRDC_INGRESO_PASADA.Attach(entity);
                this._context.T_PRDC_INGRESO_PASADA.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #region Expulsar Produccion

        public virtual void ExpulsarProductos(IUnitOfWork uow, string predio, List<ProductosExpulsable> stocksExpulsar, bool isExplusarConTransferencia, long nuTransaccion, decimal? nuEtiquetaExterna, int userId, string metadata = "")
        {
            Impresion impresion = new Impresion();
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            AddStockTemporal(connection, tran, stocksExpulsar, nuTransaccion);

            List<Stock> stockExpulsarTotal = GetStockExpulsarTotal(connection, tran);

            UpdateStockTemporal(connection, tran, stockExpulsarTotal, nuTransaccion);

            Stock stock = PuedeExpulsarProductos(connection, tran);

            if (stock == null)
            {
                if (isExplusarConTransferencia)
                {
                    var palletTransferencia = uow.StockRepository.GetNewPalletEntity(stocksExpulsar.FirstOrDefault().UbicacionDestino, predio, nuEtiquetaExterna ?? 0, nuTransaccion);
                    uow.StockRepository.InsertPalletTransferencia(connection, tran, palletTransferencia);
                    uow.StockRepository.BulkInsertDetallePalletTransferencia(connection, tran, nuTransaccion, userId, palletTransferencia, metadata);
                    uow.StockRepository.UpdateBajaStock(connection, tran, nuTransaccion);
                    uow.StockRepository.UpdateAltaStockEquipamientos(connection, tran, nuTransaccion);
                    uow.StockRepository.InsertStockEquipamientos(connection, tran, nuTransaccion);
                }
                else
                {
                    uow.StockRepository.UpdateBajaStock(connection, tran, nuTransaccion);
                    uow.StockRepository.UpdateAltaStock(connection, tran, nuTransaccion);
                    uow.StockRepository.InsertStock(connection, tran, nuTransaccion);
                }
            }
            else
                throw new ValidationFailedException("PRD113_Sec0_Error_01", new string[] { stock.Producto, stock.Identificador });

            DeleteStockTemporal(connection, tran, stocksExpulsar, nuTransaccion);

        }

        public virtual List<Stock> GetStockExpulsarTotal(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            st.CD_ENDERECO as Ubicacion,
                            st.CD_EMPRESA as Empresa,
                            st.CD_PRODUTO as Producto,
                            st.CD_FAIXA as Faixa,
                            st.NU_IDENTIFICADOR as Identificador,
                            s.QT_ESTOQUE - s.QT_RESERVA_SAIDA  as Cantidad
                        FROM
                            T_STOCK_TEMP st
                        LEFT JOIN T_STOCK s on s.CD_ENDERECO = st.CD_ENDERECO AND
                             s.CD_EMPRESA = st.CD_EMPRESA AND
                             s.CD_PRODUTO = st.CD_PRODUTO AND
                             s.CD_FAIXA = st.CD_FAIXA AND
                             s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR
                        WHERE st.QT_ESTOQUE is null
                        ";

            return _dapper.Query<Stock>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual Stock PuedeExpulsarProductos(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            st.CD_ENDERECO as Ubicacion,
                            st.CD_EMPRESA as Empresa,
                            st.CD_PRODUTO as Producto,
                            st.CD_FAIXA as Faixa,
                            st.NU_IDENTIFICADOR as Identificador
                        FROM
                            T_STOCK_TEMP st
                        LEFT JOIN T_STOCK s on s.CD_ENDERECO = st.CD_ENDERECO AND
                             s.CD_EMPRESA = st.CD_EMPRESA AND
                             s.CD_PRODUTO = st.CD_PRODUTO AND
                             s.CD_FAIXA = st.CD_FAIXA AND
                             s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR
                        WHERE st.QT_ESTOQUE > (s.QT_ESTOQUE - s.QT_RESERVA_SAIDA)
                        ";

            return _dapper.Query<Stock>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual void AddStockTemporal(DbConnection connection, DbTransaction tran, List<ProductosExpulsable> stocksExpulsar, long nuTransaccion)
        {
            _dapper.BulkInsert(connection, tran, stocksExpulsar, "T_STOCK_TEMP", new Dictionary<string, Func<ProductosExpulsable, ColumnInfo>>
            {
                { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Identificador)},
                { "DT_FABRICACAO", x => new ColumnInfo(x.Vencimiento,DbType.DateTime)},
                { "QT_ESTOQUE", x => new ColumnInfo(x.Cantidad,DbType.Decimal)},
                { "CD_ENDERECO_DEST", x => new ColumnInfo(x.UbicacionDestino)},
                { "IDROW", x => new ColumnInfo(x.IdRow)}
            });
        }

        public virtual void UpdateStockTemporal(DbConnection connection, DbTransaction tran, List<Stock> stocksExpulsar, long nuTransaccion)
        {
            _dapper.BulkUpdate(connection, tran, stocksExpulsar, "T_STOCK_TEMP", new Dictionary<string, Func<Stock, ColumnInfo>>
            {
                { "QT_ESTOQUE", x => new ColumnInfo(x.Cantidad, DbType.Decimal)}
            }, new Dictionary<string, Func<Stock, ColumnInfo>>
            {
                { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x =>new ColumnInfo(x.Identificador)}
            });
        }

        public virtual void DeleteStockTemporal(DbConnection connection, DbTransaction tran, List<ProductosExpulsable> stocksExpulsar, long nuTransaccion)
        {
            _dapper.BulkDelete(connection, tran, stocksExpulsar, "T_STOCK_TEMP", new Dictionary<string, Func<ProductosExpulsable, object>>
            {
                { "CD_ENDERECO", x => x.Ubicacion},
                { "CD_EMPRESA", x => x.Empresa},
                { "CD_PRODUTO", x => x.Producto},
                { "CD_FAIXA", x => x.Faixa},
                { "NU_IDENTIFICADOR", x => x.Identificador}
            });
        }

        #endregion

        #endregion
    }
}
