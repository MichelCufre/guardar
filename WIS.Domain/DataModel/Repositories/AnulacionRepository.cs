using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class AnulacionRepository
    {
        protected WISDB _context;
        protected string _application;
        protected int _userId;
        protected AnulacionMapper _mapper;
        protected readonly IDapper _dapper;
        protected PreparacionRepository _preparacionRepository;
        protected TransaccionRepository _transaccionRepository;

        public AnulacionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            _userId = userId;
            _dapper = dapper;
            _context = context;
            _application = application;
            _mapper = new AnulacionMapper();
            _preparacionRepository = new PreparacionRepository(_context, application, userId, _dapper);
            _transaccionRepository = new TransaccionRepository(_context, application, userId, _dapper);
        }

        #region Any
        public virtual bool AnyAnulacionPendiente(int nuPrep)
        {
            List<string> estados = new List<string> { EstadoAnulacion.EnProceso, EstadoAnulacion.AnulacionPendiente };
            return _context.T_ANULACION_PREPARACION.Any(x => x.NU_PREPARACION == nuPrep && estados.Contains(x.ND_ESTADO));
        }

        #endregion

        #region Get

        #endregion

        #region Add
        public virtual void AddAnulacionPreparacion(AnulacionPreparacion anulacion)
        {
            anulacion.NroAnulacionPreparacion = this._context.GetNextSequenceValueInt(_dapper, "S_NU_ANULACION_PREPARACION");
            anulacion.Estado = EstadoAnulacion.AnulacionPendiente;

            T_ANULACION_PREPARACION anulacionPrep = this._mapper.MapToEntity(anulacion);
            this._context.T_ANULACION_PREPARACION.Add(anulacionPrep);
        }

        #endregion

        #region Update

        #endregion

        #region Remove

        #endregion

        #region Dapper
        public virtual void GenerarDetallesDeAnulacion(DbConnection connection)
        {
            string sql = $@"
                INSERT INTO T_DET_ANULACION_PREPARACION (
                    NU_ANULACION_PREPARACION, 
                    NU_PREPARACION, 
                    CD_CLIENTE, 
                    CD_EMPRESA, 
                    CD_ENDERECO, 
                    NU_PEDIDO, 
                    CD_PRODUTO, 
                    CD_FAIXA, 
                    NU_IDENTIFICADOR, 
                    NU_SEQ_PREPARACION, 
                    DT_ADDROW, 
                    DT_UPDROW)
            SELECT 
                dp.NU_ANULACION_PREPARACION, 
                dp.NU_PREPARACION, 
                dp.CD_CLIENTE, 
                dp.CD_EMPRESA, 
                dp.CD_ENDERECO, 
                dp.NU_PEDIDO, 
                dp.CD_PRODUTO, 
                dp.CD_FAIXA, 
                dp.NU_IDENTIFICADOR, 
                dp.NU_SEQ_PREPARACION, 
                :FechaModificacion, 
                :FechaModificacion
            FROM V_ANULACION_PREPARACION_PEND dp 
            LEFT JOIN T_DET_ANULACION_PREPARACION dap ON dp.NU_PREPARACION = dap.NU_PREPARACION 
                AND dp.CD_CLIENTE = dap.CD_CLIENTE 
                AND dp.CD_EMPRESA = dap.CD_EMPRESA 
                AND dp.CD_ENDERECO = dap.CD_ENDERECO 
                AND dp.NU_PEDIDO = dap.NU_PEDIDO 
                AND dp.CD_PRODUTO = dap.CD_PRODUTO 
                AND dp.CD_FAIXA = dap.CD_FAIXA 
                AND dp.NU_IDENTIFICADOR = dap.NU_IDENTIFICADOR 
                AND dp.NU_SEQ_PREPARACION = dap.NU_SEQ_PREPARACION
            WHERE (dap.NU_PREPARACION IS NULL 
                AND dap.CD_CLIENTE IS NULL 
                AND dap.CD_EMPRESA IS NULL 
                AND dap.CD_ENDERECO IS NULL 
                AND dap.NU_PEDIDO IS NULL 
                AND dap.CD_PRODUTO IS NULL 
                AND dap.CD_FAIXA IS NULL 
                AND dap.NU_IDENTIFICADOR IS NULL 
                AND dap.NU_SEQ_PREPARACION IS NULL)";

            _dapper.Execute(connection, sql, param: new
            {
                FechaModificacion = DateTime.Now,
            });
        }

        public virtual async Task MarcarPickingParaAnular(List<AnularPickingPedidoPendiente> detalles, IAnularPickingPedidoPendienteContext context, long nuTransaccion, int userId, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);
                var bulkContext = GetBulkOperationContext(detalles, context, connection, nuTransaccion, userId);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertAnulacionPreparaciones(connection, tran, bulkContext.InsertAnulacionPreparacion);
                    await BulkUpdateEstadoPickingPedidoAnulado(connection, tran, bulkContext.UpdatePickingPedidoPendiente);

                    tran.Commit();
                }
            }
        }

        public virtual async Task BulkUpdateEstadoPickingPedidoAnulado(DbConnection connection, DbTransaction tran, List<object> updatePickingPedidoPendiente)
        {
            string sql = @"
                UPDATE T_DET_PICKING 
                SET NU_TRANSACCION = :NuTransaccion, ND_ESTADO = :EstadoAnulacion
                WHERE NU_PREPARACION = :Preparacion 
                    AND NU_PEDIDO = :Pedido
                    AND CD_EMPRESA = :Empresa
                    AND CD_CLIENTE = :Cliente
                    AND ND_ESTADO = :EstadoPicking";

            await _dapper.ExecuteAsync(connection, sql, updatePickingPedidoPendiente, transaction: tran);
        }

        public virtual async Task MarcarPickingParaAnularAutomatismo(List<AnularPickingPedidoPendienteAutomatismo> detalles, AnularPickingPedidoPendienteAutomatismoContext context, long nuTransaccion, int userId, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    var bulkContext = GetBulkOperationContextAutomatismo(detalles, context, connection, tran, nuTransaccion, userId);

                    await BulkInsertAnulacionPreparaciones(connection, tran, bulkContext.InsertAnulacionPreparacion);

                    await BulkUpdateDetallePicking(connection, tran, bulkContext.UpdatePickingPedidoPendiente);
                    await BulkInsertPickingPedidoAnular(connection, tran, bulkContext.InsertPickingPedidoPendienteAnular);

                    tran.Commit();
                }
            }
        }

        public virtual async Task BulkInsertPickingPedidoAnular(DbConnection connection, DbTransaction tran, IEnumerable<object> picks)
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
                               NU_TRANSACCION = :Transaccion,
                               ND_ESTADO = :Estado
                           WHERE NU_PREPARACION = :NumeroPreparacion 
                           AND CD_ENDERECO = :Ubicacion
                           AND NU_PEDIDO = :Pedido
                           AND CD_CLIENTE = :Cliente
                           AND CD_EMPRESA = :Empresa
                           AND CD_PRODUTO = :Producto
                           AND NU_IDENTIFICADOR = :Lote
                           AND CD_FAIXA = :Faixa
                           AND NU_SEQ_PREPARACION = :NumeroSecuencia";

            await _dapper.ExecuteAsync(connection, sql, picks, transaction: tran);
        }

        public virtual async Task BulkInsertAnulacionPreparaciones(DbConnection connection, DbTransaction tran, List<object> insertAnulacionPreparacion)
        {
            string sql = @"
                INSERT INTO T_ANULACION_PREPARACION (NU_ANULACION_PREPARACION, NU_PREPARACION, ND_ESTADO, DS_ANULACION, DT_ADDROW, TP_ANULACION, TP_AGRUPACION, USERID) 
                VALUES (:NroAnulacionPreparacion, :Preparacion, :Estado, :Descripcion, :Alta, :TipoAnulacion, :TipoAgrupacion, :UserId)";

            await _dapper.ExecuteAsync(connection, sql, insertAnulacionPreparacion, transaction: tran);
        }

        public virtual AnularPickingBulkOperationContext GetBulkOperationContext(List<AnularPickingPedidoPendiente> detalles, IAnularPickingPedidoPendienteContext context, DbConnection connection, long nuTransaccion, int userId)
        {
            var bulkContext = new AnularPickingBulkOperationContext();
            var cantidadCabezalAnulacion = detalles.GroupBy(x => x.Preparacion).Select(x => x.Key).Distinct().Count();
            var etiquetaLoteIds = GetNewNroAnulacionPreparacionIds(cantidadCabezalAnulacion, connection);

            foreach (var detalle in detalles)
            {
                detalle.EstadoAnulacion = EstadoDetallePreparacion.ESTADO_ANULACION_PENDIENTE;

                var anulacion = new AnulacionPreparacion()
                {
                    Preparacion = detalle.Preparacion,
                    Alta = DateTime.Now,
                    Modificacion = DateTime.Now,
                    TipoAnulacion = 1,
                    TipoAgrupacion = "TODO",
                    Descripcion = "Anulación Prep. " + detalle.Preparacion,
                    UserId = userId
                };

                var etiquetaLoteId = etiquetaLoteIds.FirstOrDefault();

                anulacion.NroAnulacionPreparacion = etiquetaLoteId;
                anulacion.Estado = EstadoAnulacion.AnulacionPendiente;

                bulkContext.InsertAnulacionPreparacion.Add(anulacion);
                etiquetaLoteIds.Remove(etiquetaLoteId);

                if (context.IsEmpresaDocumental(detalles.FirstOrDefault().Empresa))
                {
                    detalle.EstadoAnulacion = EstadoDetallePreparacion.ESTADO_ANULACION_DOC_PEND;
                }

                bulkContext.UpdatePickingPedidoPendiente.Add(detalle);
            }

            return bulkContext;
        }

        public virtual AnularPickingBulkOperationContext GetBulkOperationContextAutomatismo(List<AnularPickingPedidoPendienteAutomatismo> colAnulaciones, AnularPickingPedidoPendienteAutomatismoContext context, DbConnection connection, DbTransaction tran, long nuTransaccion, int userId)
        {
            var bulkContext = new AnularPickingBulkOperationContext();
            var cantidadCabezalAnulacion = colAnulaciones.GroupBy(x => x.Preparacion).Select(x => x.Key).Distinct().Count();
            var etiquetaLoteIds = GetNewNroAnulacionPreparacionIds(cantidadCabezalAnulacion, connection, tran);

            foreach (var detAnulacion in colAnulaciones)
            {
                //anulacion.EstadoAnulacion = EstadoDetallePreparacion.ESTADO_ANULACION_PENDIENTE;

                var anulacion = new AnulacionPreparacion()
                {
                    Preparacion = detAnulacion.Preparacion,
                    Alta = DateTime.Now,
                    Modificacion = DateTime.Now,
                    TipoAnulacion = 1,
                    TipoAgrupacion = "TODO",
                    Descripcion = "Anulación Prep. AUTOMATISMO",
                    UserId = userId
                };

                var etiquetaLoteId = etiquetaLoteIds.FirstOrDefault();

                anulacion.NroAnulacionPreparacion = etiquetaLoteId;
                anulacion.Estado = EstadoAnulacion.AnulacionPendiente;

                bulkContext.InsertAnulacionPreparacion.Add(anulacion);
                etiquetaLoteIds.Remove(etiquetaLoteId);

                var prep = _preparacionRepository.GetPreparacionPorNumero(detAnulacion.Preparacion);

                var estadoAnulacion = EstadoDetallePreparacion.ESTADO_ANULACION_PENDIENTE;

                if (context.IsEmpresaDocumental(colAnulaciones.FirstOrDefault().Empresa))
                {
                    estadoAnulacion = EstadoDetallePreparacion.ESTADO_ANULACION_DOC_PEND;
                }

                List<DetallePreparacion> detPickingPendientes = new List<DetallePreparacion>();

                switch (prep.Agrupacion)
                {
                    case Agrupacion.Pedido:
                        detPickingPendientes = _preparacionRepository.GetDetallesPreparacionPedidoPendientesAutomatismo(detAnulacion.Preparacion, detAnulacion.Pedido, detAnulacion.TipoAgente, detAnulacion.CodigoAgente, detAnulacion.ComparteContenedorPicking)?.ToList();
                        break;
                    case Agrupacion.Cliente:
                        detPickingPendientes = _preparacionRepository.GetDetallesPreparacionClientePendientesAutomatismo(detAnulacion.Preparacion, detAnulacion.TipoAgente, detAnulacion.CodigoAgente, detAnulacion.ComparteContenedorPicking)?.ToList();
                        break;
                    case Agrupacion.Ruta:
                        detPickingPendientes = _preparacionRepository.GetDetallesPreparacionRutaPendientesAutomatismo(detAnulacion.Preparacion, detAnulacion.Carga, detAnulacion.ComparteContenedorPicking)?.ToList();
                        break;
                    case Agrupacion.Onda:
                        detPickingPendientes = _preparacionRepository.GetDetallesPreparacionOndaPendientesAutomatismo(detAnulacion.Preparacion, detAnulacion.ComparteContenedorPicking)?.ToList();
                        break;
                }


                foreach (var prod in detAnulacion.Detalle)
                {

                    foreach (var prodPicking in detPickingPendientes.Where(W => W.Producto == prod.CodigoProducto && prod.CantidadAnular > 0))
                    {

                        if (prod.CantidadAnular >= prodPicking.Cantidad) //Anula la linea
                        {
                            prodPicking.Estado = estadoAnulacion;
                            bulkContext.UpdatePickingPedidoPendiente.Add(prodPicking);

                            prod.CantidadAnular -= prodPicking.Cantidad;
                        }
                        else //Parte la linea
                        {
                            var newDetPick = _preparacionRepository.GetNewDetallePickingObject(prodPicking);
                            newDetPick.Cantidad = prod.CantidadAnular;
                            newDetPick.Estado = estadoAnulacion;
                            newDetPick.NumeroSecuencia = _preparacionRepository.GetSecuenciasDetallePicking(1, connection, tran).FirstOrDefault();
                            bulkContext.InsertPickingPedidoPendienteAnular.Add(newDetPick);

                            prodPicking.Cantidad -= prod.CantidadAnular;
                            bulkContext.UpdatePickingPedidoPendiente.Add(prodPicking);

                            prod.CantidadAnular -= prod.CantidadAnular;
                        }
                    }

                }
            }

            return bulkContext;
        }

        public virtual List<int> GetNewNroAnulacionPreparacionIds(int count, DbConnection connection, DbTransaction transaction = null)
        {
            return _dapper.GetNextSequenceValues<int>(connection, "S_NU_ANULACION_PREPARACION", count, transaction).ToList();
        }

        //Get
        public virtual List<AnulacionPreparacion> GetAnulacionesPendientes(DbConnection connection)
        {
            string sql = GetSqlSelectAnulaciones();
            return _dapper.Query<AnulacionPreparacion>(connection, sql, commandType: CommandType.Text).ToList();
        }

        public static string GetSqlSelectAnulaciones()
        {
            return $@"SELECT 
                        ap.NU_ANULACION_PREPARACION as NroAnulacionPreparacion,
                        ap.NU_PREPARACION as Preparacion,
                        ap.ND_ESTADO as Estado,
                        ap.DS_ANULACION as Descripcion,
                        ap.TP_ANULACION as TipoAnulacion,
                        ap.TP_AGRUPACION as TipoAgrupacion,
                        ap.USERID as UserId,
                        ap.DT_ADDROW as Alta,
                        ap.DT_UPDROW as Modificacion    
                    FROM T_ANULACION_PREPARACION ap 
                    INNER JOIN V_ANULACION_PREPARACION_PEND app ON app.NU_ANULACION_PREPARACION = ap.NU_ANULACION_PREPARACION
                    GROUP BY 
                        ap.NU_ANULACION_PREPARACION,
                        ap.NU_PREPARACION,
                        ap.ND_ESTADO,
                        ap.DS_ANULACION,
                        ap.TP_ANULACION,
                        ap.TP_AGRUPACION,
                        ap.USERID,
                        ap.DT_ADDROW,
                        ap.DT_UPDROW
                    ORDER BY ap.NU_ANULACION_PREPARACION";
        }

        public virtual List<int> GetAnulacionesPendientes(DbConnection connection, DbTransaction tran, List<Preparacion> preparaciones)
        {
            IEnumerable<int> resultado = new List<int>();
            List<string> estados = new List<string> { EstadoAnulacion.EnProceso, EstadoAnulacion.AnulacionPendiente };

            _dapper.BulkInsert(connection, tran, preparaciones, "T_PICKING_TEMP", new Dictionary<string, Func<Preparacion, ColumnInfo>>
            {
                { "NU_PREPARACION", x => new ColumnInfo(x.Id)}
            });

            string sql = @"SELECT 
                        P.NU_PREPARACION 
                        FROM T_ANULACION_PREPARACION P 
                        INNER JOIN T_PICKING_TEMP T ON P.NU_PREPARACION = T.NU_PREPARACION
                        WHERE P.ND_ESTADO in ('SANUEPR','SANUPEN')";

            resultado = _dapper.Query<int>(connection, sql, transaction: tran);

            _dapper.BulkDelete(connection, tran, preparaciones, "T_PICKING_TEMP", new Dictionary<string, Func<Preparacion, object>>
            {
                { "NU_PREPARACION", x => x.Id}
            });

            return resultado.ToList();
        }

        public virtual List<int> GetAnulacionesEnTraspaso(DbConnection connection, DbTransaction tran, List<Preparacion> preparaciones)
        {
            IEnumerable<int> resultado = new List<int>();
            List<string> estados = new List<string> { EstadoAnulacion.EnProceso, EstadoAnulacion.AnulacionPendiente };

            _dapper.BulkInsert(connection, tran, preparaciones, "T_PICKING_TEMP", new Dictionary<string, Func<Preparacion, ColumnInfo>>
            {
                { "NU_PREPARACION", x => new ColumnInfo(x.Id)}
            });

            string sql = @"SELECT 
                        TR.NU_PREPARACION 
                        FROM T_TRASPASO TR
                        INNER JOIN T_PICKING_TEMP T ON TR.NU_PREPARACION = T.NU_PREPARACION
                        WHERE TR.ID_ESTADO in ('ESTRASP_EN_EDICION','ESTRASP_EN_PROCESO')";

            resultado = _dapper.Query<int>(connection, sql, transaction: tran);

            _dapper.BulkDelete(connection, tran, preparaciones, "T_PICKING_TEMP", new Dictionary<string, Func<Preparacion, object>>
            {
                { "NU_PREPARACION", x => x.Id}
            });

            return resultado.ToList();
        }

        public virtual List<AnulacionPreparacionDetalle> GetDetallesPendientes(int nroAnulacion, int preparacion, DbConnection connection)
        {
            string sql = GetSqlSelectDetallesAnulacion();
            return _dapper.Query<AnulacionPreparacionDetalle>(connection, sql, new
            {
                nroAnulacion = nroAnulacion,
                preparacion = preparacion
            }, commandType: CommandType.Text).ToList();
        }

        public static string GetSqlSelectDetallesAnulacion()
        {
            return @"SELECT 
                    ap.NU_ANULACION_PREPARACION as NroAnulacionPreparacion,
                    ap.TP_ANULACION as TipoAnulacion,
                    ap.TP_AGRUPACION as TipoAgrupacion,
                    ap.ND_ESTADO_ANULACION as EstadoAnulacion,
                    ap.USERID_ANULACION as UserIdAnulacion,
    
                    ap.NU_PREPARACION as NumeroPreparacion,
                    ap.CD_CLIENTE as Cliente,
                    ap.CD_EMPRESA as Empresa,
                    ap.CD_ENDERECO as Ubicacion,
                    ap.CD_FAIXA as Faixa,
                    ap.CD_FORNECEDOR as Proveedor,
                    ap.CD_FUNCIONARIO as Usuario,
                    ap.CD_FUNC_PICKEO as UsuarioPickeo,
                    ap.CD_PRODUTO as Producto,
                    ap.DT_ADDROW as FechaAlta,
                    ap.DT_FABRICACAO_PICKEO as VencimientoPickeo,
                    ap.DT_PICKEO as FechaPickeo,
                    ap.DT_SEPARACION as FechaSeparacion,
                    ap.DT_UPDROW as FechaModificacion,
                    ap.FL_CANCELADO as CanceladoId,
                    ap.FL_ERROR_CONTROL as ErrorControl,
                    ap.ID_AGRUPACION as Agrupacion,
                    ap.ID_AREA_AVERIA as AreaAveria,
                    ap.ID_AVERIA_PICKEO as AveriaPickeo,
                    ap.ID_ESPECIFICA_IDENTIFICADOR as EspecificaLote,
                    ap.ND_ESTADO_DETALLE_PICKING as EstadoDetPicking,
                    ap.NU_CARGA as Carga,
                    ap.NU_CONTENEDOR as NroContenedor,
                    ap.NU_CONTENEDOR_PICKEO as NumeroContenedorPickeo,
                    ap.NU_CONTENEDOR_SYS as NumeroContenedorSys,
                    ap.NU_IDENTIFICADOR as Lote,
                    ap.NU_PEDIDO as Pedido,
                    ap.NU_SEQ_PREPARACION as NumeroSecuencia,
                    ap.NU_TRANSACCION as Transaccion,
                    ap.QT_CONTROL as CantidadControl,
                    ap.QT_CONTROLADO as CantidadControlada,
                    ap.QT_PICKEO as CantidadPickeo,
                    ap.QT_PREPARADO as CantidadPreparada,
                    ap.QT_PRODUTO as Cantidad,
                    ap.VL_ESTADO_REFERENCIA as ReferenciaEstado,
                    ap.TP_ARMADO_EGRESO as TipoArmadoEgreso,
                    ap.FL_FACTURA_AUTO_COMPLETAR as FacturaAutoCompletar,
                    ap.ID_DET_PICKING_LPN as IdDetallePickingLpn
                FROM V_ANULACION_PREPARACION_PEND ap
                WHERE ap.NU_ANULACION_PREPARACION = :nroAnulacion 
                AND ap.NU_PREPARACION = :preparacion
                ORDER BY ap.NU_PEDIDO";
        }

        public virtual DetallePedido GetDetalle(AnulacionPreparacionDetalle detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                        NU_PEDIDO as Id,
                        CD_CLIENTE as Cliente,
                        CD_EMPRESA as Empresa,
                        CD_FAIXA as Faixa,
                        CD_PRODUTO as Producto,
                        DS_MEMO as Memo,
                        DT_ADDROW as FechaAlta,
                        DT_GENERICO_1 as FechaGenerica_1,
                        DT_UPDROW as FechaModificacion,
                        ID_AGRUPACION as Agrupacion,
                        ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificadorId,
                        NU_GENERICO_1 as NuGenerico_1,
                        NU_IDENTIFICADOR as Identificador,
                        NU_TRANSACCION as Transaccion,
                        QT_ABASTECIDO as CantidadAbastecida,
                        QT_ANULADO as CantidadAnulada,
                        QT_ANULADO_FACTURA as CantidadAnuladaFactura,
                        QT_CARGADO as CantidadCargada,
                        QT_CONTROLADO as CantidadControlada,
                        QT_CROSS_DOCK as CantidadCrossDocking,
                        QT_EXPEDIDO as CantidadExpedida,
                        QT_FACTURADO as CantidadFacturada,
                        QT_LIBERADO as CantidadLiberada,
                        QT_PEDIDO as Cantidad,
                        QT_PEDIDO_ORIGINAL as CantidadOriginal,
                        QT_PREPARADO as CantidadPreparada,
                        QT_TRANSFERIDO as CantidadTransferida,
                        QT_UND_ASOCIADO_CAMION as CantUndAsociadoCamion,
                        VL_GENERICO_1 as VlGenerico_1,
                        VL_PORCENTAJE_TOLERANCIA as PorcentajeTolerancia,
                        VL_SERIALIZADO_1 as DatosSerializados
                    FROM T_DET_PEDIDO_SAIDA
                    WHERE NU_PEDIDO = :Pedido AND CD_EMPRESA = :Empresa AND CD_CLIENTE = :Cliente 
                    AND CD_PRODUTO = :Producto AND CD_FAIXA = :Faixa AND NU_IDENTIFICADOR= :Lote AND ID_ESPECIFICA_IDENTIFICADOR = :EspecificaLote ";

            return _dapper.Query<DetallePedido>(connection, sql, detalle, transaction: tran, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual Pedido GetPedido(AnulacionPreparacionDetalle detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            NU_PEDIDO as Id,
                            CD_CLIENTE as Cliente,
                            CD_EMPRESA as Empresa,
                            CD_CONDICION_LIBERACION as CondicionLiberacion,
                            CD_FUN_RESP as FuncionarioResponsable,
                            CD_ORIGEN as Origen,
                            CD_PUNTO_ENTREGA as PuntoEntrega,
                            CD_ROTA as Ruta,
                            CD_SITUACAO as Estado,
                            CD_TRANSPORTADORA as CodigoTransportadora,
                            CD_UF as CodigoUF,
                            CD_ZONA as Zona,
                            DS_ANEXO1 as Anexo,
                            DS_ANEXO2 as Anexo2,
                            DS_ANEXO3 as Anexo3,
                            DS_ANEXO4 as Anexo4,
                            DS_ENDERECO as DireccionEntrega,
                            DS_MEMO as Memo,
                            DS_MEMO_1 as Memo1,
                            DT_ADDROW as FechaAlta,
                            DT_EMITIDO as FechaEmision,
                            DT_ENTREGA as FechaEntrega,
                            DT_FUN_RESP as FechaFuncionarioResponsable,
                            DT_GENERICO_1 as FechaGenerica_1,
                            DT_LIBERAR_DESDE as FechaLiberarDesde,
                            DT_LIBERAR_HASTA as FechaLiberarHasta,
                            DT_ULT_PREPARACION as FechaUltimaPreparacion,
                            DT_UPDROW as FechaModificacion,
                            FL_SYNC_REALIZADA as SincronizacionRealizadaId,
                            ID_AGRUPACION as Agrupacion,
                            ID_MANUAL as ManualId,
                            ND_ACTIVIDAD as Actividad,
                            NU_GENERICO_1 as NuGenerico_1,
                            NU_INTERFAZ_FACTURACION as NroIntzFacturacion,
                            NU_ORDEN_ENTREGA as OrdenEntrega,
                            NU_ORDEN_LIBERACION as NumeroOrdenLiberacion,
                            NU_PRDC_INGRESO as IngresoProduccion,
                            NU_PREDIO as Predio,
                            NU_PREPARACION_MANUAL as NroPrepManual,
                            NU_PREPARACION_PROGRAMADA as PreparacionProgramada,
                            NU_TRANSACCION as Transaccion,
                            NU_ULT_PREPARACION as NumeroUltimaPreparacion,
                            TP_EXPEDICION as TipoExpedicionId,
                            TP_PEDIDO as Tipo,
                            VL_COMPARTE_CONTENEDOR_ENTREGA as ComparteContenedorEntrega,
                            VL_COMPARTE_CONTENEDOR_PICKING as ComparteContenedorPicking,
                            VL_GENERICO_1 as VlGenerico_1,
                            VL_SERIALIZADO_1 as VlSerealizado_1,
                            NU_CARGA as NuCarga,
                            NU_TELEFONE as Telefono,
                            NU_TELEFONE2 as TelefonoSecundario,
                            VL_LONGITUD as Longitud,
                            VL_LATITUD as Latitud
                    FROM T_PEDIDO_SAIDA
                    WHERE NU_PEDIDO = :Pedido AND CD_EMPRESA = :Empresa AND CD_CLIENTE = :Cliente";

            return _dapper.Query<Pedido>(connection, sql, detalle, transaction: tran, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual DetallePedido GetDetalleAUTO(AnulacionPreparacionDetalle detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = $@"SELECT 
                        NU_PEDIDO as Id,
                        CD_CLIENTE as Cliente,
                        CD_EMPRESA as Empresa,
                        CD_FAIXA as Faixa,
                        CD_PRODUTO as Producto,
                        DS_MEMO as Memo,
                        DT_ADDROW as FechaAlta,
                        DT_GENERICO_1 as FechaGenerica_1,
                        DT_UPDROW as FechaModificacion,
                        ID_AGRUPACION as Agrupacion,
                        ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificadorId,
                        NU_GENERICO_1 as NuGenerico_1,
                        NU_IDENTIFICADOR as Identificador,
                        NU_TRANSACCION as Transaccion,
                        QT_ABASTECIDO as CantidadAbastecida,
                        QT_ANULADO as CantidadAnulada,
                        QT_ANULADO_FACTURA as CantidadAnuladaFactura,
                        QT_CARGADO as CantidadCargada,
                        QT_CONTROLADO as CantidadControlada,
                        QT_CROSS_DOCK as CantidadCrossDocking,
                        QT_EXPEDIDO as CantidadExpedida,
                        QT_FACTURADO as CantidadFacturada,
                        QT_LIBERADO as CantidadLiberada,
                        QT_PEDIDO as Cantidad,
                        QT_PEDIDO_ORIGINAL as CantidadOriginal,
                        QT_PREPARADO as CantidadPreparada,
                        QT_TRANSFERIDO as CantidadTransferida,
                        QT_UND_ASOCIADO_CAMION as CantUndAsociadoCamion,
                        VL_GENERICO_1 as VlGenerico_1,
                        VL_PORCENTAJE_TOLERANCIA as PorcentajeTolerancia,
                        VL_SERIALIZADO_1 as DatosSerializados
                    FROM T_DET_PEDIDO_SAIDA
                    WHERE NU_PEDIDO = :Pedido AND CD_EMPRESA = :Empresa AND CD_CLIENTE = :Cliente 
                    AND CD_PRODUTO = :Producto AND CD_FAIXA = :Faixa AND NU_IDENTIFICADOR= '{ManejoIdentificadorDb.IdentificadorAuto}' AND ID_ESPECIFICA_IDENTIFICADOR = :EspecificaLote ";

            return _dapper.Query<DetallePedido>(connection, sql, detalle, transaction: tran, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual decimal GetCantidadProducto(Stock stock, DbTransaction tran, DbConnection connection)
        {
            string sql = @"SELECT COALESCE(SUM(QT_PRODUTO),0) FROM T_DET_PICKING 
                         WHERE CD_ENDERECO = :Ubicacion 
                         AND CD_EMPRESA = :Empresa 
                         AND CD_PRODUTO = :Producto 
                         AND CD_FAIXA = :Faixa 
                         AND NU_IDENTIFICADOR = :Identificador
                         AND QT_PREPARADO IS NULL
                         GROUP BY CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR";

            return _dapper.Query<decimal>(connection, sql, stock, CommandType.Text, tran).FirstOrDefault();
        }

        public virtual List<DetallePedido> GetDetallesAsociados(AnulacionPreparacionDetalle detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                        NU_PEDIDO as Id,
                        CD_CLIENTE as Cliente,
                        CD_EMPRESA as Empresa,
                        CD_FAIXA as Faixa,
                        CD_PRODUTO as Producto,
                        DS_MEMO as Memo,
                        DT_ADDROW as FechaAlta,
                        DT_GENERICO_1 as FechaGenerica_1,
                        DT_UPDROW as FechaModificacion,
                        ID_AGRUPACION as Agrupacion,
                        ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificadorId,
                        NU_GENERICO_1 as NuGenerico_1,
                        NU_IDENTIFICADOR as Identificador,
                        NU_TRANSACCION as Transaccion,
                        QT_ABASTECIDO as CantidadAbastecida,
                        QT_ANULADO as CantidadAnulada,
                        QT_ANULADO_FACTURA as CantidadAnuladaFactura,
                        QT_CARGADO as CantidadCargada,
                        QT_CONTROLADO as CantidadControlada,
                        QT_CROSS_DOCK as CantidadCrossDocking,
                        QT_EXPEDIDO as CantidadExpedida,
                        QT_FACTURADO as CantidadFacturada,
                        QT_LIBERADO as CantidadLiberada,
                        QT_PEDIDO as Cantidad,
                        QT_PEDIDO_ORIGINAL as CantidadOriginal,
                        QT_PREPARADO as CantidadPreparada,
                        QT_TRANSFERIDO as CantidadTransferida,
                        QT_UND_ASOCIADO_CAMION as CantUndAsociadoCamion,
                        VL_GENERICO_1 as VlGenerico_1,
                        VL_PORCENTAJE_TOLERANCIA as PorcentajeTolerancia,
                        VL_SERIALIZADO_1 as DatosSerializados
                    FROM T_DET_PEDIDO_SAIDA
                    WHERE NU_PEDIDO = :Pedido AND CD_EMPRESA = :Empresa AND CD_CLIENTE = :Cliente 
                    AND CD_PRODUTO = :Producto AND CD_FAIXA = :Faixa AND NU_IDENTIFICADOR= :Lote AND ID_ESPECIFICA_IDENTIFICADOR = 'N' 
                    AND QT_PEDIDO = :Cantidad AND QT_LIBERADO = :Cantidad";

            return _dapper.Query<DetallePedido>(connection, sql, detalle, transaction: tran, commandType: CommandType.Text).ToList();
        }

        public virtual bool PendienteDetPicking(AnulacionPreparacionDetalle detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT *
                           FROM T_DET_PICKING
                            WHERE NU_PEDIDO = :Pedido 
                            AND CD_CLIENTE = :Cliente
                            AND CD_EMPRESA = :Empresa 
                            AND NU_CARGA = :Carga
                            AND COALESCE(QT_PREPARADO,1) > 0 ";

            var query = _dapper.Query<DetallePreparacion>(connection, sql, detalle, transaction: tran, commandType: CommandType.Text).FirstOrDefault();
            if (query != null)
                return true;
            return false;
        }

        public virtual CargaCamion GetClienteCamion(AnulacionPreparacionDetalle detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            CD_CAMION as Camion,
                            CD_CLIENTE as Cliente,
                            CD_EMPRESA as Empresa,
                            DT_ADDROW as FechaAlta,
                            DT_UPDROW as FechaModificacion,
                            ID_CARGAR as IdCargar,
                            NU_CARGA as Carga,
                            TP_MODALIDAD as TipoModalidad,
                            FL_SYNC_REALIZADA as SincronizacionRealizadaId
                          FROM
                            T_CLIENTE_CAMION
                            WHERE CD_CLIENTE = :Cliente
                            AND CD_EMPRESA = :Empresa 
                            AND NU_CARGA = :Carga";

            return _dapper.Query<CargaCamion>(connection, sql, detalle, transaction: tran, commandType: CommandType.Text).FirstOrDefault();
        }

        //Add
        public virtual void AddStock(Stock stock, DbTransaction tran, DbConnection connection)
        {
            string sql = $@"INSERT INTO T_STOCK 
                            (CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR, QT_ESTOQUE, QT_RESERVA_SAIDA, QT_TRANSITO_ENTRADA,
                            ID_AVERIA, ID_INVENTARIO, ID_CTRL_CALIDAD, NU_TRANSACCION, DT_INVENTARIO, DT_UPDROW) 
                            VALUES( :Ubicacion, :Empresa, :Producto, :Faixa, :Identificador, :Cantidad, :ReservaSalida, :CantidadTransitoEntrada,
                            :Averia, :Inventario, :ControlCalidad, :NumeroTransaccion, :FechaInventario, :FechaModificacion)";

            stock.FechaInventario = stock.FechaModificacion = DateTime.Now;

            _dapper.Execute(connection, sql, stock, transaction: tran);
        }

        public virtual void GuardarErrores(int nroAnulacion, List<string> errores, DbConnection connection)
        {
            var model = Map(nroAnulacion, errores);
            string sql = @"INSERT INTO T_ANULACION_PREPARACION_ERROR (NU_ANULACION_PREPARACION, DS_ERROR)
             VALUES( :nroAnulacion, :error)";
            _dapper.Execute(connection, sql, model);
        }

        public virtual void AddLogPedidoAnulado(PedidoAnulado ped, DbTransaction tran, DbConnection connection)
        {
            var model = MapPedidoAnulado(ped);
            string sql = $@"INSERT INTO T_LOG_PEDIDO_ANULADO 
                            (NU_LOG_PEDIDO_ANULADO, NU_PEDIDO, CD_EMPRESA, CD_CLIENTE, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR, ID_ESPECIFICA_IDENTIFICADOR,
                            QT_ANULADO, DS_MOTIVO, CD_FUNCIONARIO, CD_APLICACAO, NU_INTERFAZ_EJECUCION, DT_ADDROW) 
                            VALUES( :Id, :Pedido, :Empresa, :Cliente, :Producto, :Embalaje, :Identificador, :EspecificaIdentificador,
                            :CantidadAnulada, :Motivo, :Funcionario, :Aplicacion, :InterfazEjecucion, :FechaAlta)";
            _dapper.Execute(connection, sql, model, transaction: tran);
        }

        //Update
        public virtual void UpdateEstadoAnulacion(int nroAnulacion, string estado, DbConnection connection)
        {
            string sql = @"UPDATE T_ANULACION_PREPARACION SET ND_ESTADO = :estado, DT_UPDROW = :fechaModificacion
                         WHERE NU_ANULACION_PREPARACION = :nroAnulacion ";

            _dapper.Execute(connection, sql, new
            {
                nroAnulacion = nroAnulacion,
                estado = estado,
                fechaModificacion = DateTime.Now,
            });
        }

        public virtual void UpdateDetPicking(AnulacionPreparacionDetalle detalle, DbConnection connection, DbTransaction tran)
        {
            var sql = @"UPDATE T_DET_PICKING 
                        SET QT_PREPARADO = :CantidadPreparada, 
                            CD_FUNCIONARIO = :UserIdAnulacion, 
                            ND_ESTADO = :EstadoDetPicking, 
                            NU_TRANSACCION = :Transaccion, 
                            DT_UPDROW = :FechaModificacion
                        WHERE CD_PRODUTO = :Producto  
                            AND CD_EMPRESA = :Empresa  
                            AND NU_IDENTIFICADOR = :Lote 
                            AND CD_FAIXA = :Faixa 
                            AND NU_PREPARACION = :NumeroPreparacion  
                            AND NU_PEDIDO = :Pedido 
                            AND CD_CLIENTE = :Cliente  
                            AND CD_ENDERECO = :Ubicacion 
                            AND NU_SEQ_PREPARACION = :NumeroSecuencia";

            detalle.FechaModificacion = DateTime.Now;

            _dapper.Execute(connection, sql, detalle, transaction: tran);
        }

        public virtual void UpdateStock(Stock stock, DbTransaction tran, DbConnection connection)
        {
            string sql = @" UPDATE T_STOCK 
                            SET QT_RESERVA_SAIDA = :ReservaSalida, 
                                NU_TRANSACCION = :NumeroTransaccion, 
                                DT_UPDROW = :FechaModificacion
                            WHERE CD_ENDERECO = :Ubicacion 
                                AND CD_PRODUTO = :Producto 
                                AND CD_FAIXA = :Faixa 
                                AND NU_IDENTIFICADOR = :Identificador 
                                AND CD_EMPRESA = :Empresa ";

            stock.FechaModificacion = DateTime.Now;

            _dapper.Execute(connection, sql, stock, transaction: tran);
        }

        public virtual void UpdateDetPedido(DetallePedido detalle, DbTransaction tran, DbConnection connection)
        {
            UpdateDetPedido(new List<DetallePedido> { detalle }, tran, connection);
        }

        public virtual void UpdateDetPedido(List<DetallePedido> detalles, DbTransaction tran, DbConnection connection)
        {
            var models = MapDetalles(detalles);
            var sql = $@"UPDATE T_DET_PEDIDO_SAIDA 
                        SET QT_PEDIDO = :Cantidad, 
                            QT_LIBERADO = :CantidadLiberada, 
                            QT_ANULADO = :CantidadAnulada, 
                            QT_PEDIDO_ORIGINAL = :CantidadOriginal, 
                            NU_TRANSACCION = :Transaccion, 
                            NU_TRANSACCION_DELETE = :TransaccionDelete, 
                            DT_UPDROW = :FechaModificacion
                        WHERE NU_PEDIDO = :Id  
                            AND CD_CLIENTE = :Cliente 
                            AND CD_EMPRESA = :Empresa 
                            AND CD_PRODUTO = :Producto 
                            AND CD_FAIXA = :Faixa 
                            AND NU_IDENTIFICADOR = :Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :EspecificaIdentificador ";

            _dapper.Execute(connection, sql, models, transaction: tran);
        }

        //Delete
        public virtual void DeleteDetPedido(List<DetallePedido> detalles, DbTransaction tran, DbConnection connection)
        {
            var models = MapDetalles(detalles);

            string sql = $@"DELETE T_DET_PEDIDO_SAIDA 
                            WHERE NU_PEDIDO = :Id  
                            AND CD_CLIENTE = :Cliente 
                            AND CD_EMPRESA = :Empresa 
                            AND CD_PRODUTO = :Producto 
                            AND CD_FAIXA = :Faixa 
                            AND NU_IDENTIFICADOR = :Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :EspecificaIdentificador ";

            _dapper.Execute(connection, sql, models, transaction: tran);
        }

        public virtual void DeleteClienteCamion(CargaCamion cargaCamion, DbConnection connection, DbTransaction tran)
        {
            string sql = @"DELETE T_CLIENTE_CAMION 
                           WHERE CD_CAMION = :Camion 
                           AND NU_CARGA = :Carga 
                           AND CD_CLIENTE = :Cliente";

            _dapper.Execute(connection, sql, cargaCamion, transaction: tran);
        }

        //Aux
        public virtual List<object> Map(int nroAnulacion, List<string> errores)
        {
            var result = new List<object>();

            foreach (var e in errores)
            {
                result.Add(new
                {
                    nroAnulacion = nroAnulacion,
                    error = e
                });
            }
            return result;
        }

        public virtual object MapDetalle(DetallePedido detalle)
        {
            return new
            {
                Id = detalle.Id,
                Cliente = detalle.Cliente,
                Empresa = detalle.Empresa,
                Producto = detalle.Producto,
                Faixa = detalle.Faixa,
                Identificador = detalle.Identificador,
                EspecificaIdentificador = detalle.EspecificaIdentificadorId,
                Cantidad = detalle.Cantidad,
                CantidadLiberada = detalle.CantidadLiberada,
                CantidadAnulada = detalle.CantidadAnulada,
                CantidadOriginal = detalle.CantidadOriginal,
                Transaccion = detalle.Transaccion,
                TransaccionDelete = detalle.TransaccionDelete,
                FechaModificacion = DateTime.Now,
            };
        }

        public virtual List<object> MapDetalles(List<DetallePedido> detalles)
        {
            var result = new List<object>();

            foreach (var det in detalles)
            {
                result.Add(MapDetalle(det));
            }

            return result;
        }

        public virtual object MapPedidoAnulado(PedidoAnulado ped)
        {
            return new
            {
                Id = ped.Id,
                Pedido = ped.Pedido,
                Empresa = ped.Empresa,
                Cliente = ped.Cliente,
                Producto = ped.Producto,
                Embalaje = ped.Embalaje,
                Identificador = ped.Identificador,
                EspecificaIdentificador = ped.EspecificaIdentificadorId,
                CantidadAnulada = ped.CantidadAnulada,
                Motivo = ped.Motivo,
                Funcionario = ped.Funcionario,
                Aplicacion = ped.Aplicacion,
                InterfazEjecucion = ped.InterfazEjecucion,
                FechaAlta = DateTime.Now,
            };
        }

        public virtual void GenerarStockFaltante(long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            string sql = $@"
                INSERT INTO T_STOCK (
                    CD_ENDERECO, 
                    CD_EMPRESA, 
                    CD_PRODUTO, 
                    CD_FAIXA, 
                    NU_IDENTIFICADOR, 
                    QT_ESTOQUE, 
                    QT_RESERVA_SAIDA, 
                    QT_TRANSITO_ENTRADA, 
                    DT_UPDROW, 
                    DT_INVENTARIO,
                    ID_AVERIA,
                    ID_INVENTARIO,
                    ID_CTRL_CALIDAD,
                    NU_TRANSACCION
                )
                SELECT 
                    dp.CD_ENDERECO, 
                    dp.CD_EMPRESA, 
                    dp.CD_PRODUTO, 
                    dp.CD_FAIXA, 
                    dp.NU_IDENTIFICADOR,
                    0, 
                    sum(dp.QT_PRODUTO),
                    0,
                    :FechaModificacion,
                    :FechaModificacion,
                    'N',
                    'R',
                    'C',
                    :Transaccion
                FROM V_ANULACION_PREPARACION_PEND dp                    
                LEFT JOIN T_STOCK sto ON dp.CD_ENDERECO = sto.CD_ENDERECO 
                    AND dp.CD_EMPRESA = sto.CD_EMPRESA 
                    AND dp.CD_PRODUTO = sto.CD_PRODUTO 
                    AND dp.CD_FAIXA = sto.CD_FAIXA 
                    AND dp.NU_IDENTIFICADOR = sto.NU_IDENTIFICADOR
                WHERE dp.NU_IDENTIFICADOR != '(AUTO)'
                    AND (sto.CD_ENDERECO IS NULL   
                    AND sto.CD_EMPRESA IS NULL 
                    AND sto.CD_PRODUTO IS NULL 
                    AND sto.CD_FAIXA IS NULL 
                    AND sto.NU_IDENTIFICADOR IS NULL)
                GROUP BY 
                    dp.CD_ENDERECO, 
                    dp.CD_EMPRESA, 
                    dp.CD_PRODUTO, 
                    dp.CD_FAIXA, 
                    dp.NU_IDENTIFICADOR";

            _dapper.Execute(connection, sql, new
            {
                Transaccion = nuTransaccion,
                FechaModificacion = DateTime.Now,
            }, transaction: tran);
        }

        public virtual long GetNextIdLogPedidoAnulado(DbConnection connection, DbTransaction tran)
        {
            return _dapper.GetNextSequenceValue<long>(connection, Secuencias.S_LOG_PEDIDO_ANULADO, tran);
        }

        public virtual long GetNextIdLogPedidoAnuladoLpn(DbConnection connection, DbTransaction tran)
        {
            return _dapper.GetNextSequenceValue<long>(connection, Secuencias.S_LOG_PEDIDO_ANULADO_LPN, tran);
        }

        public virtual void ActualizarAnulacionesDocumentales(DbConnection connection, ref long? nuTransaccion)
        {
            string sql = $@"
                SELECT dp.NU_PREPARACION as NumeroPreparacion,
                    dp.CD_PRODUTO as Producto,
                    dp.CD_FAIXA as Faixa,
                    dp.NU_IDENTIFICADOR as Lote,
                    dp.CD_EMPRESA as Empresa,
                    dp.CD_ENDERECO as Ubicacion,
                    dp.NU_PEDIDO as Pedido,
                    dp.CD_CLIENTE as Cliente,
                    dp.NU_SEQ_PREPARACION as NumeroSecuencia,
                    CASE 
                        WHEN dapr.ID_ESTADO = '{EstadoAnularReservaPreparacion.EJECUTADA}' THEN '{EstadoDetallePreparacion.ESTADO_ANULACION_EJECUTADA_DOC}'
                        WHEN dapr.ID_ESTADO = '{EstadoAnularReservaPreparacion.ERROR}' THEN '{EstadoDetallePreparacion.ESTADO_ANULACION_DOC_ERROR}'
                        ELSE '{EstadoDetallePreparacion.ESTADO_ANULACION_DOC_ENV}'
                    END as Estado
                FROM T_DET_PICKING dp
                INNER JOIN (
                    SELECT NU_PREPARACION,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        CD_EMPRESA,
                        CD_ENDERECO,
                        NU_PEDIDO,
                        CD_CLIENTE,
                        NU_SEQ_PREPARACION,
                        CONCAT(NU_PREPARACION, CONCAT('#', CONCAT(CD_PRODUTO, CONCAT('#', CONCAT(CD_FAIXA, CONCAT('#', CONCAT(NU_IDENTIFICADOR, CONCAT('#', CONCAT(CD_EMPRESA, CONCAT('#', CONCAT(CD_ENDERECO, CONCAT('#', CONCAT(NU_PEDIDO, CONCAT('#', CONCAT(CD_CLIENTE, CONCAT('#', NU_SEQ_PREPARACION)))))))))))))))) AS ID_ANULACION
                        FROM T_DET_PICKING
                ) dpa ON dpa.NU_PREPARACION = dp.NU_PREPARACION
                    AND dpa.CD_PRODUTO = dp.CD_PRODUTO
                    AND dpa.CD_FAIXA = dp.CD_FAIXA
                    AND dpa.NU_IDENTIFICADOR = dp.NU_IDENTIFICADOR
                    AND dpa.CD_EMPRESA = dp.CD_EMPRESA
                    AND dpa.CD_ENDERECO = dp.CD_ENDERECO
                    AND dpa.NU_PEDIDO = dp.NU_PEDIDO
                    AND dpa.CD_CLIENTE = dp.CD_CLIENTE
                    AND dpa.NU_SEQ_PREPARACION = dp.NU_SEQ_PREPARACION
                INNER JOIN T_DOCUMENTO_ANU_PREP_RESERVA dapr ON dapr.ID_ANULACION = dpa.ID_ANULACION
                INNER JOIN V_EMPRESA_DOCUMENTAL ed ON ed.CD_EMPRESA = dp.CD_EMPRESA
                WHERE dp.ND_ESTADO = '{EstadoDetallePreparacion.ESTADO_ANULACION_DOC_ENV}' 
                    AND ed.FL_DOCUMENTAL = 'S'";

            var preparaciones = _dapper.Query<DetallePreparacion>(connection, sql);

            if (preparaciones.Count() > 0)
            {
                var transaccion = nuTransaccion;
                if (!transaccion.HasValue)
                    transaccion = nuTransaccion = _transaccionRepository.CreateTransaction($"Proceso de anulación", connection, app: _application, userId: _userId).Result;

                _dapper.BulkUpdate(connection, null, preparaciones, "T_DET_PICKING", new Dictionary<string, Func<DetallePreparacion, ColumnInfo>>
                {
                    { "ND_ESTADO", x => new ColumnInfo(x.Estado)},
                    { "NU_TRANSACCION", x => new ColumnInfo(transaccion, DbType.Int64)},
                    { "DT_UPDROW", x => new ColumnInfo(x.FechaModificacion, DbType.DateTime)},
                }, new Dictionary<string, Func<DetallePreparacion, ColumnInfo>>
                {
                    { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                    { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                    { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
                    { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                    { "NU_PREPARACION", x => new ColumnInfo(x.NumeroPreparacion)},
                    { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                    { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                    { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                    { "NU_SEQ_PREPARACION", x => new ColumnInfo(x.NumeroSecuencia)},
                });
            }
        }

        public virtual void AnularPreparacionesDocumentales(DbConnection connection, ref long? nuTransaccion)
        {
            using (var tran = connection.BeginTransaction(this._dapper.GetSnapshotIsolationLevel()))
            {
                try
                {
                    string sql = $@"
                        SELECT dp.NU_PREPARACION as NumeroPreparacion,
                            dp.CD_PRODUTO as Producto,
                            dp.CD_FAIXA as Faixa,
                            dp.NU_IDENTIFICADOR as Lote,
                            dp.CD_EMPRESA as Empresa,
                            dp.CD_ENDERECO as Ubicacion,
                            dp.NU_PEDIDO as Pedido,
                            dp.CD_CLIENTE as Cliente,
                            dp.NU_SEQ_PREPARACION as NumeroSecuencia,
                            dp.ID_ESPECIFICA_IDENTIFICADOR as EspecificaLote,
                            dp.QT_PRODUTO as Cantidad,
                            '{EstadoDetallePreparacion.ESTADO_ANULACION_DOC_ENV}' as Estado
                        FROM T_DET_PICKING dp
                        INNER JOIN V_EMPRESA_DOCUMENTAL ed ON ed.CD_EMPRESA = dp.CD_EMPRESA
                        WHERE dp.ND_ESTADO IN ('{EstadoDetallePreparacion.ESTADO_ANULACION_DOC_PEND}','{EstadoDetallePreparacion.ESTADO_ANULACION_PENDIENTE}') 
                            AND ed.FL_DOCUMENTAL = 'S'";

                    var preparaciones = _dapper.Query<DetallePreparacion>(connection, sql, null, CommandType.Text, transaction: tran);

                    if (preparaciones.Count() > 0)
                    {
                        var transaccion = nuTransaccion;
                        if (!transaccion.HasValue)
                            transaccion = nuTransaccion = _transaccionRepository.CreateTransaction($"Proceso de anulación", connection, app: _application, userId: _userId).Result;

                        _dapper.BulkUpdate(connection, tran, preparaciones, "T_DET_PICKING", new Dictionary<string, Func<DetallePreparacion, ColumnInfo>>
                        {
                            { "ND_ESTADO", x => new ColumnInfo(x.Estado)},
                            { "NU_TRANSACCION", x => new ColumnInfo(transaccion, DbType.Int64)},
                            { "DT_UPDROW", x => new ColumnInfo(DateTime.Now)},
                        }, new Dictionary<string, Func<DetallePreparacion, ColumnInfo>>
                        {
                            { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                            { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                            { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
                            { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                            { "NU_PREPARACION", x => new ColumnInfo(x.NumeroPreparacion)},
                            { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                            { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                            { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                            { "NU_SEQ_PREPARACION", x => new ColumnInfo(x.NumeroSecuencia)},
                        });

                        sql = $@"
                            INSERT INTO T_DOCUMENTO_ANU_PREP_RESERVA (
                                CD_EMPRESA,
                                CD_FAIXA,
                                CD_PRODUTO,
                                DT_ADDROW,
                                DT_UPDATEROW,
                                ID_ANULACION,
                                ID_ESPECIFICA_IDENTIFICADOR,
                                ID_ESTADO,
                                NU_IDENTIFICADOR,
                                NU_PREPARACION,
                                QT_ANULAR
                            )
                            VALUES (    
                                :Empresa,
                                :Faixa,
                                :Producto,
                                :FechaModificacion,
                                :FechaModificacion,
                                CONCAT(:NumeroPreparacion, CONCAT('#', CONCAT(:Producto, CONCAT('#', CONCAT(:Faixa, CONCAT('#', CONCAT(:Lote, CONCAT('#', CONCAT(:Empresa, CONCAT('#', CONCAT(:Ubicacion, CONCAT('#', CONCAT(:Pedido, CONCAT('#', CONCAT(:Cliente, CONCAT('#', :NumeroSecuencia)))))))))))))))),
                                :EspecificaLote,
                                '{EstadoAnularReservaPreparacion.PENDIENTE}',
                                :Lote,
                                :NumeroPreparacion,
                                :Cantidad
                            )
                        ";

                        _dapper.Execute(connection, sql, param: preparaciones, transaction: tran);
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        #region Lpn

        public virtual void UpdateDetallePreparacionLpn(DetallePreparacionLpn detPickingLpn, DbConnection connection, DbTransaction tran)
        {
            string sql = @" UPDATE T_DET_PICKING_LPN
                            SET QT_RESERVA = :CantidadReservada, 
                                NU_TRANSACCION = :Transaccion, 
                                DT_UPDROW = :FechaModificacion
                            WHERE NU_PREPARACION = :NroPreparacion
                            AND ID_DET_PICKING_LPN = :IdDetallePickingLpn ";

            _dapper.Execute(connection, sql, detPickingLpn, transaction: tran);
        }

        public virtual DetallePreparacionLpn GetDetallePreparacionLpn(int nuPreparacion, long idDetPickingLpn, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            NU_PREPARACION as NroPreparacion,
                            ID_DET_PICKING_LPN as IdDetallePickingLpn,
                            ID_LPN_DET as IdDetalleLpn,
                            NU_LPN as NroLpn,
                            CD_EMPRESA as Empresa,
                            CD_PRODUTO as Producto,
                            CD_FAIXA as Faixa,
                            NU_IDENTIFICADOR as Lote,
                            VL_ATRIBUTOS as Atributos,
                            QT_RESERVA as CantidadReservada,
                            TP_LPN_TIPO as TipoLpn,
                            CD_ENDERECO as Ubicacion,
                            DT_ADDROW as FechaAlta,
                            DT_UPDROW as FechaModificacion,
                            NU_TRANSACCION as Transaccion,
                            NU_TRANSACCION_DELETE as TransaccionDelete,
                            ID_LPN_EXTERNO as IdExternoLpn,
                            NU_DET_PED_SAI_ATRIB as IdConfiguracion                       
                        FROM T_DET_PICKING_LPN
                        WHERE NU_PREPARACION = :NumeroPreparacion
                        AND ID_DET_PICKING_LPN = :IdDetallePickingLpn";

            return _dapper.Query<DetallePreparacionLpn>(connection, sql, param: new
            {
                NumeroPreparacion = nuPreparacion,
                IdDetallePickingLpn = idDetPickingLpn
            }, transaction: tran, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual void UpdateDetalleLpn(LpnDetalle detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @" UPDATE T_LPN_DET
                            SET QT_RESERVA_SAIDA = QT_RESERVA_SAIDA - :Cantidad, 
                                NU_TRANSACCION = :NumeroTransaccion
                            WHERE ID_LPN_DET = :Id
                            AND NU_LPN = :NumeroLPN
                            AND CD_PRODUTO = :CodigoProducto
                            AND CD_FAIXA = :Faixa
                            AND CD_EMPRESA = :Empresa
                            AND NU_IDENTIFICADOR = :Lote ";

            _dapper.Execute(connection, sql, detalle, transaction: tran);
        }

        public virtual DetallePedidoLpn GetDetallePedidoLpn(AnulacionPreparacionDetalle detalleAnulacion, DetallePreparacionLpn detPickingLpn, DbConnection connection, DbTransaction tran, bool auto)
        {
            string sql = @"SELECT 
                            NU_PEDIDO as Pedido,
                            CD_CLIENTE as Cliente,
                            CD_EMPRESA as Empresa,
                            CD_PRODUTO as Producto,
                            CD_FAIXA as Faixa,
                            NU_IDENTIFICADOR as Identificador,
                            ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                            ID_LPN_EXTERNO as IdLpnExterno,
                            TP_LPN_TIPO as Tipo,
                            QT_PEDIDO as CantidadPedida,
                            QT_LIBERADO as CantidadLiberada,
                            QT_ANULADO as CantidadAnulada,
                            DT_ADDROW as FechaAlta,
                            DT_UPDROW as FechaModificacion,
                            NU_TRANSACCION as Transaccion,
                            NU_TRANSACCION_DELETE as TransaccionDelete,              
                            NU_LPN as NumeroLpn              
                        FROM T_DET_PEDIDO_SAIDA_LPN
                        WHERE NU_PEDIDO = :Pedido
                            AND CD_CLIENTE = :Cliente
                            AND CD_EMPRESA = :Empresa
                            AND CD_PRODUTO = :Producto
                            AND CD_FAIXA = :Faixa
                            AND NU_IDENTIFICADOR = :Identificador
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                            AND ID_LPN_EXTERNO = :IdLpnExterno
                            AND TP_LPN_TIPO = :Tipo ";

            return _dapper.Query<DetallePedidoLpn>(connection, sql, param: new
            {
                Pedido = detalleAnulacion.Pedido,
                Cliente = detalleAnulacion.Cliente,
                Empresa = detalleAnulacion.Empresa,
                Producto = detalleAnulacion.Producto,
                Faixa = detalleAnulacion.Faixa,
                Identificador = auto ? ManejoIdentificadorDb.IdentificadorAuto : detalleAnulacion.Lote,
                IdEspecificaIdentificador = detalleAnulacion.EspecificaLote,
                Tipo = detPickingLpn.TipoLpn,
                IdLpnExterno = detPickingLpn.IdExternoLpn
            }, transaction: tran, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual void UpdateDetallePedidoLpn(DetallePedidoLpn detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @" UPDATE T_DET_PEDIDO_SAIDA_LPN
                            SET QT_PEDIDO = :CantidadPedida, 
                                QT_LIBERADO = :CantidadLiberada, 
                                QT_ANULADO = :CantidadAnulada,
                                DT_UPDROW = :FechaModificacion,
                                NU_TRANSACCION = :Transaccion
                            WHERE NU_PEDIDO = :Pedido
                            AND CD_CLIENTE = :Cliente 
                            AND CD_EMPRESA = :Empresa 
                            AND CD_PRODUTO = :Producto 
                            AND CD_FAIXA = :Faixa 
                            AND NU_IDENTIFICADOR = :Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                            AND ID_LPN_EXTERNO = :IdLpnExterno 
                            AND TP_LPN_TIPO = :Tipo ";

            _dapper.Execute(connection, sql, detalle, transaction: tran);
        }

        public virtual void DeleteDetallePedidoLpn(DetallePedidoLpn detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @" UPDATE T_DET_PEDIDO_SAIDA_LPN
                            SET DT_UPDROW = :FechaModificacion,
                                NU_TRANSACCION = :Transaccion,
                                NU_TRANSACCION_DELETE = :TransaccionDelete
                            WHERE NU_PEDIDO = :Pedido 
                            AND CD_CLIENTE = :Cliente
                            AND CD_EMPRESA = :Empresa
                            AND CD_PRODUTO = :Producto
                            AND CD_FAIXA = :Faixa
                            AND NU_IDENTIFICADOR = :Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                            AND ID_LPN_EXTERNO = :IdLpnExterno 
                            AND TP_LPN_TIPO = :Tipo ";

            _dapper.Execute(connection, sql, detalle, transaction: tran);

            sql = @"DELETE FROM T_DET_PEDIDO_SAIDA_LPN
                    WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa 
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                    AND ID_LPN_EXTERNO = :IdLpnExterno 
                    AND TP_LPN_TIPO = :Tipo ";

            _dapper.Execute(connection, sql, detalle, transaction: tran);
        }

        public virtual DetallePedidoLpnAtributo GetDetallePedidoLpnAtributo(AnulacionPreparacionDetalle detalleAnulacion, DetallePreparacionLpn detPickingLpn, DbConnection connection, DbTransaction tran, bool auto)
        {
            string sql = @"SELECT 
                            NU_PEDIDO as Pedido,
                            CD_CLIENTE as Cliente,
                            CD_EMPRESA as Empresa,
                            CD_PRODUTO as Producto,
                            CD_FAIXA as Faixa,
                            NU_IDENTIFICADOR as Identificador,
                            ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                            TP_LPN_TIPO as Tipo,
                            ID_LPN_EXTERNO as IdLpnExterno,
                            NU_DET_PED_SAI_ATRIB as IdConfiguracion,
                            QT_PEDIDO as CantidadPedida,
                            QT_LIBERADO as CantidadLiberada,
                            QT_ANULADO as CantidadAnulada,
                            DT_ADDROW as FechaAlta,
                            DT_UPDROW as FechaModificacion,
                            NU_TRANSACCION as Transaccion,
                            NU_TRANSACCION_DELETE as TransaccionDelete
                        FROM T_DET_PEDIDO_SAIDA_LPN_ATRIB
                        WHERE NU_PEDIDO = :Pedido
                            AND CD_CLIENTE = :Cliente 
                            AND CD_EMPRESA = :Empresa 
                            AND CD_PRODUTO = :Producto 
                            AND CD_FAIXA = :Faixa 
                            AND NU_IDENTIFICADOR = :Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                            AND ID_LPN_EXTERNO = :IdLpnExterno 
                            AND TP_LPN_TIPO = :Tipo 
                            AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            return _dapper.Query<DetallePedidoLpnAtributo>(connection, sql, param: new
            {
                Pedido = detalleAnulacion.Pedido,
                Cliente = detalleAnulacion.Cliente,
                Empresa = detalleAnulacion.Empresa,
                Producto = detalleAnulacion.Producto,
                Faixa = detalleAnulacion.Faixa,
                Identificador = auto ? ManejoIdentificadorDb.IdentificadorAuto : detalleAnulacion.Lote,
                IdEspecificaIdentificador = detalleAnulacion.EspecificaLote,
                Tipo = detPickingLpn.TipoLpn,
                IdLpnExterno = detPickingLpn.IdExternoLpn,
                IdConfiguracion = detPickingLpn.IdConfiguracion.Value
            }, transaction: tran, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual void UpdateDetallePedidoLpnAtributo(DetallePedidoLpnAtributo detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @" UPDATE T_DET_PEDIDO_SAIDA_LPN_ATRIB
                            SET QT_PEDIDO = :CantidadPedida, 
                                QT_LIBERADO = :CantidadLiberada,
                                QT_ANULADO = :CantidadAnulada,
                                DT_UPDROW = :FechaModificacion,
                                NU_TRANSACCION = :Transaccion
                            WHERE NU_PEDIDO = :Pedido
                            AND CD_CLIENTE = :Cliente 
                            AND CD_EMPRESA = :Empresa 
                            AND CD_PRODUTO = :Producto 
                            AND CD_FAIXA = :Faixa 
                            AND NU_IDENTIFICADOR = :Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                            AND ID_LPN_EXTERNO = :IdLpnExterno 
                            AND TP_LPN_TIPO = :Tipo 
                            AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            _dapper.Execute(connection, sql, detalle, transaction: tran);
        }

        public virtual void DeleteDetallePedidoLpnAtributo(DetallePedidoLpnAtributo detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @" UPDATE T_DET_PEDIDO_SAIDA_LPN_ATRIB
                            SET DT_UPDROW = :FechaModificacion,
                                NU_TRANSACCION = :Transaccion,
                                NU_TRANSACCION_DELETE = :TransaccionDelete
                            WHERE NU_PEDIDO = :Pedido
                            AND CD_CLIENTE = :Cliente 
                            AND CD_EMPRESA = :Empresa 
                            AND CD_PRODUTO = :Producto 
                            AND CD_FAIXA = :Faixa 
                            AND NU_IDENTIFICADOR = :Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                            AND ID_LPN_EXTERNO = :IdLpnExterno 
                            AND TP_LPN_TIPO = :Tipo 
                            AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            _dapper.Execute(connection, sql, detalle, transaction: tran);

            sql = @"DELETE FROM T_DET_PEDIDO_SAIDA_LPN_ATRIB                            
                    WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa 
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                    AND ID_LPN_EXTERNO = :IdLpnExterno 
                    AND TP_LPN_TIPO = :Tipo 
                    AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            _dapper.Execute(connection, sql, detalle, transaction: tran);
        }

        public virtual DetallePedidoAtributo GetDetallePedidoAtributo(AnulacionPreparacionDetalle detalleAnulacion, long idConfiguracion, DbConnection connection, DbTransaction tran, bool auto)
        {
            string sql = @"SELECT 
                            NU_PEDIDO as Pedido,
                            CD_CLIENTE as Cliente,
                            CD_EMPRESA as Empresa,
                            CD_PRODUTO as Producto,
                            CD_FAIXA as Faixa,
                            NU_IDENTIFICADOR as Identificador,
                            ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                            NU_DET_PED_SAI_ATRIB as IdConfiguracion,
                            QT_PEDIDO as CantidadPedida,
                            QT_LIBERADO as CantidadLiberada,
                            QT_ANULADO as CantidadAnulada,
                            DT_ADDROW as FechaAlta,
                            DT_UPDROW as FechaModificacion,
                            NU_TRANSACCION as Transaccion,
                            NU_TRANSACCION_DELETE as TransaccionDelete              
                        FROM T_DET_PEDIDO_SAIDA_ATRIB
                        WHERE NU_PEDIDO = :Pedido 
                            AND CD_CLIENTE = :Cliente 
                            AND CD_EMPRESA = :Empresa 
                            AND CD_PRODUTO = :Producto 
                            AND CD_FAIXA = :Faixa 
                            AND NU_IDENTIFICADOR = :Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                            AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            return _dapper.Query<DetallePedidoAtributo>(connection, sql, param: new
            {
                Pedido = detalleAnulacion.Pedido,
                Cliente = detalleAnulacion.Cliente,
                Empresa = detalleAnulacion.Empresa,
                Producto = detalleAnulacion.Producto,
                Faixa = detalleAnulacion.Faixa,
                Identificador = auto ? ManejoIdentificadorDb.IdentificadorAuto : detalleAnulacion.Lote,
                IdEspecificaIdentificador = detalleAnulacion.EspecificaLote,
                IdConfiguracion = idConfiguracion
            }, transaction: tran, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual void UpdateDetallePedidoAtributo(DetallePedidoAtributo detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @" UPDATE T_DET_PEDIDO_SAIDA_ATRIB
                            SET QT_PEDIDO = :CantidadPedida, 
                                QT_LIBERADO = :CantidadLiberada, 
                                QT_ANULADO = :CantidadAnulada,
                                DT_UPDROW = :FechaModificacion,
                                NU_TRANSACCION = :Transaccion
                            WHERE NU_PEDIDO = :Pedido 
                            AND CD_CLIENTE = :Cliente 
                            AND CD_EMPRESA = :Empresa 
                            AND CD_PRODUTO = :Producto 
                            AND CD_FAIXA = :Faixa 
                            AND NU_IDENTIFICADOR = :Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                            AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            _dapper.Execute(connection, sql, detalle, transaction: tran);
        }

        public virtual void DeleteDetallePedidoAtributo(DetallePedidoAtributo detalle, DbConnection connection, DbTransaction tran)
        {
            string sql = @" UPDATE T_DET_PEDIDO_SAIDA_ATRIB
                            SET DT_UPDROW = :FechaModificacion,
                                NU_TRANSACCION = :Transaccion,
                                NU_TRANSACCION_DELETE = :TransaccionDelete
                            WHERE NU_PEDIDO = :Pedido 
                            AND CD_CLIENTE = :Cliente 
                            AND CD_EMPRESA = :Empresa 
                            AND CD_PRODUTO = :Producto 
                            AND CD_FAIXA = :Faixa 
                            AND NU_IDENTIFICADOR = :Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                            AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            _dapper.Execute(connection, sql, detalle, transaction: tran);

            sql = @"DELETE FROM T_DET_PEDIDO_SAIDA_ATRIB                            
                    WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa 
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                    AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            _dapper.Execute(connection, sql, detalle, transaction: tran);
        }

        public virtual void AddLogPedidoAnuladoLpn(PedidoAnuladoLpn pedidoAnuladoLpn, DbConnection connection, DbTransaction tran)
        {
            string sql = @"INSERT INTO T_LOG_PEDIDO_ANULADO_LPN 
                    (NU_LOG_PEDIDO_ANULADO_LPN, 
                     NU_LOG_PEDIDO_ANULADO, 
                     TP_OPERACION,
                     ID_LPN_EXTERNO,
                     TP_LPN_TIPO,
                     NU_DET_PED_SAI_ATRIB,
                     QT_ANULADO,
                     DT_ADDROW) 
                    VALUES 
                    (:Id,
                     :IdLogPedidoAnulado,
                     :TipoOperacion,
                     :IdExternoLpn,
                     :TipoLpn,
                     :IdConfiguracion,
                     :CantidadAnulada,
                     :FechaInsercion)";

            _dapper.Execute(connection, sql, pedidoAnuladoLpn, transaction: tran);
        }

        #endregion

        #region PRE120AnulacionPreparacion
        public virtual (List<int>,List<int>) ProcesarAnulacion(List<DetallePreparacion> detalles, List<int> preparaciones, bool anulacionParcial, long nuTransaccion, List<int> empresasDocumentales, int tipo, string modo)
        {
            List<int> preparacionesPendientes = null;
            List<int> preparacionesEnTraspaso = null;
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            preparacionesPendientes = GetAnulacionesPendientes(connection, tran, preparaciones.Select(x => new Preparacion { Id = x }).ToList());
            preparacionesEnTraspaso= GetAnulacionesEnTraspaso(connection, tran, preparaciones.Select(x => new Preparacion { Id = x }).ToList());


            List<AnulacionPreparacion> preparacionesNoPendientes = GetAnulacionesNoPendientes(connection, tran, preparaciones.Select(x => new Preparacion { Id = x }).ToList());
            List<int> nroAnulacionPreparaciones = GetNewNroAnulacionPreparacionIds(preparacionesNoPendientes.Count(), connection, tran);

            foreach (var anulacionPreparacion in preparacionesNoPendientes)
            {
                var nroAnulacionPreparacion = nroAnulacionPreparaciones.FirstOrDefault();
                nroAnulacionPreparaciones.Remove(nroAnulacionPreparacion);
                anulacionPreparacion.NroAnulacionPreparacion = nroAnulacionPreparacion;
                anulacionPreparacion.Estado = EstadoAnulacion.AnulacionPendiente;
                anulacionPreparacion.Descripcion = $"Anulación para la preparación {anulacionPreparacion.Preparacion}";
                anulacionPreparacion.TipoAnulacion = tipo;
                anulacionPreparacion.TipoAgrupacion = modo;
                anulacionPreparacion.Alta = DateTime.Now;
                anulacionPreparacion.UserId = _userId;
            }

            AddAnulacionesPendientes(connection, tran, preparacionesNoPendientes);

            var detallesMarcarAnular = detalles.Join(preparacionesNoPendientes,
                dl => dl.NumeroPreparacion,
                pnp => pnp.Preparacion,
                (dl, pnp) => dl).ToList();

            if (anulacionParcial)
            {
                AddAnulacionParcialPreparacionTemporal(connection, tran, detallesMarcarAnular, nuTransaccion);
                UpdateAnulacionPreparacionEmpresaDocumental(connection, tran, nuTransaccion);
                UpdateAnulacionPreparacionEmpresa(connection, tran, nuTransaccion);

                List<AnulacionPreparacionDetalle> detallesParciales = GetDetallePreparacionAnulacionParcial(connection, tran);

                int cantidadNuevosRegistros = detallesParciales.Count();
                int cantidadNuevosRegistrosLpn = detallesParciales.Where(x => x.IdDetallePickingLpn != null).Count();
                if (cantidadNuevosRegistros > 0)
                {
                    var newSecPreparacion = _preparacionRepository.GetSecuenciasDetallePicking(cantidadNuevosRegistros, connection, tran);
                    var newIdDetallePickingLpn = _preparacionRepository.GetNewIdDetallePickingLpn(cantidadNuevosRegistrosLpn, connection, tran);

                    foreach (var detalle in detallesParciales)
                    {
                        var newSec = newSecPreparacion[0];
                        if (detalle.IdDetallePickingLpn != null)
                        {
                            var newIdPickLpn = newIdDetallePickingLpn[0];
                            detalle.IdDetallePickingLpnDest = newIdPickLpn;
                            newIdDetallePickingLpn.RemoveAt(0);
                        }
                        detalle.NumeroSecuenciaDest = newSec;
                        newSecPreparacion.RemoveAt(0);
                    }
                }

                UpdateAnulacionParcialPreparacionTemporal(connection, tran, detallesParciales);

                InsertDetallePreparacionParcialAnulacionEmpresaDocumental(connection, tran, nuTransaccion);
                InsertDetallePreparacionParcialAnulacion(connection, tran, nuTransaccion);

                InsertDetallePreparacionParcialLpnAnulacion(connection, tran, nuTransaccion);

                UpdatePreparacionCantidadPendienteParcial(connection, tran, nuTransaccion);
                UpdatePreparacionCantidadPendienteLpnParcial(connection, tran, nuTransaccion);
            }
            else
            {
                AddAnulacionPreparacionTemporal(connection, tran, detallesMarcarAnular);
                UpdateAnulacionPreparacionEmpresaDocumental(connection, tran, nuTransaccion);
                UpdateAnulacionPreparacionEmpresa(connection, tran, nuTransaccion);
            }

            DeleteAnulacionPreparacionTemporal(connection, tran, detallesMarcarAnular, nuTransaccion);
            DeleteAnulacionPreparacionSinDetalles(connection, tran);

            return (preparacionesPendientes,preparacionesEnTraspaso);
        }

        #region Update
        public virtual void UpdatePreparacionCantidadPendienteParcial(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new DynamicParameters(new
            {
                FechaModificacion = DateTime.Now,
                Transaccion = nuTransaccion,
                Usuario = _userId,
                EstadoWhere = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE
            });

            var alias = "dp";
            var from = $@"
                T_DET_PICKING dp
                INNER JOIN (
                    SELECT 
                        dp.NU_PREPARACION,
                        dp.NU_PEDIDO,
                        dp.CD_CLIENTE,
                        dp.CD_EMPRESA,
                        dp.CD_ENDERECO,
                        dp.CD_PRODUTO,
                        dp.CD_FAIXA,
                        dp.NU_IDENTIFICADOR,
                        dp.NU_SEQ_PREPARACION,
                        max(dpt.QT_ANULAR) QT_ANULAR
                    FROM T_DET_PICKING dp
                    INNER JOIN T_DET_PICKING_TEMP dpt on dpt.NU_PREPARACION = dp.NU_PREPARACION
                            AND dpt.NU_PEDIDO = dp.NU_PEDIDO
                            AND dpt.CD_CLIENTE = dp.CD_CLIENTE
                            AND dpt.CD_EMPRESA = dp.CD_EMPRESA
                            AND dpt.CD_ENDERECO = dp.CD_ENDERECO
                            AND dpt.CD_PRODUTO = dp.CD_PRODUTO
                            AND dpt.CD_FAIXA = dp.CD_FAIXA
                            AND dpt.NU_IDENTIFICADOR = dp.NU_IDENTIFICADOR
                            AND dpt.NU_SEQ_PREPARACION = dp.NU_SEQ_PREPARACION
                            AND dpt.QT_ANULAR <> dpt.QT_PRODUTO
                            AND dpt.QT_ANULAR is not null
                   WHERE dp.ND_ESTADO = :EstadoWhere
                   GROUP by dp.NU_PREPARACION,
                        dp.NU_PEDIDO,
                        dp.CD_CLIENTE,
                        dp.CD_EMPRESA,
                        dp.CD_ENDERECO,
                        dp.CD_PRODUTO,
                        dp.CD_FAIXA,
                        dp.NU_IDENTIFICADOR,
                        dp.NU_SEQ_PREPARACION
                ) dpt ON  dpt.NU_PREPARACION = dp.NU_PREPARACION
                        AND dpt.NU_PEDIDO = dp.NU_PEDIDO
                        AND dpt.CD_CLIENTE = dp.CD_CLIENTE
                        AND dpt.CD_EMPRESA = dp.CD_EMPRESA
                        AND dpt.CD_ENDERECO = dp.CD_ENDERECO
                        AND dpt.CD_PRODUTO = dp.CD_PRODUTO
                        AND dpt.CD_FAIXA = dp.CD_FAIXA
                        AND dpt.NU_IDENTIFICADOR = dp.NU_IDENTIFICADOR
                        AND dpt.NU_SEQ_PREPARACION = dp.NU_SEQ_PREPARACION";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                CD_FUNCIONARIO = :Usuario,
                QT_PRODUTO = QT_PRODUTO - QT_ANULAR";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdatePreparacionCantidadPendienteLpnParcial(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new DynamicParameters(new
            {
                FechaModificacion = DateTime.Now,
                Transaccion = nuTransaccion,
                EstadoWhere = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE
            });

            var alias = "dp";
            var from = $@"
                T_DET_PICKING_LPN dp
                INNER JOIN (
                    SELECT 
                        dpl.NU_PREPARACION,
                        dpl.ID_DET_PICKING_LPN,
                        max(dpt.QT_ANULAR) QT_ANULAR
                    FROM  T_DET_PICKING_TEMP  dpt
                    INNER JOIN T_DET_PICKING_LPN dpl on dpt.NU_PREPARACION = dpl.NU_PREPARACION
                            AND dpt.ID_DET_PICKING_LPN = dpl.ID_DET_PICKING_LPN
                    INNER JOIN T_DET_PICKING dp ON dp.CD_PRODUTO =  dpt.CD_PRODUTO AND
                        dp.CD_EMPRESA =  dpt.CD_EMPRESA AND
                        dp.NU_IDENTIFICADOR =  dpt.NU_IDENTIFICADOR AND
                        dp.CD_FAIXA =  dpt.CD_FAIXA AND
                        dp.NU_PREPARACION =  dpt.NU_PREPARACION AND
                        dp.NU_PEDIDO =  dpt.NU_PEDIDO AND
                        dp.CD_CLIENTE =  dpt.CD_CLIENTE AND
                        dp.CD_ENDERECO =  dpt.CD_ENDERECO AND
                        dp.NU_SEQ_PREPARACION =  dpt.NU_SEQ_PREPARACION AND
                        dp.ND_ESTADO = :EstadoWhere
                    WHERE dpt.QT_ANULAR <> dpt.QT_PRODUTO
                        AND dpt.QT_ANULAR is not null
                    GROUP by dpl.NU_PREPARACION,
                        dpl.ID_DET_PICKING_LPN
                ) dpt ON  dpt.NU_PREPARACION = dp.NU_PREPARACION
                        AND dpt.ID_DET_PICKING_LPN = dp.ID_DET_PICKING_LPN";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                QT_RESERVA = QT_RESERVA - QT_ANULAR";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateAnulacionParcialPreparacionTemporal(DbConnection connection, DbTransaction tran, List<AnulacionPreparacionDetalle> pedidosAsociar)
        {
            _dapper.BulkUpdate(connection, tran, pedidosAsociar, "T_DET_PICKING_TEMP", new Dictionary<string, Func<AnulacionPreparacionDetalle, ColumnInfo>>
            {
                { "NU_SEQ_PREPARACION_DEST", x => new ColumnInfo(x.NumeroSecuenciaDest, DbType.Int32)},
                { "ID_DET_PICKING_LPN_DEST", x => new ColumnInfo(x.IdDetallePickingLpnDest, DbType.Int64)}
            }, new Dictionary<string, Func<AnulacionPreparacionDetalle, ColumnInfo>>
            {
                { "NU_PREPARACION", x => new ColumnInfo(x.NumeroPreparacion)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x =>new ColumnInfo( x.Lote)},
                { "NU_SEQ_PREPARACION", x => new ColumnInfo(x.NumeroSecuencia)},
            });
        }

        public virtual void UpdateAnulacionPreparacionEmpresa(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {

            var param = new DynamicParameters(new
            {
                FechaModificacion = DateTime.Now,
                Transaccion = nuTransaccion,
                Usuario = _userId,
                Estado = EstadoDetallePreparacion.ESTADO_ANULACION_PENDIENTE,
                EstadoWhere = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE
            });

            var alias = "dp";
            var from = $@"
                T_DET_PICKING dp
                INNER JOIN (
                    SELECT 
                        dp.NU_PREPARACION,
                        dp.NU_PEDIDO,
                        dp.CD_CLIENTE,
                        dp.CD_EMPRESA,
                        dp.CD_ENDERECO,
                        dp.CD_PRODUTO,
                        dp.CD_FAIXA,
                        dp.NU_IDENTIFICADOR,
                        dp.NU_SEQ_PREPARACION
                    FROM T_DET_PICKING dp
                    INNER JOIN T_DET_PICKING_TEMP dpt on dpt.NU_PREPARACION = dp.NU_PREPARACION
                            AND dpt.NU_PEDIDO = dp.NU_PEDIDO
                            AND dpt.CD_CLIENTE = dp.CD_CLIENTE
                            AND dpt.CD_EMPRESA = dp.CD_EMPRESA
                            AND dpt.CD_ENDERECO = dp.CD_ENDERECO
                            AND dpt.CD_PRODUTO = dp.CD_PRODUTO
                            AND dpt.CD_FAIXA = dp.CD_FAIXA
                            AND dpt.NU_IDENTIFICADOR = dp.NU_IDENTIFICADOR
                            AND dpt.NU_SEQ_PREPARACION = dp.NU_SEQ_PREPARACION
                            AND (dpt.QT_ANULAR is null OR dpt.QT_ANULAR = dpt.QT_PRODUTO)
                   INNER JOIN V_EMPRESA_DOCUMENTAL ed on ed.CD_EMPRESA = DPT.CD_EMPRESA and ed.FL_DOCUMENTAL = 'N'
                   WHERE dp.ND_ESTADO = :EstadoWhere
                   GROUP by dp.NU_PREPARACION,
                        dp.NU_PEDIDO,
                        dp.CD_CLIENTE,
                        dp.CD_EMPRESA,
                        dp.CD_ENDERECO,
                        dp.CD_PRODUTO,
                        dp.CD_FAIXA,
                        dp.NU_IDENTIFICADOR,
                        dp.NU_SEQ_PREPARACION
                ) dpt ON  dpt.NU_PREPARACION = dp.NU_PREPARACION
                        AND dpt.NU_PEDIDO = dp.NU_PEDIDO
                        AND dpt.CD_CLIENTE = dp.CD_CLIENTE
                        AND dpt.CD_EMPRESA = dp.CD_EMPRESA
                        AND dpt.CD_ENDERECO = dp.CD_ENDERECO
                        AND dpt.CD_PRODUTO = dp.CD_PRODUTO
                        AND dpt.CD_FAIXA = dp.CD_FAIXA
                        AND dpt.NU_IDENTIFICADOR = dp.NU_IDENTIFICADOR
                        AND dpt.NU_SEQ_PREPARACION = dp.NU_SEQ_PREPARACION";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                CD_FUNCIONARIO = :Usuario,
                ND_ESTADO = :Estado
                ";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateAnulacionPreparacionEmpresaDocumental(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new DynamicParameters(new
            {
                FechaModificacion = DateTime.Now,
                Transaccion = nuTransaccion,
                Usuario = _userId,
                Estado = EstadoDetallePreparacion.ESTADO_ANULACION_DOC_PEND,
                EstadoWhere = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE
            });

            var alias = "dp";
            var from = @$"
                T_DET_PICKING dp
                INNER JOIN (
                    SELECT 
                        dp.NU_PREPARACION,
                        dp.NU_PEDIDO,
                        dp.CD_CLIENTE,
                        dp.CD_EMPRESA,
                        dp.CD_ENDERECO,
                        dp.CD_PRODUTO,
                        dp.CD_FAIXA,
                        dp.NU_IDENTIFICADOR,
                        dp.NU_SEQ_PREPARACION
                    FROM T_DET_PICKING dp
                    INNER JOIN T_DET_PICKING_TEMP dpt on dpt.NU_PREPARACION = dp.NU_PREPARACION
                            AND dpt.NU_PEDIDO = dp.NU_PEDIDO
                            AND dpt.CD_CLIENTE = dp.CD_CLIENTE
                            AND dpt.CD_EMPRESA = dp.CD_EMPRESA
                            AND dpt.CD_ENDERECO = dp.CD_ENDERECO
                            AND dpt.CD_PRODUTO = dp.CD_PRODUTO
                            AND dpt.CD_FAIXA = dp.CD_FAIXA
                            AND dpt.NU_IDENTIFICADOR = dp.NU_IDENTIFICADOR
                            AND dpt.NU_SEQ_PREPARACION = dp.NU_SEQ_PREPARACION
                            AND (dpt.QT_ANULAR is null OR dpt.QT_ANULAR = dpt.QT_PRODUTO)
                   INNER JOIN V_EMPRESA_DOCUMENTAL ed on ed.CD_EMPRESA = dp.CD_EMPRESA and ed.FL_DOCUMENTAL = 'S'
                   WHERE dp.ND_ESTADO = :EstadoWhere
                   GROUP by dp.NU_PREPARACION,
                        dp.NU_PEDIDO,
                        dp.CD_CLIENTE,
                        dp.CD_EMPRESA,
                        dp.CD_ENDERECO,
                        dp.CD_PRODUTO,
                        dp.CD_FAIXA,
                        dp.NU_IDENTIFICADOR,
                        dp.NU_SEQ_PREPARACION
                ) dpt ON  dpt.NU_PREPARACION = dp.NU_PREPARACION
                        AND dpt.NU_PEDIDO = dp.NU_PEDIDO
                        AND dpt.CD_CLIENTE = dp.CD_CLIENTE
                        AND dpt.CD_EMPRESA = dp.CD_EMPRESA
                        AND dpt.CD_ENDERECO = dp.CD_ENDERECO
                        AND dpt.CD_PRODUTO = dp.CD_PRODUTO
                        AND dpt.CD_FAIXA = dp.CD_FAIXA
                        AND dpt.NU_IDENTIFICADOR = dp.NU_IDENTIFICADOR
                        AND dpt.NU_SEQ_PREPARACION = dp.NU_SEQ_PREPARACION";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                CD_FUNCIONARIO = :Usuario,
                ND_ESTADO = :Estado
                ";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }


        #endregion

        #region Insert

        public virtual void InsertDetallePreparacionParcialLpnAnulacion(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new DynamicParameters(new
            {
                DT_ADDROW = DateTime.Now,
                ND_ESTADO = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE,
                NU_TRANSACCION = nuTransaccion,
                EstadoWhere = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE
            });
            string sql = @" INSERT INTO T_DET_PICKING_LPN
                    (
                        NU_PREPARACION,
                        ID_DET_PICKING_LPN,
                        ID_LPN_DET,
                        NU_LPN,
                        CD_EMPRESA,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        VL_ATRIBUTOS,
                        QT_RESERVA,
                        TP_LPN_TIPO,
                        CD_ENDERECO,
                        DT_ADDROW,
                        NU_TRANSACCION,
                        ID_LPN_EXTERNO,
                        NU_DET_PED_SAI_ATRIB
                    )
                    SELECT
                        dpt.NU_PREPARACION,
                        dpt.ID_DET_PICKING_LPN_DEST,
                        dpl.ID_LPN_DET,
                        dpl.NU_LPN,
                        dpl.CD_EMPRESA,
                        dpl.CD_PRODUTO,
                        dpl.CD_FAIXA,
                        dpl.NU_IDENTIFICADOR,
                        dpl.VL_ATRIBUTOS,
                        dpt.QT_ANULAR,
                        dpl.TP_LPN_TIPO,
                        dpl.CD_ENDERECO,
                        :DT_ADDROW,
                        :NU_TRANSACCION,
                        dpl.ID_LPN_EXTERNO,
                        dpl.NU_DET_PED_SAI_ATRIB
                    FROM T_DET_PICKING_TEMP dpt
                    INNER JOIN T_DET_PICKING_LPN dpl ON  dpl.NU_PREPARACION = dpt.NU_PREPARACION 
                        AND dpl.ID_DET_PICKING_LPN = dpt.ID_DET_PICKING_LPN
                    INNER JOIN T_DET_PICKING dp ON dp.CD_PRODUTO =  dpt.CD_PRODUTO AND
                        dp.CD_EMPRESA =  dpt.CD_EMPRESA AND
                        dp.NU_IDENTIFICADOR =  dpt.NU_IDENTIFICADOR AND
                        dp.CD_FAIXA =  dpt.CD_FAIXA AND
                        dp.NU_PREPARACION =  dpt.NU_PREPARACION AND
                        dp.NU_PEDIDO =  dpt.NU_PEDIDO AND
                        dp.CD_CLIENTE =  dpt.CD_CLIENTE AND
                        dp.CD_ENDERECO =  dpt.CD_ENDERECO AND
                        dp.NU_SEQ_PREPARACION =  dpt.NU_SEQ_PREPARACION AND
                        dp.ND_ESTADO = :EstadoWhere
                    WHERE dpt.QT_ANULAR <> dpt.QT_PRODUTO
                        AND dpt.QT_ANULAR is not null";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void InsertDetallePreparacionParcialAnulacion(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new DynamicParameters(new
            {
                DT_ADDROW = DateTime.Now,
                CD_FUNCIONARIO = _userId,
                ND_ESTADO = EstadoDetallePreparacion.ESTADO_ANULACION_PENDIENTE,
                NU_TRANSACCION = nuTransaccion,
                EstadoWhere = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE
            });
            string sql = @" INSERT INTO T_DET_PICKING
                    (
                        NU_PREPARACION,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        CD_EMPRESA,
                        CD_ENDERECO,
                        NU_PEDIDO,
                        CD_CLIENTE,
                        NU_SEQ_PREPARACION,
                        ID_ESPECIFICA_IDENTIFICADOR,
                        NU_CARGA,
                        ID_AGRUPACION,
                        QT_PRODUTO,
                        CD_FUNCIONARIO,
                        DT_ADDROW,
                        NU_TRANSACCION,
                        ND_ESTADO,
                        ID_DET_PICKING_LPN
                    )
                    SELECT
                        dpt.NU_PREPARACION,
                        dpt.CD_PRODUTO,
                        dpt.CD_FAIXA,
                        dpt.NU_IDENTIFICADOR,
                        dpt.CD_EMPRESA,
                        dpt.CD_ENDERECO,
                        dpt.NU_PEDIDO,
                        dpt.CD_CLIENTE,
                        dpt.NU_SEQ_PREPARACION_DEST,
                        DP.ID_ESPECIFICA_IDENTIFICADOR,
                        dp.NU_CARGA,
                        dp.ID_AGRUPACION,
                        dpt.QT_ANULAR,
                        :CD_FUNCIONARIO,
                        :DT_ADDROW,
                        :NU_TRANSACCION,
                        :ND_ESTADO,
                        dpt.ID_DET_PICKING_LPN_DEST
                    FROM T_DET_PICKING_TEMP dpt
                    INNER JOIN T_DET_PICKING dp ON  dp.CD_PRODUTO =  dpt.CD_PRODUTO AND
                        dp.CD_EMPRESA =  dpt.CD_EMPRESA AND
                        dp.NU_IDENTIFICADOR =  dpt.NU_IDENTIFICADOR AND
                        dp.CD_FAIXA =  dpt.CD_FAIXA AND
                        dp.NU_PREPARACION =  dpt.NU_PREPARACION AND
                        dp.NU_PEDIDO =  dpt.NU_PEDIDO AND
                        dp.CD_CLIENTE =  dpt.CD_CLIENTE AND
                        dp.CD_ENDERECO =  dpt.CD_ENDERECO AND
                        dp.NU_SEQ_PREPARACION =  dpt.NU_SEQ_PREPARACION 
                    INNER JOIN V_EMPRESA_DOCUMENTAL ed on ed.CD_EMPRESA = DPT.CD_EMPRESA and ed.FL_DOCUMENTAL = 'N'
                    WHERE dp.ND_ESTADO = :EstadoWhere
                        AND dpt.QT_ANULAR <> dpt.QT_PRODUTO
                        AND dpt.QT_ANULAR is not null";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void InsertDetallePreparacionParcialAnulacionEmpresaDocumental(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new DynamicParameters(new
            {
                DT_ADDROW = DateTime.Now,
                CD_FUNCIONARIO = _userId,
                ND_ESTADO = EstadoDetallePreparacion.ESTADO_ANULACION_DOC_PEND,
                NU_TRANSACCION = nuTransaccion,
                EstadoWhere = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE
            });
            string sql = @"INSERT INTO T_DET_PICKING
                    (
                        NU_PREPARACION,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        CD_EMPRESA,
                        CD_ENDERECO,
                        NU_PEDIDO,
                        CD_CLIENTE,
                        NU_SEQ_PREPARACION,
                        ID_ESPECIFICA_IDENTIFICADOR,
                        NU_CARGA,
                        ID_AGRUPACION,
                        QT_PRODUTO,
                        CD_FUNCIONARIO,
                        DT_ADDROW,
                        NU_TRANSACCION,
                        ND_ESTADO,
                        ID_DET_PICKING_LPN
                    )
                    SELECT
                        dpt.NU_PREPARACION,
                        dpt.CD_PRODUTO,
                        dpt.CD_FAIXA,
                        dpt.NU_IDENTIFICADOR,
                        dpt.CD_EMPRESA,
                        dpt.CD_ENDERECO,
                        dpt.NU_PEDIDO,
                        dpt.CD_CLIENTE,
                        dpt.NU_SEQ_PREPARACION_DEST,
                        DP.ID_ESPECIFICA_IDENTIFICADOR,
                        dp.NU_CARGA,
                        dp.ID_AGRUPACION,
                        dpt.QT_ANULAR,
                        :CD_FUNCIONARIO,
                        :DT_ADDROW,
                        :NU_TRANSACCION,
                        :ND_ESTADO,
                        dpt.ID_DET_PICKING_LPN_DEST
                    FROM T_DET_PICKING_TEMP dpt
                    INNER JOIN T_DET_PICKING dp ON  dp.CD_PRODUTO =  dpt.CD_PRODUTO AND
                        dp.CD_EMPRESA =  dpt.CD_EMPRESA AND
                        dp.NU_IDENTIFICADOR =  dpt.NU_IDENTIFICADOR AND
                        dp.CD_FAIXA =  dpt.CD_FAIXA AND
                        dp.NU_PREPARACION =  dpt.NU_PREPARACION AND
                        dp.NU_PEDIDO =  dpt.NU_PEDIDO AND
                        dp.CD_CLIENTE =  dpt.CD_CLIENTE AND
                        dp.CD_ENDERECO =  dpt.CD_ENDERECO AND
                        dp.NU_SEQ_PREPARACION =  dpt.NU_SEQ_PREPARACION 
                    INNER JOIN V_EMPRESA_DOCUMENTAL ed on ed.CD_EMPRESA = DPT.CD_EMPRESA and ed.FL_DOCUMENTAL = 'S'
                    WHERE dp.ND_ESTADO = :EstadoWhere
                        AND dpt.QT_ANULAR <> dpt.QT_PRODUTO
                        AND dpt.QT_ANULAR is not null";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void AddAnulacionPreparacionTemporal(DbConnection connection, DbTransaction tran, List<DetallePreparacion> detallesMarcarAnular)
        {
            _dapper.BulkInsert(connection, tran, detallesMarcarAnular, "T_DET_PICKING_TEMP", new Dictionary<string, Func<DetallePreparacion, ColumnInfo>>
            {
                { "NU_PREPARACION", x => new ColumnInfo(x.NumeroPreparacion)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
                { "NU_SEQ_PREPARACION", x => new ColumnInfo(x.NumeroSecuencia)}
            });
        }

        public virtual void AddAnulacionParcialPreparacionTemporal(DbConnection connection, DbTransaction tran, List<DetallePreparacion> detallesMarcarAnular, long nuTransaccion)
        {
            _dapper.BulkInsert(connection, tran, detallesMarcarAnular, "T_DET_PICKING_TEMP", new Dictionary<string, Func<DetallePreparacion, ColumnInfo>>
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
                { "QT_PRODUTO" , x => new ColumnInfo(x.Cantidad)},
                { "QT_ANULAR" , x => new ColumnInfo(x.CantidadAnular)},
                { "ID_DET_PICKING_LPN" ,x => new ColumnInfo( x.IdDetallePickingLpn,DbType.Int64)}
            });
        }

        public virtual void AddAnulacionesPendientes(DbConnection connection, DbTransaction tran, List<AnulacionPreparacion> preparacionesNoPendientes)
        {
            _dapper.BulkInsert(connection, tran, preparacionesNoPendientes, "T_ANULACION_PREPARACION", new Dictionary<string, Func<AnulacionPreparacion, ColumnInfo>>
            {
                { "NU_ANULACION_PREPARACION", x => new ColumnInfo(x.NroAnulacionPreparacion)},
                { "NU_PREPARACION", x => new ColumnInfo(x.Preparacion)},
                { "ND_ESTADO", x => new ColumnInfo(x.Estado)},
                { "DS_ANULACION", x => new ColumnInfo(x.Descripcion)},
                { "DT_ADDROW", x => new ColumnInfo(x.Alta)},
                { "TP_ANULACION", x => new ColumnInfo(x.TipoAnulacion)},
                { "TP_AGRUPACION", x => new ColumnInfo(x.TipoAgrupacion)},
                { "USERID", x => new ColumnInfo(x.UserId)}
            });
        }

        #endregion

        #region Get

        public virtual List<AnulacionPreparacionDetalle> GetDetallePreparacionAnulacionParcial(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            NU_PREPARACION as NumeroPreparacion,
                            CD_CLIENTE as Cliente,
                            CD_EMPRESA as Empresa,
                            NU_PEDIDO as Pedido,
                            CD_ENDERECO as Ubicacion,
                            CD_PRODUTO as Producto,
                            CD_FAIXA as Faixa,
                            NU_IDENTIFICADOR as Lote,
                            NU_SEQ_PREPARACION as NumeroSecuencia,
                            ID_DET_PICKING_LPN as IdDetallePickingLpn
                        FROM
                            T_DET_PICKING_TEMP
                        WHERE   QT_ANULAR <> QT_PRODUTO
                            AND QT_ANULAR is not null";

            return _dapper.Query<AnulacionPreparacionDetalle>(connection, sql, transaction: tran, commandType: CommandType.Text).ToList();
        }

        public virtual List<AnulacionPreparacion> GetAnulacionesNoPendientes(DbConnection connection, DbTransaction tran, List<Preparacion> preparaciones)
        {
            IEnumerable<AnulacionPreparacion> resultado = new List<AnulacionPreparacion>();
            List<string> estados = new List<string> { EstadoAnulacion.EnProceso, EstadoAnulacion.AnulacionPendiente };

            _dapper.BulkInsert(connection, tran, preparaciones, "T_PICKING_TEMP", new Dictionary<string, Func<Preparacion, ColumnInfo>>
            {
                { "NU_PREPARACION", x => new ColumnInfo(x.Id)}
            });

            string sql = @"SELECT 
                        P.NU_PREPARACION as  Preparacion
                        FROM T_PICKING_TEMP   P 
                        LEFT JOIN T_ANULACION_PREPARACION T ON P.NU_PREPARACION = T.NU_PREPARACION AND T.ND_ESTADO in ('SANUEPR','SANUPEN')
                        LEFT JOIN T_TRASPASO TR ON TR.NU_PREPARACION = P.NU_PREPARACION AND TR.ID_ESTADO in ('ESTRASP_EN_EDICION','ESTRASP_EN_PROCESO')
                        WHERE T.NU_PREPARACION is null AND
                            TR.NU_TRASPASO is null";

            resultado = _dapper.Query<AnulacionPreparacion>(connection, sql, transaction: tran);

            _dapper.BulkDelete(connection, tran, preparaciones, "T_PICKING_TEMP", new Dictionary<string, Func<Preparacion, object>>
            {
                { "NU_PREPARACION", x => x.Id}
            });

            return resultado.ToList();
        }

        #endregion

        #region Delete

        public virtual void DeleteAnulacionPreparacionTemporal(DbConnection connection, DbTransaction tran, List<DetallePreparacion> detallesMarcarAnular, long nuTransaccion)
        {
            _dapper.BulkDelete(connection, tran, detallesMarcarAnular, "T_DET_PICKING_TEMP", new Dictionary<string, Func<DetallePreparacion, object>>
            {
                { "NU_PREPARACION", x => x.NumeroPreparacion},
                { "NU_PEDIDO", x => x.Pedido},
                { "CD_CLIENTE", x => x.Cliente},
                { "CD_EMPRESA", x => x.Empresa},
                { "CD_ENDERECO", x => x.Ubicacion},
                { "CD_PRODUTO", x => x.Producto},
                { "CD_FAIXA", x => x.Faixa},
                { "NU_IDENTIFICADOR", x => x.Lote},
                { "NU_SEQ_PREPARACION", x => x.NumeroSecuencia}
            });
        }

        public virtual void DeleteAnulacionPreparacionSinDetalles(DbConnection connection, DbTransaction tran)
        {
            var estadosAnulacionPendiente = EstadoDetallePreparacion.GetEstadosAnulacionPendiente();
            var sql = @"SELECT 
                            ap.NU_ANULACION_PREPARACION as NroAnulacionPreparacion,
                            ap.NU_PREPARACION as Preparacion
                        FROM T_ANULACION_PREPARACION ap
                        LEFT JOIN T_DET_PICKING dp ON ap.NU_PREPARACION = dp.NU_PREPARACION AND dp.ND_ESTADO IN :estadosAnulacionPendiente 
                        WHERE ap.ND_ESTADO = 'SANUPEN' AND dp.NU_PREPARACION IS NULL                         
                        GROUP BY
                            ap.NU_ANULACION_PREPARACION,
                            ap.NU_PREPARACION ";

            var anulacionesAEliminar = _dapper.Query<AnulacionPreparacion>(connection, sql, new
            {
                estadosAnulacionPendiente = estadosAnulacionPendiente
            }, commandType: CommandType.Text, transaction: tran).ToList();

            _dapper.BulkDelete(connection, tran, anulacionesAEliminar, "T_ANULACION_PREPARACION", new Dictionary<string, Func<AnulacionPreparacion, object>>
            {
                { "NU_ANULACION_PREPARACION", x => x.NroAnulacionPreparacion},
                { "NU_PREPARACION", x => x.Preparacion}
            });
        }

        #endregion

        #endregion

        #endregion
    }
}