using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.Inventario;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class InventarioRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly InventarioMapper _mapper;
        protected readonly IDapper _dapper;

        public static List<string> ESTADOS_INVENTARIO_INACTIVO = new List<string>() { EstadoInventario.Cancelado, EstadoInventario.Cerrado };

        public InventarioRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new InventarioMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool TieneConteoFinalizado(decimal nroInventario)
        {
            return this._context.T_INVENTARIO_ENDERECO_DET
                .Include("T_INVENTARIO_ENDERECO")
                .AsNoTracking()
                .Any(x => x.T_INVENTARIO_ENDERECO.NU_INVENTARIO == nroInventario &&
                    (x.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF ||
                     x.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_OK));
        }

        public virtual bool HayConteosFinalizadosOActualizados(decimal nroInventario)
        {
            return _context.T_INVENTARIO_ENDERECO_DET
                .Any(x => x.T_INVENTARIO_ENDERECO.NU_INVENTARIO == nroInventario
                    && (x.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO ||
                        x.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF ||
                        x.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_OK));
        }

        public virtual bool TieneConteoPendiente(decimal nroInventario)
        {
            return this._context.T_INVENTARIO_ENDERECO_DET
                .AsNoTracking()
                .Any(x => x.T_INVENTARIO_ENDERECO.NU_INVENTARIO == nroInventario
                    && x.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR);
        }

        public virtual bool TieneConteos(decimal nuInventario)
        {
            return _context.T_INVENTARIO_ENDERECO_DET
               .Include("T_INVENTARIO_ENDERECO")
               .AsNoTracking()
               .Where(x => x.T_INVENTARIO_ENDERECO.NU_INVENTARIO == nuInventario)
               .Count() > 0;
        }

        public virtual bool IsUbicacionInInventario(string cdEndereco, decimal nuInventario)
        {
            var ubicacionInInventario = this._context.T_INVENTARIO_ENDERECO.Local
                .Any(x => x.CD_ENDERECO == cdEndereco
                    && x.NU_INVENTARIO == nuInventario
                    && x.ND_ESTADO_INVENTARIO_ENDERECO == EstadoInventarioUbicacionDb.ND_ESTADO_ENDERECO_PENDIENTE);

            ubicacionInInventario = ubicacionInInventario || this._context.T_INVENTARIO_ENDERECO
                    .AsNoTracking()
                    .Any(x => x.CD_ENDERECO == cdEndereco
                        && x.NU_INVENTARIO == nuInventario
                        && x.ND_ESTADO_INVENTARIO_ENDERECO == EstadoInventarioUbicacionDb.ND_ESTADO_ENDERECO_PENDIENTE);

            return ubicacionInInventario;
        }

        public virtual bool RegistroEnOtroInventario(IInventario inventario, string ubicacion, string producto, int empresa, long? nuLpn = null, int? idDetalleLpn = null)
        {
            bool existeRegistro = false;
            if (inventario.TipoInventario == TipoInventario.Ubicacion)
            {
                return RegistroEnOtroInventarioUbicacion(inventario, ubicacion, tpInventarioUbicacion: true);
            }
            else if (inventario.TipoInventario == TipoInventario.Registro)
            {
                existeRegistro = RegistroEnOtroInventarioUbicacion(inventario, ubicacion);

                if (!existeRegistro)
                    return RegistroEnOtroInventarioRegistro(inventario, ubicacion, producto, empresa, tpInventarioRegistro: true);
            }
            else if (inventario.TipoInventario == TipoInventario.Lpn)
            {
                existeRegistro = RegistroEnOtroInventarioUbicacion(inventario, ubicacion);

                if (!existeRegistro)
                {
                    existeRegistro = RegistroEnOtroInventarioRegistro(inventario, ubicacion, producto, empresa);

                    if (!existeRegistro)
                        return RegistroEnOtroInventarioLpn(inventario, ubicacion, producto, empresa, nuLpn, tpInventarioLpn: true);
                }
            }
            else if (inventario.TipoInventario == TipoInventario.DetalleLpn)
            {
                existeRegistro = RegistroEnOtroInventarioUbicacion(inventario, ubicacion);

                if (!existeRegistro)
                {
                    existeRegistro = RegistroEnOtroInventarioRegistro(inventario, ubicacion, producto, empresa);

                    if (!existeRegistro)
                    {
                        existeRegistro = RegistroEnOtroInventarioLpn(inventario, ubicacion, producto, empresa, nuLpn);

                        if (!existeRegistro)
                            return RegistroEnOtroInventarioDetalleLpn(inventario, ubicacion, producto, empresa, nuLpn, idDetalleLpn, tpInventarioDetalleLpn: true);
                    }
                }

            }

            return existeRegistro;
        }

        public virtual bool RegistroEnOtroInventarioUbicacion(IInventario inventario, string ubicacion, bool tpInventarioUbicacion = false)
        {
            return _context.T_INVENTARIO_ENDERECO_DET
                .Include("T_INVENTARIO_ENDERECO")
                .Include("T_INVENTARIO_ENDERECO.T_INVENTARIO")
                .Any(x => !ESTADOS_INVENTARIO_INACTIVO.Contains(x.T_INVENTARIO_ENDERECO.T_INVENTARIO.ND_ESTADO_INVENTARIO)
                    && x.T_INVENTARIO_ENDERECO.NU_INVENTARIO != inventario.NumeroInventario
                    && x.T_INVENTARIO_ENDERECO.CD_ENDERECO == ubicacion
                    && (!tpInventarioUbicacion ?
                       (x.T_INVENTARIO_ENDERECO.T_INVENTARIO.TP_INVENTARIO == TipoInventario.Ubicacion &&
                        x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO) : true));
        }

        public virtual bool RegistroEnOtroInventarioRegistro(IInventario inventario, string ubicacion, string producto, int empresa, bool tpInventarioRegistro = false)
        {
            return _context.T_INVENTARIO_ENDERECO_DET
                .Include("T_INVENTARIO_ENDERECO")
                .Include("T_INVENTARIO_ENDERECO.T_INVENTARIO")
                .Any(x => !ESTADOS_INVENTARIO_INACTIVO.Contains(x.T_INVENTARIO_ENDERECO.T_INVENTARIO.ND_ESTADO_INVENTARIO)
                    && x.T_INVENTARIO_ENDERECO.NU_INVENTARIO != inventario.NumeroInventario
                    && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO
                    && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_OK
                    && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_REC
                    && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_CONTADO
                    && x.T_INVENTARIO_ENDERECO.CD_ENDERECO == ubicacion
                    && x.CD_PRODUTO == producto
                    && x.CD_EMPRESA == empresa
                    && (!tpInventarioRegistro ? x.T_INVENTARIO_ENDERECO.T_INVENTARIO.TP_INVENTARIO == TipoInventario.Registro : true));
        }

        public virtual bool RegistroEnOtroInventarioLpn(IInventario inventario, string ubicacion, string producto, int empresa, long? nuLpn, bool tpInventarioLpn = false)
        {
            return _context.T_INVENTARIO_ENDERECO_DET
                .Include("T_INVENTARIO_ENDERECO")
                .Include("T_INVENTARIO_ENDERECO.T_INVENTARIO")
                .Any(x => !ESTADOS_INVENTARIO_INACTIVO.Contains(x.T_INVENTARIO_ENDERECO.T_INVENTARIO.ND_ESTADO_INVENTARIO)
                    && x.T_INVENTARIO_ENDERECO.NU_INVENTARIO != inventario.NumeroInventario
                    && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO
                    && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_OK
                    && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_REC
                    && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_CONTADO
                    && x.T_INVENTARIO_ENDERECO.CD_ENDERECO == ubicacion
                    && x.CD_PRODUTO == producto
                    && x.CD_EMPRESA == empresa
                    && (nuLpn.HasValue ? x.NU_LPN == nuLpn.Value : true)
                    && (!tpInventarioLpn ? x.T_INVENTARIO_ENDERECO.T_INVENTARIO.TP_INVENTARIO == TipoInventario.Lpn : true));
        }

        public virtual bool RegistroEnOtroInventarioDetalleLpn(IInventario inventario, string ubicacion, string producto, int empresa, long? nuLpn, int? idDetalleLpn, bool tpInventarioDetalleLpn = false)
        {
            return _context.T_INVENTARIO_ENDERECO_DET
            .Include("T_INVENTARIO_ENDERECO")
            .Include("T_INVENTARIO_ENDERECO.T_INVENTARIO")
            .Any(x => !ESTADOS_INVENTARIO_INACTIVO.Contains(x.T_INVENTARIO_ENDERECO.T_INVENTARIO.ND_ESTADO_INVENTARIO)
                && x.T_INVENTARIO_ENDERECO.NU_INVENTARIO != inventario.NumeroInventario
                && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO
                && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_OK
                && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_REC
                && x.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_CONTADO
                && x.T_INVENTARIO_ENDERECO.CD_ENDERECO == ubicacion
                && x.CD_PRODUTO == producto
                && x.CD_EMPRESA == empresa
                && (nuLpn.HasValue ? x.NU_LPN == nuLpn.Value : true)
                && (idDetalleLpn.HasValue ? x.ID_LPN_DET == idDetalleLpn.Value : true)
                && (!tpInventarioDetalleLpn ? x.T_INVENTARIO_ENDERECO.T_INVENTARIO.TP_INVENTARIO == TipoInventario.DetalleLpn : true));
        }

        public virtual bool TieneUbicaciones(decimal nuInventario)
        {
            return _context.T_INVENTARIO_ENDERECO
                .AsNoTracking()
                .Where(x => x.NU_INVENTARIO == nuInventario)
                .Count() > 0;
        }

        public virtual bool TieneConteosFinDif(decimal nuInventario)
        {
            return _context.T_INVENTARIO_ENDERECO_DET
                .AsNoTracking()
                .Any(x => x.T_INVENTARIO_ENDERECO.NU_INVENTARIO == nuInventario
                    && (x.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF
                        || x.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR));
        }

        public virtual bool TieneErroresEnUbicaciones(decimal nuInventario)
        {
            return _context.T_INV_ENDERECO_DET_ERROR
                .AsNoTracking()
                .Any(x => x.NU_INVENTARIO == nuInventario);
        }

        public virtual bool AnyInventarioEnUbicacion(string idEndereco)
        {
            return this._context.T_INVENTARIO_ENDERECO.Any(w => w.CD_ENDERECO == idEndereco);
        }

        #endregion

        #region Get

        public virtual IInventario GetInventario(decimal nuInventario)
        {
            T_INVENTARIO inventarioEndeEntity = this._context.T_INVENTARIO
                .Include("T_INVENTARIO_ENDERECO")
                .Include("T_INVENTARIO_ENDERECO.T_INVENTARIO_ENDERECO_DET")
                .Where(x => x.NU_INVENTARIO == nuInventario)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapToInventario(inventarioEndeEntity);
        }

        public virtual InventarioUbicacion GetInventarioEndereco(decimal nuInventarioEndereco)
        {
            T_INVENTARIO_ENDERECO inventarioEndeEntity = this._context.T_INVENTARIO_ENDERECO.Include("T_INVENTARIO_ENDERECO_DET").Where(x => x.NU_INVENTARIO_ENDERECO == nuInventarioEndereco)
                .AsNoTracking().FirstOrDefault();

            if (inventarioEndeEntity == null)
                throw new ValidationFailedException("INV_Db_Error_InventarioEnderecoNotExist");

            return this._mapper.MapToInventarioEndereco(inventarioEndeEntity);
        }

        public virtual InventarioUbicacion GetInventarioEnderecoByInvAndUbicacion(decimal nuInventario, string ubicacion)
        {
            var inventarioEndeEntity = this._context.T_INVENTARIO_ENDERECO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_INVENTARIO == nuInventario
                    && x.CD_ENDERECO == ubicacion);

            if (inventarioEndeEntity == null)
                inventarioEndeEntity = this._context.T_INVENTARIO_ENDERECO.Local.FirstOrDefault(x => x.NU_INVENTARIO == nuInventario && x.CD_ENDERECO == ubicacion);

            if (inventarioEndeEntity == null)
                throw new ValidationFailedException("INV_Db_Error_InventarioEnderecoNotExist");

            return this._mapper.MapToInventarioEndereco(inventarioEndeEntity);
        }

        public virtual InventarioUbicacionDetalle GetInventarioEnderecoDetalle(decimal nuInventarioEnderecoDet)
        {
            var entity = this._context.T_INVENTARIO_ENDERECO_DET
                .Include("T_INVENTARIO_ENDERECO")
                .FirstOrDefault(x => x.NU_INVENTARIO_ENDERECO_DET == nuInventarioEnderecoDet);

            if (entity == null)
                throw new ValidationFailedException("INV_Db_Error_InventarioEnderecoDetalleNotExist");

            return this._mapper.MapToInventarioEnderecoDetalle(entity);
        }

        public virtual List<InventarioUbicacionDetalle> GetsInvEnderecoDetByNuInv(decimal nuInv)
        {
            return _context.T_INVENTARIO_ENDERECO_DET
                .Include("T_INVENTARIO_ENDERECO")
                .Where(x => x.T_INVENTARIO_ENDERECO.T_INVENTARIO.NU_INVENTARIO == nuInv)
                .AsNoTracking()
                .Select(x => _mapper.MapToInventarioEnderecoDetalle(x))
                .ToList();
        }

        public virtual List<InventarioUbicacionDetalle> GetsInvEnderecoDetallesFinalizados(decimal nroInventario)
        {
            return _context.T_INVENTARIO_ENDERECO_DET
                .Include("T_INVENTARIO_ENDERECO")
                .Where(x => x.T_INVENTARIO_ENDERECO.NU_INVENTARIO == nroInventario
                    && x.CD_PRODUTO != null
                    && (x.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF
                        || x.ND_ESTADO_INV_ENDERECO_DET == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_OK))
                .AsNoTracking()
                .Select(x => _mapper.MapToInventarioEnderecoDetalle(x))
                .ToList();
        }

        public virtual List<InventarioSelectRegistroLpn> GetDetallesFinalesPorRegistro(IInventario inventario, InventarioSelectRegistroLpn registro)
        {
            return _context.V_INV417_REG_DISP
                .Where(i => i.NU_INVENTARIO == inventario.NumeroInventario
                    && i.CD_ENDERECO == registro.Ubicacion
                    && i.CD_EMPRESA == registro.Empresa
                    && i.CD_PRODUTO == registro.Producto
                    && i.CD_FAIXA == registro.Faixa
                    && i.NU_IDENTIFICADOR == registro.Identificador
                    && i.QT_ESTOQUE > 0
                    && (inventario.ExcluirSueltos ? (i.NU_LPN != "-" && !string.IsNullOrEmpty(i.NU_LPN)) : true)
                    && (inventario.ExcluirLpns ? (i.NU_LPN == "-" || string.IsNullOrEmpty(i.NU_LPN)) : true))
                .Select(i => _mapper.Map(i))
                .ToList();
        }

        public virtual List<InventarioSelectRegistroLpn> GetDetallesFinalesPorLpn(InventarioSelectRegistroLpn registro)
        {
            return _context.V_INV417_REG_DISP
                .Where(i => i.NU_INVENTARIO == registro.NuInventario
                    && i.CD_ENDERECO == registro.Ubicacion
                    && i.CD_EMPRESA == registro.Empresa
                    && i.CD_PRODUTO == registro.Producto
                    && i.CD_FAIXA == registro.Faixa
                    && i.NU_IDENTIFICADOR == registro.Identificador
                    && i.NU_LPN == registro.NroLpn
                    && i.QT_ESTOQUE > 0)
                .Select(i => _mapper.Map(i))
                .ToList();
        }

        public virtual List<InventarioUbicacionDetalleAtributo> GetAtributosDetalle(decimal id)
        {
            return _context.T_INVENTARIO_ENDERECO_DET_ATR
                .Where(a => a.NU_INVENTARIO_ENDERECO_DET == id)
                .Select(i => _mapper.Map(i))
                .ToList();
        }

        public virtual decimal GetNextNuInventario()
        {
            return this._context.GetNextSequenceValueDecimal(_dapper, Secuencias.S_INVENTARIO);
        }

        public virtual decimal GetNextNuInventarioEndereco()
        {
            return this._context.GetNextSequenceValueDecimal(_dapper, Secuencias.S_INVENTARIO_ENDERECO);
        }

        public virtual decimal GetNextNuInventarioEnderecoDet()
        {
            return this._context.GetNextSequenceValueDecimal(_dapper, Secuencias.S_INVENTARIO_ENDERECO_DET);
        }

        public virtual long GetNextNuInstanciaConteo()
        {
            return this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_NU_INSTANCIA_CONTEO);
        }

        public virtual long? GetInstanciaConteo(string cdEndereco, decimal nuInventario)
        {
            return (_context.T_INVENTARIO_ENDERECO_DET
                .Where(x => x.T_INVENTARIO_ENDERECO.T_INVENTARIO.NU_INVENTARIO == nuInventario
                    && x.T_INVENTARIO_ENDERECO.T_INVENTARIO.ND_ESTADO_INVENTARIO == EstadoInventario.EnProceso
                    && x.T_INVENTARIO_ENDERECO.CD_ENDERECO == cdEndereco
                    && x.NU_INSTANCIA_CONTEO != null)
                .OrderByDescending(x => x.NU_INSTANCIA_CONTEO)
                .Select(x => x.NU_INSTANCIA_CONTEO)
                ?.FirstOrDefault());
        }

        #endregion

        #region Add

        public virtual void AddInventario(IInventario objInterfaz)
        {
            T_INVENTARIO entity = this._mapper.MapFromInventario(objInterfaz);
            this._context.T_INVENTARIO.Add(entity);
        }

        public virtual void AddInventarioEndereco(InventarioUbicacion objInterfaz)
        {
            T_INVENTARIO_ENDERECO entity = this._mapper.MapFromInventarioEndereco(objInterfaz);
            this._context.T_INVENTARIO_ENDERECO.Add(entity);
        }

        public virtual void AddInventarioEnderecoDetalle(InventarioUbicacionDetalle objEntity)
        {
            T_INVENTARIO_ENDERECO_DET entity = this._mapper.MapFromInventarioEnderecoDetalle(objEntity);
            this._context.T_INVENTARIO_ENDERECO_DET.Add(entity);
        }

        public virtual void AddInventarioEnderecoDetalleAtr(InventarioUbicacionDetalleAtributo objEntity)
        {
            T_INVENTARIO_ENDERECO_DET_ATR entity = this._mapper.Map(objEntity);
            this._context.T_INVENTARIO_ENDERECO_DET_ATR.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateInventario(IInventario inventario)
        {
            inventario.FechaModificacion = DateTime.Now;

            T_INVENTARIO entity = this._mapper.MapFromInventario(inventario, false);
            T_INVENTARIO attachedEntity = _context.T_INVENTARIO.Local
                .FirstOrDefault(w => w.NU_INVENTARIO == entity.NU_INVENTARIO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_INVENTARIO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateInventarioEndereco(InventarioUbicacion inventarioEndereco)
        {
            T_INVENTARIO_ENDERECO entity = this._mapper.MapFromInventarioEndereco(inventarioEndereco);
            T_INVENTARIO_ENDERECO attachedEntity = _context.T_INVENTARIO_ENDERECO.Local
                .FirstOrDefault(w => w.NU_INVENTARIO_ENDERECO == entity.NU_INVENTARIO_ENDERECO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_INVENTARIO_ENDERECO.Attach(entity);
                _context.Entry<T_INVENTARIO_ENDERECO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateInventarioEnderecoDetalle(InventarioUbicacionDetalle enderecoDet)
        {
            enderecoDet.FechaModificacion = DateTime.Now;

            T_INVENTARIO_ENDERECO_DET entity = this._mapper.MapFromInventarioEnderecoDetalle(enderecoDet);
            T_INVENTARIO_ENDERECO_DET attachedEntity = _context.T_INVENTARIO_ENDERECO_DET.Local
                .FirstOrDefault(w => w.NU_INVENTARIO_ENDERECO_DET == entity.NU_INVENTARIO_ENDERECO_DET);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_INVENTARIO_ENDERECO_DET.Attach(entity);
                _context.Entry<T_INVENTARIO_ENDERECO_DET>(entity).State = EntityState.Modified;
            }

        }

        public virtual void FinalizarUbicacionesInventario(decimal nuInventario, long nuTransaccion)
        {
            _context.T_INVENTARIO_ENDERECO
                .Where(d => d.NU_INVENTARIO == nuInventario)
                .ExecuteUpdate(setters => setters
                    .SetProperty(d => d.ND_ESTADO_INVENTARIO_ENDERECO, EstadoInventarioUbicacionDb.ND_ESTADO_ENDERECO_FINALIZADO)
                    .SetProperty(d => d.NU_TRANSACCION, nuTransaccion));
        }

        #endregion

        #region Remove

        public virtual void DeleteInventarioEndereco(InventarioUbicacion objInterfaz)
        {
            T_INVENTARIO_ENDERECO entity = this._mapper.MapFromInventarioEndereco(objInterfaz);
            T_INVENTARIO_ENDERECO attachedEntity = _context.T_INVENTARIO_ENDERECO.Local
                .FirstOrDefault(x => x.NU_INVENTARIO_ENDERECO == entity.NU_INVENTARIO_ENDERECO);

            if (attachedEntity != null)
            {
                _context.T_INVENTARIO_ENDERECO.Remove(attachedEntity);
            }
            else
            {
                _context.T_INVENTARIO_ENDERECO.Attach(entity);
                _context.T_INVENTARIO_ENDERECO.Remove(entity);
            }
        }

        public virtual void DeleteInventarioEnderecoDetalle(InventarioUbicacionDetalle objInterfaz)
        {
            T_INVENTARIO_ENDERECO_DET entity = this._mapper.MapFromInventarioEnderecoDetalle(objInterfaz);
            T_INVENTARIO_ENDERECO_DET attachedEntity = _context.T_INVENTARIO_ENDERECO_DET.Local
                .FirstOrDefault(x => x.NU_INVENTARIO_ENDERECO_DET == entity.NU_INVENTARIO_ENDERECO_DET);

            if (attachedEntity != null)
            {
                _context.T_INVENTARIO_ENDERECO_DET.Remove(attachedEntity);
            }
            else
            {
                _context.T_INVENTARIO_ENDERECO_DET.Attach(entity);
                _context.T_INVENTARIO_ENDERECO_DET.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual int GetCantidadConteosPendientesByInventario(decimal nuInventario, string estado)
        {
            string sql = @" 
                SELECT COUNT(*) 
                FROM T_INVENTARIO_ENDERECO ie
                INNER JOIN T_INVENTARIO_ENDERECO_DET iet ON ie.NU_INVENTARIO_ENDERECO = iet.NU_INVENTARIO_ENDERECO
                WHERE ie.NU_INVENTARIO = :nuInventario AND iet.ND_ESTADO_INV_ENDERECO_DET = :estado ";

            return _dapper.Query<int>(_context.Database.GetDbConnection(), sql, param: new
            {
                nuInventario = nuInventario,
                estado = estado
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).FirstOrDefault();
        }

        public virtual int GetCantidadDetallesFinalizadosByInventario(decimal nuInventario)
        {
            string sql = @" 
                SELECT COUNT(*) 
                FROM T_INVENTARIO_ENDERECO ie
                INNER JOIN T_INVENTARIO_ENDERECO_DET iet ON ie.NU_INVENTARIO_ENDERECO = iet.NU_INVENTARIO_ENDERECO
                WHERE ie.NU_INVENTARIO = :nuInventario 
                    AND CD_PRODUTO IS NOT NULL 
                    AND (iet.ND_ESTADO_INV_ENDERECO_DET = 'SINVEDFINDIF' OR iet.ND_ESTADO_INV_ENDERECO_DET = 'SINVEDFINOK')";

            return _dapper.Query<int>(_context.Database.GetDbConnection(), sql, param: new
            {
                nuInventario = nuInventario
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).FirstOrDefault();
        }

        public virtual void DesmarcarDiferenciaStock(decimal nuInventario, long numeroTransaccion)
        {
            var alias = "ST";
            var from = @"
                T_STOCK ST
                INNER JOIN (
                    SELECT 
                        ie.CD_ENDERECO,
                        iet.CD_EMPRESA,
                        iet.CD_PRODUTO,
                        iet.NU_IDENTIFICADOR,
                        iet.CD_FAIXA
                    FROM  T_INVENTARIO_ENDERECO ie
                    INNER JOIN T_INVENTARIO_ENDERECO_DET iet ON ie.NU_INVENTARIO_ENDERECO = iet.NU_INVENTARIO_ENDERECO
                    WHERE ie.NU_INVENTARIO = :nuInventario
                    GROUP BY 
                        ie.CD_ENDERECO,
                        iet.CD_EMPRESA,
                        iet.CD_PRODUTO,
                        iet.NU_IDENTIFICADOR ,
                        iet.CD_FAIXA
                ) iv ON iv.CD_ENDERECO = ST.CD_ENDERECO 
                    AND iv.CD_PRODUTO = ST.CD_PRODUTO 
                    AND iv.CD_FAIXA = ST.CD_FAIXA 
                    AND iv.NU_IDENTIFICADOR = ST.NU_IDENTIFICADOR 
                    AND iv.CD_EMPRESA = ST.CD_EMPRESA ";

            var set = @"
                NU_TRANSACCION = :NumeroTransaccion, 
                ID_INVENTARIO = 'R',
                DT_UPDROW = :FechaModificacion ";

            var where = @" ID_INVENTARIO = 'D' ";

            _dapper.ExecuteUpdate(_context.Database.GetDbConnection(), alias, from, set, where, param: new
            {
                nuInventario = nuInventario,
                NumeroTransaccion = numeroTransaccion,
                FechaModificacion = DateTime.Now,
            }, commandType: CommandType.Text, _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void FinalizarLpnSinStock(decimal nuInventario, long numeroTransaccion, List<Lpn> lpns = null)
        {
            if (lpns == null)
            {
                var alias = "L";
                var from = @"
                              T_LPN L
                              INNER JOIN (
                                  SELECT 
                                      iet.NU_LPN
                                  FROM  T_INVENTARIO_ENDERECO ie
                                  INNER JOIN T_INVENTARIO_ENDERECO_DET iet ON ie.NU_INVENTARIO_ENDERECO = iet.NU_INVENTARIO_ENDERECO and iet.NU_LPN is not NULL
                                  LEFT JOIN T_LPN_DET LD on LD.NU_LPN =  iet.NU_LPN and LD.QT_ESTOQUE > 0
                                  WHERE ie.NU_INVENTARIO = :nuInventario AND
                                        LD.NU_LPN is null
                                  GROUP BY 
                                      iet.NU_LPN
                              ) iv ON iv.NU_LPN = L.NU_LPN";

                var set = @"
                              NU_TRANSACCION = :NumeroTransaccion, 
                              ID_ESTADO = :ID_ESTADO ,
                              DT_FIN = :DT_FIN";

                var where = @"";

                _dapper.ExecuteUpdate(_context.Database.GetDbConnection(), alias, from, set, where, param: new
                {
                    nuInventario = nuInventario,
                    NumeroTransaccion = numeroTransaccion,
                    ID_ESTADO = EstadosLPN.Finalizado,
                    DT_FIN = DateTime.Now
                }, commandType: CommandType.Text, _context.Database.CurrentTransaction?.GetDbTransaction());
            }
            else
            {
                string sql = @"INSERT INTO T_INVENTARIO_TEMP (NU_LPN) VALUES (:NumeroLPN)";
                _dapper.Execute(_context.Database.GetDbConnection(), sql, lpns, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
                ;

                var alias = "L";
                var from = @$"
                   T_LPN L
                   INNER JOIN (
                            SELECT  
	                            iet.NU_LPN
                            from 
                            (
	                             SELECT 
		                             iet.NU_LPN,
		                             iet.NU_INVENTARIO_ENDERECO,
	                                 SUM(coalesce(ld.QT_ESTOQUE,0)) QT_ESTOQUE
	                             FROM  T_INVENTARIO_ENDERECO ie
	                             INNER JOIN T_INVENTARIO_ENDERECO_DET iet ON ie.NU_INVENTARIO_ENDERECO = iet.NU_INVENTARIO_ENDERECO and iet.NU_LPN is not NULL
                                 INNER JOIN T_INVENTARIO_TEMP it ON it.NU_LPN = iet.NU_LPN 
	                             LEFT JOIN T_LPN_DET LD on LD.NU_LPN =  iet.NU_LPN
	                             WHERE ie.NU_INVENTARIO = :nuInventario
	                               GROUP BY 
		                             iet.NU_LPN,
		                             iet.NU_INVENTARIO_ENDERECO
                            ) iet
                            LEFT JOIN T_INVENTARIO_ENDERECO_DET iet1 ON iet.NU_INVENTARIO_ENDERECO = iet1.NU_INVENTARIO_ENDERECO AND iet.NU_LPN = iet1.NU_LPN
	                            AND (iet1.ND_ESTADO_INV_ENDERECO_DET = :Estado1 OR iet1.ND_ESTADO_INV_ENDERECO_DET = :Estado2)
                            WHERE
                               iet1.NU_INVENTARIO_ENDERECO is null AND
                               iet.QT_ESTOQUE = 0
                            GROUP BY 
                                 iet.NU_LPN
                   ) iv ON iv.NU_LPN = L.NU_LPN";

                var set = @"
               NU_TRANSACCION = :NumeroTransaccion, 
               ID_ESTADO = :ID_ESTADO,
               DT_FIN = :DT_FIN";

                var where = @"";

                _dapper.ExecuteUpdate(_context.Database.GetDbConnection(), alias, from, set, where, param: new
                {
                    nuInventario = nuInventario,
                    ID_ESTADO = EstadosLPN.Finalizado,
                    DT_FIN = DateTime.Now,
                    NumeroTransaccion = numeroTransaccion,
                    Estado1 = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF,
                    Estado2 = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR,
                }, commandType: CommandType.Text, _context.Database.CurrentTransaction?.GetDbTransaction());

                sql = @"DELETE T_INVENTARIO_TEMP WHERE NU_LPN = :NumeroLPN";

                _dapper.Execute(_context.Database.GetDbConnection(), sql, lpns, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
            }
        }

        public virtual void DesmarcarDiferenciasLpn(decimal nuInventario, long numeroTransaccion)
        {
            var alias = "LD";
            var from = @"
                T_LPN_DET LD
                INNER JOIN (
                    SELECT 
                        iet.NU_LPN,
                        iet.ID_LPN_DET
                    FROM  T_INVENTARIO_ENDERECO ie
                    INNER JOIN T_INVENTARIO_ENDERECO_DET iet ON ie.NU_INVENTARIO_ENDERECO = iet.NU_INVENTARIO_ENDERECO
                    WHERE ie.NU_INVENTARIO = :nuInventario
                    GROUP BY 
                        iet.NU_LPN,
                        iet.ID_LPN_DET
                ) iv ON iv.NU_LPN = LD.NU_LPN 
                    AND iv.ID_LPN_DET = LD.ID_LPN_DET ";

            var set = @"
                NU_TRANSACCION = :NumeroTransaccion, 
                ID_INVENTARIO = 'R' ";

            var where = @" ID_INVENTARIO = 'D' ";

            _dapper.ExecuteUpdate(_context.Database.GetDbConnection(), alias, from, set, where, param: new
            {
                nuInventario = nuInventario,
                NumeroTransaccion = numeroTransaccion
            }, commandType: CommandType.Text, _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void FinalizarConteosPendientes(decimal nuInventario, long numeroTransaccion)
        {
            var alias = "ied";
            var from = @"
                T_INVENTARIO_ENDERECO_DET ied
                INNER JOIN (
                    SELECT 
                        ied.NU_INVENTARIO_ENDERECO_DET
                    FROM  T_INVENTARIO_ENDERECO ie
                    INNER JOIN T_INVENTARIO_ENDERECO_DET ied ON ie.NU_INVENTARIO_ENDERECO = ied.NU_INVENTARIO_ENDERECO
                    WHERE ie.NU_INVENTARIO = :nuInventario
                     AND ied.ND_ESTADO_INV_ENDERECO_DET = :EstadoWhere
                    GROUP BY 
                        ied.NU_INVENTARIO_ENDERECO_DET
                ) iv ON ied.NU_INVENTARIO_ENDERECO_DET = iv.NU_INVENTARIO_ENDERECO_DET";

            var set = @"
                NU_TRANSACCION = :NumeroTransaccion, 
                ND_ESTADO_INV_ENDERECO_DET = :Estado,
                DT_UPDROW = :FechaModificacion ";

            var where = @"";

            _dapper.ExecuteUpdate(_context.Database.GetDbConnection(), alias, from, set, where, param: new
            {
                nuInventario = nuInventario,
                NumeroTransaccion = numeroTransaccion,
                EstadoWhere = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR,
                Estado = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_CANCELADO,
                FechaModificacion = DateTime.Now
            }, commandType: CommandType.Text, _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual IEnumerable<InventarioOperacion> GetInventarioOperacion(IEnumerable<InventarioUbicacionDetalle> keys)
        {
            string sql = @"INSERT INTO T_INVENTARIO_TEMP (NU_INVENTARIO_ENDERECO_DET) VALUES (:Id)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"SELECT 
                        IED.NU_INVENTARIO as NumeroInventario,
                        IED.NU_INVENTARIO_ENDERECO as NumeroInventarioUbicacion,
                        IED.NU_INVENTARIO_ENDERECO_DET as NumeroInventarioUbicacionDetalle,
                        IED.TP_INVENTARIO as TipoInventario,
                        IED.ND_ESTADO_INVENTARIO as Estado,
	                    IED.NU_PREDIO as Predio,
	                    IED.CD_ENDERECO as Ubicacion,
                        IED.FL_RESTABLECER_LPN_FINALIZADO as RestablecerLpnFinalizado,
                        IED.FL_MODIFICAR_STOCK_EN_DIF as ModificarStockEnDiferencia,
                        IED.FL_GENERAR_PRIMER_CONTEO as GenerarPrimerConteo
                    FROM V_INVENTARIO_ENDE_DET IED
                    INNER JOIN T_INVENTARIO_TEMP T ON IED.NU_INVENTARIO_ENDERECO_DET = T.NU_INVENTARIO_ENDERECO_DET ";

            var invEstados = _dapper.Query<InventarioOperacion>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            sql = @"DELETE T_INVENTARIO_TEMP WHERE NU_INVENTARIO_ENDERECO_DET = :Id";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return invEstados;
        }

        public virtual IEnumerable<InventarioUbicacionDetalle> GetDetallesInventarioReales(IEnumerable<InventarioSelectRegistroLpn> registros)
        {
            string sql = @" INSERT INTO T_INVENTARIO_TEMP (NU_INVENTARIO_ENDERECO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR) 
                            VALUES (:NuInventarioUbicacion,:Empresa, :Producto, :Faixa, :Identificador)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, registros, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"SELECT 
                        IED.NU_INVENTARIO_ENDERECO_DET as Id,
                        IED.NU_INVENTARIO_ENDERECO as IdInventarioUbicacion,
                        IED.NU_CONTEO as NuConteoDetalle,
                        IED.CD_EMPRESA as Empresa,
                        IED.CD_PRODUTO as Producto,
                        IED.NU_IDENTIFICADOR as Identificador,
                        IED.QT_INVENTARIO as CantidadInventario,
                        IED.QT_DIFERENCIA as CantidadDiferencia,
                        IED.DT_VENCIMIENTO as Vencimiento,
                        IED.ND_ESTADO_INV_ENDERECO_DET as Estado,
                        IED.CD_USUARIO as UserId,
                        IED.DT_ADDROW as FechaAlta,
                        IED.DT_UPDROW as FechaModificacion,
                        IED.QT_TIEMPO_INSUMIDO as TiempoInsumido,
                        IED.CD_MOTIVO_AJUSTE as MotivoAjuste,
                        IED.NU_INSTANCIA_CONTEO as NuInstanciaConteo,
                        IED.CD_FAIXA as Faixa,
                        IED.NU_TRANSACCION as NumeroTransaccion,
                        IED.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                        IED.NU_LPN as NumeroLPN,
                        IED.ID_LPN_DET as IdDetalleLPN,
                        IED.FL_CONTEO_ESPERADO as ConteoEsperado
                    FROM T_INVENTARIO_ENDERECO_DET IED
                    INNER JOIN T_INVENTARIO_TEMP T ON IED.NU_INVENTARIO_ENDERECO = T.NU_INVENTARIO_ENDERECO
                        AND IED.CD_EMPRESA = T.CD_EMPRESA
                        AND IED.CD_PRODUTO = T.CD_PRODUTO
                        AND IED.CD_FAIXA = T.CD_FAIXA
                        AND IED.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR";

            var detalles = _dapper.Query<InventarioUbicacionDetalle>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            var query = @"DELETE T_INVENTARIO_TEMP WHERE NU_INVENTARIO_ENDERECO = :NuInventarioUbicacion";

            _dapper.Execute(_context.Database.GetDbConnection(), query, param: new
            {
                NuInventarioUbicacion = registros.FirstOrDefault().NuInventarioUbicacion
            }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return detalles;
        }

        public virtual IEnumerable<InventarioUbicacionDetalle> GetDetallesInventario(IEnumerable<InventarioUbicacionDetalle> keys)
        {
            string sql = @"INSERT INTO T_INVENTARIO_TEMP (NU_INVENTARIO_ENDERECO_DET) VALUES (:Id)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"SELECT 
                        IED.NU_INVENTARIO_ENDERECO_DET as Id,
                        IED.NU_INVENTARIO_ENDERECO as IdInventarioUbicacion,
                        IED.NU_CONTEO as NuConteoDetalle,
                        IED.CD_EMPRESA as Empresa,
                        IED.CD_PRODUTO as Producto,
                        IED.NU_IDENTIFICADOR as Identificador,
                        IED.QT_INVENTARIO as CantidadInventario,
                        IED.QT_DIFERENCIA as CantidadDiferencia,
                        IED.DT_VENCIMIENTO as Vencimiento,
                        IED.ND_ESTADO_INV_ENDERECO_DET as Estado,
                        IED.CD_USUARIO as UserId,
                        IED.DT_ADDROW as FechaAlta,
                        IED.DT_UPDROW as FechaModificacion,
                        IED.QT_TIEMPO_INSUMIDO as TiempoInsumido,
                        IED.CD_MOTIVO_AJUSTE as MotivoAjuste,
                        IED.NU_INSTANCIA_CONTEO as NuInstanciaConteo,
                        IED.CD_FAIXA as Faixa,
                        IED.NU_TRANSACCION as NumeroTransaccion,
                        IED.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                        IED.NU_LPN as NumeroLPN,
                        IED.ID_LPN_DET as IdDetalleLPN,
                        IED.FL_CONTEO_ESPERADO as ConteoEsperado
                    FROM T_INVENTARIO_ENDERECO_DET IED
                    INNER JOIN T_INVENTARIO_TEMP T ON IED.NU_INVENTARIO_ENDERECO_DET = T.NU_INVENTARIO_ENDERECO_DET ";

            var detalles = _dapper.Query<InventarioUbicacionDetalle>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            var query = @"DELETE T_INVENTARIO_TEMP WHERE NU_INVENTARIO_ENDERECO_DET = :Id";

            _dapper.Execute(_context.Database.GetDbConnection(), query, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return detalles;
        }

        public virtual IEnumerable<InventarioUbicacionDetalle> GetDetallesInventarioRealesLpn(IEnumerable<InventarioSelectRegistroLpn> registros)
        {
            string sql = @" INSERT INTO T_INVENTARIO_TEMP (NU_INVENTARIO_ENDERECO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR, NU_LPN) 
                            VALUES (:NuInventarioUbicacion,:Empresa, :Producto, :Faixa, :Identificador, :NroLpnReal)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, registros, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"SELECT 
                        IED.NU_INVENTARIO_ENDERECO_DET as Id,
                        IED.NU_INVENTARIO_ENDERECO as IdInventarioUbicacion,
                        IED.NU_CONTEO as NuConteoDetalle,
                        IED.CD_EMPRESA as Empresa,
                        IED.CD_PRODUTO as Producto,
                        IED.NU_IDENTIFICADOR as Identificador,
                        IED.QT_INVENTARIO as CantidadInventario,
                        IED.QT_DIFERENCIA as CantidadDiferencia,
                        IED.DT_VENCIMIENTO as Vencimiento,
                        IED.ND_ESTADO_INV_ENDERECO_DET as Estado,
                        IED.CD_USUARIO as UserId,
                        IED.DT_ADDROW as FechaAlta,
                        IED.DT_UPDROW as FechaModificacion,
                        IED.QT_TIEMPO_INSUMIDO as TiempoInsumido,
                        IED.CD_MOTIVO_AJUSTE as MotivoAjuste,
                        IED.NU_INSTANCIA_CONTEO as NuInstanciaConteo,
                        IED.CD_FAIXA as Faixa,
                        IED.NU_TRANSACCION as NumeroTransaccion,
                        IED.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                        IED.NU_LPN as NumeroLPN,
                        IED.ID_LPN_DET as IdDetalleLPN,
                        IED.FL_CONTEO_ESPERADO as ConteoEsperado
                    FROM T_INVENTARIO_ENDERECO_DET IED
                    INNER JOIN T_INVENTARIO_TEMP T ON IED.NU_INVENTARIO_ENDERECO = T.NU_INVENTARIO_ENDERECO
                        AND IED.CD_EMPRESA = T.CD_EMPRESA
                        AND IED.CD_PRODUTO = T.CD_PRODUTO
                        AND IED.CD_FAIXA = T.CD_FAIXA
                        AND IED.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR
                        AND IED.NU_LPN = T.NU_LPN ";

            var detalles = _dapper.Query<InventarioUbicacionDetalle>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            var query = @"DELETE T_INVENTARIO_TEMP WHERE NU_INVENTARIO_ENDERECO = :NuInventarioUbicacion";

            _dapper.Execute(_context.Database.GetDbConnection(), query, param: new
            {
                NuInventarioUbicacion = registros.FirstOrDefault().NuInventarioUbicacion
            }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return detalles;
        }

        public virtual IEnumerable<InventarioSelectRegistroLpn> GetDetallesFinalesUbicaciones(IEnumerable<InventarioUbicacion> ubicaciones)
        {
            string sql = @" INSERT INTO T_INVENTARIO_TEMP (NU_INVENTARIO,CD_ENDERECO) VALUES (:IdInventario, :Ubicacion)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, ubicaciones, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"SELECT 
                        RG.NU_INVENTARIO as NuInventario,
                        RG.CD_ENDERECO as Ubicacion,
                        RG.CD_EMPRESA as Empresa,
                        RG.CD_PRODUTO as Producto,
                        RG.CD_FAIXA as Faixa,
                        RG.NU_IDENTIFICADOR as Identificador,
                        RG.QT_ESTOQUE as Cantidad,
                        RG.DT_FABRICACAO as Vencimiento,
                        RG.NU_LPN as NroLpn,
                        RG.ID_LPN_DET as IdDetalleLpn
                    FROM V_INV417_REG_DISP RG
                    INNER JOIN T_INVENTARIO_TEMP T ON RG.CD_ENDERECO = T.CD_ENDERECO 
                        AND RG.NU_INVENTARIO = T.NU_INVENTARIO";

            var result = _dapper.Query<InventarioSelectRegistroLpn>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            sql = @"DELETE T_INVENTARIO_TEMP WHERE NU_INVENTARIO = :IdInventario";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, ubicaciones, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return result;
        }

        public virtual IEnumerable<Stock> GetStockByDetallesInventario(IEnumerable<InventarioSelectRegistroLpn> registros)
        {
            string sql = @" INSERT INTO T_INVENTARIO_TEMP (ID_OPERACION, CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR) 
                            VALUES (:IdOperacion, :Ubicacion,:Empresa, :Producto, :Faixa, :Identificador)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, registros, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"SELECT 
                        STO.CD_ENDERECO as Ubicacion,
                        STO.CD_EMPRESA as Empresa,    
                        STO.CD_PRODUTO as Producto,
                        STO.CD_FAIXA as Faixa,
                        STO.NU_IDENTIFICADOR as Identificador,
                        MIN(STO.QT_ESTOQUE) as Cantidad,
                        MIN(STO.QT_RESERVA_SAIDA) as ReservaSalida,
                        MIN(STO.QT_TRANSITO_ENTRADA) as CantidadTransitoEntrada,
                        MIN(STO.DT_FABRICACAO) as Vencimiento,    
                        MIN(STO.ID_AVERIA) as Averia,    
                        MIN(STO.ID_INVENTARIO) as Inventario,
                        MIN(STO.ID_CTRL_CALIDAD) as ControlCalidad,
                        MIN(STO.DT_INVENTARIO) as FechaInventario,
                        MIN(STO.DT_UPDROW) as FechaModificacion,
                        MIN(STO.CD_MOTIVO_AVERIA) as MotivoAveria,
                        MIN(STO.NU_TRANSACCION) as NumeroTransaccion
                    FROM T_STOCK STO
                    INNER JOIN T_INVENTARIO_TEMP T ON STO.CD_ENDERECO = T.CD_ENDERECO
                        AND STO.CD_EMPRESA = T.CD_EMPRESA
                        AND STO.CD_PRODUTO = T.CD_PRODUTO
                        AND STO.CD_FAIXA = T.CD_FAIXA
                        AND STO.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR
                    GROUP BY
                        STO.CD_ENDERECO,
                        STO.CD_EMPRESA,    
                        STO.CD_PRODUTO,
                        STO.CD_FAIXA,
                        STO.NU_IDENTIFICADOR";

            var detalles = _dapper.Query<Stock>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            sql = @"DELETE T_INVENTARIO_TEMP WHERE ID_OPERACION = :IdOperacion";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param: new
            {
                IdOperacion = registros.FirstOrDefault().IdOperacion
            }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return detalles;
        }

        public virtual IEnumerable<StockSuelto> GetStockSueltoByDetallesInventario(IEnumerable<InventarioSelectRegistroLpn> registros)
        {
            string sql = @" INSERT INTO T_INVENTARIO_TEMP (ID_OPERACION, CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR) 
                            VALUES (:IdOperacion, :Ubicacion,:Empresa, :Producto, :Faixa, :Identificador)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, registros, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"SELECT 
                        STO.CD_ENDERECO as Ubicacion,
                        STO.CD_EMPRESA as Empresa,    
                        STO.CD_PRODUTO as Producto,
                        STO.CD_FAIXA as Faixa,
                        STO.NU_IDENTIFICADOR as Identificador,
                        MIN(STO.DT_FABRICACAO) as Vencimiento,    
                        MIN(STO.QT_ESTOQUE) as Cantidad,
                        MIN(STO.QT_RESERVA_SAIDA) as ReservaSalida,
                        MIN(STO.QT_TRANSITO_ENTRADA) as CantidadTransitoEntrada,
                        MIN(STO.QT_ESTOQUE_LPN) as CantidadLpn,
                        MIN(STO.QT_ESTOQUE_SUELTO) as CantidadSuelta,
                        MIN(STO.ID_AVERIA) as Averia,    
                        MIN(STO.ID_INVENTARIO) as Inventario,
                        MIN(STO.ID_CTRL_CALIDAD) as ControlCalidad,                        
                        MIN(STO.CD_MOTIVO_AVERIA) as MotivoAveria
                    FROM V_STOCK_SIN_LPN STO
                    INNER JOIN T_INVENTARIO_TEMP T ON STO.CD_ENDERECO = T.CD_ENDERECO
                        AND STO.CD_EMPRESA = T.CD_EMPRESA
                        AND STO.CD_PRODUTO = T.CD_PRODUTO
                        AND STO.CD_FAIXA = T.CD_FAIXA
                        AND STO.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR
                    GROUP BY
                        STO.CD_ENDERECO,
                        STO.CD_EMPRESA,    
                        STO.CD_PRODUTO,
                        STO.CD_FAIXA,
                        STO.NU_IDENTIFICADOR";

            var detalles = _dapper.Query<StockSuelto>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            sql = @"DELETE T_INVENTARIO_TEMP WHERE ID_OPERACION = :IdOperacion";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param: new
            {
                IdOperacion = registros.FirstOrDefault().IdOperacion
            }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return detalles;
        }

        public virtual void SetearAtributosDetalleInventario(IEnumerable<InventarioSelectRegistroLpn> registros)
        {
            string sql = @" INSERT INTO T_INVENTARIO_TEMP (ID_OPERACION, NU_INVENTARIO_ENDERECO_DET, NU_LPN, ID_LPN_DET) 
                            VALUES (:IdOperacion, :NuInventarioUbicacionDetalle, :NroLpnReal, :IdDetalleLpnReal)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, registros, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @$"INSERT INTO T_INVENTARIO_ENDERECO_DET_ATR
                        (NU_INVENTARIO_ENDERECO_DET,
                        ID_ATRIBUTO,
                        VL_ATRIBUTO) 
                    SELECT 
                        T.NU_INVENTARIO_ENDERECO_DET,
                        LDA.ID_ATRIBUTO,
                        LDA.VL_LPN_DET_ATRIBUTO            
                    FROM T_LPN_DET_ATRIBUTO LDA
                    INNER JOIN T_INVENTARIO_TEMP T ON LDA.NU_LPN = T.NU_LPN
                        AND LDA.ID_LPN_DET = T.ID_LPN_DET
                    INNER JOIN T_ATRIBUTO A ON LDA.ID_ATRIBUTO = A.ID_ATRIBUTO
                    WHERE A.ID_ATRIBUTO_TIPO != 6
                    GROUP BY
                        T.NU_INVENTARIO_ENDERECO_DET,
                        LDA.ID_ATRIBUTO,
                        LDA.VL_LPN_DET_ATRIBUTO ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"DELETE T_INVENTARIO_TEMP WHERE ID_OPERACION = :IdOperacion";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param: new
            {
                IdOperacion = registros.FirstOrDefault().IdOperacion
            }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

        }

        #endregion
    }
}
