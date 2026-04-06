using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class EstrategiaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly EstrategiaMapper _mapper;
        protected readonly IDapper _dapper;

        public EstrategiaRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new EstrategiaMapper();
            this._dapper = dapper;
        }

        #region Any
        public virtual bool AnyCodigoLogica(short nuLogica)
        {
            return this._context.V_REC275_LOGICAS
                .AsNoTracking()
                .Any(x => x.NU_ALM_LOGICA == nuLogica);
        }

        public virtual bool AnySugerenciaLogica(int codigoLogica)
        {
            return this._context.T_ALM_SUGERENCIA
                .AsNoTracking()
                .Any(x => x.NU_ALM_LOGICA_INSTANCIA == codigoLogica);
        }

        public virtual bool AnySugerenciaEstrategia(int codigoEstrategia)
        {
            return this._context.T_ALM_SUGERENCIA
                .AsNoTracking()
                .Any(x => x.NU_ALM_ESTRATEGIA == codigoEstrategia);
        }

        public virtual bool AnySugerenciaEstrategiaPendiente(int codigoEstrategia)
        {
            return this._context.T_ALM_SUGERENCIA
                .AsNoTracking()
                .Any(x => x.NU_ALM_ESTRATEGIA == codigoEstrategia && x.CD_ESTADO == "P");
        }

        public virtual bool AnySugerenciaEstrategiaPendiente(int codigoEstrategia, string predio, string tipoOperativa, string codigoOperativa, string tipoEntidad, string codigoEntidad, string empresa)
        {
            var sugerencias = this._context.T_ALM_SUGERENCIA
                .AsNoTracking()
                .Where(x => x.NU_ALM_ESTRATEGIA == codigoEstrategia
                    && x.NU_PREDIO == predio
                    && x.TP_ALM_OPERATIVA_ASOCIABLE == tipoOperativa
                    && x.CD_ALM_OPERATIVA_ASOCIABLE == codigoOperativa
                    && x.CD_ESTADO == "P");

            switch (tipoEntidad)
            {
                case AlmacenamientoDb.TIPO_ENTIDAD_CLASE:
                    sugerencias = sugerencias.Where(x => x.CD_CLASSE == codigoEntidad);
                    break;
                case AlmacenamientoDb.TIPO_ENTIDAD_GRUPO:
                    sugerencias = sugerencias.Where(x => x.CD_GRUPO == codigoEntidad);
                    break;
                case AlmacenamientoDb.TIPO_ENTIDAD_PRODUCTO:
                    sugerencias = sugerencias.Where(x => x.CD_PRODUTO == codigoEntidad && x.CD_EMPRESA == int.Parse(empresa));
                    break;
            }

            return sugerencias.Any();
        }

        public virtual bool AnySugerenciaLogicaPendiente(int codigoLogica)
        {
            return this._context.T_ALM_SUGERENCIA
                .AsNoTracking()
                .Any(x => x.NU_ALM_LOGICA_INSTANCIA == codigoLogica && x.CD_ESTADO == "P");
        }

        public virtual bool AnyDetalleSugerencia(int estrategia, string predio, string tipoOperativa, string codigoOperativa, string codigoClase,
            string codigoGrupo, int empresa, string producto, string codigoReferencia, string codigoAgrupador, string enderecoSugerido)
        {
            return this._context.V_REC280_PANEL_SUGERENCIA_DET
                .AsNoTracking()
                .Any(x => x.NU_ALM_ESTRATEGIA == estrategia &&
                          x.NU_PREDIO == predio &&
                          x.TP_ALM_OPERATIVA_ASOCIABLE == tipoOperativa &&
                          x.CD_ALM_OPERATIVA_ASOCIABLE == codigoOperativa &&
                          x.CD_CLASSE == codigoClase &&
                          x.CD_GRUPO == codigoGrupo &&
                          x.CD_EMPRESA_PRODUTO == empresa &&
                          x.CD_PRODUTO == producto &&
                          x.CD_REFERENCIA == codigoReferencia &&
                          x.CD_AGRUPADOR == codigoAgrupador &&
                          x.CD_ENDERECO_SUGERIDO == enderecoSugerido);
        }

        #endregion

        #region Get

        public virtual Estrategia GetEstrategiaByCod(string codigoEstrategia)
        {
            return this._mapper.MapToObject(this._context.T_ALM_ESTRATEGIA
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ALM_ESTRATEGIA == int.Parse(codigoEstrategia)));
        }

        public virtual InstanciaLogica GetInstanciaByCod(string codigoInstancia)
        {
            return this._mapper.MapToObject(this._context.T_ALM_LOGICA_INSTANCIA
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ALM_LOGICA_INSTANCIA == int.Parse(codigoInstancia)));
        }

        public virtual AsociacionEstrategia GetAsociacionEstrategiaByCod(short numeroAsociacion)
        {
            return this._mapper.MapToObject(this._context.T_ALM_ASOCIACION
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ALM_ASOCIACION == numeroAsociacion));
        }

        public virtual InstanciaLogicaParametro GetInstanciaParametroByCod(int codigoInstanciaParametro)
        {
            return this._mapper.MapToObject(this._context.T_ALM_LOGICA_INSTANCIA_PARAM
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ALM_LOGICA_INSTANCIA_PARAM == codigoInstanciaParametro));
        }

        public virtual InstanciaLogica GetInstanciaForChangeOrder(short numeroOrden, int numeroEstrategia)
        {
            return this._mapper.MapToObject(this._context.T_ALM_LOGICA_INSTANCIA
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ORDEN == numeroOrden && x.NU_ALM_ESTRATEGIA == numeroEstrategia));
        }

        public virtual List<InstanciaLogica> GetAllInstanciaByCodEstrategia(string codigoEstrategia)
        {
            var listaInstancias = new List<InstanciaLogica>();
            var entities = this._context.T_ALM_LOGICA_INSTANCIA
                .AsNoTracking()
                .Where(x => x.NU_ALM_ESTRATEGIA == int.Parse(codigoEstrategia))
                .OrderByDescending(x => x.NU_ORDEN);

            foreach (var entity in entities)
            {
                listaInstancias.Add(this._mapper.MapToObject(entity));
            }

            return listaInstancias;
        }

        public virtual List<ParametroDefault> GetAllParametrosByCodLogica(string codigoLogica)
        {
            var listaInstancias = new List<ParametroDefault>();
            var entities = this._context.V_REC275_PARAMETROS
                .AsNoTracking()
                .Where(x => x.NU_ALM_LOGICA == short.Parse(codigoLogica));

            foreach (var entity in entities)
            {
                listaInstancias.Add(this._mapper.MapToObject(entity));
            }

            return listaInstancias;
        }

        public virtual List<Logica> GetAllLogicas()
        {
            var listaLogicas = new List<Logica>();
            var entities = this._context.V_REC275_LOGICAS
                .AsNoTracking()
                .OrderBy(l => l.NU_ALM_LOGICA);

            foreach (var entity in entities)
            {
                listaLogicas.Add(this._mapper.MapToObject(entity));
            }

            return listaLogicas;
        }

        public virtual Logica GetLogicaByCod(short numeroLogica)
        {
            return this._mapper.MapToObject(this._context.V_REC275_LOGICAS
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ALM_LOGICA == numeroLogica));
        }

        public virtual List<InstanciaLogica> GetInstanciasPosteriores(InstanciaLogica instancia)
        {
            return this._context.T_ALM_LOGICA_INSTANCIA
                .AsNoTracking()
                .Where(i => i.NU_ALM_ESTRATEGIA == instancia.Estrategia
                    && i.NU_ORDEN > instancia.Orden)
                .Select(i => this._mapper.MapToObject(i))
                .ToList();
        }

        public virtual List<EstrategiaSugerencia> GetSugerenciasByCodEstrategia(int codigoEstrategia)
        {
            return _context.T_ALM_SUGERENCIA.AsNoTracking()
                .Where(x => x.NU_ALM_ESTRATEGIA == codigoEstrategia)
                .Select(x => _mapper.MapToObject(x)).ToList();
        }

        public virtual List<EstrategiaSugerencia> GetSugerenciasByLogica(int codigoLogica)
        {
            return _context.T_ALM_SUGERENCIA.AsNoTracking()
                .Where(x => x.NU_ALM_LOGICA_INSTANCIA == codigoLogica)
                .Select(x => _mapper.MapToObject(x)).ToList();
        }

        #endregion

        #region Add

        public virtual void AddEstrategia(Estrategia estrategia)
        {
            estrategia.NumeroEstrategia = this._context.GetNextSequenceValueInt(_dapper, "S_ALM_ESTRATEGIA");
            T_ALM_ESTRATEGIA entity = this._mapper.MapToEntity(estrategia);
            this._context.T_ALM_ESTRATEGIA.Add(entity);
        }
        
        public virtual void AddInstanciaLogica(InstanciaLogica instancia)
        {
            instancia.Id = this._context.GetNextSequenceValueInt(_dapper, "S_ALM_LOGICA_INSTANCIA");
            T_ALM_LOGICA_INSTANCIA entity = this._mapper.MapToEntity(instancia);
            this._context.T_ALM_LOGICA_INSTANCIA.Add(entity);
        }
        
        public virtual void AddInstanciaParametro(InstanciaLogicaParametro instancia)
        {
            instancia.Id = this._context.GetNextSequenceValueInt(_dapper, "S_ALM_LOGICA_INSTANCIA_PARAM");
            T_ALM_LOGICA_INSTANCIA_PARAM entity = this._mapper.MapToEntity(instancia);
            this._context.T_ALM_LOGICA_INSTANCIA_PARAM.Add(entity);
        }
        
        public virtual void AddAsociacionEstrategia(AsociacionEstrategia instancia)
        {
            instancia.Id = this._context.GetNextSequenceValueInt(_dapper, "S_ALM_ASOCIACION");
            T_ALM_ASOCIACION entity = this._mapper.MapToEntity(instancia);
            this._context.T_ALM_ASOCIACION.Add(entity);
        }
        
        #endregion

        #region Update
        
        public virtual void UpdateEstrategia(Estrategia estrategia)
        {
            T_ALM_ESTRATEGIA entity = this._mapper.MapToEntity(estrategia);
            T_ALM_ESTRATEGIA attachedEntity = _context.T_ALM_ESTRATEGIA.Local
                .FirstOrDefault(c => c.NU_ALM_ESTRATEGIA == estrategia.NumeroEstrategia);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ALM_ESTRATEGIA.Attach(entity);
                _context.Entry<T_ALM_ESTRATEGIA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateEstrategiaInstancia(InstanciaLogica estrategiaLogica)
        {
            T_ALM_LOGICA_INSTANCIA entity = this._mapper.MapToEntity(estrategiaLogica);
            T_ALM_LOGICA_INSTANCIA attachedEntity = _context.T_ALM_LOGICA_INSTANCIA.Local
                .FirstOrDefault(c => c.NU_ALM_LOGICA_INSTANCIA == estrategiaLogica.Id);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ALM_LOGICA_INSTANCIA.Attach(entity);
                _context.Entry<T_ALM_LOGICA_INSTANCIA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateInstanciaLogica(InstanciaLogica instanciaLogica)
        {
            T_ALM_LOGICA_INSTANCIA entity = this._mapper.MapToEntity(instanciaLogica);
            T_ALM_LOGICA_INSTANCIA attachedEntity = _context.T_ALM_LOGICA_INSTANCIA.Local
                .FirstOrDefault(c => c.NU_ALM_LOGICA_INSTANCIA == instanciaLogica.Id);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ALM_LOGICA_INSTANCIA.Attach(entity);
                _context.Entry<T_ALM_LOGICA_INSTANCIA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateInstanciaParametro(InstanciaLogicaParametro instanciaParametro)
        {
            T_ALM_LOGICA_INSTANCIA_PARAM entity = this._mapper.MapToEntity(instanciaParametro);
            T_ALM_LOGICA_INSTANCIA_PARAM attachedEntity = _context.T_ALM_LOGICA_INSTANCIA_PARAM.Local
                .FirstOrDefault(c => c.NU_ALM_LOGICA_INSTANCIA_PARAM == instanciaParametro.Id);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ALM_LOGICA_INSTANCIA_PARAM.Attach(entity);
                _context.Entry<T_ALM_LOGICA_INSTANCIA_PARAM>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Delete

        public virtual void DeleteEstrategia(Estrategia estrategia)
        {
            T_ALM_ESTRATEGIA entity = this._context.T_ALM_ESTRATEGIA
                .FirstOrDefault(x => x.NU_ALM_ESTRATEGIA == estrategia.NumeroEstrategia);
            T_ALM_ESTRATEGIA attachedEntity = this._context.T_ALM_ESTRATEGIA.Local
                .FirstOrDefault(x => x.NU_ALM_ESTRATEGIA == estrategia.NumeroEstrategia);

            if (attachedEntity != null)
            {
                this._context.T_ALM_ESTRATEGIA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_ALM_ESTRATEGIA.Attach(entity);
                this._context.T_ALM_ESTRATEGIA.Remove(entity);
            }
        }

        public virtual void DeleteInstancia(InstanciaLogica instancia)
        {
            T_ALM_LOGICA_INSTANCIA entity = this._context.T_ALM_LOGICA_INSTANCIA
                .Include("T_ALM_LOGICA_INSTANCIA_PARAM")
                .FirstOrDefault(x => x.NU_ALM_LOGICA_INSTANCIA == instancia.Id);
            T_ALM_LOGICA_INSTANCIA attachedEntity = this._context.T_ALM_LOGICA_INSTANCIA.Local
                .FirstOrDefault(x => x.NU_ALM_LOGICA_INSTANCIA == instancia.Id);

            if (attachedEntity != null)
            {
                this._context.T_ALM_LOGICA_INSTANCIA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_ALM_LOGICA_INSTANCIA.Attach(entity);
                this._context.T_ALM_LOGICA_INSTANCIA.Remove(entity);
            }
        }

        public virtual void DeleteAsociacionEstrategia(AsociacionEstrategia asociacionEstrategia)
        {
            T_ALM_ASOCIACION entity = this._context.T_ALM_ASOCIACION
                .FirstOrDefault(x => x.NU_ALM_ASOCIACION == asociacionEstrategia.Id);
            T_ALM_ASOCIACION attachedEntity = this._context.T_ALM_ASOCIACION.Local
                .FirstOrDefault(x => x.NU_ALM_ASOCIACION == asociacionEstrategia.Id);

            if (attachedEntity != null)
            {
                this._context.T_ALM_ASOCIACION.Remove(attachedEntity);
            }
            else
            {
                this._context.T_ALM_ASOCIACION.Attach(entity);
                this._context.T_ALM_ASOCIACION.Remove(entity);
            }
        }

        public virtual void DeleteSugerencia(EstrategiaSugerencia sugerencia)
        {
            T_ALM_SUGERENCIA entity = this._context.T_ALM_SUGERENCIA
                .FirstOrDefault(x => x.NU_ALM_ESTRATEGIA == sugerencia.NumeroEstrategia
                                        && x.NU_PREDIO == sugerencia.NumeroPredio
                                        && x.TP_ALM_OPERATIVA_ASOCIABLE == sugerencia.TipoOperativa
                                        && x.CD_ALM_OPERATIVA_ASOCIABLE == sugerencia.CodigoOperativaAsociable
                                        && x.CD_CLASSE == sugerencia.CodigoClase
                                        && x.CD_GRUPO == sugerencia.CodigoGrupo
                                        && x.CD_EMPRESA == sugerencia.CodigoEmpresa
                                        && x.CD_PRODUTO == sugerencia.CodigoProducto
                                        && x.CD_REFERENCIA == sugerencia.Referencia
                                        && x.CD_AGRUPADOR == sugerencia.Agrupador
                                        && x.CD_ENDERECO_SUGERIDO == sugerencia.UbicacionSugerida
                                        && x.NU_ALM_SUGERENCIA == sugerencia.NuAlmSugerencia);

            T_ALM_SUGERENCIA attachedEntity = this._context.T_ALM_SUGERENCIA.Local
                .FirstOrDefault(x => x.NU_ALM_ESTRATEGIA == sugerencia.NumeroEstrategia
                                        && x.NU_PREDIO == sugerencia.NumeroPredio
                                        && x.TP_ALM_OPERATIVA_ASOCIABLE == sugerencia.TipoOperativa
                                        && x.CD_ALM_OPERATIVA_ASOCIABLE == sugerencia.CodigoOperativaAsociable
                                        && x.CD_CLASSE == sugerencia.CodigoClase
                                        && x.CD_GRUPO == sugerencia.CodigoGrupo
                                        && x.CD_EMPRESA == sugerencia.CodigoEmpresa
                                        && x.CD_PRODUTO == sugerencia.CodigoProducto
                                        && x.CD_REFERENCIA == sugerencia.Referencia
                                        && x.CD_AGRUPADOR == sugerencia.Agrupador
                                        && x.CD_ENDERECO_SUGERIDO == sugerencia.UbicacionSugerida
                                        && x.NU_ALM_SUGERENCIA == sugerencia.NuAlmSugerencia
                                        );

            if (attachedEntity != null)
            {
                this._context.T_ALM_SUGERENCIA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_ALM_SUGERENCIA.Remove(entity);
            }
        }

        public virtual void DeleteSugerenciaDetalle(EstrategiaSugerencia sugerencia)
        {
            List<T_ALM_SUGERENCIA_DET> entity = this._context.T_ALM_SUGERENCIA_DET
                .Where(x => x.NU_ALM_ESTRATEGIA == sugerencia.NumeroEstrategia
                                        && x.NU_PREDIO == sugerencia.NumeroPredio
                                        && x.TP_ALM_OPERATIVA_ASOCIABLE == sugerencia.TipoOperativa
                                        && x.CD_ALM_OPERATIVA_ASOCIABLE == sugerencia.CodigoOperativaAsociable
                                        && x.CD_CLASSE == sugerencia.CodigoClase
                                        && x.CD_GRUPO == sugerencia.CodigoGrupo
                                        && x.CD_EMPRESA == sugerencia.CodigoEmpresa
                                        && x.CD_PRODUTO == sugerencia.CodigoProducto
                                        && x.CD_REFERENCIA == sugerencia.Referencia
                                        && x.CD_AGRUPADOR == sugerencia.Agrupador
                                        && x.CD_ENDERECO_SUGERIDO == sugerencia.UbicacionSugerida
                                        && x.NU_ALM_SUGERENCIA == sugerencia.NuAlmSugerencia).ToList();

            List<T_ALM_SUGERENCIA_DET> attachedEntity = this._context.T_ALM_SUGERENCIA_DET.Local
                .Where(x => x.NU_ALM_ESTRATEGIA == sugerencia.NumeroEstrategia
                                        && x.NU_PREDIO == sugerencia.NumeroPredio
                                        && x.TP_ALM_OPERATIVA_ASOCIABLE == sugerencia.TipoOperativa
                                        && x.CD_ALM_OPERATIVA_ASOCIABLE == sugerencia.CodigoOperativaAsociable
                                        && x.CD_CLASSE == sugerencia.CodigoClase
                                        && x.CD_GRUPO == sugerencia.CodigoGrupo
                                        && x.CD_EMPRESA == sugerencia.CodigoEmpresa
                                        && x.CD_PRODUTO == sugerencia.CodigoProducto
                                        && x.CD_REFERENCIA == sugerencia.Referencia
                                        && x.CD_AGRUPADOR == sugerencia.Agrupador
                                        && x.CD_ENDERECO_SUGERIDO == sugerencia.UbicacionSugerida
                                        && x.NU_ALM_SUGERENCIA == sugerencia.NuAlmSugerencia).ToList();

            if (attachedEntity != null)
            {
                this._context.T_ALM_SUGERENCIA_DET.RemoveRange(attachedEntity);
            }
            else
            {
                this._context.T_ALM_SUGERENCIA_DET.RemoveRange(entity);
            }
        }

        #endregion

        #region Dapper

        #endregion
    }
}
