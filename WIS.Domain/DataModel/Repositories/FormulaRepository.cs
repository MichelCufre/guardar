using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.Produccion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class FormulaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly FormulaMapper _mapper;
        protected readonly IDapper _dapper;

        public FormulaRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new FormulaMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyFormula(string codigo)
        {
            return _context.T_PRDC_DEFINICION
                .AsNoTracking()
                .Any(d => d.CD_PRDC_DEFINICION == codigo);
        }

        public virtual bool AnyFormulaConfiguracion(string formula, int accion)
        {
            return _context.T_PRDC_CONFIGURAR_PASADA
                .AsNoTracking()
                .Any(d => d.CD_PRDC_DEFINICION == formula
                    && d.CD_ACCION_INSTANCIA == accion);
        }

        public virtual bool AnyFormulaEntrada(string formula, string producto)
        {
            return _context.T_PRDC_DET_ENTRADA
                .AsNoTracking()
                .Any(d => d.CD_PRDC_DEFINICION == formula
                    && d.CD_PRODUTO == producto);
        }

        public virtual bool AnyFormulaSalida(string formula, string producto)
        {
            return _context.T_PRDC_DET_SALIDA
                .AsNoTracking()
                .Any(d => d.CD_PRDC_DEFINICION == formula
                    && d.CD_PRODUTO == producto);
        }

        #endregion

        #region Get

        public virtual Formula GetFormula(string cdFormula)
        {
            T_PRDC_DEFINICION formula = this._context.T_PRDC_DEFINICION
                .Include("T_PRDC_DET_ENTRADA")
                .Include("T_PRDC_DET_SALIDA")
                .Include("T_PRDC_CONFIGURAR_PASADA")
                .Where(d => d.CD_PRDC_DEFINICION == cdFormula)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapEntityToObject(formula);
        }

        public virtual List<Formula> GetFormulasActivasByNameOrCode(string searchValue, int userId)
        {
            List<Formula> formulasActivas = new List<Formula>();
            List<T_PRDC_DEFINICION> entities = this._context.T_PRDC_DEFINICION
                .Join(_context.T_EMPRESA_FUNCIONARIO.Where(e => e.USERID == userId),
                prdc => prdc.CD_EMPRESA,
                ef => ef.CD_EMPRESA,
                (prdc, ef) => prdc
                )
                .Where(d => d.CD_SITUACAO == SituacionDb.Activo
                    && (d.NM_PRDC_DEFINICION.ToLower().Contains(searchValue.ToLower())
                        || d.CD_PRDC_DEFINICION.ToLower().Contains(searchValue.ToLower())))
                .ToList();

            foreach (var entity in entities)
            {
                formulasActivas.Add(this._mapper.MapEntityToObject(entity));
            }

            return formulasActivas;
        }

        public virtual List<Formula> GetFormulasActivasEmpresaByNameOrCode(string searchValue, int userId, int empresa)
        {
            List<Formula> formulasActivas = new List<Formula>();
            List<T_PRDC_DEFINICION> entities = this._context.T_PRDC_DEFINICION
                .Where(d => d.CD_SITUACAO == SituacionDb.Activo
                    && d.CD_EMPRESA == empresa
                    && (d.NM_PRDC_DEFINICION.ToLower().Contains(searchValue.ToLower())
                        || d.CD_PRDC_DEFINICION.ToLower().Contains(searchValue.ToLower())))
                .ToList();

            foreach (var entity in entities)
            {
                formulasActivas.Add(this._mapper.MapEntityToObject(entity));
            }

            return formulasActivas;
        }

        #endregion

        #region Add

        public virtual void AddFormula(Formula formula)
        {
            T_PRDC_DEFINICION entity = this._mapper.MapObjectToEntity(formula);
            int componente = 1;

            foreach (var linea in entity.T_PRDC_DET_ENTRADA)
            {
                linea.CD_COMPONENTE = Convert.ToString(componente++);
            }

            this._context.T_PRDC_DEFINICION.Add(entity);
        }

        public virtual void AddFormulaLineaEntrada(Formula formula, FormulaEntrada entrada)
        {
            T_PRDC_DET_ENTRADA entity = _mapper.MapLineaEntrada(formula, entrada);

            _context.T_PRDC_DET_ENTRADA.Add(entity);
        }

        public virtual void AddFormulaLineaSalida(Formula formula, FormulaSalida salida)
        {
            T_PRDC_DET_SALIDA entity = _mapper.MapLineaSalida(formula, salida);

            _context.T_PRDC_DET_SALIDA.Add(entity);
        }

        public virtual void AddFormulaLineaConfiguracion(Formula formula, FormulaConfiguracion configuracion)
        {
            T_PRDC_CONFIGURAR_PASADA entity = _mapper.MapLineaConfiguracion(formula, configuracion);

            _context.T_PRDC_CONFIGURAR_PASADA.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateFormula(Formula formula, EntityChanges<FormulaEntrada> cambiosEntrada, EntityChanges<FormulaSalida> cambiosSalida)
        {
            UpdateFormula(formula);
            UpdateFormulaLineasEntrada(formula, cambiosEntrada);
            UpdateFormulaLineasSalida(formula, cambiosSalida);
        }

        public virtual void UpdateFormulaLineasEntrada(Formula formula, EntityChanges<FormulaEntrada> records)
        {
            foreach (var deletedRecord in records.DeletedRecords)
            {
                DeleteFormulaLineaEntrada(formula, deletedRecord);
            }

            int componente = 1;

            var ultimoDetalleComponente = _context.T_PRDC_DET_ENTRADA
                .Where(d => d.CD_PRDC_DEFINICION == formula.Id).OrderByDescending(d => d.CD_COMPONENTE)
                .FirstOrDefault();

            if (ultimoDetalleComponente != null)
            {
                componente = int.Parse(ultimoDetalleComponente.CD_COMPONENTE) + 1;
            }

            foreach (var newRecord in records.AddedRecords)
            {
                newRecord.Componente = Convert.ToString(componente);

                AddFormulaLineaEntrada(formula, newRecord);

                componente++;
            }

            foreach (var updatedRecord in records.UpdatedRecords)
            {
                UpdateFormulaLineaEntrada(formula, updatedRecord);
            }
        }

        public virtual void UpdateFormulaLineasSalida(Formula formula, EntityChanges<FormulaSalida> records)
        {
            foreach (var deletedRecord in records.DeletedRecords)
            {
                DeleteFormulaLineaSalida(formula, deletedRecord);
            }

            foreach (var newRecord in records.AddedRecords)
            {
                AddFormulaLineaSalida(formula, newRecord);
            }

            foreach (var updatedRecord in records.UpdatedRecords)
            {
                UpdateFormulaLineaSalida(formula, updatedRecord);
            }
        }

        public virtual void UpdateFormulaLineasConfiguracion(Formula formula, EntityChanges<FormulaConfiguracion> records)
        {
            foreach (var deletedRecord in records.DeletedRecords)
            {
                DeleteFormulaLineaConfiguracion(formula, deletedRecord);
            }

            foreach (var newRecord in records.AddedRecords)
            {
                AddFormulaLineaConfiguracion(formula, newRecord);
            }

            foreach (var updatedRecord in records.UpdatedRecords)
            {
                UpdateFormulaLineaConfiguracion(formula, updatedRecord);
            }
        }

        public virtual void UpdateFormula(Formula formula)
        {
            var entity = _mapper.MapObjectToEntity(formula);
            var attachedEntity = _context.T_PRDC_DEFINICION.Local
                .Where(d => d.CD_PRDC_DEFINICION == formula.Id)
                .FirstOrDefault();

            entity.DT_UPDROW = DateTime.Now;
            entity.T_PRDC_DET_ENTRADA.Clear();
            entity.T_PRDC_DET_SALIDA.Clear();
            entity.T_PRDC_CONFIGURAR_PASADA.Clear();

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRDC_DEFINICION.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFormulaLineaEntrada(Formula formula, FormulaEntrada entrada)
        {
            var entity = _mapper.MapLineaEntrada(formula, entrada);
            var attachedEntity = _context.T_PRDC_DET_ENTRADA.Local
                .Where(d => d.CD_PRDC_DEFINICION == formula.Id
                    && d.CD_COMPONENTE == entrada.Componente
                    && d.NU_PRIORIDAD == entrada.Prioridad)
                .FirstOrDefault();

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRDC_DET_ENTRADA.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFormulaLineaSalida(Formula formula, FormulaSalida salida)
        {
            var entity = _mapper.MapLineaSalida(formula, salida);
            var attachedEntity = _context.T_PRDC_DET_SALIDA.Local
                .Where(d => d.CD_PRDC_DEFINICION == formula.Id && d.CD_PRODUTO == salida.Producto)
                .FirstOrDefault();

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRDC_DET_SALIDA.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFormulaLineaConfiguracion(Formula formula, FormulaConfiguracion configuracion)
        {
            var entity = _mapper.MapLineaConfiguracion(formula, configuracion);
            var attachedEntity = _context.T_PRDC_CONFIGURAR_PASADA.Local
                .Where(d => d.CD_PRDC_DEFINICION == formula.Id && d.CD_ACCION_INSTANCIA == configuracion.Accion.Id)
                .FirstOrDefault();

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRDC_CONFIGURAR_PASADA.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteFormula(Formula formula)
        {
            foreach (var entrada in formula.Entrada)
            {
                DeleteFormulaLineaEntrada(formula, entrada);
            }

            foreach (var salida in formula.Salida)
            {
                DeleteFormulaLineaSalida(formula, salida);
            }

            foreach (var configuracion in formula.Configuracion)
            {
                DeleteFormulaLineaConfiguracion(formula, configuracion);
            }

            var entity = _mapper.MapObjectToEntity(formula);
            var attachedEntity = _context.T_PRDC_DEFINICION.Local
                .Where(d => d.CD_PRDC_DEFINICION == entity.CD_PRDC_DEFINICION)
                .FirstOrDefault();

            entity.T_PRDC_DET_ENTRADA.Clear();
            entity.T_PRDC_DET_SALIDA.Clear();
            entity.T_PRDC_CONFIGURAR_PASADA.Clear();

            if (attachedEntity != null)
            {
                _context.T_PRDC_DEFINICION.Remove(attachedEntity);
            }
            else
            {
                _context.T_PRDC_DEFINICION.Attach(entity);
                _context.T_PRDC_DEFINICION.Remove(entity);
            }
        }

        public virtual void DeleteFormulaLineaEntrada(Formula formula, FormulaEntrada entrada)
        {
            var entity = _mapper.MapLineaEntrada(formula, entrada);
            var attachedEntity = _context.T_PRDC_DET_ENTRADA.Local
                .Where(d => d.CD_PRDC_DEFINICION == formula.Id
                    && d.CD_COMPONENTE == entrada.Componente
                    && d.NU_PRIORIDAD == entrada.Prioridad)
                .FirstOrDefault();

            if (attachedEntity != null)
            {
                _context.T_PRDC_DET_ENTRADA.Remove(attachedEntity);
            }
            else
            {
                _context.T_PRDC_DET_ENTRADA.Attach(entity);
                _context.T_PRDC_DET_ENTRADA.Remove(entity);
            }
        }

        public virtual void DeleteFormulaLineaSalida(Formula formula, FormulaSalida salida)
        {
            var entity = _mapper.MapLineaSalida(formula, salida);
            var attachedEntity = _context.T_PRDC_DET_SALIDA.Local
                .Where(d => d.CD_PRDC_DEFINICION == formula.Id && d.CD_PRODUTO == salida.Producto)
                .FirstOrDefault();

            if (attachedEntity != null)
            {
                _context.T_PRDC_DET_SALIDA.Remove(attachedEntity);
            }
            else
            {
                _context.T_PRDC_DET_SALIDA.Attach(entity);
                _context.T_PRDC_DET_SALIDA.Remove(entity);
            }
        }

        public virtual void DeleteFormulaLineaConfiguracion(Formula formula, FormulaConfiguracion configuracion)
        {
            var entity = _mapper.MapLineaConfiguracion(formula, configuracion);
            var attachedEntity = _context.T_PRDC_CONFIGURAR_PASADA.Local
                .Where(d => d.CD_PRDC_DEFINICION == formula.Id && d.CD_ACCION_INSTANCIA == configuracion.Accion.Id)
                .FirstOrDefault();

            if (attachedEntity != null)
            {
                _context.T_PRDC_CONFIGURAR_PASADA.Remove(attachedEntity);
            }
            else
            {
                _context.T_PRDC_CONFIGURAR_PASADA.Attach(entity);
                _context.T_PRDC_CONFIGURAR_PASADA.Remove(entity);
            }

        }

        #endregion

        #region Dapper

        public virtual IEnumerable<Formula> GetFormulas(IEnumerable<Formula> formulas)
        {
            IEnumerable<Formula> resultado = new List<Formula>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRDC_INGRESO_TEMP (CD_PRDC_DEFINICION) VALUES (:Id)";
                    _dapper.Execute(connection, sql, formulas, transaction: tran);

                    sql = @"SELECT 
								PD.CD_PRDC_DEFINICION as Id,
                                PD.NM_PRDC_DEFINICION as Nombre,
                                PD.DS_PRDC_DEFINICION as Descripcion,
                                PD.CD_EMPRESA as Empresa,
                                PD.CD_SITUACAO as Estado,
                                PD.DT_ADDROW as FechaAlta,
                                PD.DT_UPDROW as FechaModificacion,
                                PD.TP_PRDC_DEFINICION as Tipo,
                                PD.QT_PASADAS_POR_FORMULA as CantidadPasadasPorFormula
                        FROM T_PRDC_DEFINICION PD 
                        INNER JOIN T_PRDC_INGRESO_TEMP T ON PD.CD_PRDC_DEFINICION = T.CD_PRDC_DEFINICION";

                    resultado = _dapper.Query<Formula>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<FormulaEntrada> GetDetallesEntradaFormula(IEnumerable<Formula> formulas)
        {
            IEnumerable<FormulaEntrada> resultado = new List<FormulaEntrada>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRDC_INGRESO_TEMP (CD_PRDC_DEFINICION) VALUES (:Id)";
                    _dapper.Execute(connection, sql, formulas, transaction: tran);

                    sql = @"SELECT 
							    PDE.CD_PRDC_DEFINICION as IdFormula,
                                PDE.CD_COMPONENTE as Componente,
                                PDE.NU_PRIORIDAD as Prioridad,
                                PDE.CD_EMPRESA as Empresa,
                                PDE.CD_PRODUTO as Producto,
                                PDE.CD_FAIXA as Faixa,
                                PDE.QT_COMPLETA as CantidadCompleta,
                                PDE.QT_INCOMPLETA as CantidadIncompleta,
                                PDE.QT_PASADA_LINEA as CantidadPasadas,
                                PDE.QT_CONSUMIDA_LINEA as CantidadConsumir,
                                PDE.CD_EMPRESA_PEDIDO as EmpresaPedido,
                                PDE.DT_ADDROW as FechaAlta,
                                PDE.DT_UPDROW as FechaModificacion
                            FROM T_PRDC_DET_ENTRADA PDE 
                            INNER JOIN T_PRDC_INGRESO_TEMP T ON PDE.CD_PRDC_DEFINICION = T.CD_PRDC_DEFINICION";

                    resultado = _dapper.Query<FormulaEntrada>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<FormulaSalida> GetDetallesSalidaFormula(IEnumerable<Formula> formulas)
        {
            IEnumerable<FormulaSalida> resultado = new List<FormulaSalida>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRDC_INGRESO_TEMP (CD_PRDC_DEFINICION) VALUES (:Id)";
                    _dapper.Execute(connection, sql, formulas, transaction: tran);

                    sql = @"SELECT 
							    PDS.CD_PRDC_DEFINICION as IdFormula,
                                PDS.CD_EMPRESA as Empresa,
                                PDS.CD_PRODUTO as Producto,
                                PDS.CD_FAIXA as Faixa,
                                PDS.QT_COMPLETA as CantidadCompleta,
                                PDS.QT_INCOMPLETA as CantidadIncompleta,
                                PDS.QT_PASADA_LINEA as CantidadPasadas,
                                PDS.QT_CONSUMIDA_LINEA as CantidadProducir,
                                PDS.ID_PRODUTO_FINAL as IdProductoFinal,
                                PDS.DT_ADDROW as FechaAlta,
                                PDS.DT_UPDROW as FechaModificacion
                            FROM T_PRDC_DET_SALIDA PDS
                            INNER JOIN T_PRDC_INGRESO_TEMP T ON PDS.CD_PRDC_DEFINICION = T.CD_PRDC_DEFINICION";

                    resultado = _dapper.Query<FormulaSalida>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        #endregion
    }
}
