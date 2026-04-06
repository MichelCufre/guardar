using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class PtlRepository
    {
        protected readonly WISDB _context;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly string _aplicacion;

        public PtlRepository(WISDB context, string aplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._dapper = dapper;
            this._userId = userId;
            this._aplicacion = aplicacion;
        }

        #region Any

        public virtual bool AnyDetallePickingPendienteAutomatismo(int preparacion, int empresa, string cliente)
        {
            return _context.T_DET_PICKING
                .AsNoTracking()
                .Any(x => x.NU_PREPARACION == preparacion
                    && x.CD_CLIENTE == cliente
                    && x.CD_EMPRESA == empresa
                    && x.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO);
        }

        public virtual bool AnyDetallePreparacionPendienteAutomatismo(int preparacion, int empresa, string cliente)
        {
            return _context.T_DET_PICKING
                .AsNoTracking()
                .Any(x => x.NU_PREPARACION == preparacion
                    && x.CD_CLIENTE == cliente
                    && x.CD_EMPRESA == empresa
                    && x.ND_ESTADO == Mappers.Constants.EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO);
        }

        #endregion

        #region Dapper

        public virtual List<DetallePedido> GetPickingAgrupadoComparteCont(int preparacion, int contenedor, int cdEmpresa, string cdProduto, int tipoAgrupacion)//GERE: Mover a WmsApi
        {
            var agrupacion = GetSqlAgrupacion("PR", "CL", "PS", "PS.VL_COMPARTE_CONTENEDOR_PICKING", "PS.VL_COMPARTE_CONTENEDOR_PICKING", "");

            string sql = $@"
                SELECT
                    DP.CD_FAIXA Faixa,
                    SUM(DP.qt_preparado) CantidadPreparada,
                    ({agrupacion} ) Agrupacion
                FROM T_DET_PICKING dp 
                INNER JOIN T_PRODUTO PR ON PR.CD_PRODUTO =DP.CD_PRODUTO 
                    AND PR.CD_EMPRESA =DP.CD_EMPRESA
                INNER JOIN T_CLASSE CL ON cl.CD_CLASSE = PR.CD_CLASSE
                INNER JOIN T_PEDIDO_SAIDA PS ON DP.NU_PEDIDO = PS.NU_PEDIDO 
                    AND PS.CD_CLIENTE =DP.CD_CLIENTE 
                    AND PS.CD_EMPRESA =DP.CD_EMPRESA
                INNER JOIN T_TIPO_AGRUPACION_ENDERECO TAE ON TAE.TP_AGRUPACION_UBIC = :P_TP_AGRUPACION_UBIC
                WHERE DP.ND_ESTADO = 'ESTAD_PREPARADO' 
                    AND DP.NU_PREPARACION = :P_NU_PREPARACION 
                    AND DP.NU_CONTENEDOR = :P_NU_CONTENEDOR 
                    AND DP.CD_EMPRESA = :P_CD_EMPRESA 
                    AND DP.CD_PRODUTO = :P_CD_PRODUTO
                GROUP BY 
                    DP.CD_FAIXA, 
                    ({agrupacion})";

            var query = _dapper.Query<DetallePedido>(_context.Database.GetDbConnection(), sql, new
            {
                P_TP_AGRUPACION_UBIC = tipoAgrupacion,
                P_NU_PREPARACION = preparacion,
                P_NU_CONTENEDOR = contenedor,
                P_CD_PRODUTO = cdProduto,
                P_CD_EMPRESA = cdEmpresa,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.ToList();
        }

        public virtual bool AnyPickingPendienteEnUbicacionesAutomatismo(int preparacion, int empresa, string cliente)
        {
            string sql = $@"  
                SELECT 
                    COUNT(*)
                FROM T_DET_PICKING PICK 
                INNER JOIN T_ENDERECO_ESTOQUE ENDE ON ENDE.CD_ENDERECO = PICK.CD_ENDERECO
                INNER JOIN T_ZONA_UBICACION ZONA ON ZONA.CD_ZONA_UBICACION = ENDE.CD_ZONA_UBICACION
                WHERE ZONA.TP_ZONA_UBICACION = 'AUTOMATISMO' 
                    AND PICK.ND_ESTADO in ('ESTAD_PREP_PEND','ESTAD_PEND_AUT') 
                    AND PICK.NU_PREPARACION = :preparacion 
                    AND PICK.CD_EMPRESA = :empresa 
                    AND PICK.CD_CLIENTE = :cliente";

            var query = _dapper.Query<int>(_context.Database.GetDbConnection(), sql, new
            {
                preparacion = preparacion,
                cliente = cliente,
                empresa = empresa,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.FirstOrDefault() > 0;
        }

        public virtual List<DetallePreparacion> GetDetallesPreparacionDisponibles(int preparacion, string cliente, string vlComparteContenedorPicking, string subClase)//GERE: Mover a WmsApi
        {
            string sql = $@"
                SELECT 
                    PICK.CD_ENDERECO Ubicacion,
                    SUM(PICK.QT_PRODUTO) Cantidad
                FROM T_DET_PICKING PICK 
                INNER JOIN T_PEDIDO_SAIDA PED ON PED.NU_PEDIDO = PICK.NU_PEDIDO 
                    AND PED.CD_CLIENTE = PICK.CD_CLIENTE 
                    AND PED.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_PRODUTO PROD ON PROD.CD_PRODUTO = PICK.CD_PRODUTO 
                    AND PROD.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_CLASSE CLA ON   CLA.CD_CLASSE = PROD.CD_CLASSE
                WHERE PICK.NU_PREPARACION = :preparacion 
                    AND PICK.ND_ESTADO = :P_ND_ESTADO 
                    AND PED.CD_CLIENTE = :cliente 
                    AND COALESCE(PED.VL_COMPARTE_CONTENEDOR_PICKING,'(SIN AGRUPACION)') = :vlComparteContenedorPicking 
                    AND COALESCE(CLA.CD_SUB_CLASSE,'NULL') = :subClase
                GROUP BY PICK.CD_ENDERECO";

            var query = _dapper.Query<DetallePreparacion>(_context.Database.GetDbConnection(), sql, new
            {
                preparacion = preparacion,
                P_ND_ESTADO = Mappers.Constants.EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO,
                cliente = cliente,
                vlComparteContenedorPicking = vlComparteContenedorPicking,
                subClase = string.IsNullOrEmpty(subClase) ? "NULL" : subClase,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.ToList();
        }

        public virtual bool AnyMercaderiaEnUbicacion(string ubicacion)//GERE: Mover a WmsApi  
        {
            string sql = $@"  
                SELECT COUNT(*)
                FROM T_STOCK 
                WHERE CD_ENDERECO = :P_CD_ENDERECO";

            var query = _dapper.Query<int>(_context.Database.GetDbConnection(), sql, new
            {
                P_CD_ENDERECO = ubicacion
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.FirstOrDefault() > 0;
        }

        public virtual List<LineaCrossDocking> GetCrossDockingAgrupadoCliente(int agenda, string idEspecificaIdentificador, string lote, int cdEmpresa, string cdProduto, int tipoAgrupacion)//GERE: Mover a WmsApi
        {
            DynamicParameters parameters = new DynamicParameters();
            var agrupacion = GetSqlAgrupacion("PR", "CL", "CR", "CR.CD_CLIENTE", "CR.CD_CLIENTE", "CR.NU_AGENDA");

            string sql = $@"
                SELECT
                    CR.CD_CLIENTE Cliente,
                    CR.CD_FAIXA Faixa,
                    SUM(CR.QT_PEND_XD) Cantidad,
                    ({agrupacion}) Agrupacion
                FROM ( 
                    SELECT
                        CR.CD_CLIENTE ,
                        CR.CD_EMPRESA,
                        CR.CD_FAIXA,
                        CR.NU_AGENDA,
                        CR.CD_PRODUTO,
                        CR.QT_PEND_XD -  COALESCE(EACT.QT_PRODUTO_ETIQUETA,0) QT_PEND_XD
                    FROM (
                        SELECT
                            CR.NU_AGENDA,
                            CR.CD_CLIENTE,
                            CR.CD_PRODUTO,
                            CR.CD_EMPRESA,
                            CR.CD_FAIXA,
                            SUM(CR.QT_PRODUTO - CR.QT_PREPARADO) QT_PEND_XD 
                        FROM T_DET_CROSS_DOCK  CR
                        WHERE CR.NU_IDENTIFICADOR = :P_NU_IDENTIFICADOR1 
                            AND CR.NU_AGENDA = :P_NU_AGENDA 
                            AND CR.CD_EMPRESA = :P_CD_EMPRESA 
                            AND CR.CD_PRODUTO = :P_CD_PRODUTO 
                        GROUP BY 
                            CR.NU_AGENDA,
                            CR.CD_CLIENTE,
                            CR.CD_PRODUTO,
                            CR.CD_EMPRESA,
                            CR.CD_FAIXA
                    ) CR
                    LEFT JOIN (
                        SELECT 
                            ET.NU_AGENDA,
                            ET.CD_CLIENTE,
                            DET.CD_PRODUTO,
                            DET.CD_EMPRESA,
                            DET.CD_FAIXA,
                            SUM(COALESCE(QT_PRODUTO,0)) QT_PRODUTO_ETIQUETA
                        FROM T_ETIQUETA_LOTE et 
                        INNER JOIN T_DET_ETIQUETA_LOTE det ON DET.NU_ETIQUETA_LOTE = ET.NU_ETIQUETA_LOTE
                        WHERE ET.CD_SITUACAO = 23  
                            AND ET.CD_CLIENTE is not null 
                            AND DET.QT_PRODUTO > 0 
                            AND DET.NU_IDENTIFICADOR = :P_NU_IDENTIFICADOR2
                        GROUP BY 
                            ET.NU_AGENDA,
                            ET.CD_CLIENTE,
                            DET.CD_PRODUTO,
                            DET.CD_EMPRESA,
                            DET.CD_FAIXA
                    ) EACT ON EACT.NU_AGENDA = CR.NU_AGENDA 
                        AND EACT.CD_CLIENTE = CR.CD_CLIENTE 
                        AND EACT.CD_PRODUTO = CR.CD_PRODUTO 
                        AND EACT.CD_EMPRESA = CR.CD_EMPRESA 
                        AND EACT.CD_FAIXA = CR.CD_FAIXA
                ) CR
                INNER JOIN T_PRODUTO PR ON PR.CD_PRODUTO = CR.CD_PRODUTO 
                    AND PR.CD_EMPRESA = CR.CD_EMPRESA
                INNER JOIN T_CLASSE CL ON cl.CD_CLASSE = PR.CD_CLASSE
                INNER JOIN T_TIPO_AGRUPACION_ENDERECO TAE ON TAE.TP_AGRUPACION_UBIC = :P_TP_AGRUPACION
                WHERE CR.QT_PEND_XD > 0
                GROUP BY 
                    CR.CD_CLIENTE,
                    CR.CD_FAIXA, 
                    ({agrupacion})
            ";

            var query = _dapper.Query<LineaCrossDocking>(_context.Database.GetDbConnection(), sql, new
            {
                P_TP_AGRUPACION = tipoAgrupacion,
                P_NU_AGENDA = agenda,
                P_CD_EMPRESA = cdEmpresa,
                P_CD_PRODUTO = cdProduto,
                P_NU_IDENTIFICADOR1 = lote,
                P_NU_IDENTIFICADOR2 = lote,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.ToList();
        }

        public virtual bool AnyPendientesSeperar(int? tipoAgrupacion, string comparteAgrupacion, int tipoOperacion)
        {
            string sql = null;

            if (tipoOperacion == TipoOperacion.Preparacion)
            {
                var agrupacion = GetSqlAgrupacion("PR", "CL", "PS", "PS.VL_COMPARTE_CONTENEDOR_PICKING", "PS.VL_COMPARTE_CONTENEDOR_ENTREGA", "");
                sql = $@"
                    SELECT COUTN(*) 
                    FROM (
                        SELECT 
                            DP.NU_PEDIDO,
                            DP.CD_CLIENTE,
                            DP.CD_EMPRESA,
                            DP.CD_PRODUTO 
                        FROM T_CONTENEDOR co
                        INNER JOIN T_DET_PICKING dp ON CO.NU_PREPARACION = DP.NU_PREPARACION 
                            AND CO.NU_CONTENEDOR = DP.NU_CONTENEDOR 
                            AND DP.ND_ESTADO = 'ESTAD_PREPARADO' 
                        WHERE  CO.CD_SITUACAO = 601 
                            AND COALESCE(CO.FL_SEPARADO_DOS_FASES,'N') = 'N'
                        GROUP BY 
                            DP.NU_PEDIDO,
                            DP.CD_CLIENTE,
                            DP.CD_EMPRESA,
                            DP.CD_PRODUTO
                     ) dp
                     INNER JOIN T_PRODUTO PR ON PR.CD_PRODUTO = DP.CD_PRODUTO 
                        AND PR.CD_EMPRESA = DP.CD_EMPRESA
                     INNER JOIN T_CLASSE CL ON cl.CD_CLASSE = PR.CD_CLASSE
                     INNER JOIN T_PEDIDO_SAIDA PS ON PS.ND_ACTIVIDAD = 'ACTIVO' 
                            AND DP.NU_PEDIDO = PS.NU_PEDIDO 
                            AND PS.CD_CLIENTE = DP.CD_CLIENTE 
                            AND PS.CD_EMPRESA =DP.CD_EMPRESA 
                     INNER JOIN T_TIPO_AGRUPACION_ENDERECO TAE ON TAE.TP_AGRUPACION_UBIC = :P_TP_AGRUPACION_UBIC 
                            AND ({agrupacion}) = :P_VL_COMPARTE_AGRUPACION";
            }
            else if (tipoOperacion == TipoOperacion.CrossDockingUnaFase)
            {
                var agrupacion = GetSqlAgrupacion("PR", "CL", "DP", "", "", "DP.NU_AGENDA");
                sql = $@"
                    SELECT COUNT(*) 
                    FROM (
                        SELECT
                            CR.CD_CLIENTE ,
                            CR.CD_EMPRESA,
                            CR.NU_AGENDA,
                            CR.CD_PRODUTO,
                            CR.QT_PEND_XD,
                            COALESCE(EACT.QT_PRODUTO_ETIQUETA,0)
                        FROM (
                            SELECT 
                                CR.NU_AGENDA,
                                CR.CD_CLIENTE,
                                CR.CD_PRODUTO,
                                CR.CD_EMPRESA, 
                                CR.CD_FAIXA,
                                SUM(CR.QT_PRODUTO - CR.QT_PREPARADO) QT_PEND_XD 
                            FROM T_DET_CROSS_DOCK  CR
                            GROUP BY CR.NU_AGENDA,CR.CD_CLIENTE, CR.CD_PRODUTO,CR.CD_EMPRESA, CR.CD_FAIXA
                        ) CR
                        LEFT JOIN (
                            SELECT 
                                ET.NU_AGENDA,
                                ET.CD_CLIENTE,
                                DET.CD_PRODUTO,
                                DET.CD_EMPRESA,
                                DET.CD_FAIXA,
                                SUM(COALESCE(QT_PRODUTO,0)) QT_PRODUTO_ETIQUETA
                            FROM T_ETIQUETA_LOTE et 
                            INNER JOIN T_DET_ETIQUETA_LOTE det ON DET.NU_ETIQUETA_LOTE = ET.NU_ETIQUETA_LOTE
                            WHERE ET.CD_SITUACAO = 23  
                                AND ET.CD_CLIENTE is not null 
                                AND DET.QT_PRODUTO > 0
                            GROUP BY 
                                ET.NU_AGENDA,
                                ET.CD_CLIENTE,
                                DET.CD_PRODUTO,
                                DET.CD_EMPRESA,
                                DET.CD_FAIXA
                        ) EACT ON EACT.NU_AGENDA = CR.NU_AGENDA 
                            AND EACT.CD_CLIENTE = CR.CD_CLIENTE 
                            AND EACT.CD_PRODUTO = CR.CD_PRODUTO 
                            AND EACT.CD_EMPRESA = CR.CD_EMPRESA 
                            AND EACT.CD_FAIXA = CR.CD_FAIXA
                        WHERE CR.QT_PEND_XD -  COALESCE(EACT.QT_PRODUTO_ETIQUETA,0) > 0 
                    ) dp
                    INNER JOIN T_PRODUTO PR ON PR.CD_PRODUTO =DP.CD_PRODUTO 
                        AND PR.CD_EMPRESA =DP.CD_EMPRESA
                    INNER JOIN T_CLASSE CL ON cl.CD_CLASSE = PR.CD_CLASSE
                    INNER JOIN T_TIPO_AGRUPACION_ENDERECO TAE ON TAE.TP_AGRUPACION_UBIC = :P_TP_AGRUPACION_UBIC 
                        AND ({agrupacion}) = :P_VL_COMPARTE_AGRUPACION";
            }

            var query = _dapper.Query<int>(_context.Database.GetDbConnection(), sql, new
            {
                P_TP_AGRUPACION_UBIC = tipoAgrupacion,
                P_VL_COMPARTE_AGRUPACION = comparteAgrupacion,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.FirstOrDefault() > 0;
        }

        public virtual List<AutomatismoPtl> GetAutomatismoByTipoPtl(string tipoPtl)
        {
            var sql = @"
                SELECT 
                    A.NU_AUTOMATISMO nuAutomatismo,
                    A.DS_AUTOMATISMO dsAutomatismo
                FROM T_AUTOMATISMO A
                INNER JOIN T_AUTOMATISMO_CARACTERISTICA C ON A.NU_AUTOMATISMO = C.NU_AUTOMATISMO
                WHERE C.VL_AUTOMATISMO_CARACTERISTICA = :TIPO_PTL";

            var query = _dapper.Query<AutomatismoPtl>(_context.Database.GetDbConnection(), sql, new
            {
                TIPO_PTL = tipoPtl,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.ToList();
        }

        public virtual string GetSqlAgrupacion(string aliasProducto, string aliasClase, string aliasCliente, string columNameFlCompartePicking, string columNameFlComparteEntrega, string columNameAgenda)
        {
            string separador1 = "$";
            string separador2 = "#";
            string sql = $@"
                CASE FL_EMPRESA WHEN 'S' THEN CAST({aliasProducto}.CD_EMPRESA as VARCHAR(10)) ELSE '' END || '{separador1}' ||
                CASE FL_PRODUTO WHEN 'S' THEN {aliasProducto}.CD_PRODUTO ELSE'' END || '{separador1}' ||
                CASE FL_PRODUCTO_FAMILIA WHEN 'S' THEN CAST({aliasProducto}.CD_FAMILIA_PRODUTO as VARCHAR(10)) ELSE '' END || '{separador1}' ||
                CASE FL_PRODUCTO_RAMO WHEN 'S' THEN  CAST({aliasProducto}.CD_RAMO_PRODUTO as VARCHAR(10)) ELSE '' END || '{separador1}' ||
                CASE FL_PRODUCTO_ROTATIVIDADE WHEN 'S' THEN CAST({aliasProducto}.CD_ROTATIVIDADE as VARCHAR(10)) ELSE '' END || '{separador1}' ||
                CASE FL_PRODUCTO_CLASSE WHEN 'S' THEN {aliasProducto}.CD_CLASSE ELSE'' END || '{separador1}' ||
                CASE FL_PRODUCTO_SUB_CLASSE WHEN 'S' THEN CAST({aliasClase}.CD_SUB_CLASSE as VARCHAR(10)) ELSE '' END || '{separador1}' ||
                CASE FL_PRODUCTO_ANEXO1 WHEN 'S' THEN {aliasProducto}.DS_ANEXO1 ELSE '' END || '{separador1}' ||
                CASE FL_PRODUCTO_ANEXO2 WHEN 'S' THEN {aliasProducto}.DS_ANEXO2 ELSE '' END || '{separador1}' ||
                CASE FL_PRODUCTO_ANEXO3 WHEN 'S' THEN {aliasProducto}.DS_ANEXO3 ELSE '' END || '{separador1}' ||
                CASE FL_PRODUCTO_ANEXO4 WHEN 'S' THEN {aliasProducto}.DS_ANEXO4 ELSE '' END || '{separador1}' ||
                CASE FL_PRODUCTO_ANEXO5 WHEN 'S' THEN {aliasProducto}.DS_ANEXO5 ELSE '' END || '{separador1}' ||
                CASE FL_CLIENTE WHEN 'S' THEN {aliasCliente}.CD_CLIENTE ELSE '' END || '{separador1}' ||            
            ";

            if (!string.IsNullOrEmpty(columNameFlCompartePicking))
                sql += @$"CASE FL_COMPARTE_PICKING WHEN 'S' THEN {columNameFlCompartePicking} ELSE '' END || '{separador1}' ||";
            else
                sql += @$" ''  || '{separador1}' ||";


            if (!string.IsNullOrEmpty(columNameFlComparteEntrega))
                sql += @$"CASE FL_COMPARTE_ENTREGA WHEN 'S' THEN {columNameFlComparteEntrega} ELSE '' END";
            else
                sql += @$" '' ";

            if (!string.IsNullOrEmpty(columNameAgenda))
            {
                sql += @$" || ''{separador2}'' || {columNameAgenda}";
            }

            return sql;
        }

        public virtual List<DetallePreparacion> GetDetallesPreparacion(int preparacion, int empresa, string cliente, string ubicacion)
        {
            string sql = $@"
                SELECT 
                    PICK.NU_PREPARACION NumeroPreparacion, 
                    PICK.CD_ENDERECO Ubicacion,
                    PICK.NU_PEDIDO Pedido,
                    PICK.CD_CLIENTE Cliente,
                    cl.CD_AGENTE CodigoAgente,
                    cl.TP_AGENTE TipoAgente,
                    PICK.CD_PRODUTO Producto,
                    PICK.CD_FAIXA Faixa,
                    PICK.NU_IDENTIFICADOR Lote,
                    PICK.QT_PRODUTO Cantidad
                FROM T_DET_PICKING PICK 
                INNER JOIN T_PEDIDO_SAIDA PED ON PED.NU_PEDIDO = PICK.NU_PEDIDO 
                    AND PED.CD_CLIENTE = PICK.CD_CLIENTE 
                    AND PED.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_PRODUTO PROD ON PROD.CD_PRODUTO = PICK.CD_PRODUTO 
                    AND PROD.CD_EMPRESA = PICK.CD_EMPRESA
                INNER JOIN T_CLASSE CLA ON   CLA.CD_CLASSE = PROD.CD_CLASSE
                INNER JOIN T_CLIENTE cl ON PICK.CD_CLIENTE= cl.CD_CLIENTE
                WHERE PICK.NU_PREPARACION = :preparacion 
                    AND PICK.ND_ESTADO = :estado 
                    AND PICK.CD_ENDERECO = :ubicacion 
                    AND PICK.CD_EMPRESA = :empresa 
                    AND PED.CD_CLIENTE = :cliente";

            var query = _dapper.Query<DetallePreparacion>(_context.Database.GetDbConnection(), sql, new
            {
                preparacion = preparacion,
                estado = Mappers.Constants.EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO,
                ubicacion = ubicacion,
                empresa = empresa,
                cliente = cliente,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.ToList();
        }

        #endregion
    }
}
