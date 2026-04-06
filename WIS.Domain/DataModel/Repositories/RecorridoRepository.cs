using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Expedicion;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Recorridos;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class RecorridoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly RecorridoMapper _mapper;
        protected readonly AplicacionMapper _aplicacionMapper;
        protected readonly AplicacionRecorridoMapper _aplicacionRecorridoMapper;
        protected readonly AplicacionRecorridoUsuarioMapper _aplicacionRecorridoUsuarioMapper;

        public RecorridoRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new RecorridoMapper();
            this._aplicacionMapper = new AplicacionMapper();
            this._aplicacionRecorridoMapper = new AplicacionRecorridoMapper();
            this._aplicacionRecorridoUsuarioMapper = new AplicacionRecorridoUsuarioMapper();
        }

        #region ADD
        public virtual DetalleRecorrido AddDetalleRecorrido(DetalleRecorrido detalleRecorrido)
        {
            detalleRecorrido.Id = _context.GetNextSequenceValueLong(_dapper, Secuencias.S_RECORRIDO_DET);
            detalleRecorrido.FechaAlta = DateTime.Now;

            _context.T_RECORRIDO_DET.Add(_mapper.MapToEntity(detalleRecorrido));

            return detalleRecorrido;
        }

        public virtual Recorrido AddRecorrido(Recorrido recorrido)
        {
            recorrido.Id = _context.GetNextSequenceValueInt(_dapper, Secuencias.S_RECORRIDO);
            recorrido.FechaAlta = DateTime.Now;

            _context.T_RECORRIDO.Add(_mapper.MapToEntity(recorrido));

            return recorrido;
        }

        public virtual void ImportarDetallesRecorrido(IRecorridoServiceContext serviceContext)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            if (serviceContext.RegistrosBaja.Count > 0)
            {
                _dapper.BulkUpdate(connection, tran, serviceContext.RegistrosBaja, "T_RECORRIDO_DET", new Dictionary<string, Func<DetalleRecorrido, ColumnInfo>>
                {
                    { "NU_TRANSACCION", x => new ColumnInfo(_context.GetTransactionNumber())},
                    { "NU_TRANSACCION_DELETE", x => new ColumnInfo(_context.GetTransactionNumber())},
                    { "DT_UPDROW", x => new ColumnInfo(DateTime.Now)}
                }, new Dictionary<string, Func<DetalleRecorrido, ColumnInfo>>
                {
                    { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                    { "NU_RECORRIDO", x => new ColumnInfo(x.IdRecorrido)}
                });

                _context.SaveChanges();

                _dapper.BulkDelete(connection, tran, serviceContext.RegistrosBaja, "T_RECORRIDO_DET", new Dictionary<string, Func<DetalleRecorrido, object>>
                {
                    { "NU_RECORRIDO", x => x.IdRecorrido},
                    { "CD_ENDERECO", x => x.Ubicacion}
                });
            }

            if (serviceContext.RegistrosAlta.Count > 0)
            {
                _dapper.BulkInsert(connection, tran, serviceContext.RegistrosAlta, "T_RECORRIDO_DET", new Dictionary<string, Func<DetalleRecorrido, ColumnInfo>>
                {
                    { "NU_RECORRIDO_DET", x => new ColumnInfo(_context.GetNextSequenceValueLong(_dapper, Secuencias.S_RECORRIDO_DET))},
                    { "NU_RECORRIDO", x => new ColumnInfo(x.IdRecorrido)},
                    { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                    { "NU_ORDEN", x => new ColumnInfo(x.NumeroOrden)},
                    { "VL_ORDEN", x => new ColumnInfo(x.ValorOrden)},
                    { "DT_ADDROW", x => new ColumnInfo(DateTime.Now)},
                    { "DT_UPDROW", x => new ColumnInfo(DateTime.Now)},
                    { "NU_TRANSACCION", x => new ColumnInfo(_context.GetTransactionNumber())}
                });
            }
        }

        public virtual void AsociarRecorridoPorDefectoAplicaciones(Recorrido recorrido)
        {
            var aplicaciones = GetAplicacionesManejanRecorrido();

            foreach (var a in aplicaciones)
            {
                var aplicacionRecorrido = new AplicacionRecorrido
                {
                    IdAplicacion = a.Codigo,
                    IdRecorrido = recorrido.Id,
                    EsPredeterminado = true,
                    NuTransaccion = _context.GetTransactionNumber(),
                    FechaAlta = DateTime.Now
                };

                _context.T_APLICACION_RECORRIDO.Add(_aplicacionRecorridoMapper.MapToEntity(aplicacionRecorrido));
            }
        }

        public virtual void AddAplicacionesRecorrido(AplicacionRecorrido detalleRecorrido)
        {
            _context.T_APLICACION_RECORRIDO.Add(_aplicacionRecorridoMapper.MapToEntity(detalleRecorrido));
        }

        #endregion

        #region UPDATE

        public virtual void UpdateRecorrido(Recorrido recorrido)
        {
            recorrido.FechaModificacion = DateTime.Now;

            var entity = _mapper.MapToEntity(recorrido);

            var attachedEntity = this._context.T_RECORRIDO.Local.FirstOrDefault(x => x.NU_RECORRIDO == recorrido.Id);

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_RECORRIDO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateRecorridoAsociado(AplicacionRecorrido recorridoAsociado)
        {
            var detailEntity = _aplicacionRecorridoMapper.MapToEntity(recorridoAsociado);

            var attachedEntity = this._context.T_APLICACION_RECORRIDO.Local
                .FirstOrDefault(x => x.NU_RECORRIDO == recorridoAsociado.IdRecorrido
                    && x.CD_APLICACION == recorridoAsociado.IdAplicacion);

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_APLICACION_RECORRIDO.Attach(detailEntity);
                this._context.Entry(detailEntity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateAplicacionRecorridoUsuario(AplicacionRecorridoUsuario recorridoAplicacionUsuario)
        {
            var detailEntity = _aplicacionRecorridoUsuarioMapper.MapToEntity(recorridoAplicacionUsuario);

            var attachedEntity = this._context.T_APLICACION_RECORRIDO_USUARIO.Local
                .FirstOrDefault(x => x.NU_RECORRIDO == recorridoAplicacionUsuario.IdRecorrido
                    && x.CD_APLICACION == recorridoAplicacionUsuario.IdAplicacion
                    && x.USERID == recorridoAplicacionUsuario.UserId);

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(detailEntity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_APLICACION_RECORRIDO_USUARIO.Attach(detailEntity);
                this._context.Entry(detailEntity).State = EntityState.Modified;
            }
        }
        #endregion

        #region DELETE

        public virtual void EliminarUbicacionDeRecorridos(Ubicacion ubicacion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var detalles = GetAllDetallesRecorridosByUbicacion(ubicacion.Id);

            _dapper.BulkUpdate(connection, tran, detalles, "T_RECORRIDO_DET", new Dictionary<string, Func<DetalleRecorrido, ColumnInfo>>
            {
                { "NU_TRANSACCION", x => new ColumnInfo(_context.GetTransactionNumber())},
                { "NU_TRANSACCION_DELETE", x => new ColumnInfo(_context.GetTransactionNumber())},
                { "DT_UPDROW", x => new ColumnInfo(DateTime.Now)}
            }, new Dictionary<string, Func<DetalleRecorrido, ColumnInfo>>
            {
                { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)}
            });

            _context.SaveChanges();

            var codigoUbicacion = ubicacion.Id;

            var sql = @"DELETE FROM T_RECORRIDO_DET WHERE CD_ENDERECO = :cdEndereco";
            _dapper.Execute(connection, sql, param: new { cdEndereco = codigoUbicacion }, transaction: tran);
        }

        public virtual IEnumerable<DetalleRecorrido> GetAllDetallesRecorridosByUbicacion(string idUbicacion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var sql = "SELECT CD_ENDERECO Ubicacion, NU_RECORRIDO IdRecorrido FROM T_RECORRIDO_DET WHERE CD_ENDERECO = :ubic GROUP BY CD_ENDERECO, NU_RECORRIDO";

            var parameter = new DynamicParameters(new
            {
                ubic = idUbicacion
            });

            return _dapper.Query<DetalleRecorrido>(connection, sql, param: parameter, commandType: CommandType.Text, transaction: tran).ToList();
        }
        public virtual IEnumerable<DetalleRecorrido> GetAllDetallesRecorridosByIdRecorrido(int idRecorrido)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var sql = @"SELECT 
                            NU_RECORRIDO_DET as Id,
                            NU_RECORRIDO as IdRecorrido,
                            CD_ENDERECO as Ubicacion,
                            NU_ORDEN as NuOrden,
                            NU_TRANSACCION as Transaccion,
                            VL_ORDEN as ValorOrden,
                            NU_TRANSACCION_DELETE as TransaccionDelete,
                            DT_ADDROW as FechaAlta,
                            DT_UPDROW as FechaModificacion
                        FROM T_RECORRIDO_DET 
                        WHERE NU_RECORRIDO = :idRecorrido ";

            return _dapper.Query<DetalleRecorrido>(connection, sql, param: new { idRecorrido = idRecorrido }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual void EliminarAplicacionAsociada(AplicacionRecorrido recorridoAplicacion)
        {
            var entity = this._aplicacionRecorridoMapper.MapToEntity(recorridoAplicacion);
            var attachedEntity = this._context.T_APLICACION_RECORRIDO.Local
                .FirstOrDefault(d => d.NU_RECORRIDO == recorridoAplicacion.IdRecorrido && d.CD_APLICACION == recorridoAplicacion.IdAplicacion);

            if (attachedEntity != null)
            {
                this._context.T_APLICACION_RECORRIDO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_APLICACION_RECORRIDO.Attach(entity);
                this._context.T_APLICACION_RECORRIDO.Remove(entity);
            }
        }

        #endregion

        #region GET
        public virtual Recorrido GetRecorridoPorDefectoParaPredio(string numeroPredio)
        {
            var entity = _context.T_RECORRIDO.AsNoTracking().FirstOrDefault(i => i.NU_PREDIO == numeroPredio && i.FL_DEFAULT == "S");

            return _mapper.MapToObject(entity);
        }

        public virtual Recorrido GetRecorridoById(int nuRecorrido)
        {
            var entity = _context.T_RECORRIDO.AsNoTracking().FirstOrDefault(i => i.NU_RECORRIDO == nuRecorrido);

            return _mapper.MapToObject(entity);
        }

        public virtual IEnumerable<DetalleRecorrido> GetDetallesRecorridosExistentes(IEnumerable<DetalleRecorrido> detalles)
        {
            IEnumerable<DetalleRecorrido> resultado = [];

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    _dapper.BulkInsert(connection, tran, detalles, "T_RECORRIDO_DET_TEMP", new Dictionary<string, Func<DetalleRecorrido, ColumnInfo>>
                    {
                        { "NU_RECORRIDO", x => new ColumnInfo(x.IdRecorrido)},
                        { "VL_ORDEN", x => new ColumnInfo(x.ValorOrden)},
                    });

                    var sql = @"SELECT 
                                R.NU_RECORRIDO_DET AS Id,
                                R.NU_RECORRIDO AS IdRecorrido,
                                R.CD_ENDERECO AS Ubicacion,
                                R.NU_ORDEN AS NumeroOrden,
                                R.VL_ORDEN AS ValorOrden
                        FROM T_RECORRIDO_DET R 
                            INNER JOIN T_RECORRIDO_DET_TEMP T ON 
                                R.NU_RECORRIDO = T.NU_RECORRIDO AND
                                R.VL_ORDEN = T.VL_ORDEN";

                    resultado = _dapper.Query<DetalleRecorrido>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<DetalleRecorrido> GetUbicacionesExistentesEnRecorrido(IEnumerable<DetalleRecorrido> detalles)
        {
            IEnumerable<DetalleRecorrido> resultado = [];

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    _dapper.BulkInsert(connection, tran, detalles, "T_RECORRIDO_DET_TEMP", new Dictionary<string, Func<DetalleRecorrido, ColumnInfo>>
                    {
                        { "NU_RECORRIDO", x => new ColumnInfo(x.IdRecorrido)},
                        { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                    });

                    var sql = @"SELECT 
                                R.NU_RECORRIDO_DET AS Id,
                                R.NU_RECORRIDO AS IdRecorrido,
                                R.CD_ENDERECO AS Ubicacion,
                                R.NU_ORDEN AS NumeroOrden,
                                R.VL_ORDEN AS ValorOrden
                        FROM T_RECORRIDO_DET R 
                            INNER JOIN T_RECORRIDO_DET_TEMP T ON 
                                R.NU_RECORRIDO = T.NU_RECORRIDO AND
                                R.CD_ENDERECO = T.CD_ENDERECO";

                    resultado = _dapper.Query<DetalleRecorrido>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual List<Aplicacion> GetAplicacionesManejanRecorrido()
        {
            var aplicaciones = _context.T_APLICACION
                .AsNoTracking()
                .Where(a => a.FL_RECORRIDO == "S")
                .Select(_aplicacionMapper.MapToObject)
                .ToList();

            return aplicaciones;
        }

        public virtual int GetRecorridoPorDefectoPredio(string predio)
        {
            return _context.T_RECORRIDO.FirstOrDefault(x => x.NU_PREDIO == predio && x.FL_DEFAULT == "S").NU_RECORRIDO;
        }

        public virtual AplicacionRecorrido GetAplicacionAsociada(int recorrido, string aplicacion)
        {
            return _aplicacionRecorridoMapper.MapToObject(_context.T_APLICACION_RECORRIDO.AsNoTracking().FirstOrDefault(x => x.NU_RECORRIDO == recorrido && x.CD_APLICACION == aplicacion));
        }

        public virtual AplicacionRecorrido GetAplicacionPredeterminado(string aplicacion, string predio)
        {
            return _aplicacionRecorridoMapper.MapToObject(_context.T_APLICACION_RECORRIDO.Join(_context.T_RECORRIDO,
                        ar => ar.NU_RECORRIDO,
                        r => r.NU_RECORRIDO,
                        (ar, r) => new { AplicaionRecorrido = ar, Recorrido = r }
                ).AsNoTracking()
                    .Where(x => x.AplicaionRecorrido.CD_APLICACION == aplicacion &&
                        x.AplicaionRecorrido.FL_PREDETERMINADO == "S" &&
                        x.Recorrido.NU_PREDIO == predio)
                    .Select(x => x.AplicaionRecorrido).FirstOrDefault());
        }

        public virtual AplicacionRecorridoUsuario GetAplicacionUsuarioPredeterminado(string aplicacion, string predio, int userId)
        {
            return _aplicacionRecorridoUsuarioMapper.MapToObject(_context.T_APLICACION_RECORRIDO_USUARIO.Join(_context.T_RECORRIDO,
                        ar => ar.NU_RECORRIDO,
                        r => r.NU_RECORRIDO,
                        (ar, r) => new { AplicaionRecorrido = ar, Recorrido = r }
                ).AsNoTracking()
                    .Where(x => x.AplicaionRecorrido.CD_APLICACION == aplicacion &&
                        x.AplicaionRecorrido.FL_PREDETERMINADO == "S" &&
                        x.AplicaionRecorrido.USERID == userId &&
                        x.Recorrido.NU_PREDIO == predio)
                    .Select(x => x.AplicaionRecorrido).FirstOrDefault());
        }

        public virtual AplicacionRecorridoUsuario GetAplicacionUsuarioRecorrido(int recorrido, string aplicacion, int userId)
        {
            return _aplicacionRecorridoUsuarioMapper.MapToObject(_context.T_APLICACION_RECORRIDO_USUARIO.AsNoTracking().FirstOrDefault(x => x.NU_RECORRIDO == recorrido &&
                x.CD_APLICACION == aplicacion &&
                x.USERID == userId));
        }

        #endregion

        #region ANY

        public virtual bool AnyUbicacionSinAsociarAlRecorrido(int nuRecorrido)
        {
            return _context.V_REG700_UBIC_SIN_RECORRIDOS.Any(i => i.NU_RECORRIDO == nuRecorrido);
        }

        public virtual bool AnyPredeterminadoRecorridoAplicacion(int nuRecorrido)
        {
            return _context.T_APLICACION_RECORRIDO.Any(a => a.NU_RECORRIDO == nuRecorrido && a.FL_PREDETERMINADO == "S");
        }

        public virtual bool AnyPredeterminadoRecorridoAplicacionUsuario(int nuRecorrido)
        {
            return _context.T_APLICACION_RECORRIDO_USUARIO.Any(a => a.NU_RECORRIDO == nuRecorrido && a.FL_PREDETERMINADO == "S");
        }

        public virtual bool AnyAplicacionUsuarioAsociadoPredeterminada(DbConnection connection, DbTransaction tran, List<AplicacionRecorridoUsuario> aplicaciones)
        {
            string sql = @"
                    SELECT 
                        arut.NU_RECORRIDO as Recorrido,
                        arut.CD_APLICACION as Aplicacion,
                        arut.USERID as UserId
                    FROM T_APLICACION_REC_USER_TEMP arut 
                    INNER JOIN T_APLICACION_RECORRIDO_USUARIO aru on aru.CD_APLICACION = arut.CD_APLICACION AND
                        aru.USERID = arut.USERID AND 
                        aru.NU_RECORRIDO = arut.NU_RECORRIDO
                    WHERE aru.FL_PREDETERMINADO = 'S'";

            return _dapper.Query<PedidoAsociarUnidad>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault() != null;
        }

        public virtual bool AnyNoDefaultAplicacionUsuarioAsociado(string aplicacion, int userId, int recorrido, string predio)
        {
            return _context.T_APLICACION_RECORRIDO_USUARIO.
                    Join(_context.T_RECORRIDO,
                    aru => aru.NU_RECORRIDO,
                    r => r.NU_RECORRIDO,
                    (aru, r) => new { AplicacionRecorridoUsuario = aru, Recorrido = r }).Any(i => i.AplicacionRecorridoUsuario.CD_APLICACION == aplicacion && i.AplicacionRecorridoUsuario.USERID == userId && i.AplicacionRecorridoUsuario.NU_RECORRIDO != recorrido && i.Recorrido.NU_PREDIO == predio);
        }

        public virtual bool AnyAplicacionRecorridoAsociado(AplicacionRecorrido recorridoAsociado)
        {
            return _context.T_APLICACION_RECORRIDO.Any(i => i.NU_RECORRIDO == recorridoAsociado.IdRecorrido && i.CD_APLICACION == recorridoAsociado.IdAplicacion);
        }

        public virtual bool ExisteNombreRecorrido(string nombre, int? nuRecorrido)
        {
            return _context.T_RECORRIDO
                .Any(r => r.NM_RECORRIDO.ToUpper() == nombre.ToUpper()
                    && (nuRecorrido.HasValue ? r.NU_RECORRIDO != nuRecorrido.Value : true));
        }

        public virtual bool AnyUbicacionAsociadaRecorrido(int nuRecorrido, string ubicacion)
        {
            return _context.T_RECORRIDO_DET.Any(i => i.NU_RECORRIDO == nuRecorrido && i.CD_ENDERECO == ubicacion);
        }

        public virtual bool AnyRecorridoPredioPersonalizado(string predio)
        {
            return _context.T_RECORRIDO.Any(i => i.NU_PREDIO == predio && i.FL_DEFAULT == "N");
        }

        #endregion

        #region Dapper

        public virtual void AsociarAplicacionUsuario(List<AplicacionRecorridoUsuario> aplicaciones)
        {

            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();


            AddAplicacionUsuarioTemp(connection, tran, aplicaciones);
            var aplicacionesExistentes = GetAplicacionUsuarioRecorridoAsociado(connection, tran);

            RemoveAplicacionUsuarioTemp(connection, tran, aplicacionesExistentes);

            AddAsociacionAplicacionesUsuariosRecorrido(connection, tran);

            if (aplicaciones.FirstOrDefault().NumeroRecorridoDefault != aplicaciones.FirstOrDefault().IdRecorrido)
            {
                AddAsociacionAplicacionesUsuariosRecorridoDefault(connection, tran, aplicaciones.FirstOrDefault().NumeroRecorridoDefault);
            }

            RemoveAplicacionUsuarioTemp(connection, tran, aplicaciones);

        }

        public virtual void DesasociarAplicacionUsuario(List<AplicacionRecorridoUsuario> aplicaciones, string predio)
        {

            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();


            AddAplicacionUsuarioTemp(connection, tran, aplicaciones);

            if (aplicaciones.FirstOrDefault().NumeroRecorridoDefault == aplicaciones.FirstOrDefault().IdRecorrido && AnyNoDefaultAplicacionUsuarioAsociado(connection, tran, aplicaciones, predio))
            {
                throw new ValidationFailedException("REG700_msg_Error_AplicacionPorDefecto");
            }
            else if (aplicaciones.FirstOrDefault().NumeroRecorridoDefault != aplicaciones.FirstOrDefault().IdRecorrido && AnyAplicacionUsuarioAsociadoPredeterminada(connection, tran, aplicaciones))
            {
                throw new ValidationFailedException("REG700_msg_Error_AplicacionPorPredeterminada");
            }

            UpdateRecorrido(connection, tran, aplicaciones);
            RemoveAplicacionUsuario(connection, tran, aplicaciones);
            RemoveAplicacionUsuarioTemp(connection, tran, aplicaciones);

        }

        #region ADD

        public virtual void AddAplicacionUsuarioTemp(DbConnection connection, DbTransaction tran, List<AplicacionRecorridoUsuario> pedidosAsociar)
        {
            _dapper.BulkInsert(connection, tran, pedidosAsociar, "T_APLICACION_REC_USER_TEMP", new Dictionary<string, Func<AplicacionRecorridoUsuario, ColumnInfo>>
            {
                { "NU_RECORRIDO", x => new ColumnInfo(x.IdRecorrido)},
                { "CD_APLICACION", x => new ColumnInfo(x.IdAplicacion)},
                { "USERID", x => new ColumnInfo(x.UserId)},
                { "NU_TRANSACCION", x => new ColumnInfo(x.NuTransaccion)},
                { "DT_ADDROW", x => new ColumnInfo(x.FechaAlta)},
                { "NU_RECORRIDO_DEFAULT", x => new ColumnInfo(x.NumeroRecorridoDefault)},
            });
        }

        public virtual void AddAsociacionAplicacionesUsuariosRecorrido(DbConnection connection, DbTransaction tran)
        {

            string sql = @"INSERT INTO  T_APLICACION_RECORRIDO_USUARIO (NU_RECORRIDO,CD_APLICACION,USERID,FL_PREDETERMINADO,NU_TRANSACCION,DT_ADDROW) (
                    SELECT 
                        arut.NU_RECORRIDO,
                        arut.CD_APLICACION,
                        arut.USERID as UserId,
                        CASE WHEN arut.NU_RECORRIDO = arut.NU_RECORRIDO_DEFAULT THEN 'S' ELSE 'N' END as FL_PREDETERMINADO,
                        arut.NU_TRANSACCION,
                        arut.DT_ADDROW
                    FROM T_APLICACION_REC_USER_TEMP arut)
                        ";

            _dapper.Execute(connection, sql, commandType: CommandType.Text, transaction: tran);

        }

        public virtual void AddAsociacionAplicacionesUsuariosRecorridoDefault(DbConnection connection, DbTransaction tran, int recorridoDefault)
        {

            string sql = @"INSERT INTO  T_APLICACION_RECORRIDO_USUARIO (NU_RECORRIDO,CD_APLICACION,USERID,FL_PREDETERMINADO,NU_TRANSACCION,DT_ADDROW) (
                    SELECT 
                        arut.NU_RECORRIDO_DEFAULT,
                        arut.CD_APLICACION ,
                        arut.USERID ,
                        'S' FL_PREDETERMINADO,
                        arut.NU_TRANSACCION,
                        arut.DT_ADDROW
                    FROM T_APLICACION_REC_USER_TEMP arut 
                    LEFT JOIN (SELECT  CD_APLICACION ,USERID FROM T_APLICACION_RECORRIDO_USUARIO  WHERE NU_RECORRIDO = :NU_RECORRIDO_DEFAULT  GROUP BY  CD_APLICACION ,USERID ) aru ON aru.CD_APLICACION = arut.CD_APLICACION
                        AND aru.USERID = arut.USERID
                    WHERE aru.CD_APLICACION is null)
                        ";

            _dapper.Execute(connection, sql, param: new { NU_RECORRIDO_DEFAULT = recorridoDefault }, commandType: CommandType.Text, transaction: tran);

        }

        #endregion

        #region UPDATE

        public virtual void UpdateRecorrido(DbConnection connection, DbTransaction tran, List<AplicacionRecorridoUsuario> aplicaciones)
        {
            _dapper.BulkUpdate(connection, tran, aplicaciones, "T_APLICACION_RECORRIDO_USUARIO", new Dictionary<string, Func<AplicacionRecorridoUsuario, ColumnInfo>>
            {
                { "NU_TRANSACCION", x => new ColumnInfo(x.NuTransaccion)},
                { "DT_UPDROW", x => new ColumnInfo(DateTime.Now)},
                { "NU_TRANSACCION_DELETE",x=> new ColumnInfo( x.NuTransaccion)},
            }, new Dictionary<string, Func<AplicacionRecorridoUsuario, ColumnInfo>>
            {
                { "NU_RECORRIDO", x => new ColumnInfo(x.IdRecorrido)},
                { "CD_APLICACION", x => new ColumnInfo(x.IdAplicacion)},
                { "USERID", x => new ColumnInfo(x.UserId)},
            });
        }

        #endregion

        #region DELETE

        public virtual void RemoveAplicacionUsuarioTemp(DbConnection connection, DbTransaction tran, List<AplicacionRecorridoUsuario> pedidosAsociar)
        {
            _dapper.BulkDelete(connection, tran, pedidosAsociar, "T_APLICACION_REC_USER_TEMP", new Dictionary<string, Func<AplicacionRecorridoUsuario, object>>
            {
                { "NU_RECORRIDO", x => x.IdRecorrido},
                { "CD_APLICACION", x => x.IdAplicacion},
                { "USERID", x => x.UserId},
            });
        }

        public virtual void RemoveAplicacionUsuario(DbConnection connection, DbTransaction tran, List<AplicacionRecorridoUsuario> aplicaciones)
        {
            _dapper.BulkDelete(connection, tran, aplicaciones, "T_APLICACION_RECORRIDO_USUARIO",
                new Dictionary<string, Func<AplicacionRecorridoUsuario, object>>
                {
                    { "NU_RECORRIDO", x => x.IdRecorrido},
                    { "CD_APLICACION", x => x.IdAplicacion},
                    { "USERID", x => x.UserId},
            });
        }

        public virtual void DeleteRecorrido(Recorrido recorrido)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var detalles = GetAllDetallesRecorridosByIdRecorrido(recorrido.Id);

            _dapper.BulkUpdate(connection, tran, detalles, "T_RECORRIDO_DET", new Dictionary<string, Func<DetalleRecorrido, ColumnInfo>>
            {
                { "NU_TRANSACCION", x => new ColumnInfo(_context.GetTransactionNumber())},
                { "NU_TRANSACCION_DELETE", x => new ColumnInfo(_context.GetTransactionNumber())},
                { "DT_UPDROW", x => new ColumnInfo(DateTime.Now)}
            }, new Dictionary<string, Func<DetalleRecorrido, ColumnInfo>>
            {
                { "NU_RECORRIDO", x => new ColumnInfo(recorrido.Id)}
            });

            _context.SaveChanges();

            _dapper.BulkDelete(connection, tran, detalles, "T_RECORRIDO_DET", new Dictionary<string, Func<DetalleRecorrido, object>>
            {
                    { "NU_RECORRIDO", x => recorrido.Id}
            });

            _context.SaveChanges();

            string sql = @"UPDATE T_APLICACION_RECORRIDO 
                    SET NU_TRANSACCION = :transaccion,
                        NU_TRANSACCION_DELETE = :transaccion, 
                        DT_UPDROW = :fechaModificacion 
                    WHERE NU_RECORRIDO = :idRecorrido";

            _dapper.Execute(connection, sql, param: new
            {
                transaccion = _context.GetTransactionNumber(),
                fechaModificacion = DateTime.Now,
                idRecorrido = recorrido.Id
            }, transaction: tran);


            sql = @"DELETE FROM T_APLICACION_RECORRIDO WHERE NU_RECORRIDO = :idRecorrido";
            _dapper.Execute(connection, sql, param: new { idRecorrido = recorrido.Id }, transaction: tran);

            sql = @"UPDATE T_APLICACION_RECORRIDO_USUARIO 
                    SET NU_TRANSACCION = :transaccion,
                        NU_TRANSACCION_DELETE = :transaccion, 
                        DT_UPDROW = :fechaModificacion 
                    WHERE NU_RECORRIDO = :idRecorrido";

            _dapper.Execute(connection, sql, param: new
            {
                transaccion = _context.GetTransactionNumber(),
                fechaModificacion = DateTime.Now,
                idRecorrido = recorrido.Id
            }, transaction: tran);


            sql = @"DELETE FROM T_APLICACION_RECORRIDO_USUARIO WHERE NU_RECORRIDO = :idRecorrido";
            _dapper.Execute(connection, sql, param: new { idRecorrido = recorrido.Id }, transaction: tran);

            sql = @"DELETE FROM T_RECORRIDO WHERE NU_RECORRIDO = :idRecorrido";
            _dapper.Execute(connection, sql, param: new { idRecorrido = recorrido.Id }, transaction: tran);
        }

        #endregion

        #region GET

        public virtual bool AnyNoDefaultAplicacionUsuarioAsociado(DbConnection connection, DbTransaction tran, List<AplicacionRecorridoUsuario> aplicaciones, string predio)
        {
            string sql = @"
                    SELECT 
                        arut.NU_RECORRIDO as IdRecorrido,
                        arut.CD_APLICACION as IdAplicacion,
                        arut.USERID as UserId
                    FROM T_APLICACION_REC_USER_TEMP arut 
                    INNER JOIN T_APLICACION_RECORRIDO_USUARIO aru on aru.CD_APLICACION = arut.CD_APLICACION AND
                        aru.USERID = arut.USERID  
                    INNER JOIN  T_RECORRIDO r on r.NU_RECORRIDO = aru.NU_RECORRIDO
                    WHERE r.NU_PREDIO = :Predio AND aru.NU_RECORRIDO <> arut.NU_RECORRIDO";

            return _dapper.Query<PedidoAsociarUnidad>(connection, sql, new DynamicParameters(new { Predio = predio }), commandType: CommandType.Text, transaction: tran).FirstOrDefault() != null;
        }

        public virtual List<AplicacionRecorridoUsuario> GetAplicacionUsuarioRecorridoAsociado(DbConnection connection, DbTransaction tran)
        {
            string sql = @"
                    SELECT 
                        arut.NU_RECORRIDO as IdRecorrido,
                        arut.CD_APLICACION as IdAplicacion,
                        arut.USERID as UserId
                    FROM T_APLICACION_REC_USER_TEMP arut 
                    INNER JOIN T_APLICACION_RECORRIDO_USUARIO aru ON aru.NU_RECORRIDO = arut.NU_RECORRIDO
                        AND aru.CD_APLICACION = arut.CD_APLICACION
                        AND aru.USERID = arut.USERID";

            return _dapper.Query<AplicacionRecorridoUsuario>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();

        }

        #endregion

        #endregion
    }
}
