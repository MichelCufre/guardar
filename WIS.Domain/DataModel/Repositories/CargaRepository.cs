using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class CargaRepository
    {
        protected WISDB _context;
        protected string _application;
        protected int _userId;
        protected CargaMapper _mapper;
        protected readonly IDapper _dapper;

		public CargaRepository(WISDB context, string application, int userid, IDapper dapper)
		{
			this._context = context;
			this._application = application;
			this._userId = userid;
			this._mapper = new CargaMapper();
			_dapper = dapper;
		}

		#region Any

		#endregion

		#region Get
		public virtual Carga GetCarga(long id)
		{
			return this._mapper.MapToObject(this._context.T_CARGA.FirstOrDefault(d => d.NU_CARGA == id));
		}
		public virtual Carga GetCarga(int preparacion)
		{
			return this._mapper.MapToObject(this._context.T_CARGA.FirstOrDefault(d => d.NU_PREPARACION == preparacion));
		}
		public virtual long? GetCargaPicking(int preparacion)
		{
			return _context.T_DET_PICKING.FirstOrDefault(d => d.NU_PREPARACION == preparacion)?.NU_CARGA;
		}
        #endregion

        #region Add
        public virtual void AddCarga(Carga carga)
		{
			carga.Id = this.GetSiguienteNumeroCarga();

			T_CARGA entity = this._mapper.MapToEntity(carga);
			this._context.T_CARGA.Add(entity);
		}

		public virtual void AddCarga(Carga carga, bool generateNewId = true)
		{
			if (generateNewId)
				carga.Id = this.GetSiguienteNumeroCarga();

			carga.Descripcion = carga.Descripcion.Length > 100 ? carga.Descripcion.Substring(0, 100) : carga.Descripcion;

			T_CARGA entity = this._mapper.MapToEntity(carga);
			this._context.T_CARGA.Add(entity);
		}
		#endregion

		#region Update

		#endregion

		#region Remove

		#endregion

		#region Dapper
		public virtual long GetSiguienteNumeroCarga()
		{
			return this._context.GetNextSequenceValueLong(_dapper, "S_CARGA");
		}

        public virtual List<long> GetNewNumeroCargas(int count, DbConnection connection, DbTransaction tran)
        {
            return _dapper.GetNextSequenceValues<long>(connection, "S_CARGA", count, tran).ToList();
        }

        public virtual IEnumerable<Carga> GetCargas(IEnumerable<Carga> cargas)
		{
			IEnumerable<Carga> resultado = new List<Carga>();

			using (var connection = this._dapper.GetDbConnection())
			{
				connection.Open();

				using (var tran = connection.BeginTransaction())
				{
					string sql = @"INSERT INTO T_DET_PICKING_TEMP (NU_CARGA) VALUES (:Id)";
					_dapper.Execute(connection, sql, cargas, transaction: tran);

					sql = @" SELECT 
                                C.NU_CARGA AS Id,
                                C.DS_CARGA AS Descripcion,
                                C.CD_ROTA AS Ruta,
                                C.DT_ADDROW AS FechaAlta,
                                C.NU_PREPARACION AS Preparacion
                            FROM T_CARGA C
                            INNER JOIN T_DET_PICKING_TEMP T ON c.NU_CARGA = T.NU_CARGA ";

					resultado = _dapper.Query<Carga>(connection, sql, transaction: tran);

					tran.Rollback();
				}
			}

			return resultado;
		}

		public virtual IEnumerable<Carga> GetCargasHabilitadas(IEnumerable<Carga> cargas, bool validacionPicking)
        {
            IEnumerable<Carga> resultado = new List<Carga>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CARGA_TEMP (NU_CARGA) VALUES (:Id)";
                    _dapper.Execute(connection, sql, cargas, transaction: tran);

                    if (validacionPicking)
                    {
                        sql = @" SELECT 
                            C.NU_CARGA as Id,
                            C.DS_CARGA as Descripcion,
                            C.CD_ROTA as Ruta,
                            C.DT_ADDROW as FechaAlta,
                            C.NU_PREPARACION as Preparacion 
                        FROM T_CARGA C 
                        INNER JOIN T_CARGA_TEMP T ON C.NU_CARGA = T.NU_CARGA 
                        INNER JOIN V_EXP010_CARGA_CAMION CC ON C.NU_CARGA = CC.NU_CARGA 
                        WHERE (CC.CD_SITUACAO_CAMION IS NULL OR CC.CD_SITUACAO_CAMION = 650) 
                        GROUP BY 
                            C.NU_CARGA,
                            C.DS_CARGA,
                            C.CD_ROTA,
                            C.DT_ADDROW,
                            C.NU_PREPARACION ";
                    }
                    else
                    {
                        sql = @" SELECT 
                            C.NU_CARGA as Id,
                            C.DS_CARGA as Descripcion,
                            C.CD_ROTA as Ruta,
                            C.DT_ADDROW as FechaAlta,
                            C.NU_PREPARACION as Preparacion 
                        FROM T_CARGA C 
                        INNER JOIN T_CARGA_TEMP T ON C.NU_CARGA = T.NU_CARGA 
                        GROUP BY 
                            C.NU_CARGA,
                            C.DS_CARGA,
                            C.CD_ROTA,
                            C.DT_ADDROW,
                            C.NU_PREPARACION ";
                    }
                    resultado = _dapper.Query<Carga>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Pedido> GetPedidosHabilitados(IEnumerable<Pedido> pedidos)
        {
            IEnumerable<Pedido> resultado = new List<Pedido>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PEDIDO_SAIDA_TEMP (NU_PEDIDO, CD_EMPRESA, CD_CLIENTE) VALUES (:Id, :Empresa, :Cliente)";
                    _dapper.Execute(connection, sql, pedidos, transaction: tran);

                    sql = GetSqlSelectPedido() +
                        @" INNER JOIN T_PEDIDO_SAIDA_TEMP T ON P.NU_PEDIDO = T.NU_PEDIDO AND P.CD_EMPRESA = T.CD_EMPRESA AND P.CD_CLIENTE = T.CD_CLIENTE 
                        INNER JOIN V_EXP013_PEDIDO_CAMION PC ON P.NU_PEDIDO = PC.NU_PEDIDO AND P.CD_EMPRESA = PC.CD_EMPRESA AND P.CD_CLIENTE = PC.CD_CLIENTE 
                        WHERE (PC.CD_SITUACAO_CAMION IS NULL OR PC.CD_SITUACAO_CAMION = 650) ";

                    resultado = _dapper.Query<Pedido>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Contenedor> GetContenedoresHabilitados(IEnumerable<Contenedor> contenedores)
        {
            IEnumerable<Contenedor> resultado = new List<Contenedor>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CONTENEDOR_TEMP (ID_EXTERNO_CONTENEDOR, TP_CONTENEDOR, CD_EMPRESA) VALUES (:IdExterno, :TipoContenedor, :Empresa)";
                    _dapper.Execute(connection, sql, contenedores, transaction: tran);

                    sql = GetSqlSelectContenedor() +
                        @" , CC.CD_EMPRESA as Empresa,
                            CC.CD_ROTA_CARGA as Ruta
                        FROM T_CONTENEDOR co 
                        INNER JOIN T_CONTENEDOR_TEMP T ON co.ID_EXTERNO_CONTENEDOR = T.ID_EXTERNO_CONTENEDOR AND co.TP_CONTENEDOR= T.TP_CONTENEDOR
                        INNER JOIN V_EXP011_CONTENEDOR_CAMION CC ON co.NU_CONTENEDOR = CC.NU_CONTENEDOR AND co.NU_PREPARACION = CC.NU_PREPARACION AND T.CD_EMPRESA = CC.CD_EMPRESA 
                        WHERE (CC.CD_SITUACAO_CAMION IS NULL OR CC.CD_SITUACAO_CAMION = 650) AND CC.CD_SITUACAO_CONTENEDOR = 601 ";
                    
                    resultado = _dapper.Query<Contenedor>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual Dictionary<string, Pedido> GetPedidosConPendientes(IEnumerable<Pedido> pedidos)
        {
            var resultado = new Dictionary<string, Pedido>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PEDIDO_SAIDA_TEMP (NU_PEDIDO, CD_EMPRESA, CD_CLIENTE) VALUES (:Id, :Empresa, :Cliente)";
                    _dapper.Execute(connection, sql, pedidos, transaction: tran);

                    sql = GetSqlSelectPedido() +
                     @" INNER JOIN T_PEDIDO_SAIDA_TEMP T ON P.NU_PEDIDO = T.NU_PEDIDO AND P.CD_EMPRESA = T.CD_EMPRESA AND P.CD_CLIENTE = T.CD_CLIENTE
                        INNER JOIN V_PEDIDOS_CON_PENDIENTES pcp ON P.NU_PEDIDO = pcp.NU_PEDIDO AND P.CD_EMPRESA = pcp.CD_EMPRESA AND P.CD_CLIENTE = pcp.CD_CLIENTE ";

                    resultado = _dapper.Query<Pedido>(connection, sql, transaction: tran).ToDictionary(pcp => $"{pcp.Id}.{pcp.Empresa}.{pcp.Cliente}", pcp => pcp);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<DetallePreparacion> GetDetsPreparacionPedido(IEnumerable<Pedido> pedidos)
        {
            IEnumerable<DetallePreparacion> resultado = new List<DetallePreparacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_DET_PICKING_TEMP (NU_PEDIDO, CD_CLIENTE, CD_EMPRESA) VALUES (:Id, :Cliente, :Empresa)";
                    _dapper.Execute(connection, sql, pedidos, transaction: tran);

                    sql = @"SELECT 
                                dp.NU_PEDIDO as Pedido,
                                dp.CD_CLIENTE as Cliente,
                                dp.CD_EMPRESA as Empresa,
                                dp.NU_CARGA as Carga
                            FROM T_DET_PICKING dp
                            INNER JOIN T_DET_PICKING_TEMP T ON 
                            dp.NU_PEDIDO = T.NU_PEDIDO AND dp.CD_CLIENTE = T.CD_CLIENTE AND dp.CD_EMPRESA = T.CD_EMPRESA
                            GROUP BY 
                                dp.NU_PEDIDO,
                                dp.CD_CLIENTE,    
                                dp.CD_EMPRESA,     
                                dp.NU_CARGA ";

                    resultado = _dapper.Query<DetallePreparacion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<ContenedorExternoCarga> GetCargasContenedores(IEnumerable<Contenedor> contenedores)
        {
            IEnumerable<ContenedorExternoCarga> resultado = new List<ContenedorExternoCarga>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CONTENEDOR_TEMP (ID_EXTERNO_CONTENEDOR, TP_CONTENEDOR, CD_EMPRESA) VALUES (:IdExterno, :TipoContenedor, :Empresa)";
                    _dapper.Execute(connection, sql, contenedores, transaction: tran);

                    sql = @"SELECT 
                                co.ID_EXTERNO_CONTENEDOR as IdExternoContenedor,
                                co.TP_CONTENEDOR as TipoContenedor,
                                dp.CD_EMPRESA as Empresa,
                                dp.CD_CLIENTE as Cliente,
                                dp.NU_CARGA as Carga
                            FROM T_CONTENEDOR co 
                            INNER JOIN T_CONTENEDOR_TEMP T ON co.ID_EXTERNO_CONTENEDOR = T.ID_EXTERNO_CONTENEDOR AND co.TP_CONTENEDOR= T.TP_CONTENEDOR
                            INNER JOIN T_DET_PICKING dp ON co.NU_CONTENEDOR = dp.NU_CONTENEDOR AND co.NU_PREPARACION = dp.NU_PREPARACION  AND t.CD_EMPRESA = dp.CD_EMPRESA
                            WHERE co.CD_SITUACAO = 601
                            GROUP BY 
                                co.ID_EXTERNO_CONTENEDOR,
                                co.TP_CONTENEDOR,
                                dp.CD_EMPRESA,
                                dp.CD_CLIENTE,
                                dp.NU_CARGA ";

                    resultado = _dapper.Query<ContenedorExternoCarga>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual string GetSqlSelectPedido()
        {
            return @"SELECT 
                    P.NU_PEDIDO as Id,
                    P.CD_CLIENTE as Cliente,
                    P.CD_EMPRESA as Empresa,
                    P.CD_CONDICION_LIBERACION as CondicionLiberacion,
                    P.CD_FUN_RESP as FuncionarioResponsable,
                    P.CD_ORIGEN as Origen,
                    P.CD_PUNTO_ENTREGA as PuntoEntrega,
                    P.CD_ROTA as Ruta,
                    P.CD_SITUACAO as Estado,
                    P.CD_TRANSPORTADORA as CodigoTransportadora,
                    P.CD_UF as CodigoUF,
                    P.CD_ZONA as Zona,
                    P.DS_ANEXO1 as Anexo,
                    P.DS_ANEXO2 as Anexo2,
                    P.DS_ANEXO3 as Anexo3,
                    P.DS_ANEXO4 as Anexo4,
                    P.DS_ENDERECO as DireccionEntrega,
                    P.DS_MEMO as Memo,
                    P.DS_MEMO_1 as Memo1,
                    P.DT_ADDROW as FechaAlta,
                    P.DT_EMITIDO as FechaEmision,
                    P.DT_ENTREGA as FechaEntrega,
                    P.DT_FUN_RESP as FechaFuncionarioResponsable,
                    P.DT_GENERICO_1 as FechaGenerica_1,
                    P.DT_LIBERAR_DESDE as FechaLiberarDesde,
                    P.DT_LIBERAR_HASTA as FechaLiberarHasta,
                    P.DT_ULT_PREPARACION as FechaUltimaPreparacion,
                    P.DT_UPDROW as FechaModificacion,
                    P.FL_SYNC_REALIZADA as SincronizacionRealizadaId,
                    P.ID_AGRUPACION as Agrupacion,
                    P.ID_MANUAL as ManualId,
                    P.ND_ACTIVIDAD as Actividad,
                    P.NU_GENERICO_1 as NuGenerico_1,
                    P.NU_INTERFAZ_FACTURACION as NroIntzFacturacion,
                    P.NU_ORDEN_ENTREGA as OrdenEntrega,
                    P.NU_ORDEN_LIBERACION as NumeroOrdenLiberacion,
                    P.NU_PRDC_INGRESO as IngresoProduccion,
                    P.NU_PREDIO as Predio,
                    P.NU_PREPARACION_MANUAL as NroPrepManual,
                    P.NU_PREPARACION_PROGRAMADA as PreparacionProgramada,
                    P.NU_TRANSACCION as Transaccion,
                    P.NU_ULT_PREPARACION as NumeroUltimaPreparacion,
                    P.TP_EXPEDICION as TipoExpedicionId,
                    P.TP_PEDIDO as Tipo,
                    P.VL_COMPARTE_CONTENEDOR_ENTREGA as ComparteContenedorEntrega,
                    P.VL_COMPARTE_CONTENEDOR_PICKING as ComparteContenedorPicking,
                    P.VL_GENERICO_1 as VlGenerico_1,
                    P.VL_SERIALIZADO_1 as VlSerealizado_1,
                    P.NU_CARGA as NuCarga
                FROM T_PEDIDO_SAIDA P ";
        }
        
        public virtual string GetSqlSelectContenedor()
        {
            return @"SELECT 
                          co.NU_CONTENEDOR as Numero,
                          co.NU_PREPARACION as NumeroPreparacion,
                          co.TP_CONTENEDOR as TipoContenedor,
                          co.CD_CAMION as CodigoCamion,
                          co.CD_AGRUPADOR as CodigoAgrupador,
                          co.CD_CAMION_CONGELADO as CodigoCamionCongelado,
                          co.CD_CAMION_FACTURADO as CamionFacturado,
                          co.CD_CANAL as CodigoCanal,
                          co.CD_ENDERECO as Ubicacion,
                          co.CD_FUNCIONARIO_EXPEDICION as CodigoFuncionarioExpedicion,
                          co.CD_PORTA as CodigoPuerta,
                          co.CD_SITUACAO as EstadoId,
                          co.CD_SUB_CLASSE as CodigoSubClase,
                          co.CD_UNIDAD_BULTO as CodigoUnidadBulto,
                          co.DS_CONTENEDOR as DescripcionContenedor,
                          co.DT_ADDROW as FechaAgregado,
                          co.DT_EXPEDIDO as FechaExpedido,
                          co.DT_PULMON as FechaPulmon,
                          co.DT_UPDROW as FechaModificado,
                          co.FL_HABILITADO as Habilitado,
                          co.FL_SEPARADO_DOS_FASES as SegundaFase,
                          co.ID_CONTENEDOR_EMPAQUE as IdContenedorEmpaque,
                          co.ID_PRECINTO_1 as Precinto1,
                          co.ID_PRECINTO_2 as Precinto2,
                          co.NU_TRANSACCION as NumeroTransaccion,
                          co.NU_UNIDAD_TRANSPORTE as NumeroUnidadTransporte,
                          co.NU_VIAJE as NumeroViaje,
                          co.PS_REAL as PesoReal,
                          co.QT_BULTO as CantidadBulto,
                          co.TP_CONTROL as TipoControl,
                          co.VL_ALTURA as Altura,
                          co.VL_CONTROL as ValorControl,
                          co.VL_CUBAGEM as ValorCubagem,
                          co.VL_LARGURA as Largo,
                          co.VL_PROFUNDIDADE as Profundidad,
                          co.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,                          
                          co.ID_EXTERNO_CONTENEDOR as IdExterno,
                          co.CD_BARRAS as CodigoBarras,
                          co.NU_LPN as NroLpn,
                          co.ID_EXTERNO_TRACKING as IdExternoTracking ";
        }

        public virtual void AddCarga(DbConnection connection, DbTransaction tran, Carga carga)
        {
            string sql = @"INSERT INTO T_CARGA (NU_CARGA,  DS_CARGA,NU_PREPARACION,DT_ADDROW,CD_ROTA) 
                                   VALUES (:Id, :Descripcion, :Preparacion, :FechaAlta, :Ruta)";
            _dapper.Execute(connection, sql, carga, transaction: tran);
        }
        #endregion
    }
}
