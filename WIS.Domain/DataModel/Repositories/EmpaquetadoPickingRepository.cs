using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Impresiones.Dtos;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class EmpaquetadoPickingRepository
    {
        protected readonly string _application;
        protected WISDB _context;
        protected readonly int _userId;
        protected readonly IDapper _dapper;

        public EmpaquetadoPickingRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._context = context;
        }

        #region Any

        public virtual bool ContenedorPuedeUsarseEnEmpaque(int nuContenedor, int nuPreparacion)
        {
            return _context.T_DET_PICKING
                .Where(dp => dp.NU_PREPARACION == nuPreparacion
                    && dp.NU_CONTENEDOR == nuContenedor)
                .Join(_context.T_PEDIDO_SAIDA,
                    dp => new { dp.NU_PEDIDO, dp.CD_CLIENTE, dp.CD_EMPRESA },
                    ps => new { ps.NU_PEDIDO, ps.CD_CLIENTE, ps.CD_EMPRESA },
                    (dp, ps) => new { DetallePicking = dp, Pedido = ps })
                .Join(_context.T_TIPO_EXPEDICION,
                    dpps => new { dpps.Pedido.TP_EXPEDICION },
                    te => new { te.TP_EXPEDICION },
                    (dpps, te) => new { DetallePicking = dpps.DetallePicking, Pedido = dpps.Pedido, TipoExpedicion = te })
                .Where(dppste => dppste.TipoExpedicion.FL_EMPAQUETA_CONTENEDOR == "S")
                .AsNoTracking()
                .Count() > 0;
        }

        public virtual bool TieneStockProductoContenedor(int nuContenedor, int nuPreparacion, string cdProducto)
        {
            return _context.T_DET_PICKING.AsNoTracking()
                .Any(dp => dp.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO
                    && dp.NU_CONTENEDOR == nuContenedor
                    && dp.NU_PREPARACION == nuPreparacion
                    && dp.CD_PRODUTO == cdProducto);
        }

        public virtual bool TieneMasDeUnPedidoProductoContenedor(int nuContenedor, int nuPreparacion, string cdProducto, bool filtrarComparteContenedorEntrega, string comparteContenedorEntrega)
        {
            return _context.V_PEDIDO_PRODUTO_CONTENEDOR
                .AsNoTracking()
                .Where(x => x.NU_CONTENEDOR == nuContenedor
                    && x.NU_PREPARACION == nuPreparacion
                    && x.CD_PRODUTO == cdProducto
                    && (!filtrarComparteContenedorEntrega ? true
                        : (!string.IsNullOrEmpty(comparteContenedorEntrega) ? x.VL_COMPARTE_CONTENEDOR_ENTREGA == comparteContenedorEntrega
                            : string.IsNullOrEmpty(x.VL_COMPARTE_CONTENEDOR_ENTREGA))))
                .Count() > 1;
        }

        public virtual bool TieneMasDeUnPedidoProductoLote(int nuContenedor, int nuPreparacion, string cdProducto, bool filtrarComparteContenedorEntrega, string comparteContenedorEntrega, out decimal qtProducto, out string identificador)
        {
            qtProducto = 0;
            identificador = string.Empty;

            var query = _context.V_PRD_PEDIDO_LOTE_CONTENEDOR
                .AsNoTracking()
                .Where(x => x.NU_CONTENEDOR == nuContenedor
                    && x.NU_PREPARACION == nuPreparacion
                    && x.CD_PRODUTO == cdProducto
                    && (!filtrarComparteContenedorEntrega ? true
                        : (!string.IsNullOrEmpty(comparteContenedorEntrega) ? x.VL_COMPARTE_CONTENEDOR_ENTREGA == comparteContenedorEntrega
                            : string.IsNullOrEmpty(x.VL_COMPARTE_CONTENEDOR_ENTREGA))))
                .ToList();

            if (query.Count == 1)
            {
                var infoLote = query.FirstOrDefault();

                qtProducto = infoLote.QT_PRODUTO ?? 0;
                identificador = infoLote.NU_IDENTIFICADOR;
            }

            return query.Count > 1;
        }

        public virtual bool TieneMasDeUnProductoLotePorPedido(int nuContenedor, int nuPreparacion, string cdProducto, string pedido)
        {
            var total = _context.V_PRD_PEDIDO_LOTE_CONTENEDOR
                .AsNoTracking()
                .Where(x => x.NU_CONTENEDOR == nuContenedor
                    && x.NU_PREPARACION == nuPreparacion
                    && x.CD_PRODUTO == cdProducto
                    && x.NU_PEDIDO == pedido)
                .Count();


            return total > 1;
        }

        public virtual bool IsPedidoTodoAsignadoCamion(int empresa, string cdCliente, string nuPedido, int cdCamion)
        {
            bool response = false;
            var estadosAnulacion = EstadoDetallePreparacion.GetEstadosAnulacion();

            var situacionesContenedor = new List<short> { 601, 602 };
            var result = _context.T_DET_PICKING
                .Where(dp => dp.NU_PEDIDO == nuPedido
                    && dp.CD_CLIENTE == cdCliente
                    && dp.CD_EMPRESA == empresa
                    && !estadosAnulacion.Contains(dp.ND_ESTADO))
                .GroupJoin(_context.T_CLIENTE_CAMION,
                    dp => new { dp.CD_CLIENTE, dp.CD_EMPRESA, NU_CARGA = (dp.NU_CARGA ?? -1) },
                    cc => new { cc.CD_CLIENTE, cc.CD_EMPRESA, cc.NU_CARGA },
                    (dp, cc) => new { DetallePicking = dp, ClienteCamion = cc })
                .SelectMany(dpcc => dpcc.ClienteCamion.DefaultIfEmpty(),
                    (dpcc, cc) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = cc })
                .GroupJoin(_context.T_CONTENEDOR,
                    dpcc => new { dpcc.DetallePicking.NU_PREPARACION, NU_CONTENEDOR = (dpcc.DetallePicking.NU_CONTENEDOR ?? -1) },
                    c => new { c.NU_PREPARACION, c.NU_CONTENEDOR },
                    (dpcc, c) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = dpcc.ClienteCamion, Contenedor = c })
                .SelectMany(dpcc => dpcc.Contenedor.DefaultIfEmpty(),
                    (dpcc, c) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = dpcc.ClienteCamion, Contenedor = c })
                .AsNoTracking()
                .Where(dpccc => dpccc.Contenedor == null || situacionesContenedor.Contains(dpccc.Contenedor.CD_SITUACAO ?? 601))
                .GroupBy(dpccc => new { dpccc.ClienteCamion.CD_CAMION })
                .Select(g => g.Key.CD_CAMION);

            if (result == null || result.Count() == 0)
                response = false;

            if (result.Count() == 1 && result.FirstOrDefault() == cdCamion)
                response = true;

            if (result.Count() > 1)
                response = false;

            return response;
        }

        public virtual int AnyCamionPedido(string nuPedido, string cdCliente, int empresa)
        {
            var camiones = _context.T_DET_PICKING
                .Where(dp => dp.NU_PEDIDO == nuPedido
                    && dp.CD_CLIENTE == cdCliente
                    && dp.CD_EMPRESA == empresa)
                .GroupJoin(_context.T_CLIENTE_CAMION,
                    dp => new { dp.CD_CLIENTE, dp.CD_EMPRESA, NU_CARGA = (dp.NU_CARGA ?? -1) },
                    cc => new { cc.CD_CLIENTE, cc.CD_EMPRESA, cc.NU_CARGA },
                    (dp, cc) => new { DetallePicking = dp, ClienteCamion = cc })
                .SelectMany(dpcc => dpcc.ClienteCamion.DefaultIfEmpty(),
                    (dpcc, cc) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = cc })
                .GroupJoin(_context.T_CAMION,
                    dpcc => new { dpcc.ClienteCamion.CD_CAMION },
                    c => new { c.CD_CAMION },
                    (dpcc, c) => new { DetallePicking = dpcc.DetallePicking, Camion = c })
                .SelectMany(dpc => dpc.Camion.DefaultIfEmpty(),
                    (dpc, c) => new { DetallePicking = dpc.DetallePicking, Camion = c })
                .Where(dpc => dpc.Camion != null)
                .GroupBy(dpc => new { dpc.Camion.CD_CAMION })
                .Select(g => new { g.Key.CD_CAMION });

            var camion = camiones
                .OrderBy(c => c.CD_CAMION)
                .FirstOrDefault()?.CD_CAMION ?? -1;

            return camion == 0 ? -1 : camion;
        }

        public virtual bool IsCamionTodoEmpaquetado(int cdCamion)
        {
            return _context.T_DET_PICKING
                .Join(_context.T_CLIENTE_CAMION.Where(cc => cc.CD_CAMION == cdCamion),
                    dp => new { dp.CD_EMPRESA, dp.CD_CLIENTE, NU_CARGA = (dp.NU_CARGA ?? -1) },
                    cc => new { cc.CD_EMPRESA, cc.CD_CLIENTE, cc.NU_CARGA },
                    (dp, cc) => new { DetallePicking = dp, ClienteCamion = cc })
                .Join(_context.T_CONTENEDOR.Where(c => (c.ID_CONTENEDOR_EMPAQUE == "N" || c.ID_CONTENEDOR_EMPAQUE == null)),
                    dpcc => new { dpcc.DetallePicking.NU_PREPARACION, NU_CONTENEDOR = (dpcc.DetallePicking.NU_CONTENEDOR ?? -1) },
                    c => new { c.NU_PREPARACION, c.NU_CONTENEDOR },
                    (dpcc, c) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = dpcc.ClienteCamion, Contenedor = c })
                .FirstOrDefault() == null;

        }

        public virtual bool IsCamionTodoLiberado(int cdCamion)
        {
            return _context.T_DET_PEDIDO_SAIDA
                .Where(dps => ((dps.QT_PEDIDO ?? 0) - (dps.QT_ANULADO ?? 0)) > (dps.QT_LIBERADO ?? 0))
                .GroupJoin(_context.T_DET_PICKING,
                    dps => new { dps.CD_PRODUTO, dps.CD_FAIXA, dps.NU_IDENTIFICADOR, dps.CD_EMPRESA, dps.NU_PEDIDO, dps.CD_CLIENTE, dps.ID_ESPECIFICA_IDENTIFICADOR },
                    dp => new { dp.CD_PRODUTO, dp.CD_FAIXA, dp.NU_IDENTIFICADOR, dp.CD_EMPRESA, dp.NU_PEDIDO, dp.CD_CLIENTE, dp.ID_ESPECIFICA_IDENTIFICADOR },
                    (dps, dp) => new { DetallePedido = dps, DetallePicking = dp })
                .SelectMany(dpsdp => dpsdp.DetallePicking.DefaultIfEmpty(),
                    (dpsdp, dp) => new { DetallePedido = dpsdp.DetallePedido, DetallePicking = dp })
                .GroupJoin(_context.T_CLIENTE_CAMION.Where(cc => cc.CD_CAMION == cdCamion),
                    dpsdp => new { dpsdp.DetallePicking.CD_EMPRESA, dpsdp.DetallePicking.CD_CLIENTE, NU_CARGA = (dpsdp.DetallePicking.NU_CARGA ?? -1) },
                    cc => new { cc.CD_EMPRESA, cc.CD_CLIENTE, cc.NU_CARGA },
                    (dpsdp, cc) => new { DetallePedido = dpsdp.DetallePedido, DetallePicking = dpsdp.DetallePicking, ClienteCarga = cc })
                .SelectMany(dpsdpcc => dpsdpcc.ClienteCarga.DefaultIfEmpty(),
                    (dpsdpcc, cc) => new { DetallePedido = dpsdpcc.DetallePedido, DetallePicking = dpsdpcc.DetallePicking, ClienteCarga = cc })
                .AsNoTracking()
                .FirstOrDefault() == null;
        }

        public virtual bool IsCamionTodoPickeado(int cdCamion)
        {
            var estados = EstadoDetallePreparacion.GetEstadosAnulacion();
            estados.Add(EstadoDetallePreparacion.ESTADO_PREPARADO);

            return _context.T_DET_PICKING
                .Where(dp => !estados.Contains(dp.ND_ESTADO))
                .Join(_context.T_CLIENTE_CAMION.Where(cc => cc.CD_CAMION == cdCamion),
                    dp => new { dp.CD_EMPRESA, dp.CD_CLIENTE, NU_CARGA = (dp.NU_CARGA ?? -1) },
                    cc => new { cc.CD_EMPRESA, cc.CD_CLIENTE, cc.NU_CARGA },
                    (dp, cc) => new { DetallePicking = dp, ClienteCamion = cc })
                .AsNoTracking()
                .FirstOrDefault() == null;
        }

        public virtual bool ExisteCamionPedidoEmpaque(int empresa, string cdCliente, string nuPedido, int? cdCamion)
        {
            return _context.V_PEDIDO_EMPAQUE
                .AsNoTracking()
                .Any(x => x.CD_EMPRESA == empresa
                    && x.CD_CLIENTE == cdCliente
                    && x.NU_PEDIDO == nuPedido
                    && x.CD_CAMION == cdCamion);
        }

        #endregion

        #region Get

        public virtual ContenedorDestinoData GetContenedorDestinoData(int nuContenedor, int nuPreparacion)
        {
            return _context.T_DET_PICKING
                .Where(dp => dp.NU_CONTENEDOR == nuContenedor
                    && dp.NU_PREPARACION == nuPreparacion)
                .Join(_context.T_CONTENEDOR,
                    dp => new { dp.NU_PREPARACION, NU_CONTENEDOR = (dp.NU_CONTENEDOR ?? -1) },
                    c => new { c.NU_PREPARACION, c.NU_CONTENEDOR },
                    (dp, c) => new { DetallePicking = dp, Contenedor = c })
                .Join(_context.T_PEDIDO_SAIDA,
                    dpc => new { dpc.DetallePicking.NU_PEDIDO, dpc.DetallePicking.CD_CLIENTE, dpc.DetallePicking.CD_EMPRESA },
                    ps => new { ps.NU_PEDIDO, ps.CD_CLIENTE, ps.CD_EMPRESA },
                    (dpc, ps) => new { DetallePicking = dpc.DetallePicking, Contenedor = dpc.Contenedor, Pedido = ps })
                .Join(_context.T_CLIENTE,
                    dpcps => new { dpcps.Pedido.CD_EMPRESA, dpcps.Pedido.CD_CLIENTE },
                    c => new { c.CD_EMPRESA, c.CD_CLIENTE },
                    (dpcps, c) => new { DetallePicking = dpcps.DetallePicking, Contenedor = dpcps.Contenedor, Pedido = dpcps.Pedido, Cliente = c })
                .AsNoTracking()
                .Select(x => new ContenedorDestinoData
                {
                    NumeroContenedor = x.DetallePicking.NU_CONTENEDOR.Value,
                    NumeroPreparacion = x.DetallePicking.NU_PREPARACION,
                    CodigoEmpresa = x.DetallePicking.CD_EMPRESA,
                    CodigoCliente = x.DetallePicking.CD_CLIENTE,
                    DescripcionCliente = x.Cliente.DS_CLIENTE,
                    NumeroPedido = x.Pedido.NU_PEDIDO,
                    Direccion = x.Pedido.DS_ENDERECO,
                    CompartContenedorEntrega = x.Pedido.VL_COMPARTE_CONTENEDOR_ENTREGA,
                    TipoPedido = x.Pedido.TP_PEDIDO,
                    CodigoZona = x.Pedido.CD_ZONA,
                    TipoExpedicion = x.Pedido.TP_EXPEDICION,
                    CodigoRota = (x.Pedido.CD_ROTA ?? -1).ToString(),
                    FechaEntrega = x.Pedido.DT_ENTREGA,
                    Anexo4 = x.Pedido.DS_ANEXO4,
                    Ubicacion = x.Contenedor.CD_ENDERECO,
                    SubClase = x.Contenedor.CD_SUB_CLASSE,
                    TipoContenedor = x.Contenedor.TP_CONTENEDOR,
                    IdExternoContenedor = x.Contenedor.ID_EXTERNO_CONTENEDOR
                })
                .FirstOrDefault();
        }

        public virtual int GetCantPedidosContenedor(int nuContenedor, int nuPreparacion, out int cantClientes, out string codigoDescripcionCliente)
        {
            cantClientes = 0;
            codigoDescripcionCliente = string.Empty;

            var query = _context.T_DET_PICKING
                .Where(dp => dp.NU_CONTENEDOR == nuContenedor
                    && dp.NU_PREPARACION == nuPreparacion)
                .Join(_context.T_CONTENEDOR,
                    dp => new { dp.NU_PREPARACION, NU_CONTENEDOR = (dp.NU_CONTENEDOR ?? -1) },
                    c => new { c.NU_PREPARACION, c.NU_CONTENEDOR },
                    (dp, c) => new { DetallePicking = dp, Contenedor = c })
                .Join(_context.T_PEDIDO_SAIDA,
                    dpc => new { dpc.DetallePicking.NU_PEDIDO, dpc.DetallePicking.CD_CLIENTE, dpc.DetallePicking.CD_EMPRESA },
                    ps => new { ps.NU_PEDIDO, ps.CD_CLIENTE, ps.CD_EMPRESA },
                    (dpc, ps) => new { DetallePicking = dpc.DetallePicking, Contenedor = dpc.Contenedor, Pedido = ps })
                .Join(_context.T_CLIENTE,
                    dpcps => new { dpcps.Pedido.CD_EMPRESA, dpcps.Pedido.CD_CLIENTE },
                    c => new { c.CD_EMPRESA, c.CD_CLIENTE },
                    (dpcps, c) => new { DetallePicking = dpcps.DetallePicking, Contenedor = dpcps.Contenedor, Pedido = dpcps.Pedido, Cliente = c })
                .AsNoTracking();

            cantClientes = query
                .Select(c => c.Cliente)
                .GroupBy(c => new { c.CD_CLIENTE, c.CD_EMPRESA })
                .Count();

            if (cantClientes == 1)
            {
                var cliente = query.Select(c => c.Cliente).FirstOrDefault();
                codigoDescripcionCliente = $"{cliente.CD_CLIENTE} - {cliente.DS_CLIENTE}";
            }

            return query.Select(p => p.Pedido)
                .GroupBy(p => new { p.NU_PEDIDO, p.CD_EMPRESA, p.CD_CLIENTE })
                .Count();
        }

        public virtual InfoPedidoPre340 GetInfoPedido(string pedido, string cliente, int empresa)
        {
            return _context.V_PEDIDO_CLIENTE_PR340
                .AsNoTracking()
                .Where(dp => dp.NU_PEDIDO == pedido
                    && dp.CD_CLIENTE == cliente
                    && dp.CD_EMPRESA == empresa)
                .Select(dp => new InfoPedidoPre340
                {
                    CD_CLIENTE = dp.CD_CLIENTE,
                    CD_CONDICION_LIBERACION = dp.CD_CONDICION_LIBERACION,
                    CD_EMPRESA = dp.CD_EMPRESA,
                    CD_ORIGEN = dp.CD_ORIGEN,
                    CD_ROTA = dp.CD_ROTA,
                    CD_SITUACAO = dp.CD_SITUACAO,
                    CD_TRANSPORTADORA = dp.CD_TRANSPORTADORA,
                    CD_ZONA = dp.CD_ZONA,
                    DS_ANEXO4 = dp.DS_ANEXO4,
                    DS_CLIENTE = dp.DS_CLIENTE,
                    DS_ENDERECO = dp.DS_ENDERECO,
                    DS_MEMO = dp.DS_MEMO,
                    DS_ROTA = dp.DS_ROTA,
                    DS_SITUACAO = dp.DS_SITUACAO,
                    DT_ADDROW = dp.DT_ADDROW,
                    DT_EMITIDO = dp.DT_EMITIDO,
                    DT_ENTREGA = dp.DT_ENTREGA,
                    DT_LIBERAR_DESDE = dp.DT_LIBERAR_DESDE,
                    DT_LIBERAR_HASTA = dp.DT_LIBERAR_HASTA,
                    DT_ULT_PREPARACION = dp.DT_ULT_PREPARACION,
                    DT_UPDROW = dp.DT_UPDROW,
                    ID_AGRUPACION = dp.ID_AGRUPACION,
                    ID_MANUAL = dp.ID_MANUAL,
                    NM_EMPRESA = dp.NM_EMPRESA,
                    NU_ORDEN_LIBERACION = dp.NU_ORDEN_LIBERACION,
                    NU_PEDIDO = dp.NU_PEDIDO,
                    NU_PRDC_INGRESO = dp.NU_PRDC_INGRESO,
                    NU_PREPARACION_MANUAL = dp.NU_PREPARACION_MANUAL,
                    NU_ULT_PREPARACION = dp.NU_ULT_PREPARACION,
                    TP_EXPEDICION = dp.TP_EXPEDICION,
                    TP_PEDIDO = dp.TP_PEDIDO,
                    VL_SERIALIZADO_1 = dp.VL_SERIALIZADO_1,
                })
                .FirstOrDefault();
        }

        public virtual int? Get_Camion_Pedido(string p_Pedido, string p_Cliente, int p_empresa)
        {
            return _context.V_PEDIDO_EMPAQUE
                .AsNoTracking()
                .Where(pe => pe.NU_PEDIDO == p_Pedido
                    && pe.CD_CLIENTE == p_Cliente
                    && pe.CD_EMPRESA == p_empresa)
                .Select(pe => pe.CD_CAMION)
                .FirstOrDefault();
        }

        public virtual List<CompatibilidadContenedor> GetCompatibilidadContenedores(int nuContenedor, int nuPreparacion)
        {
            return _context.T_DET_PICKING
            .Where(dp => dp.NU_CONTENEDOR == nuContenedor
                && dp.NU_PREPARACION == nuPreparacion)
            .Join(_context.T_PEDIDO_SAIDA,
                dp => new { dp.NU_PEDIDO, dp.CD_CLIENTE, dp.CD_EMPRESA },
                ps => new { ps.NU_PEDIDO, ps.CD_CLIENTE, ps.CD_EMPRESA },
                (dp, ps) => new { DetallePicking = dp, Pedido = ps })
            .AsNoTracking()
            .GroupBy(x => new
            {
                x.DetallePicking.CD_EMPRESA,
                x.DetallePicking.CD_CLIENTE,
                x.DetallePicking.NU_PEDIDO,
                x.Pedido.VL_COMPARTE_CONTENEDOR_ENTREGA
            })
            .Select(g => new CompatibilidadContenedor
            {
                CodigoCliente = g.Key.CD_CLIENTE,
                Empresa = g.Key.CD_EMPRESA,
                CompartContenedorEntrega = g.Key.VL_COMPARTE_CONTENEDOR_ENTREGA,
                NumeroPedido = g.Key.NU_PEDIDO,
            })
            .ToList();
        }

        public virtual string PedidoIniciadosEnContenedores(int nuContenedor, int nuPreparacion)
        {
            var pedidosContenedor = _context.T_DET_PICKING
                .AsNoTracking()
                .Where(dp => dp.NU_CONTENEDOR == nuContenedor
                    && dp.NU_PREPARACION == nuPreparacion)
                .GroupBy(dp => new
                {
                    dp.NU_PEDIDO,
                    dp.CD_CLIENTE,
                    dp.CD_EMPRESA,
                })
                .Select(g => new
                {
                    g.Key.NU_PEDIDO,
                    g.Key.CD_CLIENTE,
                    g.Key.CD_EMPRESA,
                });

            var contenedores = _context.T_DET_PICKING
                .Where(dp => dp.NU_PREPARACION == nuPreparacion)
                .Join(pedidosContenedor,
                    dp => new { dp.NU_PEDIDO, dp.CD_CLIENTE, dp.CD_EMPRESA },
                    pc => new { pc.NU_PEDIDO, pc.CD_CLIENTE, pc.CD_EMPRESA },
                    (dp, pc) => dp)
                .Join(_context.T_CONTENEDOR.Where(c => c.ID_CONTENEDOR_EMPAQUE == "S" && c.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion),
                    dp => new { dp.NU_PREPARACION, NU_CONTENEDOR = (dp.NU_CONTENEDOR ?? -1) },
                    co => new { co.NU_PREPARACION, co.NU_CONTENEDOR },
                    (dp, co) => new { DetallePicking = dp, Contendedor = co })
                .GroupJoin(_context.T_CLIENTE_CAMION,
                    dp => new { NU_CARGA = (dp.DetallePicking.NU_CARGA ?? -1), dp.DetallePicking.CD_CLIENTE, dp.DetallePicking.CD_EMPRESA },
                    cc => new { cc.NU_CARGA, cc.CD_CLIENTE, cc.CD_EMPRESA },
                    (dp, cc) => new { DetallePicking = dp.DetallePicking, ClienteCamion = cc, Contendedor = dp.Contendedor })
                .SelectMany(dpcc => dpcc.ClienteCamion.DefaultIfEmpty(),
                    (dpcc, cc) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = cc, Contenedor = dpcc.Contendedor })
                .GroupJoin(_context.T_CAMION,
                    dpcc => new { dpcc.ClienteCamion.CD_CAMION },
                    c => new { c.CD_CAMION },
                    (dpcc, c) => new { DetallePicking = dpcc.DetallePicking, Camion = c, Contenedor = dpcc.Contenedor })
                .SelectMany(dpc => dpc.Camion.DefaultIfEmpty(),
                    (dpc, c) => new { DetallePicking = dpc.DetallePicking, Camion = c, Contenedor = dpc.Contenedor })
                .AsNoTracking()
                .Where(dpc => dpc.Camion == null || dpc.Camion.DT_FACTURACION == null)
                .GroupBy(dpc => new { dpc.Contenedor.ID_EXTERNO_CONTENEDOR, dpc.Contenedor.TP_CONTENEDOR })
                .Select(co => $"{co.Key.ID_EXTERNO_CONTENEDOR}-{co.Key.TP_CONTENEDOR}")
                .ToList();

            return string.Join(",", contenedores);
        }

        public virtual DatosClientePedidoOriginal GetDatosClientePedidoOriginal(int nuContenedor, int nuPreparacion)
        {
            int cantidadPedidos = _context.T_DET_PICKING
                .AsNoTracking()
                .Where(x => x.NU_CONTENEDOR == nuContenedor
                    && x.NU_PREPARACION == nuPreparacion)
                .GroupBy(x => new
                {
                    x.NU_PEDIDO,
                    x.CD_CLIENTE,
                    x.CD_EMPRESA
                })
                .Count();

            if (cantidadPedidos == 1)
            {
                return _context.T_DET_PICKING
                    .Where(x => x.NU_CONTENEDOR == nuContenedor
                        && x.NU_PREPARACION == nuPreparacion)
                    .Join(_context.T_PEDIDO_SAIDA,
                        dp => new { dp.NU_PEDIDO, dp.CD_EMPRESA, dp.CD_CLIENTE },
                        ps => new { ps.NU_PEDIDO, ps.CD_EMPRESA, ps.CD_CLIENTE },
                        (dp, ps) => new { DetallePicking = dp, Pedido = ps })
                    .Join(_context.T_ROTA,
                        dpps => new { CD_ROTA = (short)(dpps.Pedido.CD_ROTA ?? -1) },
                        r => new { r.CD_ROTA },
                        (dpps, r) => new { DetallePicking = dpps.DetallePicking, Pedido = dpps.Pedido, Ruta = r })
                    .Join(_context.T_CLIENTE,
                        dppsr => new { dppsr.Pedido.CD_EMPRESA, dppsr.Pedido.CD_CLIENTE },
                        c => new { c.CD_EMPRESA, c.CD_CLIENTE },
                        (dppsr, c) => new { DetallePicking = dppsr.DetallePicking, Pedido = dppsr.Pedido, Ruta = dppsr.Ruta, Cliente = c })
                    .AsNoTracking()
                    .Select(x => new DatosClientePedidoOriginal
                    {
                        Empresa = x.DetallePicking.CD_EMPRESA,
                        CodigoCliente = x.DetallePicking.CD_CLIENTE,
                        DescripcionCliente = x.Cliente.DS_CLIENTE,
                        NumeroPedido = x.DetallePicking.NU_PEDIDO,
                        Direccion = x.Pedido.DS_ENDERECO,
                        CompartContenedorEntrega = x.Pedido.VL_COMPARTE_CONTENEDOR_ENTREGA,
                        TipoPedido = x.Pedido.TP_PEDIDO,
                        CodigoZona = x.Pedido.CD_ZONA,
                        TipoExpedicion = x.Pedido.TP_EXPEDICION,
                        CodigoRuta = (short)(x.Pedido.CD_ROTA ?? -1),
                        DescripcionRuta = x.Ruta.DS_ROTA,
                        FechaEntrega = x.Pedido.DT_ENTREGA.ToString(),
                        Anexo4 = x.Pedido.DS_ANEXO4,
                    })
                    .FirstOrDefault();
            }

            return null;
        }

        public virtual int GetCantidadLineasContenedor(int nucontenedor, int nuPreparacion)
        {
            return _context.T_DET_PICKING
                .AsNoTracking()
                .Where(dp => dp.NU_PREPARACION == nuPreparacion
                    && dp.NU_CONTENEDOR == nucontenedor)
                .GroupBy(x => new { x.CD_PRODUTO, x.NU_IDENTIFICADOR })
                .Count();
        }

        public virtual List<V_PRD_PEDIDO_LOTE_CONTENEDOR> GetProductosPedidosLoteContenedor(int nucontenedor, int nuPreparacion, string nuPedido, string cdCliente, int empresa)
        {
            return _context.V_PRD_PEDIDO_LOTE_CONTENEDOR
                .AsNoTracking()
                .Where(x => x.NU_PREPARACION == nuPreparacion
                    && x.NU_CONTENEDOR == nucontenedor
                    && x.NU_PEDIDO == nuPedido
                    && x.CD_CLIENTE == cdCliente
                    && x.CD_EMPRESA == empresa)
                .ToList();
        }

        public virtual PedidoProductoContenedor GetOnlyPedidoProductoContenedor(int nuContenedor, int nuPreparacion, string cdProducto)
        {
            var value = _context.V_PEDIDO_PRODUTO_CONTENEDOR
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_CONTENEDOR == nuContenedor
                    && x.NU_PREPARACION == nuPreparacion
                    && x.CD_PRODUTO == cdProducto);

            return new PedidoProductoContenedor()
            {
                NumeroContenedor = value.NU_CONTENEDOR,
                NumeroPreparacion = value.NU_PREPARACION,
                CantidadProducto = value.QT_PRODUTO,
                CodigoCliente = value.CD_CLIENTE,
                CodigoProducto = value.CD_PRODUTO,
                DireccionEntrega = value.DS_ENTREGA,
                Empresa = value.CD_EMPRESA,
                NumeroPedido = value.NU_PEDIDO,
                TipoExpedicion = value.TP_EXPEDICION,
                ComparteContenedorEntrega = value.VL_COMPARTE_CONTENEDOR_ENTREGA
            };
        }

        public virtual PedidoLoteContenedor GetPedidoProductoLote(int nuContenedor, int nuPreparacion, string cdProducto, string identificador)
        {
            var result = _context.V_PRD_PEDIDO_LOTE_CONTENEDOR
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_CONTENEDOR == nuContenedor
                    && x.NU_PREPARACION == nuPreparacion
                    && x.CD_PRODUTO == cdProducto
                    && x.NU_IDENTIFICADOR == identificador);

            return new PedidoLoteContenedor
            {
                CodigoCliente = result.CD_CLIENTE,
                Anexo1 = result.DS_ANEXO1,
                Anexo2 = result.DS_ANEXO2,
                Anexo3 = result.DS_ANEXO3,
                Anexo4 = result.DS_ANEXO4,
                CantidadProducto = result.QT_PRODUTO,
                CodigoEmpresa = result.CD_EMPRESA,
                CodigoProducto = result.CD_PRODUTO,
                ComparteContenedorEntrega = result.VL_COMPARTE_CONTENEDOR_ENTREGA,
                ComparteContenedorPicking = result.VL_COMPARTE_CONTENEDOR_PICKING,
                DescripcionProducto = result.DS_PRODUTO,
                DescripcionUbicacion = result.DS_ENDERECO,
                Faixa = result.CD_FAIXA,
                Identificador = result.NU_IDENTIFICADOR,
                NumeroContenedor = result.NU_CONTENEDOR,
                NumeroPedido = result.NU_PEDIDO,
                NumeroPreparacion = result.NU_PREPARACION
            };
        }

        public virtual PedidoProductoContenedor GetPedidoProductoContenedor(int nuContenedor, int nuPreparacion, string cdProducto)
        {
            var value = _context.V_PEDIDO_PRODUTO_CONTENEDOR
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_CONTENEDOR == nuContenedor
                    && x.NU_PREPARACION == nuPreparacion
                    && x.CD_PRODUTO == cdProducto);

            return new PedidoProductoContenedor()
            {
                NumeroContenedor = value.NU_CONTENEDOR,
                NumeroPreparacion = value.NU_PREPARACION,
                CantidadProducto = value.QT_PRODUTO,
                CodigoCliente = value.CD_CLIENTE,
                CodigoProducto = value.CD_PRODUTO,
                DireccionEntrega = value.DS_ENTREGA,
                Empresa = value.CD_EMPRESA,
                NumeroPedido = value.NU_PEDIDO,
                TipoExpedicion = value.TP_EXPEDICION,
                ComparteContenedorEntrega = value.VL_COMPARTE_CONTENEDOR_ENTREGA
            };
        }

        public virtual string GetDsAnexo4DetallePedido(string nuPedido, string cdCliente, int empresa, string cdProducto, int cdFaixa, string identificador)
        {
            return _context.T_DET_PEDIDO_SAIDA
                .AsNoTracking()
                .Where(dps => dps.NU_PEDIDO == nuPedido
                    && dps.CD_CLIENTE == cdCliente
                    && dps.CD_EMPRESA == empresa
                    && dps.CD_PRODUTO == cdProducto
                    && dps.CD_FAIXA == cdFaixa
                    && dps.NU_IDENTIFICADOR == identificador)
                .Select(dps => dps.DS_ANEXO4)
                .FirstOrDefault();
        }

        public virtual decimal GetCantidadDetallePedido(string nuPedido, string cdCliente, int empresa, string cdProducto, int cdFaixa, string identificador)
        {
            return _context.V_PRD_PEDIDO_LOTE_CONTENEDOR
                .AsNoTracking()
                .Where(pplc => pplc.NU_PEDIDO == nuPedido
                    && pplc.CD_CLIENTE == cdCliente
                    && pplc.CD_EMPRESA == empresa
                    && pplc.CD_PRODUTO == cdProducto
                    && pplc.CD_FAIXA == cdFaixa
                    && pplc.NU_IDENTIFICADOR == identificador)
                .Select(pplc => pplc.QT_PRODUTO)
                .FirstOrDefault() ?? 0;
        }

        public virtual bool QuedaPedidoContenedor(int nuContenedor, int nuPreparacion, string cdCliente, int empresa, string nuPedido)
        {
            return _context.T_DET_PICKING
                .AsNoTracking()
                .Where(dp => dp.NU_CONTENEDOR == nuContenedor
                    && dp.NU_PREPARACION == nuPreparacion
                    && dp.NU_PEDIDO == nuPedido
                    && dp.CD_CLIENTE == cdCliente
                    && dp.CD_EMPRESA == empresa)
                .Count() > 0;
        }

        public virtual bool IsPedidoTodoLiberado(int empresa, string cdCliente, string nuPedido)
        {
            bool response = false;

            var detallePedido = _context.T_DET_PEDIDO_SAIDA
                .AsNoTracking()
                .Where(dps => dps.CD_EMPRESA == empresa
                    && dps.CD_CLIENTE == cdCliente
                    && dps.NU_PEDIDO == nuPedido)
                .GroupBy(dps => true)
                .Select(g => new
                {
                    CantidadLiberada = g.Sum(dps => dps.QT_LIBERADO ?? 0),
                    CantidadPedido = g.Sum(dps => dps.QT_PEDIDO ?? 0),
                    CantidadAnulado = g.Sum(dps => dps.QT_ANULADO ?? 0),
                })
                .FirstOrDefault();

            var estadosAnulacion = EstadoDetallePreparacion.GetEstadosAnulacion();

            var detallePicking = _context.T_DET_PICKING
                .AsNoTracking()
                .Where(dp => dp.CD_EMPRESA == empresa
                    && dp.CD_CLIENTE == cdCliente
                    && dp.NU_PEDIDO == nuPedido
                    && !estadosAnulacion.Contains(dp.ND_ESTADO))
                .GroupBy(dp => true)
                .Select(g => new
                {
                    CantidadProducto = g.Sum(dp => dp.QT_PRODUTO ?? 0)
                })
                .FirstOrDefault();

            var qtLiberada = detallePedido?.CantidadLiberada ?? 0;
            var qtPedido = detallePedido?.CantidadPedido ?? 0;
            var qtAnulado = detallePedido?.CantidadAnulado ?? 0;
            var qtProducto = detallePicking?.CantidadProducto ?? 0;

            if (qtLiberada < (qtPedido - qtAnulado))
                response = false;
            else if (qtProducto == qtLiberada)
                response = true;

            return response;
        }

        public virtual List<long> GetCargas(int empresa, string cdCliente, string nuPedido, out int? cdCamion)
        {
            cdCamion = GetCamionPedido(empresa, cdCliente, nuPedido);

            var situacionesContenedor = new List<short> { SituacionDb.ContenedorEnPreparacion, SituacionDb.ContenedorEnCamion };
            var estadosAnulacion = EstadoDetallePreparacion.GetEstadosAnulacion();

            return _context.T_DET_PICKING
                .Where(dp => dp.NU_PEDIDO == nuPedido
                    && dp.CD_CLIENTE == cdCliente
                    && dp.CD_EMPRESA == empresa
                    && !estadosAnulacion.Contains(dp.ND_ESTADO)
                    && dp.NU_CARGA != null)
                .GroupJoin(_context.T_CLIENTE_CAMION,
                    dp => new { dp.CD_CLIENTE, dp.CD_EMPRESA, NU_CARGA = (dp.NU_CARGA ?? -1) },
                    cc => new { cc.CD_CLIENTE, cc.CD_EMPRESA, cc.NU_CARGA },
                    (dp, cc) => new { DetallePicking = dp, ClienteCamion = cc })
                .SelectMany(dpcc => dpcc.ClienteCamion.DefaultIfEmpty(),
                    (dpcc, cc) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = cc })
                .GroupJoin(_context.T_CONTENEDOR,
                    dpcc => new { dpcc.DetallePicking.NU_PREPARACION, NU_CONTENEDOR = (dpcc.DetallePicking.NU_CONTENEDOR ?? -1) },
                    c => new { c.NU_PREPARACION, c.NU_CONTENEDOR },
                    (dpcc, c) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = dpcc.ClienteCamion, Contenedor = c })
                .SelectMany(dpcc => dpcc.Contenedor.DefaultIfEmpty(),
                    (dpcc, c) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = dpcc.ClienteCamion, Contenedor = c })
                .AsNoTracking()
                .Where(dpccc => dpccc.ClienteCamion == null
                    && (dpccc.Contenedor == null || situacionesContenedor.Contains(dpccc.Contenedor.CD_SITUACAO ?? SituacionDb.ContenedorEnPreparacion)))
                .GroupBy(x => new { x.DetallePicking.NU_CARGA })
                .Select(g => g.Key.NU_CARGA.Value)
                .ToList();
        }

        public virtual List<long> GetCargas(int nuPreparacion, int nuContenedor)
        {
            var situacionesContenedor = new List<short> { SituacionDb.ContenedorEnPreparacion, SituacionDb.ContenedorEnCamion };
            var estadosAnulacion = EstadoDetallePreparacion.GetEstadosAnulacion();

            return _context.T_DET_PICKING
                .Where(dp => dp.NU_PREPARACION == nuPreparacion
                    && dp.NU_CONTENEDOR == nuContenedor
                    && !estadosAnulacion.Contains(dp.ND_ESTADO)
                    && dp.NU_CARGA != null)
                .GroupJoin(_context.T_CLIENTE_CAMION,
                    dp => new { dp.CD_CLIENTE, dp.CD_EMPRESA, NU_CARGA = (dp.NU_CARGA ?? -1) },
                    cc => new { cc.CD_CLIENTE, cc.CD_EMPRESA, cc.NU_CARGA },
                    (dp, cc) => new { DetallePicking = dp, ClienteCamion = cc })
                .SelectMany(dpcc => dpcc.ClienteCamion.DefaultIfEmpty(),
                    (dpcc, cc) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = cc })
                .GroupJoin(_context.T_CONTENEDOR,
                    dpcc => new { dpcc.DetallePicking.NU_PREPARACION, NU_CONTENEDOR = (dpcc.DetallePicking.NU_CONTENEDOR ?? -1) },
                    c => new { c.NU_PREPARACION, c.NU_CONTENEDOR },
                    (dpcc, c) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = dpcc.ClienteCamion, Contenedor = c })
                .SelectMany(dpcc => dpcc.Contenedor.DefaultIfEmpty(),
                    (dpcc, c) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = dpcc.ClienteCamion, Contenedor = c })
                .AsNoTracking()
                .Where(dpccc => dpccc.ClienteCamion == null
                    && (dpccc.Contenedor == null || situacionesContenedor.Contains(dpccc.Contenedor.CD_SITUACAO ?? SituacionDb.ContenedorEnPreparacion)))
                .GroupBy(x => new { x.DetallePicking.NU_CARGA })
                .Select(g => g.Key.NU_CARGA.Value)
                .ToList();
        }

        public virtual bool AnyContenedoresEquipo(string ubicacion)
        {
            return _context.T_CONTENEDOR.Any(x => x.CD_ENDERECO == ubicacion && x.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion);
        }

        public virtual int? GetCamionPedido(int empresa, string cdCliente, string nuPedido)
        {
            var camiones = _context.T_DET_PICKING
                .Where(dp => dp.NU_PEDIDO == nuPedido
                    && dp.CD_CLIENTE == cdCliente
                    && dp.CD_EMPRESA == empresa)
                .GroupJoin(_context.T_CLIENTE_CAMION,
                    dp => new { dp.CD_CLIENTE, dp.CD_EMPRESA, NU_CARGA = (dp.NU_CARGA ?? -1) },
                    cc => new { cc.CD_CLIENTE, cc.CD_EMPRESA, cc.NU_CARGA },
                    (dp, cc) => new { DetallePicking = dp, ClienteCamion = cc })
                .SelectMany(dpcc => dpcc.ClienteCamion.DefaultIfEmpty(),
                    (dpcc, cc) => new { DetallePicking = dpcc.DetallePicking, ClienteCamion = cc })
                .GroupJoin(_context.T_CAMION,
                    dpcc => new { dpcc.ClienteCamion.CD_CAMION },
                    c => new { c.CD_CAMION },
                    (dpcc, c) => new { DetallePicking = dpcc.DetallePicking, Camion = c })
                .SelectMany(dpc => dpc.Camion.DefaultIfEmpty(),
                    (dpc, c) => new { DetallePicking = dpc.DetallePicking, Camion = c })
                .Where(dpc => dpc.Camion != null && dpc.Camion.DT_FACTURACION == null && dpc.Camion.CD_SITUACAO == SituacionDb.CamionAguardandoCarga)
                .GroupBy(dpc => new { dpc.Camion.CD_CAMION })
                .Select(g => new { g.Key.CD_CAMION });

            return camiones
                .OrderBy(c => c.CD_CAMION)
                .FirstOrDefault()?.CD_CAMION;
        }

        public virtual DatosContenedorFinPicking GetDatosContenedorFinPicking(int nuPreparacion, int empresa, string cdCliente, string nuPedido)
        {
            var situacionesContenedor = new List<short> { 601, 602 };
            var datosContenedor = _context.T_DET_PICKING
                .Where(dp => dp.NU_PREPARACION == nuPreparacion
                    && dp.CD_EMPRESA == empresa
                    && dp.CD_CLIENTE == cdCliente
                    && dp.NU_PEDIDO == nuPedido)
                .Join(_context.T_CONTENEDOR.Where(c => situacionesContenedor.Contains(c.CD_SITUACAO ?? -1)),
                    dp => new { dp.NU_PREPARACION, NU_CONTENEDOR = (dp.NU_CONTENEDOR ?? -1) },
                    c => new { c.NU_PREPARACION, c.NU_CONTENEDOR },
                    (dp, c) => c)
                .AsNoTracking()
                .GroupBy(c => new
                {
                    c.NU_CONTENEDOR,
                    c.QT_BULTO,
                })
                .Select(g => new
                {
                    g.Key.NU_CONTENEDOR,
                    g.Key.QT_BULTO,
                })
                .GroupBy(c => true)
                .Select(g => new AuxCantidadDatosContenedorFinPicking
                {
                    CantidadContenedores = g.Count(),
                    CantidadTotalBultos = g.Sum(c => (c.QT_BULTO ?? 1)),
                })
                .FirstOrDefault();

            int cantidadColumnas = 0;
            if (datosContenedor.CantidadContenedores <= 10)
                cantidadColumnas = 1;
            else if (datosContenedor.CantidadContenedores > 10 && datosContenedor.CantidadContenedores < 21)
                cantidadColumnas = 2;
            else if (datosContenedor.CantidadContenedores > 20 && datosContenedor.CantidadContenedores < 31)
                cantidadColumnas = 3;
            else
                cantidadColumnas = 4;

            var contenedoresPicking = _context.T_DET_PICKING
                .Where(dp => dp.CD_CLIENTE == cdCliente
                    && dp.CD_EMPRESA == empresa
                    && dp.NU_PEDIDO == nuPedido)
                .Join(_context.T_CONTENEDOR.Where(c => situacionesContenedor.Contains(c.CD_SITUACAO ?? -1)),
                    dp => new { dp.NU_PREPARACION, NU_CONTENEDOR = (dp.NU_CONTENEDOR ?? -1) },
                    c => new { c.NU_PREPARACION, c.NU_CONTENEDOR },
                    (dp, c) => new { DetallePicking = dp, Contenedor = c })
                .AsNoTracking()
                .GroupBy(dps => new { dps.Contenedor.NU_CONTENEDOR, dps.Contenedor.QT_BULTO })
                .Select(g => new
                {
                    NumeroContenedor = g.Key.NU_CONTENEDOR,
                    CantidadBultos = g.Key.QT_BULTO ?? 1
                });

            var contenedores = contenedoresPicking
                .OrderBy(cp => cp.NumeroContenedor)
                .ToList();

            int columna = 1;
            int linea = 1;
            string bulto = string.Empty;
            DatosContenedorFinPicking result = new DatosContenedorFinPicking();
            result.CantidadTotalBultos = datosContenedor.CantidadTotalBultos;

            foreach (var contenedor in contenedores)
            {
                if (contenedor.CantidadBultos > 1)
                    bulto = " x " + contenedor.CantidadBultos;

                if (linea == 1)
                {

                    if (columna == 1)
                        result.vLCont001 = contenedor.NumeroContenedor + bulto;
                    else
                        result.vLCont001 = result.vLCont001 + ", " + contenedor.NumeroContenedor + bulto;
                }
                else if (linea == 2)
                {

                    if (columna == 1)
                        result.vLCont002 = contenedor.NumeroContenedor + bulto;
                    else
                        result.vLCont002 = result.vLCont002 + ", " + contenedor.NumeroContenedor + bulto;
                }
                else if (linea == 3)
                {

                    if (columna == 1)
                        result.vLCont003 = contenedor.NumeroContenedor + bulto;
                    else
                        result.vLCont003 = result.vLCont003 + ", " + contenedor.NumeroContenedor + bulto;
                }
                else if (linea == 4)
                {

                    if (columna == 1)
                        result.vLCont004 = contenedor.NumeroContenedor + bulto;
                    else
                        result.vLCont004 = result.vLCont004 + ", " + contenedor.NumeroContenedor + bulto;
                }
                else if (linea == 5)
                {

                    if (columna == 1)
                        result.vLCont005 = contenedor.NumeroContenedor + bulto;
                    else
                        result.vLCont005 = result.vLCont005 + ", " + contenedor.NumeroContenedor + bulto;
                }
                else if (linea == 6)
                {

                    if (columna == 1)
                        result.vLCont006 = contenedor.NumeroContenedor + bulto;
                    else
                        result.vLCont006 = result.vLCont006 + ", " + contenedor.NumeroContenedor + bulto;
                }
                else if (linea == 7)
                {

                    if (columna == 1)
                        result.vLCont007 = contenedor.NumeroContenedor + bulto;
                    else
                        result.vLCont007 = result.vLCont007 + ", " + contenedor.NumeroContenedor + bulto;
                }
                else if (linea == 8)
                {

                    if (columna == 1)
                        result.vLCont008 = contenedor.NumeroContenedor + bulto;
                    else
                        result.vLCont008 = result.vLCont008 + ", " + contenedor.NumeroContenedor + bulto;
                }
                else if (linea == 9)
                {

                    if (columna == 1)
                        result.vLCont009 = contenedor.NumeroContenedor + bulto;
                    else
                        result.vLCont009 = result.vLCont009 + ", " + contenedor.NumeroContenedor + bulto;
                }
                else if (linea == 10)
                {

                    if (columna == 1)
                        result.vLCont010 = contenedor.NumeroContenedor + bulto;
                    else
                        result.vLCont010 = result.vLCont010 + ", " + contenedor.NumeroContenedor + bulto;
                }

                if (columna == cantidadColumnas)
                {
                    if (linea < 10)
                    {
                        columna = 1;
                        linea = linea + 1;
                    }
                    else
                        break;
                }
                else
                {
                    columna = columna + 1;
                }
            }

            return result;
        }

        public virtual DatosContenedorBulto GetDatosContenedorBulto(int nuPreparacion, int nuContenedor)
        {
            return _context.T_DET_PICKING
                .Where(dp => dp.NU_PREPARACION == nuPreparacion
                    && dp.NU_CONTENEDOR == nuContenedor)
                .Join(_context.T_CONTENEDOR.Where(c => c.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion && c.ID_CONTENEDOR_EMPAQUE == "S"),
                    dp => new { dp.NU_PREPARACION, NU_CONTENEDOR = (dp.NU_CONTENEDOR ?? -1) },
                    c => new { c.NU_PREPARACION, c.NU_CONTENEDOR },
                    (dp, c) => new { DetallePicking = dp, Contenedor = c })
                .Join(_context.T_CLIENTE,
                    dpc => new { dpc.DetallePicking.CD_EMPRESA, dpc.DetallePicking.CD_CLIENTE },
                    c => new { c.CD_EMPRESA, c.CD_CLIENTE },
                    (dpc, c) => new { DetallePicking = dpc.DetallePicking, Contenedor = dpc.Contenedor, Cliente = c })
                .AsNoTracking()
                .GroupBy(x => new
                {
                    x.Contenedor.NU_CONTENEDOR,
                    x.Contenedor.NU_PREPARACION,
                    x.Cliente.DS_CLIENTE,
                    x.Contenedor.QT_BULTO,
                    x.Cliente.CD_CLIENTE,
                    x.DetallePicking.NU_PEDIDO,
                    x.Cliente.CD_EMPRESA,
                })
                .Select(g => new DatosContenedorBulto
                {
                    NumeroContenedor = g.Key.NU_CONTENEDOR,
                    NumeroPreparacion = g.Key.NU_PREPARACION,
                    DescripcionCliente = g.Key.DS_CLIENTE,
                    CantidadBultos = g.Key.QT_BULTO ?? 0,
                    CodigoCliente = g.Key.CD_CLIENTE,
                    NumeroPedido = g.Key.NU_PEDIDO,
                    Empresa = g.Key.CD_EMPRESA,
                })
                .FirstOrDefault();
        }

        public virtual int GetProximoNumeroEtiquetaUnidadTransferencia()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_UT_EXTERNO_NRO");
        }

        public virtual int? GetCamionPedidoEmpaque(int empresa, string cdCliente, string nuPedido)
        {
            return _context.V_PEDIDO_EMPAQUE
                .AsNoTracking()
                .Where(pe => pe.NU_PEDIDO == nuPedido
                    && pe.CD_CLIENTE == cdCliente
                    && pe.CD_EMPRESA == empresa)
                .Select(pe => pe.CD_CAMION)
                .FirstOrDefault();
        }

        #endregion

        #region Add

        #endregion

        #region Update

        #endregion

        #region Remove

        public virtual void CheckDeleteContenedor(int nuContenedor, int nuPreparacion, out bool contenedorOrigenVacio)
        {
            contenedorOrigenVacio = !_context.T_DET_PICKING.AsNoTracking().Any(x => x.NU_CONTENEDOR == nuContenedor && x.NU_PREPARACION == nuPreparacion);
            if (contenedorOrigenVacio)
            {
                var contenedor = _context.T_CONTENEDOR.FirstOrDefault(x => x.NU_PREPARACION == nuPreparacion && x.NU_CONTENEDOR == nuContenedor);
                var nuUt = contenedor.NU_UNIDAD_TRANSPORTE;

                _context.T_CONTENEDOR.Remove(contenedor);
                _context.SaveChanges();

                if (!_context.T_CONTENEDOR.Any(x => x.NU_UNIDAD_TRANSPORTE == nuUt))
                {
                    var ut = _context.T_UNIDAD_TRANSPORTE.FirstOrDefault(x => x.NU_UNIDAD_TRANSPORTE == nuUt);
                    if (ut != null)
                        _context.T_UNIDAD_TRANSPORTE.Remove(ut);
                }
            }
        }

        #endregion

        #region Dapper

        public virtual bool PedidoTodoPickeado(int empresa, string cliente, string ped)
        {
            string sql = @"
                SELECT 'x'
                 FROM T_DET_PICKING DP
                 WHERE DP.cd_empresa =  :CD_EMPRESA
                    AND DP.cd_cliente =  :CD_CLIENTE
                    AND DP.nu_pedido =  :NU_PEDIDO
                    AND DP.qt_preparado IS NULL
                 UNION ALL
                 SELECT 'x'
                 FROM T_DET_PICKING DP
                 INNER JOIN T_PEDIDO_SAIDA PS ON DP.cd_empresa = PS.cd_empresa
                    AND DP.cd_cliente = PS.cd_cliente
                    AND DP.nu_pedido = PS.nu_pedido
                 LEFT JOIN T_CONTENEDOR CO ON DP.nu_contenedor = CO.nu_contenedor
                    AND DP.nu_preparacion = CO.nu_preparacion
                 WHERE PS.cd_empresa = :CD_EMPRESA
                    AND PS.cd_cliente = :CD_CLIENTE
                    AND WISFUN!K_INTERFAZ.GET_UNSERIALIZADO(PS.vl_serializado_1,1) = :NU_PEDIDO  
                    AND PS.tp_pedido = 'WISMAC'
                    AND PS.cd_situacao <> 58
                    AND COALESCE(CO.cd_situacao,-1) <> 605";

            var rest = _dapper.Query<string>(_context.Database.GetDbConnection(), sql, new
            {
                CD_EMPRESA = empresa,
                CD_CLIENTE = cliente,
                NU_PEDIDO = ped
            }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).FirstOrDefault();

            return string.IsNullOrEmpty(rest);
        }

        public virtual bool PedidoTodoEmpaquetado(int empresa, string cliente, string ped)
        {
            string sql = @" 
                SELECT 'x'
                FROM T_DET_PICKING DP,
                     T_CONTENEDOR CO
                WHERE DP.nu_contenedor  = CO.nu_contenedor
                    AND DP.nu_preparacion = CO.nu_preparacion
                    AND 'N' = COALESCE(CO.ID_CONTENEDOR_EMPAQUE,'N')
                    AND DP.nu_pedido = :NU_PEDIDO
                    AND DP.cd_cliente = :CD_CLIENTE
                    AND DP.cd_empresa = :CD_EMPRESA
                UNION ALL
                SELECT 'x'
                FROM T_DET_PICKING DP
                INNER JOIN T_PEDIDO_SAIDA PS ON DP.cd_empresa     = PS.cd_empresa
                    AND DP.cd_cliente = PS.cd_cliente
                    AND DP.nu_pedido = PS.nu_pedido
                LEFT JOIN T_CONTENEDOR CO ON DP.nu_contenedor  = CO.nu_contenedor
                    AND DP.nu_preparacion = CO.nu_preparacion
                WHERE PS.cd_empresa = :CD_EMPRESA
                    AND PS.cd_cliente = :CD_CLIENTE
                    AND  WISFUN!K_INTERFAZ.GET_UNSERIALIZADO(PS.vl_serializado_1,1) = :NU_PEDIDO
                    AND PS.tp_pedido = 'WISMAC'
                    AND PS.cd_situacao <> 58
                    AND COALESCE(CO.cd_situacao,-1)  <> 605";

            var rest = _dapper.Query<string>(_context.Database.GetDbConnection(), sql, new
            {
                CD_EMPRESA = empresa,
                CD_CLIENTE = cliente,
                NU_PEDIDO = ped
            }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).FirstOrDefault();

            return string.IsNullOrEmpty(rest);
        }

        public virtual int CrearCamion(string cdCliente, int empresa, string nuPedido, string predio, short puerta, long nroTransaccion)
        {
            var pedido = _context.T_PEDIDO_SAIDA.AsNoTracking().FirstOrDefault(x => x.CD_CLIENTE == cdCliente && x.CD_EMPRESA == empresa && x.NU_PEDIDO == nuPedido);
            var cliente = _context.T_CLIENTE.AsNoTracking().FirstOrDefault(x => x.CD_CLIENTE == cdCliente && x.CD_EMPRESA == empresa);

            var sql = @"INSERT INTO T_CAMION(
                        CD_CAMION,
                        CD_PLACA_CARRO,
                        CD_ROTA,
                        DS_CAMION,
                        CD_EMPRESA,
                        DT_PROGRAMADO,                        
                        TP_ARMADO_EGRESO,
                        ND_ARMADO_EGRESO,
                        CD_TRANSPORTADORA,
                        DT_ADDROW,
                        CD_SITUACAO,
                        NU_PREDIO,
                        CD_PORTA,
                        NU_TRANSACCION,
                        ID_RESPETA_ORD_CARGA,
                        FL_AUTO_CARGA,
                        FL_CIERRE_PARCIAL,
                        FL_TRACKING,
                        FL_RUTEO,
                        FL_HABILITADO_CARGA,
                        FL_HABILITADO_ARMADO,
                        FL_HABILITADO_CIERRE)
                       VALUES(
                        :cdCamion,
                        :cdPlacaCarro,
                        :cdRuta,
                        :dsCamion,
                        :empresa,
                        :fechaActual,
                        :tpArmadoEgreso,
                        :ndArmadoEgreso,
                        :cdTransportadora,
                        :fechaActual,
                        :cdSituacion,
                        :nuPredio,
                        :cdPuerta,
                        :nroTransaccion,
                        'N',
                        'S',
                        'N',
                        'N',
                        'N',
                        'N',
                        'N',
                        'N')";

            var cdCamion = _context.GetNextSequenceValueInt(_dapper, Secuencias.S_CD_CAMION);

            var dsCamion = $"{pedido.TP_PEDIDO} - {pedido.NU_PEDIDO} - {pedido.CD_CLIENTE} - {cliente.DS_CLIENTE}";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, new
            {
                cdCamion = cdCamion,
                cdPlacaCarro = "(Sin definir)",
                cdRuta = pedido?.CD_ROTA ?? 1,
                dsCamion = dsCamion.Length > 50 ? dsCamion.Substring(0, 49) : dsCamion,
                empresa = pedido.CD_EMPRESA,
                fechaActual = DateTime.Now,
                tpArmadoEgreso = TipoArmadoEgreso.Empaque,
                ndArmadoEgreso = TipoArmadoEgreso.ArmadoEmpaquetado,
                cdTransportadora = pedido?.CD_TRANSPORTADORA ?? 1,
                cdSituacion = SituacionDb.CamionAguardandoCarga,
                nuPredio = predio,
                cdPuerta = puerta,
                nroTransaccion = nroTransaccion,
            }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return cdCamion;
        }

        #endregion

        #region Logica

        public virtual void MoverContenedorEmpaquetado(int nuContenedorOrigen, int nuContenedorDestino, int nuPreparacionOrigen, int nuPreparacionDestino, string cdCliente, string nuPedido, string cdProducto, decimal cdFaixa, int empresa, string nuIdentificador, decimal cantidad, long nuTransaccion, out bool isContenedorOrigenVacio)
        {
            long? nuCarga = null;
            decimal qtTotalTransferido = 0;
            decimal qtNuevoRegistro = 0;
            int nuSecuenciaPreparacion = 0;

            var lstPicking = _context.T_DET_PICKING
                .AsNoTracking()
                .Where(x => x.NU_CONTENEDOR == nuContenedorOrigen
                    && x.NU_PREPARACION == nuPreparacionOrigen
                    && x.CD_PRODUTO == cdProducto
                    && x.CD_EMPRESA == empresa
                    && x.CD_FAIXA == cdFaixa
                    && x.NU_IDENTIFICADOR == nuIdentificador
                    && x.CD_CLIENTE == cdCliente
                    && x.NU_PEDIDO == nuPedido
                    && x.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO)
                .OrderByDescending(x => x.QT_PREPARADO)
                .ToList();

            foreach (var detPicking in lstPicking)
            {
                if (qtTotalTransferido >= cantidad)
                    break;

                var detPickingDestino = _context.T_DET_PICKING
                    .AsNoTracking()
                    .FirstOrDefault(f => f.NU_CONTENEDOR == nuContenedorDestino
                        && f.NU_PREPARACION == nuPreparacionDestino);

                if (detPickingDestino != null)
                {
                    nuCarga = detPickingDestino.NU_CARGA;
                }
                else if (nuCarga == null)
                {
                    var carga = this._context.T_CARGA.FirstOrDefault(d => d.NU_CARGA == detPicking.NU_CARGA);

                    var nuevaCarga = new T_CARGA
                    {
                        NU_CARGA = _context.GetNextSequenceValueLong(_dapper, Secuencias.S_CARGA),
                        DS_CARGA = "Carga creada para empaquetado de picking",
                        DT_ADDROW = DateTime.Now,
                        CD_ROTA = carga.CD_ROTA,
                        NU_PREPARACION = nuPreparacionDestino,
                    };

                    _context.T_CARGA.Add(nuevaCarga);
                    _context.SaveChanges();

                    nuCarga = nuevaCarga.NU_CARGA;
                }

                if (detPicking.QT_PREPARADO <= (cantidad - qtTotalTransferido))
                {
                    nuSecuenciaPreparacion = nuPreparacionOrigen != nuPreparacionDestino ? _context.GetNextSequenceValueInt(_dapper, Secuencias.S_DET_PICKING)
                                             : detPicking.NU_SEQ_PREPARACION;

                    var detalle = _context.T_DET_PICKING
                        .FirstOrDefault(x => x.CD_PRODUTO == detPicking.CD_PRODUTO
                            && x.CD_EMPRESA == detPicking.CD_EMPRESA
                            && x.NU_IDENTIFICADOR == detPicking.NU_IDENTIFICADOR
                            && x.CD_FAIXA == detPicking.CD_FAIXA
                            && x.NU_PREPARACION == detPicking.NU_PREPARACION
                            && x.NU_PEDIDO == detPicking.NU_PEDIDO
                            && x.CD_CLIENTE == detPicking.CD_CLIENTE
                            && x.CD_ENDERECO == detPicking.CD_ENDERECO
                            && x.NU_SEQ_PREPARACION == detPicking.NU_SEQ_PREPARACION);

                    if (nuPreparacionDestino != nuPreparacionOrigen)
                    {
                        var newDetPicking = new T_DET_PICKING()
                        {
                            NU_PREPARACION = nuPreparacionDestino,
                            NU_CONTENEDOR = nuContenedorDestino,
                            CD_PRODUTO = detalle.CD_PRODUTO,
                            CD_ENDERECO = detalle.CD_ENDERECO,
                            NU_PEDIDO = detalle.NU_PEDIDO,
                            QT_PRODUTO = detalle.QT_PRODUTO,
                            CD_FUNCIONARIO = detalle.CD_FUNCIONARIO,
                            CD_FUNC_PICKEO = detalle.CD_FUNC_PICKEO,
                            NU_SEQ_PREPARACION = nuSecuenciaPreparacion,
                            CD_FAIXA = detalle.CD_FAIXA,
                            ID_ESPECIFICA_IDENTIFICADOR = detalle.ID_ESPECIFICA_IDENTIFICADOR,
                            CD_CLIENTE = detalle.CD_CLIENTE,
                            QT_PREPARADO = detalle.QT_PREPARADO,
                            DT_PICKEO = detalle.DT_PICKEO,
                            DT_ADDROW = DateTime.Now,
                            CD_EMPRESA = detalle.CD_EMPRESA,
                            NU_IDENTIFICADOR = detalle.NU_IDENTIFICADOR,
                            NU_CARGA = nuCarga,
                            ID_AGRUPACION = detalle.ID_AGRUPACION,
                            NU_CONTENEDOR_PICKEO = detalle.NU_CONTENEDOR_PICKEO,
                            DT_UPDROW = DateTime.Now,
                            NU_TRANSACCION = nuTransaccion,
                            ND_ESTADO = Mappers.Constants.EstadoDetallePreparacion.ESTADO_PREPARADO,
                            CD_FORNECEDOR = detalle.CD_FORNECEDOR,
                            DT_FABRICACAO_PICKEO = detalle.DT_FABRICACAO_PICKEO,
                            DT_SEPARACION = detalle.DT_SEPARACION,
                            FL_CANCELADO = detalle.FL_CANCELADO,
                            FL_ERROR_CONTROL = detalle.FL_ERROR_CONTROL,
                            ID_AREA_AVERIA = detalle.ID_AREA_AVERIA,
                            ID_AVERIA_PICKEO = detalle.ID_AVERIA_PICKEO,
                            NU_CONTENEDOR_SYS = detalle.NU_CONTENEDOR_SYS,
                            QT_CONTROL = detalle.QT_CONTROL,
                            QT_CONTROLADO = detalle.QT_CONTROLADO,
                            QT_PICKEO = detalle.QT_PICKEO,
                            VL_ESTADO_REFERENCIA = detalle.VL_ESTADO_REFERENCIA
                        };

                        _context.Remove(detalle);
                        _context.Add(newDetPicking);
                        _context.SaveChanges();
                    }
                    else
                    {
                        detalle.NU_CARGA = nuCarga;
                        detalle.NU_CONTENEDOR = nuContenedorDestino;
                        detalle.CD_FUNCIONARIO = _userId;
                        detalle.DT_UPDROW = DateTime.Now;
                        detalle.NU_SEQ_PREPARACION = nuSecuenciaPreparacion;
                        detalle.NU_TRANSACCION = nuTransaccion;
                    }
                }
                else
                {
                    qtNuevoRegistro = cantidad - qtTotalTransferido;

                    var detalle = _context.T_DET_PICKING
                        .FirstOrDefault(x => x.CD_PRODUTO == detPicking.CD_PRODUTO
                            && x.CD_EMPRESA == detPicking.CD_EMPRESA
                            && x.NU_IDENTIFICADOR == detPicking.NU_IDENTIFICADOR
                            && x.CD_FAIXA == detPicking.CD_FAIXA
                            && x.NU_PREPARACION == detPicking.NU_PREPARACION
                            && x.NU_PEDIDO == detPicking.NU_PEDIDO
                            && x.CD_CLIENTE == detPicking.CD_CLIENTE
                            && x.CD_ENDERECO == detPicking.CD_ENDERECO
                            && x.NU_SEQ_PREPARACION == detPicking.NU_SEQ_PREPARACION);

                    detalle.QT_PRODUTO = detalle.QT_PRODUTO - qtNuevoRegistro;
                    detalle.QT_PREPARADO = detalle.QT_PREPARADO - qtNuevoRegistro;
                    detalle.DT_UPDROW = DateTime.Now;
                    detalle.NU_TRANSACCION = nuTransaccion;

                    _context.T_DET_PICKING.Add(new T_DET_PICKING()
                    {
                        NU_PREPARACION = nuPreparacionDestino,
                        NU_CONTENEDOR = nuContenedorDestino,
                        CD_PRODUTO = detPicking.CD_PRODUTO,
                        CD_ENDERECO = detPicking.CD_ENDERECO,
                        NU_PEDIDO = detPicking.NU_PEDIDO,
                        QT_PRODUTO = qtNuevoRegistro,
                        CD_FUNCIONARIO = _userId,
                        CD_FUNC_PICKEO = detPicking.CD_FUNC_PICKEO,
                        NU_SEQ_PREPARACION = _context.GetNextSequenceValueInt(_dapper, Secuencias.S_DET_PICKING),
                        CD_FAIXA = detPicking.CD_FAIXA,
                        ID_ESPECIFICA_IDENTIFICADOR = detPicking.ID_ESPECIFICA_IDENTIFICADOR,
                        CD_CLIENTE = detPicking.CD_CLIENTE,
                        QT_PREPARADO = qtNuevoRegistro,
                        QT_PICKEO = qtNuevoRegistro,
                        DT_PICKEO = detPicking.DT_PICKEO,
                        DT_ADDROW = DateTime.Now,
                        CD_EMPRESA = detPicking.CD_EMPRESA,
                        NU_IDENTIFICADOR = detPicking.NU_IDENTIFICADOR,
                        NU_CARGA = nuCarga,
                        ID_AGRUPACION = detPicking.ID_AGRUPACION,
                        NU_CONTENEDOR_PICKEO = detPicking.NU_CONTENEDOR_PICKEO,
                        DT_UPDROW = DateTime.Now,
                        NU_TRANSACCION = nuTransaccion,
                        ND_ESTADO = Mappers.Constants.EstadoDetallePreparacion.ESTADO_PREPARADO
                    });
                }

                qtTotalTransferido = qtTotalTransferido + (detPicking.QT_PREPARADO ?? 0);
            }

            _context.SaveChanges();
            CheckDeleteContenedor(nuContenedorOrigen, nuPreparacionOrigen, out isContenedorOrigenVacio);
        }

        public virtual void MoverStockEmpaquetado(string ubicacionOrigen, string cdProducto, decimal cdFaxia, int empresa, string nuIdentificador, string ubicacionDestino, decimal cantidad, long nuTransaccion, out bool result)
        {
            result = true;
            if (ubicacionOrigen != ubicacionDestino)
            {
                //***********Resto stock en el origen***********
                string updateStockOrigenQuery = @"
                    UPDATE T_STOCK
                    SET QT_ESTOQUE =  QT_ESTOQUE - :qtestoque,
                        QT_RESERVA_SAIDA = QT_RESERVA_SAIDA - :cantidad,
                        DT_UPDROW = :fechaActualizado,
                        NU_TRANSACCION = :nuTransaccion
                     WHERE CD_ENDERECO = :ubicacionOrigen
                        AND CD_PRODUTO = :cdProducto
                        AND CD_FAIXA = :cdFaxia
                        AND NU_IDENTIFICADOR = :nuIdentificador 
                        AND CD_EMPRESA = :empresa ";

                var parameters = new
                {
                    qtestoque = cantidad,
                    cantidad = cantidad,
                    fechaActualizado = DateTime.Now,
                    nuTransaccion = nuTransaccion,
                    ubicacionOrigen = ubicacionOrigen,
                    cdProducto = cdProducto,
                    cdFaxia = cdFaxia,
                    empresa = empresa,
                    nuIdentificador = nuIdentificador
                };

                var connection = _context.Database.GetDbConnection();
                var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

                var queryOrigenResult = _dapper.Execute(connection, updateStockOrigenQuery, parameters, transaction: tran);

                if (queryOrigenResult == 0)
                    result = false;

                //***********Agrego stock en el destino***********
                string updateStockDestinoQuery = @"
                    UPDATE T_STOCK
                    SET QT_ESTOQUE = QT_ESTOQUE + :cantidad,
                        QT_RESERVA_SAIDA =  QT_RESERVA_SAIDA + :cantidad,
                        DT_UPDROW = :fechaActualizado,
                        NU_TRANSACCION = :nuTransaccion
                     WHERE CD_ENDERECO = :ubicacionDestino
                        AND CD_PRODUTO = :cdProducto
                        AND CD_FAIXA = :cdFaxia
                        AND NU_IDENTIFICADOR =  :nuIdentificador
                        AND CD_EMPRESA = :empresa ";

                var queryDestinoResult = _dapper.Execute(connection, updateStockDestinoQuery, new
                {
                    cantidad = cantidad,
                    fechaActualizado = DateTime.Now,
                    nuTransaccion = nuTransaccion,
                    ubicacionDestino = ubicacionDestino,
                    cdProducto = cdProducto,
                    cdFaxia = cdFaxia,
                    empresa = empresa,
                    nuIdentificador = nuIdentificador
                }, transaction: tran);

                if (queryDestinoResult == 0)
                {
                    string addStockDestinoQuery = @"
                        INSERT INTO T_STOCK (
                            cd_endereco,			   cd_empresa,			cd_produto,
                            cd_faixa,				   nu_identificador, 	dt_fabricacao,
                            qt_transito_entrada,      qt_reserva_saida,	qt_estoque ,
                            id_averia,				   id_inventario,		id_ctrl_calidad,
                            dt_inventario,	      	   dt_updrow,           NU_TRANSACCION)	
                        VALUES (
                            :ubicacionDestino,        :empresa,			:cdProducto,
                            :cdFaxia,			       :nuIdentificador,    null,
                            :qtTransitoEntrada,	   :cantidad,           :cantidad,
                            :idAveria,                :idInventario,       :idCtrlCalidad,
                            :fechaInventario, 		   :fechaActualizado,   :nuTransaccion
                        )";

                    _dapper.Execute(connection, addStockDestinoQuery, new
                    {
                        ubicacionDestino = ubicacionDestino,
                        cdFaxia = cdFaxia,
                        qtTransitoEntrada = 0,
                        idAveria = "N",
                        fechaInventario = DateTime.Now,
                        empresa = empresa,
                        nuIdentificador = nuIdentificador,
                        cantidad = cantidad,
                        idInventario = "R",
                        fechaActualizado = DateTime.Now,
                        cdProducto = cdProducto,
                        idCtrlCalidad = "C",
                        nuTransaccion = nuTransaccion
                    }, transaction: tran);
                }
            }
        }

        public virtual bool FacturarCamion(IUnitOfWork uow, int cdCamion, Logger logger, out Error error, bool ignorarSinLiberar = true, bool ignorarSinPickear = true, bool ignorarSinEmpaquetar = true)
        {
            logger.Debug($"FacturarCamion - Inicio");

            error = null;

            var camion = _context.T_CAMION.FirstOrDefault(x => x.CD_CAMION == cdCamion);

            if (camion.DT_FACTURACION != null || camion.NU_INTERFAZ_EJECUCION_FACT != null)
            {
                error = new Error("EXP110SelecProd_form_Msg_CamionFacturado");
                return false;
            }

            if (!_context.T_CLIENTE_CAMION.Any(x => x.CD_CAMION == cdCamion))
            {
                error = new Error("EXP110SelecProd_form_Msg_CamionVacio");
                return false;
            }

            FacturarPedidosPermitidos(cdCamion, logger, out error);

            if (error != null)
                return false;

            var sql = @"
                SELECT
                    MAX(COALESCE(TE.FL_EMPAQUETA_CONTENEDOR,'N'))
                FROM   
                    T_CLIENTE_CAMION cc,
                    T_PEDIDO_SAIDA ps,
                    T_DET_PICKING dp,
                    T_TIPO_EXPEDICION te
                 WHERE ps.NU_PEDIDO = dp.NU_PEDIDO
                    AND ps.CD_CLIENTE = dp.CD_CLIENTE
                    AND ps.CD_EMPRESA = dp.CD_EMPRESA
                    AND CC.CD_CLIENTE = PS.CD_CLIENTE
                    AND CC.CD_EMPRESA = PS.CD_EMPRESA
                    AND cc.NU_CARGA = dp.NU_CARGA
                    AND cc.CD_CAMION = :cdCamion
                    AND te.TP_EXPEDICION = ps.TP_EXPEDICION
                    AND te.FL_PERMITE_FACTURACION_PARCIAL = 'S'
                 GROUP BY 
                te.FL_EMPAQUETA_CONTENEDOR";

            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var empaquetaContenedor = _dapper.Query<string>(connection, sql, new
            {
                cdCamion = cdCamion
            }, transaction: tran).FirstOrDefault();

            logger.Debug($"FacturarCamion - Inicio validaciones Camión: {cdCamion}");

            if (!ignorarSinLiberar && !IsCamionTodoLiberado(cdCamion))
            {
                error = new Error("EXP110SelecProd_form_Msg_MercaderiaSinLiberar");
                return false;
            }
            else if (!ignorarSinPickear && !IsCamionTodoPickeado(cdCamion))
            {
                error = new Error("EXP110SelecProd_form_Msg_MercaderiaSinPickear");
                return false;
            }
            else if ((!ignorarSinEmpaquetar || empaquetaContenedor == "S") && !IsCamionTodoEmpaquetado(cdCamion))
            {
                error = new Error("EXP110SelecProd_form_Msg_MercaderiaSinEmpaquetar");
                return false;
            }

            logger.Debug($"FacturarCamion - Finaliza validaciones correctamente.");

            logger.Debug($"FacturarCamion - Inicio update camión");

            sql = @"
                UPDATE T_CAMION
                SET DT_FACTURACION = :fechaActual,                                            
                    NU_INTERFAZ_EJECUCION_FACT = -1,
                    NU_TRANSACCION = :nroTransaccion
                WHERE CD_CAMION = :cdCamion";

            _dapper.Execute(connection, sql, new
            {
                fechaActual = DateTime.Now,
                nroTransaccion = uow.GetTransactionNumber(),
                cdCamion = cdCamion
            }, transaction: tran);

            uow.SaveChanges();

            logger.Debug($"FacturarCamion - Fin update camión");

            logger.Debug($"FacturarCamion - Inicio update contenedores");

            sql = @"
                SELECT 
                    DP.NU_CONTENEDOR as Numero,
                    DP.NU_PREPARACION as NumeroPreparacion
                FROM
                    T_DET_PICKING DP,
                    T_CONTENEDOR CON,
		            T_CLIENTE_CAMION CC
		        WHERE DP.NU_PREPARACION = CON.NU_PREPARACION
                    AND DP.NU_CONTENEDOR = CON.NU_CONTENEDOR
                    AND DP.NU_CARGA = CC.NU_CARGA
		            AND DP.CD_CLIENTE = CC.CD_CLIENTE
		            AND DP.CD_EMPRESA = CC.CD_EMPRESA
                    AND CON.CD_CAMION_FACTURADO IS NULL
		            AND CC.CD_CAMION = :cdCamion                                              
		        GROUP BY 
                    DP.NU_CONTENEDOR,
                    DP.NU_PREPARACION";

            var contenedores = _dapper.Query<Contenedor>(connection, sql, new
            {
                cdCamion = cdCamion,
                nroTransaccion = uow.GetTransactionNumber()
            }, transaction: tran)
            .Select(c=> new {
                cdCamion = cdCamion,
                nroTransaccion = uow.GetTransactionNumber(),
                nuContenedor = c.Numero,
                nuPreparacion = c.NumeroPreparacion
            });

            logger.Debug($"FacturarCamion - Cantidad de contenedores a actualizar : {contenedores.Count()}");
            
            sql = @"UPDATE T_CONTENEDOR
                    SET CD_CAMION_FACTURADO = :cdCamion,
                        NU_TRANSACCION = :nroTransaccion
                    WHERE NU_CONTENEDOR  = :nuContenedor
                        AND NU_PREPARACION = :nuPreparacion";

            _dapper.Execute(connection, sql, contenedores, transaction: tran);

            logger.Debug($"FacturarCamion - Fin update contenedores");

            uow.SaveChanges();

            logger.Debug($"FacturarCamion - Fin");

            return true;
        }

        public virtual void FacturarPedidosPermitidos(int cdCamion, Logger logger, out Error error)
        {
            logger.Debug($"FacturarPedidosPermitidos - Inicio");

            error = null;

            var estadosAnulacion = EstadoDetallePreparacion.GetEstadosAnulacion();

            var pedidos = _context.T_DET_PICKING
                .Where(dp => !estadosAnulacion.Contains(dp.ND_ESTADO))
                .Join(_context.T_PEDIDO_SAIDA,
                    dp => new { dp.NU_PEDIDO, dp.CD_CLIENTE, dp.CD_EMPRESA },
                    ps => new { ps.NU_PEDIDO, ps.CD_CLIENTE, ps.CD_EMPRESA },
                    (dp, ps) => new { DetallePicking = dp, Pedido = ps })
                .Join(_context.T_TIPO_EXPEDICION.Where(te => te.FL_PERMITE_FACTURACION_PARCIAL == "N"),
                    dpps => new { dpps.Pedido.TP_EXPEDICION },
                    te => new { te.TP_EXPEDICION },
                    (dpps, te) => new { DetallePicking = dpps.DetallePicking, Pedido = dpps.Pedido, TipoExpedicion = te })
                .Join(_context.T_CLIENTE_CAMION.Where(cc => cc.CD_CAMION == cdCamion),
                    dppste => new { dppste.DetallePicking.CD_CLIENTE, dppste.DetallePicking.CD_EMPRESA, NU_CARGA = (dppste.DetallePicking.NU_CARGA ?? -1) },
                    cc => new { cc.CD_CLIENTE, cc.CD_EMPRESA, cc.NU_CARGA },
                    (dppste, cc) => new { DetallePicking = dppste.DetallePicking, Pedido = dppste.Pedido, TipoExpedicion = dppste.TipoExpedicion, ClienteCamion = cc })
                .AsNoTracking()
                .GroupBy(x => new
                {
                    x.Pedido.CD_EMPRESA,
                    x.Pedido.CD_CLIENTE,
                    x.Pedido.NU_PEDIDO,
                    x.TipoExpedicion.FL_EMPAQUETA_CONTENEDOR,
                    x.TipoExpedicion.FL_PERMITE_FACTURACION_PARCIAL,
                })
                .Select(g => new FacturarPedidosPermitidos
                {
                    Empresa = g.Key.CD_EMPRESA,
                    Cliente = g.Key.CD_CLIENTE,
                    Pedido = g.Key.NU_PEDIDO,
                    EmpaquetaContenedor = (g.Key.FL_EMPAQUETA_CONTENEDOR == "S"),
                    PermiteFacturacionParcial = (g.Key.FL_PERMITE_FACTURACION_PARCIAL == "S"),
                })
                .ToList();

            logger.Debug($"FacturarPedidosPermitidos - Cantidad de pedidos a facturar: {pedidos.Count()}");

            Error msg = null;
            foreach (var pedido in pedidos)
            {
                logger.Debug($"FacturarPedidosPermitidos - Inicio validaciones Pedido: {pedido.Pedido.Count()} Cliente: {pedido.Cliente} Empresa: {pedido.Empresa}");

                if (!IsPedidoTodoLiberado(pedido.Empresa, pedido.Cliente, pedido.Pedido))
                {
                    msg = new Error("EXP110SelecProd_form_Msg_EgresoPedidoNoAdmiteFacturacionParcial");
                }
                else if (!PedidoTodoPickeado(pedido.Empresa, pedido.Cliente, pedido.Pedido))
                {
                    msg = new Error("EXP110SelecProd_form_Msg_EgresoPedidoNoAdmiteFacturacionParcialMercaderiaSinPreparar");
                }
                else if (!IsPedidoTodoAsignadoCamion(pedido.Empresa, pedido.Cliente, pedido.Pedido, cdCamion))
                {
                    msg = new Error("EXP110SelecProd_form_Msg_EgresoPedidoNoAdmiteFacturacionParcialNoTieneTodasLasCargasAsociadas");
                }
                else if (pedido.EmpaquetaContenedor && !PedidoTodoEmpaquetado(pedido.Empresa, pedido.Cliente, pedido.Pedido))
                {
                    msg = new Error("EXP110SelecProd_form_Msg_EgresoCamionVacio");
                }

                if (msg != null)
                {
                    error = msg;
                    break;
                }

                logger.Debug($"FacturarPedidosPermitidos - Finaliza validaciones correctamente");
            }

            logger.Debug($"FacturarPedidosPermitidos - Fin");
        }

        #endregion
    }
}
