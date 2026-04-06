using Microsoft.EntityFrameworkCore;
using NLog;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Factories;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AutomatismoRepository
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly WISDB _context;
        protected readonly IDapper _dapper;
        protected readonly int _userId;
        protected readonly string _cdAplicacion;
        protected readonly AutomatismoMapper _mapper;

        public AutomatismoRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            _context = context;
            _dapper = dapper;
            _userId = userId;
            _cdAplicacion = cdAplicacion;
            _mapper = new AutomatismoMapper(new AutomatismoFactory());
        }

        #region Any

        public virtual bool AnyAutomatismo(Producto producto)
        {
            return this._context.V_PRODUCTOS_ASOCIADOS_AUTOMATISMO
                .AsNoTracking()
                .Any(paa => paa.CD_EMPRESA == producto.CodigoEmpresa
                    && paa.CD_PRODUTO == producto.Codigo);
        }

        #endregion

        #region Get
        public virtual IQueryable<T_AUTOMATISMO> GetQueryWithAllIncludes()
        {
            return this._context.T_AUTOMATISMO
                .Include("T_AUTOMATISMO_POSICION")
                .Include("T_AUTOMATISMO_INTERFAZ")
                .Include("T_AUTOMATISMO_PUESTO")
                //.Include("T_AUTOMATISMO_POSICION.T_ENDERECO_ESTOQUE")
                .Include("T_AUTOMATISMO_INTERFAZ.T_INTEGRACION_SERVICIO")
                .Include("T_AUTOMATISMO_CARACTERISTICA")
                .AsNoTracking();
        }

        public virtual List<IAutomatismo> GetAllByTipo(string tipo)
        {
            return this._mapper.Map(this.GetQueryWithAllIncludes().Where(w => w.ND_TP_AUTOMATISMO == tipo).ToList());
        }

        public virtual List<IAutomatismo> GetAutomatismos(string endereco, string tipoEndereco)
        {
            return this._mapper.Map(
                this._context.T_AUTOMATISMO
                .Include("T_AUTOMATISMO_POSICION")
                .Where(w => w.T_AUTOMATISMO_POSICION.Any(a => a.CD_ENDERECO == endereco && a.ND_TIPO_ENDERECO == tipoEndereco))
                .ToList());
        }

        public virtual List<IAutomatismo> GetAutomatismos()
        {
            return this._mapper.Map(this.GetQueryWithAllIncludes().ToList());
        }

        public virtual IAutomatismo GetAutomatismoByZona(string zona)
        {
            return this._mapper.Map(this.GetQueryWithAllIncludes().FirstOrDefault(w => w.CD_ZONA_UBICACION == zona));
        }

        public virtual IAutomatismo GetAutomatismoByCodigo(string codigo)
        {
            return this._mapper.Map(this.GetQueryWithAllIncludes().FirstOrDefault(w => w.CD_AUTOMATISMO == codigo));
        }

        public virtual IAutomatismo GetAutomatismoById(int id)
        {
            return this._mapper.Map(this.GetQueryWithAllIncludes().FirstOrDefault(f => f.NU_AUTOMATISMO == id));
        }

        public virtual IAutomatismo GetAutomatismoByPuesto(string idPuesto)
        {
            return this._mapper.Map(this.GetQueryWithAllIncludes().FirstOrDefault(w => w.T_AUTOMATISMO_PUESTO != null && w.T_AUTOMATISMO_PUESTO.Any(a => a.ID_PUESTO == idPuesto)));
        }

        public virtual int GetNextNumeroAutomatismo()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_AUTOMATISMO);
        }

        public virtual bool IsUbicacionAutomatismo(string ubicacion)
		{
			return _context.T_ENDERECO_ESTOQUE
				.Include("T_ZONA_UBICACION")
				.AsNoTracking()
				.Any(ee => ee.CD_ENDERECO == ubicacion
					&& ee.T_ZONA_UBICACION.TP_ZONA_UBICACION == TipoUbicacionZonaDb.Automatismo);
		}


        #endregion

        #region Add

        public virtual void Add(IAutomatismo automatismo)
        {
            _context.T_AUTOMATISMO.Add(_mapper.Map(automatismo));
        }

        #endregion

        #region Update

        public virtual void Update(IAutomatismo automatismo)
        {
            var entity = _mapper.Map(automatismo);
            var attachedEntity = _context.T_AUTOMATISMO.Local.FirstOrDefault(x => x.NU_AUTOMATISMO == entity.NU_AUTOMATISMO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_AUTOMATISMO.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveAutomatismo(IAutomatismo automatismo)
        {
            var entity = this._mapper.Map(automatismo);
            var attachedEntity = _context.T_AUTOMATISMO.Local.FirstOrDefault(w => w.NU_AUTOMATISMO == entity.NU_AUTOMATISMO);

            if (attachedEntity != null)
            {
                this._context.T_AUTOMATISMO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_AUTOMATISMO.Attach(entity);
                this._context.T_AUTOMATISMO.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<ProductoZonaAutomatismo> GetZonasAutomatismoByProductos(IEnumerable<Producto> productos)
        {
            IEnumerable<ProductoZonaAutomatismo> resultado = new List<ProductoZonaAutomatismo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRODUTO_TEMP (CD_PRODUTO, CD_EMPRESA) VALUES (:Codigo, :CodigoEmpresa)";
                    _dapper.Execute(connection, sql, productos, transaction: tran);

                    sql = @"
                        SELECT 
                            T.CD_PRODUTO AS Codigo, 
                            PAA.CD_ZONA_UBICACION AS Zona,
                            PAA.FL_CONF_CD_BARRAS_AUT AS ConfirmarCodigoBarras,
                            PAA.CD_UNIDAD_CAJA_AUT AS UnidadCaja,
                            PAA.QT_UNIDAD_CAJA_AUT AS CantidadUnidadCaja

                        FROM V_PRODUCTOS_ASOCIADOS_AUTOMATISMO PAA 
                        INNER JOIN T_PRODUTO_TEMP T ON PAA.CD_PRODUTO = T.CD_PRODUTO 
                            AND PAA.CD_EMPRESA = T.CD_EMPRESA
                        GROUP BY 
                            T.CD_PRODUTO, 
                            PAA.CD_ZONA_UBICACION,
                            PAA.FL_CONF_CD_BARRAS_AUT,
                            PAA.CD_UNIDAD_CAJA_AUT,
                            PAA.QT_UNIDAD_CAJA_AUT";

                    resultado = _dapper.Query<ProductoZonaAutomatismo>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Producto> GetProductosAutomatismo(IEnumerable<Producto> productos)
        {
            IEnumerable<Producto> resultado = new List<Producto>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRODUTO_TEMP (CD_PRODUTO, CD_EMPRESA) VALUES (:Codigo, :CodigoEmpresa)";
                    _dapper.Execute(connection, sql, productos, transaction: tran);

                    sql = @"
                        SELECT 
                            PAA.CD_PRODUTO AS Codigo, 
                            PAA.CD_EMPRESA AS CodigoEmpresa
                        FROM V_PRODUCTOS_ASOCIADOS_AUTOMATISMO PAA 
                        INNER JOIN T_PRODUTO_TEMP T ON PAA.CD_PRODUTO = T.CD_PRODUTO 
                            AND PAA.CD_EMPRESA = T.CD_EMPRESA
                        GROUP BY 
                            PAA.CD_PRODUTO, 
                            PAA.CD_EMPRESA";

                    resultado = _dapper.Query<Producto>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<CodigoBarrasZonaAutomatismo> GetZonasAutomatismoByCodigosBarras(IEnumerable<CodigoBarras> codigoBarras)
		{
			IEnumerable<CodigoBarrasZonaAutomatismo> resultado = new List<CodigoBarrasZonaAutomatismo>();

			using (var connection = this._dapper.GetDbConnection())
			{
				connection.Open();

				using (var tran = connection.BeginTransaction())
				{
					string sql = @"INSERT INTO T_CODIGO_BARRAS_TEMP (CD_BARRAS, CD_EMPRESA, CD_PRODUTO) VALUES (:Codigo, :Empresa, :Producto)";
					_dapper.Execute(connection, sql, codigoBarras, transaction: tran);

					sql = @"
                        SELECT 
                            T.CD_BARRAS AS Codigo, 
                            PAA.CD_ZONA_UBICACION AS Zona
                        FROM V_PRODUCTOS_ASOCIADOS_AUTOMATISMO PAA
                        INNER JOIN T_CODIGO_BARRAS_TEMP T ON PAA.CD_PRODUTO = T.CD_PRODUTO 
                            AND PAA.CD_EMPRESA = T.CD_EMPRESA
                        GROUP BY 
                            T.CD_BARRAS, 
                            PAA.CD_ZONA_UBICACION";

					resultado = _dapper.Query<CodigoBarrasZonaAutomatismo>(connection, sql, transaction: tran);

					tran.Rollback();
				}
			}

			return resultado;
		}

        public virtual IEnumerable<CodigoBarras> GetCodigosBarrasAutomatismo(IEnumerable<CodigoBarras> codigoBarras)
		{
			IEnumerable<CodigoBarras> resultado = new List<CodigoBarras>();

			using (var connection = this._dapper.GetDbConnection())
			{
				connection.Open();

				using (var tran = connection.BeginTransaction())
				{
					string sql = @"INSERT INTO T_CODIGO_BARRAS_TEMP (CD_BARRAS, CD_EMPRESA, CD_PRODUTO) VALUES (:Codigo, :Empresa, :Producto)";
					_dapper.Execute(connection, sql, codigoBarras, transaction: tran);

					sql = @"
                        SELECT 
                            T.CD_BARRAS AS Codigo, 
                            T.CD_EMPRESA AS Empresa
                        FROM V_PRODUCTOS_ASOCIADOS_AUTOMATISMO PAA
                        INNER JOIN T_CODIGO_BARRAS_TEMP T ON PAA.CD_PRODUTO = T.CD_PRODUTO 
                            AND PAA.CD_EMPRESA = T.CD_EMPRESA
                        GROUP BY 
                            T.CD_BARRAS, 
                            T.CD_EMPRESA";

					resultado = _dapper.Query<CodigoBarras>(connection, sql, transaction: tran);

					tran.Rollback();
				}
			}

			return resultado;
		}

        public virtual IEnumerable<UbicacionPickingZonaAutomatismo> GetZonasAutomatismoByUbicacionesPicking(IEnumerable<UbicacionPickingProducto> ubicacionesPicking)
        {
            IEnumerable<UbicacionPickingZonaAutomatismo> resultado = new List<UbicacionPickingZonaAutomatismo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_PRODUTO_TEMP (CD_PRODUTO, CD_EMPRESA, CD_ENDERECO_SEPARACAO) VALUES (:CodigoProducto, :Empresa, :UbicacionSeparacion)";
                    _dapper.Execute(connection, sql, ubicacionesPicking, transaction: tran);

                    sql = @"
                        SELECT 
                            PP.CD_PRODUTO AS Producto,
                            PP.CD_ENDERECO_SEPARACAO AS Ubicacion,
                            PAA.CD_ZONA_UBICACION AS Zona
                        FROM T_PICKING_PRODUTO PP
                        INNER JOIN T_ENDERECO_ESTOQUE EE ON EE.CD_ENDERECO = PP.CD_ENDERECO_SEPARACAO 
                        INNER JOIN V_PRODUCTOS_ASOCIADOS_AUTOMATISMO PAA ON PP.CD_PRODUTO = PAA.CD_PRODUTO 
                            AND PP.CD_EMPRESA = PAA.CD_EMPRESA
                            AND EE.CD_ZONA_UBICACION = PAA.CD_ZONA_UBICACION
                        INNER JOIN T_PICKING_PRODUTO_TEMP T ON PP.CD_PRODUTO = T.CD_PRODUTO 
                            AND PP.CD_EMPRESA = T.CD_EMPRESA
                            AND PP.CD_ENDERECO_SEPARACAO = T.CD_ENDERECO_SEPARACAO
                        GROUP BY 
                            PP.CD_PRODUTO, 
                            PP.CD_ENDERECO_SEPARACAO,
                            PAA.CD_ZONA_UBICACION";

                    resultado = _dapper.Query<UbicacionPickingZonaAutomatismo>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<UbicacionPickingProducto> GetUbicacionesPickingAutomatismo(IEnumerable<UbicacionPickingProducto> ubicacionesPicking)
        {
            IEnumerable<UbicacionPickingProducto> resultado = new List<UbicacionPickingProducto>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_PRODUTO_TEMP (CD_PRODUTO, CD_EMPRESA, CD_ENDERECO_SEPARACAO) VALUES (:CodigoProducto, :Empresa, :UbicacionSeparacion)";
                    _dapper.Execute(connection, sql, ubicacionesPicking, transaction: tran);

                    sql = @"
                        SELECT 
                            T.CD_PRODUTO AS CodigoProducto, 
                            T.CD_EMPRESA AS Empresa,
                            T.CD_ENDERECO_SEPARACAO AS UbicacionSeparacion
                        FROM T_PICKING_PRODUTO_TEMP T
                        INNER JOIN T_ENDERECO_ESTOQUE EE ON EE.CD_ENDERECO = T.CD_ENDERECO_SEPARACAO 
                        INNER JOIN T_ZONA_UBICACION ZU ON EE.CD_ZONA_UBICACION = ZU.CD_ZONA_UBICACION 
                            AND ZU.TP_ZONA_UBICACION = :TP_ZONA_UBICACION
                        GROUP BY 
                            T.CD_PRODUTO, 
                            T.CD_EMPRESA,
                            T.CD_ENDERECO_SEPARACAO";

                    resultado = _dapper.Query<UbicacionPickingProducto>(connection, sql, new
                    {
                        TP_ZONA_UBICACION = TipoUbicacionZonaDb.Automatismo
                    }, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<ProductoAutomatismoRequest> GetProductosAutomatismoByUbicacionesPicking(IEnumerable<UbicacionPickingAutomatismoRequest> ubicacionesPicking)
        {
            IEnumerable<ProductoAutomatismoRequest> resultado = new List<ProductoAutomatismoRequest>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_PRODUTO_TEMP (CD_PRODUTO, CD_EMPRESA, CD_ENDERECO_SEPARACAO, TP_OPERACION) VALUES (:Producto, :Empresa, :Ubicacion, :TipoOperacion)";
                    _dapper.Execute(connection, sql, ubicacionesPicking, transaction: tran);

                    sql = @"
                         SELECT 
                            P.CD_PRODUTO AS Codigo,
                            P.DS_PRODUTO AS Descripcion,
                            P.PS_BRUTO AS PesoBruto,
                            P.VL_LARGURA AS Ancho,
                            P.VL_ALTURA AS Altura,
                            P.VL_PROFUNDIDADE AS Profundidad,
                            P.ID_MANEJO_IDENTIFICADOR AS ManejoIdentificador, 
                            P.TP_MANEJO_FECHA AS TipoManejoFecha,
                            EE.CD_ZONA_UBICACION AS Zona,
                            T.TP_OPERACION AS TipoOperacion,
                            PP.FL_CONF_CD_BARRAS_AUT AS ConfirmarCodigoBarras,
                            PP.CD_UNIDAD_CAJA_AUT AS UnidadCaja,
                            PP.QT_UNIDAD_CAJA_AUT AS CantidadUnidadCaja,
                            P.QT_UND_BULTO AS UnidadBulto
                        FROM T_PICKING_PRODUTO_TEMP T
                        INNER JOIN T_PICKING_PRODUTO PP
                            ON PP.CD_PRODUTO = T.CD_PRODUTO
                            AND PP.CD_EMPRESA = T.CD_EMPRESA
                            AND PP.CD_ENDERECO_SEPARACAO = T.CD_ENDERECO_SEPARACAO
                        INNER JOIN T_PRODUTO P ON T.CD_PRODUTO = P.CD_PRODUTO 
                            AND T.CD_EMPRESA = P.CD_EMPRESA
                        INNER JOIN T_ENDERECO_ESTOQUE EE ON EE.CD_ENDERECO = T.CD_ENDERECO_SEPARACAO 
                        INNER JOIN T_ZONA_UBICACION ZU ON EE.CD_ZONA_UBICACION = ZU.CD_ZONA_UBICACION
                            AND ZU.TP_ZONA_UBICACION = :TP_ZONA_UBICACION   
                        INNER JOIN T_TIPO_AREA TA ON EE.CD_AREA_ARMAZ = TA.CD_AREA_ARMAZ
                            AND TA.ID_AREA_PICKING = 'S'
                            AND TA.ID_DISP_ESTOQUE = 'S'
                        GROUP BY 
                            P.CD_PRODUTO, 
                            P.DS_PRODUTO,
                            P.PS_BRUTO,
                            P.VL_LARGURA,
                            P.VL_ALTURA,
                            P.VL_PROFUNDIDADE,
                            P.ID_MANEJO_IDENTIFICADOR,
                            P.TP_MANEJO_FECHA,
                            EE.CD_ZONA_UBICACION,
                            T.TP_OPERACION,
                            PP.FL_CONF_CD_BARRAS_AUT,
                            PP.CD_UNIDAD_CAJA_AUT,
                            PP.QT_UNIDAD_CAJA_AUT,
                            P.QT_UND_BULTO";

                    resultado = _dapper.Query<ProductoAutomatismoRequest>(connection, sql, new
                    {
                        TP_ZONA_UBICACION = TipoUbicacionZonaDb.Automatismo,
                    }, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<CodigoBarraAutomatismoRequest> GetCodigosBarrasAutomatismoByUbicacionesPicking(IEnumerable<UbicacionPickingAutomatismoRequest> ubicacionesPicking)
        {
            IEnumerable<CodigoBarraAutomatismoRequest> resultado = new List<CodigoBarraAutomatismoRequest>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_PRODUTO_TEMP (CD_PRODUTO, CD_EMPRESA, CD_ENDERECO_SEPARACAO, TP_OPERACION) VALUES (:Producto, :Empresa, :Ubicacion, :TipoOperacion)";
                    _dapper.Execute(connection, sql, ubicacionesPicking, transaction: tran);

                    sql = @"
                        SELECT 
                            CB.CD_BARRAS AS Codigo,
                            CB.CD_PRODUTO AS Producto,
                            EE.CD_ZONA_UBICACION AS Zona,
                            T.TP_OPERACION AS TipoOperacion
                        FROM T_PICKING_PRODUTO_TEMP T
                        INNER JOIN T_CODIGO_BARRAS CB ON T.CD_PRODUTO = CB.CD_PRODUTO 
                            AND T.CD_EMPRESA = CB.CD_EMPRESA
                        INNER JOIN T_ENDERECO_ESTOQUE EE ON EE.CD_ENDERECO = T.CD_ENDERECO_SEPARACAO 
                        INNER JOIN T_ZONA_UBICACION ZU ON EE.CD_ZONA_UBICACION = ZU.CD_ZONA_UBICACION
                            AND ZU.TP_ZONA_UBICACION = :TP_ZONA_UBICACION   
                        INNER JOIN T_TIPO_AREA TA ON EE.CD_AREA_ARMAZ = TA.CD_AREA_ARMAZ
                            AND TA.ID_AREA_PICKING = 'S'
                            AND TA.ID_DISP_ESTOQUE = 'S'
                        GROUP BY 
                            CB.CD_BARRAS,
                            CB.CD_PRODUTO, 
                            EE.CD_ZONA_UBICACION,
                            T.TP_OPERACION";

                    resultado = _dapper.Query<CodigoBarraAutomatismoRequest>(connection, sql, new
                    {
                        TP_ZONA_UBICACION = TipoUbicacionZonaDb.Automatismo,
                    }, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<UbicacionPickingProducto> GetEmpresasAutomatismoByUbicacionesPicking(IEnumerable<AjusteStockRequest> ajustes)
        {
            IEnumerable<UbicacionPickingProducto> resultado = new List<UbicacionPickingProducto>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_PRODUTO_TEMP (CD_PRODUTO, CD_ENDERECO_SEPARACAO) VALUES (:Producto, :Ubicacion)";
                    _dapper.Execute(connection, sql, ajustes, transaction: tran);

                    sql = @"
                        SELECT 
                            MIN(PP.CD_EMPRESA) AS Empresa,
                            PP.CD_PRODUTO AS CodigoProducto,
                            PP.CD_ENDERECO_SEPARACAO AS UbicacionSeparacion
                        FROM T_PICKING_PRODUTO PP
                        INNER JOIN T_PICKING_PRODUTO_TEMP T ON T.CD_PRODUTO = PP.CD_PRODUTO 
                            AND T.CD_ENDERECO_SEPARACAO = PP.CD_ENDERECO_SEPARACAO
                        INNER JOIN T_ENDERECO_ESTOQUE EE ON EE.CD_ENDERECO = PP.CD_ENDERECO_SEPARACAO 
                        INNER JOIN T_ZONA_UBICACION ZU ON EE.CD_ZONA_UBICACION = ZU.CD_ZONA_UBICACION
                            AND ZU.TP_ZONA_UBICACION = :TP_ZONA_UBICACION   
                        INNER JOIN T_TIPO_AREA TA ON EE.CD_AREA_ARMAZ = TA.CD_AREA_ARMAZ
                            AND TA.ID_AREA_PICKING = 'S'
                            AND TA.ID_DISP_ESTOQUE = 'S'
                        GROUP BY 
                            PP.CD_PRODUTO, 
                            PP.CD_ENDERECO_SEPARACAO";

                    resultado = _dapper.Query<UbicacionPickingProducto>(connection, sql, new
                    {
                        TP_ZONA_UBICACION = TipoUbicacionZonaDb.Automatismo,
                    }, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<string> GetProductosAutomatismo(string zona, int empresa, List<EntradaStockLineaAutomatismoRequest> detalles)
        {
            IEnumerable<string> resultado = new List<string>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRODUTO_TEMP (CD_PRODUTO, CD_EMPRESA) VALUES (:Producto, :Empresa)";
                    _dapper.Execute(connection, sql, detalles.Select(d => new
                    {
                        Producto = d.Producto,
                        Empresa = empresa,
                    }), transaction: tran);

                    sql = @"
                        SELECT 
                            PP.CD_PRODUTO
                        FROM T_PICKING_PRODUTO PP
                        INNER JOIN T_PRODUTO_TEMP T ON T.CD_PRODUTO = PP.CD_PRODUTO 
                            AND T.CD_EMPRESA = PP.CD_EMPRESA
                        INNER JOIN T_ENDERECO_ESTOQUE EE ON EE.CD_ENDERECO = PP.CD_ENDERECO_SEPARACAO 
                        INNER JOIN T_ZONA_UBICACION ZU ON EE.CD_ZONA_UBICACION = ZU.CD_ZONA_UBICACION
                            AND ZU.TP_ZONA_UBICACION = :TP_ZONA_UBICACION
                            AND ZU.CD_ZONA_UBICACION = :CD_ZONA_UBICACION
                        INNER JOIN T_TIPO_AREA TA ON EE.CD_AREA_ARMAZ = TA.CD_AREA_ARMAZ
                            AND TA.ID_AREA_PICKING = 'S'
                            AND TA.ID_DISP_ESTOQUE = 'S'
                        GROUP BY 
                            PP.CD_PRODUTO";

                    resultado = _dapper.Query<string>(connection, sql, new
                    {
                        CD_ZONA_UBICACION = zona,
                        TP_ZONA_UBICACION = TipoUbicacionZonaDb.Automatismo,
                    }, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        #endregion
    }
}
