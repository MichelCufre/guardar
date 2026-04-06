using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.General.Enums;
using WIS.Domain.Registro;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ProductoRepository
    {
        protected readonly int _userId;
        protected readonly WISDB _context;
        protected readonly IDapper _dapper;
        protected readonly string _cdAplicacion;
        protected readonly ProductoMapper _mapper;
        protected readonly ParametroRepository _parametroRepository;

        public ProductoRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ProductoMapper();
            this._dapper = dapper;
            this._parametroRepository = new ParametroRepository(_context, cdAplicacion, userId, dapper);
        }

        #region Any

        public virtual bool ExisteProducto(int cdEmpresa, string cdProducto)
        {
            return this._context.T_PRODUTO.Any(d => d.CD_EMPRESA == cdEmpresa && d.CD_PRODUTO == cdProducto && d.T_EMPRESA.T_EMPRESA_FUNCIONARIO.Any(e => e.USERID == this._userId));
        }

        public virtual bool ExisteProductoFamilia(int codigoFamilia)
        {
            return this._context.T_PRODUTO.Any(d => d.CD_FAMILIA_PRODUTO == codigoFamilia);
        }

        public virtual bool ExisteReferenciaProductoEmpresa(string referencia, int empresa)
        {
            return this._context.T_PRODUTO.Any(x => x.CD_PRODUTO_EMPRESA == referencia && x.CD_EMPRESA == empresa);
        }

        public virtual bool ExistePalletAsignadoProducto(int empresa, string codigoProducto, short codigoPalet)
        {
            return this._context.V_PRODUCTO_PALLET_WREG605.Any(pa => pa.CD_EMPRESA == empresa && pa.CD_PRODUTO == codigoProducto && pa.CD_FAIXA == 1 && pa.CD_PALLET == codigoPalet);
        }

        public virtual bool RamoEnUso(short codigoRamo)
        {
            return this._context.T_PRODUTO.Any(x => x.CD_RAMO_PRODUTO == codigoRamo);
        }

        public virtual bool ProductoAceptaDecimales(int idEmpresa, string codigoProducto)
        {
            string aceptaDecimales = this._context.T_PRODUTO.FirstOrDefault(p => p.CD_PRODUTO == codigoProducto && p.CD_EMPRESA == idEmpresa)?.FL_ACEPTA_DECIMALES;

            return this._mapper.MapStringToBoolean(aceptaDecimales);
        }

        public virtual bool AnyProducto(string cdProducto, int empresa)
        {
            return this._context.T_PRODUTO.Any(d => d.CD_PRODUTO == cdProducto && d.CD_EMPRESA == empresa);
        }

        public virtual bool AnyCodigoExternoDistintoProductoProveedor(int empresa, string cliente, string codigoExterno, string producto)
        {
            return _context.T_PRODUTO_CONVERTOR.Any(p => p.CD_EMPRESA == empresa && p.CD_CLIENTE == cliente && p.CD_EXTERNO == codigoExterno && p.CD_PRODUTO != producto);
        }

        public virtual bool AnyProductoProveedor(int empresa, string cliente, string producto)
        {
            return this._context.T_PRODUTO_CONVERTOR
                .AsNoTracking()
                .Any(d => d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && d.CD_PRODUTO == producto);
        }

        #endregion

        #region Get
        public virtual Producto GetPro(string descripcionProducto, int empresa)
        {
            Producto producto = GetByDescriptionPartial(empresa, descripcionProducto).FirstOrDefault();

            return producto;
        }

        public virtual List<Producto> GetByDescriptionPartial(int cdEmpresa, string value)
        {
            return (from d in (from d in _context.T_PRODUTO.AsNoTracking()
                               where d.CD_EMPRESA == cdEmpresa && string.Concat(d.CD_PRODUTO + " ", d.DS_PRODUTO).ToLower().Contains(value.ToLower())
                               select d).ToList()
                    select _mapper.MapToObject(d)).ToList();
        }

        public virtual ProductoProveedor GetProductoProveedor(int empresa, string producto, string cliente)
        {
            T_PRODUTO_CONVERTOR attachedEntity = _context.T_PRODUTO_CONVERTOR
                .FirstOrDefault(x => x.CD_EMPRESA == empresa
                   && x.CD_CLIENTE == cliente
                   && x.CD_PRODUTO == producto);

            return this._mapper.MapToObject(attachedEntity);
        }

        public virtual string ExisteMercadologicoEmpresaProducto(string mercado, int empresa)
        {
            return this._context.T_PRODUTO.FirstOrDefault(x => x.CD_MERCADOLOGICO == mercado && x.CD_EMPRESA == empresa)?.CD_PRODUTO;
        }

        public virtual Producto GetProductoEmpresaAsignadaUsuario(int cdEmpresa, string cdProducto, bool retornarException = true)
        {
            var entity = _context.T_PRODUTO
                .Join(_context.T_EMPRESA_FUNCIONARIO,
                    p => p.CD_EMPRESA,
                    ef => ef.CD_EMPRESA,
                    (p, ef) => new { Producto = p, EmpresaFuncionario = ef })
                .AsNoTracking()
                .Where(d => d.Producto.CD_EMPRESA == cdEmpresa
                    && d.Producto.CD_PRODUTO == cdProducto
                    && d.EmpresaFuncionario.USERID == this._userId)
                .Select(x => x.Producto)
                .Include("T_CLASSE")
                .FirstOrDefault();

            if (entity == null)
            {
                if (retornarException)
                    throw new EntityNotFoundException("General_Sec0_Error_Error05");
                else
                    return null;
            }

            var clase = this._context.T_CLASSE.FirstOrDefault(x => x.CD_CLASSE == entity.CD_CLASSE);
            var rotatividad = this._context.T_ROTATIVIDADE.FirstOrDefault(x => x.CD_ROTATIVIDADE == entity.CD_ROTATIVIDADE);
            var empresa = this._context.T_EMPRESA.FirstOrDefault(x => x.CD_EMPRESA == cdEmpresa);
            var familia = this._context.T_FAMILIA_PRODUTO.FirstOrDefault(d => d.CD_FAMILIA_PRODUTO == entity.CD_FAMILIA_PRODUTO);

            entity.T_EMPRESA = empresa;
            entity.T_CLASSE = clase;
            entity.T_ROTATIVIDADE = rotatividad;
            entity.T_FAMILIA_PRODUTO = familia;

            return this._mapper.MapToObject(entity);
        }

        public virtual Producto GetProducto(int cdEmpresa, string cdProducto)
        {
            var entity = this._context.T_PRODUTO
                .Include("T_CLASSE")
                .AsNoTracking()
                .Where(d => d.CD_EMPRESA == cdEmpresa && d.CD_PRODUTO == cdProducto)
                .FirstOrDefault();

            if (entity == null)
                return null;

            var clase = this._context.T_CLASSE.FirstOrDefault(x => x.CD_CLASSE == entity.CD_CLASSE);
            var rotatividad = this._context.T_ROTATIVIDADE.FirstOrDefault(x => x.CD_ROTATIVIDADE == entity.CD_ROTATIVIDADE);
            var empresa = this._context.T_EMPRESA.FirstOrDefault(x => x.CD_EMPRESA == cdEmpresa);
            var familia = this._context.T_FAMILIA_PRODUTO.FirstOrDefault(d => d.CD_FAMILIA_PRODUTO == entity.CD_FAMILIA_PRODUTO);

            entity.T_EMPRESA = empresa;
            entity.T_CLASSE = clase;
            entity.T_ROTATIVIDADE = rotatividad;
            entity.T_FAMILIA_PRODUTO = familia;

            return this._mapper.MapToObject(entity);
        }

        public virtual string GetDescripcion(int empresa, string producto)
        {
            return _context.T_PRODUTO.FirstOrDefault(p => p.CD_PRODUTO == producto && p.CD_EMPRESA == empresa)?.DS_PRODUTO;
        }

        public virtual ManejoIdentificador GetProductoManejoIdentificador(int idEmpresa, string codigoProducto)
        {
            string idManejo = _context.T_PRODUTO.Where(p => p.CD_PRODUTO == codigoProducto && p.CD_EMPRESA == idEmpresa)
                                     .Select(p => (p != null ? p.ID_MANEJO_IDENTIFICADOR : ManejoIdentificadorDb.Producto))
                                     .FirstOrDefault();

            return this._mapper.MapManejoIdentificador(idManejo);
        }

        public virtual ManejoFechaProducto GetProductoManejoFecha(int idEmpresa, string codigoProducto)
        {
            string idManejo = _context.T_PRODUTO.Where(p => p.CD_PRODUTO == codigoProducto && p.CD_EMPRESA == idEmpresa)
                                     .Select(p => (p != null ? p.TP_MANEJO_FECHA : ManejoFechaProductoDb.Duradero))
                                     .FirstOrDefault();

            return this._mapper.MapManejoFecha(idManejo);
        }

        public virtual string GetReferenciaProductoEmpresa(string referencia, int empresa)
        {
            return this._context.T_PRODUTO.FirstOrDefault(x => x.CD_PRODUTO_EMPRESA == referencia && x.CD_EMPRESA == empresa).CD_PRODUTO;
        }

        public virtual List<Producto> GetByDescriptionOrCodePartial(int cdEmpresa, string value, bool? permiteProductosInactivos = null)
        {
            if (!permiteProductosInactivos.HasValue)
            {
                permiteProductosInactivos = (_parametroRepository.GetParameter(ParamManager.USAR_PRODUCTOS_INACTIVOS, new Dictionary<string, string>
                {
                    [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{cdEmpresa}",
                }) ?? "S") == "S";
            }

            return this._context.T_PRODUTO
                .AsNoTracking()
                .Where(d => d.CD_EMPRESA == cdEmpresa
                    && (d.CD_PRODUTO.ToLower().Contains(value.ToLower()) || d.DS_PRODUTO.ToLower().Contains(value.ToLower()))
                    && (d.T_EMPRESA.T_EMPRESA_FUNCIONARIO.Any(e => e.USERID == this._userId))
                    && (permiteProductosInactivos.Value ? true : d.CD_SITUACAO == SituacionDb.Activo))
                .Select(d => this._mapper.MapToObject(d))
                .ToList();
        }

        public virtual List<SelectOption> GetByDescriptionOrCodePartial(string cdCliente, int empresa, string searchValue)
        {
            searchValue = searchValue.ToLower();

            return _context.V_REC500_SEARCH_PRODUCTO
               .Where(a => a.CD_EMPRESA == empresa && a.VL_CAMPO_BUSQUEDA.ToLower().Contains(searchValue))
               .Take(50)
               .ToList()
               .Select(s => new SelectOption { Value = s.CD_PRODUTO, Label = $"{s.CD_PRODUTO} - {s.DS_PRODUTO}" })
               .ToList();
        }

        public virtual string GetClaseProducto(string producto, int empresa)
        {
            return _context.T_PRODUTO.AsNoTracking()
                .FirstOrDefault(p => p.CD_PRODUTO == producto && p.CD_EMPRESA == empresa)?.CD_CLASSE;
        }

        public virtual ProductoPallet GetProductoPallet(int cdEmpresa, string cdProducto, short cdPallet, decimal faixa)
        {
            var entity = this._context.T_PRODUTO_PALLET
                .FirstOrDefault(d => d.CD_EMPRESA == cdEmpresa &&
                    d.CD_PRODUTO == cdProducto &&
                    d.CD_PALLET == cdPallet &&
                    d.CD_FAIXA == faixa);

            if (entity == null)
                return null;

            return this._mapper.MapToObject(entity);
        }

        #endregion

        #region Add

        public virtual void AddProducto(Producto producto)
        {
            var entity = this._mapper.MapToEntity(producto);

            entity.SG_PRODUTO = "X";
            entity.DS_DIFER_PESO_QTDE = "X";
            entity.CD_GRUPO_CONSULTA = "S/N";
            entity.ID_CROSS_DOCKING = "N";
            entity.DT_SITUACAO = DateTime.Now;
            entity.DT_ADDROW = DateTime.Now;
            entity.TP_PESO_PRODUTO = 1;

            this._context.T_PRODUTO.Add(entity);
        }

        public virtual void AddProductoEmbalaje(Producto producto)
        {
            this._context.T_PRODUTO_FAIXA.Add(new T_PRODUTO_FAIXA
            {
                CD_PRODUTO = producto.Codigo,
                CD_EMPRESA = producto.CodigoEmpresa,
                CD_FAIXA = 1,
                CD_EMBALAGEM_FAIXA = "UND",
                QT_UNIDADE_EMBALAGEM = 1,
                DT_ADDROW = DateTime.Now,
                DT_UPDROW = DateTime.Now
            });
        }

        public virtual void AddProductoPallet(ProductoPallet productoPallet)
        {
            var productoFaixa = this._context.T_PRODUTO_FAIXA
                .FirstOrDefault(p => p.CD_PRODUTO == productoPallet.CodigoProducto
                    && p.CD_EMPRESA == productoPallet.Empresa);

            productoPallet.Embalaje = productoFaixa.CD_FAIXA;

            T_PRODUTO_PALLET entity = this._mapper.MapToEntity(productoPallet);

            this._context.T_PRODUTO_PALLET.Add(entity);
        }

        public virtual void AddProductoProveedor(ProductoProveedor productoProveedor)
        {
            T_PRODUTO_CONVERTOR entity = this._mapper.MapToEntity(productoProveedor);
            this._context.T_PRODUTO_CONVERTOR.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateProducto(Producto producto)
        {
            var entity = this._mapper.MapToEntity(producto);
            var attachedEntity = _context.T_PRODUTO.Local
                .FirstOrDefault(x => x.CD_EMPRESA == entity.CD_EMPRESA
                    && x.CD_PRODUTO == entity.CD_PRODUTO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRODUTO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateProductoPallet(ProductoPallet productoPallet)
        {
            var entity = this._mapper.MapToEntity(productoPallet);
            var attachedEntity = _context.T_PRODUTO_PALLET.Local
                .FirstOrDefault(x => x.CD_EMPRESA == entity.CD_EMPRESA
                    && x.CD_PALLET == entity.CD_PALLET
                    && x.CD_PRODUTO == entity.CD_PRODUTO
                    && x.CD_FAIXA == entity.CD_FAIXA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRODUTO_PALLET.Attach(entity);
                _context.Entry<T_PRODUTO_PALLET>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateProductoProveedor(ProductoProveedor ProductoProveedor)
        {
            var entity = this._mapper.MapToEntity(ProductoProveedor);
            var attachedEntity = _context.T_PRODUTO_CONVERTOR.Local
                .FirstOrDefault(x => x.CD_EMPRESA == entity.CD_EMPRESA
                    && x.CD_CLIENTE == entity.CD_CLIENTE
                    && x.CD_PRODUTO == entity.CD_PRODUTO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRODUTO_CONVERTOR.Attach(entity);
                _context.Entry<T_PRODUTO_CONVERTOR>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveProductoPallet(ProductoPallet productoPallet)
        {
            var entity = this._mapper.MapToEntity(productoPallet);
            var attachedEntity = _context.T_PRODUTO_PALLET.Local
                .FirstOrDefault(x => x.CD_EMPRESA == entity.CD_EMPRESA
                    && x.CD_PALLET == entity.CD_PALLET
                    && x.CD_PRODUTO == entity.CD_PRODUTO
                    && x.CD_FAIXA == entity.CD_FAIXA);

            if (attachedEntity != null)
            {
                this._context.T_PRODUTO_PALLET.Remove(attachedEntity);
            }
            else
            {
                this._context.T_PRODUTO_PALLET.Attach(entity);
                this._context.T_PRODUTO_PALLET.Remove(entity);
            }
        }

        public virtual void RemoveProductoProveedor(ProductoProveedor productoProveedor)
        {
            var entity = this._mapper.MapToEntity(productoProveedor);

            var attachedEntity = _context.T_PRODUTO_CONVERTOR.Local
                .FirstOrDefault(x => x.CD_EMPRESA == entity.CD_EMPRESA
                    && x.CD_CLIENTE == entity.CD_CLIENTE
                    && x.CD_PRODUTO == entity.CD_PRODUTO);

            if (attachedEntity != null)
            {
                this._context.T_PRODUTO_CONVERTOR.Remove(attachedEntity);
            }
            else
            {
                this._context.T_PRODUTO_CONVERTOR.Attach(entity);
                this._context.T_PRODUTO_CONVERTOR.Remove(entity);
            }
        }
        #endregion

        #region Dapper

        #region Producto

        public virtual IEnumerable<Producto> GetProductos(IEnumerable<Producto> productos, out HashSet<string> noEditables)
        {
            IEnumerable<Producto> resultado = new List<Producto>();

            noEditables = new HashSet<string>();
            var permiteModificarSinValidar = (_parametroRepository.GetParameter(ParamManager.PERMITIR_MOD_DATOS_LOGISTICOS) ?? "N") == "S";

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRODUTO_TEMP (CD_PRODUTO, CD_EMPRESA) VALUES (:Codigo, :CodigoEmpresa)";
                    _dapper.Execute(connection, sql, productos, transaction: tran);

                    sql = GetSqlSelectProducto() +
                        @"INNER JOIN T_PRODUTO_TEMP T ON P.CD_PRODUTO = T.CD_PRODUTO AND P.CD_EMPRESA = T.CD_EMPRESA";

                    resultado = _dapper.Query<Producto>(connection, sql, transaction: tran);

                    if (!permiteModificarSinValidar)
                    {
                        sql = @"SELECT PUH.CD_PRODUTO 
                            FROM V_API_PROD_UPDATE_HABILITADO PUH 
                                INNER JOIN T_PRODUTO_TEMP T ON PUH.CD_PRODUTO = T.CD_PRODUTO AND PUH.CD_EMPRESA = T.CD_EMPRESA";

                        foreach (var codigo in _dapper.Query<string>(connection, sql, transaction: tran))
                        {
                            noEditables.Add(codigo);
                        }
                    }

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Producto> GetProductos(IEnumerable<Producto> productos)
        {
            IEnumerable<Producto> resultado = new List<Producto>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRODUTO_TEMP (CD_PRODUTO, CD_EMPRESA) VALUES (:Codigo, :CodigoEmpresa)";
                    _dapper.Execute(connection, sql, productos, transaction: tran);

                    sql = GetSqlSelectProducto() +
                        @"INNER JOIN T_PRODUTO_TEMP T ON P.CD_PRODUTO = T.CD_PRODUTO AND P.CD_EMPRESA = T.CD_EMPRESA";

                    resultado = _dapper.Query<Producto>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual Dictionary<string, string> GetCodigosByProductoEmpresa(List<Producto> productos)
        {
            Dictionary<string, string> resultado = new Dictionary<string, string>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRODUTO_TEMP (CD_PRODUTO_EMPRESA, CD_EMPRESA) VALUES (:CodigoProductoEmpresa, :CodigoEmpresa)";
                    _dapper.Execute(connection, sql, productos, transaction: tran);

                    sql = @"SELECT P.CD_PRODUTO AS Codigo, P.CD_PRODUTO_EMPRESA AS CodigoProductoEmpresa
                        FROM T_PRODUTO P
                            INNER JOIN T_PRODUTO_TEMP T ON P.CD_PRODUTO_EMPRESA = T.CD_PRODUTO_EMPRESA
                                AND P.CD_EMPRESA = T.CD_EMPRESA";

                    foreach (var p in _dapper.Query<Producto>(connection, sql, transaction: tran))
                    {
                        resultado[p.CodigoProductoEmpresa] = p.Codigo;
                    }

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task AddProductos(List<Producto> productos, IProductoServiceContext context, CancellationToken cancelToken = default)
        {
            await AddProductos(GetBulkOperationContext(productos, context), cancelToken);
        }

        public virtual async Task AddProductos(ProductoBulkOperationContext context, CancellationToken cancelToken)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertClases(connection, tran, context.NewClases);
                    await BulkInsertFamilias(connection, tran, context.NewFamilias);
                    await BulkInsertRamos(connection, tran, context.NewRamos);
                    await BulkInsertProductos(connection, tran, context.NewProductos);
                    await BulkInsertFaixas(connection, tran, context.NewFaixas);
                    await BulkUpdateProductos(connection, tran, context.UpdProductos);
                    await BulkInsertVentanaLiberacion(connection, tran, context.NewVentanaLiberacion);

                    tran.Commit();
                }
            }
        }

        public virtual async Task BulkInsertVentanaLiberacion(DbConnection connection, DbTransaction tran, List<object> newVentanaLiberacion)
        {
            var sql = @"INSERT INTO T_DET_DOMINIO 
                        (NU_DOMINIO,
                        CD_DOMINIO,
                        CD_DOMINIO_VALOR) values (:Dominio,
                        :Codigo,
                        :Valor
                        )";

            await _dapper.ExecuteAsync(connection, sql, newVentanaLiberacion, transaction: tran);

        }

        public virtual ProductoBulkOperationContext GetBulkOperationContext(List<Producto> productos, IProductoServiceContext serviceContext)
        {
            var context = new ProductoBulkOperationContext();
            var newProductos = new Dictionary<string, Producto>();
            var newClases = new Dictionary<string, object>();
            var newFamilias = new Dictionary<int, object>();
            var newRamos = new Dictionary<short, object>();
            var newVentanaLiberacion = new Dictionary<string, object>();

            foreach (var producto in productos)
            {
                newProductos[producto.Codigo] = producto;

                if (!string.IsNullOrEmpty(producto.CodigoClase) && !serviceContext.ExisteClase(producto.CodigoClase))
                {
                    newClases[producto.CodigoClase] = GetClaseEntity(producto.CodigoClase);
                }

                if (!serviceContext.ExisteFamilia(producto.CodigoFamilia))
                {
                    newFamilias[producto.CodigoFamilia] = GetFamiliaEntity(producto.CodigoFamilia);
                }

                if (!serviceContext.ExisteRamo(producto.Ramo))
                {
                    newRamos[producto.Ramo] = GetRamoEntity(producto.Ramo);
                }

                if (!string.IsNullOrEmpty(producto.VentanaLiberacion) && !serviceContext.ExisteVentanaLiberacion(producto.VentanaLiberacion))
                    newVentanaLiberacion[producto.VentanaLiberacion] = GetVentanaLiberacionEntity(producto.VentanaLiberacion);
            }

            foreach (var model in serviceContext.Productos.Values)
            {
                var producto = newProductos[model.Codigo];
                newProductos.Remove(model.Codigo);
                producto = Map(producto, model);
                context.UpdProductos.Add(GetProductoEntity(producto));
            }

            foreach (var producto in newProductos.Values)
            {
                context.NewProductos.Add(GetProductoEntity(producto));
                context.NewFaixas.Add(GetFaixaEntity(producto));
            }

            context.NewClases.AddRange(newClases.Values);
            context.NewFamilias.AddRange(newFamilias.Values);
            context.NewRamos.AddRange(newRamos.Values);
            context.NewVentanaLiberacion.AddRange(newVentanaLiberacion.Values);

            return context;
        }

        public virtual async Task BulkInsertRamos(DbConnection connection, DbTransaction tran, List<object> ramos)
        {
            string sql = @"INSERT INTO T_RAMO_PRODUTO (CD_RAMO_PRODUTO, DS_RAMO_PRODUTO, DT_ADDROW) VALUES (:Codigo, :Descripcion, :Alta)";
            await _dapper.ExecuteAsync(connection, sql, ramos, transaction: tran);
        }

        public virtual async Task BulkInsertFamilias(DbConnection connection, DbTransaction tran, List<object> familias)
        {
            string sql = @"INSERT INTO T_FAMILIA_PRODUTO (CD_FAMILIA_PRODUTO, DS_FAMILIA_PRODUTO, DT_ADDROW) VALUES (:Codigo, :Descripcion, :Alta)";
            await _dapper.ExecuteAsync(connection, sql, familias, transaction: tran);
        }

        public virtual async Task BulkInsertClases(DbConnection connection, DbTransaction tran, List<object> clases)
        {
            string sql = @"INSERT INTO T_CLASSE (CD_CLASSE, DS_CLASSE, DT_ADDROW) VALUES (:Codigo, :Descripcion, :Alta)";
            await _dapper.ExecuteAsync(connection, sql, clases, transaction: tran);
        }

        public virtual async Task BulkInsertFaixas(DbConnection connection, DbTransaction tran, List<object> faixas)
        {
            var sql = @"INSERT INTO T_PRODUTO_FAIXA (CD_EMPRESA, CD_PRODUTO, CD_FAIXA, DT_UPDROW, DT_ADDROW, CD_EMBALAGEM_FAIXA, VL_ALTURA, VL_LARGURA,VL_PROFUNDIDADE) 
                        VALUES (:CodigoEmpresa, :Codigo, :CodigoFaixa, :Updrow, :Addrow, :CdEmbalagemFaixa, :VL_ALTURA, :VL_LARGURA, :VL_PROFUNDIDADE)";

            await _dapper.ExecuteAsync(connection, sql, faixas, transaction: tran);
        }

        public virtual async Task BulkUpdateProductos(DbConnection connection, DbTransaction tran, List<object> productos)
        {
            string sql = @"
                        UPDATE T_PRODUTO SET 
                            CD_CLASSE = :CodigoClase,
                            CD_EXCLUSIVO = :Exclusivo, 
                            CD_FAMILIA_PRODUTO = :CodigoFamilia,
                            CD_GRUPO_CONSULTA = :GrupoConsulta,
                            CD_NAM = :NAM, 
                            CD_NIVEL = :Nivel,
                            CD_PRODUTO_EMPRESA = :CodigoProductoEmpresa, 
                            CD_PRODUTO_UNICO = :ProductoUnico,
                            CD_RAMO_PRODUTO = :Ramo,
                            CD_ROTATIVIDADE = :CodigoRotatividad,
                            CD_SITUACAO = :Situacion,
                            CD_UND_MEDIDA_FACT = :UndMedidaFact,
                            CD_UNIDADE_MEDIDA = :UnidadMedida,
                            CD_UNID_EMB = :UndEmb,
                            CODIGO_BASE= :CodigoBase,
                            COLOR= :Color,
                            DS_ANEXO1 = :Anexo1, 
                            DS_ANEXO2 = :Anexo2, 
                            DS_ANEXO3 = :Anexo3, 
                            DS_ANEXO4 = :Anexo4, 
                            DS_ANEXO5 = :Anexo5, 
                            DS_DIFER_PESO_QTDE = :DescDifPeso, 
                            DS_DISPLAY = :DescripcionDisplay,
                            DS_HELP_COLECTOR = :AyudaColector, 
                            DS_PRODUTO = :Descripcion,
                            DS_REDUZIDA = :DescripcionReducida,
                            DT_SITUACAO = :FechaSituacion,
                            DT_UPDROW = :Modificacion,
                            FL_ACEPTA_DECIMALES = :AceptaDecimales,
                            FT_CONVERSAO = :Conversion,
                            ID_AGRUPACION = :Agrupacion,
                            ID_CROSS_DOCKING = :IdCrossDocking,
                            ID_MANEJA_TOMA_DATO = :ManejaTomaDato,
                            ID_MANEJO_IDENTIFICADOR= :ManejoIdentificador,
                            ID_REDONDEO_VALIDEZ= :RedondeoValidez,
                            ND_FACTURACION_COMP1= :Componente1,
                            ND_FACTURACION_COMP2= :Componente2,
                            NU_TRANSACCION = :Transaccion, 
                            PS_BRUTO = :PesoBruto,
                            PS_LIQUIDO = :PesoNeto,
                            QT_DIAS_DURACAO = :DiasDuracion,
                            QT_DIAS_VALIDADE = :DiasValidez, 
                            QT_DIAS_VALIDADE_LIBERACION = :DiasLiberacion,
                            QT_ESTOQUE_MAXIMO = :StockMaximo,
                            QT_ESTOQUE_MINIMO = :StockMinimo,
                            QT_GENERICO = :CantidadGenerica,
                            QT_PADRON_STOCK= :CantidadPadronStock,
                            QT_SUBBULTO = :SubBulto, 
                            QT_UND_BULTO = :UnidadBulto,
                            QT_UND_DISTRIBUCION = :UnidadDistribucion,
                            SG_PRODUTO = :SgProducto,
                            TALLE= :Talle,
                            TEMPORADA= :Temporada,
                            TP_DISPLAY = :TipoDisplay, 
                            TP_MANEJO_FECHA = :TipoManejoFecha,
                            TP_PESO_PRODUTO = :TipoPeso,
                            VL_ALTURA = :Altura,
                            VL_AVISO_AJUSTE = :AvisoAjusteInventario, 
                            VL_CATEGORIA_01= :Categoria1,
                            VL_CATEGORIA_02= :Categoria2,
                            VL_CUBAGEM = :VolumenCC,
                            VL_CUSTO_ULT_ENT = :UltimoCosto, 
                            VL_LARGURA = :Ancho,
                            VL_PRECIO_DISTRIB = :PrecioDistribucion,
                            VL_PRECIO_EGRESO = :PrecioEgreso,
                            VL_PRECIO_INGRESO = :PrecioIngreso,
                            VL_PRECIO_SEG_DISTR = :PrecioSegDistribucion,
                            VL_PRECIO_SEG_STOCK = :PrecioSegStock,
                            VL_PRECIO_STOCK = :PrecioStock,
                            VL_PROFUNDIDADE = :Profundidad,
                            VL_PRECO_VENDA = :PrecioVenta ,
                            CD_VENTANA_LIBERACION = :VentanaLiberacion,
                            ND_MODALIDAD_INGRESO_LOTE = :ModalidadIngresoLote
                        WHERE CD_PRODUTO = :Codigo AND CD_EMPRESA = :CodigoEmpresa";

            await _dapper.ExecuteAsync(connection, sql, productos, transaction: tran);
        }

        public virtual async Task BulkInsertProductos(DbConnection connection, DbTransaction tran, List<object> productos)
        {
            var sql = @"INSERT INTO T_PRODUTO 
                        (CD_EMPRESA,
                        CD_PRODUTO,
                        CD_PRODUTO_EMPRESA,
                        CD_NAM,
                        CD_MERCADOLOGICO,
                        SG_PRODUTO,
                        TP_PESO_PRODUTO,
                        DS_DIFER_PESO_QTDE,
                        DS_PRODUTO,
                        CD_UNIDADE_MEDIDA,
                        CD_FAMILIA_PRODUTO,
                        CD_ROTATIVIDADE,
                        CD_CLASSE,
                        QT_ESTOQUE_MINIMO,
                        QT_ESTOQUE_MAXIMO,
                        PS_LIQUIDO,
                        PS_BRUTO,
                        FT_CONVERSAO,
                        VL_CUBAGEM,
                        VL_PRECO_VENDA,
                        VL_CUSTO_ULT_ENT,
                        CD_ORIGEM,
                        DS_REDUZIDA,
                        CD_NIVEL,
                        CD_UNID_EMB,
                        CD_SITUACAO,
                        DT_SITUACAO,
                        QT_DIAS_VALIDADE,
                        QT_DIAS_DURACAO,
                        ID_CROSS_DOCKING,
                        ID_REDONDEO_VALIDEZ,
                        ID_AGRUPACION,
                        ID_MANEJO_IDENTIFICADOR,
                        TP_DISPLAY,
                        DS_ANEXO1,
                        DS_ANEXO2,
                        DS_ANEXO3,
                        DS_ANEXO4,
                        DT_UPDROW,
                        DT_ADDROW,
                        VL_ALTURA,
                        VL_LARGURA,
                        VL_PROFUNDIDADE,
                        TP_MANEJO_FECHA,
                        VL_AVISO_AJUSTE,
                        DS_HELP_COLECTOR,
                        QT_SUBBULTO,
                        CD_EXCLUSIVO,
                        QT_UND_DISTRIBUCION,
                        QT_DIAS_VALIDADE_LIBERACION,
                        QT_UND_BULTO,
                        ID_MANEJA_TOMA_DATO,
                        DS_ANEXO5,
                        CD_GRUPO_CONSULTA,
                        DS_DISPLAY,
                        VL_PRECIO_SEG_DISTR,
                        VL_PRECIO_SEG_STOCK,
                        VL_PRECIO_DISTRIB,
                        VL_PRECIO_EGRESO,
                        VL_PRECIO_INGRESO,
                        VL_PRECIO_STOCK,
                        CD_UND_MEDIDA_FACT,
                        CD_PRODUTO_UNICO,
                        CD_RAMO_PRODUTO,
                        FL_ACEPTA_DECIMALES,
                        ND_FACTURACION_COMP1,
                        ND_FACTURACION_COMP2,
                        ND_MODALIDAD_INGRESO_LOTE,
                        QT_PADRON_STOCK,
                        QT_GENERICO,
                        CODIGO_BASE,
                        TALLE,
                        COLOR,
                        TEMPORADA,
                        VL_CATEGORIA_01,
                        VL_CATEGORIA_02,
                        NU_TRANSACCION,
                        CD_VENTANA_LIBERACION) values (:CodigoEmpresa,
                        :Codigo,
                        :CodigoProductoEmpresa,
                        :NAM,
                        :CodigoMercadologico,
                        :SgProducto,
                        :TipoPeso,
                        :DescDifPeso,
                        :Descripcion,
                        :UnidadMedida,
                        :CodigoFamilia,
                        :CodigoRotatividad,
                        :CodigoClase,
                        :StockMinimo,
                        :StockMaximo,
                        :PesoNeto,
                        :PesoBruto,
                        :Conversion,
                        :VolumenCC,
                        :PrecioVenta,
                        :UltimoCosto,
                        :CodOrigen,
                        :DescripcionReducida,
                        :Nivel,
                        :UndEmb,
                        :Situacion,
                        :FechaSituacion,
                        :DiasValidez,
                        :DiasDuracion,
                        :IdCrossDocking,
                        :RedondeoValidez,
                        :Agrupacion,
                        :ManejoIdentificador,
                        :TipoDisplay,
                        :Anexo1,
                        :Anexo2,
                        :Anexo3,
                        :Anexo4,
                        null,
                        :DT_ADDROW,
                        :Altura,
                        :Ancho,
                        :Profundidad,
                        :TipoManejoFecha,
                        :AvisoAjusteInventario,
                        :AyudaColector,
                        :SubBulto,
                        :Exclusivo,
                        :UnidadDistribucion,
                        :DiasLiberacion,
                        :UnidadBulto,
                        :ManejaTomaDato,
                        :Anexo5,
                        :GrupoConsulta,
                        :DescripcionDisplay,
                        :PrecioSegDistribucion,
                        :PrecioSegStock,
                        :PrecioDistribucion,
                        :PrecioEgreso,
                        :PrecioIngreso,
                        :PrecioStock,
                        :UndMedidaFact,
                        :ProductoUnico,
                        :Ramo,
                        :AceptaDecimales,
                        :Componente1,
                        :Componente2,
                        :ModalidadIngresoLote,
                        :CantidadPadronStock,
                        :CantidadGenerica,
                        :CodigoBase,
                        :Talle,
                        :Color,
                        :Temporada,
                        :Categoria1,
                        :Categoria2,
                        :Transaccion,
                        :VentanaLiberacion)";

            await _dapper.ExecuteAsync(connection, sql, productos, transaction: tran);

            var sqlCB = @"INSERT INTO T_CODIGO_BARRAS (
                            CD_BARRAS,
                            CD_EMPRESA,
                            CD_PRODUTO,
                            TP_CODIGO_BARRAS,
                            DT_UPDROW,
                            DT_ADDROW,
                            QT_EMBALAGEM,
                            NU_PRIORIDADE_USO,
                            NU_TRANSACCION,
                            NU_TRANSACCION_DELETE
                            ) VALUES 
                            (
                                'W' || :Codigo,
                                :CodigoEmpresa,
                                :Codigo,
                                1,
                                SYSDATE,
                                SYSDATE,
                                null,
                                0,
                                null,
                                null
                            )";

            await _dapper.ExecuteAsync(connection, sqlCB, productos, transaction: tran);
        }

        public virtual object GetRamoEntity(short ramo)
        {
            return new
            {
                Codigo = ramo,
                Descripcion = ramo,
                Alta = DateTime.Now
            };
        }

        public virtual object GetFamiliaEntity(int codigoFamilia)
        {
            return new
            {
                Codigo = codigoFamilia,
                Descripcion = codigoFamilia,
                Alta = DateTime.Now
            };
        }

        public virtual object GetClaseEntity(string codigoClase)
        {
            return new
            {
                Codigo = codigoClase,
                Descripcion = codigoClase,
                Alta = DateTime.Now
            };
        }
        public virtual object GetVentanaLiberacionEntity(string valor)
        {
            return new
            {
                Dominio = CodigoDominioDb.VentanaLiberacion + "_" + valor,
                Codigo = CodigoDominioDb.VentanaLiberacion,
                Valor = valor,
            };
        }
        public virtual object GetFaixaEntity(Producto producto)
        {
            return new
            {
                CodigoEmpresa = producto.CodigoEmpresa,
                Codigo = producto.Codigo,
                CodigoFaixa = 1,
                Updrow = DateTime.Now,
                Addrow = DateTime.Now,
                CdEmbalagemFaixa = producto.UnidadMedida,
                VL_ALTURA = 0,
                VL_LARGURA = 0,
                VL_PROFUNDIDADE = 0
            };
        }

        public virtual object GetProductoEntity(Producto producto)
        {
            return new
            {
                CodigoEmpresa = producto.CodigoEmpresa,
                Codigo = producto.Codigo,
                CodigoProductoEmpresa = producto.CodigoProductoEmpresa,
                NAM = _mapper.NullIfEmpty(producto.NAM),
                CodigoMercadologico = producto.CodigoMercadologico,
                SgProducto = producto.SgProducto,
                TipoPeso = producto.TipoPeso,
                DescDifPeso = producto.DescDifPeso,
                Descripcion = producto.Descripcion,
                UnidadMedida = producto.UnidadMedida,
                CodigoFamilia = producto.CodigoFamilia,
                CodigoRotatividad = producto.CodigoRotatividad,
                CodigoClase = producto.CodigoClase,
                StockMinimo = producto.StockMinimo,
                StockMaximo = producto.StockMaximo,
                PesoNeto = producto.PesoNeto,
                PesoBruto = producto.PesoBruto,
                Conversion = producto.Conversion,
                VolumenCC = producto.VolumenCC,
                PrecioVenta = producto.PrecioVenta,
                UltimoCosto = producto.UltimoCosto,
                CodOrigen = producto.CdOrigen,
                DescripcionReducida = producto.DescripcionReducida,
                UndEmb = producto.UndEmb,
                Nivel = producto.Nivel,
                Situacion = producto.Situacion,
                FechaSituacion = producto.FechaSituacion,
                DiasValidez = producto.DiasValidez,
                DiasDuracion = producto.DiasDuracion,
                IdCrossDocking = producto.IdCrossDocking,
                RedondeoValidez = producto.RedondeoValidez,
                Agrupacion = producto.Agrupacion,
                ManejoIdentificador = producto.ManejoIdentificadorId,
                TipoDisplay = producto.TipoDisplay,
                Anexo1 = producto.Anexo1,
                Anexo2 = producto.Anexo2,
                Anexo3 = producto.Anexo3,
                Anexo4 = producto.Anexo4,
                DT_ADDROW = producto.FechaIngreso ?? DateTime.Now,
                Modificacion = producto.FechaModificacion ?? DateTime.Now,
                Altura = producto.Altura,
                Ancho = producto.Ancho,
                Profundidad = producto.Profundidad,
                TipoManejoFecha = producto.TipoManejoFecha,
                AvisoAjusteInventario = producto.AvisoAjusteInventario,
                AyudaColector = producto.AyudaColector,
                SubBulto = producto.SubBulto,
                Exclusivo = producto.Exclusivo,
                UnidadDistribucion = producto.UnidadDistribucion,
                DiasLiberacion = producto.DiasLiberacion,
                UnidadBulto = producto.UnidadBulto,
                ManejaTomaDato = producto.ManejaTomaDato,
                Anexo5 = producto.Anexo5,
                GrupoConsulta = _mapper.NullIfEmpty(producto.GrupoConsulta),
                DescripcionDisplay = producto.DescripcionDisplay,
                PrecioSegDistribucion = producto.PrecioSegDistribucion,
                PrecioSegStock = producto.PrecioSegStock,
                PrecioDistribucion = producto.PrecioDistribucion,
                PrecioEgreso = producto.PrecioEgreso,
                PrecioIngreso = producto.PrecioIngreso,
                PrecioStock = producto.PrecioStock,
                UndMedidaFact = producto.UndMedidaFact,
                ProductoUnico = producto.ProductoUnico,
                Ramo = producto.Ramo,
                AceptaDecimales = producto.AceptaDecimalesId,
                Componente1 = _mapper.NullIfEmpty(producto.Componente1)?.ToUpper(),
                Componente2 = _mapper.NullIfEmpty(producto.Componente2)?.ToUpper(),
                ModalidadIngresoLote = producto.ModalidadIngresoLote,
                CantidadPadronStock = producto.CantidadPadronStock,
                CantidadGenerica = producto.CantidadGenerica,
                CodigoBase = producto.CodigoBase,
                Talle = producto.Talle,
                Color = producto.Color,
                Temporada = producto.Temporada,
                Categoria1 = producto.Categoria1,
                Categoria2 = producto.Categoria2,
                Transaccion = producto.NumeroTransaccion,
                VentanaLiberacion = producto.VentanaLiberacion
            };
        }

        public static string GetSqlSelectProducto()
        {
            return @"SELECT 
                    P.CD_CLASSE as CodigoClase,
                    P.CD_EMPRESA as CodigoEmpresa,
                    P.CD_EXCLUSIVO as Exclusivo,
                    P.CD_FAMILIA_PRODUTO as CodigoFamilia,
                    P.CD_GRUPO_CONSULTA as GrupoConsulta,
                    P.CD_MERCADOLOGICO as CodigoMercadologico,
                    P.CD_NAM as NAM,
                    P.CD_NIVEL as Nivel,
                    P.CD_ORIGEM as CdOrigen,
                    P.CD_PRODUTO as Codigo,
                    P.CD_PRODUTO_EMPRESA as CodigoProductoEmpresa,
                    P.CD_PRODUTO_UNICO as ProductoUnico,
                    P.CD_RAMO_PRODUTO as Ramo,
                    P.CD_ROTATIVIDADE as CodigoRotatividad,
                    P.CD_SITUACAO as Situacion,
                    P.CD_UND_MEDIDA_FACT as UndMedidaFact,
                    P.CD_UNIDADE_MEDIDA as UnidadMedida,
                    P.CD_UNID_EMB as UndEmb,
                    P.DS_ANEXO1 as Anexo1,
                    P.DS_ANEXO2 as Anexo2,
                    P.DS_ANEXO3 as Anexo3,
                    P.DS_ANEXO4 as Anexo4,
                    P.DS_ANEXO5 as Anexo5,
                    P.DS_DIFER_PESO_QTDE as DescDifPeso,
                    P.DS_DISPLAY as DescripcionDisplay,
                    P.DS_HELP_COLECTOR as AyudaColector,
                    P.DS_PRODUTO as Descripcion,
                    P.DS_REDUZIDA as DescripcionReducida,
                    P.DT_ADDROW as FechaIngreso,
                    P.DT_SITUACAO as FechaSituacion,
                    P.DT_UPDROW as FechaModificacion,
                    P.FL_ACEPTA_DECIMALES as AceptaDecimalesId,
                    P.FT_CONVERSAO as Conversion,
                    P.ID_AGRUPACION as Agrupacion,
                    P.ID_CROSS_DOCKING as IdCrossDocking,
                    P.ID_MANEJA_TOMA_DATO as ManejaTomaDato,
                    P.ID_MANEJO_IDENTIFICADOR as ManejoIdentificadorId,
                    P.ID_REDONDEO_VALIDEZ as RedondeoValidez,
                    P.ND_FACTURACION_COMP1 as Componente1,
                    P.ND_FACTURACION_COMP2 as Componente2,
                    P.ND_MODALIDAD_INGRESO_LOTE as ModalidadIngresoLote,
                    P.PS_BRUTO as PesoBruto,
                    P.PS_LIQUIDO as PesoNeto,
                    P.QT_DIAS_DURACAO as DiasDuracion,
                    P.QT_DIAS_VALIDADE as DiasValidez,
                    P.QT_DIAS_VALIDADE_LIBERACION as DiasLiberacion,
                    P.QT_ESTOQUE_MAXIMO as StockMaximo,
                    P.QT_ESTOQUE_MINIMO as StockMinimo,
                    P.QT_GENERICO as CantidadGenerica,
                    P.QT_PADRON_STOCK as CantidadPadronStock,
                    P.QT_SUBBULTO as SubBulto,
                    P.QT_UND_BULTO as UnidadBulto,
                    P.QT_UND_DISTRIBUCION as UnidadDistribucion,
                    P.SG_PRODUTO as SgProducto,
                    P.TP_DISPLAY as TipoDisplay,
                    P.TP_MANEJO_FECHA as TipoManejoFecha,
                    P.TP_PESO_PRODUTO as TipoPeso,
                    P.VL_ALTURA as Altura,
                    P.VL_AVISO_AJUSTE as AvisoAjusteInventario,
                    P.VL_CUBAGEM as VolumenCC,
                    P.VL_CUSTO_ULT_ENT as UltimoCosto,
                    P.VL_LARGURA as Ancho,
                    P.VL_PRECIO_DISTRIB as PrecioDistribucion,
                    P.VL_PRECIO_EGRESO as PrecioEgreso,
                    P.VL_PRECIO_INGRESO as PrecioIngreso,
                    P.VL_PRECIO_SEG_DISTR as PrecioSegDistribucion,
                    P.VL_PRECIO_SEG_STOCK as PrecioSegStock,
                    P.VL_PRECIO_STOCK as PrecioStock,
                    P.VL_PRECO_VENDA as PrecioVenta,
                    P.VL_PROFUNDIDADE as Profundidad,
                    P.CODIGO_BASE as CodigoBase,
                    P.TALLE as Talle,
                    P.COLOR as Color,
                    P.TEMPORADA as Temporada,
                    P.VL_CATEGORIA_01 as Categoria1,
                    P.VL_CATEGORIA_02 as Categoria2,
                    P.CD_VENTANA_LIBERACION as VentanaLiberacion
                    FROM T_PRODUTO P ";
        }

        public virtual async Task<Producto> GetProductoOrNull(int empresa, string codigo)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync();

                Producto model = GetProducto(connection, new Producto()
                {
                    CodigoEmpresa = empresa,
                    Codigo = codigo
                });

                if (model != null)
                {
                    model.ManejoIdentificador = MapManejoIdentificador(model.ManejoIdentificadorId);
                }

                return model;
            }
        }

        public virtual ManejoIdentificador MapManejoIdentificador(string manejoIdentificadorId)
        {
            switch (manejoIdentificadorId)
            {
                case ManejoIdentificadorDb.Lote: return ManejoIdentificador.Lote;
                case ManejoIdentificadorDb.Serie: return ManejoIdentificador.Serie;
                case ManejoIdentificadorDb.Producto:
                default:
                    return ManejoIdentificador.Producto;
            }
        }

        public virtual Producto GetProducto(DbConnection connection, Producto model)
        {
            string sql = GetSqlSelectProducto() +
                @"WHERE P.CD_PRODUTO = :codigo AND P.CD_EMPRESA = :empresa";

            var query = _dapper.Query<Producto>(connection, sql, param: new
            {
                codigo = model.Codigo,
                empresa = model.CodigoEmpresa
            }, commandType: CommandType.Text);

            return query.FirstOrDefault();
        }

        public virtual Producto Map(Producto request, Producto model = null)
        {
            Producto producto = new Producto();

            producto.Situacion = request.Situacion;
            producto.AceptaDecimales = request.AceptaDecimales;
            producto.AceptaDecimalesId = request.AceptaDecimalesId;
            producto.Agrupacion = request.Agrupacion ?? model?.Agrupacion;
            producto.Altura = request.Altura ?? model?.Altura;
            producto.Ancho = request.Ancho ?? model?.Ancho;
            producto.Anexo1 = request.Anexo1 ?? model?.Anexo1;
            producto.Anexo2 = request.Anexo2 ?? model?.Anexo2;
            producto.Anexo3 = request.Anexo3 ?? model?.Anexo3;
            producto.Anexo4 = request.Anexo4 ?? model?.Anexo4;
            producto.Anexo5 = request.Anexo5 ?? model?.Anexo5;
            producto.AvisoAjusteInventario = request.AvisoAjusteInventario ?? model?.AvisoAjusteInventario;
            producto.AyudaColector = request.AyudaColector ?? model?.AyudaColector;
            producto.CantidadGenerica = request.CantidadGenerica ?? model?.CantidadGenerica;
            producto.CantidadPadronStock = request.CantidadPadronStock ?? model?.CantidadPadronStock;
            producto.CdOrigen = request.CdOrigen ?? model?.CdOrigen;
            producto.Clase = request.Clase ?? model?.Clase;
            producto.Codigo = request.Codigo;
            producto.CodigoClase = request.CodigoClase ?? model?.CodigoClase;
            producto.CodigoEmpresa = request.CodigoEmpresa;
            producto.CodigoFamilia = request.CodigoFamilia;
            producto.CodigoMercadologico = request.CodigoMercadologico ?? model?.CodigoMercadologico;
            producto.CodigoProductoEmpresa = request.CodigoProductoEmpresa ?? model?.CodigoProductoEmpresa;
            producto.CodigoRotatividad = request.CodigoRotatividad ?? model?.CodigoRotatividad;
            producto.Componente1 = request.Componente1 ?? model?.Componente1;
            producto.Componente2 = request.Componente2 ?? model?.Componente2;
            producto.Conversion = request.Conversion ?? model?.Conversion;
            producto.DescDifPeso = request.DescDifPeso ?? model?.DescDifPeso;
            producto.Descripcion = request.Descripcion ?? model?.Descripcion;
            producto.DescripcionDisplay = request.DescripcionDisplay ?? model?.DescripcionDisplay;
            producto.DescripcionReducida = request.DescripcionReducida ?? model?.DescripcionReducida;
            producto.DiasDuracion = request.DiasDuracion ?? model?.DiasDuracion;
            producto.DiasLiberacion = request.DiasLiberacion ?? model?.DiasLiberacion;
            producto.DiasValidez = request.DiasValidez ?? model?.DiasValidez;
            producto.Empresa = request.Empresa ?? model?.Empresa;
            producto.Exclusivo = request.Exclusivo ?? model?.Exclusivo;
            producto.Familia = request.Familia ?? model?.Familia;
            producto.FechaIngreso = request.FechaIngreso ?? model?.FechaIngreso;
            producto.FechaModificacion = request.FechaModificacion ?? model?.FechaModificacion;
            producto.FechaSituacion = request.FechaSituacion ?? model?.FechaSituacion;
            producto.GrupoConsulta = request.GrupoConsulta ?? model?.GrupoConsulta;
            producto.IdCrossDocking = request.IdCrossDocking ?? model?.IdCrossDocking;
            producto.ManejaTomaDato = request.ManejaTomaDato ?? model?.ManejaTomaDato;
            producto.ManejoIdentificador = request.ManejoIdentificador;
            producto.ManejoIdentificadorId = request.ManejoIdentificadorId ?? model?.ManejoIdentificadorId;
            producto.ModalidadIngresoLote = request.ModalidadIngresoLote ?? model?.ModalidadIngresoLote;
            producto.NAM = request.NAM ?? model?.NAM;
            producto.Nivel = request.Nivel ?? model?.Nivel;
            producto.PesoBruto = request.PesoBruto ?? model?.PesoBruto;
            producto.PesoNeto = request.PesoNeto ?? model?.PesoNeto;
            producto.PrecioDistribucion = request.PrecioDistribucion ?? model?.PrecioDistribucion;
            producto.PrecioEgreso = request.PrecioEgreso ?? model?.PrecioEgreso;
            producto.PrecioIngreso = request.PrecioIngreso ?? model?.PrecioIngreso;
            producto.PrecioSegDistribucion = request.PrecioSegDistribucion ?? model?.PrecioSegDistribucion;
            producto.PrecioSegStock = request.PrecioSegStock ?? model?.PrecioSegStock;
            producto.PrecioStock = request.PrecioStock ?? model?.PrecioStock;
            producto.PrecioVenta = request.PrecioVenta ?? model?.PrecioVenta;
            producto.ProductoUnico = request.ProductoUnico ?? model?.ProductoUnico;
            producto.Profundidad = request.Profundidad ?? model?.Profundidad;
            producto.Ramo = request.Ramo;
            producto.RedondeoValidez = request.RedondeoValidez ?? model?.RedondeoValidez;
            producto.Rotatividad = request.Rotatividad ?? model?.Rotatividad;
            producto.SgProducto = request.SgProducto ?? model?.SgProducto;
            producto.StockMaximo = request.StockMaximo ?? model?.StockMaximo;
            producto.StockMinimo = request.StockMinimo ?? model?.StockMinimo;
            producto.SubBulto = request.SubBulto ?? model?.SubBulto;
            producto.TipoDisplay = request.TipoDisplay ?? model?.TipoDisplay;
            producto.TipoPeso = request.TipoPeso ?? model?.TipoPeso;
            producto.UltimoCosto = request.UltimoCosto ?? model?.UltimoCosto;
            producto.UndEmb = request.UndEmb ?? model?.UndEmb;
            producto.UndMedidaFact = request.UndMedidaFact ?? model?.UndMedidaFact;
            producto.UnidadBulto = request.UnidadBulto;
            producto.UnidadDistribucion = request.UnidadDistribucion;
            producto.UnidadMedida = request.UnidadMedida;
            producto.VolumenCC = request.VolumenCC ?? model?.VolumenCC;
            producto.TipoManejoFecha = request.TipoManejoFecha;
            producto.CodigoBase = request.CodigoBase;
            producto.Talle = request.Talle;
            producto.Color = request.Color;
            producto.Temporada = request.Temporada;
            producto.Categoria1 = request.Categoria1;
            producto.Categoria2 = request.Categoria2;
            producto.NumeroTransaccion = request.NumeroTransaccion ?? model?.NumeroTransaccion;
            producto.VentanaLiberacion = request.VentanaLiberacion;

            return producto;
        }

        #endregion

        #region Producto proveedor

        public virtual Dictionary<string, string> GetCodigosExternos(List<ProductoProveedor> productosProveedor)
        {
            Dictionary<string, string> resultado = new Dictionary<string, string>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRODUTO_CONVERTOR_TEMP (CD_EXTERNO, CD_CLIENTE, CD_EMPRESA) VALUES (:CodigoExterno, :Cliente, :Empresa)";
                    _dapper.Execute(connection, sql, productosProveedor, transaction: tran);

                    sql = GetSqlSelectProductoProveedor() +
                        @"INNER JOIN T_PRODUTO_CONVERTOR_TEMP T ON PP.CD_EXTERNO = T.CD_EXTERNO 
                                AND PP.CD_CLIENTE = T.CD_CLIENTE 
                                AND PP.CD_EMPRESA = T.CD_EMPRESA";

                    foreach (var pp in _dapper.Query<ProductoProveedor>(connection, sql, transaction: tran))
                    {
                        var key = $"{pp.CodigoExterno}.{pp.Cliente}.{pp.Empresa}";
                        resultado[key] = pp.CodigoProducto;
                    }

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<ProductoProveedor> GetProductosProveedor(IEnumerable<ProductoProveedor> productosProveedor)
        {
            IEnumerable<ProductoProveedor> resultado = new List<ProductoProveedor>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRODUTO_CONVERTOR_TEMP (CD_PRODUTO, CD_CLIENTE, CD_EMPRESA) VALUES (:CodigoProducto, :Cliente, :Empresa)";
                    _dapper.Execute(connection, sql, productosProveedor, transaction: tran);

                    sql = GetSqlSelectProductoProveedor() +
                        @"INNER JOIN T_PRODUTO_CONVERTOR_TEMP T ON PP.CD_PRODUTO = T.CD_PRODUTO 
                            AND PP.CD_CLIENTE = T.CD_CLIENTE 
                            AND PP.CD_EMPRESA = T.CD_EMPRESA";

                    resultado = _dapper.Query<ProductoProveedor>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task<ProductoProveedor> GetProductoProveedorOrNull(string producto, int empresa, string tipoAgente, string codigoAgente, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var agente = new AgenteRepository(_context, _cdAplicacion, _userId, _dapper).GetAgenteOrNull(empresa, codigoAgente, tipoAgente).Result;

                if (agente == null)
                    return null;

                return GetProductoProveedor(producto, empresa, agente.CodigoInterno, connection);
            }
        }

        public virtual ProductoProveedor GetProductoProveedor(string producto, int empresa, string cliente, DbConnection connection)
        {
            string sql = GetSqlSelectProductoProveedor() +
                @"WHERE PP.CD_PRODUTO = :codigo AND PP.CD_EMPRESA = :empresa AND PP.CD_CLIENTE = :cliente";

            var query = _dapper.Query<ProductoProveedor>(connection, sql, param: new
            {
                codigo = producto,
                empresa = empresa,
                cliente = cliente
            }, commandType: CommandType.Text);

            return query.FirstOrDefault();
        }

        public static string GetSqlSelectProductoProveedor()
        {
            return @"SELECT 
                    PP.CD_CLIENTE as Cliente,
                    PP.CD_EMPRESA as Empresa,
                    PP.CD_EXTERNO as CodigoExterno,
                    PP.CD_PRODUTO as CodigoProducto
                FROM T_PRODUTO_CONVERTOR PP ";
        }

        public virtual async Task AddProductosProveedor(List<ProductoProveedor> productos, IProductoProveedorServiceContext context, CancellationToken cancelToken = default)
        {
            await AddProductosProveedor(GetBulkOperationContext(productos, context), cancelToken);
        }

        public virtual async Task AddProductosProveedor(ProductoProveedorBulkOperationContext context, CancellationToken cancelToken)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertProductosProveedor(connection, tran, context.NewProductos);
                    await BulkDeleteProductosProveedor(connection, tran, context.DelProductos);

                    tran.Commit();
                }
            }
        }

        public virtual ProductoProveedorBulkOperationContext GetBulkOperationContext(List<ProductoProveedor> productos, IProductoProveedorServiceContext serviceContext)
        {
            var context = new ProductoProveedorBulkOperationContext();
            var newProductos = new Dictionary<string, ProductoProveedor>();

            foreach (var producto in productos)
            {
                var key = $"{producto.CodigoProducto}.{producto.Cliente}.{producto.Empresa}";
                newProductos[key] = producto;
            }

            foreach (var model in serviceContext.ProductosProveedor.Values)
            {
                var key = $"{model.CodigoProducto}.{model.Cliente}.{model.Empresa}";
                var producto = newProductos[key];
                newProductos.Remove(key);
                context.DelProductos.Add(GetProductoProveedorEntity(producto));
            }

            foreach (var producto in newProductos.Values)
            {
                context.NewProductos.Add(GetProductoProveedorEntity(producto));
            }

            return context;
        }

        public virtual object GetProductoProveedorEntity(ProductoProveedor producto)
        {
            return new
            {
                producto = producto.CodigoProducto,
                empresa = producto.Empresa,
                cliente = producto.Cliente,
                externo = producto.CodigoExterno,
                fechaIngreso = DateTime.Now
            };
        }

        public virtual async Task BulkInsertProductosProveedor(DbConnection connection, DbTransaction tran, List<object> productos)
        {
            string sql = @"INSERT INTO T_PRODUTO_CONVERTOR (CD_PRODUTO, CD_EMPRESA, CD_CLIENTE, CD_EXTERNO, DT_ADDROW) 
                VALUES(:producto, :empresa, :cliente, :externo, :fechaIngreso)";
            await _dapper.ExecuteAsync(connection, sql, productos, transaction: tran);
        }

        public virtual async Task BulkDeleteProductosProveedor(DbConnection connection, DbTransaction tran, List<object> productos)
        {
            string sql = @"DELETE FROM T_PRODUTO_CONVERTOR 
                WHERE CD_PRODUTO = :producto AND CD_EMPRESA = :empresa AND CD_CLIENTE = :cliente";
            await _dapper.ExecuteAsync(connection, sql, productos, transaction: tran);
        }

        #endregion

        #endregion
    }
}
