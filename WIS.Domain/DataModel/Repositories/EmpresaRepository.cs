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
using WIS.Domain.General;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.General.Enums;
using WIS.Domain.Recepcion;
using WIS.Domain.Security;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class EmpresaRepository
    {
        protected readonly int _userId;
        protected readonly WISDB _context;
        protected readonly IDapper _dapper;
        protected readonly string _application;
        protected readonly EmpresaMapper _mapper;

        public EmpresaRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new EmpresaMapper();
            _dapper = dapper;
            this._dapper = dapper;
        }

        #region Any

        public virtual bool IsEmpresaDocumental(int empresa)
        {
            return this._context.V_EMPRESA_DOCUMENTAL
                .AsNoTracking()
                .Any(e => e.CD_EMPRESA == empresa
                    && e.FL_DOCUMENTAL == "S");
        }

        public virtual bool AnyEmpresaDocumentalActiva()
        {
            return this._context.V_EMPRESA_DOCUMENTAL
                .Join(this._context.T_EMPRESA,
                    ed => ed.CD_EMPRESA,
                    e => e.CD_EMPRESA,
                    (ed, e) => new { EmpresaDocumental = ed, Empresa = e })
                .AsNoTracking()
                .Any(ede => ede.EmpresaDocumental.FL_DOCUMENTAL == "S"
                    && ede.Empresa.CD_SITUACAO == SituacionDb.Activo);
        }

        public virtual bool AnyEmpresa(int cdEmpresa)
        {
            return this._context.T_EMPRESA
                .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == this._userId),
                    e => e.CD_EMPRESA,
                    ef => ef.CD_EMPRESA,
                    (e, ef) => e)
                .AsNoTracking()
                .Any(e => e.CD_EMPRESA == cdEmpresa);
        }

        public virtual bool AnyEmpresaGeneral(int cdEmpresa)
        {
            return this._context.T_EMPRESA
                .AsNoTracking()
                .Any(d => d.CD_EMPRESA == cdEmpresa);
        }

        public virtual bool AnyEmpresaListaPrecio(int cdEmpresa, int precioLista)
        {
            return this._context.T_EMPRESA
                .AsNoTracking()
                .Any(d => d.CD_EMPRESA == cdEmpresa
                    && d.CD_LISTA_PRECIO == precioLista);
        }

        public virtual bool AnyEmpresaSinListaPrecio(int nuEjecucion)
        {
            return this._context.T_FACTURACION_RESULTADO
                .Join(_context.T_EMPRESA.Where(e => !e.CD_LISTA_PRECIO.HasValue),
                    fr => fr.CD_EMPRESA,
                    e => e.CD_EMPRESA,
                    (fr, e) => fr)
                .AsNoTracking().Any(d => d.NU_EJECUCION == nuEjecucion);
        }

        #endregion

        #region Get

        public virtual Empresa GetEmpresa(int codigo)
        {
            var entity = this._context.T_EMPRESA
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_EMPRESA == codigo);

            return this._mapper.MapToObject(entity);
        }

        public virtual Empresa GetEmpresaConRelaciones(int codigo)
        {
            var entity = this._context.T_EMPRESA
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_EMPRESA == codigo);

            T_PAIS_SUBDIVISION_LOCALIDAD localidad = this._context.T_PAIS_SUBDIVISION_LOCALIDAD
                .Include("T_PAIS_SUBDIVISION")
                .AsNoTracking()
                .FirstOrDefault(s => s.ID_LOCALIDAD == entity.ID_LOCALIDAD);

            if (localidad != null)
                localidad.T_PAIS_SUBDIVISION.T_PAIS = this._context.T_PAIS
                    .AsNoTracking()
                    .FirstOrDefault(x => x.CD_PAIS == localidad.T_PAIS_SUBDIVISION.CD_PAIS);

            var tipoFiscal = _context.T_DET_DOMINIO
                .AsNoTracking()
                .FirstOrDefault(w => w.CD_DOMINIO == CodigoDominioDb.TiposFiscales
                    && w.NU_DOMINIO == entity.ND_TIPO_FISCAL);

            var tipoNotificacion = _context.T_DET_DOMINIO
                .AsNoTracking()
                .FirstOrDefault(w => w.CD_DOMINIO == CodigoDominioDb.TiposNotificaciones
                    && w.NU_DOMINIO == entity.TP_NOTIFICACION);

            return this._mapper.MapToObjectWithRelations(entity, localidad, tipoFiscal, tipoNotificacion);
        }

        public virtual Empresa GetEmpresaUnicaParaUsuario(int user)
        {
            var empresasAsignadas = _context.T_EMPRESA_FUNCIONARIO
                .AsNoTracking()
                .Where(w => w.USERID == user)
                .Select(w => w.CD_EMPRESA);

            int cantEmpresas = (empresasAsignadas?.Count() ?? 0);

            if (cantEmpresas > 1 || cantEmpresas == 0)
            {
                return null;
            }
            else
            {
                return _mapper.MapToObject(this._context.T_EMPRESA.AsNoTracking().FirstOrDefault(d => d.CD_EMPRESA == empresasAsignadas.FirstOrDefault()));
            }

        }

        public virtual List<Empresa> GetEmpresasParaUsuario(int user)
        {
            List<Empresa> empresas = new List<Empresa>();

            var empresasAsignadas = _context.T_EMPRESA_FUNCIONARIO
                .Include("T_EMPRESA")
                .AsNoTracking()
                .Where(w => w.USERID == user);

            foreach (var empresaAsginada in empresasAsignadas)
            {
                empresas.Add(this._mapper.MapToObject(empresaAsginada.T_EMPRESA));
            }

            return empresas;
        }

        public virtual List<Empresa> GetEmpresasFacturacionEjecucionParaUsuario(int user, int nuEjecucion)
        {
            List<Empresa> empresas = new List<Empresa>();

            var empresasAsignadas = _context.T_EMPRESA_FUNCIONARIO
                .Include("T_EMPRESA")
                .Where(ef => ef.USERID == user)
                .Select(ef => ef.T_EMPRESA)
                .GroupJoin(_context.T_FACTURACION_EJEC_EMPRESA.Where(fee => fee.NU_EJECUCION == nuEjecucion),
                    e => new { e.CD_EMPRESA },
                    fee => new { fee.CD_EMPRESA },
                    (e, fees) => new { Empresa = e, FacturacionEjecEmpresas = fees })
                .SelectMany(s => s.FacturacionEjecEmpresas.DefaultIfEmpty(), (e, fee) => new { Empresa = e.Empresa, FacturacionEjecEmpresa = fee })
                .Where(w => w.FacturacionEjecEmpresa != null)
                .AsNoTracking()
                .Select(s => s.Empresa);

            foreach (var empresaAsginada in empresasAsignadas)
            {
                empresas.Add(this._mapper.MapToObject(empresaAsginada));
            }

            return empresas;
        }

        public virtual List<int> GetEmpresasAsignadasUsuario(int user)
        {
            return _context.T_EMPRESA_FUNCIONARIO
                .AsNoTracking()
                .Where(w => w.USERID == user)
                .Select(w => w.CD_EMPRESA)
                .ToList();
        }
        public virtual List<Empresa> GetEmpresasByUsuario(int user)
        {
            var empresas = new List<Empresa>();
            var empresasUsuario = this._context.T_EMPRESA
                .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == this._userId),
                    e => e.CD_EMPRESA,
                    ef => ef.CD_EMPRESA,
                    (e, ef) => e)
                .AsNoTracking();

            foreach (var entry in empresasUsuario)
            {
                empresas.Add(this._mapper.MapToObject(entry));
            }

            return empresas;
        }
        public virtual string GetNombre(int cdEmpresa)
        {
            return this._context.T_EMPRESA
                .AsNoTracking()
                .Where(d => d.CD_EMPRESA == cdEmpresa)
                .Select(d => d.NM_EMPRESA)
                .FirstOrDefault();
        }

        public virtual List<Empresa> GetByNombrePartial(string nombre)
        {
            var empresas = new List<Empresa>();
            var empresasUsuario = this._context.T_EMPRESA
                .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == this._userId),
                    e => e.CD_EMPRESA,
                    ef => ef.CD_EMPRESA,
                    (e, ef) => e)
                .AsNoTracking();
            var entries = empresasUsuario
                .Where(e => e.NM_EMPRESA.ToLower().Contains(nombre.ToLower()));

            foreach (var entry in entries)
            {
                empresas.Add(this._mapper.MapToObject(entry));
            }

            return empresas;
        }

        public virtual List<Empresa> GetByNombreOrCodePartial(string value)
        {
            return GetByNombreOrCodePartialForUsuario(value, this._userId);
        }

        public virtual List<Empresa> GetEmpresasUsuarioNoDocumentalesByNombreOrCodePartial(string value)
        {
            return GetEmpresasUsuarioByNombreOrCodePartial(value, false);
        }

        public virtual List<Empresa> GetEmpresasUsuarioByNombreOrCodePartial(string value, bool documental)
        {
            var empresasUsuario = this._context.T_EMPRESA
                .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == this._userId),
                    e => e.CD_EMPRESA,
                    ef => ef.CD_EMPRESA,
                    (e, ef) => e)
                .Join(this._context.V_EMPRESA_DOCUMENTAL.Where(ed => ed.FL_DOCUMENTAL == (documental ? "S" : "N")),
                    e => e.CD_EMPRESA,
                    ed => ed.CD_EMPRESA,
                    (e, ed) => e)
                .AsNoTracking();

            int cdEmpresa;

            if (int.TryParse(value, out cdEmpresa))
            {
                empresasUsuario = empresasUsuario
                    .Where(d => (d.CD_EMPRESA == cdEmpresa
                        || (d.CD_EMPRESA.ToString().Contains(cdEmpresa.ToString()))
                        || (d.NM_EMPRESA.ToLower().Contains(value.ToLower()))));
            }
            else
            {
                empresasUsuario = empresasUsuario
                    .Where(d => d.NM_EMPRESA.ToLower().Contains(value.ToLower()));
            }

            return empresasUsuario
                .Select(d => this._mapper.MapToObject(d))
                .ToList();
        }

        public virtual List<Empresa> GetByNombreCodeOrNroFiscalPartial(string value)
        {
            int cdEmpresa;
            var empresas = this._context.T_EMPRESA.AsNoTracking();

            if (int.TryParse(value, out cdEmpresa))
            {
                empresas = empresas
                    .Where(d => d.NM_EMPRESA.ToLower().Contains(value.ToLower())
                        || d.CD_EMPRESA == cdEmpresa
                        || d.CD_CGC_EMPRESA.Contains(value));
            }
            else
            {
                empresas = empresas
                    .Where(d => d.NM_EMPRESA.ToLower().Contains(value.ToLower())
                        || d.CD_CGC_EMPRESA.Contains(value));
            }

            return empresas
                .Select(d => this._mapper.MapToObject(d))
                .ToList();
        }

        public virtual List<Empresa> GetByNombreOrCodePartialForUsuario(string value, int user)
        {
            var empresasUsuario = this._context.T_EMPRESA
                .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == user),
                    e => e.CD_EMPRESA,
                    ef => ef.CD_EMPRESA,
                    (e, ef) => e)
                .AsNoTracking();

            int cdEmpresa;

            if (int.TryParse(value, out cdEmpresa))
            {
                empresasUsuario = empresasUsuario
                    .Where(d => (d.CD_EMPRESA == cdEmpresa
                        || (d.CD_EMPRESA.ToString().Contains(cdEmpresa.ToString()))
                        || (d.NM_EMPRESA.ToLower().Contains(value.ToLower()))));
            }
            else
            {
                empresasUsuario = empresasUsuario
                    .Where(d => d.NM_EMPRESA.ToLower().Contains(value.ToLower()));
            }

            return empresasUsuario
                .Select(d => this._mapper.MapToObject(d))
                .ToList();
        }

        public virtual List<Empresa> GetEmpresasUsuarioDocumentalesByNombreOrCodePartial(string value, int user)
        {
            return GetEmpresasUsuarioByNombreOrCodePartial(value, true);
        }

        public virtual HashSet<int> GetEmpresasDocumentales()
        {
            var result = new HashSet<int>();
            var empresas = this._context.V_EMPRESA_DOCUMENTAL
                .AsNoTracking()
                .Where(e => e.FL_DOCUMENTAL == "S")
                .Select(e => e.CD_EMPRESA);

            foreach (var empresa in empresas)
            {
                if (!result.Contains(empresa))
                {
                    result.Add(empresa);
                }
            }

            return result;
        }

        public virtual byte[] GetFirma(int empresa, string contenido)
        {
            var model = _context.T_EMPRESA
                .AsNoTracking()
                .FirstOrDefault(e => e.CD_EMPRESA == empresa);

            if (!string.IsNullOrEmpty(model?.VL_SECRET))
            {
                var secret = Encrypter.Decrypt(model.VL_SECRET, model.VL_SECRETSALT, (int)model.VL_SECRETFORMAT.Value);
                return Signer.ComputeHash(secret, contenido);
            }

            return null;
        }

        public virtual List<Empresa> GetEmpresas()
        {
            List<Empresa> empresas = new List<Empresa>();
            List<T_EMPRESA> entities = this._context.T_EMPRESA.AsNoTracking().ToList();

            foreach (var entity in entities)
            {
                empresas.Add(this._mapper.MapToObject(entity));
            }

            return empresas;
        }

        public virtual bool IsCodigoMultidatoHabilitado(int empresa, string codigoMultidato)
        {
            return _context.T_CODIGO_MULTIDATO_EMPRESA
                .AsNoTracking()
                .Any(c => c.CD_EMPRESA == empresa
                    && c.CD_CODIGO_MULTIDATO == codigoMultidato
                    && c.FL_HABILITADO == "S");
        }

        public virtual List<int> GetCdEmpresasForUsuario(int user)
        {

            return this._context.T_EMPRESA.AsNoTracking()
                .Where(d => d.T_EMPRESA_FUNCIONARIO.Any(e => e.USERID == user)).ToList().Select(d => d.CD_EMPRESA).ToList();
        }

        #endregion

        #region Add

        public virtual void AddEmpresa(Empresa empresa)
        {
            AddEmpresa(empresa, null);
        }

        public virtual void AddEmpresa(Empresa empresa, string[] secretInfo)
        {
            T_EMPRESA entity = this._mapper.MapToEntity(empresa);

            if (secretInfo != null)
            {
                entity.VL_SECRET = secretInfo[0];
                entity.VL_SECRETSALT = secretInfo[1];
                entity.VL_SECRETFORMAT = int.Parse(secretInfo[2]);
            }

            this._context.T_EMPRESA.Add(entity);
        }

        public virtual void AsignarEmpresasUsuario(int userId, List<int> empresas)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var empresasFuncionario = empresas.Select(cdEmpresa => new EmpresaFuncionario
            {
                Empresa = cdEmpresa,
                Usuario = userId
            }).ToList();

            _dapper.BulkInsert(connection, tran, empresasFuncionario, "T_EMPRESA_FUNCIONARIO",
                new Dictionary<string, Func<EmpresaFuncionario, ColumnInfo>>
                {
                    { "CD_EMPRESA", x => new ColumnInfo(x.Empresa) },
                    { "USERID", x => new ColumnInfo(x.Usuario) }
                });
        }

        #endregion

        #region Remove

        public virtual void RemoverEmpresasUsuarios(List<int> empresas, List<int> usuarios)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var empresaFuncionarios = empresas
                .SelectMany(empresa => usuarios, (cdEmpresa, userId) => new EmpresaFuncionario
                {
                    Empresa = cdEmpresa,
                    Usuario = userId
                })
                .ToList();

            _dapper.BulkDelete(connection, tran, empresaFuncionarios, "T_EMPRESA_FUNCIONARIO",
                new Dictionary<string, Func<EmpresaFuncionario, object>>
                {
                    { "CD_EMPRESA", x => x.Empresa },
                    { "USERID", x => x.Usuario }
                });
        }

        #endregion

        #region Update

        public virtual void UpdateEmpresa(Empresa empresa)
        {
            UpdateEmpresa(empresa, null);
        }

        public virtual void UpdateEmpresa(Empresa empresa, string[] secretInfo)
        {
            var entity = this._mapper.MapToEntity(empresa);
            var attachedEntity = this._context.T_EMPRESA.Local
                .FirstOrDefault(e => e.CD_EMPRESA == entity.CD_EMPRESA);

            if (empresa.IsNotifiedByWebhook)
            {
                if (secretInfo != null)
                {
                    entity.VL_SECRET = secretInfo[0];
                    entity.VL_SECRETSALT = secretInfo[1];
                    entity.VL_SECRETFORMAT = int.Parse(secretInfo[2]);
                }
                else
                {
                    var model = _context.T_EMPRESA.AsNoTracking().FirstOrDefault(e => e.CD_EMPRESA == empresa.Id);
                    entity.VL_SECRET = model.VL_SECRET;
                    entity.VL_SECRETSALT = model.VL_SECRETSALT;
                    entity.VL_SECRETFORMAT = model.VL_SECRETFORMAT;
                }
            }

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = this._context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_EMPRESA.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<Empresa> GetEmpresas(IEnumerable<Empresa> empresas)
        {
            IEnumerable<Empresa> resultado = new List<Empresa>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_EMPRESA_TEMP (CD_EMPRESA) VALUES (:Id)";
                    _dapper.Execute(connection, sql, empresas, transaction: tran);

                    sql = GetSqlSelectEmpresa() +
                        @"INNER JOIN T_EMPRESA_TEMP T ON E.CD_EMPRESA = T.CD_EMPRESA";

                    resultado = _dapper.Query<Empresa>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task<Empresa> GetEmpresaOrNull(int codigo, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                Empresa model = GetEmpresa(connection, new Empresa()
                {
                    Id = codigo
                });

                model = Fill(model);

                return model;
            }
        }

        public virtual Empresa Fill(Empresa model)
        {
            if (model != null)
            {
                model.IsLocked = model.IsLockedId == "S";
                model.TipoFiscal = new DominioDetalle() { Id = model.TipoFiscalId };

                if (!string.IsNullOrEmpty(model.TipoNotificacionId))
                {
                    model.TipoNotificacion = new DominioDetalle() { Id = model.TipoNotificacionId };
                }
            }
            model = MapInternal(model);
            return model;
        }

        public virtual async Task AddEmpresas(List<Empresa> empresas, IEmpresaServiceContext context, CancellationToken cancelToken = default)
        {
            await AddEmpresas(GetBulkOperationContext(empresas, context), cancelToken);
        }

        public virtual async Task AddEmpresas(EmpresaBulkOperationContext context, CancellationToken cancelToken)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertEmpresas(connection, tran, context.NewEmpresas);
                    await BulkInsertUsuariosEmpresa(connection, tran, context.NewUsuariosEmpresa);
                    await BulkInsertTiposRecepcionEmpresa(connection, tran, context.NewTiposRecepcionEmpresa);
                    await BulkInsertTiposRecepcionReporteEmpresa(connection, tran, context.NewTiposRecepcionReporteEmpresa);
                    await BulkInsertAgentesDefault(connection, tran, context.Agentes);
                    await BulkUpdateEmpresas(connection, tran, context.UpdEmpresas);

                    tran.Commit();
                }
            }
        }

        public virtual EmpresaBulkOperationContext GetBulkOperationContext(List<Empresa> empresas, IEmpresaServiceContext serviceContext)
        {
            var context = new EmpresaBulkOperationContext();

            foreach (var empresa in empresas)
            {
                var model = serviceContext.GetEmpresa(empresa.Id);

                Map(empresa, model);

                if (model == null)
                {
                    context.NewEmpresas.Add(GetEmpresaEntity(empresa));

                    foreach (var u in serviceContext.UsuariosAsignables)
                    {
                        context.NewUsuariosEmpresa.Add(new { Codigo = empresa.Id, userId = u });
                    }

                    foreach (var tr in serviceContext.TiposRecepcion)
                    {
                        string[] datos = tr.Split(':');
                        string tpRecepcion = datos[0];
                        string tpRecExt = datos[1];

                        context.NewTiposRecepcionEmpresa.Add(GetTipoRecepcionEmpresa(empresa.Id, tpRecepcion, tpRecExt));

                        foreach (var reporte in serviceContext.GetReportesTipoRecepcion(tpRecepcion))
                        {
                            context.NewTiposRecepcionReporteEmpresa.Add(GetTipoRecepcionReporteEmpresaEntity(empresa.Id, tpRecExt, reporte));
                        }
                    }

                    context.Agentes.Add(GetClienteArmadoKit(empresa.Id));
                    context.Agentes.Add(GetClienteMuestra(empresa.Id));
                    context.Agentes.Add(GetProveedorMuestra(empresa.Id));
                }
                else
                {
                    context.UpdEmpresas.Add(GetEmpresaEntity(empresa));
                }
            }

            return context;
        }

        public virtual object GetClienteArmadoKit(int empresa)
        {
            return new
            {
                Empresa = empresa,
                Tipo = TipoAgenteDb.Cliente,
                Codigo = AgentesPorDefectoEmpresaDb.ClienteArmadoKitCodigo,
                CodigoAgente = AgentesPorDefectoEmpresaDb.ClienteArmadoKitCodigo,
                Ruta = 0,
                Descripcion = AgentesPorDefectoEmpresaDb.ClienteMuestrasDescripcion,
                Estado = 15,
                FechaModificacion = DateTime.Now,
            };
        }

        public virtual object GetClienteMuestra(int empresa)
        {
            return new
            {
                Empresa = empresa,
                Tipo = TipoAgenteDb.Cliente,
                Codigo = AgentesPorDefectoEmpresaDb.ClienteMuestrasCodigo,
                CodigoAgente = AgentesPorDefectoEmpresaDb.ClienteMuestrasCodigo,
                Ruta = 1,
                Descripcion = AgentesPorDefectoEmpresaDb.ClienteMuestrasDescripcion,
                Estado = 15,
                FechaModificacion = DateTime.Now,
            };
        }

        public virtual object GetProveedorMuestra(int empresa)
        {
            return new
            {
                Empresa = empresa,
                Tipo = TipoAgenteDb.Proveedor,
                Codigo = AgentesPorDefectoEmpresaDb.ProveedorMuestrasCodigo,
                CodigoAgente = AgentesPorDefectoEmpresaDb.ProveedorMuestrasCodigo,
                Ruta = 1,
                Descripcion = AgentesPorDefectoEmpresaDb.ProveedorMuestrasDescripcion,
                Estado = 15,
                FechaModificacion = DateTime.Now,
            };
        }

        public virtual object GetTipoRecepcionEmpresa(int empresa, string tipoRecepcion, string tipoRecExt)
        {
            return new
            {
                cdEmpresa = empresa,
                ManejoInterfaz = "D",
                TipoRec = tipoRecepcion,
                TipoRecExt = tipoRecExt,
                Descripcion = $"Tipo de recepcion {tipoRecepcion}",
                Habilitado = "S"
            };
        }

        public virtual object GetTipoRecepcionReporteEmpresaEntity(int empresa, string tipoRecExt, string reporte)
        {
            return new
            {
                cdEmpresa = empresa,
                TipoRecExt = tipoRecExt,
                Reporte = reporte
            };
        }

        public virtual Dictionary<string, List<string>> GetReportesTiposRecepcion(string paramTpRec)
        {
            var reportesTipoRecepcion = new Dictionary<string, List<string>>();

            if (!string.IsNullOrEmpty(paramTpRec))
            {
                var tiposRecepcion = new HashSet<string>();
                var tpRec = paramTpRec.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (var tp in tpRec)
                {
                    var datos = tp.Split(':');
                    var tpRecepcion = datos[0];

                    if (!tiposRecepcion.Contains(tpRecepcion))
                    {
                        tiposRecepcion.Add(tpRecepcion);
                    }
                }

                var sql = @"
                    SELECT 
                        TP_RECEPCION AS TipoRecepcion, 
                        CD_REPORTE AS CodigoReporte 
                    FROM T_RECEPCION_TIPO_REPORTE_DEF 
                    WHERE TP_RECEPCION IN :tipos";

                using (var connection = _dapper.GetDbConnection())
                {
                    connection.Open();

                    var query = _dapper.Query<EmpresaRecepcionTipoReporte>(connection, sql, param: new
                    {
                        tipos = tiposRecepcion.ToArray()
                    }, commandType: CommandType.Text);

                    foreach (var tipoRecepcionReporte in query)
                    {
                        if (!reportesTipoRecepcion.ContainsKey(tipoRecepcionReporte.TipoRecepcion))
                        {
                            reportesTipoRecepcion[tipoRecepcionReporte.TipoRecepcion] = new List<string>();
                        }

                        reportesTipoRecepcion[tipoRecepcionReporte.TipoRecepcion].Add(tipoRecepcionReporte.CodigoReporte);
                    }
                }
            }

            return reportesTipoRecepcion;
        }

        public virtual List<string> GetTiposRecepcion(string paramTpRec)
        {
            var tiposRecepcion = new List<string>();

            if (!string.IsNullOrEmpty(paramTpRec))
            {
                var dictTpRec = new Dictionary<string, string>();
                var tpRec = paramTpRec.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (var tp in tpRec)
                {
                    string[] datos = tp.Split(':');
                    string tpRecepcion = datos[0];
                    dictTpRec[tpRecepcion.ToUpper()] = tp;
                }

                var sql = "SELECT TP_RECEPCION FROM T_RECEPCION_TIPO WHERE TP_RECEPCION IN :tipos";

                using (var connection = _dapper.GetDbConnection())
                {
                    connection.Open();

                    var query = _dapper.Query<string>(connection, sql, param: new
                    {
                        tipos = dictTpRec.Keys.ToArray()
                    }, commandType: CommandType.Text);

                    foreach (var tipo in query)
                    {
                        tiposRecepcion.Add(dictTpRec[tipo.ToUpper()]);
                    }
                }
            }

            return tiposRecepcion;
        }

        public virtual List<int> GetUsuariosAsignables()
        {
            var sql = @"SELECT DISTINCT U.USERID 
                FROM USERS U
                INNER JOIN V_RECURSOS_USUARIO RU ON U.USERID = RU.USERID 
                WHERE RU.UNIQUENAME= :recurso
                    AND RU.FL_ACTIVO = 'S'";

            using (var connection = _dapper.GetDbConnection())
            {
                connection.Open();

                return _dapper.Query<int>(connection, sql, param: new
                {
                    recurso = Resources.GENERAL_UsuarioTodasLasEmpresas
                }, commandType: CommandType.Text).ToList();
            }
        }

        public virtual async Task BulkInsertEmpresas(DbConnection connection, DbTransaction tran, List<object> empresas)
        {
            string sql = @"INSERT INTO T_EMPRESA (
                        CD_EMPRESA, NM_EMPRESA, CD_SITUACAO, DT_ADDROW, DS_ENDERECO, NU_TELEFONE, CD_CGC_EMPRESA, CD_FORN_DEVOLUCAO, VL_POS_PALETE, VL_POS_PALETE_DIA, 
                        QT_DIAS_POR_PERIODO, DS_CP_POSTAL, CD_CLIENTE_ARMADO_KIT, DS_ANEXO1, DS_ANEXO2, DS_ANEXO3, DS_ANEXO4, IM_MINIMO_STOCK, ID_UND_FACT_EMPRESA, 
                        ID_OPERATIVO, TP_ALMACENAJE_Y_SEGURO, ID_DAP, CD_EMPRESA_DE_CONSOLIDADO, ID_LOCALIDAD, ND_TIPO_FISCAL, CD_LISTA_PRECIO) 
                        VALUES (:Codigo, :Nombre,:Estado, :FechaInsercion, :Direccion, :Telefono, :NumeroFiscal,  :ProveedorDevolucion, :ValorPallet, 
                        :ValorPalletDia, :CantidadDiasPeriodo, :CodigoPostal, :ClienteArmadoKit, :Anexo1, :Anexo2, :Anexo3, :Anexo4, :ValorMinimoStock, 
                        :IdUnidadFactura, :IdOperativo, :TipoDeAlmacenajeYSeguro, :IdDAP, :EmpresaConsolidado, :IdLocalidad, :TipoFiscal, :ListaPrecio)";

            await _dapper.ExecuteAsync(connection, sql, empresas, transaction: tran);

            string sqlDominio = @"INSERT INTO T_DET_DOMINIO (
                                NU_DOMINIO, CD_DOMINIO, CD_DOMINIO_VALOR, DS_DOMINIO_VALOR)
                                VALUES (:NumeroDominio, :CodigoDominio, :Codigo , :DescripcionDominio)";

            await _dapper.ExecuteAsync(connection, sqlDominio, empresas, transaction: tran);
        }

        public virtual async Task BulkInsertTiposRecepcionEmpresa(DbConnection connection, DbTransaction tran, List<object> tipos)
        {
            // NU_RECEPCION_REL_EMPRESA_TIPO se asigna por trigger
            var sql = @"INSERT INTO T_RECEPCION_REL_EMPRESA_TIPO
                            (NU_RECEPCION_REL_EMPRESA_TIPO, TP_RECEPCION_EXTERNO, TP_RECEPCION, CD_EMPRESA, 
                            FL_MANEJO_INTERFAZ, DS_RECEPCION_EXTERNO, FL_HABILITADO) 
                            VALUES (NULL, :TipoRecExt, :TipoRec, :cdEmpresa, :ManejoInterfaz, :Descripcion, :Habilitado)";

            await _dapper.ExecuteAsync(connection, sql, tipos, transaction: tran);
        }

        public virtual async Task BulkInsertAgentesDefault(DbConnection connection, DbTransaction tran, List<object> agentes)
        {
            // NU_RECEPCION_REL_EMPRESA_TIPO se asigna por trigger
            var sql = @"INSERT INTO T_CLIENTE (CD_EMPRESA, CD_CLIENTE, CD_ROTA, DS_CLIENTE, CD_AGENTE, TP_AGENTE, CD_SITUACAO, DT_ALTERACAO, DT_CADASTRAMENTO, DT_SITUACAO) 
                        VALUES (:Empresa, :Codigo, :Ruta, :Descripcion, :CodigoAgente, :Tipo, :Estado, :FechaModificacion, :FechaModificacion, :FechaModificacion)";

            await _dapper.ExecuteAsync(connection, sql, agentes, transaction: tran);
        }

        public virtual async Task BulkInsertTiposRecepcionReporteEmpresa(DbConnection connection, DbTransaction tran, List<object> tipos)
        {
            // NU_REC_EMP_TIPO_REP se asigna por trigger
            var sql = @"INSERT INTO T_RECEPCION_EMP_TIPO_REPORTE (NU_REC_EMP_TIPO_REP, CD_EMPRESA, TP_RECEPCION_EXTERNO, CD_REPORTE)
                            VALUES (NULL, :cdEmpresa, :TipoRecExt, :Reporte)";

            await _dapper.ExecuteAsync(connection, sql, tipos, transaction: tran);
        }

        public virtual async Task BulkInsertUsuariosEmpresa(DbConnection connection, DbTransaction tran, List<object> usuarios)
        {
            var sql = @"INSERT INTO T_EMPRESA_FUNCIONARIO (CD_EMPRESA, USERID) VALUES (:Codigo,:userId)";
            await _dapper.ExecuteAsync(connection, sql, usuarios, transaction: tran);
        }

        public virtual async Task BulkUpdateEmpresas(DbConnection connection, DbTransaction tran, List<object> empresas)
        {
            string sql = @"
                        UPDATE T_EMPRESA SET                 
                        NM_EMPRESA= :Nombre,
                        CD_SITUACAO= :Estado,
                        DT_UPDROW= :FechaModificacion,
                        DS_ENDERECO= :Direccion,              
                        NU_TELEFONE= :Telefono,
                        CD_CGC_EMPRESA= :NumeroFiscal,              
                        CD_FORN_DEVOLUCAO= :ProveedorDevolucion,
                        VL_POS_PALETE= :ValorPallet,
                        VL_POS_PALETE_DIA= :ValorPalletDia,
                        QT_DIAS_POR_PERIODO= :CantidadDiasPeriodo,
                        DS_ANEXO1= :Anexo1,
                        DS_ANEXO2= :Anexo2,
                        DS_ANEXO3= :Anexo3,
                        DS_ANEXO4= :Anexo4,
                        DS_CP_POSTAL= :CodigoPostal,
                        CD_CLIENTE_ARMADO_KIT= :ClienteArmadoKit,
                        IM_MINIMO_STOCK= :ValorMinimoStock,
                        ID_UND_FACT_EMPRESA= :IdUnidadFactura,
                        ID_OPERATIVO= :IdOperativo,
                        TP_ALMACENAJE_Y_SEGURO= :TipoDeAlmacenajeYSeguro,
                        ID_DAP= :IdDAP,
                        CD_EMPRESA_DE_CONSOLIDADO= :EmpresaConsolidado,
                        ID_LOCALIDAD= :IdLocalidad,
                        ND_TIPO_FISCAL= :TipoFiscal,
                        CD_LISTA_PRECIO= :ListaPrecio
                        WHERE CD_EMPRESA = :Codigo";

            await _dapper.ExecuteAsync(connection, sql, empresas, transaction: tran);
        }

        public static object GetEmpresaEntity(Empresa empresa)
        {
            return new
            {
                Codigo = empresa.Id,
                Nombre = empresa.Nombre,
                Estado = empresa.EstadoId,
                FechaInsercion = empresa.FechaInsercion,
                FechaModificacion = empresa.FechaModificacion,
                Direccion = empresa.Direccion,
                Telefono = empresa.Telefono,
                NumeroFiscal = empresa.NumeroFiscal,
                TipoFiscal = empresa.TipoFiscalId,
                CodigoPostal = empresa.CodigoPostal,
                ClienteArmadoKit = empresa.CdClienteArmadoKit,
                Anexo1 = empresa.Anexo1,
                Anexo2 = empresa.Anexo2,
                Anexo3 = empresa.Anexo3,
                Anexo4 = empresa.Anexo4,
                IdLocalidad = empresa.IdLocalidad,
                TipoDeAlmacenajeYSeguro = empresa.cdTipoDeAlmacenajeYSeguro,
                ValorMinimoStock = empresa.ValorMinimoStock,
                EmpresaConsolidado = empresa.EmpresaConsolidado,
                ProveedorDevolucion = empresa.ProveedorDevolucion,
                ListaPrecio = empresa.ListaPrecio,
                IdDAP = empresa.IdDAP,
                IdOperativo = empresa.IdOperativo,
                IdUnidadFactura = empresa.IdUnidadFactura,
                CantidadDiasPeriodo = empresa.CantidadDiasPeriodo,
                ValorPallet = empresa.ValorPallet,
                ValorPalletDia = empresa.ValorPalletDia,
                NumeroDominio = $"{ParamManager.PARAM_EMPR}_{empresa.Id}",
                CodigoDominio = ParamManager.PARAM_EMPR,
                DescripcionDominio = "Empresa " + empresa.Id
            };
        }

        public static string GetSqlSelectEmpresa()
        {
            return @"SELECT
                            E.CD_EMPRESA as Id,
                            E.NM_EMPRESA as Nombre,
                            E.CD_SITUACAO as EstadoId,
                            E.DT_ADDROW as FechaInsercion,
                            E.DT_UPDROW as FechaModificacion,
                            E.DS_ENDERECO as Direccion,
                            E.NU_TELEFONE as Telefono,
                            E.CD_CGC_EMPRESA as NumeroFiscal,
                            E.ND_TIPO_FISCAL as TipoFiscalId,
                            E.DS_CP_POSTAL as CodigoPostal,
                            E.CD_CLIENTE_ARMADO_KIT as cdClienteArmadoKit,
                            E.DS_ANEXO1 as Anexo1,
                            E.DS_ANEXO2 as Anexo2,
                            E.DS_ANEXO3 as Anexo3,
                            E.DS_ANEXO4 as Anexo4,
                            E.ID_LOCALIDAD as IdLocalidad,
                            E.TP_ALMACENAJE_Y_SEGURO as cdTipoDeAlmacenajeYSeguro,
                            E.IM_MINIMO_STOCK as ValorMinimoStock,
                            E.CD_EMPRESA_DE_CONSOLIDADO as EmpresaConsolidado,
                            E.CD_FORN_DEVOLUCAO as ProveedorDevolucion,
                            E.CD_LISTA_PRECIO as ListaPrecio,
                            E.ID_DAP as IdDAP,
                            E.ID_OPERATIVO as IdOperativo,
                            E.ID_UND_FACT_EMPRESA as IdUnidadFactura,
                            E.QT_DIAS_POR_PERIODO as CantidadDiasPeriodo,
                            E.VL_POS_PALETE as ValorPallet,
                            E.VL_POS_PALETE_DIA as ValorPalletDia,
                            E.TP_NOTIFICACION AS TipoNotificacionId,
                            E.VL_PAYLOAD_URL AS PayloadUrl,
                            E.FL_LOCKED AS IsLockedId
                        FROM 
                            T_EMPRESA E ";
        }

        public virtual Empresa GetEmpresa(DbConnection connection, Empresa empresa)
        {
            string sql = GetSqlSelectEmpresa() +
                @"WHERE CD_EMPRESA = :Codigo";

            var query = _dapper.Query<Empresa>(connection, sql, param: new
            {
                Codigo = empresa.Id
            }, commandType: CommandType.Text);

            return query.FirstOrDefault();
        }

        public virtual Empresa Map(Empresa empresa, Empresa model = null)
        {
            var idLocalidad = new PaisSubdivisionLocalidadRepository(_context, _application, _userId, _dapper).GetLocalidadId(empresa.MunicipioId, empresa.SubdivisionId).Result?.Id;

            if (model == null)
            {
                empresa.FechaInsercion = DateTime.Now;
            }
            else
            {
                empresa.FechaModificacion = DateTime.Now;
            }

            empresa.Direccion = empresa.Direccion ?? model?.Direccion;
            empresa.Telefono = empresa.Telefono ?? model?.Telefono;
            empresa.TipoFiscal = empresa.TipoFiscal ?? model?.TipoFiscal;
            empresa.NumeroFiscal = empresa.NumeroFiscal ?? model?.NumeroFiscal;
            empresa.CodigoPostal = empresa.CodigoPostal ?? model?.CodigoPostal;
            empresa.CdClienteArmadoKit = empresa.CdClienteArmadoKit ?? model?.CdClienteArmadoKit ?? AgentesPorDefectoEmpresaDb.ClienteArmadoKitCodigo;
            empresa.CodigoPostal = empresa.CodigoPostal ?? model?.CodigoPostal;
            empresa.Anexo1 = empresa.Anexo1 ?? model?.Anexo1;
            empresa.Anexo2 = empresa.Anexo2 ?? model?.Anexo2;
            empresa.Anexo3 = empresa.Anexo3 ?? model?.Anexo3;
            empresa.Anexo4 = empresa.Anexo4 ?? model?.Anexo4;
            empresa.IdLocalidad = idLocalidad ?? model?.IdLocalidad;
            empresa.cdTipoDeAlmacenajeYSeguro = empresa.cdTipoDeAlmacenajeYSeguro ?? model?.cdTipoDeAlmacenajeYSeguro;
            empresa.ValorMinimoStock = empresa.ValorMinimoStock ?? model?.ValorMinimoStock;
            empresa.EmpresaConsolidado = empresa.EmpresaConsolidado ?? model?.EmpresaConsolidado;
            empresa.ProveedorDevolucion = empresa.ProveedorDevolucion ?? model?.ProveedorDevolucion;
            empresa.ListaPrecio = empresa.ListaPrecio ?? model?.ListaPrecio;
            empresa.IdDAP = empresa.IdDAP ?? model?.IdDAP;
            empresa.IdOperativo = empresa.IdOperativo ?? model?.IdOperativo;
            empresa.IdUnidadFactura = empresa.IdUnidadFactura ?? model?.IdUnidadFactura;
            empresa.CantidadDiasPeriodo = empresa.CantidadDiasPeriodo ?? model?.CantidadDiasPeriodo;
            empresa.ValorPallet = empresa.ValorPallet ?? model?.ValorPallet;
            empresa.ValorPalletDia = empresa.ValorPalletDia ?? model?.ValorPalletDia;

            return empresa;
        }

        public virtual async Task<bool> EmpresaAsignada(int empresa, string loginName, CancellationToken cancelToken = default)
        {
            string sql = @"SELECT 1 
                    FROM USERS U 
                    INNER JOIN T_EMPRESA_FUNCIONARIO EF ON U.USERID = EF.USERID 
                    WHERE U.LOGINNAME = :loginName AND EF.CD_EMPRESA = :empresa";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var query = _dapper.Query<int>(connection, sql, param: new
                {
                    loginName = loginName.ToLower(),
                    empresa = empresa
                }, commandType: CommandType.Text);

                return query.Count() > 0;
            }
        }

        public virtual async Task UpdateLock(int empresa, bool isLocked, CancellationToken cancelToken = default)
        {
            string sqlParametro =
                @"SELECT VL_PARAMETRO  
                FROM T_LPARAMETRO_CONFIGURACION
                WHERE CD_PARAMETRO = :codigo and ND_ENTIDAD = :entidad";

            var paramParametro = new DynamicParameters(new
            {
                codigo = ParamManager.WEBHOOK_BLOQUEAR_EMPRESA,
                entidad = $"{ParamManager.PARAM_EMPR}_{empresa}"
            });

            string sql = @"UPDATE T_EMPRESA
                SET FL_LOCKED = :locked
                WHERE CD_EMPRESA = :empresa";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var vlParametro = _dapper.Query<string>(connection, sqlParametro, param: paramParametro, commandType: CommandType.Text).FirstOrDefault();

                if (vlParametro != "N" || !isLocked)
                {
                    await _dapper.ExecuteAsync(connection, sql, param: new
                    {
                        empresa = empresa,
                        locked = isLocked ? "S" : "N",
                    }, commandType: CommandType.Text);
                }
            }
        }

        public virtual Empresa MapInternal(Empresa empresa)
        {
            if (empresa != null)
            {
                switch (empresa.EstadoId)
                {
                    case SituacionDb.Activo: empresa.Estado = EstadoEmpresa.Activo; break;
                    case SituacionDb.Inactivo: empresa.Estado = EstadoEmpresa.Inactivo; break;
                    default: empresa.Estado = EstadoEmpresa.Unknown; break;
                }
            }
            return empresa;
        }
        #endregion
    }
}
