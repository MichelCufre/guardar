using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ImpresionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly ImpresionMapper _mapper;
        protected readonly IDapper _dapper;

        public ImpresionRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ImpresionMapper();
            _dapper = dapper;

        }

        #region Any

        public virtual bool ExisteImpresoraPredio(string impresora, string predio)
        {
            return this._context.T_IMPRESORA.Any(x => x.NU_PREDIO == predio && x.CD_IMPRESORA == impresora);
        }

        public virtual bool ExisteLenguajeImpresion(string cdLenguaje)
        {
            return this._context.T_LENGUAJE_IMPRESION.Any(x => x.CD_LENGUAJE_IMPRESION == cdLenguaje);
        }

        public virtual bool ExisteServidorImpresion(int cdServidor)
        {
            return this._context.T_IMPRESORA_SERVIDOR.Any(x => x.CD_SERVIDOR == cdServidor);
        }

        #endregion

        #region Add

        public virtual int Add(Impresion impresion)
        {
            impresion.Id = this._context.GetNextSequenceValueInt(_dapper, "S_NU_IMPRESION");

            this._context.T_IMPRESION.Add(this._mapper.MapToEntity(impresion));

            return impresion.Id;
        }

        public virtual void AddImpresion(Impresion impresion)
        {
            this._context.T_IMPRESION.Add(this._mapper.MapToEntity(impresion));
        }

        public virtual int GetSecuenciaImpresion()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_NU_IMPRESION");
        }

        public virtual void AddDetalleImpresion(DetalleImpresion detalle)
        {
            this._context.T_DET_IMPRESION.Add(this._mapper.MapToEntityDetalleImpresion(detalle));
        }

        public virtual void AddServidorImpresion(ServidorImpresion servidor)
        {
            this._context.T_IMPRESORA_SERVIDOR.Add(this._mapper.MapToEntityServidorImpresion(servidor));
        }

        #endregion

        #region Get

        public virtual Impresion GetImpresion(int numero)
        {
            return this._mapper.MapToObject(this._context.T_IMPRESION
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_IMPRESION == numero));
        }

        public virtual Impresion GetImpresionWithDetail(int numero)
        {
            return this._mapper.MapToObject(this._context.T_IMPRESION
                .Include("T_DET_IMPRESION")
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_IMPRESION == numero));
        }

        public virtual Impresion GetImpresion(int funcionario, string predio)
        {
            return this._mapper.MapToObject(this._context.T_IMPRESION
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_FUNCIONARIO == funcionario && x.NU_PREDIO == predio));
        }

        public virtual Impresion GetImpresion(string tipoEtiqueta)
        {
            return this._mapper.MapToObject(this._context.T_IMPRESION
                .AsNoTracking()
                .FirstOrDefault(x => x.TP_LABEL == tipoEtiqueta));
        }

        public virtual Impresion ObtenerImpresoraUltimaImpresion(int funcionario, string predio)
        {
            T_IMPRESION entity;

            if (predio.Equals(GeneralDb.PredioSinDefinir))
                entity = this._context.T_IMPRESION.Where(x => x.CD_FUNCIONARIO == funcionario).OrderByDescending(x => x.DT_GENERADO).FirstOrDefault();
            else
                entity = this._context.T_IMPRESION.Where(x => x.CD_FUNCIONARIO == funcionario && x.NU_PREDIO == predio).OrderByDescending(x => x.DT_GENERADO).FirstOrDefault();

            if (entity == null)
                return null;

            return this._mapper.MapToObject(entity);
        }

        public virtual LenguajeImpresion GetLenguajeImpresion(string codigo)
        {
            T_LENGUAJE_IMPRESION entity = this._context.T_LENGUAJE_IMPRESION.FirstOrDefault(x => x.CD_LENGUAJE_IMPRESION == codigo);

            if (entity == null)
                return null;

            return this._mapper.MapToObjectLenguajeImpresion(entity);
        }

        public virtual List<LenguajeImpresion> GetLenguajesImpresion()
        {
            var lenguajesImpresion = new List<LenguajeImpresion>();
            var lenguajes = this._context.T_LENGUAJE_IMPRESION.AsNoTracking().ToList();

            foreach (var entity in lenguajes)
            {
                lenguajesImpresion.Add(this._mapper.MapToObjectLenguajeImpresion(entity));
            }

            return lenguajesImpresion;
        }

        public virtual List<EtiquetaEstiloLenguaje> GetEstiloByLenguaje(string lenguaje, string tipo)
        {
            var estilos = new List<EtiquetaEstiloLenguaje>();
            var entities = this._context.V_ESTILOS_LENGUAJES
                .AsNoTracking()
                .Where(x => x.FL_HABILITADO == "S" && x.CD_LENGUAJE_IMPRESION == lenguaje && x.TP_LABEL == tipo);

            foreach (var entity in entities)
            {
                estilos.Add(this._mapper.MapToObject(entity));
            }

            return estilos;
        }

        public virtual List<EtiquetaEstiloLenguaje> GetEstiloByTipo(string tipo)
        {
            var estilos = new List<EtiquetaEstiloLenguaje>();
            var entities = this._context.V_ESTILOS_LENGUAJES
                .AsNoTracking()
                .Where(x => x.FL_HABILITADO == "S" && x.TP_LABEL == tipo);

            foreach (var entity in entities)
            {
                estilos.Add(this._mapper.MapToObject(entity));
            }

            return estilos;
        }

        public virtual List<EtiquetaEstiloLenguaje> GetEstiloByLenguajeContenedor(string lenguaje, string tipo, string tipoContenedor)
        {
            var estilos = new List<EtiquetaEstiloLenguaje>();
            var entities = this._context.V_ESTILOS_LENGUAJES
                .AsNoTracking()
                .Where(x => x.FL_HABILITADO == "S" && x.CD_LENGUAJE_IMPRESION == lenguaje && x.TP_LABEL == tipo && x.TP_CONTENEDOR == tipoContenedor);

            foreach (var entity in entities)
            {
                estilos.Add(this._mapper.MapToObject(entity));
            }

            return estilos;
        }

        public virtual List<EtiquetaEstiloLenguaje> GetEstiloByTipoContenedor(string tipo, string tipoContenedor)
        {
            var estilos = new List<EtiquetaEstiloLenguaje>();
            var entities = this._context.V_ESTILOS_LENGUAJES
                .AsNoTracking()
                .Where(x => x.FL_HABILITADO == "S" && x.TP_LABEL == tipo && x.TP_CONTENEDOR == tipoContenedor);

            foreach (var entity in entities)
            {
                estilos.Add(this._mapper.MapToObject(entity));
            }

            return estilos;
        }

        public virtual List<TipoContenedor> GetTiposContenedoresImpresion()
        {
            return this._context.V_ESTILOS_LENGUAJES
                .AsNoTracking()
                .Where(x => x.FL_HABILITADO == "S" && x.TP_CONTENEDOR != null)
                .GroupBy(x => x.TP_CONTENEDOR)
                .Select(x => new TipoContenedor
                {
                    Id = x.Key,
                    Descripcion = x.Min(t => t.DS_TIPO_CONTENEDOR)
                })
                .ToList();
        }

        public virtual ServidorImpresion GetServidorImpresion(int codigoServidor)
        {
            T_IMPRESORA_SERVIDOR entity = this._context.T_IMPRESORA_SERVIDOR.FirstOrDefault(x => x.CD_SERVIDOR == codigoServidor);

            if (entity == null)
                return null;

            return this._mapper.MapToObjectServidorImpresion(entity);
        }

        public virtual List<ServidorImpresion> GetServidoresImpresion()
        {
            var servidoresImpresion = new List<ServidorImpresion>();
            var servidores = this._context.T_IMPRESORA_SERVIDOR.AsNoTracking().ToList();

            foreach (var entity in servidores)
            {
                servidoresImpresion.Add(this._mapper.MapToObjectServidorImpresion(entity));
            }

            return servidoresImpresion;
        }

        public virtual void GetDiccionarioInformacionPedido(Dictionary<string, string> claves, string numeroPedido, string codigoCliente, int codigoEmpresa)
        {
            var pedido = _context.T_PEDIDO_SAIDA.FirstOrDefault(x => x.NU_PEDIDO == numeroPedido && x.CD_CLIENTE == codigoCliente && x.CD_EMPRESA == codigoEmpresa);

            PropertyInfo[] propertiesPedido = typeof(T_PEDIDO_SAIDA).GetProperties();
            foreach (PropertyInfo p in propertiesPedido)
            {
                var propertyValue = p.GetValue(pedido);
                claves.Add($"T_PEDIDO_SAIDA.{p.Name}", propertyValue?.ToString());
            }

            var tipoPedido = _context.T_TIPO_PEDIDO.FirstOrDefault(x => x.TP_PEDIDO == pedido.TP_PEDIDO);
            PropertyInfo[] propertiesTipoPedido = typeof(T_TIPO_PEDIDO).GetProperties();
            foreach (PropertyInfo p in propertiesTipoPedido)
            {
                var propertyValue = p.GetValue(tipoPedido);
                claves.Add($"T_TIPO_PEDIDO.{p.Name}", propertyValue?.ToString());
            }
        }

        public virtual void GetDiccionarioInformacionCliente(Dictionary<string, string> claves, string codigoCliente, int codigoEmpresa)
        {
            var cliente = _context.T_CLIENTE.FirstOrDefault(x => x.CD_CLIENTE == codigoCliente && x.CD_EMPRESA == codigoEmpresa);

            PropertyInfo[] properties = typeof(T_CLIENTE).GetProperties();
            foreach (PropertyInfo p in properties)
            {
                var propertyValue = p.GetValue(cliente);
                claves.Add($"T_CLIENTE.{p.Name}", propertyValue?.ToString());
            }
        }

        public virtual void GetDiccionarioInformacionEmpresa(Dictionary<string, string> claves, int codigoEmpresa)
        {
            var empresa = _context.T_EMPRESA.FirstOrDefault(x => x.CD_EMPRESA == codigoEmpresa);

            PropertyInfo[] properties = typeof(T_EMPRESA).GetProperties();
            foreach (PropertyInfo p in properties)
            {
                var propertyValue = p.GetValue(empresa);
                claves.Add($"T_EMPRESA.{p.Name}", propertyValue?.ToString());
            }
        }

        #endregion

        #region Update

        public virtual void ActualizarImpresion(int numImpresion, int cantRegistros)
        {
            T_IMPRESION entity = this._context.T_IMPRESION.FirstOrDefault(x => x.NU_IMPRESION == numImpresion);

            entity.QT_REGISTROS = cantRegistros;

            T_IMPRESION attachedEntity = _context.T_IMPRESION.Local.FirstOrDefault(x => x.NU_IMPRESION == entity.NU_IMPRESION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_IMPRESION.Attach(entity);
                _context.Entry<T_IMPRESION>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateServidorImpresion(ServidorImpresion servidor)
        {
            T_IMPRESORA_SERVIDOR entity = this._mapper.MapToEntityServidorImpresion(servidor);

            T_IMPRESORA_SERVIDOR attachedEntity = _context.T_IMPRESORA_SERVIDOR.Local.FirstOrDefault(w => w.CD_SERVIDOR == entity.CD_SERVIDOR);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_IMPRESORA_SERVIDOR.Attach(entity);
                _context.Entry<T_IMPRESORA_SERVIDOR>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteServidorImpresion(int codigoServidor)
        {
            T_IMPRESORA_SERVIDOR entity = this._context.T_IMPRESORA_SERVIDOR
                .FirstOrDefault(x => x.CD_SERVIDOR == codigoServidor);
            T_IMPRESORA_SERVIDOR attachedEntity = _context.T_IMPRESORA_SERVIDOR.Local
                .FirstOrDefault(w => w.CD_SERVIDOR == entity.CD_SERVIDOR);

            if (attachedEntity != null)
            {
                _context.T_IMPRESORA_SERVIDOR.Remove(attachedEntity);
            }
            else
            {
                _context.T_IMPRESORA_SERVIDOR.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual async Task<List<Impresion>> GetImpresionesPendientes()
        {
            var parms = new DynamicParameters();
            var impresiones = await Task.FromResult(_dapper.GetAll<Impresion>(
                @"SELECT 
                    imp.NU_IMPRESION AS Id,
                    imp.CD_ESTADO AS Estado,
                    imp.CD_FUNCIONARIO AS Usuario,
                    imp.NM_IMPRESORA AS NombreImpresora,
                    imp.CD_IMPRESORA AS CodigoImpresora
                FROM T_IMPRESION imp
                WHERE imp.CD_ESTADO = 'SIMPENV'
                ORDER BY imp.NU_IMPRESION ASC",
                parms,
                commandType: CommandType.Text));

            if (impresiones == null)
                return new List<Impresion>();

            foreach (var impresion in impresiones)
            {
                var parmsDetalle = new DynamicParameters(new
                {
                    id = impresion.Id.ToString()
                });

                impresion.Detalles = await Task.FromResult(_dapper.GetAll<DetalleImpresion>(
                    @"SELECT 
                        imp.NU_IMPRESION AS NumeroImpresion,
                        imp.NU_REGISTRO AS Registro,
                        imp.VL_DATO AS Contenido,
                        imp.CD_ESTADO AS Estado
                    FROM T_DET_IMPRESION imp
                    WHERE imp.NU_IMPRESION = :id
                    ORDER BY imp.NU_REGISTRO ASC",
                    parmsDetalle,
                    commandType: CommandType.Text));
            }

            return impresiones;
        }

        public virtual async Task UpdateImpresion(Impresion impresion)
        {
            var error = impresion.Error ?? string.Empty;
            error = string.IsNullOrEmpty(error) ? string.Empty : error.Substring(0, Math.Min(error.Length, 200));

            var param = new DynamicParameters(new
            {
                procesado = impresion?.Procesado,
                error = error,
                estado = impresion.Estado,
                impresion = impresion.Id
            });

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();
                await _dapper.ExecuteAsync(connection, @"UPDATE T_IMPRESION 
                    SET CD_ESTADO = :estado,
                        DS_ERROR = :error,
                        DT_PROCESADO = :procesado
                    WHERE NU_IMPRESION = :impresion", param);
            }
        }

        public virtual async Task UpdateDetalleImpresion(Impresion impresion, DetalleImpresion detalle)
        {
            var error = detalle.Error ?? string.Empty;
            error = string.IsNullOrEmpty(error) ? string.Empty : error.Substring(0, Math.Min(error.Length, 200));

            var param = new DynamicParameters(new
            {
                procesado = detalle?.FechaProcesado,
                error = error,
                estado = detalle.Estado,
                impresion = impresion.Id,
                registro = detalle.Registro
            });

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();
                await _dapper.ExecuteAsync(connection, @"UPDATE T_DET_IMPRESION 
                    SET CD_ESTADO = :estado, 
                        DS_ERROR = :error, 
                        DT_PROCESADO = :procesado 
                    WHERE NU_IMPRESION = :impresion AND NU_REGISTRO = :registro", param);
            }
        }

        public virtual void RegenerarImpresion(int nuImpresion, string estado, int userId)
        {
            var impresion = _context.T_IMPRESION
                .FirstOrDefault(i => i.NU_IMPRESION == nuImpresion);

            impresion.CD_ESTADO = estado;
            impresion.DS_ERROR = "";
            impresion.CD_FUNCIONARIO = userId;
            impresion.DT_PROCESADO = DateTime.Now;

            _context.T_DET_IMPRESION
                .Where(d => d.NU_IMPRESION == nuImpresion)
                .ExecuteUpdate(setters => setters
                    .SetProperty(d => d.CD_ESTADO, estado)
                    .SetProperty(d => d.DS_ERROR, "")
                    .SetProperty(d => d.DT_PROCESADO, DateTime.Now));
        }

        #endregion

    }
}
