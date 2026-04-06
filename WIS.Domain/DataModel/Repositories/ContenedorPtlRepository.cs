using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ContenedorPtlRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly ContenedorMapper _mapper;
        protected readonly IDapper _dapper;

        public ContenedorPtlRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            _context = context;
            _cdAplicacion = application;
            _userId = userId;
            _mapper = new ContenedorMapper();
            _dapper = dapper;
        }

        #region Any

        public virtual bool AnyDiferenteComparteAgrupacionDetallePicking(int preparacion, int contenedor, int? tipoAgrupacion, string comparteAgrupacion)
        {
            string sql = @" 
                SELECT 1 
                FROM (
                    SELECT  
                        DCD.NU_PEDIDO,
                        DCD.CD_CLIENTE, 
                        DCD.CD_EMPRESA, 
                        DCD.CD_PRODUTO 
                    FROM T_DET_CROSS_DOCK dcd
                    INNER JOIN t_etiqueta_lote el ON dcd.nu_agenda = el.nu_agenda 
                        AND dcd.cd_cliente = el.cd_cliente
                    WHERE EL.TP_ETIQUETA = 'W' 
                        AND DCD.NU_PREPARACION = :P_NU_PREPARACION 
                        AND EL.NU_EXTERNO_ETIQUETA = TO_CHAR(:P_NU_CONTENEDOR) 
                        AND el.cd_cliente IS NOT NULL
                    GROUP BY 
                        DCD.NU_PEDIDO,
                        DCD.CD_CLIENTE, 
                        DCD.CD_EMPRESA, 
                        DCD.CD_PRODUTO

                    UNION ALL

                    SELECT 
                        dp.NU_PEDIDO,
                        dp.CD_CLIENTE, 
                        dp.CD_EMPRESA, 
                        dp.CD_PRODUTO 
                    FROM t_det_picking dp  
                    WHERE dp.nd_estado = 'ESTAD_PREPARADO' 
                        AND dp.nu_preparacion = :P_NU_PREPARACION 
                        AND dp.nu_contenedor = :P_NU_CONTENEDOR
                    GROUP BY 
                        dp.NU_PEDIDO,
                        dp.CD_CLIENTE, 
                        dp.CD_EMPRESA, 
                        dp.CD_PRODUTO
                ) dp
                INNER JOIN t_produto pr ON pr.cd_produto = dp.cd_produto 
                    AND pr.cd_empresa = dp.cd_empresa
                INNER JOIN t_classe cl ON cl.cd_classe = pr.cd_classe
                INNER JOIN t_pedido_saida ps ON dp.nu_pedido = ps.nu_pedido 
                    AND ps.cd_cliente = dp.cd_cliente and ps.cd_empresa = dp.cd_empresa
                INNER JOIN T_TIPO_AGRUPACION_ENDERECO tae ON tae.TP_AGRUPACION_UBIC = :P_TP_AGRUPACION_UBIC
                    AND (
                        CASE WHEN FL_EMPRESA = 'S' THEN PR.CD_EMPRESA ELSE '' END || '$' ||
                        CASE WHEN FL_PRODUTO = 'S' THEN PR.CD_PRODUTO ELSE '' END || '$' ||
                        CASE WHEN FL_PRODUCTO_FAMILIA = 'S' THEN PR.CD_FAMILIA_PRODUTO ELSE '' END || '$' ||
                        CASE WHEN FL_PRODUCTO_RAMO = 'S' THEN PR.CD_RAMO_PRODUTO,'' END || '$' ||
                        CASE WHEN FL_PRODUCTO_ROTATIVIDADE = 'S' THEN PR.CD_ROTATIVIDADE ELSE '' END || '$' ||
                        CASE WHEN FL_PRODUCTO_CLASSE = 'S' THEN PR.CD_CLASSE ELSE '' END || '$' ||
                        CASE WHEN FL_PRODUCTO_SUB_CLASSE = 'S' THEN CL.CD_SUB_CLASSE ELSE '' END || '$' ||
                        CASE WHEN FL_PRODUCTO_ANEXO1 = 'S' THEN PR.DS_ANEXO1 ELSE '' END || '$' ||
                        CASE WHEN FL_PRODUCTO_ANEXO2 = 'S' THEN PR.DS_ANEXO2 ELSE '' END || '$' ||
                        CASE WHEN FL_PRODUCTO_ANEXO3 = 'S' THEN PR.DS_ANEXO3 ELSE '' END || '$' ||
                        CASE WHEN FL_PRODUCTO_ANEXO4 = 'S' THEN PR.DS_ANEXO4 ELSE '' END || '$' ||
                        CASE WHEN FL_PRODUCTO_ANEXO5 = 'S' THEN PR.DS_ANEXO4 ELSE '' END || '$' ||
                        CASE WHEN FL_CLIENTE = 'S' THEN ps.CD_CLIENTE ELSE '' END || '$' ||
                        CASE WHEN FL_COMPARTE_PICKING = 'S' THEN ps.VL_COMPARTE_CONTENEDOR_PICKING ELSE '' END || '$' ||
                        CASE WHEN FL_COMPARTE_ENTREGA = 'S' THEN ps.VL_COMPARTE_CONTENEDOR_ENTREGA ELSE '' END 
                    ) <> :P_VL_COMPARTE_AGRUPACION";

            var query = _dapper.Query<int>(_context.Database.GetDbConnection(), sql, new
            {
                P_TP_AGRUPACION_UBIC = tipoAgrupacion,
                P_NU_PREPARACION = preparacion,
                P_NU_CONTENEDOR = contenedor,
                P_VL_COMPARTE_AGRUPACION = comparteAgrupacion,
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return query.Count() > 0;
        }

        public virtual bool ExisteContenedorClientePredefinido(int numeroContenedor, string tipoContenedor, string codigoCliente, int codigoEmpresa)
        {
            return _context.T_CONTENEDORES_PREDEFINIDOS
                .Any(x => x.NU_CONTENEDOR == numeroContenedor
                    && x.TP_CONTENEDOR == tipoContenedor
                    && x.CD_CLIENTE == codigoCliente
                    && x.CD_EMPRESA == codigoEmpresa);
        }

        #endregion

        #region Get

        public virtual (bool, long?, short?) ExisteContenedorEnOtraPreparacion(int numeroContenedor, long numeroPreparacion, List<short> estadosContenedorEnUso)
        {
            if (_context.T_CONTENEDOR.Any(x => x.NU_CONTENEDOR == numeroContenedor && x.NU_PREPARACION != numeroPreparacion && estadosContenedorEnUso.Contains((x.CD_SITUACAO ?? 0))))
            {
                var contenedor = _context.T_CONTENEDOR
                    .FirstOrDefault(x => x.NU_CONTENEDOR == numeroContenedor
                        && x.NU_PREPARACION != numeroPreparacion
                        && estadosContenedorEnUso.Contains((x.CD_SITUACAO ?? 0)));

                return (true, contenedor.NU_PREPARACION, contenedor.CD_SITUACAO);
            }
            else
                return (false, null, null);
        }

        public virtual Contenedor GetContenedorPorEstado(int numeroPreparacion, int numeroContenedor, EstadoContenedor estado)
        {
            var situacionContenedor = _mapper.MapEstadoContenedor(estado);
            T_CONTENEDOR entity = _context.T_CONTENEDOR.AsNoTracking()
             .FirstOrDefault(con => con.NU_CONTENEDOR == numeroContenedor
                && con.NU_PREPARACION == numeroPreparacion
                && con.CD_SITUACAO == situacionContenedor);

            return entity == null ? null : _mapper.MapToObject(entity);
        }

        #endregion

        #region Add

        public virtual void AddContenedor(Contenedor contenedor)
        {
            contenedor.FechaAgregado = DateTime.Now;
            T_CONTENEDOR nuevoContenedor = _mapper.MapToEntity(contenedor);
            this._context.T_CONTENEDOR.Add(nuevoContenedor);
        }

        #endregion

        #region Update

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
