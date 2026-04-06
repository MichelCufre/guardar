using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Registro;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class PuertaEmbarqueRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly PuertaEmbarqueMapper _mapper;
        protected readonly ParametroRepository _paramRepository;
        protected readonly IDapper _dapper;

        public PuertaEmbarqueRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new PuertaEmbarqueMapper();
            this._paramRepository = new ParametroRepository(context, cdAplicacion, userId, dapper);
        }

        #region Any

        public virtual bool AnyPuertaEmbarque(short id)
        {
            return this._context.T_PORTA_EMBARQUE.Any(d => d.CD_PORTA == id);
        }

        public virtual bool AnyPuertaEmbarquePertenecePredio(short puerta, string predio)
        {
            return this._context.T_PORTA_EMBARQUE.Include("T_ENDERECO_ESTOQUE").Any(d => d.CD_PORTA == puerta && d.T_ENDERECO_ESTOQUE.NU_PREDIO == predio);
        }

        #endregion

        #region Get

        public virtual PuertaEmbarque GetPuertaEmbarque(short idPuerta)
        {
            T_PORTA_EMBARQUE entity = this._context.T_PORTA_EMBARQUE.Include("T_ENDERECO_ESTOQUE").AsNoTracking().FirstOrDefault(d => d.CD_PORTA == idPuerta);
            return this._mapper.MapToObject(entity);
        }

        public virtual List<PuertaEmbarque> GetByDescripcionOrCodePartial(string value, string predio)
        {
            List<T_PORTA_EMBARQUE> puertasEntity;
            List<PuertaEmbarque> puertas = new List<PuertaEmbarque>();

            bool ignorarPredio = (string.IsNullOrEmpty(predio) || predio == GeneralDb.PredioSinDefinir);

            if (short.TryParse(value, out short puerta))
                puertasEntity = this._context.T_PORTA_EMBARQUE
                    .Where(d => (ignorarPredio || d.NU_PREDIO == predio)
                        && (d.CD_PORTA == puerta
                            || (d.CD_PORTA.ToString().Contains(puerta.ToString()))
                            || (d.DS_PORTA.ToLower().Contains(value.ToLower()))))
                    .ToList();
            else
                puertasEntity = this._context.T_PORTA_EMBARQUE
                    .Where(d => (ignorarPredio || d.NU_PREDIO == predio)
                        && d.DS_PORTA.ToLower().Contains(value.ToLower()))
                    .ToList();

            foreach (var entity in puertasEntity)
            {
                puertas.Add(this._mapper.MapToObject(entity));
            }

            return puertas;
        }

        public virtual List<PuertaEmbarque> GetPuertasActivas(string numeroPredio = "")
        {
            var query = this._context.T_PORTA_EMBARQUE.Include("T_ENDERECO_ESTOQUE").AsNoTracking()
                                        .Where(s => s.CD_SITUACAO == SituacionDb.Activo);

            if (!string.IsNullOrEmpty(numeroPredio) && numeroPredio != GeneralDb.PredioSinDefinir)
            {
                query = query.Where(s => s.NU_PREDIO == numeroPredio);
            }

            return query.Select(s => _mapper.MapToObject(s)).ToList();
        }

        public virtual List<PuertaEmbarque> GetPuertasPredio(HashSet<string> predios)
        {
            return _context.T_PORTA_EMBARQUE.AsNoTracking()
                .Where(p => predios.Contains(p.NU_PREDIO))
                .Select(p => _mapper.MapToObject(p)).ToList();
        }

        public virtual PuertaEmbarqueConfiguracion GetConfiguracionPuertaEmbarque(int empresa)
        {
            PuertaEmbarqueConfiguracion configuracion = new PuertaEmbarqueConfiguracion();

            Dictionary<string, string> colParams = new Dictionary<string, string>();
            colParams[ParamManager.PARAM_EMPR] = string.Format("{0}_{1}", ParamManager.PARAM_EMPR, empresa);

            if (!int.TryParse(_paramRepository.GetParameter("REG080_CD_FAMILIA_PRINCIPAL", colParams), out int idFamiliaPrincipal))
                throw new EntityNotFoundException("General_Sec0_Error_Error80");

            if (!short.TryParse(_paramRepository.GetParameter("REG080_CD_ROTATIVIDADE", colParams), out short idRotatividad))
                throw new EntityNotFoundException("General_Sec0_Error_Error80");

            string idClase = _paramRepository.GetParameter("REG080_CD_CLASSE", colParams);
            if (string.IsNullOrEmpty(idClase))
                throw new EntityNotFoundException("General_Sec0_Error_Error80");

            string prefijo = _paramRepository.GetParameter(ParamManager.PREFIJO_PUERTA, colParams);
            if (string.IsNullOrEmpty(prefijo))
                throw new EntityNotFoundException("General_Sec0_Error_Error80");

            configuracion.Clase = idClase;
            configuracion.FamiliaPrincipal = idFamiliaPrincipal;
            configuracion.Rotatividad = idRotatividad;
            configuracion.PrefijoPuerta = prefijo;

            return configuracion;
        }

        public virtual PuertaEmbarque GetPuertaEmbarqueByUbicacion(string endereco)
        {
            var entity = this._context.T_PORTA_EMBARQUE.Include("T_ENDERECO_ESTOQUE").AsNoTracking().FirstOrDefault(d => d.CD_ENDERECO == endereco);
            return this._mapper.MapToObject(entity);
        }

        public virtual PuertaEmbarque GetPuertaDefecto(string predio)
        {
            var entity = this._context.T_PORTA_EMBARQUE.Include("T_ENDERECO_ESTOQUE").AsNoTracking().FirstOrDefault(d => d.NU_PREDIO == predio);
            return this._mapper.MapToObject(entity);
        }

        public virtual short? GetFirstPuertaByPredio(string predio)
        {
            return _context.V_PUERTA_EMBARQUE_WREG080.FirstOrDefault(w => w.NU_PREDIO == predio && w.CD_SITUACAO == SituacionDb.Activo && w.TP_PUERTA != TipoPuertaEmbarqueDb.Entrada)?.CD_PORTA;
        }

        public virtual string GetUbicacionPuertaEmbarque(short? nuPuerta)
        {
            return _context.T_PORTA_EMBARQUE.FirstOrDefault(x => x.CD_PORTA == nuPuerta)?.CD_ENDERECO;
        }

        #endregion

        #region Add

        public virtual void AddPuertaEmbarque(PuertaEmbarque puerta)
        {
            var entity = this._mapper.MapToEntity(puerta);

            entity.CD_ENDERECO = this._mapper.NullIfEmpty(puerta.Ubicacion.Id);

            this._context.T_PORTA_EMBARQUE.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdatePuertaEmbarque(PuertaEmbarque puerta)
        {
            var entity = this._mapper.MapToEntity(puerta);
            var attachedEntity = _context.T_PORTA_EMBARQUE.Local
                .FirstOrDefault(c => c.CD_PORTA == entity.CD_PORTA);

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PORTA_EMBARQUE.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Dapper

        public virtual string GetUbicacionPuerta(short? puerta)
        {
            var param = new DynamicParameters(new
            {
                CD_PORTA = puerta
            });

            string sql = @"SELECT CD_ENDERECO FROM T_PORTA_EMBARQUE WHERE CD_PORTA = :CD_PORTA";

            return _dapper.Query<string>(_context.Database.GetDbConnection(), sql, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).FirstOrDefault();
        }

        #endregion
    }
}
