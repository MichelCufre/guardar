using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento;
using WIS.Domain.Expedicion;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Persistence.Database;
using WIS.Persistence.General;
using Preparacion = WIS.Domain.Picking.Preparacion;

namespace WIS.Domain.DataModel.Repositories
{
    public class PreparacionRepository
    {
        protected int _userId;
        protected WISDB _context;
        protected string _application;
        protected readonly IDapper _dapper;
        protected PreparacionMapper _mapper;
        protected ContenedorMapper _mapperContenedor;
        protected CargaMapper _mapperCarga;
        protected ParametroRepository _paramRepo;

        public PreparacionRepository(WISDB _context, string application, int userId, IDapper dapper)
        {
            this._userId = userId;
            this._dapper = dapper;
            this._context = _context;
            this._application = application;
            this._mapperCarga = new CargaMapper();
            this._mapper = new PreparacionMapper();
            this._mapperContenedor = new ContenedorMapper();
            this._paramRepo = new ParametroRepository(_context, application, userId, dapper);
        }

        #region Any

        public virtual bool ExistenContenedoresCompartidos(int NuPreparacion, int NuContenedor)
        {
            List<short> situacionesCamion = new List<short>
            {
                SituacionDb.CamionAguardandoCarga,
                SituacionDb.CamionCargando
            };

            var query3 = _context.V_QT_CAMIONES_CONTENEDOR.Where(qcc => qcc.NU_PREPARACION == NuPreparacion && qcc.NU_CONTENEDOR == NuContenedor
                && (qcc.CD_SITUACAO == null || situacionesCamion.Contains(qcc.CD_SITUACAO ?? SituacionDb.CamionAguardandoCarga)))
                .GroupBy(qcc => qcc.CD_CAMION);

            var qtCamiones = query3.Count();

            return qtCamiones > 1;
        }

        public virtual bool AnyContenedorNoFacturado(Camion camion)
        {
            return _context.V_CONT_CAMION_FACT
                .Any(x => x.CD_CAMION == camion.Id
                    && x.CD_CAMION_CONT == null
                    && x.CD_CAMION_FACT_CONT == null);
        }

        public virtual bool AnyContenedorSinFinalizarControl(Camion camion, out int cantCont)
        {
            var enProceso = new List<string>() { "P", "R" };

            cantCont = _context.V_CAMION_CTRL_CONTENEDORES
                .AsNoTracking()
                .Where(c => c.CD_CAMION == camion.Id
                    && enProceso.Contains(c.VL_CONTROL))?
                .Count() ?? 0;

            return cantCont > 0;
        }

        public virtual bool AnyContenedorSinControl(Camion camion, out int cantCont)
        {
            var finalizados = new List<string>() { "C", "D" };

            cantCont = _context.V_CAMION_CTRL_CONTENEDORES
                .AsNoTracking()
                .Where(c => c.CD_CAMION == camion.Id
                    && !finalizados.Contains(c.VL_CONTROL))?
                .Count() ?? 0;

            return cantCont > 0;

        }

        public virtual bool AnyProductoSinPreparar(int CdCamion)
        {
            return _context.V_PRODUCTOS_SIN_PREP_WEXP041.Any(s => s.CD_CAMION == CdCamion);
        }

        public virtual bool AnyContenedorSinEmbarcar(int CdCamion)
        {
            return _context.V_CONT_SIN_EMBARCAR_WEXP041.Any(s => s.CD_CAMION == CdCamion);
        }

        public virtual bool AnyContenedorEmbarcado(int CdCamion)
        {
            return _context.V_CONT_EMBARCADOS_WEXP041.Any(s => s.CD_CAMION == CdCamion);
        }

        public virtual bool ExistenAnulacionesPendientes(int prep, int nuCont, string pedido, int emp, string cliente, out string producto)
        {
            producto = string.Empty;
            List<T_DET_PICKING> lpick = _context.T_DET_PICKING
                .Where(p => p.NU_PREPARACION == prep
                    && p.NU_PEDIDO == pedido
                    && p.NU_CONTENEDOR == nuCont
                    && p.CD_EMPRESA == emp
                    && p.CD_CLIENTE == cliente)
                .ToList();

            foreach (T_DET_PICKING detPick in lpick)
            {
                producto = detPick.CD_PRODUTO;
                V_ANULACIONES_PENDIENTES anuPend = _context.V_ANULACIONES_PENDIENTES
                    .FirstOrDefault(a => a.NU_PEDIDO == detPick.NU_PEDIDO
                        && a.CD_CLIENTE == detPick.CD_CLIENTE
                        && a.CD_EMPRESA == detPick.CD_EMPRESA
                        && a.CD_PRODUTO == detPick.CD_PRODUTO
                        && a.NU_IDENTIFICADOR == detPick.NU_IDENTIFICADOR
                        && a.QT_PENDIENTE > 0);

                V_ANULACIONES_PENDIENTES anuPendAUTO = _context.V_ANULACIONES_PENDIENTES
                    .FirstOrDefault(a => a.NU_PEDIDO == detPick.NU_PEDIDO
                        && a.CD_CLIENTE == detPick.CD_CLIENTE
                        && a.CD_EMPRESA == detPick.CD_EMPRESA
                        && a.CD_PRODUTO == detPick.CD_PRODUTO
                        && a.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto
                        && a.QT_PENDIENTE > 0);

                if (anuPend != null)
                    return true;

                if (anuPendAUTO != null)
                    return true;
            }
            return false;
        }

        public virtual bool AnyAnulacionPendiente(int camion)
        {
            return this._context.V_ANULACIONES_PENDIENT_CAMION.Any(d => d.CD_CAMION == camion);
        }

        public virtual bool AnyContenedorNoAsignadoPedido(int empresa, string cliente, string pedido, int camion)
        {
            List<short> situacionesAdmitidas = new List<short>
            {
                SituacionDb.ContenedorEnPreparacion,
                SituacionDb.ContenedorEnCamion
            };

            var listaCamiones = this._context.V_PEDIDO_ASIG_CAMION
                .Where(d => d.CD_EMPRESA == empresa
                    && d.CD_CLIENTE == cliente
                    && d.NU_PEDIDO == pedido
                    && situacionesAdmitidas.Contains(d.CD_SITUACAO ?? SituacionDb.ContenedorEnPreparacion))
                .Select(dpc => dpc.CD_CAMION)
                .Distinct()
                .ToList();

            return listaCamiones.Count > 1 || listaCamiones.First() != camion;
        }

        public virtual bool AnyDetalleParaCarga(long carga)
        {
            return this._context.T_DET_PICKING.AsNoTracking().Any(d => d.NU_CARGA == carga);
        }

        public virtual bool PuedeAgregarCargaPedidoCamion(long carga)
        {
            var detalle = this._context.T_DET_PICKING
                .AsNoTracking()
                .Where(d => d.NU_CARGA == carga && d.NU_CONTENEDOR != null)
                .FirstOrDefault();

            if (detalle == null)
                return true;

            var contenedor = this._context.T_CONTENEDOR
                .Where(d => d.NU_CONTENEDOR == detalle.NU_CONTENEDOR
                    && d.NU_PREPARACION == detalle.NU_PREPARACION)
                .FirstOrDefault();

            return contenedor != null && contenedor.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion;
        }

        public virtual bool ExistenMultiplesPedidosCarga(long carga)
        {
            return this._context.V_CARGAS_CON_MULTIPLE_PEDIDO.AsNoTracking().Any(d => d.NU_CARGA == carga);
        }

        public virtual bool AnyContenedorSinPrecinto(int camion, int empresa, string cliente, string pedido)
        {
            return this._context.V_CONTENEDOR_PRECINTO.Any(e => e.CD_CAMION == camion
                    && e.CD_CLIENTE == cliente
                    && e.CD_EMPRESA == empresa
                    && e.NU_PEDIDO == pedido
                    && string.IsNullOrEmpty(e.ID_PRECINTO_1)
                    && string.IsNullOrEmpty(e.ID_PRECINTO_2));
        }

        public virtual bool AnyContenedorSinPrecintoParcial(int camion, int empresa, string cliente, string pedido)
        {
            return this._context.V_CONTENEDOR_PRECINTO.Any(e => e.CD_CAMION == camion
                    && e.CD_CLIENTE == cliente
                    && e.CD_EMPRESA == empresa
                    && e.NU_PEDIDO == pedido
                    && (string.IsNullOrEmpty(e.ID_PRECINTO_1)
                    || string.IsNullOrEmpty(e.ID_PRECINTO_2)));
        }

        public virtual bool AnyContenedorNoEmpaquetado(List<CargaCamion> cargaCamion, int empresa, string cliente, string pedido)
        {
            var pickingStates = new List<string> { EstadoDetallePreparacion.ESTADO_PREPARADO };
            var cargas = cargaCamion.Select(x => x.Carga).Distinct().ToList();

            return this._context.T_DET_PICKING
                .Include("T_CONTENEDOR")
                .AsNoTracking()
                .Any(dpc => dpc.CD_CLIENTE == cliente
                    && dpc.NU_PEDIDO == pedido
                    && dpc.CD_EMPRESA == empresa
                    && pickingStates.Contains(dpc.ND_ESTADO)
                    && cargas.Contains(dpc.NU_CARGA ?? 0)
                    && (dpc.T_CONTENEDOR.ID_CONTENEDOR_EMPAQUE == "N" || dpc.T_CONTENEDOR.ID_CONTENEDOR_EMPAQUE == null));
        }

        public virtual bool AnyLineaSinPickear(int empresa, string cliente, string pedido)
        {
            var estados = EstadoDetallePreparacion.GetEstadosPickingPendiente();

            var res = this._context.T_DET_PICKING
                .AsNoTracking()
                .Any(dp => dp.CD_CLIENTE == cliente
                    && dp.NU_PEDIDO == pedido
                    && dp.CD_EMPRESA == empresa
                    && estados.Contains(dp.ND_ESTADO));

            if (!res)
            {
                var qtLiberada = _context.T_DET_PEDIDO_SAIDA
                    .AsNoTracking()
                    .Where(dp => dp.CD_CLIENTE == cliente
                        && dp.NU_PEDIDO == pedido
                        && dp.CD_EMPRESA == empresa)
                    .Sum(s => s.QT_LIBERADO ?? 0);

                var qtPreparado = _context.T_DET_PICKING
                    .AsNoTracking()
                    .Where(dp => dp.CD_CLIENTE == cliente
                        && dp.NU_PEDIDO == pedido
                        && dp.CD_EMPRESA == empresa)
                    .Sum(s => s.QT_PREPARADO ?? 0);

                res = qtLiberada > qtPreparado;
            }
            return res;
        }

        public virtual bool AnyDetPicking(int preparacion, int empresa)
        {
            return this._context.T_DET_PICKING
                .AsNoTracking()
                .Any(dp => dp.NU_PREPARACION == preparacion && dp.CD_EMPRESA == empresa);
        }

        public virtual bool AnyPreparacion(int numero)
        {
            return _context.T_PICKING
                .AsNoTracking()
                .Any(a => a.NU_PREPARACION == numero);
        }

        public virtual bool PuedoDesasociarPedido(int prep, string pedido, string cliente, int empresa)
        {
            return !this._context.T_DET_PICKING
                .AsNoTracking()
                .Any(d => d.NU_PREPARACION == prep
                    && d.NU_PEDIDO == pedido
                    && d.CD_EMPRESA == empresa
                    && d.CD_CLIENTE == cliente);
        }

        public virtual bool IsFacturacionRequeridaContenedor(Contenedor contenedor)
        {
            string value = this._context.T_DET_PICKING
                .Where(d => d.NU_PREPARACION == contenedor.NumeroPreparacion && d.NU_CONTENEDOR == contenedor.Numero)
                .Join(
                    this._context.T_PEDIDO_SAIDA,
                    dp => new { dp.CD_EMPRESA, dp.CD_CLIENTE, dp.NU_PEDIDO },
                    ps => new { ps.CD_EMPRESA, ps.CD_CLIENTE, ps.NU_PEDIDO },
                    (dp, ps) => new { Picking = dp, Pedido = ps }
                )
                .Join(
                    this._context.T_TIPO_EXPEDICION,
                    dps => dps.Pedido.TP_EXPEDICION,
                    te => te.TP_EXPEDICION,
                    (dps, te) => new { dps.Picking, dps.Pedido, TipoExpedicion = te }
                )
                .AsNoTracking()
                .Select(d => d.TipoExpedicion.FL_FACTURACION_REQUERIDA)
                .FirstOrDefault();

            return this._mapper.MapStringToBoolean(value);
        }

        public virtual bool ContenedorConPedidoEnsamblado(int preparacion, int contenedor)
        {
            return (from dp in _context.T_DET_PICKING.AsNoTracking()
                    join ps in _context.T_PEDIDO_SAIDA.AsNoTracking() on new { dp.CD_EMPRESA, dp.NU_PEDIDO, dp.CD_CLIENTE } equals new { ps.CD_EMPRESA, ps.NU_PEDIDO, ps.CD_CLIENTE }
                    where dp.NU_CONTENEDOR == contenedor && dp.NU_PREPARACION == preparacion
                    select "X").Any();
        }

        public virtual bool AnyPendientesEnsamblarProduccion(string ingresoProduccion)
        {
            var estados = EstadoDetallePreparacion.GetEstadosPickingPendiente();

            return (from dp in _context.T_DET_PICKING.AsNoTracking()
                    join ps in _context.T_PEDIDO_SAIDA.AsNoTracking() on new { dp.CD_EMPRESA, dp.NU_PEDIDO, dp.CD_CLIENTE } equals new { ps.CD_EMPRESA, ps.NU_PEDIDO, ps.CD_CLIENTE }
                    where estados.Contains(dp.ND_ESTADO)
                    && ps.NU_PRDC_INGRESO == ingresoProduccion
                    select "X").Any();
        }

        public virtual bool AnyPendientesContenedorEnsamblarProduccion(string ingresoProduccion)
        {
            var detallesPicking = _context.T_DET_PICKING.AsNoTracking()
               .Join(_context.T_PEDIDO_SAIDA,
               dp => new { dp.CD_EMPRESA, dp.NU_PEDIDO, dp.CD_CLIENTE },
               pd => new { pd.CD_EMPRESA, pd.NU_PEDIDO, pd.CD_CLIENTE },
               (dp, pd) => new { Pedido = pd, Preparacion = dp }
               ).Where(x => x.Preparacion.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO
                   && x.Pedido.NU_PRDC_INGRESO == ingresoProduccion).Select(x => x.Preparacion);

            return detallesPicking.AsNoTracking()
                .Join(_context.T_CONTENEDOR,
                  dp => new { dp.NU_PREPARACION, dp.NU_CONTENEDOR },
                  cc => new { cc.NU_PREPARACION, NU_CONTENEDOR = (int?)cc.NU_CONTENEDOR },
                  (dp, cc) => cc).Where(x => x.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion).Any()
                  ;

        }

        public virtual bool AnyPedidoProduccionFinalizado(string ingresoProduccion)
        {
            return (from ps in _context.T_PEDIDO_SAIDA.AsNoTracking()
                    join dps in _context.T_DET_PEDIDO_SAIDA.AsNoTracking() on new { ps.CD_EMPRESA, ps.NU_PEDIDO, ps.CD_CLIENTE } equals new { dps.CD_EMPRESA, dps.NU_PEDIDO, dps.CD_CLIENTE }
                    where ps.NU_PRDC_INGRESO == ingresoProduccion
                    && dps.QT_PEDIDO - dps.QT_LIBERADO - dps.QT_ANULADO > 0
                    select "X")
                   .Any();
        }

        public virtual bool IsPreparacionAsociable(string tpOperativa, int nuPreparacion)
        {
            return this._context.V_PREP_DISP_ASOCIAR
                .Any(p => p.TP_OPERATIVA == tpOperativa
                    && p.NU_PREPARACION == nuPreparacion);
        }

        public virtual bool AnyDetallePicking(int preparacion, int contenedor)
        {
            return _context.T_DET_PICKING.AsNoTracking().Any(d => d.NU_PREPARACION == preparacion && d.NU_CONTENEDOR == contenedor);
        }

        public virtual bool EsPickingManualNoFinalizado(int nuPreparacion)
        {
            return _context.T_PICKING
                .AsNoTracking()
                .Any(p => p.NU_PREPARACION == nuPreparacion &&
                    p.CD_SITUACAO == SituacionDb.PreparacionIniciada && new string[] { "L", "P" }.Contains(p.TP_PREPARACION));
        }

        public virtual bool PreparacionPertenecePredio(int nuPreparacion, string predio)
        {
            return _context.T_PICKING
                .AsNoTracking()
                .Any(p => p.NU_PREPARACION == nuPreparacion && p.NU_PREDIO == predio);
        }

        public virtual bool AnyPreparacionPendienteNotificarAutomatismo()
        {
            return _context.T_PICKING.AsNoTracking().Any(p => p.ID_AVISO == "A");
        }
        #endregion

        #region Get

        public virtual List<string> GetNumerosPedidosDeUnContenedor(int numeroContenedor, int numeroPreparacion)
        {
            return this._context.T_DET_PICKING
                .AsNoTracking()
                .Where(w => w.NU_PREPARACION == numeroPreparacion
                    && w.NU_CONTENEDOR == numeroContenedor)
                .Select(w => w.NU_PEDIDO)
                .ToList();
        }

        public virtual List<long> GetCargasPedidoCamion(int camion, string cliente, string pedido, int empresa)
        {
            return this._context.V_CAMION_CARGA_PEDIDO.Where(w => w.CD_CAMION == camion && w.CD_CLIENTE == cliente && w.NU_PEDIDO == pedido && w.CD_EMPRESA == empresa).Select(s => s.NU_CARGA).ToList();
        }

        public virtual List<DetallePreparacion> GetDetalleLiberadosPreparacionByCarga(List<long> cargas)
        {
            var estadosAnulacion = EstadoDetallePreparacion.GetEstadosAnulacion();
            return this.GetDetallesPreparacion(d => cargas.Contains(d.NU_CARGA ?? -1) && !estadosAnulacion.Contains(d.ND_ESTADO));
        }

        public virtual long GetNextNumeroCarga()
        {
            return this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_CARGA);
        }

        public virtual int GetNextNumeroPreparacion()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_PREPARACION);
        }

        public virtual List<Contenedor> GetContenedores(Camion camion)
        {
            var contenedores = new List<Contenedor>();

            foreach (var carga in camion.Cargas)
            {
                var contenedoresAgregar = new List<Contenedor>();

                var entities = this._context.T_DET_PICKING
                    .Include("T_CONTENEDOR")
                    .Where(dp => dp.CD_CLIENTE == carga.Cliente
                        && dp.CD_EMPRESA == carga.Empresa
                        && dp.NU_CARGA == carga.Carga
                        && dp.T_CONTENEDOR.CD_CAMION_FACTURADO == null
                        && dp.T_CONTENEDOR != null
                        && dp.QT_PREPARADO != null //Se excluyen contenedores que ya esten facturados y no preparados
                        && dp.T_CONTENEDOR.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion) //TODO: Implementar situacion para interfaces
                    .Select(dp => dp.T_CONTENEDOR)
                    .Distinct()
                    .ToList();

                foreach (var entity in entities)
                {
                    contenedoresAgregar.Add(this._mapperContenedor.MapToObject(entity));
                }

                contenedores.AddRange(contenedoresAgregar);
            }

            return contenedores;
        }

        public virtual List<DetallePreparacion> GetDetallesPreparacion(Expression<Func<T_DET_PICKING, bool>> condition)
        {
            var detalles = new List<DetallePreparacion>();

            var contenedores = new List<Contenedor>();

            List<T_DET_PICKING> entities = this._context.T_DET_PICKING
                .Include("T_CONTENEDOR")
                .AsNoTracking()
                .Where(condition)
                .ToList();

            foreach (var entity in entities)
            {
                var detalle = this._mapper.MapToObject(entity);

                if (entity.T_CONTENEDOR != null)
                {
                    detalle.Contenedor = contenedores.FirstOrDefault(d => d.Numero == entity.T_CONTENEDOR.NU_CONTENEDOR);

                    if (detalle.Contenedor == null)
                    {
                        detalle.Contenedor = this._mapperContenedor.MapToObject(entity.T_CONTENEDOR);

                        contenedores.Add(detalle.Contenedor);
                    }

                }
                detalles.Add(detalle);
            }

            return detalles;
        }

        public virtual DetallePreparacion GetDetallePreparacion(Expression<Func<T_DET_PICKING, bool>> condition)
        {
            T_DET_PICKING entity = this._context.T_DET_PICKING
                .Include("T_CONTENEDOR")
                .AsNoTracking()
                .Where(condition)
                .FirstOrDefault();

            if (entity == null)
                return null;

            var detalle = this._mapper.MapToObject(entity);

            if (entity.T_CONTENEDOR != null)
                detalle.Contenedor = this._mapperContenedor.MapToObject(entity.T_CONTENEDOR);

            return detalle;
        }

        public virtual long? GetCargaContenedor(ContenedorFacturar contenedor)
        {
            long? nuCarga = _context.T_DET_PICKING
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_PREPARACION == contenedor.NumeroPreparacion
                    && x.NU_PEDIDO == contenedor.NumeroPedido
                    && x.CD_EMPRESA == contenedor.CodigoEmpresa
                    && x.CD_CLIENTE == contenedor.CodigoCliente
                    && x.NU_CONTENEDOR == contenedor.NumeroContenedor)?.NU_CARGA;

            return nuCarga;
        }

        public virtual List<DetallePreparacion> GetDatosPickingContenedor(ContenedorFacturar contenedor)
        {
            return this.GetDetallesPreparacion(x => x.NU_PREPARACION == contenedor.NumeroPreparacion && x.NU_PEDIDO == contenedor.NumeroPedido && x.CD_EMPRESA == contenedor.CodigoEmpresa
                && x.CD_CLIENTE == contenedor.CodigoCliente && x.NU_CONTENEDOR == contenedor.NumeroContenedor);
        }

        public virtual List<DetallePreparacion> GetDetallePreparacionByCarga(List<long> cargas)
        {
            return this.GetDetallesPreparacion(d => cargas.Contains(d.NU_CARGA ?? -1) && d.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO);//se excluyen cargas no preparadas
        }

        public virtual List<DetallePreparacion> GetDetallePreparacionByPedido(int empresa, string cliente, string pedido)
        {
            return this.GetDetallesPreparacion(d => d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && d.NU_PEDIDO == pedido && d.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO);
        }

        public virtual List<DetallePreparacion> GetDetallePreparacionByPedidoConCarga(int empresa, string cliente, string pedido)
        {
            var estadosAnulacion = EstadoDetallePreparacion.GetEstadosAnulacion();
            return this.GetDetallesPreparacion(d => d.NU_PEDIDO == pedido && d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && !estadosAnulacion.Contains(d.ND_ESTADO));
        }

        public virtual List<DetallePreparacion> GetDetallePreparacionByPedidoCarga(int empresa, string cliente, string pedido, long carga)
        {
            return this.GetDetallesPreparacion(d => d.NU_PEDIDO == pedido && d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && d.NU_CARGA == carga && d.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO);
        }

        public virtual List<DetPickingCamionWEXP040> GetsDetPreparacionCamion(int CdCamion)
        {
            List<DetPickingCamionWEXP040> list = new List<DetPickingCamionWEXP040>();
            List<V_DET_PICKING_CAMION_WEXP040> detPickingcamion = _context.V_DET_PICKING_CAMION_WEXP040.AsNoTracking().Where(v => v.CD_CAMION == CdCamion).ToList();
            foreach (var det in detPickingcamion)
            {
                DetPickingCamionWEXP040 picking = new DetPickingCamionWEXP040();
                picking.CodigoCamion = det.CD_CAMION;
                picking.NumeroCarga = det.NU_CARGA;
                picking.NumeroPreparacion = det.NU_PREPARACION;
                picking.CodigoEmpresa = det.CD_EMPRESA;
                picking.NumeroPedido = det.NU_PEDIDO;
                picking.CodigoCliente = det.CD_CLIENTE;
                picking.NumeroContenedor = det.NU_CONTENEDOR;
                picking.EmpaquetaContenedor = det.FL_EMPAQUETA_CONTENEDOR;
                picking.PermiteFactSinPrecinto = det.FL_PERMITE_FACT_SIN_PRECINTO;
                picking.Agrupacion = det.ID_AGRUPACION;
                list.Add(picking);

            }
            return list;
        }

        public virtual AnulacionPendienteCamion GetPrimeraAnulacionPendienteCamion(int cdCamion)
        {
            var anulacion = this._context.V_ANULACIONES_PENDIENT_CAMION.Where(d => d.CD_CAMION == cdCamion).FirstOrDefault();

            if (anulacion == null)
                return null;

            return this._mapper.MapToObject(anulacion);
        }

        public virtual DetallePreparacion GetDetallePreparacion(int NuPreparacion, int NuContenedor)
        {
            return this.GetDetallePreparacion(f => f.NU_PREPARACION == NuPreparacion && f.NU_CONTENEDOR == NuContenedor);
        }

        public virtual List<DetallePreparacion> GetDetallesPreparacion(int preparacion, int contenedor)
        {
            return this.GetDetallesPreparacion(f => f.NU_PREPARACION == preparacion && f.NU_CONTENEDOR == contenedor);
        }

        public virtual DetallePreparacion GetDetallePreparacionByKey(int prep, int empresa, string cliente, string pedido, int nuSeq, string endereco, string producto, string identificador, decimal faixa)
        {
            return this.GetDetallePreparacion(f => f.NU_PREPARACION == prep && f.CD_EMPRESA == empresa && f.CD_CLIENTE == cliente && f.NU_PEDIDO == pedido
            && f.NU_SEQ_PREPARACION == nuSeq && f.CD_ENDERECO == endereco && f.CD_PRODUTO == producto && f.NU_IDENTIFICADOR == identificador && f.CD_FAIXA == faixa);
        }


        public virtual int GetCantidadContenedoresEmbargados(int CdCamion)
        {
            return _context.V_CONT_SIN_EMBARCAR_WEXP041.Where(x => x.CD_CAMION == CdCamion).Count();
        }

        public virtual int GetContenedoresSinEmbarcar(int cdCamion)
        {
            return _context.V_CONT_SIN_EMBARCAR_WEXP041.Where(x => x.CD_CAMION == cdCamion).Count();
        }

        public virtual int GetContenedoresSinPreparar(int cdCamion)
        {
            return _context.V_PRODUCTOS_SIN_PREP_WEXP041.Where(x => x.CD_CAMION == cdCamion).Count();
        }

        public virtual List<ProductoContenedor> GetsProductoContenedor(int numeroContenedor, int nuPreparacion)
        {
            List<ProductoContenedor> list = new List<ProductoContenedor>();
            var productos = (from dp in _context.T_DET_PICKING
                             join c in _context.T_CONTENEDOR on dp.NU_CONTENEDOR equals c.NU_CONTENEDOR
                             where c.NU_PREPARACION == dp.NU_PREPARACION
                                && c.NU_CONTENEDOR == numeroContenedor
                                && c.NU_PREPARACION == nuPreparacion
                                && c.CD_SITUACAO == 601
                             select (new { c.CD_ENDERECO, dp.CD_EMPRESA, dp.CD_PRODUTO, dp.CD_FAIXA, dp.NU_IDENTIFICADOR, dp.QT_PREPARADO })).ToList();

            foreach (var a in productos)
            {
                ProductoContenedor prd = new ProductoContenedor();
                prd.CantidadPreparada = a.QT_PREPARADO;
                prd.Ubicacion = a.CD_ENDERECO;
                prd.CodigoEmpresa = a.CD_EMPRESA;
                prd.CodigoProducto = a.CD_PRODUTO;
                prd.Lote = a.NU_IDENTIFICADOR;
                prd.CodigoFaixa = a.CD_FAIXA;
                list.Add(prd);
            }

            return list;
        }

        public virtual List<string> GetClientesContenedor(int preparacion, int contenedor)
        {
            return this._context.T_DET_PICKING
                .AsNoTracking()
                .Where(x => x.NU_PREPARACION == preparacion && x.NU_CONTENEDOR == contenedor)
                .GroupBy(x => x.CD_CLIENTE)
                .Select(x => x.Key)
                .ToList();
        }

        public virtual List<string> GetPedidos(int preparacion)
        {
            return this._context.T_DET_PICKING
                .AsNoTracking()
                .Where(x => x.NU_PREPARACION == preparacion)
                .GroupBy(o => new { o.NU_PEDIDO })
                .Select(c => c.Key.NU_PEDIDO)
                .ToList();
        }

        public virtual List<string> GetClientesCarga(long carga)
        {
            return this._context.T_DET_PICKING
                .AsNoTracking()
                .Where(x => x.NU_CARGA == carga)
                .GroupBy(x => x.CD_CLIENTE)
                .Select(x => x.Key)
                .ToList();
        }

        public virtual List<string> GetGrupoExpedicionContenedor(int preparacion, int contenedor)
        {
            return this._context.T_DET_PICKING
                .Where(d => d.NU_PREPARACION == preparacion && d.NU_CONTENEDOR == contenedor)
                .Join(
                    this._context.T_PEDIDO_SAIDA,
                    dp => new { dp.CD_CLIENTE, dp.CD_EMPRESA, dp.NU_PEDIDO },
                    ps => new { ps.CD_CLIENTE, ps.CD_EMPRESA, ps.NU_PEDIDO },
                    (dp, ps) => new { DetPicking = dp, Pedido = ps }
                )
                .AsNoTracking()
                .GroupBy(d => d.Pedido.TP_EXPEDICION)
                .Select(d => d.Key)
                .ToList();
        }

        public virtual string GetGrupoExpedicionCarga(int preparacion, int empresa, string cliente, long carga)
        {
            return this._context.V_EXP010_CARGA_CAMION.Where(d => d.NU_PREPARACION == preparacion && d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && d.NU_CARGA == carga).Select(d => d.CD_GRUPO_EXPEDICION).FirstOrDefault();
        }

        public virtual string GetGrupoExpedicionContenedor(int preparacion, int empresa, string cliente, long carga, int contenedor)
        {
            return this._context.V_EXP011_CONTENEDOR_CAMION.Where(d => d.NU_PREPARACION == preparacion && d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && d.NU_CARGA == carga && d.NU_CONTENEDOR == contenedor).Select(d => d.CD_GRUPO_EXPEDICION).FirstOrDefault();
        }

        public virtual List<Carga> GetCargasPedido(string cliente, string pedido, int empresa)
        {
            var cargas = this._context.T_DET_PICKING
                .AsNoTracking()
                .Where(d => d.NU_PEDIDO == pedido && d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && d.NU_CARGA != null)
                .AsEnumerable()
                .GroupBy(d => d.NU_CARGA)
                .ToList()
                .Join(
                    this._context.T_CARGA.AsNoTracking(),
                    d => d.Key,
                    c => c.NU_CARGA,
                    (d, c) => new { Picking = d.Key, Carga = c })
                .Select(d => d.Carga)
                .ToList();

            var cargasRetornar = new List<Carga>();

            foreach (var carga in cargas)
            {
                cargasRetornar.Add(this._mapperCarga.MapToObject(carga));
            }

            return cargasRetornar;
        }

        public virtual Preparacion GetPreparacionPorNumero(int numero)
        {
            return _mapper.MapToObjectWhitoutDetail(_context.T_PICKING.FirstOrDefault(x => x.NU_PREPARACION == numero));
        }

        public virtual PreparacionManualConfiguracion GetConfiguracionPreparacionManual()
        {
            var configuracion = new PreparacionManualConfiguracion();

            configuracion.ControlTotal = (_paramRepo.GetParameter(ParamManager.WPRE300_Control_Picking_Total) ?? "N") == "S";

            configuracion.PermitePickearVencido = (_paramRepo.GetParameter(ParamManager.PRE052_DEFAULT_PICK_VENCIDO) ?? "N") == "S";
            configuracion.PermitePickearVencidoHabilitado = (_paramRepo.GetParameter(ParamManager.PRE052_ENABLED_PICK_VENCIDO) ?? "S") == "S";

            configuracion.PermitePickearAveriado = (_paramRepo.GetParameter(ParamManager.PRE052_DEFAULT_PICK_AVERIADO) ?? "N") == "S";
            configuracion.PermitePickearAveriadoHabilitado = (_paramRepo.GetParameter(ParamManager.PRE052_ENABLED_PICK_AVERIADO) ?? "S") == "S";

            configuracion.ValidarProductoProveedor = (_paramRepo.GetParameter(ParamManager.PRE052_DEFAULT_PROD_PROVEEDOR) ?? "N") == "S";
            configuracion.ValidarProductoProveedorHabilitado = (_paramRepo.GetParameter(ParamManager.PRE052_ENABLED_PROD_PROVEEDOR) ?? "S") == "S";

            return configuracion;
        }

        public virtual LiberacionConfiguracion GetLiberacionConfiguracion(string empresa = null)
        {
            LiberacionConfiguracion configuracion = new LiberacionConfiguracion();

            configuracion.UbicacionCompleta = this.GetValorParametroDefault("WPRE050_DEFAULT_PALLET_COMP", empresa);
            configuracion.UbicacionCompletaHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_PALLET_COMP", empresa);

            configuracion.UbicacionIncompleta = this.GetValorParametroDefault("WPRE050_DEFAULT_PALLET_INCO", empresa);
            configuracion.UbicacionIncompletaHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_PALLET_INCO", empresa);

            configuracion.AgruparCamion = this.GetValorParametroDefault("WPRE050_DEFAULT_AGRUP_CAM", empresa);
            configuracion.AgruparCamionHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_AGRUP_CAM", empresa);

            configuracion.PrepSoloCamion = this.GetValorParametroDefault("WPRE050_DEFAULT_PREP_CAM", empresa);
            configuracion.PrepSoloCamionHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_PREP_CAM", empresa);

            configuracion.DefaultStock = this.GetValorParametroDefault("WPRE050_DEFAULT_STOCK", empresa);
            configuracion.DefaultStockHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_STOCK", empresa);

            configuracion.Pedidos = this.GetValorParametroDefault("WPRE050_DEFAULT_PEDIDOS", empresa);
            configuracion.PedidosHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_PEDIDOS", empresa);

            configuracion.RepartirEscazes = this.GetValorParametroDefault("WPRE050_DEFAULT_REPA_ESCACEZ", empresa);
            configuracion.RepartirEscazesHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_REPA_ESCACEZ", empresa);

            configuracion.ControlStockDMTI = this.GetValorParametroDefault("WPRE050_DEFAULT_CTRL_STOCK", empresa);
            configuracion.ControlStockDMTIHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_CTRL_STOCK", empresa);

            configuracion.RespetaFifo = this.GetValorParametroDefault("WPRE050_DEFAULT_RESPETA_FIFO", empresa);
            configuracion.RespetaFifoHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_RESPETA_FIFO", empresa);

            configuracion.ManejoAgrupador = this.GetValorParametroDefault("WPRE050_VLMANEJO_AGRUPADOR", empresa);
            configuracion.ManejoAgrupadorHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_AGRUPADOR", empresa);

            configuracion.ManejaVidaUtil = this.GetValorParametroDefault("WPRE050_DEFAULT_MANEVIDAUTIL", empresa);
            configuracion.ManejaVidaUtilHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_MANEVIDAUTIL", empresa);

            configuracion.LiberarPorUnidad = this.GetValorParametroDefault("WPRE050_DEFAULT_LIB_UNI", empresa);
            configuracion.LiberarPorUnidadHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_LIB_UNI", empresa);

            configuracion.LiberarPorCurvas = this.GetValorParametroDefault("WPRE050_DEFAULT_LIB_CURVAS", empresa);
            configuracion.LiberarPorCurvasHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_LIB_CURVAS", empresa);

            configuracion.ManejoDocumental = this.GetValorParametroEnabled(ParamManager.MANEJO_DOCUMENTAL, empresa);

            configuracion.RequiereUbicacion = this.GetValorParametroDefault("WPRE050_DEFAULT_REQUIERE_UBIC", empresa);
            configuracion.RequiereUbicacionHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_REQUIERE_UBIC", empresa);

            configuracion.PriorizarDesborde = this.GetValorParametroDefault("WPRE050_DEFAULT_PRIO_DESBORDE", empresa);
            configuracion.PriorizarDesbordeHabilitado = this.GetValorParametroEnabled("WPRE050_ENABLED_PRIO_DESBORDE", empresa);

            configuracion.ExcluirUbicacionesPicking = this.GetValorParametroDefault("PRE052_DEFAULT_EX_UBIC_PICKING", empresa);
            configuracion.ExcluirUbicacionesPickingHabilitado = this.GetValorParametroEnabled("PRE052_ENABLED_EX_UBIC_PICKING", empresa);

            return configuracion;
        }

        public virtual List<Preparacion> GetPrepManualSinFinalizar(List<int> preps)
        {
            List<string> tpPrepManual = new List<string>() { TipoPreparacionDb.Pedido, TipoPreparacionDb.Libre };

            return _context.T_PICKING.AsNoTracking()
                .Where(p => preps.Contains(p.NU_PREPARACION)
                    && tpPrepManual.Contains(p.TP_PREPARACION)
                    && p.CD_SITUACAO != SituacionDb.PreparacionFinalizada)
                .Select(p => _mapper.MapToObject(p))
                .ToList();
        }

        public virtual string GetValorParametroDefault(string parametro, string empresa)
        {
            string value;

            Dictionary<string, string> colParams = new Dictionary<string, string>();
            colParams[ParamManager.PARAM_EMPR] = string.Format("{0}_{1}", ParamManager.PARAM_EMPR, string.IsNullOrEmpty(empresa) ? 0 : int.Parse(empresa));

            value = _paramRepo.GetParameter(parametro, colParams);

            return value;
        }

        public virtual bool GetValorParametroEnabled(string parametro, string empresa)
        {
            string value;

            Dictionary<string, string> colParams = new Dictionary<string, string>();
            colParams[ParamManager.PARAM_EMPR] = string.Format("{0}_{1}", ParamManager.PARAM_EMPR, string.IsNullOrEmpty(empresa) ? 0 : int.Parse(empresa));

            value = _paramRepo.GetParameter(parametro, colParams);

            return this._mapper.MapStringToBoolean(value);
        }

        public virtual Dictionary<string, decimal> GetCantidadesPreparacion(int numPreparacion, int empresa, string cliente, string pedido)
        {
            Dictionary<string, decimal> cantidades = new Dictionary<string, decimal>();

            decimal qt_producto_total = 0;
            int qt_pickeo_total = 0;

            var query2 = this._context.T_DET_PICKING
                .Where(s => s.NU_PREPARACION == numPreparacion
                    && s.CD_EMPRESA == empresa
                    && s.CD_CLIENTE == cliente
                    && s.NU_PEDIDO == pedido)
                .ToList()
                .GroupBy(x => new { x.CD_EMPRESA, x.NU_PREPARACION, x.CD_CLIENTE, x.NU_PEDIDO });

            var query = query2.Select(g => new
            {
                QT_PRODUCTOS_TOTALES = g.Sum(x => x.QT_PRODUTO),
                QT_PICKEOS_TOTALES = g.Count()
            });

            qt_producto_total = (decimal)query.FirstOrDefault().QT_PRODUCTOS_TOTALES;
            qt_pickeo_total = query.FirstOrDefault().QT_PICKEOS_TOTALES;

            cantidades.Add("producto_total", qt_producto_total);
            cantidades.Add("pickeo_total", qt_pickeo_total);

            return cantidades;
        }

        public virtual long? GetCarga(int preparacion, int contenedor)
        {
            return _context.T_DET_PICKING
                .AsNoTracking()
                .FirstOrDefault(w => w.NU_PREPARACION == preparacion
                    && w.NU_CONTENEDOR == contenedor)?.NU_CARGA;
        }

        public virtual DetallePreparacion GetPedidoDetallePreparacion(int NuPreparacion, int NuContenedor)
        {
            T_DET_PICKING detPick = _context.T_DET_PICKING
                .AsNoTracking()
                .FirstOrDefault(f => f.NU_PREPARACION == NuPreparacion
                    && f.NU_CONTENEDOR == NuContenedor);
            return detPick == null ? null : _mapper.MapToObject(detPick);
        }

        public virtual List<DetallePreparacion> GetDetallesPreparacionPorInsumo(int preparacion, int contenedor, string pedido, string cliente, int empresa, string producto)
        {
            return _context.T_DET_PICKING
                .Where(w => w.NU_CONTENEDOR == contenedor
                    && w.CD_PRODUTO == producto
                    && w.NU_PREPARACION == preparacion
                    && w.NU_PEDIDO == pedido
                    && w.CD_CLIENTE == cliente
                    && w.CD_EMPRESA == empresa)
                .AsNoTracking()
                .Select(w => _mapper.MapToObject(w))
                .ToList();
            ;
        }

        public virtual List<Preparacion> GetPreparacionByNumeroODescripcion(string value, string tpOperativa, int empresa)
        {
            var preparaciones = this._context.V_PREP_DISP_ASOCIAR
                .Where(p => p.TP_OPERATIVA == tpOperativa
                    && p.CD_EMPRESA == empresa);

            if (int.TryParse(value, out int nroPrep))
                preparaciones = preparaciones
                    .Where(e => e.NU_PREPARACION == nroPrep
                        || (e.NU_PREPARACION.ToString().Contains(nroPrep.ToString()))
                        || e.DS_PREPARACION.ToLower().Contains(value.ToLower()));
            else
                preparaciones = preparaciones
                    .Where(e => e.DS_PREPARACION.ToLower().Contains(value.ToLower()));

            return preparaciones.Select(e => _mapper.MapToObject(e)).ToList();
        }

        public virtual List<Preparacion> GetPreparacionesPendienteNotificarAutomatismo()
        {
            return _context.T_PICKING
                .AsNoTracking()
                .Include("T_DET_PICKING")
                .AsNoTracking()
                .Where(w => w.ID_AVISO == "A")
                .Select(w => _mapper.MapToObject(w))
                .ToList();
        }

        public virtual int GetNextNumeroSecPreparacion()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_DET_PICKING);
        }

        public virtual List<DetallePreparacionLpn> GetDetallesPreparacionPendienteLpn(int nroPreparacion)
        {
            return this._context.T_DET_PICKING_LPN.Where(x => x.NU_PREPARACION == nroPreparacion && x.QT_RESERVA > 0)
                .Join(this._context.T_LPN_DET,
                    dpl => new { dpl.NU_LPN, dpl.ID_LPN_DET, dpl.CD_PRODUTO, dpl.NU_IDENTIFICADOR },
                    ld => new { NU_LPN = (long?)ld.NU_LPN, ID_LPN_DET = (int?)ld.ID_LPN_DET, ld.CD_PRODUTO, ld.NU_IDENTIFICADOR },
                    (dpl, ld) => new { DetallePickingLpn = dpl, DetalleLpn = ld })
                .Join(this._context.T_DET_PICKING,
                    dplld => new {  dplld.DetallePickingLpn.NU_PREPARACION, ID_DET_PICKING_LPN = (long?)dplld.DetallePickingLpn.ID_DET_PICKING_LPN },
                    dp => new { dp.NU_PREPARACION, dp.ID_DET_PICKING_LPN },
                    (dplld,dp) => new { DetallePickingLpn = dplld.DetallePickingLpn, DetalleLpn = dplld.DetalleLpn,Picking = dp })
                .AsNoTracking()
                .Select(x => new DetallePreparacionLpn
                {
                    NroPreparacion = nroPreparacion,
                    NroLpn = x.DetalleLpn.NU_LPN,
                    IdDetalleLpn = x.DetalleLpn.ID_LPN_DET,
                    Empresa = x.DetalleLpn.CD_EMPRESA,
                    Producto = x.DetalleLpn.CD_PRODUTO,
                    Lote = x.DetalleLpn.NU_IDENTIFICADOR,
                    Faixa = x.DetalleLpn.CD_FAIXA,
                    Cantidad = x.DetallePickingLpn.QT_RESERVA,
                    IdDetallePickingLpn = x.DetallePickingLpn.ID_DET_PICKING_LPN,
                    Pedido = x.Picking.NU_PEDIDO,
                    Cliente = x.Picking.CD_CLIENTE,
                    NumeroSecuencia = x.Picking.NU_SEQ_PREPARACION,
                    EspecificaLote = x.Picking.ID_ESPECIFICA_IDENTIFICADOR,
                    Vencimiento = x.DetalleLpn.DT_FABRICACAO,
                    Agrupacion = x.Picking.ID_AGRUPACION,
                    Ubicacion = x.Picking.CD_ENDERECO
                }).ToList();
        }

        public virtual List<DetallePreparacionLpn> GetDetallesLpnContenedor(int nroPreparacion)
        {
            return this._context.T_CONTENEDOR.Where(x => x.NU_PREPARACION == nroPreparacion && x.NU_LPN != null && x.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion)
               .Join(this._context.T_LPN_DET,
                   c => new { c.NU_LPN },
                   ld => new { NU_LPN = (long?)ld.NU_LPN },
                   (c, ld) => new { DetalleLpn = ld, Contenedor = c })
                .AsNoTracking()
               .Select(x => new DetallePreparacionLpn
               {
                   NroLpn = x.DetalleLpn.NU_LPN,
                   IdDetalleLpn = x.DetalleLpn.ID_LPN_DET,
                   Producto = x.DetalleLpn.CD_PRODUTO,
                   Empresa = x.DetalleLpn.CD_EMPRESA,
                   Lote = x.DetalleLpn.NU_IDENTIFICADOR,
                   Faixa = x.DetalleLpn.CD_FAIXA,
                   Cantidad = x.DetalleLpn.QT_ESTOQUE,
                   Vencimiento = x.DetalleLpn.DT_FABRICACAO,
                   Ubicacion = x.Contenedor.CD_ENDERECO
               }).ToList();
        }

        #endregion

        #region Add

        public virtual long CopiarCarga(long nroCarga, string desc = null, short? cdRota = null)
        {
            //Crear carga nueva, en base a anterior
            var carga = _context.T_CARGA.Where(c => c.NU_CARGA == nroCarga).FirstOrDefault();

            nroCarga = GetNextNumeroCarga();

            T_CARGA nuevaCarga = new T_CARGA
            {
                NU_CARGA = nroCarga,
                DS_CARGA = desc ?? carga.DS_CARGA,
                CD_ROTA = cdRota ?? carga.CD_ROTA,
                DT_ADDROW = DateTime.Now,
                NU_PREPARACION = carga.NU_PREPARACION
            };

            if (nuevaCarga.DS_CARGA.Length > 48)
                nuevaCarga.DS_CARGA = nuevaCarga.DS_CARGA.Substring(0, 48);

            _context.T_CARGA.Add(nuevaCarga);

            return nroCarga;
        }

        public virtual int AddPreparacion(Preparacion preparacion)
        {
            preparacion.Id = this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_PREPARACION);

            T_PICKING picking = this._mapper.MapToEntity(preparacion);

            foreach (var documento in preparacion.Documentos)
            {
                picking.T_DOCUMENTO_LIBERACION.Add(new T_DOCUMENTO_LIBERACION
                {
                    NU_PREPARACION = preparacion.Id,
                    TP_DOCUMENTO = documento.Tipo,
                    NU_DOCUMENTO = documento.Numero
                });
            }

            this._context.T_PICKING.Add(picking);

            return preparacion.Id;
        }

        public virtual void AddPrepNoAnular(PrepNoAnular prep)
        {
            this._context.T_PREP_NO_ANULAR.Add(this._mapper.MapToEntity(prep));
        }

        public virtual void AddDetallePreparacion(DetallePreparacion detalle)
        {
            if (detalle.NumeroSecuencia < 1)
                detalle.NumeroSecuencia = GetNextNumeroSecPreparacion();

            var entity = this._mapper.MapToEntity(detalle);

            this._context.T_DET_PICKING.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateDetallePreparacion(DetallePreparacion detP)
        {
            T_DET_PICKING entity = this._mapper.MapToEntity(detP);
            T_DET_PICKING attachedEntity = _context.T_DET_PICKING.Local
                .FirstOrDefault(x => x.CD_PRODUTO.Equals(detP.Producto)
                    && x.CD_EMPRESA == detP.Empresa
                    && x.NU_IDENTIFICADOR.Equals(detP.Lote)
                    && x.CD_FAIXA == detP.Faixa
                    && x.NU_PREPARACION == detP.NumeroPreparacion
                    && x.NU_PEDIDO.Equals(detP.Pedido)
                    && x.CD_CLIENTE.Equals(detP.Cliente)
                    && x.CD_ENDERECO.Equals(detP.Ubicacion)
                    && x.NU_SEQ_PREPARACION == detP.NumeroSecuencia);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DET_PICKING.Attach(entity);
                _context.Entry<T_DET_PICKING>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdatePreparacion(Preparacion prepa)
        {
            T_PICKING entity = this._mapper.MapToEntity(prepa);
            T_PICKING attachedEntity = _context.T_PICKING.Local.FirstOrDefault(x => x.NU_PREPARACION == prepa.Id);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PICKING.Attach(entity);
                _context.Entry<T_PICKING>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdatePrepNoAnular(PrepNoAnular prep)
        {
            T_PREP_NO_ANULAR entity = this._mapper.MapToEntity(prep);
            T_PREP_NO_ANULAR attachedEntity = _context.T_PREP_NO_ANULAR.Local
                .FirstOrDefault(s => s.CD_CLIENTE == prep.cdCliente
                    && s.CD_EMPRESA == prep.cdEmpresa
                    && s.NU_PEDIDO == prep.nuPedido
                    && s.NU_PREPARACION == prep.nuPreparacion);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PREP_NO_ANULAR.Attach(entity);
                _context.Entry<T_PREP_NO_ANULAR>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemovePreparacion(int nroPreparacion)
        {
            var entity = this._context.T_PICKING
                .FirstOrDefault(d => d.NU_PREPARACION == nroPreparacion);
            var attachedEntity = this._context.T_PICKING.Local
                .FirstOrDefault(d => d.NU_PREPARACION == nroPreparacion);

            if (attachedEntity != null)
            {
                this._context.T_PICKING.Remove(attachedEntity);
            }
            else
            {
                this._context.T_PICKING.Remove(entity);
            }
        }

        public virtual void RemovePrepNoAnular(string cdCliente, int cdEmpresa, string nuPedido, int nuPreparacion)
        {
            _context.T_PREP_NO_ANULAR.Remove(_context.T_PREP_NO_ANULAR.FirstOrDefault(s => s.CD_CLIENTE == cdCliente && s.CD_EMPRESA == cdEmpresa && s.NU_PEDIDO == nuPedido && s.NU_PREPARACION == nuPreparacion));
        }


        public virtual void RemoveTablasTemporalesPickingAdministrativo()
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();


            string sql = @"DELETE FROM T_STOCK_TEMP";
            _dapper.Execute(connection, sql, commandType: CommandType.Text, transaction: tran);

            sql = @"DELETE FROM T_LPN_TEMP";
            _dapper.Execute(connection, sql, commandType: CommandType.Text, transaction: tran);

            sql = @"DELETE FROM T_DET_PEDIDO_SAIDA_TEMP";
            _dapper.Execute(connection, sql, commandType: CommandType.Text, transaction: tran);

            sql = @"DELETE FROM T_DOC_PREPARACION_RESERV_TEMP";
            _dapper.Execute(connection, sql, commandType: CommandType.Text, transaction: tran);

            sql = @"DELETE FROM T_DET_PICKING_TEMP";
            _dapper.Execute(connection, sql, commandType: CommandType.Text, transaction: tran);
        }

        #endregion

        #region Dapper

        public virtual void ProcesarReabastecimientoPredio(string fechaDesde, string fechaHasta, string empresa, string predioDefault, string predio, string logic)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime? fechaI = DateTimeExtension.ParseFromIso(fechaDesde);
            DateTime? fechaH = DateTimeExtension.ParseFromIso(fechaHasta);
            int emp = int.Parse(empresa);
            string sql = "K_LIBERA.PR_CARGAR_STOCK_RESERVA";

            _dapper.Query<object>(_context.Database.GetDbConnection(), sql, param: new
            {
                P_DESDE = fechaI,
                P_HASTA = fechaH,
                P_EMPRESA = emp,
                P_PREDIO_STOCK = (string.IsNullOrEmpty(predioDefault) ? "-1" : predioDefault),
                P_PREDIO_NECESIDAD = predio,
                P_LOGICA = logic,
            }, commandType: CommandType.StoredProcedure, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #region Api Picking

        public virtual async Task Preparar(List<DetallePreparacion> pickeos, IPickingServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    var bulkContext = await GetBulkOperationContext(pickeos, context, connection, tran);

                    await BulkInsertContenedor(connection, tran, bulkContext.NewContenedores.Values);
                    await BulkUpdateContenedor(connection, tran, bulkContext.UpdateContenedores.Values);

                    await BulkUpdateStock(connection, tran, bulkContext.UpdateStockBaja.Values, false);
                    await BulkUpdateStock(connection, tran, bulkContext.UpdateStockAlta.Values, true);
                    await BulkInsertStock(connection, tran, bulkContext.NewStock.Values);

                    await BulkUpdateRemoveDetallePikcing(connection, tran, bulkContext.RemoveDetallesPicking);
                    await BulkRemoveDetallePikcing(connection, tran, bulkContext.RemoveDetallesPicking);
                    await BulkUpdateDetallePicking(connection, tran, bulkContext.UpdateDetallesPicking);
                    await BulkInsertDetallePicking(connection, tran, bulkContext.NewDetallesPicking);

                    await BulkUpdateReservaDocumental(connection, tran, bulkContext.UpdateReservasDocumentales.Values);
                    await BulkRemoveReservaDocumental(connection, tran, bulkContext.RemoveReservasDocumentales.Values);
                    await BulkInsertReservaDocumental(connection, tran, bulkContext.NewReservasDocumentales.Values);

                    await BulkUpdateEquipos(connection, tran, bulkContext.UpdateEquipos.Values);

                    tran.Commit();
                }
            }
        }

        public virtual async Task<PickingBulkOperationContext> GetBulkOperationContext(List<DetallePreparacion> pickeos, IPickingServiceContext serviceContext, DbConnection connection, DbTransaction tran)
        {
            int countSecuencia = 0;
            Preparacion preparacion = null;
            Preparacion preparacionDestino = null;
            string predioOperacion = string.Empty;

            var context = new PickingBulkOperationContext();
            var manejaDocumental = serviceContext.ManejaDocumental();
            var detallesModificados = new List<DetallePreparacion>();
            var contenedorSecuenciaAsociada = new Dictionary<string, int>();

            var nuTransaccion = await CreateTransaction($"Api de picking", connection, tran);
            var usuarioFinal = pickeos.FirstOrDefault().UsuarioPickeo;

            var cantNuevosContenedores = pickeos.Where(p => !p.ExisteContenedor)
                .GroupBy(x => new { x.IdExternoContenedor, x.TipoContenedor })
                .Select(x => x.Key)
                .Count();

            var secuenciasContenedores = new List<int>();
            if (cantNuevosContenedores > 0)
                secuenciasContenedores = GetNewContenedores(cantNuevosContenedores, connection, tran);

            var preparaciones = pickeos
                .GroupBy(x => new { x.NumeroPreparacion, x.Agrupacion })
                .Select(x => x.Key)
                .OrderBy(x => x.NumeroPreparacion)
                .ToList();

            foreach (var prep in preparaciones)
            {
                IEnumerable<DetallePreparacion> detallesAgrupados = new List<DetallePreparacion>();
                var detallesPrep = pickeos.Where(x => x.NumeroPreparacion == prep.NumeroPreparacion);

                switch (prep.Agrupacion)
                {
                    case Agrupacion.Pedido:
                        detallesAgrupados = detallesPrep.GroupBy(x => new { x.NumeroPreparacion, x.Ubicacion, x.Pedido, x.Empresa, x.Cliente, x.Producto, x.Lote, x.Faixa, x.IdExternoContenedor, x.TipoContenedor })
                        .Select(x => new DetallePreparacion
                        {
                            NumeroPreparacion = x.Key.NumeroPreparacion,
                            Ubicacion = x.Key.Ubicacion,
                            Pedido = x.Key.Pedido,
                            Empresa = x.Key.Empresa,
                            Cliente = x.Key.Cliente,
                            Producto = x.Key.Producto,
                            Lote = x.Key.Lote,
                            Faixa = x.Key.Faixa,
                            IdExternoContenedor = x.Key.IdExternoContenedor,
                            TipoContenedor = x.Key.TipoContenedor,
                            Contenedor = x.FirstOrDefault(d => d.IdExternoContenedor == x.Key.IdExternoContenedor && d.TipoContenedor == x.Key.TipoContenedor).Contenedor,
                            Cantidad = x.Sum(d => d.Cantidad),
                            Estado = x.Min(d => d.Estado)
                        });
                        break;
                    case Agrupacion.Cliente:
                        detallesAgrupados = detallesPrep.GroupBy(x => new { x.NumeroPreparacion, x.Ubicacion, x.Empresa, x.Cliente, x.Producto, x.Lote, x.Faixa, x.ComparteContenedorPicking, x.IdExternoContenedor, x.TipoContenedor })
                        .Select(x => new DetallePreparacion
                        {
                            NumeroPreparacion = x.Key.NumeroPreparacion,
                            Ubicacion = x.Key.Ubicacion,
                            Empresa = x.Key.Empresa,
                            Cliente = x.Key.Cliente,
                            Producto = x.Key.Producto,
                            Lote = x.Key.Lote,
                            Faixa = x.Key.Faixa,
                            ComparteContenedorPicking = x.Key.ComparteContenedorPicking,
                            IdExternoContenedor = x.Key.IdExternoContenedor,
                            TipoContenedor = x.Key.TipoContenedor,
                            Contenedor = x.FirstOrDefault(d => d.IdExternoContenedor == x.Key.IdExternoContenedor && d.TipoContenedor == x.Key.TipoContenedor).Contenedor,
                            Cantidad = x.Sum(d => d.Cantidad),
                            Estado = x.Min(d => d.Estado)
                        });
                        break;
                    case Agrupacion.Ruta:
                        detallesAgrupados = detallesPrep.GroupBy(x => new { x.NumeroPreparacion, x.Ubicacion, x.Empresa, x.Producto, x.Lote, x.Faixa, x.Carga, x.ComparteContenedorPicking, x.IdExternoContenedor, x.TipoContenedor })
                        .Select(x => new DetallePreparacion
                        {
                            NumeroPreparacion = x.Key.NumeroPreparacion,
                            Ubicacion = x.Key.Ubicacion,
                            Empresa = x.Key.Empresa,
                            Producto = x.Key.Producto,
                            Lote = x.Key.Lote,
                            Faixa = x.Key.Faixa,
                            Carga = x.Key.Carga,
                            ComparteContenedorPicking = x.Key.ComparteContenedorPicking,
                            IdExternoContenedor = x.Key.IdExternoContenedor,
                            TipoContenedor = x.Key.TipoContenedor,
                            Contenedor = x.FirstOrDefault(d => d.IdExternoContenedor == x.Key.IdExternoContenedor && d.TipoContenedor == x.Key.TipoContenedor).Contenedor,
                            Cantidad = x.Sum(d => d.Cantidad),
                            Estado = x.Min(d => d.Estado)
                        });
                        break;
                    case Agrupacion.Onda:
                        detallesAgrupados = detallesPrep.GroupBy(x => new { x.NumeroPreparacion, x.Ubicacion, x.Empresa, x.Producto, x.Lote, x.Faixa, x.ComparteContenedorPicking, x.IdExternoContenedor, x.TipoContenedor })
                        .Select(x => new DetallePreparacion
                        {
                            NumeroPreparacion = x.Key.NumeroPreparacion,
                            Ubicacion = x.Key.Ubicacion,
                            Empresa = x.Key.Empresa,
                            Producto = x.Key.Producto,
                            Lote = x.Key.Lote,
                            Faixa = x.Key.Faixa,
                            ComparteContenedorPicking = x.Key.ComparteContenedorPicking,
                            IdExternoContenedor = x.Key.IdExternoContenedor,
                            TipoContenedor = x.Key.TipoContenedor,
                            Contenedor = x.FirstOrDefault(d => d.IdExternoContenedor == x.Key.IdExternoContenedor && d.TipoContenedor == x.Key.TipoContenedor).Contenedor,
                            Cantidad = x.Sum(d => d.Cantidad),
                            Estado = x.Min(d => d.Estado)
                        });
                        break;
                }

                foreach (var det in detallesAgrupados)
                {
                    var detallesPendientes = serviceContext.GetDetallesPendiente(det, prep.Agrupacion);

                    var cantidadPendienteDistribuir = det.Cantidad;
                    foreach (var detallePendiente in detallesPendientes)
                    {
                        if (cantidadPendienteDistribuir == 0)
                            break;

                        if (cantidadPendienteDistribuir >= detallePendiente.Cantidad)
                        {
                            detallePendiente.FechaModificacion = DateTime.Now;
                            detallePendiente.Transaccion = nuTransaccion;
                            detallePendiente.TransaccionDelete = nuTransaccion;
                            context.RemoveDetallesPicking.Add(GetRemoveDetallePickingEntity(detallePendiente));

                            cantidadPendienteDistribuir -= detallePendiente.Cantidad;
                            detallePendiente.Cantidad = detallePendiente.Cantidad;
                        }
                        else
                        {
                            detallePendiente.Cantidad -= cantidadPendienteDistribuir; //Para el update del detalle de picking.
                            detallePendiente.Transaccion = nuTransaccion;
                            detallePendiente.FechaModificacion = DateTime.Now;
                            context.UpdateDetallesPicking.Add(GetUpdateDetallePickingEntity(detallePendiente));

                            detallePendiente.Cantidad = cantidadPendienteDistribuir;
                            cantidadPendienteDistribuir = 0;
                        }

                        detallePendiente.IdExternoContenedor = det.IdExternoContenedor;
                        detallePendiente.TipoContenedor = det.TipoContenedor;
                        detallePendiente.Contenedor = det.Contenedor;
                        detallePendiente.Agrupacion = prep.Agrupacion;

                        int numeroContenedor;
                        if (det.Contenedor.Numero != -1)
                            numeroContenedor = det.Contenedor.Numero;
                        else
                        {
                            var keyCont = $"{det.IdExternoContenedor}.{det.TipoContenedor}";
                            if (!contenedorSecuenciaAsociada.ContainsKey(keyCont))
                            {
                                var nuCont = secuenciasContenedores.FirstOrDefault();
                                contenedorSecuenciaAsociada[keyCont] = nuCont;
                                numeroContenedor = nuCont;
                                secuenciasContenedores.Remove(nuCont);
                            }
                            else
                                numeroContenedor = contenedorSecuenciaAsociada[keyCont];
                        }

                        detallePendiente.NroContenedor = numeroContenedor;

                        detallesModificados.Add(detallePendiente);
                        countSecuencia++;
                    }
                }
            }

            var secuencias = GetSecuenciasDetallePicking(countSecuencia, connection, tran);

            foreach (var pick in detallesModificados)
            {
                pick.Transaccion = nuTransaccion;
                if (preparacion == null)
                {
                    preparacion = serviceContext.GetPreparacion(pick.NumeroPreparacion);
                    predioOperacion = preparacion.Predio;
                }
                else if (preparacion.Id != pick.NumeroPreparacion)
                {
                    preparacion = serviceContext.GetPreparacion(pick.NumeroPreparacion);
                    predioOperacion = preparacion.Predio;
                }

                #region Contenedor

                var ubicacionContenedor = serviceContext.GetUbicacionEquipo(predioOperacion);

                var contenedor = serviceContext.GetContenedor(pick.IdExternoContenedor, pick.TipoContenedor);
                var keyContenedor = $"{pick.NumeroPreparacion}.{pick.NroContenedor}";

                if (contenedor == null)
                {
                    if (string.IsNullOrEmpty(pick.Contenedor.Ubicacion))
                        pick.Contenedor.Ubicacion = ubicacionContenedor.Ubicacion;
                    else if (ubicacionContenedor != null)
                        ubicacionContenedor.Ubicacion = pick.Contenedor.Ubicacion;

                    if (!context.NewContenedores.ContainsKey(keyContenedor))
                        context.NewContenedores[keyContenedor] = GetNewContenedorEntity(pick);

                    if (ubicacionContenedor != null && ubicacionContenedor.AutoAsignado == "N" && !context.UpdateEquipos.ContainsKey(ubicacionContenedor.CodigoEquipo))
                        context.UpdateEquipos[ubicacionContenedor.CodigoEquipo] = GetUpdateEquipoEntity(ubicacionContenedor.CodigoEquipo);

                    if (ubicacionContenedor == null && !string.IsNullOrEmpty(pick.Contenedor.Ubicacion))
                        ubicacionContenedor = new UbicacionEquipo() { Ubicacion = pick.Contenedor.Ubicacion };
                }
                else
                {
                    preparacionDestino = serviceContext.GetPreparacion(contenedor.NumeroPreparacion);

                    if (ubicacionContenedor != null)
                        ubicacionContenedor.Ubicacion = contenedor.Ubicacion;
                    else
                        ubicacionContenedor = new UbicacionEquipo() { Ubicacion = contenedor.Ubicacion };

                    if (!context.UpdateContenedores.ContainsKey(keyContenedor))
                        context.UpdateContenedores[keyContenedor] = GetUpdateContenedorEntity(pick, preparacionDestino.Id);
                }
                #endregion

                #region Stock

                var keyStock = $"{pick.Ubicacion}.{pick.Empresa}.{pick.Producto}.{pick.Lote}.{pick.Faixa.ToString("#.###")}";
                if (!context.UpdateStockBaja.ContainsKey(keyStock))
                    context.UpdateStockBaja[keyStock] = GetUpdateStockEntity(pick, pick.Ubicacion);
                else
                    context.UpdateStockBaja[keyStock].Cantidad += pick.Cantidad;

                var vencimiento = serviceContext.GetStock(pick.Ubicacion, pick.Empresa, pick.Producto, pick.Lote, pick.Faixa).Vencimiento;

                var keyStockDestino = $"{ubicacionContenedor.Ubicacion}.{pick.Empresa}.{pick.Producto}.{pick.Lote}.{pick.Faixa.ToString("#.###")}";
                var stockDestino = serviceContext.GetStock(ubicacionContenedor.Ubicacion, pick.Empresa, pick.Producto, pick.Lote, pick.Faixa);

                if (stockDestino == null)
                {
                    if (!context.NewStock.ContainsKey(keyStockDestino))
                        context.NewStock[keyStockDestino] = GetNewStockEntity(pick, ubicacionContenedor.Ubicacion);
                    else
                        context.NewStock[keyStockDestino].Cantidad += pick.Cantidad;
                }
                else
                {
                    if (!context.UpdateStockAlta.ContainsKey(keyStockDestino))
                        context.UpdateStockAlta[keyStockDestino] = GetUpdateStockEntity(pick, ubicacionContenedor.Ubicacion);
                    else
                        context.UpdateStockAlta[keyStockDestino].Cantidad += pick.Cantidad;
                }

                #endregion

                #region Picking

                pick.VencimientoPickeo = vencimiento;
                pick.FechaAlta = DateTime.Now;
                pick.NumeroSecuencia = secuencias.FirstOrDefault();
                pick.CantidadPreparada = pick.Cantidad;
                pick.FechaPickeo = DateTime.Now;
                pick.NumeroContenedorPickeo = pick.NroContenedor;
                pick.UsuarioPickeo = usuarioFinal;
                pick.Estado = EstadoDetallePreparacion.ESTADO_PREPARADO;
                pick.Transaccion = nuTransaccion;

                if (preparacionDestino != null && pick.NumeroPreparacion != preparacionDestino.Id)
                {
                    var pickDestino = serviceContext.DetallesPreparacionDestino.FirstOrDefault(d => d.NumeroPreparacion == preparacionDestino.Id && d.NroContenedor == contenedor.Numero);
                    pick.NumeroPreparacion = preparacionDestino.Id;
                    pick.Carga = pickDestino.Carga;
                    pick.Agrupacion = pickDestino.Agrupacion;

                    if (manejaDocumental)
                        ReasignarPreparacionReservaDocumental(pick, context, serviceContext, preparacion.Id);
                }
                else if (manejaDocumental)
                    AsignarPreparacionReservaDocumental(pick, context, serviceContext);

                context.NewDetallesPicking.Add(GetNewDetallePickingEntity(pick));
                secuencias.Remove(pick.NumeroSecuencia);

                #endregion
            }

            return context;
        }

        public virtual void AsignarPreparacionReservaDocumental(DetallePreparacion pick, PickingBulkOperationContext context, IPickingServiceContext serviceContext)
        {
            var saldoAPreparar = pick.CantidadPreparada ?? 0;
            var reservas = serviceContext.GetReservasDetalles(pick.NumeroPreparacion, pick.Empresa, pick.Producto, pick.Lote, pick.Faixa);

            foreach (var reserva in reservas)
            {
                if (saldoAPreparar == 0)
                    break;

                var saldoReserva = reserva.CantidadDisponible();
                var cantidad = Math.Min(saldoAPreparar, saldoReserva);

                reserva.CantidadPreparada = (reserva.CantidadPreparada ?? 0) + cantidad;
                reserva.NumeroTransaccion = pick.Transaccion;
                reserva.FechaModificacion = DateTime.Now;
                reserva.Auditoria = string.Format("{0}${1}${2}", _application, _userId, pick.Transaccion);

                var keyReserva = $"{reserva.NroDocumento}.{reserva.TipoDocumento}.{reserva.Preparacion}.{reserva.Empresa}.{reserva.Producto}.{reserva.Faixa.ToString("#.###")}.{reserva.NroIdentificadorPicking}.{reserva.Identificador}";
                if (!context.UpdateReservasDocumentales.ContainsKey(keyReserva))
                    context.UpdateReservasDocumentales[keyReserva] = reserva;
                else
                    context.UpdateReservasDocumentales[keyReserva].CantidadPreparada += cantidad;

                saldoAPreparar -= cantidad;
            }
        }

        public virtual void ReasignarPreparacionReservaDocumental(DetallePreparacion pick, PickingBulkOperationContext context, IPickingServiceContext serviceContext, int preparacionOrigen)
        {
            var auditoria = string.Format("{0}${1}${2}", _application, _userId, pick.Transaccion);

            var saldoAReasignar = pick.CantidadPreparada ?? 0;
            var reservasOrigen = serviceContext.GetReservasDetalles(preparacionOrigen, pick.Empresa, pick.Producto, pick.Lote, pick.Faixa);

            foreach (var reserva in reservasOrigen)
            {
                if (saldoAReasignar == 0)
                {
                    break;
                }

                var saldoReserva = reserva.CantidadDisponible();
                var cambio = (decimal)0;

                if (saldoAReasignar <= saldoReserva)
                {
                    reserva.CantidadProducto = (reserva.CantidadProducto ?? 0) - saldoAReasignar;
                    cambio = saldoAReasignar;
                    saldoAReasignar = 0;
                }
                else
                {
                    reserva.CantidadProducto = (reserva.CantidadProducto ?? 0) - saldoReserva;
                    saldoAReasignar -= saldoReserva;
                    cambio = saldoReserva;
                }
                reserva.NumeroTransaccion = pick.Transaccion;
                reserva.FechaModificacion = DateTime.Now;
                reserva.Auditoria = auditoria;

                var keyReservaOrigen = $"{reserva.NroDocumento}.{reserva.TipoDocumento}.{reserva.Preparacion}.{reserva.Empresa}.{reserva.Producto}.{reserva.Faixa.ToString("#.###")}.{reserva.NroIdentificadorPicking}.{reserva.Identificador}";
                if (!context.UpdateReservasDocumentales.ContainsKey(keyReservaOrigen))
                    context.UpdateReservasDocumentales[keyReservaOrigen] = reserva;
                else
                    context.UpdateReservasDocumentales[keyReservaOrigen].CantidadProducto -= cambio;

                if ((reserva.CantidadProducto ?? 0) == 0)
                {
                    context.UpdateReservasDocumentales[keyReservaOrigen].NumeroTransaccionDelete = pick.Transaccion;

                    if (!context.RemoveReservasDocumentales.ContainsKey(keyReservaOrigen))
                        context.RemoveReservasDocumentales[keyReservaOrigen] = reserva;
                }

                var reservaDestino = serviceContext.GetReservasDocumento(reserva.NroDocumento, reserva.TipoDocumento, pick.NumeroPreparacion, pick.Empresa, pick.Producto, pick.Lote, pick.Faixa);
                var keyReservaDestino = $"{reserva.NroDocumento}.{reserva.TipoDocumento}.{reservaDestino.Preparacion}.{reserva.Empresa}.{reserva.Producto}.{reserva.Faixa.ToString("#.###")}.{reserva.NroIdentificadorPicking}.{reserva.Identificador}";

                if (reservaDestino != null)
                {
                    if (!context.UpdateReservasDocumentales.ContainsKey(keyReservaDestino))
                    {
                        reservaDestino.CantidadProducto = (reservaDestino.CantidadProducto ?? 0) + cambio;
                        reservaDestino.CantidadPreparada = (reservaDestino.CantidadPreparada ?? 0) + cambio;
                        reservaDestino.Auditoria = auditoria;
                        reservaDestino.FechaModificacion = DateTime.Now;

                        context.UpdateReservasDocumentales[keyReservaDestino] = reservaDestino;
                    }
                    else
                    {
                        context.UpdateReservasDocumentales[keyReservaDestino].CantidadProducto += cambio;
                        context.UpdateReservasDocumentales[keyReservaDestino].CantidadPreparada += cambio;
                    }
                }
                else
                {
                    if (!context.NewReservasDocumentales.ContainsKey(keyReservaDestino))
                    {
                        var newReserva = new DocumentoPreparacionReserva()
                        {
                            NroDocumento = reserva.NroDocumento,
                            TipoDocumento = reserva.TipoDocumento,
                            Preparacion = pick.NumeroPreparacion,
                            Empresa = pick.Empresa,
                            Producto = pick.Producto,
                            Identificador = pick.Lote,
                            Faixa = pick.Faixa,
                            NroIdentificadorPicking = pick.Lote,
                            EspecificaIdentificadorId = pick.EspecificaLote,
                            CantidadProducto = cambio,
                            CantidadPreparada = cambio,
                            CantidadAnular = 0,
                            Auditoria = auditoria,
                            FechaAlta = DateTime.Now
                        };
                        context.NewReservasDocumentales[keyReservaDestino] = newReserva;
                    }
                    else
                    {
                        context.NewReservasDocumentales[keyReservaDestino].CantidadProducto += cambio;
                        context.NewReservasDocumentales[keyReservaDestino].CantidadPreparada += cambio;
                    }
                }
            }
        }

        public virtual async Task BulkInsertContenedor(DbConnection connection, DbTransaction tran, IEnumerable<object> contenedores)
        {
            string sql = @"INSERT INTO T_CONTENEDOR 
                               (NU_PREPARACION,
                                NU_CONTENEDOR,
                                TP_CONTENEDOR,
                                CD_SITUACAO,
                                CD_ENDERECO,
                                CD_SUB_CLASSE,
                                DT_ADDROW,
                                FL_HABILITADO,
                                NU_TRANSACCION,
                                ID_EXTERNO_CONTENEDOR,
                                CD_BARRAS) 
                            VALUES 
                                (:NumeroPreparacion,
                                :Numero,
                                :TipoContenedor,
                                :Estado,
                                :Ubicacion,
                                :CodigoSubClase,
                                :FechaAgregado,
                                :Habilitado,
                                :NumeroTransaccion,
                                :IdExterno,
                                :CodigoBarras)";

            await _dapper.ExecuteAsync(connection, sql, contenedores, transaction: tran);
        }

        public virtual async Task BulkUpdateContenedor(DbConnection connection, DbTransaction tran, IEnumerable<object> contenedores)
        {
            string sql = @"UPDATE T_CONTENEDOR 
                           SET DT_UPDROW = :FechaModificado, 
                           NU_TRANSACCION = :NumeroTransaccion
                           WHERE NU_PREPARACION = :NumeroPreparacion 
                           AND NU_CONTENEDOR = :Numero";

            await _dapper.ExecuteAsync(connection, sql, contenedores, transaction: tran);
        }

        public virtual async Task BulkUpdateStock(DbConnection connection, DbTransaction tran, IEnumerable<object> stocks, bool incrementaStock)
        {
            var sql = @"UPDATE T_STOCK SET 
                        NU_TRANSACCION = :NumeroTransaccion, 
                        DT_UPDROW = :FechaModificacion ";

            if (incrementaStock)
                sql += ", QT_ESTOQUE = QT_ESTOQUE + :Cantidad , QT_RESERVA_SAIDA = QT_RESERVA_SAIDA + :Cantidad ";
            else
                sql += ", QT_ESTOQUE = QT_ESTOQUE - :Cantidad , QT_RESERVA_SAIDA = QT_RESERVA_SAIDA - :Cantidad ";

            sql += @" WHERE CD_ENDERECO = :Ubicacion
                        AND CD_PRODUTO = :Producto
                        AND CD_FAIXA = :Faixa
                        AND NU_IDENTIFICADOR = :Identificador 
                        AND CD_EMPRESA = :Empresa ";

            await _dapper.ExecuteAsync(connection, sql, stocks, transaction: tran);
        }

        public virtual async Task BulkInsertStock(DbConnection connection, DbTransaction tran, IEnumerable<object> stocks)
        {
            string sql = @"INSERT INTO T_STOCK 
                            (CD_ENDERECO,
                            CD_EMPRESA,
                            CD_PRODUTO,
                            CD_FAIXA,
                            NU_IDENTIFICADOR,
                            QT_ESTOQUE,
                            QT_RESERVA_SAIDA,
                            QT_TRANSITO_ENTRADA,
                            DT_FABRICACAO,
                            ID_AVERIA,
                            ID_INVENTARIO,
                            DT_INVENTARIO,
                            ID_CTRL_CALIDAD,
                            DT_UPDROW,
                            NU_TRANSACCION) 
                        VALUES 
                            (:Ubicacion,
                            :Empresa,
                            :Producto,
                            :Faixa,
                            :Identificador,
                            :Cantidad,
                            :ReservaSalida, 
                            :CantidadTransitoEntrada,
                            :Vencimiento,
                            :Averia, 
                            :Inventario,
                            :FechaInventario,
                            :ControlCalidad,
                            :FechaModificacion,
                            :NumeroTransaccion)";

            await _dapper.ExecuteAsync(connection, sql, stocks, transaction: tran);
        }

        public virtual async Task BulkInsertDetallePicking(DbConnection connection, DbTransaction tran, IEnumerable<object> picks)
        {
            string sql = @"INSERT INTO T_DET_PICKING 
                            (NU_PREPARACION,
                            CD_ENDERECO,
                            NU_PEDIDO,
                            CD_EMPRESA,
                            CD_CLIENTE,
                            CD_PRODUTO,
                            CD_FAIXA,
                            NU_IDENTIFICADOR,
                            NU_SEQ_PREPARACION,
                            ID_ESPECIFICA_IDENTIFICADOR,
                            NU_CARGA,
                            ID_AGRUPACION,
                            QT_PRODUTO,
                            QT_PREPARADO,
                            QT_PICKEO,
                            NU_CONTENEDOR,
                            CD_FUNCIONARIO,
                            DT_PICKEO,
                            CD_FUNC_PICKEO,
                            DT_ADDROW,
                            DT_UPDROW,
                            DT_FABRICACAO_PICKEO,
                            NU_TRANSACCION,
                            ND_ESTADO)
                        VALUES 
                            (:NumeroPreparacion,
                            :Ubicacion,
                            :Pedido,
                            :Empresa,
                            :Cliente,
                            :Producto,
                            :Faixa, 
                            :Lote,
                            :NumeroSecuencia,
                            :EspecificaLote, 
                            :Carga,
                            :Agrupacion,
                            :Cantidad,
                            :CantidadPreparada,
                            :CantidadPickeo,
                            :NroContenedor,
                            :Usuario,
                            :FechaPickeo,
                            :UsuarioPickeo,
                            :FechaAlta,
                            :FechaModificacion,
                            :VencimientoPickeo,
                            :Transaccion,
                            :Estado)";

            await _dapper.ExecuteAsync(connection, sql, picks, transaction: tran);
        }

        public virtual async Task BulkUpdateDetallePicking(DbConnection connection, DbTransaction tran, IEnumerable<object> picks)
        {
            string sql = @"UPDATE T_DET_PICKING SET 
                               DT_UPDROW = :FechaModificacion, 
                               QT_PRODUTO = :Cantidad,
                               NU_TRANSACCION = :Transaccion
                           WHERE NU_PREPARACION = :NumeroPreparacion 
                           AND CD_ENDERECO = :Ubicacion
                           AND NU_PEDIDO = :Pedido
                           AND CD_CLIENTE = :Cliente
                           AND CD_EMPRESA = :Empresa
                           AND CD_PRODUTO = :Producto
                           AND NU_IDENTIFICADOR = :Lote
                           AND CD_FAIXA = :Faixa
                           AND ND_ESTADO = :Estado ";

            await _dapper.ExecuteAsync(connection, sql, picks, transaction: tran);
        }

        public virtual async Task BulkUpdateRemoveDetallePikcing(DbConnection connection, DbTransaction tran, IEnumerable<object> picks)
        {
            string sql = @"UPDATE T_DET_PICKING SET 
                               DT_UPDROW = :FechaModificacion, 
                               NU_TRANSACCION = :Transaccion,
                               NU_TRANSACCION_DELETE = :TransaccionDelete
                           WHERE NU_PREPARACION = :NumeroPreparacion 
                           AND CD_ENDERECO = :Ubicacion
                           AND NU_PEDIDO = :Pedido
                           AND CD_CLIENTE = :Cliente
                           AND CD_EMPRESA = :Empresa
                           AND CD_PRODUTO = :Producto
                           AND NU_IDENTIFICADOR = :Lote
                           AND CD_FAIXA = :Faixa
                           AND ND_ESTADO = :Estado ";

            await _dapper.ExecuteAsync(connection, sql, picks, transaction: tran);
        }

        public virtual async Task BulkRemoveDetallePikcing(DbConnection connection, DbTransaction tran, IEnumerable<object> picks)
        {
            string sql = @"DELETE FROM T_DET_PICKING
                           WHERE NU_PREPARACION = :NumeroPreparacion 
                           AND CD_ENDERECO = :Ubicacion
                           AND NU_PEDIDO = :Pedido
                           AND CD_CLIENTE = :Cliente
                           AND CD_EMPRESA = :Empresa
                           AND CD_PRODUTO = :Producto
                           AND NU_IDENTIFICADOR = :Lote
                           AND CD_FAIXA = :Faixa
                           AND ND_ESTADO = :Estado";

            await _dapper.ExecuteAsync(connection, sql, picks, transaction: tran);
        }

        public virtual async Task BulkUpdateReservaDocumental(DbConnection connection, DbTransaction tran, IEnumerable<object> reservas)
        {
            string sql = @"UPDATE T_DOCUMENTO_PREPARACION_RESERV SET 
                               QT_PRODUTO = :CantidadProducto,
                               QT_PREPARADO = :CantidadPreparada,
                               DT_UPDROW = :FechaModificacion, 
                               VL_DATO_AUDITORIA = :Auditoria,
                               NU_TRANSACCION_DELETE = :NumeroTransaccionDelete,
                           WHERE NU_DOCUMENTO = :NroDocumento 
                           AND TP_DOCUMENTO = :TipoDocumento
                           AND NU_PREPARACION = :Preparacion
                           AND CD_EMPRESA = :Empresa
                           AND CD_PRODUTO = :Producto
                           AND CD_FAIXA = :Faixa
                           AND NU_IDENTIFICADOR_PICKING_DET = :NroIdentificadorPicking
                           AND NU_IDENTIFICADOR = :Identificador ";

            await _dapper.ExecuteAsync(connection, sql, reservas, transaction: tran);
        }

        public virtual async Task BulkRemoveReservaDocumental(DbConnection connection, DbTransaction tran, IEnumerable<object> reservas)
        {
            string sql = @"DELETE FROM T_DOCUMENTO_PREPARACION_RESERV SET 
                           WHERE NU_DOCUMENTO = :NroDocumento 
                           AND TP_DOCUMENTO = :TipoDocumento
                           AND NU_PREPARACION = :Preparacion
                           AND CD_EMPRESA = :Empresa
                           AND CD_PRODUTO = :Producto
                           AND CD_FAIXA = :Faixa
                           AND NU_IDENTIFICADOR_PICKING_DET = :NroIdentificadorPicking
                           AND NU_IDENTIFICADOR = :Identificador ";

            await _dapper.ExecuteAsync(connection, sql, reservas, transaction: tran);
        }

        public virtual async Task BulkInsertReservaDocumental(DbConnection connection, DbTransaction tran, IEnumerable<object> reservas)
        {
            string sql = @"INSERT INTO T_DOCUMENTO_PREPARACION_RESERV 
                            (NU_DOCUMENTO,
                            TP_DOCUMENTO,
                            NU_PREPARACION,
                            CD_EMPRESA,
                            CD_PRODUTO,
                            CD_FAIXA,
                            NU_IDENTIFICADOR,
                            ID_ESPECIFICA_IDENTIFICADOR,
                            NU_IDENTIFICADOR_PICKING_DET,
                            QT_PRODUTO,
                            QT_PREPARADO,
                            QT_ANULAR,
                            DT_ADDROW,
                            VL_DATO_AUDITORIA)
                        VALUES 
                            (:NroDocumento,
                            :TipoDocumento,
                            :Preparacion,
                            :Empresa,
                            :Producto,
                            :Faixa, 
                            :Identificador,
                            :EspecificaIdentificadorId,
                            :NroIdentificadorPicking, 
                            :CantidadProducto,
                            :CantidadPreparada,
                            :CantidadAnular,
                            :FechaAlta,
                            :Auditoria)";

            await _dapper.ExecuteAsync(connection, sql, reservas, transaction: tran);
        }

        public virtual async Task BulkUpdateEquipos(DbConnection connection, DbTransaction tran, IEnumerable<object> equipos)
        {
            string sql = @"UPDATE T_EQUIPO 
                           SET DT_UPDROW = :FechaModificado, 
                           TP_OPERATIVA = :TipoOperativa
                           WHERE CD_EQUIPO = :CodigoEquipo";

            await _dapper.ExecuteAsync(connection, sql, equipos, transaction: tran);
        }

        public virtual async Task<long> CreateTransaction(string descripcion, DbConnection connection, DbTransaction tran)
        {
            var transaccionRepository = new TransaccionRepository(_context, _application, _userId, _dapper);
            return await transaccionRepository.CreateTransaction(descripcion, connection, tran, _application, _userId);
        }

        public virtual DetallePreparacion GetNewDetallePickingObject(DetallePreparacion dp)
        {
            return new DetallePreparacion()
            {
                NumeroPreparacion = dp.NumeroPreparacion,
                Ubicacion = dp.Ubicacion,
                Pedido = dp.Pedido,
                Cliente = dp.Cliente,
                Empresa = dp.Empresa,
                Producto = dp.Producto,
                Lote = dp.Lote,
                Faixa = dp.Faixa,
                NumeroSecuencia = dp.NumeroSecuencia,
                EspecificaLote = dp.EspecificaLote,
                Agrupacion = dp.Agrupacion,
                Carga = dp.Carga,
                Cantidad = dp.Cantidad,
                CantidadPreparada = dp.CantidadPreparada,
                CantidadPickeo = dp.CantidadPickeo,
                NroContenedor = dp.NroContenedor,
                Usuario = dp.Usuario,
                FechaPickeo = dp.FechaPickeo,
                UsuarioPickeo = dp.UsuarioPickeo,
                FechaAlta = dp.FechaAlta,
                FechaModificacion = dp.FechaModificacion,
                VencimientoPickeo = dp.VencimientoPickeo,
                Estado = dp.Estado,
                Transaccion = dp.Transaccion,
                AreaAveria = dp.AreaAveria,
                AveriaPickeo = dp.AveriaPickeo,
                Cancelado = dp.Cancelado,
                CantidadControl = dp.CantidadControl,
                CantidadControlada = dp.CantidadControlada,
                ErrorControl = dp.ErrorControl,
                FechaSeparacion = dp.FechaSeparacion,
                IdDetallePickingLpn = dp.IdDetallePickingLpn,
                IdExternoContenedor = dp.IdExternoContenedor,
                NumeroContenedorPickeo = dp.NumeroContenedorPickeo,
                NumeroContenedorSys = dp.NumeroContenedorSys,
                Proveedor = dp.Proveedor,
                ReferenciaEstado = dp.ReferenciaEstado,
                TransaccionDelete = dp.TransaccionDelete,
            };
        }

        public virtual object GetNewDetallePickingEntity(DetallePreparacion dp)
        {
            return new
            {
                NumeroPreparacion = dp.NumeroPreparacion,
                Ubicacion = dp.Ubicacion,
                Pedido = dp.Pedido,
                Cliente = dp.Cliente,
                Empresa = dp.Empresa,
                Producto = dp.Producto,
                Lote = dp.Lote,
                Faixa = dp.Faixa,
                NumeroSecuencia = dp.NumeroSecuencia,
                EspecificaLote = dp.EspecificaLote,
                Agrupacion = dp.Agrupacion,
                Carga = dp.Carga,
                Cantidad = dp.Cantidad,
                CantidadPreparada = dp.Cantidad,
                CantidadPickeo = dp.Cantidad,
                NroContenedor = dp.NroContenedor,
                Usuario = dp.Usuario,
                FechaPickeo = dp.FechaPickeo,
                UsuarioPickeo = dp.UsuarioPickeo,
                FechaAlta = dp.FechaAlta,
                FechaModificacion = dp.FechaModificacion,
                VencimientoPickeo = dp.VencimientoPickeo,
                Estado = dp.Estado,
                Transaccion = dp.Transaccion,
            };
        }

        public virtual object GetUpdateDetallePickingEntity(DetallePreparacion dp)
        {
            return new
            {
                NumeroPreparacion = dp.NumeroPreparacion,
                Ubicacion = dp.Ubicacion,
                Pedido = dp.Pedido,
                Cliente = dp.Cliente,
                Empresa = dp.Empresa,
                Producto = dp.Producto,
                Lote = dp.Lote,
                Faixa = dp.Faixa,
                Estado = dp.Estado,
                Cantidad = dp.Cantidad,
                Transaccion = dp.Transaccion,
                FechaModificacion = dp.FechaModificacion,
            };
        }

        public virtual object GetRemoveDetallePickingEntity(DetallePreparacion dp)
        {
            return new
            {
                NumeroPreparacion = dp.NumeroPreparacion,
                Ubicacion = dp.Ubicacion,
                Pedido = dp.Pedido,
                Cliente = dp.Cliente,
                Empresa = dp.Empresa,
                Producto = dp.Producto,
                Lote = dp.Lote,
                Faixa = dp.Faixa,
                Estado = dp.Estado,
                FechaModificacion = dp.FechaModificacion,
                Transaccion = dp.Transaccion,
                TransaccionDelete = dp.TransaccionDelete
            };
        }

        public virtual object GetNewContenedorEntity(DetallePreparacion dp)
        {
            return new
            {
                Numero = (int)dp.NroContenedor,
                NumeroPreparacion = dp.NumeroPreparacion,
                TipoContenedor = dp.Contenedor.TipoContenedor,
                IdExterno = dp.Contenedor.IdExterno,
                CodigoBarras = dp.Contenedor.CodigoBarras,
                Estado = SituacionDb.ContenedorEnPreparacion,
                Ubicacion = dp.Contenedor.Ubicacion,
                CodigoSubClase = dp.Contenedor.CodigoSubClase,
                Habilitado = "S",
                FechaAgregado = DateTime.Now,
                NumeroTransaccion = dp.Transaccion,
            };
        }

        public virtual object GetUpdateContenedorEntity(DetallePreparacion dp, int prepDestino)
        {
            return new
            {
                Numero = (int)dp.NroContenedor,
                NumeroPreparacion = prepDestino,
                FechaModificado = DateTime.Now,
                NumeroTransaccion = dp.Transaccion,
            };
        }

        public virtual Stock GetUpdateStockEntity(DetallePreparacion dp, string ubicacionDestino)
        {
            return new Stock
            {
                Ubicacion = ubicacionDestino,
                Empresa = dp.Empresa,
                Producto = dp.Producto,
                Faixa = dp.Faixa,
                Identificador = dp.Lote,
                Cantidad = dp.Cantidad,
                FechaModificacion = DateTime.Now,
                NumeroTransaccion = dp.Transaccion
            };
        }

        public virtual Stock GetNewStockEntity(DetallePreparacion dp, string ubicacion)
        {
            return new Stock
            {
                Ubicacion = ubicacion,
                Empresa = dp.Empresa,
                Producto = dp.Producto,
                Faixa = dp.Faixa,
                Identificador = dp.Lote,
                Cantidad = dp.Cantidad,
                ReservaSalida = dp.Cantidad,
                CantidadTransitoEntrada = 0,
                Vencimiento = (DateTime?)null,
                Averia = "N",
                Inventario = "R",
                FechaInventario = DateTime.Now,
                ControlCalidad = EstadoControlCalidad.Controlado,
                FechaModificacion = DateTime.Now,
                NumeroTransaccion = dp.Transaccion,
            };
        }

        public virtual object GetUpdateEquipoEntity(int cdEquipo)
        {
            return new
            {
                CodigoEquipo = cdEquipo,
                TipoOperativa = TipoOperacionDb.Picking,
                FechaModificado = DateTime.Now,
            };
        }

        public virtual List<int> GetNewContenedores(int count, DbConnection connection, DbTransaction tran)
        {
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_NU_CONTENEDOR, count, tran).ToList();
        }

        #endregion

        #region PreparacionAdministrativa

        public virtual void AddPickingLpnTemporal(List<LpnPicking> stocksExpulsar)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            _dapper.BulkInsert(connection, tran, stocksExpulsar, "T_LPN_TEMP", new Dictionary<string, Func<LpnPicking, ColumnInfo>>
            {
                { "NU_LPN", x => new ColumnInfo(x.Lpn)}
            });
        }

        public virtual List<LpnDetalle> GetDetallesLpnsPreparar()
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            string sql = @"SELECT 
                            l.CD_ENDERECO as Ubicacion,
                            ld.CD_EMPRESA as Empresa,
                            ld.CD_PRODUTO as CodigoProducto,
                            ld.CD_FAIXA as Faixa,
                            ld.NU_IDENTIFICADOR as Lote,
                            ld.ID_LPN_DET Id,
                            ld.QT_ESTOQUE Cantidad,
                            ld.DT_FABRICACAO Vencimiento,
                            l.TP_LPN_TIPO  Tipo,
                            l.ID_LPN_EXTERNO IdExterno,
                            l.NU_LPN NumeroLpn
                        FROM
                            T_LPN_TEMP tmp
                        INNER JOIN T_LPN_DET ld on ld.NU_LPN = tmp.NU_LPN 
                        INNER JOIN T_LPN l on l.NU_LPN = tmp.NU_LPN 
                        WHERE ld.QT_ESTOQUE > 0
                        ";

            return _dapper.Query<LpnDetalle>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<long> GetNewIdDetallePickingLpn(int count)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();
            return _dapper.GetNextSequenceValues<long>(connection, Secuencias.S_DET_PICKING_LPN, count, tran).ToList();
        }

        public virtual void PuedePrepararse(bool isEmpresaDocumental, List<StockPicking> stocksPicking, List<LpnDetalle> detallesLpns, string predio)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            AddStockTemp(stocksPicking, detallesLpns, connection, tran);

            var stock = AnySuficienteStockParaPreparar(connection, tran);
            if (stock != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_01", new string[] { stock.Producto, stock.Identificador, stock.Empresa.ToString(), stock.Ubicacion });
            }
            if (isEmpresaDocumental)
            {
                stock = AnySuficienteStockDocumental(connection, tran,predio);
                if (stock != null)
                {
                    throw new ValidationFailedException("PRE052_Sec0_Error_02", new string[] { stock.Producto, stock.Identificador, stock.Empresa.ToString() });
                }
            }

            var stockLpn = AnyReservaLpns(connection, tran);
            if (stock != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_03", new string[] { stockLpn.NumeroLPN.ToString() });
            }
        }

        public virtual Lpn AnyReservaLpns(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            st.NU_LPN as NumeroLPN
                        FROM
                            T_LPN_TEMP st
                        LEFT JOIN V_PRE052_STOCK_LPN s on s.NU_LPN = st.NU_LPN 
                        WHERE s.NU_LPN is null
                        ";

            return _dapper.Query<Lpn>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual Stock AnySuficienteStockParaPreparar(DbConnection connection, DbTransaction tran)
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
                        WHERE (st.QT_ESTOQUE > (s.QT_ESTOQUE - s.QT_RESERVA_SAIDA)) OR  s.CD_ENDERECO is null
                        ";

            return _dapper.Query<Stock>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual Stock AnySuficienteStockDocumental(DbConnection connection, DbTransaction tran,string predio)
        {
            string sql = @"SELECT 
                            st.CD_EMPRESA as Empresa,
                            st.CD_PRODUTO as Producto,
                            st.CD_FAIXA as Faixa,
                            st.NU_IDENTIFICADOR as Identificador
                        FROM
                            T_STOCK_TEMP st
                        LEFT JOIN (
                                    SELECT 
                                        SD.CD_EMPRESA,
                                        SD.CD_PRODUTO,
                                        SD.NU_IDENTIFICADOR,
                                        SD.CD_FAIXA,
                                        SUM(QT_INGRESADA - COALESCE(QT_RESERVADA,0) - COALESCE(QT_DESAFECTADA,0)) QT_DISPO          
                                    FROM T_DET_DOCUMENTO           SD,
                                        T_DOCUMENTO               DO,
                                        T_DOCUMENTO_TIPO          DP,
                                        T_DOCUMENTO_TIPO_ESTADO   DE
                                    WHERE DO.NU_DOCUMENTO = SD.NU_DOCUMENTO AND
                                        DO.TP_DOCUMENTO= SD.TP_DOCUMENTO AND
                                        DO.TP_DOCUMENTO = DP.TP_DOCUMENTO AND
                                        (DO.NU_PREDIO = :Predio OR DO.NU_PREDIO is null) AND
                                        DP.FL_HABILITADO = 'S' AND
                                        DP.FL_DISPONIBILIZA_STOCK = 'S' AND
                                        DO.TP_DOCUMENTO = DE.TP_DOCUMENTO AND
                                        DO.ID_ESTADO = DE.ID_ESTADO AND
                                        DE.FL_DISPONIBILIZA_STOCK = 'S' group by SD.CD_EMPRESA, SD.CD_PRODUTO, SD.NU_IDENTIFICADOR, SD.CD_FAIXA
                        ) s on s.CD_EMPRESA = st.CD_EMPRESA AND
                             s.CD_PRODUTO = st.CD_PRODUTO AND
                             s.CD_FAIXA = st.CD_FAIXA AND
                             s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR
                        WHERE st.QT_ESTOQUE > s.QT_DISPO OR s.QT_DISPO IS NULL
                        ";

            return _dapper.Query<Stock>(connection, sql, param: new { Predio = predio }, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual void AddStockTemp(List<StockPicking> stocksPicking, List<LpnDetalle> detallesLpns, DbConnection connection, DbTransaction tran)
        {
            var stocks = stocksPicking.ToList();
            foreach (var detalle in detallesLpns)
            {
                var stock = stocks.FirstOrDefault(x => x.Ubicacion == detalle.Ubicacion && x.Empresa == detalle.Empresa && x.Producto == detalle.CodigoProducto && x.Faixa == detalle.Faixa && x.Identificador == detalle.Lote);
                if (stock == null)
                {
                    stocks.Add(new StockPicking
                    {
                        Ubicacion = detalle.Ubicacion,
                        Empresa = detalle.Empresa,
                        Producto = detalle.CodigoProducto,
                        Faixa = detalle.Faixa,
                        Identificador = detalle.Lote,
                        Cantidad = detalle.Cantidad,
                    });
                }
                else
                {
                    stocks.Remove(stock);
                    stocks.Add(new StockPicking
                    {
                        Ubicacion = detalle.Ubicacion,
                        Empresa = detalle.Empresa,
                        Producto = detalle.CodigoProducto,
                        Faixa = detalle.Faixa,
                        Identificador = detalle.Lote,
                        Cantidad = stock.Cantidad + detalle.Cantidad
                    });
                }
            }

            string sql = @"INSERT INTO T_STOCK_TEMP (CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, NU_IDENTIFICADOR, CD_FAIXA,QT_ESTOQUE) 
                                   VALUES (:Ubicacion, :Empresa, :Producto, :Identificador, :Faixa,:Cantidad)";
            _dapper.Execute(connection, sql, stocks, transaction: tran);
        }

        public virtual void GenerarPedidoAndDetalles(IUnitOfWork uow, Pedido pedido, bool crearPedido, List<DetallePedido> detallesPedido, List<DetallePedidoLpn> detallesPedidoLpn, long nuTransaccion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            if (crearPedido)
            {
                uow.PedidoRepository.InsertPedidos(connection, tran, pedido);
            }
            uow.PedidoRepository.BulkInsertLineasTemp(connection, tran, detallesPedido);

            uow.PedidoRepository.BulkUpdateLineas(connection, tran, nuTransaccion);

            uow.PedidoRepository.InsertDetallePedidoInexistente(connection, tran, nuTransaccion);

            uow.PedidoRepository.BulkInsertDetallesLpn(connection, tran, detallesPedidoLpn);

        }

        public virtual List<DocumentoLinea> GetDetallesDocumentoCandidatosByDetallePedido(string predio )
        {

            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            string sql = @"SELECT 
                            d.NU_DOCUMENTO Documento,
                            d.TP_DOCUMENTO TipoDocumento,
                            st.CD_EMPRESA as Empresa,
                            st.CD_PRODUTO as Producto,
                            st.CD_FAIXA as Faixa,
                            st.NU_IDENTIFICADOR as Identificador,
                            d.QT_DISPO  as CantidadDisponible
                        FROM
                            T_STOCK_TEMP st
                        LEFT JOIN (
                                    SELECT 
                                        DO.NU_DOCUMENTO,
                                        DO.TP_DOCUMENTO,
                                        SD.CD_EMPRESA,
                                        SD.CD_PRODUTO,
                                        SD.NU_IDENTIFICADOR,
                                        SD.CD_FAIXA,
                                        SUM(QT_INGRESADA - COALESCE(QT_RESERVADA,0) - COALESCE(QT_DESAFECTADA,0)) QT_DISPO          
                                    FROM T_DET_DOCUMENTO           SD,
                                        T_DOCUMENTO               DO,
                                        T_DOCUMENTO_TIPO          DP,
                                        T_DOCUMENTO_TIPO_ESTADO   DE
                                    WHERE DO.NU_DOCUMENTO = SD.NU_DOCUMENTO AND
                                        DO.TP_DOCUMENTO= SD.TP_DOCUMENTO AND
                                        DO.TP_DOCUMENTO = DP.TP_DOCUMENTO AND
                                        (DO.NU_PREDIO = :Predio OR DO.NU_PREDIO is null) AND
                                        DP.FL_HABILITADO = 'S' AND
                                        DP.FL_DISPONIBILIZA_STOCK = 'S' AND
                                        DO.TP_DOCUMENTO = DE.TP_DOCUMENTO AND
                                        DO.ID_ESTADO = DE.ID_ESTADO AND
                                        DE.FL_DISPONIBILIZA_STOCK = 'S' 
                                    GROUP BY DO.NU_DOCUMENTO, 
                                        DO.TP_DOCUMENTO,
                                        SD.CD_EMPRESA,
                                        SD.CD_PRODUTO,
                                        SD.NU_IDENTIFICADOR, 
                                        SD.CD_FAIXA 
                        ) d on d.CD_EMPRESA = st.CD_EMPRESA AND
                             d.CD_PRODUTO = st.CD_PRODUTO AND
                             d.CD_FAIXA = st.CD_FAIXA AND
                             d.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR
                        WHERE d.QT_DISPO > 0
                        ";

            return _dapper.Query<DocumentoLinea>(connection, sql, param: new { Predio = predio }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual void GenerarPreparacionAndReserva(IUnitOfWork uow, bool crearPreparacionAndCarga, Pedido pedido, Preparacion preparacion, Carga carga, List<DocumentoPreparacionReserva> documentoReserva, List<DetallePreparacion> detallePreparacion, List<DetallePreparacionLpn> detallesLpnsPreparacion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            if (crearPreparacionAndCarga)
            {
                uow.PreparacionRepository.AddPreparacion(connection, tran, preparacion);
                uow.CargaRepository.AddCarga(connection, tran, carga);
            }

            if (documentoReserva.Count() > 0)
            {
                uow.DocumentoRepository.AddPreparacionReservaTemp(connection, tran, documentoReserva);
                uow.DocumentoRepository.UpdatePreparacionReserva(connection, tran);
                uow.DocumentoRepository.AddPreparacionReserva(connection, tran);
                uow.DocumentoRepository.UpdateDocumentoReserva(connection, tran);
            }

            uow.PreparacionRepository.AddPreparacionDetalleTemp(connection, tran, detallePreparacion);
            uow.PreparacionRepository.UpdatePreparacionDetalle(connection, tran, uow.GetTransactionNumber());
            uow.PreparacionRepository.AddPreparacionDetalleLpn(connection, tran, preparacion, uow.GetTransactionNumber());
            uow.PreparacionRepository.AddDetallesLpnsPreparacion(connection, tran, detallesLpnsPreparacion);
            uow.PreparacionRepository.UpdateDetallesLpnsReservaPreparacion(connection, tran, detallesLpnsPreparacion);
            uow.PreparacionRepository.UpdateStockReservaPreparacion(connection, tran, uow.GetTransactionNumber());
            uow.PedidoRepository.BulkUpdateLineasCantidadLiberada(connection, tran, uow.GetTransactionNumber());
            uow.PedidoRepository.BulkUpdateLineasCantidadLiberadaLpn(connection, tran, detallesLpnsPreparacion);
            uow.PedidoRepository.BulkUpdatePedidoUltimaPreparacion(connection, tran, new List<Pedido>() { pedido }, preparacion.Id, uow.GetTransactionNumber());
        }

        public virtual void UpdateDetallesLpnsReservaPreparacion(DbConnection connection, DbTransaction tran, List<DetallePreparacionLpn> detallesLpnsPreparacion)
        {
            _dapper.BulkUpdate(connection, tran, detallesLpnsPreparacion, "T_LPN_DET", new Dictionary<string, Func<DetallePreparacionLpn, ColumnInfo>>
            {
                { "QT_RESERVA_SAIDA", x => new ColumnInfo(x.CantidadReservada,OperacionDb.OperacionMas)},
                { "NU_TRANSACCION", x => new ColumnInfo(x.Transaccion)},
            }, new Dictionary<string, Func<DetallePreparacionLpn, ColumnInfo>>
            {
                { "ID_LPN_DET", x => new ColumnInfo(x.IdDetalleLpn)},
                { "NU_LPN", x => new ColumnInfo(x.NroLpn)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
            });
        }

        public virtual void UpdateStockReservaPreparacion(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var alias = "s";
            var from = $@"
                T_STOCK s
                INNER JOIN (
                    SELECT 
                        s.CD_ENDERECO,
                        s.CD_EMPRESA,
                        s.CD_PRODUTO,
                        s.CD_FAIXA,
                        s.NU_IDENTIFICADOR,
                        SUM(st.QT_ESTOQUE) QT_PRODUTO_TEMP
                    FROM  T_STOCK_TEMP  st
                    INNER JOIN T_STOCK s on 
                         s.CD_ENDERECO = st.CD_ENDERECO AND
                         s.CD_PRODUTO = st.CD_PRODUTO AND
                         s.CD_FAIXA = st.CD_FAIXA AND
                         s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR AND
                         s.CD_EMPRESA = st.CD_EMPRESA 
                    GROUP by   s.CD_ENDERECO,
                        s.CD_EMPRESA,
                        s.CD_PRODUTO,
                        s.CD_FAIXA,
                        s.NU_IDENTIFICADOR
                ) st ON 
                     st.CD_ENDERECO  = s.CD_ENDERECO AND
                     st.CD_PRODUTO  = s.CD_PRODUTO AND
                     st.CD_FAIXA  = s.CD_FAIXA AND
                     st.NU_IDENTIFICADOR  = s.NU_IDENTIFICADOR AND
                     st.CD_EMPRESA  = s.CD_EMPRESA ";

            var set = @"
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                QT_RESERVA_SAIDA = QT_RESERVA_SAIDA + QT_PRODUTO_TEMP";

            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void AddPreparacionDetalleTemp(DbConnection connection, DbTransaction tran, List<DetallePreparacion> detallePreparacion)
        {
            _dapper.BulkInsert(connection, tran, detallePreparacion, "T_DET_PICKING_TEMP", new Dictionary<string, Func<DetallePreparacion, ColumnInfo>>
            {
                { "NU_PREPARACION", x => new ColumnInfo(x.NumeroPreparacion)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
                { "NU_SEQ_PREPARACION", x => new ColumnInfo(x.NumeroSecuencia)},
                { "ND_ESTADO", x => new ColumnInfo(x.Estado)},
                { "DT_ADDROW", x => new ColumnInfo(x.FechaAlta)},
                { "NU_CARGA", x => new ColumnInfo(x.Carga)},
                { "QT_PRODUTO", x => new ColumnInfo(x.Cantidad)},
                { "ID_DET_PICKING_LPN", x => new ColumnInfo(x.IdDetallePickingLpn,DbType.Int64)},
            });
        }

        public virtual void AddPreparacion(DbConnection connection, DbTransaction tran, Preparacion preparacion)
        {
            string sql = @"INSERT INTO T_PICKING (NU_PREPARACION,  DS_PREPARACION,CD_FUNCIONARIO,TP_PREPARACION,CD_SITUACAO,
                            FL_MERCADERIA_AVERIADA,CD_ONDA,NU_PREDIO,CD_EMPRESA,QT_RECHAZOS,ID_AGRUPACION,
                            FL_PICK_VENCIDO,CD_CONTENEDOR_VALIDACION,DT_INICIO) 
                                   VALUES (:Id, :Descripcion, :Usuario,:Tipo, :Situacion,
                            :FlAceptaMercaderiaAveriada,:Onda,:Predio,:Empresa,:CantidadRechazo,:Agrupacion,
                            :FlPermitePickVencido,:CodigoContenedorValidado,:FechaInicio)";
            _dapper.Execute(connection, sql, preparacion, transaction: tran);
        }

        public virtual void AddDetallesLpnsPreparacion(DbConnection connection, DbTransaction tran, List<DetallePreparacionLpn> detallesLpnsPreparacion)
        {
            _dapper.BulkInsert(connection, tran, detallesLpnsPreparacion, "T_DET_PICKING_LPN", new Dictionary<string, Func<DetallePreparacionLpn, ColumnInfo>>
            {
                { "NU_PREPARACION", x => new ColumnInfo(x.NroPreparacion)},
                { "ID_DET_PICKING_LPN", x => new ColumnInfo(x.IdDetallePickingLpn)},
                { "ID_LPN_DET", x => new ColumnInfo(x.IdDetalleLpn)},
                { "TP_LPN_TIPO", x => new ColumnInfo(x.TipoLpn)},
                { "NU_LPN", x => new ColumnInfo(x.NroLpn)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
                { "QT_RESERVA", x => new ColumnInfo(x.CantidadReservada)},
                { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                { "DT_ADDROW", x => new ColumnInfo(x.FechaAlta)},
                { "NU_TRANSACCION", x => new ColumnInfo(x.Transaccion)},
                { "ID_LPN_EXTERNO", x => new ColumnInfo(x.IdExternoLpn)},
            });
        }

        public virtual void AddPreparacionDetalleLpn(DbConnection connection, DbTransaction tran, Preparacion preparacion, long nuTransaccion)
        {
            string sql = @"INSERT INTO T_DET_PICKING (NU_PREPARACION,CD_CLIENTE,CD_EMPRESA,NU_PEDIDO,
                            CD_ENDERECO,CD_PRODUTO,CD_FAIXA,NU_IDENTIFICADOR,NU_SEQ_PREPARACION,ND_ESTADO,
                            NU_CARGA,QT_PRODUTO,ID_DET_PICKING_LPN,ID_AGRUPACION,ID_ESPECIFICA_IDENTIFICADOR,DT_ADDROW) 
                            SELECT dpt.NU_PREPARACION,dpt.CD_CLIENTE,dpt.CD_EMPRESA,dpt.NU_PEDIDO,
                                  dpt.CD_ENDERECO,dpt.CD_PRODUTO,dpt.CD_FAIXA,dpt.NU_IDENTIFICADOR,dpt.NU_SEQ_PREPARACION,dpt.ND_ESTADO,
                                  dpt.NU_CARGA,dpt.QT_PRODUTO,dpt.ID_DET_PICKING_LPN ,:Agrupacion,:EspecificaIdentificador,:FechaAlta
                             FROM T_DET_PICKING_TEMP dpt
                             LEFT JOIN T_DET_PICKING dp on dp.NU_PREPARACION = dpt.NU_PREPARACION AND
                                  dp.CD_CLIENTE = dpt.CD_CLIENTE AND
                                  dp.CD_EMPRESA = dpt.CD_EMPRESA AND
                                  dp.NU_PEDIDO = dpt.NU_PEDIDO AND
                                  dp.CD_ENDERECO = dpt.CD_ENDERECO AND
                                  dp.CD_PRODUTO = dpt.CD_PRODUTO AND
                                  dp.CD_FAIXA = dpt.CD_FAIXA AND
                                  dp.NU_IDENTIFICADOR = dpt.NU_IDENTIFICADOR AND
                                  dp.ND_ESTADO = dpt.ND_ESTADO AND
                                  (dpt.ID_DET_PICKING_LPN is null OR dp.ID_DET_PICKING_LPN = dpt.ID_DET_PICKING_LPN )
                            WHERE dp.NU_PREPARACION is null";
            _dapper.Execute(connection, sql, param: new { FechaAlta = DateTime.Now, Transaccion = nuTransaccion, Agrupacion = Agrupacion.Pedido, EspecificaIdentificador = "S" }, transaction: tran);
        }

        public virtual void UpdatePreparacionDetalle(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var alias = "dp";
            var from = $@"
                 T_DET_PICKING dp
                INNER JOIN (
                    SELECT 
                        dp.NU_PREPARACION,
                        dp.CD_CLIENTE,
                        dp.CD_EMPRESA,
                        dp.NU_PEDIDO,
                        dp.CD_ENDERECO,
                        dp.CD_PRODUTO,
                        dp.CD_FAIXA,
                        dp.NU_IDENTIFICADOR,
                        dp.NU_SEQ_PREPARACION,
                        SUM(dpt.QT_PRODUTO) QT_PRODUTO_TEMP
                    FROM  T_DET_PICKING_TEMP  dpt
                    INNER JOIN T_DET_PICKING dp on 
                         dp.NU_PREPARACION = dpt.NU_PREPARACION AND
                         dp.CD_CLIENTE = dpt.CD_CLIENTE AND
                         dp.CD_EMPRESA = dpt.CD_EMPRESA AND
                         dp.NU_PEDIDO = dpt.NU_PEDIDO AND
                         dp.CD_ENDERECO = dpt.CD_ENDERECO AND
                         dp.CD_PRODUTO = dpt.CD_PRODUTO AND
                         dp.CD_FAIXA = dpt.CD_FAIXA AND
                         dp.NU_IDENTIFICADOR = dpt.NU_IDENTIFICADOR AND
                         dp.ND_ESTADO = dpt.ND_ESTADO AND 
                         (dpt.ID_DET_PICKING_LPN is null OR dp.ID_DET_PICKING_LPN = dpt.ID_DET_PICKING_LPN)
                    GROUP by  dp.NU_PREPARACION ,
                        dp.CD_CLIENTE, 
                        dp.CD_EMPRESA, 
                        dp.NU_PEDIDO,
                        dp.CD_ENDERECO, 
                        dp.CD_PRODUTO, 
                        dp.CD_FAIXA,
                        dp.NU_IDENTIFICADOR,
                        dp.NU_SEQ_PREPARACION
                ) dpt ON  
                    dpt.NU_PREPARACION  = dp.NU_PREPARACION AND
                    dpt.CD_CLIENTE  = dp.CD_CLIENTE AND
                     dpt.CD_EMPRESA  = dp.CD_EMPRESA AND
                     dpt.NU_PEDIDO  = dp.NU_PEDIDO AND
                     dpt.CD_ENDERECO  = dp.CD_ENDERECO AND
                     dpt.CD_PRODUTO  = dp.CD_PRODUTO AND
                     dpt.CD_FAIXA  = dp.CD_FAIXA AND
                     dpt.NU_IDENTIFICADOR  = dp.NU_IDENTIFICADOR AND
                     dpt.NU_SEQ_PREPARACION = dp.NU_SEQ_PREPARACION";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                QT_PRODUTO = QT_PRODUTO + QT_PRODUTO_TEMP";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #endregion

        #region Get

        public virtual IEnumerable<AnulacionPreparacion> GetAnulacionPreparacionPendiente(IEnumerable<AnulacionPreparacion> preparaciones)
        {
            IEnumerable<AnulacionPreparacion> resultado = new List<AnulacionPreparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_TEMP (NU_PREPARACION) VALUES (:Preparacion)";
                    _dapper.Execute(connection, sql, preparaciones, transaction: tran);

                    sql = GetSqlSelectAnulacionPreparacion() +
                        @" INNER JOIN T_PICKING_TEMP T ON P.NU_PREPARACION = T.NU_PREPARACION";

                    resultado = _dapper.Query<AnulacionPreparacion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<DetallePreparacion> GetDetallesPreparacionPendientesAutomatismo(int preparacion)
        {
            IEnumerable<DetallePreparacion> resultado = new List<DetallePreparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                var sql = @" 
                        SELECT 
                            DP.NU_PREPARACION AS NumeroPreparacion,
                            DP.NU_PEDIDO AS Pedido,
                            DP.CD_EMPRESA AS Empresa,
                            C.TP_AGENTE AS TipoAgente, 
                            C.CD_AGENTE AS CodigoAgente,
                            DP.CD_CLIENTE AS Cliente,
                            DP.ND_ESTADO AS Estado
                        FROM T_DET_PICKING DP
                        INNER JOIN T_CLIENTE C ON DP.CD_CLIENTE = C.CD_CLIENTE
                        WHERE DP.NU_PREPARACION = :NU_PREPARACION
                            AND DP.ND_ESTADO = :ND_ESTADO
                        GROUP BY
                            DP.NU_PREPARACION,
                            DP.NU_PEDIDO,
                            DP.CD_EMPRESA,
                            C.TP_AGENTE,  
                            C.CD_AGENTE,
                            DP.CD_CLIENTE,
                            DP.ND_ESTADO";

                resultado = _dapper.Query<DetallePreparacion>(connection, sql, new
                {
                    NU_PREPARACION = preparacion,
                    ND_ESTADO = EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO,
                });
            }

            return resultado;
        }

        public virtual IEnumerable<DetallePreparacion> GetDetallesPreparacionPedidoPendientesAutomatismo(int preparacion, string pedido, string tipoAgente, string codigoAgente, string comparteContenedorPicking)
        {
            IEnumerable<DetallePreparacion> resultado = new List<DetallePreparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                var sql = @" 
                    SELECT 
                            DP.NU_PREPARACION AS NumeroPreparacion,
                            DP.CD_PRODUTO AS Producto,
                            DP.CD_FAIXA AS Faixa,
                            DP.NU_IDENTIFICADOR AS Lote,
                            DP.CD_EMPRESA AS Empresa,
                            DP.CD_ENDERECO AS Ubicacion,
                            DP.NU_PEDIDO  AS Pedido,
                            DP.CD_CLIENTE AS Cliente,
                            DP.NU_SEQ_PREPARACION AS NumeroSecuencia,
                            DP.ID_ESPECIFICA_IDENTIFICADOR AS EspecificaLote,
                            DP.NU_CARGA AS Carga,
                            DP.ID_AGRUPACION AS Agrupacion,
                            DP.QT_PRODUTO AS Cantidad,
                            DP.QT_PREPARADO AS CantidadPreparada,
                            DP.NU_CONTENEDOR AS NroContenedor,
                            DP.CD_FUNCIONARIO AS Usuario,
                            DP.NU_CONTENEDOR_SYS AS NumeroContenedorSys,
                            DP.QT_PICKEO AS CantidadPickeo,
                            DP.DT_PICKEO AS FechaPickeo,
                            DP.NU_CONTENEDOR_PICKEO AS NumeroContenedorPickeo,
                            DP.CD_FUNC_PICKEO AS UsuarioPickeo,
                            DP.DT_ADDROW AS FechaAlta,
                            DP.DT_UPDROW AS FechaModificacion,
                            DP.DT_FABRICACAO_PICKEO AS VencimientoPickeo,
                            DP.ID_AVERIA_PICKEO AS AveriaPickeo,
                            DP.CD_FORNECEDOR AS Proveedor,
                            DP.ID_AREA_AVERIA AS AreaAveria,
                            DP.FL_CANCELADO AS CanceladoId,
                            DP.QT_CONTROLADO AS CantidadControlada,
                            DP.QT_CONTROL AS CantidadControl,
                            DP.FL_ERROR_CONTROL AS ErrorControl,
                            DP.NU_TRANSACCION AS Transaccion,
                            DP.ND_ESTADO AS Estado,
                            DP.VL_ESTADO_REFERENCIA AS ReferenciaEstado,
                            DP.DT_SEPARACION AS FechaSeparacion,
                            DP.NU_TRANSACCION_DELETE AS TransaccionDelete,
                            DP.ID_DET_PICKING_LPN AS IdDetallePickingLpn,
                            P.VL_COMPARTE_CONTENEDOR_PICKING AS ComparteContenedorPicking
                    FROM T_DET_PICKING DP
                    INNER JOIN T_CLIENTE C ON DP.CD_CLIENTE = C.CD_CLIENTE
                    INNER JOIN T_PEDIDO_SAIDA P ON P.NU_PEDIDO = DP.NU_PEDIDO
                        AND P.CD_EMPRESA = DP.CD_EMPRESA
                        AND P.CD_CLIENTE = DP.CD_CLIENTE
                    WHERE DP.NU_PREPARACION = :NU_PREPARACION
                        AND DP.NU_PEDIDO = :NU_PEDIDO
                        AND C.TP_AGENTE = :TP_AGENTE
                        AND C.CD_AGENTE = :CD_AGENTE
                        AND COALESCE(P.VL_COMPARTE_CONTENEDOR_PICKING, '') = :VL_COMPARTE_CONTENEDOR_PICKING
                        AND DP.ND_ESTADO = :ND_ESTADO";

                resultado = _dapper.Query<DetallePreparacion>(connection, sql, new
                {
                    NU_PREPARACION = preparacion,
                    NU_PEDIDO = pedido,
                    TP_AGENTE = tipoAgente,
                    CD_AGENTE = codigoAgente,
                    VL_COMPARTE_CONTENEDOR_PICKING = comparteContenedorPicking ?? "",
                    ND_ESTADO = EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO,
                });
            }

            return resultado;
        }

        public virtual IEnumerable<DetallePreparacion> GetDetallesPreparacionClientePendientesAutomatismo(int preparacion, string tipoAgente, string codigoAgente, string comparteContenedorPicking)
        {
            IEnumerable<DetallePreparacion> resultado = new List<DetallePreparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                var sql = @" 
                    SELECT 
                            DP.NU_PREPARACION AS NumeroPreparacion,
                            DP.CD_PRODUTO AS Producto,
                            DP.CD_FAIXA AS Faixa,
                            DP.NU_IDENTIFICADOR AS Lote,
                            DP.CD_EMPRESA AS Empresa,
                            DP.CD_ENDERECO AS Ubicacion,
                            DP.NU_PEDIDO  AS Pedido,
                            DP.CD_CLIENTE AS Cliente,
                            DP.NU_SEQ_PREPARACION AS NumeroSecuencia,
                            DP.ID_ESPECIFICA_IDENTIFICADOR AS EspecificaLote,
                            DP.NU_CARGA AS Carga,
                            DP.ID_AGRUPACION AS Agrupacion,
                            DP.QT_PRODUTO AS Cantidad,
                            DP.QT_PREPARADO AS CantidadPreparada,
                            DP.NU_CONTENEDOR AS NroContenedor,
                            DP.CD_FUNCIONARIO AS Usuario,
                            DP.NU_CONTENEDOR_SYS AS NumeroContenedorSys,
                            DP.QT_PICKEO AS CantidadPickeo,
                            DP.DT_PICKEO AS FechaPickeo,
                            DP.NU_CONTENEDOR_PICKEO AS NumeroContenedorPickeo,
                            DP.CD_FUNC_PICKEO AS UsuarioPickeo,
                            DP.DT_ADDROW AS FechaAlta,
                            DP.DT_UPDROW AS FechaModificacion,
                            DP.DT_FABRICACAO_PICKEO AS VencimientoPickeo,
                            DP.ID_AVERIA_PICKEO AS AveriaPickeo,
                            DP.CD_FORNECEDOR AS Proveedor,
                            DP.ID_AREA_AVERIA AS AreaAveria,
                            DP.FL_CANCELADO AS CanceladoId,
                            DP.QT_CONTROLADO AS CantidadControlada,
                            DP.QT_CONTROL AS CantidadControl,
                            DP.FL_ERROR_CONTROL AS ErrorControl,
                            DP.NU_TRANSACCION AS Transaccion,
                            DP.ND_ESTADO AS Estado,
                            DP.VL_ESTADO_REFERENCIA AS ReferenciaEstado,
                            DP.DT_SEPARACION AS FechaSeparacion,
                            DP.NU_TRANSACCION_DELETE AS TransaccionDelete,
                            DP.ID_DET_PICKING_LPN AS IdDetallePickingLpn,
                            P.VL_COMPARTE_CONTENEDOR_PICKING AS ComparteContenedorPicking
                    FROM T_DET_PICKING DP
                    INNER JOIN T_CLIENTE C ON DP.CD_CLIENTE = C.CD_CLIENTE
                    INNER JOIN T_PEDIDO_SAIDA P ON P.NU_PEDIDO = DP.NU_PEDIDO
                        AND P.CD_EMPRESA = DP.CD_EMPRESA
                        AND P.CD_CLIENTE = DP.CD_CLIENTE
                    WHERE DP.NU_PREPARACION = :NU_PREPARACION
                        AND C.TP_AGENTE = :TP_AGENTE
                        AND C.CD_AGENTE = :CD_AGENTE
                        AND COALESCE(P.VL_COMPARTE_CONTENEDOR_PICKING, '') = :VL_COMPARTE_CONTENEDOR_PICKING
                        AND DP.ND_ESTADO = :ND_ESTADO";

                resultado = _dapper.Query<DetallePreparacion>(connection, sql, new
                {
                    NU_PREPARACION = preparacion,
                    TP_AGENTE = tipoAgente,
                    CD_AGENTE = codigoAgente,
                    VL_COMPARTE_CONTENEDOR_PICKING = comparteContenedorPicking ?? "",
                    ND_ESTADO = EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO,
                });
            }

            return resultado;
        }

        public virtual IEnumerable<DetallePreparacion> GetDetallesPreparacionRutaPendientesAutomatismo(int preparacion, long? carga, string comparteContenedorPicking)
        {
            IEnumerable<DetallePreparacion> resultado = new List<DetallePreparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                var sql = @" 
                    SELECT 
                            DP.NU_PREPARACION AS NumeroPreparacion,
                            DP.CD_PRODUTO AS Producto,
                            DP.CD_FAIXA AS Faixa,
                            DP.NU_IDENTIFICADOR AS Lote,
                            DP.CD_EMPRESA AS Empresa,
                            DP.CD_ENDERECO AS Ubicacion,
                            DP.NU_PEDIDO  AS Pedido,
                            DP.CD_CLIENTE AS Cliente,
                            DP.NU_SEQ_PREPARACION AS NumeroSecuencia,
                            DP.ID_ESPECIFICA_IDENTIFICADOR AS EspecificaLote,
                            DP.NU_CARGA AS Carga,
                            DP.ID_AGRUPACION AS Agrupacion,
                            DP.QT_PRODUTO AS Cantidad,
                            DP.QT_PREPARADO AS CantidadPreparada,
                            DP.NU_CONTENEDOR AS NroContenedor,
                            DP.CD_FUNCIONARIO AS Usuario,
                            DP.NU_CONTENEDOR_SYS AS NumeroContenedorSys,
                            DP.QT_PICKEO AS CantidadPickeo,
                            DP.DT_PICKEO AS FechaPickeo,
                            DP.NU_CONTENEDOR_PICKEO AS NumeroContenedorPickeo,
                            DP.CD_FUNC_PICKEO AS UsuarioPickeo,
                            DP.DT_ADDROW AS FechaAlta,
                            DP.DT_UPDROW AS FechaModificacion,
                            DP.DT_FABRICACAO_PICKEO AS VencimientoPickeo,
                            DP.ID_AVERIA_PICKEO AS AveriaPickeo,
                            DP.CD_FORNECEDOR AS Proveedor,
                            DP.ID_AREA_AVERIA AS AreaAveria,
                            DP.FL_CANCELADO AS CanceladoId,
                            DP.QT_CONTROLADO AS CantidadControlada,
                            DP.QT_CONTROL AS CantidadControl,
                            DP.FL_ERROR_CONTROL AS ErrorControl,
                            DP.NU_TRANSACCION AS Transaccion,
                            DP.ND_ESTADO AS Estado,
                            DP.VL_ESTADO_REFERENCIA AS ReferenciaEstado,
                            DP.DT_SEPARACION AS FechaSeparacion,
                            DP.NU_TRANSACCION_DELETE AS TransaccionDelete,
                            DP.ID_DET_PICKING_LPN AS IdDetallePickingLpn,
                            P.VL_COMPARTE_CONTENEDOR_PICKING AS ComparteContenedorPicking
                    FROM T_DET_PICKING DP
                    INNER JOIN T_CLIENTE C ON DP.CD_CLIENTE = C.CD_CLIENTE
                    INNER JOIN T_PEDIDO_SAIDA P ON P.NU_PEDIDO = DP.NU_PEDIDO
                        AND P.CD_EMPRESA = DP.CD_EMPRESA
                        AND P.CD_CLIENTE = DP.CD_CLIENTE
                    WHERE DP.NU_PREPARACION = :NU_PREPARACION
                        AND DP.NU_CARGA = :NU_CARGA
                        AND COALESCE(P.VL_COMPARTE_CONTENEDOR_PICKING, '') = :VL_COMPARTE_CONTENEDOR_PICKING
                        AND DP.ND_ESTADO = :ND_ESTADO";

                resultado = _dapper.Query<DetallePreparacion>(connection, sql, new
                {
                    NU_PREPARACION = preparacion,
                    NU_CARGA = carga,
                    VL_COMPARTE_CONTENEDOR_PICKING = comparteContenedorPicking ?? "",
                    ND_ESTADO = EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO,
                });
            }

            return resultado;
        }

        public virtual IEnumerable<DetallePreparacion> GetDetallesPreparacionOndaPendientesAutomatismo(int preparacion, string comparteContenedorPicking)
        {
            IEnumerable<DetallePreparacion> resultado = new List<DetallePreparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                var sql = @" 
                    SELECT 
                            DP.NU_PREPARACION AS NumeroPreparacion,
                            DP.CD_PRODUTO AS Producto,
                            DP.CD_FAIXA AS Faixa,
                            DP.NU_IDENTIFICADOR AS Lote,
                            DP.CD_EMPRESA AS Empresa,
                            DP.CD_ENDERECO AS Ubicacion,
                            DP.NU_PEDIDO  AS Pedido,
                            DP.CD_CLIENTE AS Cliente,
                            DP.NU_SEQ_PREPARACION AS NumeroSecuencia,
                            DP.ID_ESPECIFICA_IDENTIFICADOR AS EspecificaLote,
                            DP.NU_CARGA AS Carga,
                            DP.ID_AGRUPACION AS Agrupacion,
                            DP.QT_PRODUTO AS Cantidad,
                            DP.QT_PREPARADO AS CantidadPreparada,
                            DP.NU_CONTENEDOR AS NroContenedor,
                            DP.CD_FUNCIONARIO AS Usuario,
                            DP.NU_CONTENEDOR_SYS AS NumeroContenedorSys,
                            DP.QT_PICKEO AS CantidadPickeo,
                            DP.DT_PICKEO AS FechaPickeo,
                            DP.NU_CONTENEDOR_PICKEO AS NumeroContenedorPickeo,
                            DP.CD_FUNC_PICKEO AS UsuarioPickeo,
                            DP.DT_ADDROW AS FechaAlta,
                            DP.DT_UPDROW AS FechaModificacion,
                            DP.DT_FABRICACAO_PICKEO AS VencimientoPickeo,
                            DP.ID_AVERIA_PICKEO AS AveriaPickeo,
                            DP.CD_FORNECEDOR AS Proveedor,
                            DP.ID_AREA_AVERIA AS AreaAveria,
                            DP.FL_CANCELADO AS CanceladoId,
                            DP.QT_CONTROLADO AS CantidadControlada,
                            DP.QT_CONTROL AS CantidadControl,
                            DP.FL_ERROR_CONTROL AS ErrorControl,
                            DP.NU_TRANSACCION AS Transaccion,
                            DP.ND_ESTADO AS Estado,
                            DP.VL_ESTADO_REFERENCIA AS ReferenciaEstado,
                            DP.DT_SEPARACION AS FechaSeparacion,
                            DP.NU_TRANSACCION_DELETE AS TransaccionDelete,
                            DP.ID_DET_PICKING_LPN AS IdDetallePickingLpn,
                            P.VL_COMPARTE_CONTENEDOR_PICKING AS ComparteContenedorPicking
                    FROM T_DET_PICKING DP
                    INNER JOIN T_CLIENTE C ON DP.CD_CLIENTE = C.CD_CLIENTE
                    INNER JOIN T_PEDIDO_SAIDA P ON P.NU_PEDIDO = DP.NU_PEDIDO
                        AND P.CD_EMPRESA = DP.CD_EMPRESA
                        AND P.CD_CLIENTE = DP.CD_CLIENTE
                    WHERE DP.NU_PREPARACION = :NU_PREPARACION
                        AND COALESCE(P.VL_COMPARTE_CONTENEDOR_PICKING, '') = :VL_COMPARTE_CONTENEDOR_PICKING
                        AND DP.ND_ESTADO = :ND_ESTADO";

                resultado = _dapper.Query<DetallePreparacion>(connection, sql, new
                {
                    NU_PREPARACION = preparacion,
                    VL_COMPARTE_CONTENEDOR_PICKING = comparteContenedorPicking ?? "",
                    ND_ESTADO = EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO,
                });
            }

            return resultado;
        }

        public virtual IEnumerable<DetallePreparacion> GetDetallesPreparacionPedidos(IEnumerable<DetallePreparacion> pedidos)
        {
            IEnumerable<DetallePreparacion> resultado = new List<DetallePreparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_DET_PICKING_TEMP (NU_PREPARACION,NU_PEDIDO, CD_EMPRESA, CD_CLIENTE) VALUES (:NumeroPreparacion,:Pedido, :Empresa, :Cliente)";
                    _dapper.Execute(connection, sql, pedidos, transaction: tran);

                    sql = GetSqlSelectDetallePreparacion() +
                        @" INNER JOIN T_DET_PICKING_TEMP T ON P.NU_PREPARACION = T.NU_PREPARACION AND P.NU_PEDIDO = T.NU_PEDIDO AND  P.CD_EMPRESA = T.CD_EMPRESA AND  P.CD_CLIENTE = T.CD_CLIENTE ";

                    resultado = _dapper.Query<DetallePreparacion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectDetallePreparacion()
        {
            return @"
                SELECT 
                    P.NU_PREPARACION AS NumeroPreparacion,
                    P.CD_PRODUTO AS Producto,
                    P.CD_FAIXA AS Faixa,
                    P.NU_IDENTIFICADOR AS Lote,
                    P.CD_EMPRESA AS Empresa,
                    P.CD_ENDERECO AS Ubicacion,
                    P.NU_PEDIDO  AS Pedido,
                    P.CD_CLIENTE AS Cliente,
                    P.NU_SEQ_PREPARACION AS NumeroSecuencia,
                    P.ID_ESPECIFICA_IDENTIFICADOR AS EspecificaLote,
                    P.NU_CARGA AS Carga,
                    P.ID_AGRUPACION AS Agrupacion,
                    P.QT_PRODUTO AS Cantidad,
                    P.QT_PREPARADO AS CantidadPreparada,
                    P.NU_CONTENEDOR AS NroContenedor,
                    P.CD_FUNCIONARIO AS Usuario,
                    P.NU_CONTENEDOR_SYS AS NumeroContenedorSys,
                    P.QT_PICKEO AS CantidadPickeo,
                    P.DT_PICKEO AS FechaPickeo,
                    P.NU_CONTENEDOR_PICKEO AS NumeroContenedorPickeo,
                    P.CD_FUNC_PICKEO AS UsuarioPickeo,
                    P.DT_ADDROW AS FechaAlta,
                    P.DT_UPDROW AS FechaModificacion,
                    P.DT_FABRICACAO_PICKEO AS VencimientoPickeo,
                    P.ID_AVERIA_PICKEO AS AveriaPickeo,
                    P.CD_FORNECEDOR AS Proveedor,
                    P.ID_AREA_AVERIA AS AreaAveria,
                    P.FL_CANCELADO AS CanceladoId,
                    P.QT_CONTROLADO AS CantidadControlada,
                    P.QT_CONTROL AS CantidadControl,
                    P.FL_ERROR_CONTROL AS ErrorControl,
                    P.NU_TRANSACCION AS Transaccion,
                    P.ND_ESTADO AS Estado,
                    P.VL_ESTADO_REFERENCIA AS ReferenciaEstado,
                    P.DT_SEPARACION AS FechaSeparacion,
                    P.NU_TRANSACCION_DELETE AS TransaccionDelete,
                    P.ID_DET_PICKING_LPN AS IdDetallePickingLpn
                FROM T_DET_PICKING P ";
        }

        public static string GetSqlSelectAnulacionPreparacion()
        {
            return @"
                SELECT 
                    P.NU_ANULACION_PREPARACION AS NroAnulacionPreparacion,
                    P.NU_PREPARACION AS Preparacion,
                    P.ND_ESTADO AS Estado,
                    P.DS_ANULACION AS Descripcion,
                    P.DT_ADDROW AS Alta,
                    P.DT_UPDROW AS Modificacion,
                    P.TP_ANULACION AS TipoAnulacion,
                    P.TP_AGRUPACION AS TipoAgrupacion,
                    P.USERID AS UserId
                FROM T_ANULACION_PREPARACION P ";
        }

        public virtual IEnumerable<Preparacion> GetPreparaciones(IEnumerable<Preparacion> preparaciones)
        {
            IEnumerable<Preparacion> resultado = new List<Preparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_TEMP (NU_PREPARACION) VALUES (:Id)";
                    _dapper.Execute(connection, sql, preparaciones, transaction: tran);

                    sql = GetSqlPreparacion() +
                        @" INNER JOIN T_PICKING_TEMP T ON P.NU_PREPARACION = T.NU_PREPARACION";

                    resultado = _dapper.Query<Preparacion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlPreparacion()
        {
            return @"SELECT
	                    P.NU_PREPARACION as Id,	                    
	                    P.CD_EMPRESA as Empresa,
	                    P.TP_PREPARACION as Tipo,
	                    P.CD_SITUACAO as Situacion,
	                    P.NU_PREDIO as Predio,
                        P.ID_AGRUPACION as Agrupacion,
                        P.CD_ONDA as Onda,
                        P.CD_CONTENEDOR_VALIDACION as CodigoContenedorValidado,
                        P.CD_DESTINO as CodigoDestino,
                        P.CD_FUNCIONARIO as Usuario,
                        P.CD_FUNCIONARIO_ASIGNADO as UsuarioAsignado,
                        P.DS_PREPARACION as Descripcion,
                        P.DT_FIN as FechaFin,
                        P.DT_INICIO as FechaInicio,
                        P.FL_CONTROLA_STOCK_DOCUMENTO as ControlaStockDocumental,
                        P.FL_EXCLUIR_UBICACIONES_PICKING as ExcluirUbicacionesPicking,
                        P.FL_LIBERAR_POR_CURVAS as DebeLiberarPorCurvas,
                        P.FL_LIBERAR_POR_UNIDADES as DebeLiberarPorUnidades,
                        P.FL_MERCADERIA_AVERIADA as AceptaMercaderiaAveriada,
                        P.FL_MODAL_PALLET_COMPLETO as ModalPalletCompleto,
                        P.FL_MODAL_PALLET_INCOMPLETO as ModalPalletIncompleto,
                        P.FL_PICK_AGRUPADO_POR_CAMION as PickingEsAgrupadoPorCamion,
                        P.FL_PICK_VENCIDO as PermitePickVencido,
                        P.FL_PREPARAR_SOLO_CON_CAMION as PrepararSoloConCamion,
                        P.FL_PRIORIZAR_DESBORDE as PriozarDesborde,
                        P.FL_PRIORIZAR_LOTE_PICK as PriorizarLotePick,
                        P.FL_REQUIERE_UBICACION as RequiereUbicacion,
                        P.FL_RESPETAR_FIFO_EN_LOTE_AUTO as RespetarFifoEnLoteAUTO,
                        P.FL_SIMULACRO as Simulacro,
                        P.FL_USAR_SOLO_STK_PICKING as UsarSoloStkPicking,
                        P.FL_VALIDAR_PRODUCTO_PROVEEDOR as ValidarProductoProveedor,
                        P.FL_VENTANA_POR_CLIENTE as ManejaVidaUtil,
                        P.ID_AVISO as IdAviso,
                        P.NU_DOCUMENTO as NumeroDocumemto,
                        P.NU_TRANSACCION as Transaccion,
                        P.NU_TRANSACCION_DELETE as TransaccionDelete,
                        P.QT_RECHAZOS as CantidadRechazo,
                        P.TP_DOCUMENTO as TipoDocumento,
                        P.VL_CURSOR_PEDIDO as CursorPedido,
                        P.VL_CURSOR_STOCK as CursorStock,
                        P.VL_PORCENTAJE_REPARTO_COMUN as PorcentajeRepartoComun,
                        P.VL_PORCENTAJE_VENTANA as ValorVidaUtil,
                        P.VL_REPARTIR_ESCASEZ as RepartirEscasez
                    FROM T_PICKING P ";
        }

        public virtual IEnumerable<DetallePreparacion> GetDetallesPendientes(IEnumerable<DetallePreparacion> detalles)
        {
            IEnumerable<DetallePreparacion> resultado = new List<DetallePreparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_DET_PICKING_TEMP (NU_PREPARACION, CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR) VALUES (:NumeroPreparacion, :Ubicacion, :Empresa, :Producto, :Faixa, :Lote)";
                    _dapper.Execute(connection, sql, detalles, transaction: tran);

                    sql = @"
                        SELECT 
                            P.NU_PREPARACION AS NumeroPreparacion,
                            P.CD_PRODUTO AS Producto,
                            P.CD_FAIXA AS Faixa,
                            P.NU_IDENTIFICADOR AS Lote,
                            P.CD_EMPRESA AS Empresa,
                            P.CD_ENDERECO AS Ubicacion,
                            P.NU_PEDIDO  AS Pedido,
                            P.CD_CLIENTE AS Cliente,
                            P.NU_SEQ_PREPARACION AS NumeroSecuencia,
                            P.ID_ESPECIFICA_IDENTIFICADOR AS EspecificaLote,
                            P.NU_CARGA AS Carga,
                            P.ID_AGRUPACION AS Agrupacion,
                            P.QT_PRODUTO AS Cantidad,
                            P.QT_PREPARADO AS CantidadPreparada,
                            P.NU_CONTENEDOR AS NroContenedor,
                            P.CD_FUNCIONARIO AS Usuario,
                            P.NU_CONTENEDOR_SYS AS NumeroContenedorSys,
                            P.QT_PICKEO AS CantidadPickeo,
                            P.DT_PICKEO AS FechaPickeo,
                            P.NU_CONTENEDOR_PICKEO AS NumeroContenedorPickeo,
                            P.CD_FUNC_PICKEO AS UsuarioPickeo,
                            P.DT_ADDROW AS FechaAlta,
                            P.DT_UPDROW AS FechaModificacion,
                            P.DT_FABRICACAO_PICKEO AS VencimientoPickeo,
                            P.ID_AVERIA_PICKEO AS AveriaPickeo,
                            P.CD_FORNECEDOR AS Proveedor,
                            P.ID_AREA_AVERIA AS AreaAveria,
                            P.FL_CANCELADO AS CanceladoId,
                            P.QT_CONTROLADO AS CantidadControlada,
                            P.QT_CONTROL AS CantidadControl,
                            P.FL_ERROR_CONTROL AS ErrorControl,
                            P.NU_TRANSACCION AS Transaccion,
                            P.ND_ESTADO AS Estado,
                            P.VL_ESTADO_REFERENCIA AS ReferenciaEstado,
                            P.DT_SEPARACION AS FechaSeparacion,
                            P.NU_TRANSACCION_DELETE AS TransaccionDelete,
                            P.ID_DET_PICKING_LPN AS IdDetallePickingLpn,
                            PS.VL_COMPARTE_CONTENEDOR_PICKING AS ComparteContenedorPicking
                        FROM T_DET_PICKING P 
                        INNER JOIN T_DET_PICKING_TEMP T ON P.NU_PREPARACION = T.NU_PREPARACION 
                            AND P.CD_ENDERECO = T.CD_ENDERECO                             
                            AND P.CD_EMPRESA = T.CD_EMPRESA 
                            AND P.CD_PRODUTO = T.CD_PRODUTO 
                            AND P.CD_FAIXA = T.CD_FAIXA 
                            AND P.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR 
                        INNER JOIN T_PEDIDO_SAIDA PS ON P.NU_PEDIDO = PS.NU_PEDIDO 
                            AND P.CD_EMPRESA = PS.CD_EMPRESA 
                            AND P.CD_CLIENTE = PS.CD_CLIENTE 
                        WHERE P.ND_ESTADO in ('ESTAD_PREP_PEND','ESTAD_PEND_AUT')";

                    resultado = _dapper.Query<DetallePreparacion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<DetallePreparacion> GetDetallesPreparacionDestino(IEnumerable<Contenedor> contenedores)
        {
            IEnumerable<DetallePreparacion> resultado = new List<DetallePreparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CONTENEDOR_TEMP (NU_PREPARACION, NU_CONTENEDOR) VALUES (:NumeroPreparacion, :Numero)";
                    _dapper.Execute(connection, sql, contenedores, transaction: tran);

                    sql = GetSqlSelectDetallePreparacion() +
                    @" INNER JOIN T_CONTENEDOR_TEMP T ON P.NU_PREPARACION = T.NU_PREPARACION AND P.NU_CONTENEDOR = T.NU_CONTENEDOR ";

                    resultado = _dapper.Query<DetallePreparacion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual Dictionary<int, Carga> GetPreparacionCarga(IEnumerable<Contenedor> contenedores)
        {
            var resultado = new Dictionary<int, Carga>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CONTENEDOR_TEMP (NU_CONTENEDOR, NU_PREPARACION) VALUES (:Numero, :NumeroPreparacion)";
                    _dapper.Execute(connection, sql, contenedores, transaction: tran);

                    sql = @"SELECT 
                                DP.NU_PREPARACION AS NumeroPreparacion,
                                MIN(DP.NU_CARGA) AS Carga
                            FROM T_DET_PICKING DP 
                            INNER JOIN T_CONTENEDOR_TEMP T ON DP.NU_CONTENEDOR = T.NU_CONTENEDOR AND DP.NU_PREPARACION = T.NU_PREPARACION
                            INNER JOIN T_CONTENEDOR C ON T.NU_CONTENEDOR = C.NU_CONTENEDOR AND T.NU_PREPARACION = C.NU_PREPARACION
                            WHERE C.CD_SITUACAO = 601
                            GROUP BY DP.NU_PREPARACION ";

                    resultado = _dapper.Query<DetallePreparacion>(connection, sql, transaction: tran)
                        .ToDictionary(d => d.NumeroPreparacion, d => new Carga() { Id = (d.Carga ?? 0) });

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual List<CargaAsociarUnidad> GetCargasCliente(DbConnection connection, DbTransaction tran)
        {
            string sql = @$"SELECT 
                            ct.NU_CARGA as Carga, 
                            dp.CD_EMPRESA as Empresa, 
                            dp.CD_CLIENTE as Cliente 
                        FROM T_CARGA_TEMP  ct
                        INNER JOIN T_DET_PICKING  dp ON  dp.NU_PREPARACION = ct.NU_PREPARACION
                            AND dp.CD_EMPRESA = ct.CD_EMPRESA
                            AND dp.NU_CARGA = ct.NU_CARGA
                        LEFT JOIN T_CLIENTE_CAMION cc on cc.NU_CARGA = dp.NU_CARGA 
                            AND cc.CD_CLIENTE = dp.CD_CLIENTE
                        WHERE cc.CD_CAMION is null
                        GROUP BY 
                            ct.NU_CARGA, 
                            dp.CD_EMPRESA, 
                            dp.CD_CLIENTE";

            return _dapper.Query<CargaAsociarUnidad>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<int> GetSecuenciasDetallePicking(int count, DbConnection connection, DbTransaction tran)
        {
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_DET_PICKING, count, tran).ToList();
        }

        public virtual List<long> GetNewIdDetallePickingLpn(int count, DbConnection connection, DbTransaction tran)
        {
            return _dapper.GetNextSequenceValues<long>(connection, "S_DET_PICKING_LPN", count, tran).ToList();
        }

        public virtual LiberacionOndaParametrosCalculados GetCalculosLiberacion(IEnumerable<LiberacionOndaPedido> keys)
        {
            var resultado = new LiberacionOndaParametrosCalculados();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    _dapper.BulkInsert(connection, tran, keys, "T_PEDIDO_SAIDA_TEMP", new Dictionary<string, Func<LiberacionOndaPedido, ColumnInfo>>
                    {
                        { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                        { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                        { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)}
                    });

                    var sql = @"SELECT
	                            QT_LINEAS as CantidadLineas,	                    
	                            QT_UNIDADES as Unidades,
	                            VL_PESO_TOTAL as Peso,
	                            VL_VOLUMEN_TOTAL as VolumenTotal,
	                            VL_VOLUMEN_FINAL as VolumenFinal
                            FROM V_PRE050_PEND_LIB_CALCULOS";

                    resultado = _dapper.Query<LiberacionOndaParametrosCalculados>(connection, sql, transaction: tran).FirstOrDefault();

                    resultado.FilasSeleccionadas = keys.Count();

                    tran.Rollback();
                }
            }

            return resultado;
        }
        #endregion

        #region Any

        public virtual bool TieneMasDeUnConjuntoDeAgrupacionContenedor(int preparacion, int numeroContenedor)
        {
            string sql = $@"
                SELECT 
                    COUNT(DISTINCT PICK.NU_PREPARACION || PICK.NU_CONTENEDOR || PED.CD_EMPRESA || PED.CD_CLIENTE || COALESCE(PED.VL_COMPARTE_CONTENEDOR_PICKING, '(SIN AGRUPACION)') || CLA.CD_SUB_CLASSE)
                FROM T_DET_PICKING PICK 
                INNER JOIN T_PEDIDO_SAIDA PED ON PED.NU_PEDIDO = PICK.NU_PEDIDO 
                    AND PED.CD_CLIENTE = PICK.CD_CLIENTE 
                    AND PED.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_PRODUTO PROD ON PROD.CD_PRODUTO = PICK.CD_PRODUTO 
                    AND PROD.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_CLASSE CLA ON CLA.CD_CLASSE = PROD.CD_CLASSE
                WHERE PICK.NU_PREPARACION = :preparacion 
                    AND PICK.NU_CONTENEDOR = :numeroContenedor
                GROUP BY 
                    PICK.NU_PREPARACION,
                    PICK.NU_CONTENEDOR,
                    PED.CD_EMPRESA,
                    PED.CD_CLIENTE,
                    PED.VL_COMPARTE_CONTENEDOR_PICKING,
                    CLA.CD_SUB_CLASSE";

            var query = _dapper.Query<int>(_context.Database.GetDbConnection(), sql, new
            {
                preparacion = preparacion,
                numeroContenedor = numeroContenedor
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.FirstOrDefault() > 1;
        }

        public virtual bool TieneAgrupaciones(int preparacion, int numeroContenedor)
        {
            string sql = $@"
                SELECT 
                    COUNT(*)
                FROM T_DET_PICKING PICK 
                INNER JOIN T_PEDIDO_SAIDA PED ON PED.NU_PEDIDO = PICK.NU_PEDIDO 
                    AND PED.CD_CLIENTE = PICK.CD_CLIENTE 
                    AND PED.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_PRODUTO PROD ON PROD.CD_PRODUTO = PICK.CD_PRODUTO 
                    AND PROD.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_CLASSE CLA ON CLA.CD_CLASSE = PROD.CD_CLASSE
                WHERE PICK.NU_PREPARACION = :preparacion 
                    AND PICK.NU_CONTENEDOR = :numeroContenedor 
                GROUP BY 
                    PED.CD_EMPRESA,
                    PED.CD_CLIENTE,
                    PED.VL_COMPARTE_CONTENEDOR_PICKING,
                    CLA.CD_SUB_CLASSE";

            var query = _dapper.Query<int>(_context.Database.GetDbConnection(), sql, new
            {
                preparacion = preparacion,
                numeroContenedor = numeroContenedor
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.FirstOrDefault() > 0;
        }

        public virtual bool TieneOtraAgrupacion(int preparacion, int numeroContenedor, string vlComparteContenedor, string subClase, string cliente)
        {
            string sql = $@"
                SELECT 
                    COUNT(*)
                FROM T_DET_PICKING PICK 
                INNER JOIN T_PEDIDO_SAIDA PED ON PED.NU_PEDIDO = PICK.NU_PEDIDO 
                    AND PED.CD_CLIENTE = PICK.CD_CLIENTE 
                    AND PED.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_PRODUTO PROD ON PROD.CD_PRODUTO = PICK.CD_PRODUTO 
                    AND PROD.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_CLASSE CLA ON CLA.CD_CLASSE = PROD.CD_CLASSE
                WHERE PICK.NU_PREPARACION = :preparacion 
                    AND PICK.NU_CONTENEDOR = :numeroContenedor 
                    AND (
                        PED.CD_CLIENTE <> :cliente 
                        OR PED.VL_COMPARTE_CONTENEDOR_PICKING <> :vlComparteContenedor 
                        OR CLA.CD_SUB_CLASSE <> :subClase
                    ) 
                GROUP BY 
                    PED.CD_EMPRESA,
                    PED.CD_CLIENTE,
                    PED.VL_COMPARTE_CONTENEDOR_PICKING,
                    CLA.CD_SUB_CLASSE";

            var query = _dapper.Query<int>(_context.Database.GetDbConnection(), sql, new
            {
                preparacion = preparacion,
                numeroContenedor = numeroContenedor,
                cliente = cliente,
                vlComparteContenedor = vlComparteContenedor,
                subClase = subClase,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.FirstOrDefault() > 0;
        }

        public virtual bool AnyPickingPendienteConLoteAuto(int preparacion, string vlComparteContenedorPicking, string subClase)
        {
            string sql = $@"
                SELECT 
                    COUNT(*)
                FROM T_DET_PICKING PICK 
                INNER JOIN T_PEDIDO_SAIDA PED ON PED.NU_PEDIDO = PICK.NU_PEDIDO 
                    AND PED.CD_CLIENTE = PICK.CD_CLIENTE 
                    AND PED.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_PRODUTO PROD ON PROD.CD_PRODUTO = PICK.CD_PRODUTO 
                    AND PROD.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_CLASSE CLA ON CLA.CD_CLASSE = PROD.CD_CLASSE
                WHERE PICK.ND_ESTADO = :P_ND_ESTADO 
                    AND PICK.NU_IDENTIFICADOR = '(AUTO)' 
                    AND PICK.NU_PREPARACION = :preparacion 
                    AND CLA.CD_SUB_CLASSE = :subClase 
                    AND PED.VL_COMPARTE_CONTENEDOR_PICKING = :vlComparteContenedorPicking";

            var query = _dapper.Query<int>(_context.Database.GetDbConnection(), sql, new
            {
                preparacion = preparacion,
                P_ND_ESTADO = EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO,
                vlComparteContenedorPicking = vlComparteContenedorPicking,
                subClase = subClase,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.FirstOrDefault() > 0;
        }

        public virtual PtlAgrupacionContenedor GetAgrupacionContenedor(int numeroPreparacion, int numeroContenedor)
        {
            string sql = $@"
                SELECT 
                    PICK.NU_PREPARACION Preparacion,
                    PICK.NU_CONTENEDOR Contenedor,
                    PED.CD_EMPRESA Empresa,
                    PED.CD_CLIENTE Cliente,
                    PED.VL_COMPARTE_CONTENEDOR_PICKING VlComparteContenedorPicking,
                    CLA.CD_SUB_CLASSE SubClase
                FROM T_DET_PICKING PICK 
                INNER JOIN T_PEDIDO_SAIDA PED ON PED.NU_PEDIDO = PICK.NU_PEDIDO 
                    AND PED.CD_CLIENTE = PICK.CD_CLIENTE 
                    AND PED.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_PRODUTO PROD ON PROD.CD_PRODUTO = PICK.CD_PRODUTO 
                    AND PROD.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_CLASSE CLA ON CLA.CD_CLASSE = PROD.CD_CLASSE
                WHERE PICK.NU_PREPARACION = :preparacion 
                    AND PICK.NU_CONTENEDOR = :numeroContenedor
                GROUP BY 
                    PICK.NU_PREPARACION,
                    PICK.NU_CONTENEDOR,
                    PED.CD_EMPRESA,
                    PED.CD_CLIENTE,
                    PED.VL_COMPARTE_CONTENEDOR_PICKING,
                    CLA.CD_SUB_CLASSE";

            var query = _dapper.Query<PtlAgrupacionContenedor>(_context.Database.GetDbConnection(), sql, new
            {
                preparacion = numeroPreparacion,
                numeroContenedor = numeroContenedor
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.FirstOrDefault();
        }

        public virtual void AnyDetalleParaCarga(DbConnection connection, DbTransaction tran, out long? cargaConProblema)
        {
            string sql = @$"SELECT 
                            ct.NU_CARGA 
                        FROM T_CARGA_TEMP  ct
                        LEFT JOIN T_DET_PICKING  dp ON  dp.NU_PREPARACION = ct.NU_PREPARACION
                            AND dp.CD_EMPRESA = ct.CD_EMPRESA
                            AND dp.NU_CARGA = ct.NU_CARGA
                        WHERE dp.NU_PREPARACION is null";

            cargaConProblema = _dapper.Query<long?>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        #endregion

        #region Update

        public virtual void UpdateDetallesPreparacion(int preparacion, int empresa, string cliente, string estado, string estadoAnterior, long numeroTransaccion)
        {
            string sql = @"
                UPDATE T_DET_PICKING 
                SET ND_ESTADO = :estado, 
                    NU_TRANSACCION = :numeroTransaccion 
                WHERE NU_PREPARACION = :preparacion 
                    AND CD_EMPRESA = :empresa 
                    AND CD_CLIENTE = :cliente 
                    AND ND_ESTADO = :estadoAnterior";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, new
            {
                estado = estado,
                estadoAnterior = estadoAnterior,
                preparacion = preparacion,
                empresa = empresa,
                cliente = cliente,
                numeroTransaccion = numeroTransaccion,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdatePreparacionesNotificadas(int nuPreparacion, long nuTransaccion)
        {
            string sql = @"
                UPDATE T_PICKING 
                SET ID_AVISO = 'N', 
                    NU_TRANSACCION = :nuTransaccion 
                WHERE ID_AVISO = 'A' 
                    AND NU_PREPARACION = :nuPreparacion";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, new
            {
                nuTransaccion = nuTransaccion,
                nuPreparacion = nuPreparacion,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void NivelarCargasDetallePicking(ContenedorFacturar contenedor)
        {
            string sql = @"
                UPDATE T_DET_PICKING
                    SET NU_CARGA = :carga
                WHERE NU_PREPARACION = :preparacion
                    AND NU_PEDIDO = :pedido
                    AND NU_CONTENEDOR = :contenedor
                    AND CD_EMPRESA = :empresa
                    AND CD_CLIENTE = :cliente
                    AND NU_CARGA != :carga";
            _dapper.Execute(_context.Database.GetDbConnection(), sql, new
            {
                carga = contenedor.NumeroCarga,
                preparacion = contenedor.NumeroPreparacion,
                pedido = contenedor.NumeroPedido,
                contenedor = contenedor.NumeroContenedor,
                empresa = contenedor.CodigoEmpresa,
                cliente = contenedor.CodigoCliente,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #endregion

        #endregion
    }
}
